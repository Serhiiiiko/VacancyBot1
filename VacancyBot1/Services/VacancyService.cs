﻿using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using VacancyBot1.Data;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace VacancyBot1.Services;

public class VacancyService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ApplicationDbContext _dbContext;

    public VacancyService(ITelegramBotClient botClient, ApplicationDbContext dbContext)
    {
        _botClient = botClient;
        _dbContext = dbContext;
    }

    public async Task ShowVacanciesAsync(long chatId)
    {
        var vacancies = _dbContext.Vacancies.ToList();

        if (!vacancies.Any())
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Наразі немає доступних вакансій."
            );
            return;
        }

        var buttons = vacancies.Select(v =>
            new[] { InlineKeyboardButton.WithCallbackData(v.Title, $"vacancy_{v.Id}") }
        ).ToArray();

        var keyboard = new InlineKeyboardMarkup(buttons);

        await _botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Доступні вакансії:",
            replyMarkup: keyboard
        );
    }

    public async Task ShowVacancyDetailsAsync(long chatId, int vacancyId)
    {
        var vacancy = _dbContext.Vacancies.Find(vacancyId);

        if (vacancy == null)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Вакансію не знайдено."
            );
            return;
        }

        var applyButton = InlineKeyboardButton.WithCallbackData("Подати заявку", $"apply_{vacancy.Id}");
        var backButton = InlineKeyboardButton.WithCallbackData("⬅️ Назад до вакансій", "back_to_vacancies");
        var mainMenuButton = InlineKeyboardButton.WithCallbackData("🏠 Головне меню", "back_to_main");

        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[] { applyButton },
            new[] { backButton }
        });


        if (!string.IsNullOrEmpty(vacancy.ImagePath) && System.IO.File.Exists(vacancy.ImagePath))
        {
            using (var stream = System.IO.File.OpenRead(vacancy.ImagePath))
            {
                var inputFiles = new InputFileStream(stream, Path.GetFileName(vacancy.ImagePath));

                await _botClient.SendPhotoAsync(
                    chatId: chatId,
                    photo: inputFiles,
                    caption: $"<b>{vacancy.Title}</b>\n\nОпис вакансії:\n{vacancy.Description}\n\nВимоги:\n{vacancy.Requirements}",
                    parseMode: ParseMode.Html,
                    replyMarkup: keyboard
                );
            }
        }
        else
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"<b>{vacancy.Title}</b>\n\nОпис вакансії:\n{vacancy.Description}\n\nВимоги:\n{vacancy.Requirements}",
                parseMode: ParseMode.Html,
                replyMarkup: keyboard
            );
        }
    }
}
