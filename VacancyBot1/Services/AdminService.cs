using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using VacancyBot1.Data;
using VacancyBot1.Models;
using System.Collections.Concurrent;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using System;

namespace VacancyBot1.Services
{
    public class AdminService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ApplicationDbContext _dbContext;

<<<<<<< HEAD
=======
        // State management for admin commands
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161
        private readonly ConcurrentDictionary<long, AdminState> _adminStates = new ConcurrentDictionary<long, AdminState>();

        public AdminService(ITelegramBotClient botClient, ApplicationDbContext dbContext)
        {
            _botClient = botClient;
            _dbContext = dbContext;
        }

        public bool IsAdmin(long userId)
        {
            return _dbContext.Admins.Any(a => a.TelegramId == userId);
        }

        public async Task AddVacancyAsync(Message message)
        {
<<<<<<< HEAD
            if (!IsAdmin(message.Chat.Id))
=======
            if (IsAdmin(message.Chat.Id))
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161
            {
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "У вас немає прав адміністратора."
                );
                return;
            }

            var state = new AdminState
            {
                Command = AdminCommand.AddVacancy,
                Step = AdminStep.Title
            };

            _adminStates[message.From.Id] = state;

            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Введіть назву вакансії:"
            );
        }

        public async Task EditVacancyAsync(Message message)
        {
            if (!IsAdmin(message.From.Id))
            {
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "У вас немає прав адміністратора."
                );
                return;
            }

<<<<<<< HEAD
=======
            // Show list of vacancies to edit
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161
            var vacancies = _dbContext.Vacancies.ToList();

            if (!vacancies.Any())
            {
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Наразі немає доступних вакансій для редагування."
                );
                return;
            }

            var buttons = vacancies.Select(v =>
                InlineKeyboardButton.WithCallbackData(v.Title, $"editvacancy_{v.Id}")
            );

            var keyboard = new InlineKeyboardMarkup(buttons);

            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Оберіть вакансію для редагування:",
                replyMarkup: keyboard
            );
        }

        public async Task DeleteVacancyAsync(Message message)
        {
            if (!IsAdmin(message.From.Id))
            {
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "У вас немає прав адміністратора."
                );
                return;
            }

<<<<<<< HEAD
=======
            // Show list of vacancies to delete
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161
            var vacancies = _dbContext.Vacancies.ToList();

            if (!vacancies.Any())
            {
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Наразі немає доступних вакансій для видалення."
                );
                return;
            }

            var buttons = vacancies.Select(v =>
                InlineKeyboardButton.WithCallbackData(v.Title, $"deletevacancy_{v.Id}")
            );

            var keyboard = new InlineKeyboardMarkup(buttons);

            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Оберіть вакансію для видалення:",
                replyMarkup: keyboard
            );
        }

<<<<<<< HEAD
        internal async Task ViewCandidatesAsync(Message message)
        {
            throw new NotImplementedException();
        }

=======
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161
        public async Task HandleAdminInputAsync(Message message)
        {
            if (!_adminStates.TryGetValue(message.From.Id, out var state))
                return;

            switch (state.Command)
            {
                case AdminCommand.AddVacancy:
                    await HandleAddVacancyAsync(message, state);
                    break;
                case AdminCommand.EditVacancy:
                    await HandleEditVacancyAsync(message, state);
                    break;
<<<<<<< HEAD
=======
                    // Handle other commands if necessary
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161
            }
        }

        public async Task HandleAdminCallbackQueryAsync(CallbackQuery callbackQuery)
        {
            if (!_adminStates.TryGetValue(callbackQuery.From.Id, out var state))
            {
                state = new AdminState();
            }

            if (callbackQuery.Data.StartsWith("editvacancy_"))
            {
                int vacancyId = int.Parse(callbackQuery.Data.Split('_')[1]);

                state.Command = AdminCommand.EditVacancy;
                state.Step = AdminStep.Title;
                state.VacancyId = vacancyId;
                _adminStates[callbackQuery.From.Id] = state;

                await _botClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: "Введіть нову назву вакансії:"
                );
            }
            else if (callbackQuery.Data.StartsWith("deletevacancy_"))
            {
                int vacancyId = int.Parse(callbackQuery.Data.Split('_')[1]);

<<<<<<< HEAD
=======
                // Delete the vacancy
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161
                var vacancy = _dbContext.Vacancies.Find(vacancyId);
                if (vacancy != null)
                {
                    _dbContext.Vacancies.Remove(vacancy);
                    await _dbContext.SaveChangesAsync();

                    await _botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Вакансію видалено."
                    );
                }
                else
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Вакансію не знайдено."
                    );
                }
            }
        }

        private async Task HandleAddVacancyAsync(Message message, AdminState state)
        {
            switch (state.Step)
            {
                case AdminStep.Title:
                    state.Title = message.Text;
                    state.Step = AdminStep.Description;
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Введіть опис вакансії:"
                    );
                    break;
                case AdminStep.Description:
                    state.Description = message.Text;
                    state.Step = AdminStep.Requirements;
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Введіть вимоги до посади:"
                    );
                    break;
                case AdminStep.Requirements:
                    state.Requirements = message.Text;
                    state.Step = AdminStep.Image;
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
<<<<<<< HEAD
                        text: "Надішліть зображення вакансії (або надішліть skip, щоб пропустити):"
