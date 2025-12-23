# Sonixy Backend - Docker Deployment Guide

## 🚀 Quick Start

### Prerequisites
- Docker Desktop installed and running
- PowerShell (Windows)
- 8GB RAM minimum
- 20GB free disk space

### One-Command Deployment
```powershell
cd be
.\deploy.ps1
```

This will:
1. ✅ Build all Docker images
2. ✅ Create `.env` file (if not exists)
3. ✅ Start all services (Gateway + 4 microservices + MongoDB)
4. ✅ Display service URLs and status

---

## 📦 Available Scripts

| Script | Description |
|--------|-------------|
| `deploy.ps1` | **Complete deployment** - Build & start everything |
| `deploy-build.ps1` | Build all Docker images |
| `deploy-start.ps1` | Start all services |
| `deploy-stop.ps1` | Stop all services (preserves data) |
| `deploy-logs.ps1` | View service logs |

### Usage Examples

**Build images only:**
```powershell
.\deploy-build.ps1
```

**Start services:**
```powershell
.\deploy-start.ps1
```

**View logs (all services):**
```powershell
.\deploy-logs.ps1
```

**View logs (specific service, follow mode):**
```powershell
.\deploy-logs.ps1 -Service gateway -Follow
```

**Stop services:**
```powershell
.\deploy-stop.ps1
```

**Stop and delete all data:**
```powershell
docker-compose down -v  # ⚠️ WARNING: Deletes all database data!
```

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────┐
│           Docker Network (sonixy-network)       │
├─────────────────────────────────────────────────┤
│                                                 │
│  ┌─────────────┐                                │
│  │   MongoDB   │ :27017                         │
│  └──────┬──────┘                                │
│         │                                       │
│  ┌──────┴───────┬──────────┬──────────┬──────────┬──────────┐ │
│  │              │          │          │          │          │ │
│  ▼              ▼          ▼          ▼          ▼          ▼ │
│ ┌────┐      ┌────┐    ┌────┐    ┌────┐    ┌────┐    ┌────┐    │
│ │ID  │:5000 │User│    │Post│    │Soc │    │Ana │    │Feed│    │
│ │Svc │      │Svc │    │Svc │    │Svc │    │Svc │    │Svc │    │
│ └──┬─┘      └──┬─┘    └──┬─┘    └──┬─┘    └──┬─┘    └──┬─┘    │
│    │           │         │         │          │          │      │
│    └───────────┴─────────┴─────────┴──────────┴──────────┘      │
│                │                                                │
│         ┌──────▼──────┐                                         │
│         │   Gateway   │ :7200                                   │
│         └─────────────┘                                         │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
              │
              ▼
     (External Access)
```

---

## 🔧 Service Configuration

### Ports

| Service | Container Port | Host Port | URL |
|---------|---------------|-----------|-----|
| Gateway | 8100 | 5100 | http://localhost:5100 |
| Identity | 8088 | 5008 | http://localhost:5008 |
| User | 8089 | 5009 | http://localhost:5009 |
| Post | 8090 | 5010 | http://localhost:5010 |
| Social | 8091 | 5011 | http://localhost:5011 |
| Analytics | 8092 | 5012 | http://localhost:5012 |
| Feed | 8093 | 5013 | http://localhost:5013 |
| MongoDB | 27017 | 27017 | mongodb://localhost:27017 |
| Redis | 6379 | 6379 | redis://localhost:6379 |
| RabbitMQ | 5672/15672 | 5673/15673 | http://localhost:15673 |
| MinIO (API) | 9000 | 9100 | http://localhost:9100 |
| MinIO (Console) | 9001 | 9102 | http://localhost:9102 |
| Cloudflared | N/A | N/A | (Tunnel - no exposed ports) |

### Environment Variables

Edit `.env` file:
```bash
# MongoDB
MONGO_PASSWORD=YourStrongPassword123!

# JWT (minimum 32 characters)
JWT_SECRET=YourSuperSecretKeyForJWTTokensMinimum32Chars
```

⚠️ **IMPORTANT**: Change these before production!

---

## 📊 Swagger Documentation

Each service has Swagger UI:

- **Identity**: http://localhost:5008 - Auth, JWT tokens
- **User**: http://localhost:5009 - User profiles
- **Post**: http://localhost:5010 - Posts, feed
- **Social**: http://localhost:5011 - Follow, likes
- **Analytics**: http://localhost:5012 - Event Logging
- **Feed**: http://localhost:5013 - Personalized Timeline
- **Gateway**: http://localhost:5100 - Main API endpoint

---

## 🐳 Docker Commands Reference

### Container Management

```powershell
# List running containers
docker-compose ps

# View all containers (including stopped)
docker ps -a

# Restart specific service
docker-compose restart gateway

# Rebuild specific service
docker-compose build gateway
docker-compose up -d gateway

# Execute command in container
docker-compose exec gateway sh
```

### Logs and Debugging

```powershell
# Follow logs (all services)
docker-compose logs -f

