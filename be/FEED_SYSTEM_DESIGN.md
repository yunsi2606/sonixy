# User Behavior Logging & Intelligent Personalized Feed Architecture
**Author:** yunsi2606
**Date:** 2025-12-23

## 1. High-Level Architecture Overview

The system introduces two new distinct microservices to handle high-volume data and complex logic without impacting the core transactional services (`PostService`, `UserService`).

```mermaid
graph TD
    Client[Client App (Web/Mobile)]
    GW[API Gateway (Ocelot)]
    
    subgraph Core Services
        Auth[IdentityService]
        User[UserService]
        Post[PostService]
        Social[SocialGraphService]
    end

    subgraph New Services
        Analytics[AnalyticsService]
        Feed[FeedService]
    end

    subgraph Infrastructure
        MQ[(RabbitMQ)]
        Redis[(Redis Cache)]
        Mongo[(MongoDB Cluster)]
    end

    %% Data Flow
    Client --> GW
    GW --> Analytics
    GW --> Feed
    GW --> Post

    %% Event Flow
    Analytics -- "Publish: UserAction" --> MQ
    Post -- "Publish: PostCreated" --> MQ
    
    %% Consumption
    MQ -- "Consume: PostCreated" --> Feed
    MQ -- "Consume: UserAction" --> Feed
    
    %% Storage Interactions
    Analytics --> Mongo
    Feed --> Redis
    Feed --> Mongo
```

---

## 2. Microservice 1: User Behavior Logging (`AnalyticsService`)

### Responsibilities
*   **High-Performance Ingestion**: Acts as a "firehose" endpoint for frontend clients to send telemetry data.
*   **Validation & Sanitation**: Ensures incoming events have required fields (`UserId`, `TargetId`, `ActionType`) but performs minimal logic to ensure low latency.
*   **Event Publishing**: Immediately pushes validated events to the Message Broker (RabbitMQ) for asynchronous processing by other subscribers.
*   **Archival**: Persists raw logs to a write-optimized store (MongoDB Time Series) for audit trails and batch processing.

### Event API Contract
**Endpoint**: `POST /api/analytics/events`

**Payload:**
```json
{
  "userId": "guid",
  "targetId": "guid",  // PostId or ProfileUserId
  "targetType": "Post", // Enum: Post, User, Comment
  "actionType": "Like", // Enum: View, Like, Scroll, Share, Click
  "durationMs": 1500,   // Optional: For dwell time
  "timestamp": "2025-12-23T09:00:00Z",
  "metadata": {
    "device": "mobile",
    "source": "feed_v1"
  }
}
```

### Scalability Strategy
*   **Async Processing**: API responds `202 Accepted` immediately after basic validation.
*   **Bulk Insert**: Uses MongoDB bulk write operations for archival.
*   **Stateless**: Can scale horizontally behind the Gateway to handle thousands of concurrent requests.

---

## 3. Microservice 2: Intelligent Feed (`FeedService`)

### Responsibilities
*   **Feed Aggregation**: Assembles a user's timeline from multiple sources.
*   **Personalization Engine**: Calculates scores for posts based on user interests and signals.
*   **Interest Profiling**: Maintains a dynamic profile of what a user likes (e.g., Tags, Categories, Authors) based on `AnalyticsService` events.
*   **Hybrid Delivery**: Combines "Push" (Fan-out) for speed and "Pull" (On-demand) for cost efficiency.

### Data Flow & Logic

#### A. Interest Profiling (The "Learning" Phase)
1.  **Input**: Listens to `UserAction` events from RabbitMQ.
2.  **Logic**:
    *   If `Action == Like` on a Post about "Tech", increment `UserInterest:Tech` score by +5.
    *   If `Action == Scroll` past "Sports", decrement `UserInterest:Sports` by -0.5.
    *   If `Action == View` (> 5000ms), increment topic score by +1.
3.  **Storage**: Redis Hash or MongoDB Document `UserProfile:{UserId}`.

#### B. The Feed Generation (The "Serving" Phase)
We use a **Hybrid Fan-Out Strategy**:

1.  **For Active Users (Push Model)**:
    *   When `PostService` publishes `PostCreated`:
    *   `FeedService` finds followers of the author.
    *   For *Active* followers (logged in last 24h), calculate a **Relevance Score**.
    *   If Score > Threshold, push PostID to `Redis:Timeline:{FollowerId}` (Sorted Set).
    *   **Result**: Instant feed load.

2.  **For Inactive Users (Pull Model)**:
    *   If a user logs in, `FeedService` generates the feed on-the-fly (Pull) by querying generic implementation + re-ranking.

### Feed Scoring Formula (Conceptual)
```csharp
FinalScore = (Weight_Freshness * DecayFactor) 
           + (Weight_Affinity * InteractionCountWithAuthor) 
           + (Weight_Interest * TopicMatchScore)
           + (Weight_Global * ViralityBonus)
```

*   **Freshness**: Exponential decay based on `Post.CreatedAt`.
*   **Affinity**: How much `Viewer` interacts with `Author` (from historical logs).
*   **Interest**: Dot product of `Post.Tags` vector and `User.Interest` vector.

---

## 4. Logical Data Models

### A. Analytics Store (MongoDB)
*Collection: `UserLogs`*
```javascript
{
  _id: ObjectId,
  userId: UUID,
  action: String,
  targetId: UUID,
  timestamp: Date,
  meta: Object
}
```
*Index*: `{ timestamp: -1, userId: 1 }` (for time-range analysis).

### B. Feed Store (Redis)
*Key Type: Sorted Set (ZSET)*
*   **Key**: `timeline:{userId}`
*   **Member**: `postId`
*   **Score**: `ComputedRelevanceScore` (Double)

*Key Type: Hash*
*   **Key**: `user:interests:{userId}`
*   **Fields**: `{ "tech": 10.5, "cooking": 2.1 }`

---

## 5. Technology Stack & Scalability

| Component | Technology | Rationale |
|-----------|------------|-----------|
| **Event Bus** | RabbitMQ | Reliable, durable messaging for decoupling. |
| **Log Storage** | MongoDB | High write throughput, schema-less flexibility. |
| **Hot Feed** | Redis (Cluster) | Sub-millisecond read latency for timelines. |
| **Compute** | .NET 8 Worker | Efficient background processing for rankings. |

### Future Extensions
1.  **Machine Learning**: Export MongoDB logs to a Data Lake (e.g., S3/Parquet). Train a Collaborative Filtering model. Serve model via ONNX runtime in `FeedService` for better "Interest" vectors.
2.  **Bloom Filters**: Use Redis Bloom Filters in `FeedService` to ensure a user never sees the same post twice (Deduplication).