=======
                        text: "Надішліть зображення вакансії (або надішліть /skip, щоб пропустити):"
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161
                    );
                    break;
                case AdminStep.Image:
                    if (message.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
                    {
                        // Get the highest resolution photo
                        var fileId = message.Photo.Last().FileId;
                        var file = await _botClient.GetFileAsync(fileId);
                        using (var stream = new System.IO.MemoryStream())
                        {
                            await _botClient.DownloadFileAsync(file.FilePath, stream);
                            state.Image = stream.ToArray();
                        }
                    }
<<<<<<< HEAD
                    else if (message.Text == "skip")
=======
                    else if (message.Text == "/skip")
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161
                    {
                        state.Image = null;
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
<<<<<<< HEAD
                            text: "Надішліть зображення або введіть skip, щоб пропустити цей крок."
=======
                            text: "Надішліть зображення або введіть /skip, щоб пропустити цей крок."
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161
                        );
                        return;
                    }

<<<<<<< HEAD
=======
                    // Save vacancy to database
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161
                    var vacancy = new Vacancy
                    {
                        Title = state.Title,
                        Description = state.Description,
                        Requirements = state.Requirements,
                        Image = state.Image
                    };

                    _dbContext.Vacancies.Add(vacancy);
                    await _dbContext.SaveChangesAsync();

                    _adminStates.TryRemove(message.From.Id, out _);

                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Вакансію успішно додано."
                    );
                    break;
            }
        }

        private async Task HandleEditVacancyAsync(Message message, AdminState state)
        {
            var vacancy = _dbContext.Vacancies.Find(state.VacancyId);
            if (vacancy == null)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Вакансію не знайдено."
                );
                _adminStates.TryRemove(message.From.Id, out _);
                return;
            }

            switch (state.Step)
            {
                case AdminStep.Title:
                    vacancy.Title = message.Text;
                    state.Step = AdminStep.Description;
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Введіть новий опис вакансії:"
                    );
                    break;
                case AdminStep.Description:
                    vacancy.Description = message.Text;
                    state.Step = AdminStep.Requirements;
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Введіть нові вимоги до посади:"
                    );
                    break;
                case AdminStep.Requirements:
                    vacancy.Requirements = message.Text;
                    state.Step = AdminStep.Image;
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Надішліть нове зображення вакансії (або надішліть /skip, щоб пропустити):"
                    );
                    break;
                case AdminStep.Image:
                    if (message.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
                    {
                        var fileId = message.Photo.Last().FileId;
                        var file = await _botClient.GetFileAsync(fileId);
                        using (var stream = new System.IO.MemoryStream())
                        {
                            await _botClient.DownloadFileAsync(file.FilePath, stream);
                            vacancy.Image = stream.ToArray();
                        }
                    }
                    else if (message.Text == "/skip")
                    {
                        // Keep existing image
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Надішліть зображення або введіть /skip, щоб пропустити цей крок."
                        );
                        return;
                    }

                    await _dbContext.SaveChangesAsync();

                    _adminStates.TryRemove(message.From.Id, out _);

                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Вакансію успішно оновлено."
                    );
                    break;
            }
        }

<<<<<<< HEAD
=======
        internal async Task ViewCandidatesAsync(Message message)
        {
            throw new NotImplementedException();
        }

>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161
        private class AdminState
        {
            public AdminCommand Command { get; set; }
            public AdminStep Step { get; set; }
            public int VacancyId { get; set; }
<<<<<<< HEAD
            public string Title { get; set; } = default!;
            public string Description { get; set; } = default!;
            public string Requirements { get; set; } = default!;
=======
            public string Title { get; set; }
            public string Description { get; set; }
            public string Requirements { get; set; }
>>>>>>> 507a0380ee99a877d10d8417417469b5c62df161
            public byte[] Image { get; set; }
        }

        private enum AdminCommand
        {
            AddVacancy,
            EditVacancy,
            DeleteVacancy
        }

        private enum AdminStep
        {
            Title,
            Description,
            Requirements,
            Image
        }
    }
}
