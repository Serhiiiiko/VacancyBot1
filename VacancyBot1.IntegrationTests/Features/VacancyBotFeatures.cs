using LightBDD.XUnit2;
using Microsoft.EntityFrameworkCore;
using Moq;
using VacancyBot1.Data;
using VacancyBot1.Models;
using VacancyBot1.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using VacancyBot1.Handlers;

namespace VacancyBot1.IntegrationTests.Features
{
    public partial class VacancyBotFeatures
    {
        [Scenario]
        private async Task When_user_selects_view_vacancies()
        {
            var mockBotClient = new Mock<ITelegramBotClient>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_VacancyBot")
                .Options;
            using var context = new ApplicationDbContext(options);

            var user = new User
            {
                Id = 555555,
                Username = "candidateuser"
            };
            var chat = new Chat
            {
                Id = 555555
            };
            var message = new Message
            {
                Text = "Переглянути вакансії",
                From = user,
                Chat = chat
            };
            var update = new Update
            {
                Message = message
            };

            var adminService = new AdminService(mockBotClient.Object, context);
            var candidateService = new CandidateService(mockBotClient.Object, context, null, null, adminService);
            var vacancyService = new VacancyService(mockBotClient.Object, context);

            var updateHandler = new UpdateHandler(mockBotClient.Object, vacancyService, candidateService, adminService);
            await updateHandler.HandleUpdateAsync(mockBotClient.Object, update, System.Threading.CancellationToken.None);
        }

        [Scenario]
        private async Task Given_user_has_received_the_list_of_vacancies()
        {
            var mockBotClient = new Mock<ITelegramBotClient>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_VacancyBot")
                .Options;
            using var context = new ApplicationDbContext(options);

            var admin = new Admin
            {
                Id = 1,
                TelegramId = 123456789,
                Email = "admin@example.com",
                TelegramUsername = "adminuser"
            };
            context.Admins.Add(admin);

            var vacancy1 = new Vacancy
            {
                Id = 1,
                Title = "Software Engineer",
                Description = "Develop software applications.",
                Requirements = "C#, .NET Core"
            };
            var vacancy2 = new Vacancy
            {
                Id = 2,
                Title = "QA Engineer",
                Description = "Test software applications.",
                Requirements = "Testing skills, attention to detail"
            };
            context.Vacancies.AddRange(vacancy1, vacancy2);
            context.SaveChanges();

            var user = new User
            {
                Id = 555555,
                Username = "candidateuser"
            };
            var chat = new Chat
            {
                Id = 555555
            };
            var messageApply = new Message
            {
                Text = "/apply",
                From = user,
                Chat = chat
            };
            var updateApply = new Update
            {
                Message = messageApply
            };

            var adminService = new AdminService(mockBotClient.Object, context);
            var candidateService = new CandidateService(mockBotClient.Object, context, null, null, adminService);
            var vacancyService = new VacancyService(mockBotClient.Object, context);

            var updateHandler = new UpdateHandler(mockBotClient.Object, vacancyService, candidateService, adminService);
            await updateHandler.HandleUpdateAsync(mockBotClient.Object, updateApply, System.Threading.CancellationToken.None);

            var messageName = new Message
            {
                Text = "Тестовий Кандидат",
                From = user,
                Chat = chat
            };
            var messagePhone = new Message
            {
                Text = "+380501234567",
                From = user,
                Chat = chat
            };
            var messageExperience = new Message
            {
                Text = "2 роки в IT",
                From = user,
                Chat = chat
            };
            var messageEmail = new Message
            {
                Text = "candidate@example.com",
                From = user,
                Chat = chat
            };
            var messageSkip = new Message
            {
                Text = "skip",
                From = user,
                Chat = chat
            };

            var updateName = new Update { Message = messageName };
            var updatePhone = new Update { Message = messagePhone };
            var updateExperience = new Update { Message = messageExperience };
            var updateEmail = new Update { Message = messageEmail };
            var updateSkip = new Update { Message = messageSkip };

            await updateHandler.HandleUpdateAsync(mockBotClient.Object, updateName, System.Threading.CancellationToken.None);
            await updateHandler.HandleUpdateAsync(mockBotClient.Object, updatePhone, System.Threading.CancellationToken.None);
            await updateHandler.HandleUpdateAsync(mockBotClient.Object, updateExperience, System.Threading.CancellationToken.None);
            await updateHandler.HandleUpdateAsync(mockBotClient.Object, updateEmail, System.Threading.CancellationToken.None);
            await updateHandler.HandleUpdateAsync(mockBotClient.Object, updateSkip, System.Threading.CancellationToken.None);
        }

