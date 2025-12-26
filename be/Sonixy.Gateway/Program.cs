using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using MMLib.SwaggerForOcelot.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Load environment-specific Ocelot config
var environment = builder.Environment.EnvironmentName;
var ocelotFileName = $"ocelot.{environment}.json";

Console.WriteLine($"🌐 Loading Ocelot config: {ocelotFileName}");
builder.Configuration.AddJsonFile(ocelotFileName, optional: false, reloadOnChange: true);

// JWT Authentication
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] 
    ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Sonixy.IdentityService";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "Sonixy.Client";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken)
                    && path.StartsWithSegments("/hubs/notifications"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

// Add Ocelot
builder.Services.AddOcelot();

builder.Services.AddMvc();

// Swagger For Ocelot
// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerForOcelot(builder.Configuration);

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

// Middleware
app.UseRouting();
app.UseCors();
app.UseWebSockets();
app.UseAuthentication();

// app.UseSwagger();
// Swagger For Ocelot UI
app.MapWhen(
    ctx => ctx.Request.Path.StartsWithSegments("/swagger"),
    swaggerApp =>
    {
        swaggerApp.UseSwaggerForOcelotUI(opt =>
        {
            opt.PathToSwaggerGenerator = "/swagger/docs";
        });
    }
);

await app.UseOcelot();

app.Run();
