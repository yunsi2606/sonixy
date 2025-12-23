using MassTransit;
using MongoDB.Driver;
using StackExchange.Redis;
using Sonixy.FeedService.Consumers;
using Sonixy.FeedService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Redis Configuration
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var connectionString = builder.Configuration["Redis:ConnectionString"];
    return ConnectionMultiplexer.Connect(connectionString);
});

// MongoDB Configuration
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration["MongoDB:ConnectionString"];
    return new MongoClient(connectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var dbName = builder.Configuration["MongoDB:DatabaseName"];
    return client.GetDatabase(dbName);
    return client.GetDatabase(dbName);
});

// Service Clients
builder.Services.AddGrpcClient<Sonixy.Shared.Protos.PostService.PostServiceClient>(o =>
{
    o.Address = new Uri("http://post-service:8190");
});

// Register Wrapper
builder.Services.AddScoped<IPostClient, PostClient>();

// MassTransit (RabbitMQ)
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserInteractionConsumer>();
    x.AddConsumer<PostCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        var user = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        var pass = builder.Configuration["RabbitMQ:Password"] ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(user);
            h.Password(pass);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
