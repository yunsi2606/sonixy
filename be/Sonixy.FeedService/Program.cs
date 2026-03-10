using MassTransit;
using MongoDB.Driver;
using StackExchange.Redis;
using Sonixy.FeedService.Consumers;
using Sonixy.FeedService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

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
});

// Service Clients
builder.Services.AddGrpcClient<Sonixy.Shared.Protos.PostService.PostServiceClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GrpcClients:PostService"] ?? "http://post-service:8190");
});

builder.Services.AddGrpcClient<Sonixy.Shared.Protos.SocialGraphService.SocialGraphServiceClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GrpcClients:SocialGraphService"] ?? "http://social-service:8191");
});

// Register Wrapper
builder.Services.AddScoped<IPostClient, PostClient>();
builder.Services.AddScoped<ISocialClient, SocialClient>();

// JWT Configuration
var jwtSecret = builder.Configuration["Jwt:SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Sonixy.IdentityService";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "Sonixy.Client";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
