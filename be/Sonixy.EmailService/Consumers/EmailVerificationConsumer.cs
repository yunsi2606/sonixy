using MassTransit;
using Sonixy.EmailService.Services;
using Sonixy.Shared.Events;

namespace Sonixy.EmailService.Consumers;

public class EmailVerificationConsumer(
    SmtpEmailSender emailSender,
    ILogger<EmailVerificationConsumer> logger,
    IConfiguration configuration)
    : IConsumer<EmailVerificationRequestedEvent>
{
    public async Task Consume(ConsumeContext<EmailVerificationRequestedEvent> context)
    {
        var message = context.Message;
        logger.LogInformation("Processing email verification for User: {UserId}, Email: {Email}", message.UserId, message.Email);

        var verifyUrl = $"{configuration["FrontendUrl"]}/verify-email?token={message.Token}";
        
        var emailBody = $@"
            <h1>Verify your email</h1>
            <p>Please click the link below to verify your email address:</p>
            <p><a href='{verifyUrl}'>Verify Email</a></p>
            <p>This link will expire in 24 hours.</p>
        ";

        await emailSender.SendEmailAsync(message.Email, "Sonixy - Verify your email", emailBody);
    }
}
