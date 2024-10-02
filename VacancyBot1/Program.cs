using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;
using VacancyBot1.Data;
using VacancyBot1.Handlers;
using VacancyBot1.Services;

class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection, configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        serviceProvider.GetService<Bot>().RunAsync().GetAwaiter().GetResult();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite("Data Source=vacancybot.db"));

        services.AddSingleton<ITelegramBotClient>(new TelegramBotClient("7386705040:AAHQN9Xsa1n6Vq-w58dDRbkMntdpFhIqp6M"));

        services.AddTransient<VacancyService>();
        services.AddTransient<CandidateService>();
        services.AddTransient<AdminService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddTransient<IUpdateHandler, UpdateHandler>();

        services.AddSingleton<Bot>();
    }
}

public class Bot
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateHandler _updateHandler;

    public Bot(ITelegramBotClient botClient, IUpdateHandler updateHandler)
    {
        _botClient = botClient;
        _updateHandler = updateHandler;
    }

    public async Task RunAsync()
    {
        var cts = new CancellationTokenSource();

        _botClient.StartReceiving(
            _updateHandler,
            receiverOptions: null,
            cancellationToken: cts.Token
        );

        Console.WriteLine("Bot is running...");
        await Task.Delay(-1, cts.Token);
    }
}