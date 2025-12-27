# Sonixy Backend - Complete Deployment
# This script builds and starts all services in one go

Write-Host "[INFO] Sonixy Backend - Complete Deployment" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Check Docker
try {
    docker info | Out-Null
    Write-Host "[OK] Docker is running" -ForegroundColor Green
} catch {
    Write-Host "[WARN] Docker is not running. Starting Docker Desktop..." -ForegroundColor Yellow
    Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe"
    Write-Host "[INFO] Waiting for Docker to start (30 seconds)..." -ForegroundColor Yellow
    Start-Sleep -Seconds 30
    
    try {
        docker info | Out-Null
        Write-Host "[OK] Docker started successfully" -ForegroundColor Green
    } catch {
        Write-Host "[ERROR] Failed to start Docker. Please start Docker Desktop manually." -ForegroundColor Red
        exit 1
    }
}

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

Write-Host ""
Write-Host "[INFO] Step 1: Building Docker images..." -ForegroundColor Yellow
Write-Host ""

docker compose build

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "[OK] Build completed!" -ForegroundColor Green
Write-Host ""
Write-Host "[INFO] Step 2: Starting services..." -ForegroundColor Yellow
Write-Host ""

# Check/create .env
if (-not (Test-Path ".env")) {
    $envContent = @(
        "MONGO_PASSWORD=SonixyAdmin123!",
        "JWT_SECRET=YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256ChangeThisInProduction"
    )

    $envContent | Out-File -FilePath ".env" -Encoding utf8
    Write-Host "Created .env file" -ForegroundColor Green
}

docker compose up -d

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Failed to start services!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "[INFO] Waiting for services to initialize..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host ""
Write-Host "[INFO] Service Status:" -ForegroundColor Cyan
docker compose ps

Write-Host ""
Write-Host "[OK] Deployment Complete!" -ForegroundColor Green
Write-Host ""
Write-Host "[INFO] Service URLs:" -ForegroundColor Cyan
Write-Host "  API Gateway:    http://localhost:5100" -ForegroundColor White
Write-Host "  Swagger Docs:" -ForegroundColor Cyan
Write-Host "    Identity:     http://localhost:5008" -ForegroundColor White
Write-Host "    User:         http://localhost:5009" -ForegroundColor White
Write-Host "    Post:         http://localhost:5010" -ForegroundColor White
Write-Host "    Social:       http://localhost:5011" -ForegroundColor White
    Write-Host "    Analytics:    http://localhost:5012" -ForegroundColor White
    Write-Host "    Feed:         http://localhost:5013" -ForegroundColor White
    Write-Host "    Notification: http://localhost:5014" -ForegroundColor White
    Write-Host "    Email:        (Background Worker)" -ForegroundColor Gray
Write-Host ""
Write-Host "[INFO] Management Commands:" -ForegroundColor Yellow
Write-Host "  View logs:      .\deploy-logs.ps1 -Follow" -ForegroundColor White
Write-Host "  Stop services:  .\deploy-stop.ps1" -ForegroundColor White
Write-Host "  Restart:        docker-compose restart" -ForegroundColor White
Write-Host ""
Write-Host "[INFO] Next: Setup Cloudflare Tunnel - See CLOUDFLARE_TUNNEL.md" -ForegroundColor Cyan
