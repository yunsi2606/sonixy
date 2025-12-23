# Sonixy Backend - Quick Start Guide

## 🚀 Prerequisites

- .NET 10 SDK installed
- MongoDB running on `localhost:27017`
- Visual Studio Code or Visual Studio 2022

## 📦 Services Overview

### 1. **Sonixy.IdentityService** (Port 5008)
**Database**: `sonixy_identity`
**Endpoints**: `POST /api/auth/login`, `POST /api/auth/register`
**Features**: JWT Token issuance, Secure Password Hashing

---

### 2. **Sonixy.UserService** (Port 5009)
**Database**: `sonixy_users`

**Endpoints**:
- `POST /api/users` - Create user
- `GET /api/users/{id}` - Get user by ID
- `PATCH /api/users/{id}` - Update user
- `POST /api/users/batch` - Batch get users

**Features**:
- Email uniqueness validation
- Profile management
- Batch queries for other services
- MinIO Avatar Uploads

---

### 3. **Sonixy.PostService** (Port 5010)
**Database**: `sonixy_posts`

**Endpoints**:
- `POST /api/posts` - Create post
- `GET /api/posts/{id}` - Get post
- `GET /api/posts/feed?cursor={cursor}&pageSize={size}` - Public feed (cursor paginated)
- `GET /api/posts/user/{userId}` - User's posts

**Features**:
- Cursor-based pagination
- Denormalized like counts
- Visibility controls (public/followers)

---

### 3. **Sonixy.SocialGraphService** (Port 5002)
**Database**: `sonixy_social`

**Endpoints**:
- `POST /api/follows/{followingId}` - Follow user
- `DELETE /api/follows/{followingId}` - Unfollow user
- `GET /api/follows/{followingId}/status` - Check follow status
- `POST /api/likes/{postId}` - Like post
- `DELETE /api/likes/{postId}` - Unlike post
- `GET /api/likes/{postId}/count` - Get like count

**Features**:
- Idempotent operations
- Unique relationship constraints
- Social metrics aggregation

---

### 4. **Sonixy.AnalyticsService** (Port 8092)
**Database**: `sonixy_analytics` (MongoDB)

**Endpoints**:
- `POST /api/analytics/events` - Ingest user behavior events

**Features**:
- High-throughput fire-and-forget ingestion
- RabbitMQ publishing
- MongoDB archival

---

### 5. **Sonixy.FeedService** (Port 8093)
**Database**: Redis (Timeline/Interest), MongoDB (Metadata)

**Endpoints**:
- `GET /api/feed` - Get personalized timeline

**Features**:
- Hybrid Fan-out (Push/Pull)
- Interest scoring & decay
- Redis ZSET timeline storage

---

## 🏗️ Running the Services

### Run All Services (Recommended)

The easiest way to run the system with all dependencies (MongoDB, Redis, RabbitMQ) is using Docker.

```powershell
cd be
docker-compose up -d --build
```

**Service URLs:**
- **Gateway**: http://localhost:5100
- **Identity**: http://localhost:5008
- **User**: http://localhost:5009
- **Post**: http://localhost:5010
- **Social**: http://localhost:5011
- **Analytics**: http://localhost:5012
- **Feed**: http://localhost:5013

### Run Locally (Advanced)

If you want to run services manually with `dotnet run`, **ports will differ** (check `launchSettings.json` in each project). You must also ensure MongoDB, Redis, and RabbitMQ are running manually.

---

## 🗄️ MongoDB Setup

### Auto-Created Collections

When you start the services, they will automatically:
1. Connect to MongoDB
2. Create databases if not exist
3. Create indexes for optimal performance

### Collections Created

**sonixy_users**:
- `users` collection with email unique index

**sonixy_posts**:
- `posts` collection with compound indexes for feeds

**sonixy_social**:
- `follows` collection with unique follower-following constraint
- `likes` collection with unique user-post constraint

---

## 📝 Testing with Swagger

### 1. Create a User (UserService)
Navigate to `http://localhost:5009`

