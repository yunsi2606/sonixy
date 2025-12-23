using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MongoDB.Driver;
using Sonixy.IdentityService.Api.GrpcServices;
using Sonixy.IdentityService.Application.Services;
using Sonixy.IdentityService.Domain.Repositories;
using Sonixy.IdentityService.Infrastructure.Repositories;
using Sonixy.Shared.Configuration;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddAuthorization();

// Repository & Service Registration
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// MassTransit Configuration
builder.Services.AddMassTransit(x =>
{
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
    });
});


// Controllers
builder.Services.AddControllers();

// gRPC
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<Sonixy.Shared.Protos.UserService.UserServiceClient>(o => 
{
    o.Address = new Uri("http://user-service:8189");
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sonixy Identity Service API",
        Version = "v1",
        Description = @"Authentication and authorization service for the Sonixy platform.

**Responsibilities:**
- User registration and login
- JWT access token generation
- Refresh token management with rotation
- Token validation for other services
- Password hashing with BCrypt

**Features:**
- JWT Bearer authentication
- Refresh token rotation (revoke old token on refresh)
- Secure password hashing (BCrypt work factor 12)
- Token expiration: Access (15 min), Refresh (7 days)",
        Contact = new OpenApiContact
        {
            Name = "Sonixy Engineering",
            Email = "sonysam.contacts@gmail.com"
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
              .AllowAnyHeader()
              .AllowCredentials();
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sonixy Identity Service v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<IdentityGrpcService>();

app.Run();
