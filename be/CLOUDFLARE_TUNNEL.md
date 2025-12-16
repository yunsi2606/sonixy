# Sonixy Backend - Cloudflare Tunnel in Docker

## 🚀 Quick Setup (Token Method - Recommended)

### Step 1: Create Tunnel in Cloudflare Dashboard

1. Go to Cloudflare Zero Trust Dashboard: https://one.dash.cloudflare.com/
2. Navigate to **Networks** → **Tunnels**
3. Click **Create a tunnel**
4. Choose **Cloudflared**
5. Name it: `sonixy-backend`
6. Click **Save tunnel**

### Step 2: Configure Tunnel

In the tunnel config page, add **Public Hostnames**:

| Public Hostname | Service | Type |
|----------------|---------|------|
| `api.yourdomain.com` | `http://gateway:8100` | HTTP |
| `identity.yourdomain.com` | `http://identity-service:8088` | HTTP |
| `user.yourdomain.com` | `http://user-service:8089` | HTTP |
| `post.yourdomain.com` | `http://post-service:8090` | HTTP |
| `social.yourdomain.com` | `http://social-service:8091` | HTTP |

**Important**: Use Docker service names (`gateway`, `identity-service`, etc.) NOT `localhost`!

### Step 3: Get Tunnel Token

1. After saving, Cloudflare will show installation instructions
2. Copy the **Tunnel Token** (starts with `eyJ...`)
3. It's in the command: `cloudflared tunnel run --token <YOUR_TOKEN>`

### Step 4: Add Token to `.env`

Edit `be/.env`:
```bash
# MongoDB
MONGO_PASSWORD=YourStrongPassword123!

# JWT
JWT_SECRET=YourSuperSecretKeyMinimum32Chars

# Cloudflare Tunnel Token (from Step 3)
CLOUDFLARE_TUNNEL_TOKEN=eyJhIjoiYWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXoxMjM0NTY3ODkwIiwidCI6IjEyMzQ1Njc4LWFiY2QtMTIzNC1hYmNkLTEyMzQ1Njc4OTBhYiIsInMiOiJhYmNkZWZnaGlqa2xtbm9wcXJzdHV2d3h5ejEyMzQ1Njc4OTAifQ==
```

### Step 5: Start Everything

```powershell
cd be
.\deploy.ps1
```

That's it! Cloudflare Tunnel runs automatically in Docker.

---

## 🌐 Your Services Are Now Public

After deployment, your services are accessible at:

- **Main API**: `https://api.yourdomain.com`
- **Identity Swagger**: `https://identity.yourdomain.com`
- **User Swagger**: `https://user.yourdomain.com`
- **Post Swagger**: `https://post.yourdomain.com`
- **Social Swagger**: `https://social.yourdomain.com`

All with **free SSL/TLS** and **DDoS protection**! 🎉

---

## 📊 Service Communication

```
Internet (HTTPS)
    │
    ▼
Cloudflare Tunnel (cloudflared container)
    │
    ▼
Docker Network (sonixy-network)
    │
    ├─► gateway:8100          (API Gateway)
    ├─► identity-service:8088  (Identity)
    ├─► user-service:8089      (User)
    ├─► post-service:8090      (Post)
    └─► social-service:8091    (Social)
```

All services communicate using Docker service names on the internal network.

---

## 🔧 Management

### View Tunnel Status

```powershell
# Check if tunnel is running
docker-compose ps cloudflared

# View tunnel logs
docker-compose logs -f cloudflared
```

### Restart Tunnel

```powershell
docker-compose restart cloudflared
```

### Update Tunnel Configuration

1. Go to Cloudflare Dashboard
2. Edit tunnel configuration
3. Changes apply immediately - no restart needed!

---

## 🛠️ Troubleshooting

### Issue: Tunnel not connecting

**Check logs:**
```powershell
docker-compose logs cloudflared
```

**Common solutions:**
1. Verify `CLOUDFLARE_TUNNEL_TOKEN` in `.env`
2. Check if services are running: `docker-compose ps`
3. Restart tunnel: `docker-compose restart cloudflared`

### Issue: 502 Bad Gateway

**Solutions:**
1. Ensure service names are correct in Cloudflare config
2. Use `gateway:8100` not `localhost:5100`
3. Check service is healthy: `docker-compose ps gateway`

