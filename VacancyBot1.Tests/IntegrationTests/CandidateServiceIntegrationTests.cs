using Moq;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using VacancyBot1.Data;
using VacancyBot1.Models;
using VacancyBot1.Services;
using Microsoft.Extensions.Configuration;

namespace VacancyBot1.Tests.IntegrationTests
{
    public class CandidateServiceIntegrationTests
    {
        [Fact]
        public async Task CandidateApplication_ShouldSaveToDatabase_AndNotifyAdmins()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CandidateTestDb")
                .Options;

            var mockBotClient = new Mock<ITelegramBotClient>();

            var mockEmailService = new Mock<IEmailService>();

            var inMemorySettings = new Dictionary<string, string?> {
                            {"EmailSettings:EmailHost", "smtp.example.com"},
                            {"EmailSettings:EmailPort", "587"},
                            {"EmailSettings:EmailUsername", "user@example.com"},
                            {"EmailSettings:EmailPassword", "password"}
                        };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            using (var context = new ApplicationDbContext(options))
            {
                var vacancy = new Vacancy { Id = 1, Title = "Software Engineer", Description = "Develop software", Requirements = "C#, .NET" };
                context.Vacancies.Add(vacancy);

                var admin = new Admin { Id = 1, TelegramId = 123456789, Email = "admin@example.com", TelegramUsername = "adminuser" };
                context.Admins.Add(admin);

                context.SaveChanges();

                var adminService = new AdminService(mockBotClient.Object, context);
                var candidateService = new CandidateService(mockBotClient.Object, context, mockEmailService.Object, configuration, adminService);

                await candidateService.StartApplicationAsync(new User { Id = 555555, Username = "candidateuser" }, vacancy.Id);

                // Act
                await candidateService.HandleApplicationAsync(new Message
                {
                    From = new User { Id = 555555, Username = "candidateuser" },
                    Chat = new Chat { Id = 555555 },
                    Text = "Тестовий Кандидат"
                });
                
                await candidateService.HandleApplicationAsync(new Message
                {
                    From = new User { Id = 555555, Username = "candidateuser" },
                    Chat = new Chat { Id = 555555 },
                    Text = "+380501234567"
                });

                await candidateService.HandleApplicationAsync(new Message
                {
                    From = new User { Id = 555555, Username = "candidateuser" },
                    Chat = new Chat { Id = 555555 },
                    Text = "2 роки в IT"
                });

                await candidateService.HandleApplicationAsync(new Message
                {
                    From = new User { Id = 555555, Username = "candidateuser" },
                    Chat = new Chat { Id = 555555 },
                    Text = "candidate@example.com"
                });

                await candidateService.HandleApplicationAsync(new Message
                {
                    From = new User { Id = 555555, Username = "candidateuser" },
                    Chat = new Chat { Id = 555555 },
                    Text = "skip"
                });

                // Assert
                var savedCandidate = context.Candidates.FirstOrDefault(c => c.TelegramId == 555555);
                Assert.NotNull(savedCandidate);
                Assert.Equal("Тестовий Кандидат", savedCandidate.FullName);
                Assert.Equal("+380501234567", savedCandidate.PhoneNumber);
                Assert.Equal("candidate@example.com", savedCandidate.Email);


                mockEmailService.Verify(emailService => emailService.SendEmailAsync(
                    It.Is<Email>(email => email.To == admin.Email && email.Subject.Contains("Новий кандидат"))), Times.Once);
            }
        }
    }
}