# Last 100 lines from specific service
docker-compose logs --tail=100 mongodb

# Save logs to file
docker-compose logs > deployment-logs.txt
```

### Data Management

```powershell
# List volumes
docker volume ls

# Inspect volume
docker volume inspect be_mongodb_data

# Backup MongoDB data
docker-compose exec mongodb mongodump --archive > backup.archive

# Restore MongoDB data
docker-compose exec -T mongodb mongorestore --archive < backup.archive
```

### Resource Monitoring

```powershell
# View resource usage
docker stats

# Disk usage
docker system df

# Clean up unused images/containers
docker system prune -a
```

---

## 🔍 Troubleshooting

### Issue: Port already in use

**Error**: `Bind for 0.0.0.0:5050 failed: port is already allocated`

**Solution**:
```powershell
# Find process using port
netstat -ano | findstr :5050

# Kill process (replace PID)
taskkill /PID <PID> /F

# Or change port in docker-compose.yml
```

### Issue: MongoDB connection failed

**Symptoms**: Services crash with "MongoConnectionException"

**Solutions**:
```powershell
# 1. Check MongoDB is running
docker-compose ps mongodb

# 2. View MongoDB logs
docker-compose logs mongodb

# 3. Restart MongoDB
docker-compose restart mongodb

# 4. Wait for health check
docker-compose ps  # Wait for "healthy" status
```

### Issue: Build fails

**Solutions**:
```powershell
# 1. Clean Docker cache
docker builder prune -a

# 2. Remove old images
docker rmi $(docker images -q sonixy*)

# 3. Rebuild from scratch
.\deploy-build.ps1
```

### Issue: Out of memory

**Solutions**:
1. Increase Docker Desktop memory (Settings → Resources)
2. Close unnecessary applications
3. Stop and remove unused containers:
```powershell
docker system prune -a
```

---

## 💾 Data Persistence

### Docker Volumes

Data is stored in Docker volumes and persists between container restarts:

- `be_mongodb_data` - MongoDB database files
- `be_mongodb_config` - MongoDB configuration

### Backup Data

```powershell
# Backup MongoDB
docker-compose exec mongodb mongodump --archive > sonixy-backup-$(Get-Date -Format 'yyyy-MM-dd').archive

# Backup volumes (manual)
docker run --rm -v be_mongodb_data:/data -v ${PWD}:/backup alpine tar czf /backup/mongodb-data-backup.tar.gz /data
```

### Restore Data

```powershell
# Restore MongoDB from archive
docker-compose exec -T mongodb mongorestore --archive < sonixy-backup-2024-12-15.archive
```

---

## 🔐 Security Best Practices

### Before Production

1. **Change default passwords**:
   ```bash
   # .env
   MONGO_PASSWORD=<strong-random-password>
   JWT_SECRET=<minimum-32-character-random-string>
   ```

2. **Generate strong JWT secret**:
   ```powershell
   # PowerShell - Generate 64-char random string
   -join ((48..57) + (65..90) + (97..122) | Get-Random -Count 64 | ForEach-Object {[char]$_})
   ```

3. **Enable MongoDB authentication** (production):
   - Current setup uses simple password
   - For production: Use strong user/password per database

4. **Use HTTPS** (with Cloudflare Tunnel)

5. **Limit CORS origins** (update in each service `Program.cs`):
   ```csharp
   policy.WithOrigins("https://yourdomain.com")
   ```

---

## 🌍 Production Deployment

See `CLOUDFLARE_TUNNEL.md` for:
- Exposing backend to internet
- SSL/TLS setup
- Domain configuration
- Cloudflare protection

---

## 📈 Scaling

### Horizontal Scaling

```yaml
# docker-compose.yml
services:
  user-service:
    # ... existing config ...
    deploy:
      replicas: 3  # Run 3 instances
```

### Load Balancing

For production, add Nginx/Traefik:
```yaml
services:
  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
    # Configure load balancing to service replicas
```

---

## 🧪 Testing Deployment

```powershell
# 1. Test Gateway health
curl http://localhost:5100/api/identity/auth/validate?token=test

# 2. Test MongoDB connection
docker-compose exec mongodb mongosh -u admin -p $env:MONGO_PASSWORD

# 3. View service status
docker-compose ps

# 4. Check logs for errors
.\deploy-logs.ps1
```

---

## 📞 Support

- **Issues**: Create issue in GitHub repository
- **Logs**: Always include `.\deploy-logs.ps1` output
- **Status**: Include `docker-compose ps` output

---

## 🎯 Next Steps

1. ✅ Deploy backend locally: `.\deploy.ps1`
2. ✅ Test all services work
3. ✅ Setup Cloudflare Tunnel: See `CLOUDFLARE_TUNNEL.md`
4. ✅ Deploy frontend to Vercel
5. ✅ Update frontend `NEXT_PUBLIC_API_URL` to your domain
6. ✅ Go live! 🚀
