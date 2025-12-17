using System.Reflection;
using Microsoft.OpenApi;
using MongoDB.Driver;
using Sonixy.PostService.Application.Services;
using Sonixy.PostService.Domain.Repositories;
using Sonixy.PostService.Infrastructure.Repositories;
using Sonixy.PostService.Infrastructure.Storage;
using Sonixy.PostService.Application.Interfaces;
using Minio;
using Microsoft.Extensions.Options;
using Sonixy.Shared.Configuration;

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

// Repository & Service Registration
// MinIO Configuration
builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection("Minio"));

builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<MinioOptions>>().Value;
    return new MinioClient()
        .WithEndpoint(options.Endpoint)
        .WithCredentials(options.AccessKey, options.SecretKey)
        .WithSSL(options.UseSSL)
        .Build();
});

builder.Services.AddScoped<IMediaStorage, MinioMediaStorage>();

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostService, PostService>();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sonixy Post Service API",
        Version = "v1",
        Description = @"Manages post creation, feed generation, and content queries on the Sonixy platform.

**Responsibilities:**
- Post CRUD operations
- Global public feed with cursor pagination
- User-specific post queries
- Feed optimization and indexing

**Features:**
- Cursor-based pagination for stable feeds
- Denormalized like counts for performance
- Visibility controls (public/followers)",
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
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration.GetValue<string>("CORS:AllowedOrigins")
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sonixy Post Service v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
