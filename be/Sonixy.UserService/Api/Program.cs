using System.Reflection;
using System.Security.Claims;
using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MongoDB.Driver;
using Sonixy.Shared.Configuration;
using Sonixy.Shared.Extensions;
using Sonixy.UserService.Api.GrpcServices;
using Sonixy.UserService.Application.Services;
using Sonixy.UserService.Domain.Repositories;
using Sonixy.UserService.Infrastructure.Repositories;

using Microsoft.AspNetCore.Server.Kestrel.Core;
using Sonixy.UserService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel for gRPC (HTTP/2) and REST (HTTP/1.1)
builder.WebHost.ConfigureKestrel(options =>
{
    // REST API - HTTP/1.1
    options.ListenAnyIP(8089, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });

    // gRPC - HTTP/2
    options.ListenAnyIP(8189, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

// MongoDB Configuration
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>()
        ?? throw new InvalidOperationException("MongoDB settings not configured");
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped(sp =>
{
    var settings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>()
        ?? throw new InvalidOperationException("MongoDB settings not configured");
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// Repository & Service Registration
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// MinIO
builder.Services.AddSharedMinio(builder.Configuration);

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EmailVerifiedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("user-service-email-verified", e =>
        {
            e.ConfigureConsumer<EmailVerifiedConsumer>(context);
        });
    });
});

// Controllers
builder.Services.AddControllers();

// JWT Configuration
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT settings not configured");

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role,
            
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization
builder.Services.AddAuthorization();

// gRPC
builder.Services.AddGrpc();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sonixy User Service API",
        Version = "v1",
        Description = @"Manages user profiles and public identity on the Sonixy platform.

**Responsibilities:**
- User profile CRUD operations
- Public profile queries
- Batch user data retrieval for service-to-service communication

**gRPC endpoints** are also available for inter-service communication (not documented here).",
        Contact = new OpenApiContact
        {
            Name = "Sonixy Engineering",
            Email = "api@sonixy.dev"
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add JWT Bearer security definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration.GetValue<string>("CORS:AllowOrigins")
                             ?? "http://localhost:3000,http://localhost:3001";
        
        var origins = allowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries);
        
        policy.WithOrigins(origins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Static files for Swagger UI
app.UseStaticFiles();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sonixy User Service v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<UserGrpcService>();

app.Run();
