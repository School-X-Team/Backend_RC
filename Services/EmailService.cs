namespace Backend_RC.Services;
using System.Net;
using System.Net.Mail;
using RabbitMQ.Client;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpClient = new SmtpClient(_config["Email:SmtpServer"])
        {
            Port = int.Parse(_config["Email:SmtpPort"]),
            Credentials = new NetworkCredential(_config["Email:SmtpUser"], _config["Email:SmtpPassword"]),
            EnableSsl = Convert.ToBoolean(_config["Email:UseSsl"])
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_config["Email:SmtpUser"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(to);
        await smtpClient.SendMailAsync(mailMessage);
    }
}
