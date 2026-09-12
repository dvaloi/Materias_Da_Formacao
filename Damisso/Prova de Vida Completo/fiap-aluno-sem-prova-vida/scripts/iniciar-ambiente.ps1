# Scripts para iniciar o ambiente de desenvolvimento

Write-Host "=== INSS Modernizacao — Iniciar Ambiente ===" -ForegroundColor Green

$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$api = Join-Path $root "projeto\api"
$app = Join-Path $root "projeto\app"

Set-Location $api

# 1) Banco PostgreSQL (Docker) — fica junto da API
if (Get-Command docker -ErrorAction SilentlyContinue) {
    Write-Host "Subindo PostgreSQL (docker compose)..." -ForegroundColor Yellow
    docker compose up -d
    Start-Sleep -Seconds 3
} else {
    Write-Host "Docker nao encontrado. Instale o Docker Desktop e rode: docker compose up -d (em projeto/api)" -ForegroundColor Red
}

# 2) APIs (Visual Studio / dotnet) + Frontend (VS Code / npm)
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$api'; dotnet run --project src/Contribuintes.Api"
Start-Sleep -Seconds 2
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$api'; dotnet run --project src/Beneficios.Api"
Start-Sleep -Seconds 2
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$app'; if (-not (Test-Path node_modules)) { npm install }; npm run dev"

Write-Host ""
Write-Host "API (Visual Studio):  abrir projeto\api\InssModernizacao.slnx" -ForegroundColor Yellow
Write-Host "App (VS Code):        abrir pasta projeto\app" -ForegroundColor Yellow
Write-Host ""
Write-Host "Portal: http://localhost:5173" -ForegroundColor Cyan
Write-Host "Login: admin@inss.co.mz / Admin@123" -ForegroundColor Cyan
Write-Host "Postgres: localhost:5432 (inss / inss123)" -ForegroundColor Cyan
Write-Host "Consultas: docs/consultas-sql.md" -ForegroundColor Cyan
