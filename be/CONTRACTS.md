# Inter-Service Communication Contracts

This document defines the contracts for asynchronous (Event-Driven) and synchronous (gRPC/HTTP) communication between Sonixy microservices.

## 📡 architecture Overview

The system uses a hybrid communication model:
- **Asynchronous (RabbitMQ)**: For high-volume, decoupled events (Analytics, Feed Fan-out).
- **Synchronous (gRPC/HTTP)**: For immediate data retrieval where consistency is critical (User Profile resolution).

---

## 📨 Asynchronous Events (RabbitMQ)

### 1. `UserInteractionEvent`
**Publisher**: `Sonixy.AnalyticsService` (via API), Frontend (indirectly via Analytics API)
**Consumer**: `Sonixy.FeedService`, `Sonixy.AnalyticsService` (Archiver)
**Exchange**: `Sonixy.Shared.Events:UserInteractionEvent`
**Reliability**: Fire-and-Forget (Analytics), Persistent (Feed Interest Update)

```csharp
public record UserInteractionEvent(
    string UserId,
    string TargetId,
    TargetType TargetType,  // Post, User, Comment
    UserActionType ActionType, // View, Click, Like, Share, Comment, Scroll
    long DurationMs,
    DateTime Timestamp
);
```

**Usage**:
- **Feed Service**: Updates the user's "Interest Profile" in Redis (ZSET) based on weighted actions (e.g., Like > View).
- **Analytics Service**: Archives the raw event to MongoDB for data warehousing.

### 2. `PostCreatedEvent`
**Publisher**: `Sonixy.PostService`
**Consumer**: `Sonixy.FeedService`
**Exchange**: `Sonixy.Shared.Events:PostCreatedEvent`
**Reliability**: Guaranteed Delivery (Persistent Queues)

```csharp
public record PostCreatedEvent(
    string PostId,
    string AuthorId,
    string Content,
    List<string> ImageUrls,
    DateTime CreatedAt
);
```

**Usage**:
- **Feed Service**: Triggers the "Fan-out" process. The service fetches the author's active followers and pushes the `PostId` to their personal Timeline in Redis (`sonixy:feed:timeline:{userId}`).

---

## 🤝 Synchronous Contracts (gRPC / Internal HTTP)

### 1. User Client
**Service**: `Sonixy.UserService`
**Clients**: `Sonixy.PostService`

**Contract**:
```csharp
// Get User Profile
Task<UserDto> GetUserAsync(string userId);

// Batch Get Profiles (for Feed hydration)
Task<List<UserDto>> GetUsersBatchAsync(IEnumerable<string> userIds);
```

**Responsibility**:
- `PostService` calls `UserService` to enrich `PostDto` with proper `DisplayName` and `AvatarUrl`.
- If the call fails, `PostService` falls back to "Unknown User" or cached data to ensure resilience.

---

## 🗃️ Data Replication & Shared Models

### Redis Namespacing (Feed Service)
| Key Pattern | Type | Purpose | TTL |
|---|---|---|---|
| `sonixy:feed:timeline:{userId}` | ZSET | Stores `PostId` scored by `Timestamp`. Timeline for user. | 7 Days |
| `sonixy:interest:{userId}` | ZSET | Stores `Topic` scored by `InterestWeight`. Used for ranking. | 30 Days |

### MongoDB Archival (Analytics)
- **Database**: `sonixy_analytics`
- **Collection**: `user_logs`
- **Indexing**: `{ "Timestamp": -1 }`, `{ "UserId": 1, "Timestamp": -1 }`
