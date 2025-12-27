# Sonixy Backend - Start Services
# This script starts all Sonixy backend services using Docker Compose

Write-Host "🚀 Starting Sonixy Backend Services..." -ForegroundColor Cyan
Write-Host ""

# Check if Docker is running
try {
    docker info | Out-Null
    Write-Host "✅ Docker is running" -ForegroundColor Green
} catch {
    Write-Host "❌ Error: Docker is not running. Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# Navigate to backend directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

# Check if .env file exists
if (-not (Test-Path ".env")) {
    Write-Host "⚠️  Warning: .env file not found!" -ForegroundColor Yellow
    Write-Host "Creating .env with default values..." -ForegroundColor Yellow
    
    $envContent = @"
# Sonixy Backend Environment Variables
MONGO_PASSWORD=SonixyAdmin123!
JWT_SECRET=YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256ChangeThisInProduction
"@
    $envContent | Out-File -FilePath ".env" -Encoding UTF8
    Write-Host "✅ Created .env file with defaults" -ForegroundColor Green
    Write-Host "⚠️  IMPORTANT: Update .env with strong passwords before production!" -ForegroundColor Yellow
    Write-Host ""
}

Write-Host ""
Write-Host "🐳 Starting containers..." -ForegroundColor Yellow
Write-Host ""

# Start all services
docker-compose up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ All services started successfully!" -ForegroundColor Green
    Write-Host ""
    
    # Wait a bit for services to initialize
    Write-Host "⏳ Waiting for services to initialize..." -ForegroundColor Yellow
    Start-Sleep -Seconds 5
    
    Write-Host ""
    Write-Host "📊 Service Status:" -ForegroundColor Cyan
    docker-compose ps
    
    Write-Host ""
    Write-Host "🌐 Service URLs:" -ForegroundColor Cyan
    Write-Host "  Gateway:    http://localhost:5100" -ForegroundColor White
    Write-Host "  Identity:   http://localhost:5008" -ForegroundColor White
    Write-Host "  User:       http://localhost:5009" -ForegroundColor White
    Write-Host "  Post:       http://localhost:5010" -ForegroundColor White
    Write-Host "  Social:     http://localhost:5011" -ForegroundColor White
    Write-Host "  Notification: http://localhost:5014" -ForegroundColor White
    Write-Host "  MongoDB:    mongodb://localhost:27017" -ForegroundColor White
    
    Write-Host ""
    Write-Host "📝 Useful commands:" -ForegroundColor Yellow
    Write-Host "  View logs:     docker-compose logs -f" -ForegroundColor White
    Write-Host "  Stop services: docker-compose down" -ForegroundColor White
    Write-Host "  Restart:       docker-compose restart" -ForegroundColor White
    
} else {
    Write-Host ""
    Write-Host "❌ Failed to start services! Check the error messages above." -ForegroundColor Red
    exit 1
}