### Issue: Token expired or invalid

**Get new token:**
1. Cloudflare Dashboard → Tunnels
2. Click your tunnel → Configure
3. Click "View tunnel credentials"
4. Copy new token to `.env`
5. Restart: `docker-compose restart cloudflared`

---

## 🔐 Security Configuration

### Update CORS for Public Access

Edit each service's `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",              // Local dev
            "https://yourdomain.com",             // Production frontend
            "https://www.yourdomain.com",         // WWW
            "https://api.yourdomain.com"          // API Gateway
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
```

Then rebuild:
```powershell
docker-compose down
.\deploy.ps1
```

---

## 🌍 Complete Production Setup

### 1. Deploy Backend
```powershell
# First time
cd be
.\deploy.ps1

# View logs
.\deploy-logs.ps1 -Follow
```

### 2. Verify Tunnel
```powershell
# Check tunnel is connected
docker-compose logs cloudflared

# Test public endpoint
curl https://api.yourdomain.com/api/identity/auth/validate?token=test
```

### 3. Deploy Frontend to Vercel

Update `fe/.env.production`:
```bash
NEXT_PUBLIC_API_URL=https://api.yourdomain.com
```

Deploy to Vercel:
```bash
cd fe
vercel --prod
```

---

## 📋 Environment Variables Reference

Complete `.env` file:

```bash
# MongoDB
MONGO_PASSWORD=YourStrongMongoPassword123!@#

# JWT Secret (minimum 32 characters)
JWT_SECRET=YourSuperSecretJWTKeyMinimum32CharactersLongForProduction!@#$

# Cloudflare Tunnel Token (get from Cloudflare Dashboard)
CLOUDFLARE_TUNNEL_TOKEN=eyJhIjoiYWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXoxMjM0NTY3ODkwIiwidCI6IjEyMzQ1Njc4LWFiY2QtMTIzNC1hYmNkLTEyMzQ1Njc4OTBhYiIsInMiOiJhYmNkZWZnaGlqa2xtbm9wcXJzdHV2d3h5ejEyMzQ1Njc4OTAifQ==
```

---

## 🎯 Advantages of Docker Tunnel

✅ **Single Command Deployment**: Everything in one `docker-compose up`
✅ **Automatic Restarts**: Tunnel restarts with containers
✅ **Easy Management**: Standard Docker commands
✅ **Network Isolation**: Tunnel connects via Docker network
✅ **No Windows Service**: Cleaner, more portable
✅ **Logs Integration**: Tunnel logs with other services

---

## 📚 Advanced: Multiple Environments

### Development
```yaml
# docker-compose.dev.yml
services:
  cloudflared:
    command: tunnel --no-autoupdate run --token ${CLOUDFLARE_TUNNEL_TOKEN_DEV}
```

### Production
```yaml
# docker-compose.prod.yml (override)
services:
  cloudflared:
    command: tunnel --no-autoupdate run --token ${CLOUDFLARE_TUNNEL_TOKEN_PROD}
```

Run with:
```powershell
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

---

## 🔄 Updating Services

When you update code:

```powershell
# Stop all
docker-compose down

# Rebuild
.\deploy-build.ps1

# Start with tunnel
.\deploy-start.ps1
```

Tunnel automatically reconnects when gateway restarts!

---

## 📞 Quick Reference

| Command | Description |
|---------|-------------|
| `.\deploy.ps1` | Build and start everything (including tunnel) |
| `docker-compose ps` | Check all services status |
| `docker-compose logs cloudflared` | View tunnel logs |
| `docker-compose restart cloudflared` | Restart tunnel only |
| `.\deploy-logs.ps1 -Follow` | Follow all logs |
| `.\deploy-stop.ps1` | Stop all services |

---

## ✅ Checklist

- [ ] Created tunnel in Cloudflare Dashboard
- [ ] Configured public hostnames with Docker service names
- [ ] Copied tunnel token to `.env`
- [ ] Updated CORS in all services
- [ ] Ran `.\deploy.ps1`
- [ ] Verified tunnel connection: `docker-compose logs cloudflared`
- [ ] Tested public endpoint: `curl https://api.yourdomain.com`
- [ ] Deployed frontend to Vercel
- [ ] Updated frontend `NEXT_PUBLIC_API_URL`

**🎉 Done! Your Sonixy backend is now live!**
