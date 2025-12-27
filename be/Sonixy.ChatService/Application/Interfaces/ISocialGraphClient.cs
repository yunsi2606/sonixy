namespace Sonixy.ChatService.Application.Interfaces;

public interface ISocialGraphClient
{
    Task<bool> IsMutualFollowAsync(string targetUserId);
}
