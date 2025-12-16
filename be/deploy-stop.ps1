# Sonixy Backend - Stop Services
# This script stops all Sonixy backend services

Write-Host "🛑 Stopping Sonixy Backend Services..." -ForegroundColor Cyan
Write-Host ""

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

# Stop all services
docker-compose down

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ All services stopped successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "💾 Data preserved in Docker volumes" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To remove volumes (DELETE ALL DATA):" -ForegroundColor Red
    Write-Host "  docker-compose down -v" -ForegroundColor White
} else {
    Write-Host ""
    Write-Host "❌ Failed to stop services!" -ForegroundColor Red
    exit 1
}
