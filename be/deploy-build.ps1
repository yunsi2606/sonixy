# Sonixy Backend - Build Docker Images
# This script builds all Docker images for the Sonixy backend services

Write-Host "🚀 Building Sonixy Backend Docker Images..." -ForegroundColor Cyan
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

Write-Host ""
Write-Host "📦 Building images..." -ForegroundColor Yellow
Write-Host ""

# Build all images using docker-compose
docker-compose build --parallel

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ All images built successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📊 Built images:" -ForegroundColor Cyan
    docker images | Select-String "sonixy"
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "  1. Run: .\deploy-start.ps1 to start all services" -ForegroundColor White
    Write-Host "  2. Or run: docker-compose up -d" -ForegroundColor White
} else {
    Write-Host ""
    Write-Host "❌ Build failed! Check the error messages above." -ForegroundColor Red
    exit 1
}
