# Sonixy Gateway - Smart Configuration

## 🎯 Auto Environment Detection

Gateway automatically loads the correct Ocelot configuration based on `ASPNETCORE_ENVIRONMENT`:

| Environment | Config File | Target |
|------------|-------------|---------|
| **Development** | `ocelot.Development.json` | `localhost:5008-5011` (host ports) |
| **Production** | `ocelot.Production.json` | `service-name:8088-8091` (Docker network) |

---

## 📁 Configuration Files

### `ocelot.Development.json`
```json
{
  "DownstreamHostAndPorts": [{
    "Host": "localhost",
    "Port": 5008  // Host port mapping
  }]
}
```

**Used when:**
- Running services individually on host machine
- Debugging with Visual Studio
- Local development without Docker

### `ocelot.Production.json`
```json
{
  "DownstreamHostAndPorts": [{
    "Host": "identity-service",  // Docker service name
    "Port": 8088  // Container internal port
  }]
}
```

**Used when:**
- Running in Docker Compose
- Production deployment
- All services in same Docker network

---

## 🔧 How It Works

```csharp
// Program.cs automatically selects config
var environment = builder.Environment.EnvironmentName;
var ocelotFileName = $"ocelot.{environment}.json";
builder.Configuration.AddJsonFile(ocelotFileName, ...);
```

**Environment is set by:**
```bash
# Docker Compose (docker-compose.yml)
ASPNETCORE_ENVIRONMENT=Development  # or Production

# Manual run
dotnet run --environment Development
```

---

## 🚀 Usage

### Local Development
```powershell
# Gateway loads ocelot.Development.json
cd Sonixy.Gateway/Api
dotnet run
# Routes to localhost:5008, 5009, 5010, 5011
```

### Docker Deployment
```powershell
# Gateway loads ocelot.Production.json (from docker-compose.yml)
cd be
.\deploy.ps1
# Routes to identity-service:8088, user-service:8089, etc.
```

---

## ✅ Benefits

✅ **No manual config changes** between environments
✅ **Same codebase** for dev and production
✅ **Type-safe** - compile-time errors if config missing
✅ **Environment-specific** routing automatically

---

## 🐛 Troubleshooting

**Config not found error:**
```
FileNotFoundException: ocelot.Development.json
```

**Solution:** Ensure both config files exist:
- `ocelot.Development.json` (localhost)
- `ocelot.Production.json` (Docker)

**Wrong config loaded:**
Check environment variable:
```powershell
# Windows
$env:ASPNETCORE_ENVIRONMENT

# In Docker
docker exec sonixy-gateway printenv ASPNETCORE_ENVIRONMENT
```

---

## 📝 Adding New Routes

Update **BOTH** files:
1. `ocelot.Development.json` - localhost config
2. `ocelot.Production.json` - Docker config

Keep route templates and settings identical, only change:
- `Host` (localhost vs service-name)
- `Port` (host port vs container port)