        [Scenario]
        private async Task When_user_selects_a_vacancy()
        {
            var mockBotClient = new Mock<ITelegramBotClient>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_VacancyBot")
                .Options;
            using var context = new ApplicationDbContext(options);

            var user = new User
            {
                Id = 555555,
                Username = "candidateuser"
            };
            var chat = new Chat
            {
                Id = 555555
            };
            var message = new Message
            {
                Text = "Software Engineer",
                From = user,
                Chat = chat
            };
            var update = new Update
            {
                Message = message
            };

            var adminService = new AdminService(mockBotClient.Object, context);
            var candidateService = new CandidateService(mockBotClient.Object, context, null, null, adminService);
            var vacancyService = new VacancyService(mockBotClient.Object, context);

            var updateHandler = new UpdateHandler(mockBotClient.Object, vacancyService, candidateService, adminService);
            await updateHandler.HandleUpdateAsync(mockBotClient.Object, update, System.Threading.CancellationToken.None);
        }

        [Scenario]
        private async Task And_user_chooses_to_apply()
        {
            var mockBotClient = new Mock<ITelegramBotClient>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_VacancyBot")
                .Options;
            using var context = new ApplicationDbContext(options);

            var user = new User
            {
                Id = 555555,
                Username = "candidateuser"
            };
            var chat = new Chat
            {
                Id = 555555
            };
            var message = new Message
            {
                Text = "Подати заявку",
                From = user,
                Chat = chat
            };
            var update = new Update
            {
                Message = message
            };

            var adminService = new AdminService(mockBotClient.Object, context);
            var candidateService = new CandidateService(mockBotClient.Object, context, null, null, adminService);
            var vacancyService = new VacancyService(mockBotClient.Object, context);

            var updateHandler = new UpdateHandler(mockBotClient.Object, vacancyService, candidateService, adminService);
            await updateHandler.HandleUpdateAsync(mockBotClient.Object, update, System.Threading.CancellationToken.None);
        }

        [Scenario]
        private async Task And_user_provides_required_information()
        {
            var mockBotClient = new Mock<ITelegramBotClient>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_VacancyBot")
                .Options;
            using var context = new ApplicationDbContext(options);

            var user = new User
            {
                Id = 555555,
                Username = "candidateuser"
            };
            var chat = new Chat
            {
                Id = 555555
            };
            var messageName = new Message
            {
                Text = "Тестовий Кандидат",
                From = user,
                Chat = chat
            };
            var messagePhone = new Message
            {
                Text = "+380501234567",
                From = user,
                Chat = chat
            };
            var messageExperience = new Message
            {
                Text = "2 роки в IT",
                From = user,
                Chat = chat
            };
            var messageEmail = new Message
            {
                Text = "candidate@example.com",
                From = user,
                Chat = chat
            };
            var messageSkip = new Message
            {
                Text = "skip",
                From = user,
                Chat = chat
            };

            var updateName = new Update { Message = messageName };
            var updatePhone = new Update { Message = messagePhone };
            var updateExperience = new Update { Message = messageExperience };
            var updateEmail = new Update { Message = messageEmail };
            var updateSkip = new Update { Message = messageSkip };

            var adminService = new AdminService(mockBotClient.Object, context);
            var candidateService = new CandidateService(mockBotClient.Object, context, null, null, adminService);
            var vacancyService = new VacancyService(mockBotClient.Object, context);

            var updateHandler = new UpdateHandler(mockBotClient.Object, vacancyService, candidateService, adminService);
            await updateHandler.HandleUpdateAsync(mockBotClient.Object, updateName, System.Threading.CancellationToken.None);
            await updateHandler.HandleUpdateAsync(mockBotClient.Object, updatePhone, System.Threading.CancellationToken.None);
            await updateHandler.HandleUpdateAsync(mockBotClient.Object, updateExperience, System.Threading.CancellationToken.None);
            await updateHandler.HandleUpdateAsync(mockBotClient.Object, updateEmail, System.Threading.CancellationToken.None);
            await updateHandler.HandleUpdateAsync(mockBotClient.Object, updateSkip, System.Threading.CancellationToken.None);
        }
    }
}
