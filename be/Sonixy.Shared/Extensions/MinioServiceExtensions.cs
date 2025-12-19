using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Sonixy.Shared.Configuration;
using Sonixy.Shared.Infrastructure.Storage;
using Sonixy.Shared.Interfaces;

namespace Sonixy.Shared.Extensions;

public static class MinioServiceExtensions
{
    public static IServiceCollection AddSharedMinio(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioOptions>(configuration.GetSection("Minio"));
        var minioOptions = configuration.GetSection("Minio").Get<MinioOptions>();

        if (minioOptions != null)
        {
            services.AddMinio(configureClient => configureClient
                .WithEndpoint(minioOptions.Endpoint)
                .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
                .WithSSL(minioOptions.UseSSL));

            services.AddScoped<IMediaStorage, MinioMediaStorage>();
        }

        return services;
    }
}
