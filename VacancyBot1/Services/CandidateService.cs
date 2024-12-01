using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegram.Bot;
using VacancyBot1.Data;
using VacancyBot1.Models;

namespace VacancyBot1.Services;

public class CandidateService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ApplicationDbContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly AdminService _adminService;

    private readonly ConcurrentDictionary<long, CandidateState> _candidateStates = new ConcurrentDictionary<long, CandidateState>();

    public CandidateService(ITelegramBotClient botClient, ApplicationDbContext dbContext, IEmailService emailService, IConfiguration configuration, AdminService adminService)
    {
        _botClient = botClient;
        _dbContext = dbContext;
        _emailService = emailService;
        _configuration = configuration;
        _adminService = adminService;
    }

    public async Task StartApplicationAsync(User user, int vacancyId)
    {
        var state = new CandidateState
        {
            VacancyId = vacancyId,
            Step = ApplicationStep.FullName
        };

        _candidateStates[user.Id] = state;

        await _botClient.SendTextMessageAsync(
            chatId: user.Id,
            text: "Введіть ваше повне ім'я:"
        );
    }

    public async Task HandleApplicationAsync(Message message)
    {
        if (!_candidateStates.TryGetValue(message.From.Id, out var state))
            return;

        switch (state.Step)
        {
            case ApplicationStep.FullName:
                state.FullName = message.Text;
                state.Step = ApplicationStep.PhoneNumber;
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Введіть ваш номер телефону (у форматі +380XXXXXXXXX):"
                );
                break;

            case ApplicationStep.PhoneNumber:
                if (IsValidPhoneNumber(message.Text))
                {
                    state.PhoneNumber = message.Text;
                    state.Step = ApplicationStep.WorkExperience;
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Опишіть ваш досвід роботи:"
                    );
                }
                else
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Некоректний формат номера телефону. Спробуйте ще раз:"
                    );
                }
                break;

            case ApplicationStep.WorkExperience:
                state.WorkExperience = message.Text;
                state.Step = ApplicationStep.Email;
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Введіть вашу електронну пошту (необов'язково). Якщо не бажаєте вказувати, напишіть 'ні':"
                );
                break;

            case ApplicationStep.Email:
                if (!string.Equals(message.Text.Trim(), "ні", StringComparison.OrdinalIgnoreCase))
                {
                    state.Email = message.Text;
                }
                state.Step = ApplicationStep.CVFile;
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Надішліть ваше резюме у форматі PDF або зображення (або введіть skip, щоб пропустити):"
                );
                break;

            case ApplicationStep.CVFile:
                if (message.Document != null || message.Photo != null)
                {
                    string filePath = null;
                    var directory = Path.Combine("wwwroot", "CandidateFiles");
                    Directory.CreateDirectory(directory);

                    if (message.Document != null)
                    {
                        var fileId = message.Document.FileId;
                        var file = await _botClient.GetFileAsync(fileId);
                        var extension = Path.GetExtension(message.Document.FileName);
                        var fileName = $"{Guid.NewGuid()}{extension}";
                        filePath = Path.Combine(directory, fileName);

                        using (var stream = System.IO.File.OpenWrite(filePath))
                        {
                            await _botClient.DownloadFileAsync(file.FilePath, stream);
                        }
                    }
                    else if (message.Photo != null)
                    {
                        var photo = message.Photo.OrderByDescending(p => p.FileSize).FirstOrDefault();
                        var fileId = photo.FileId;
                        var file = await _botClient.GetFileAsync(fileId);
                        var fileName = $"{Guid.NewGuid()}.jpg";
                        filePath = Path.Combine(directory, fileName);

                        using (var stream = System.IO.File.OpenWrite(filePath))
                        {
                            await _botClient.DownloadFileAsync(file.FilePath, stream);
                        }
                    }

                    state.CVFilePath = filePath;
                }
                else if (message.Text?.ToLower() == "skip")
                {
                    state.CVFilePath = null;
                }
                else
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Надішліть резюме або введіть skip, щоб пропустити цей крок."
                    );
                    return;
                }

                var candidate = new Candidate
                {
                    TelegramId = message.From.Id,
                    TelegramUsername = message.From.Username,
                    FullName = state.FullName,
                    PhoneNumber = state.PhoneNumber,
                    WorkExperience = state.WorkExperience,
                    Email = state.Email,
                    CVFilePath = state.CVFilePath,
                    VacancyId = state.VacancyId
                };

                _dbContext.Candidates.Add(candidate);
                await _dbContext.SaveChangesAsync();

                _candidateStates.TryRemove(message.From.Id, out _);

                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Ваша заявка успішно надіслана!"
                );

                await NotifyAdminsAsync(candidate);

                break;

        }
    }

    private async Task NotifyAdminsAsync(Candidate candidate)
    {
        await _dbContext.Entry(candidate).Reference(c => c.Vacancy).LoadAsync();

        var admins = _dbContext.Admins.ToList();

        string candidateInfo = $"Новий кандидат на вакансію: {candidate.Vacancy.Title}\n" +
                               $"Ім'я: {candidate.FullName}\n" +
                               $"Телефон: {candidate.PhoneNumber}\n" +
                               $"Email: {candidate.Email ?? "N/A"}\n" +
                               $"Досвід роботи: {candidate.WorkExperience}\n" +
                               $"Telegram: @{candidate.TelegramUsername ?? "N/A"}\n";

        foreach (var admin in admins)
        {
            await _botClient.SendTextMessageAsync(
                chatId: admin.TelegramId,
                text: candidateInfo
            );

            if (!string.IsNullOrEmpty(candidate.CVFilePath) && System.IO.File.Exists(candidate.CVFilePath))
            {
                try
                {
                    var extension = Path.GetExtension(candidate.CVFilePath).ToLower();

                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                    {
                        await _botClient.SendPhotoAsync(
                            chatId: admin.TelegramId,
                            photo: new InputFileStream(System.IO.File.OpenRead(candidate.CVFilePath)),
                            caption: "Резюме кандидата"
                        );
                    }
                    else
                    {
                        await _botClient.SendDocumentAsync(
                            chatId: admin.TelegramId,
                            document: new InputFileStream(System.IO.File.OpenRead(candidate.CVFilePath)),
                            caption: "Резюме кандидата"
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при отправке резюме в Telegram: {ex.Message}");
                    await _botClient.SendTextMessageAsync(
                        chatId: admin.TelegramId,
                        text: "Не вдалося відправити резюме кандидата."
                    );
                }
            }
            else
            {
                await _botClient.SendTextMessageAsync(
                    chatId: admin.TelegramId,
                    text: "Кандидат не надав резюме."
                );
            }
        }

        foreach (var admin in admins.Where(a => !string.IsNullOrEmpty(a.Email)))
        {
            var email = new Email
            {
                To = admin.Email,
                Subject = "Новий кандидат на вакансію",
                Body = candidateInfo,
                AttachmentPath = candidate.CVFilePath
            };

            try
            {
                await _emailService.SendEmailAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке email: {ex.Message}");
            }
        }
    }

    public bool IsValidPhoneNumber(string phoneNumber)
    {
        var regex = new Regex(@"^\+380\d{9}$");
        return regex.IsMatch(phoneNumber);
    }

    private class CandidateState
    {
        public int VacancyId { get; set; }
        public ApplicationStep Step { get; set; }
        public string FullName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string WorkExperience { get; set; } = default!;
        public string? Email { get; set; }
        public string? CVFilePath { get; set; }

    }

    private enum ApplicationStep
    {
        FullName,
        PhoneNumber,
        WorkExperience,
        Email,
        CVFile
    }

}