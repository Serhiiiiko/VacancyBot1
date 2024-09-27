using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using VacancyBot1.Data;


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
            InlineKeyboardButton.WithCallbackData(v.Title, $"vacancy_{v.Id}")
        );

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
        var keyboard = new InlineKeyboardMarkup(applyButton);

        if (vacancy.Image != null)
        {
            //using (var stream = new System.IO.MemoryStream(vacancy.Image))
            //{
            //    var inputFile = new Telegram.Bot.Types.InputFiles.InputFile(stream, "vacancy.jpg");
            //    await _botClient.SendPhotoAsync(
            //        chatId: chatId,
            //        photo: inputFile,
            //        caption: $"<b>{vacancy.Title}</b>\n\n{vacancy.Description}\n\nВимоги:\n{vacancy.Requirements}",
            //        parseMode: ParseMode.Html,
            //        replyMarkup: keyboard
            //    );
            //}

        }
        else
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"<b>{vacancy.Title}</b>\n\n{vacancy.Description}\n\nВимоги:\n{vacancy.Requirements}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: keyboard
            );
        }
    }
}
