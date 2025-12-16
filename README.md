# Sonixy - Modern Social Platform

A production-ready microservice-based social web application demonstrating clean architecture, scalable design patterns, and premium UI/UX.

![Tech Stack](https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet)
![Next.js](https://img.shields.io/badge/Next.js-15-000000?style=for-the-badge&logo=nextdotjs)
![MongoDB](https://img.shields.io/badge/MongoDB-47A248?style=for-the-badge&logo=mongodb&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?style=for-the-badge&logo=typescript&logoColor=white)

## 🎯 Core Values

- **Identity** - User profiles and authentication
- **Community** - Follow relationships and social graph
- **Expression** - Post creation and personalized feeds
- **Performance** - Optimized queries and cursor pagination

## 🏗️ Architecture

### Microservices
```
┌─────────────┐
│   Gateway   │  → Ocelot API Gateway (planned)
└─────────────┘
      ↓
┌──────────────────────────────────────────┐
│  IdentityService  │  UserService         │
│  PostService      │  SocialGraphService  │
└──────────────────────────────────────────┘
      ↓
┌──────────────────────────────────────────┐
│           MongoDB Collections            │
└──────────────────────────────────────────┘
```

### Tech Stack

**Backend**
- .NET 10 + ASP.NET Core Web API
- MongoDB with ObjectId
- gRPC for inter-service communication (planned)
- Swagger/OpenAPI documentation
- Repository + Specification patterns
- Cursor-based pagination

**Frontend**
- Next.js 15 (App Router)
- TypeScript
- Tailwind CSS v4
- Custom design system
- SEO optimized

## 🚀 Quick Start

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 18+](https://nodejs.org/)
- [MongoDB](https://www.mongodb.com/try/download/community)

### Backend

```powershell
# UserService
cd be/Sonixy.UserService/Api
dotnet restore
dotnet run
# Swagger: http://localhost:5000

# PostService  
cd be/Sonixy.PostService/Api
dotnet restore
dotnet run
# Swagger: http://localhost:5001

# SocialGraphService
cd be/Sonixy.SocialGraphService/Api
dotnet restore
dotnet run
# Swagger: http://localhost:5002
```

### Frontend

```powershell
cd fe
npm install
npm run dev
# http://localhost:3000
```

## 📁 Project Structure

```
sonixy/
├── be/
│   ├── Sonixy.Shared/              # Common patterns & utilities
│   │   ├── Common/                 # Entity, Repository
│   │   ├── Specifications/         # ISpecification
│   │   └── Pagination/             # Cursor pagination
│   │
│   ├── Sonixy.UserService/         # User profile management
│   │   ├── Api/                    # Controllers, Program.cs
│   │   ├── Application/            # DTOs, Services
│   │   ├── Domain/                 # Entities, Repositories
│   │   └── Infrastructure/         # Data access
│   │
│   ├── Sonixy.PostService/         # Content & feeds
│   │   ├── Api/
│   │   ├── Application/
│   │   ├── Domain/
│   │   └── Infrastructure/
│   │
│   └── Sonixy.SocialGraphService/  # Follow & Like
│       ├── Api/
│       ├── Application/
│       ├── Domain/
│       └── Infrastructure/
│
└── fe/
    ├── app/                        # Next.js app directory
    │   ├── globals.css             # Design system
    │   ├── layout.tsx              # Root layout
    │   └── page.tsx                # Landing page
    ├── components/                 # (To be added)
    ├── services/                   # API clients
    └── types/                      # TypeScript definitions
```

## 🎨 Design System

### Color Palette
```css
Primary:        #7C7CFF
Secondary:      #00E5FF
Background:     #0B0D17
Surface:        rgba(255, 255, 255, 0.06)
Text:           #EDEDED
```

### Features
- **Glassmorphism** - Frosted glass effects
- **Smooth animations** - Micro-interactions
- **Dark mode first** - Premium aesthetic
- **Responsive** - Mobile-first design

## 📊 Key Features Implemented

### Backend
- ✅ Layered architecture (Api/Application/Domain/Infrastructure)
- ✅ Repository + Specification patterns
- ✅ Cursor-based pagination for feeds
- ✅ MongoDB with proper indexes
- ✅ Comprehensive Swagger documentation
- ✅ DTOs with manual mapping
- ✅ Async/await throughout

### Frontend
- ✅ Next.js 15 with App Router
- ✅ Tailwind CSS v4 design system
- ✅ SEO metadata
- ✅ Premium landing page
- ✅ Glassmorphic UI components

## 🗄️ Database Schema

**Collections:**
- `users` - User profiles
- `posts` - Post content with denormalized like counts
- `follows` - Follow relationships
- `likes` - Post likes

All collections use MongoDB ObjectId and have optimized indexes for common queries.

## 📖 API Documentation

Each microservice exposes Swagger UI:

### UserService
- `GET /api/users/{id}` - Get user profile
- `POST /api/users` - Create user
- `PATCH /api/users/{id}` - Update user
- `POST /api/users/batch` - Batch get users

### PostService
- `GET /api/posts/{id}` - Get post
- `POST /api/posts` - Create post
- `GET /api/posts/feed` - Get public feed (cursor paginated)
- `GET /api/posts/user/{userId}` - Get user's posts

### SocialGraphService
- `POST /api/follows/{followingId}` - Follow user
- `DELETE /api/follows/{followingId}` - Unfollow user
- `POST /api/likes/{postId}` - Like post
- `DELETE /api/likes/{postId}` - Unlike post
- `GET /api/likes/{postId}/count` - Get like count

## 🧪 Design Patterns

### 1. Repository Pattern
Abstraction over data access with MongoDB-specific implementation.

### 2. Specification Pattern
Encapsulates query logic for reusable, composable filters.

### 3. Cursor Pagination
Stable pagination that handles real-time data inserts gracefully.

### 4. DTO Pattern
Decouples API contracts from domain models with explicit mapping.

### 5. Layered Architecture
Clear separation: Api → Application → Domain → Infrastructure

## 🎓 Learning Resources

For detailed implementation walkthrough, see:
- [Implementation Plan](./.artifacts/implementation_plan.md)
- [Walkthrough](./.artifacts/walkthrough.md)

## 🔮 Roadmap

### Phase 1: Core Complete ✅
- [x] Backend microservices scaffold
- [x] MongoDB integration
- [x] Cursor pagination
- [x] Frontend with design system

### Phase 2: Authentication & gRPC
- [ ] IdentityService with JWT
- [ ] gRPC inter-service communication
- [ ] API Gateway (Ocelot)

### Phase 3: Full Frontend
- [ ] Authentication pages
- [ ] Feed with infinite scroll
- [ ] Profile pages
- [ ] Post creation UI

### Phase 4: Advanced Features
- [ ] Real-time updates (SignalR)
- [ ] Image uploads
- [ ] Comments
- [ ] Notifications

### Phase 5: Production
- [ ] Redis caching
- [ ] Rate limiting
- [ ] Docker compose
- [ ] Kubernetes deployment

## 🤝 Contributing

This is a demonstration project showcasing modern web architecture patterns. Feel free to use it as a reference for your own projects.

## 📄 License

MIT License - See [LICENSE](LICENSE) for details

---

**Built with clean code, scalable architecture, and attention to detail** ✨

**Stack:** .NET 10 • MongoDB • Next.js 15 • TypeScript • Tailwind CSS v4
