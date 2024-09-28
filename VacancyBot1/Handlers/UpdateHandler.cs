using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using VacancyBot1.Services;
using Telegram.Bot.Polling;


namespace VacancyBot1.Handlers
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly VacancyService _vacancyService;
        private readonly CandidateService _candidateService;
        private readonly AdminService _adminService;

        public UpdateHandler(ITelegramBotClient botClient,
                             VacancyService vacancyService,
                             CandidateService candidateService,
                             AdminService adminService)
        {
            _botClient = botClient;
            _vacancyService = vacancyService;
            _candidateService = candidateService;
            _adminService = adminService;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.Type == UpdateType.Message)
                {
                    await HandleMessageAsync(update.Message);
                }
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    await HandleCallbackQueryAsync(update.CallbackQuery);
                }
            }
            catch (Exception ex)
            {
                await HandlePollingErrorAsync(botClient, ex, cancellationToken);
            }
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error: {exception.Message}");
            return Task.CompletedTask;
        }

        private async Task HandleMessageAsync(Message message)
        {
            if (message.Text != null && message.Text.StartsWith("/"))
            {
                await HandleCommandAsync(message);
            }
            else
            {
<<<<<<< HEAD
=======
                // Check if the user is an admin in the middle of a command
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161
                if (_adminService.IsAdmin(message.From.Id))
                {
                    await _adminService.HandleAdminInputAsync(message);
                }
                else
                {
<<<<<<< HEAD
=======
                    // Check if the user is in the middle of an application
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161
                    await _candidateService.HandleApplicationAsync(message);
                }
            }
        }

        private async Task HandleCommandAsync(Message message)
        {
            switch (message.Text.Split(' ')[0])
            {
                case "/start":
                    await _vacancyService.ShowVacanciesAsync(message.Chat.Id);
                    break;
                case "/addvacancy":
                    await _adminService.AddVacancyAsync(message);
                    break;
                case "/editvacancy":
                    await _adminService.EditVacancyAsync(message);
                    break;
                case "/deletevacancy":
                    await _adminService.DeleteVacancyAsync(message);
                    break;
                case "/viewcandidates":
                    await _adminService.ViewCandidatesAsync(message);
                    break;
                default:
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Невідома команда."
                    );
                    break;
            }
        }

        private async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery)
        {
            if (_adminService.IsAdmin(callbackQuery.From.Id))
            {
                await _adminService.HandleAdminCallbackQueryAsync(callbackQuery);
                return;
            }

            if (callbackQuery.Data.StartsWith("vacancy_"))
            {
                int vacancyId = int.Parse(callbackQuery.Data.Split('_')[1]);
                await _vacancyService.ShowVacancyDetailsAsync(callbackQuery.Message.Chat.Id, vacancyId);
            }
            else if (callbackQuery.Data.StartsWith("apply_"))
            {
                int vacancyId = int.Parse(callbackQuery.Data.Split('_')[1]);
                await _candidateService.StartApplicationAsync(callbackQuery.From, vacancyId);
            }
        }
    }
}
