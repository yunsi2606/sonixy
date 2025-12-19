using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Sonixy.Shared.Configuration;
using Sonixy.Shared.Interfaces;

namespace Sonixy.Shared.Infrastructure.Storage;

public class MinioMediaStorage(IMinioClient minioClient, IOptions<MinioOptions> options) : IMediaStorage
{
    private readonly MinioOptions _options = options.Value;

    public async Task<(string UploadUrl, string ObjectKey, string PublicUrl)> GeneratePresignedUploadUrlAsync(string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var uuid = Guid.NewGuid().ToString();
        var now = DateTime.UtcNow;
        
        // Key Strategy: {yyyy}/{MM}/{uuid}.{ext} - removed "posts/" prefix to be generic
        // The bucket itself should define the context (e.g. sonixy-users, sonixy-posts)
        var objectKey = $"{now.Year}/{now.Month:D2}/{uuid}{ext}";

        // Ensure bucket exists
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(_options.Bucket);
        if (!await minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken))
        {
            await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_options.Bucket), cancellationToken);
        }

        // Always check and set policy to Public Read
        try 
        {
            var policy = $@"{{
                ""Version"": ""2012-10-17"",
                ""Statement"": [
                    {{
                        ""Effect"": ""Allow"",
                        ""Principal"": {{""AWS"": ""*""}},
                        ""Action"": [""s3:GetObject""],
                        ""Resource"": [""arn:aws:s3:::{_options.Bucket}/*""]
                    }}
                ]
            }}";
            await minioClient.SetPolicyAsync(new SetPolicyArgs().WithBucket(_options.Bucket).WithPolicy(policy), cancellationToken);
        }
        catch (Exception ex)
        {
           Console.WriteLine($"Warning: Failed to set bucket policy: {ex.Message}");
        }

        // Keep presigned URL valid for 15 minutes
        var presignedArgs = new PresignedPutObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(objectKey)
            .WithExpiry(60 * 15)
            .WithHeaders(new Dictionary<string, string> 
            { 
                { "Content-Type", contentType } 
            });

        string uploadUrl;
        
        if (!string.IsNullOrEmpty(_options.PublicUrl))
        {
             try
             {
                var publicUri = new Uri(_options.PublicUrl);
                var isSecure = publicUri.Scheme == "https";
                
                var signingClient = new MinioClient()
                    .WithEndpoint(publicUri.Host, publicUri.Port > 0 ? publicUri.Port : (isSecure ? 443 : 80))
                    .WithCredentials(_options.AccessKey, _options.SecretKey)
                    .WithSSL(isSecure)
                    .Build();

                uploadUrl = await signingClient.PresignedPutObjectAsync(presignedArgs);
             }
             catch
             {
                 uploadUrl = await minioClient.PresignedPutObjectAsync(presignedArgs);
             }
        }
        else
        {
            uploadUrl = await minioClient.PresignedPutObjectAsync(presignedArgs);
        }

        var host = !string.IsNullOrEmpty(_options.PublicUrl) ? _options.PublicUrl : $"{(_options.UseSSL ? "https" : "http")}://{_options.Endpoint}";
        var publicUrl = $"{host}/{_options.Bucket}/{objectKey}";

        return (uploadUrl, objectKey, publicUrl);
    }
}
