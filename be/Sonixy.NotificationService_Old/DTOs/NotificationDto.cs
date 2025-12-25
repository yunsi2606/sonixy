namespace Sonixy.NotificationService.DTOs;

public record NotificationDto(
    string Id,
    string ActorId,
    string ActorName,
    string ActorAvatar,
    string EntityById,
    string EntityType,
    string Action,
    bool IsRead,
    DateTime CreatedAt
);
