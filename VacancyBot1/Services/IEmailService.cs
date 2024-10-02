using VacancyBot1.Models;

namespace VacancyBot1.Services;

public interface IEmailService
{
    Task SendEmailAsync(Email request);
}
