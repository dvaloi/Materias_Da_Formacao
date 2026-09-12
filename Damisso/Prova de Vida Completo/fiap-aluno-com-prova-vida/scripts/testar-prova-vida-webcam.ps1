# Testa Prova de Vida com webcam do PC (liveness MediaPipe) - sem emulador.
# Uso: .\scripts\testar-prova-vida-webcam.ps1

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$apiDir = Join-Path $root "projeto\api"
$portWeb = 5180

function Test-Api($url) {
    try {
        $r = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 3
        return $r.StatusCode -eq 200
    } catch { return $false }
}

Write-Host "=== Prova de Vida - teste webcam (PC) ===" -ForegroundColor Cyan

$docker = docker ps --filter "name=inss-postgres" --format "{{.Names}}" 2>$null
if (-not $docker) {
    Write-Host "Subindo Postgres..." -ForegroundColor Yellow
    Push-Location $apiDir
    docker compose up -d
    Pop-Location
    Start-Sleep -Seconds 3
}

if (-not (Test-Api "http://localhost:5001/api/contribuintes/pensionistas")) {
    Write-Host "Iniciando Contribuintes.Api (:5001)..." -ForegroundColor Yellow
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$apiDir'; dotnet run --project src\Contribuintes.Api"
    Start-Sleep -Seconds 8
}

if (-not (Test-Api "http://localhost:5003/openapi/v1.json")) {
    Write-Host "Iniciando ProvaVida.Api (:5003)..." -ForegroundColor Yellow
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$apiDir'; dotnet run --project src\ProvaVida.Api"
    Start-Sleep -Seconds 6
}

if (-not (Test-Api "http://localhost:5001/api/contribuintes/pensionistas")) {
    Write-Host "ERRO: API :5001 nao respondeu." -ForegroundColor Red
    exit 1
}

Push-Location (Join-Path $root "docs")
Start-Process powershell -ArgumentList "-NoExit", "-Command", "npx --yes serve . -p $portWeb --no-clipboard"
Pop-Location
Start-Sleep -Seconds 5

$url = "http://localhost:$portWeb/prova-vida-webcam.html"
Write-Host ""
Write-Host "Abra no Chrome/Edge:" -ForegroundColor Green
Write-Host $url -ForegroundColor White
Write-Host "NUIT demo: 300400501" -ForegroundColor Cyan

Start-Process $url
