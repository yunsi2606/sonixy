namespace Sonixy.ChatService.Domain.Interfaces;

public interface IPresenceTracker
{
    Task UserConnected(string userId, string connectionId);
    Task UserDisconnected(string userId, string connectionId);
    Task<bool> IsUserOnline(string userId);
    Task<List<string>> GetOnlineUsers(IEnumerable<string> userIds);
}
