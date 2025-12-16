# Sonixy API Gateway

## 🌐 Purpose

Gateway sử dụng **Ocelot** để route requests đến các microservices backend.

---

## 📍 Routes

Gateway không có Swagger UI riêng vì nó chỉ là reverse proxy.

**Để xem API documentation, truy cập trực tiếp các services:**

| Service | Swagger URL | Description |
|---------|-------------|-------------|
| **Identity** | http://localhost:5008 | Authentication & JWT tokens |
| **User** | http://localhost:5009 | User profiles & management |
| **Post** | http://localhost:5010 | Posts & feed |
| **Social** | http://localhost:5011 | Follow & Like functionality |

---

## 🔀 Gateway Routes

Gateway forwards requests như sau:

### Identity Service
```
Gateway: http://localhost:5100/api/identity/*
↓
Forwards to: http://identity-service:8088/api/*
```

**Endpoints:**
- `POST /api/identity/auth/register` - Register new user
- `POST /api/identity/auth/login` - Login
- `POST /api/identity/auth/refresh` - Refresh token
- `POST /api/identity/auth/revoke` - Revoke refresh token
- `GET /api/identity/auth/validate` - Validate token

### User Service
```
Gateway: http://localhost:5100/api/users/*
↓
Forwards to: http://user-service:8089/api/*
```

**Endpoints:**
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user
- `GET /api/users/me` - Get current user

### Post Service
```
Gateway: http://localhost:5100/api/posts/*
↓
Forwards to: http://post-service:8090/api/*
```

**Endpoints:**
- `GET /api/posts` - Get public feed
- `GET /api/posts/{id}` - Get post by ID
- `POST /api/posts` - Create new post
- `DELETE /api/posts/{id}` - Delete post

### Social Graph Service
```
Gateway: http://localhost:5100/api/social/*
↓
Forwards to: http://social-service:8091/api/*
```

**Endpoints:**
- `POST /api/social/follows` - Follow user
- `DELETE /api/social/follows/{targetUserId}` - Unfollow
- `GET /api/social/follows/following` - Get following list
- `POST /api/social/likes` - Like post
- `DELETE /api/social/likes/{postId}` - Unlike post

---

## 🔐 Authentication

Protected endpoints require JWT Bearer token:

```bash
curl -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  http://localhost:5100/api/users/me
```

---

## ⚡ Rate Limiting

Rate limits per service (per minute):
- Identity: 100 requests
- User: 200 requests  
- Post: 300 requests
- Social: 200 requests

---

## 🧪 Testing

### 1. Test via Gateway
```bash
# Register
curl -X POST http://localhost:5100/api/identity/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!","displayName":"Test User"}'

# Login
curl -X POST http://localhost:5100/api/identity/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!"}'
```

### 2. Test Direct Service (với Swagger UI)
- Open http://localhost:5008
- Try endpoints directly
- Copy JWT từ login response
- Use "Authorize" button để add Bearer token

---

## 🏗️ Architecture

```
Frontend (Vercel)
    │
    ▼
API Gateway :5100 (Ocelot)
    │
    ├─► Identity Service :8088
    ├─► User Service :8089
    ├─► Post Service :8090
    └─► Social Service :8091
```

---

## 📝 Configuration

Gateway configuration is environment-aware:

- **Development**: `ocelot.Development.json` (localhost)
- **Production**: `ocelot.Production.json` (Docker service names)

Auto-loads based on `ASPNETCORE_ENVIRONMENT`.

---

## 💡 Why No Gateway Swagger?

Gateway is a **reverse proxy** - it doesn't have its own endpoints, it just forwards requests.

For API documentation:
- ✅ Use individual service Swagger UIs
- ✅ Services have complete, detailed docs
- ✅ Test endpoints directly in Swagger
- ❌ Gateway aggregation adds complexity with minimal benefit

**Recommendation**: Bookmark all 4 Swagger URLs for easy access!
