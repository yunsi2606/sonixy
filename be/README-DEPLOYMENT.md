# Sonixy Backend - Complete Deployment Summary

## 🚀 Quick Start Commands

```powershell
# First Time Setup
cd be
cp env.template .env
# Edit .env with your values
.\deploy.ps1

# Daily Operations
.\deploy-start.ps1  # Start services
.\deploy-stop.ps1   # Stop services
.\deploy-logs.ps1   # View logs
```

---

## 📦 What's Included

### Docker Services
- ✅ MongoDB (database)
- ✅ IdentityService (auth + JWT)
- ✅ UserService (profiles)
- ✅ PostService (posts + feed)
- ✅ SocialGraphService (follow + likes)
- ✅ API Gateway (Ocelot routing)
- ✅ Cloudflare Tunnel (public access)

### Scripts
- `deploy.ps1` - Complete build + start
- `deploy-build.ps1` - Build images only
- `deploy-start.ps1` - Start services
- `deploy-stop.ps1` - Stop services
- `deploy-logs.ps1` - View logs

---

## 🌐 Service Ports

| Service | Host Port | Container Port |
|---------|-----------|----------------|
| Gateway | 5100 | 8100 |
| Identity | 5008 | 8088 |
| User | 5009 | 8089 |
| Post | 5010 | 8090 |
| Social | 5011 | 8091 |
| MongoDB | 27017 | 27017 |

---

## 🔐 Environment Variables

Required in `.env`:

```bash
# MongoDB
MONGO_PASSWORD=<strong-password>

# JWT Secret (32+ chars)
JWT_SECRET=<minimum-32-character-secret>

# Cloudflare Tunnel Token
CLOUDFLARE_TUNNEL_TOKEN=<token-from-cloudflare-dashboard>
```

---

## 📚 Documentation

- **DEPLOYMENT.md** - Complete Docker deployment guide
- **CLOUDFLARE_TUNNEL.md** - Public access setup
- **BACKEND_GUIDE.md** - API usage and testing

---

## 🎯 Production Checklist

- [ ] Update `.env` with strong passwords
- [ ] Setup Cloudflare Tunnel (get token)
- [ ] Configure public hostnames in Cloudflare
- [ ] Update CORS in service `Program.cs` files
- [ ] Test all services: `.\deploy.ps1`
- [ ] Verify tunnel: `docker-compose logs cloudflared`
- [ ] Deploy frontend to Vercel
- [ ] Point frontend to `https://api.yourdomain.com`

---

## 🛠️ Troubleshooting

**Services won't start?**
```powershell
docker-compose down -v  # Clean slate
.\deploy.ps1           # Rebuild
```

**Tunnel not connecting?**
```powershell
docker-compose logs cloudflared  # Check logs
# Verify CLOUDFLARE_TUNNEL_TOKEN in .env
```

**Check service health:**
```powershell
docker-compose ps      # All should be "Up (healthy)"
curl http://localhost:5100  # Test gateway
```

---

## 🎉 Success Indicators

✅ `docker-compose ps` shows all services healthy
✅ Gateway responds at http://localhost:5100
✅ Swagger docs accessible at service ports
✅ Cloudflared shows "Connected" in logs
✅ Public URL works: `https://api.yourdomain.com`

**You're live! 🚀**
