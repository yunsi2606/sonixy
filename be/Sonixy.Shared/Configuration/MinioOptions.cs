namespace Sonixy.PostService.Infrastructure.Storage;

public class MinioOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Bucket { get; set; } = "media";
    public bool UseSSL { get; set; } = false;
    public string PublicUrl { get; set; } = string.Empty;
}
