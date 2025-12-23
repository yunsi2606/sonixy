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
│   Gateway   │  → Ocelot API Gateway (Port 7200)
└─────────────┘
      ↓
┌────────────────────────────────────────────────────────┐
│  IdentityService  │  UserService  │  PostService       │
│  SocialGraphService │ AnalyticsService │ FeedService   │
└────────────────────────────────────────────────────────┘
      ↓
┌────────────────────────────────────────────────────────┐
│   MongoDB Cluster  │   Redis Cache   │    RabbitMQ     │
└────────────────────────────────────────────────────────┘
```

### Tech Stack

**Backend**
- .NET 10 + ASP.NET Core Web API
- **Data**: MongoDB (Time Series & Document), Redis (Sorted Sets), MinIO (Object Storage)
- **Messaging**: RabbitMQ (MassTransit)
- **Gateway**: Ocelot
- **Docs**: Swagger/OpenAPI
- **Patterns**: repository, Specification, Event-Driven Architecture (EDA)

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
# AnalyticsService
cd be/Sonixy.AnalyticsService
dotnet run
# Swagger: http://localhost:8092

# FeedService
cd be/Sonixy.FeedService
dotnet run
# Swagger: http://localhost:8093
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
│   ├── Sonixy.Shared/              # Events, DTOs, Common patterns
│   ├── Sonixy.AnalyticsService/    # User Behavior Logging (MongoDB/RabbitMQ)
│   ├── Sonixy.FeedService/         # Intelligent Feed (Redis/RabbitMQ)
│   ├── Sonixy.Gateway/             # Ocelot API Gateway
│   ├── Sonixy.UserService/         # User Profiles
│   ├── Sonixy.PostService/         # Content
│   └── Sonixy.SocialGraphService/  # Social Graph
│
└── fe/
```

...

## 🔮 Roadmap

### Phase 1: Core Complete ✅
- [x] Backend microservices scaffold
- [x] MongoDB integration
- [x] Cursor pagination
- [x] Frontend with design system

### Phase 2: V3 Architecture & Event-Driven ✅
- [x] Analytics Service (Mongo TimeSeries)
- [x] Feed Service (Redis Hybrid Fan-out)
- [x] RabbitMQ Integration (MassTransit)
- [x] API Gateway (Ocelot)

### Phase 3: Advanced Features
- [ ] Real-time updates (SignalR)
- [ ] Kubernetes deployment (Helm Charts)
- [ ] Recommendation Engine (ML.NET)

## 🤝 Contributing

This is a demonstration project showcasing modern web architecture patterns. Feel free to use it as a reference for your own projects.

## 📄 License

MIT License - See [LICENSE](LICENSE) for details

---

**Built with clean code, scalable architecture, and attention to detail** ✨

**Stack:** .NET 10 • MongoDB • Next.js 15 • TypeScript • Tailwind CSS v4
