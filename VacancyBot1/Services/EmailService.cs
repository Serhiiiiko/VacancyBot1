using MimeKit;
using VacancyBot1.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit.Text;

namespace VacancyBot1.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly EmailSettings _emailSettings;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _emailSettings = _configuration.GetSection("EmailSettings").Get<EmailSettings>();
    }

    public async Task SendEmailAsync(Email email)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("", _emailSettings.EmailUsername));
        emailMessage.To.Add(MailboxAddress.Parse(email.To));
        emailMessage.Subject = email.Subject;

        var bodyBuilder = new BodyBuilder
        {
            TextBody = email.Body
        };

        // Добавляем вложение, если оно есть
        if (!string.IsNullOrEmpty(email.AttachmentPath) && File.Exists(email.AttachmentPath))
        {
            bodyBuilder.Attachments.Add(email.AttachmentPath);
        }

        emailMessage.Body = bodyBuilder.ToMessageBody();

        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(_emailSettings.EmailHost, _emailSettings.EmailPort, SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(_emailSettings.EmailUsername, _emailSettings.EmailPassword);
        await smtpClient.SendAsync(emailMessage);
        await smtpClient.DisconnectAsync(true);
    }

}

public class EmailSettings
{
    public string EmailHost { get; set; }
    public int EmailPort { get; set; }
    public string EmailUsername { get; set; }
    public string EmailPassword { get; set; }
}