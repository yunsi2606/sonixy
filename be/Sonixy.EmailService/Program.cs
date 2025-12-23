using MassTransit;
using Sonixy.EmailService.Consumers;
using Sonixy.EmailService.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<SmtpEmailSender>();

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EmailVerificationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var host = configuration["RabbitMQ:Host"] ?? "rabbitmq";
        var username = configuration["RabbitMQ:Username"] ?? "guest";
        var password = configuration["RabbitMQ:Password"] ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ReceiveEndpoint("email-verification-queue", e =>
        {
            // Retry Policy: 5 retries with exponential backoff
            e.UseMessageRetry(r => r.Exponential(5, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5)));
            e.ConfigureConsumer<EmailVerificationConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();
