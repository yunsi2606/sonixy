# Sonixy Backend - View Logs
# This script shows logs from all services

param(
    [Parameter(Mandatory=$false)]
    [string]$Service = "",
    
    [Parameter(Mandatory=$false)]
    [switch]$Follow = $false
)

Write-Host "📋 Viewing Sonixy Logs..." -ForegroundColor Cyan
Write-Host ""

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

if ($Service -eq "") {
    Write-Host "Showing logs from all services..." -ForegroundColor Yellow
    if ($Follow) {
        docker-compose logs -f
    } else {
        docker-compose logs --tail=100
    }
} else {
    Write-Host "Showing logs from $Service..." -ForegroundColor Yellow
    if ($Follow) {
        docker-compose logs -f $Service
    } else {
        docker-compose logs --tail=100 $Service
    }
}

Write-Host ""
Write-Host "Available services:" -ForegroundColor Cyan
Write-Host "  mongodb, identity-service, user-service, post-service, social-service, gateway" -ForegroundColor White
Write-Host ""
Write-Host "Usage examples:" -ForegroundColor Yellow
Write-Host "  .\deploy-logs.ps1                    # Show last 100 lines from all services" -ForegroundColor White
Write-Host "  .\deploy-logs.ps1 -Follow            # Follow all logs" -ForegroundColor White
Write-Host "  .\deploy-logs.ps1 -Service gateway   # Show gateway logs" -ForegroundColor White
