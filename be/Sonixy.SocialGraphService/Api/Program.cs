using System.Reflection;
using Microsoft.OpenApi;
using MongoDB.Driver;
using Sonixy.Shared.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Sonixy.SocialGraphService.Api.GrpcServices;
using Sonixy.SocialGraphService.Application.Services;
using Sonixy.SocialGraphService.Domain.Repositories;
using Sonixy.SocialGraphService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // REST (nếu có)
    options.ListenAnyIP(8091, o =>
    {
        o.Protocols = HttpProtocols.Http1;
    });

    // gRPC
    options.ListenAnyIP(8191, o =>
    {
        o.Protocols = HttpProtocols.Http2;
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
builder.Services.AddScoped<IFollowRepository, FollowRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<ISocialGraphService, SocialGraphService>();

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
        Title = "Sonixy SocialGraph Service API",
        Version = "v1",
        Description = @"Manages social interactions and relationship graphs on the Sonixy platform.

**Responsibilities:**
- Follow/Unfollow user relationships
- Like/Unlike post interactions
- Social graph queries and statistics
- Relationship validation

**Features:**
- Idempotent operations
- Efficient relationship lookups
- Aggregated like counts",
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sonixy SocialGraph Service v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<SocialGraphGrpcService>();

app.Run();
