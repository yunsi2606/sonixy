using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Sonixy.EmailService.Services;

public class SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Sonixy", configuration["Smtp:Username"]));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;

        message.Body = new TextPart("html")
        {
            Text = body
        };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(configuration["Smtp:Host"], int.Parse(configuration["Smtp:Port"] ?? "587"), MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(configuration["Smtp:Username"], configuration["Smtp:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
            logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {To}", to);
            throw; // Re-throw to trigger retry in Consumer
        }
    }
}