```json
POST /api/users
{
  "displayName": "Nhat Cuong",
  "email": "nhatcuong@example.com",
  "bio": "Software Developer",
  "avatarUrl": "https://via.placeholder.com/150"
}
```

**Copy the returned `id`** for next steps.

### 2. Create a Post (PostService)
Navigate to `http://localhost:5010`

```json
POST /api/posts
{
  "content": "Hello from Sonixy! This is my first post.",
  "visibility": "public"
}
```

Note: Current implementation uses placeholder userId

### 3. Get Public Feed
```
GET /api/posts/feed?pageSize=20
```

Returns:
```json
{
  "items": [...],
  "nextCursor": "encoded_cursor_string",
  "hasMore": true
}
```

### 4. Follow a User (SocialGraphService)
Navigate to `http://localhost:5002`

```
POST /api/follows/{userId}
```

### 5. Like a Post
```
POST /api/likes/{postId}
```

---

## 🎯 Architecture Verification

### ✅ What's Implemented

**Patterns**:
- ✅ Repository Pattern (MongoDB)
- ✅ Specification Pattern (query logic)
- ✅ Cursor Pagination
- ✅ DTO Mapping
- ✅ Layered Architecture (Api/Application/Domain/Infrastructure)

**Infrastructure**:
- ✅ MongoDB integration with ObjectId
- ✅ Unique constraints via indexes
- ✅ Compound indexes for performance
- ✅ Shared configuration (`MongoDbSettings`)

**Documentation**:
- ✅ Comprehensive Swagger on all services
- ✅ XML comments on all endpoints
- ✅ Request/response examples

**Code Quality**:
- ✅ Primary constructors (C# 12)
- ✅ Async/await throughout
- ✅ Proper error handling
- ✅ Validation attributes

---

## 🔧 Configuration

### MongoDB Connection Strings

Edit `appsettings.json` in each service's `Api` folder:

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "sonixy_users"  // or sonixy_posts, sonixy_social
  }
}
```

For production, use environment variables:
```powershell
$env:MongoDB__ConnectionString = "mongodb://production-server:27017"
```

---

## 🐛 Troubleshooting

### "MongoDB settings not configured"
**Solution**: Ensure `appsettings.json` has the `MongoDB` section

### Port already in use
**Solution**: Change port in `Properties/launchSettings.json` or kill the process:
```powershell
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### Build errors
**Solution**: Restore NuGet packages:
```powershell
dotnet restore
```

---

## 📊 Performance Tips

### Indexes
All critical indexes are auto-created on startup:
- User email uniqueness
- Post feed queries (createdAt + visibility)
- Follow relationships (unique constraint)
- Like relationships (unique constraint)

### Cursor Pagination
Use cursor pagination for feeds to avoid:
- Page drift with new data
- Inefficient `skip()` operations

Example:
```
1. GET /api/posts/feed?pageSize=20
2. Use returned nextCursor in next request
3. GET /api/posts/feed?cursor={nextCursor}&pageSize=20
```

---

## 🎓 Next Steps

### Phase 1: Current State ✅
- ✅ All services built and running
- ✅ MongoDB integration complete
- ✅ Swagger documentation

### Phase 2: Authentication
- [ ] Implement JWT in IdentityService
- [ ] Add authentication middleware
- [ ] Extract userId from JWT tokens

### Phase 3: gRPC
- [ ] Define `.proto` contracts
- [ ] Implement gRPC services
- [ ] Update service-to-service calls

### Phase 4: API Gateway
- [ ] Setup Ocelot
- [ ] Configure routing
- [ ] Add rate limiting

---

## 📚 References

- [Implementation Plan](../.artifacts/implementation_plan.md)
- [Walkthrough](../.artifacts/walkthrough.md)
- [Inter-Service Contracts](CONTRACTS.md)
- [Feed System Design](FEED_SYSTEM_DESIGN.md)
- [Main README](../README.md)

---

**Built with .NET 10, MongoDB, and production-ready patterns** 🚀
