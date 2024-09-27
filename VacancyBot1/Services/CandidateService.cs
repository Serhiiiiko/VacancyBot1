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

    // State management
    private readonly ConcurrentDictionary<long, CandidateState> _candidateStates = new ConcurrentDictionary<long, CandidateState>();

    public CandidateService(ITelegramBotClient botClient, ApplicationDbContext dbContext)
    {
        _botClient = botClient;
        _dbContext = dbContext;
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

                var candidate = new Candidate
                {
                    TelegramId = message.From.Id,
                    TelegramUsername = message.From.Username,
                    FullName = state.FullName,
                    PhoneNumber = state.PhoneNumber,
                    WorkExperience = state.WorkExperience,
                    VacancyId = state.VacancyId
                };

                _dbContext.Candidates.Add(candidate);
                await _dbContext.SaveChangesAsync();

                _candidateStates.TryRemove(message.From.Id, out _);

                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Ваша заявка успішно надіслана!"
                );

                break;
        }
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        var regex = new Regex(@"^\+380\d{9}$");
        return regex.IsMatch(phoneNumber);
    }

    private class CandidateState
    {
        public int VacancyId { get; set; }
        public ApplicationStep Step { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string WorkExperience { get; set; }
    }

    private enum ApplicationStep
    {
        FullName,
        PhoneNumber,
        WorkExperience
    }
}