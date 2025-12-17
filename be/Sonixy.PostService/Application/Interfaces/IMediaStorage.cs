namespace Sonixy.PostService.Application.Interfaces;

public interface IMediaStorage
{
    Task<(string UploadUrl, string ObjectKey, string PublicUrl)> GeneratePresignedUploadUrlAsync(string fileName, string contentType, CancellationToken cancellationToken = default);
}
