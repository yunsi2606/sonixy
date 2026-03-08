using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Nest;
using Sonixy.SearchService.Api.Consumers;
using Sonixy.SearchService.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Elasticsearch
var elasticUri = builder.Configuration["Elasticsearch:Uri"] ?? "http://elasticsearch:9200";
var settings = new ConnectionSettings(new Uri(elasticUri))
    .DefaultIndex("sonixy")
    .EnableDebugMode();
var elasticClient = new ElasticClient(settings);
builder.Services.AddSingleton<IElasticClient>(elasticClient);

// Search Service
builder.Services.AddSingleton<ISearchService, ElasticsearchSearchService>();

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreatedConsumer>();
    x.AddConsumer<PostCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq";
        var username = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        var password = builder.Configuration["RabbitMQ:Password"] ?? "guest";

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "Sonixy.IdentityService",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "Sonixy.Client",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

// CORS
var allowedOrigins = builder.Configuration["CORS:AllowOrigins"]?.Split(',') ?? ["http://localhost:3000"];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sonixy Search API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
