namespace Sonixy.PostService.Application.DTOs;

public record PresignedUrlRequestDto(string FileName, string ContentType);

public record PresignedUrlResponseDto(string UploadUrl, string ObjectKey);
