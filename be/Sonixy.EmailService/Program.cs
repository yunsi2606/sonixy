using MassTransit;
using Sonixy.EmailService.Consumers;
using Sonixy.EmailService.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<SmtpEmailSender>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EmailVerificationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("email-verification-queue", e =>
        {
            // Retry Policy: 5 retries with exponential backoff
            e.UseMessageRetry(r => r.Exponential(5, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5)));

            // Dead Letter Queue configuration (MassTransit handles this by default by moving to _error queue, 
            // but we can be explicit if needed. Default behavior is skipping the queue binding so it goes to DLX)
            // Here we just ensure the consumer is bound.
            e.ConfigureConsumer<EmailVerificationConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();
