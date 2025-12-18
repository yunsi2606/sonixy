using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Sonixy.PostService.Application.Interfaces;

namespace Sonixy.PostService.Infrastructure.Storage;

public class MinioMediaStorage(IMinioClient minioClient, IOptions<MinioOptions> options) : IMediaStorage
{
    private readonly MinioOptions _options = options.Value;

    public async Task<(string UploadUrl, string ObjectKey, string PublicUrl)> GeneratePresignedUploadUrlAsync(string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var uuid = Guid.NewGuid().ToString();
        var now = DateTime.UtcNow;
        
        // Key Strategy: posts/{yyyy}/{MM}/{uuid}.{ext}
        var objectKey = $"posts/{now.Year}/{now.Month:D2}/{uuid}{ext}";

        // Ensure bucket exists
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(_options.Bucket);
        if (!await minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken))
        {
            await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_options.Bucket), cancellationToken);
        }

        // Always check and set policy to Public Read to ensure images are accessible
        // This fixes the "missing key" (Access Denied) issue if the bucket existed but lost policy
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
        
        // Fix: Use a client configured with the Public URL for signing
        // This ensures the signature matches the Host header the browser will send (media-sonixy...)
        if (!string.IsNullOrEmpty(_options.PublicUrl))
        {
             try
             {
                var publicUri = new Uri(_options.PublicUrl);
                var isSecure = publicUri.Scheme == "https";
                
                // Construct a temporary client for signing
                var signingClient = new MinioClient()
                    .WithEndpoint(publicUri.Host, publicUri.Port > 0 ? publicUri.Port : (isSecure ? 443 : 80))
                    .WithCredentials(_options.AccessKey, _options.SecretKey)
                    .WithSSL(isSecure)
                    .Build();

                uploadUrl = await signingClient.PresignedPutObjectAsync(presignedArgs);
             }
             catch
             {
                 // Fallback to internal client if parsing fails
                 uploadUrl = await minioClient.PresignedPutObjectAsync(presignedArgs);
             }
        }
        else
        {
            uploadUrl = await minioClient.PresignedPutObjectAsync(presignedArgs);
        }

        // Generate Public URL for viewing
        // If PublicUrl is configured (e.g. CDN or specific host), use it. Otherwise use the endpoint/bucket/key
        var host = !string.IsNullOrEmpty(_options.PublicUrl) ? _options.PublicUrl : $"{(_options.UseSSL ? "https" : "http")}://{_options.Endpoint}";
        
        // MinIO default path access: http://host:9000/bucket/key
        var publicUrl = $"{host}/{_options.Bucket}/{objectKey}";

        return (uploadUrl, objectKey, publicUrl);
    }
}
