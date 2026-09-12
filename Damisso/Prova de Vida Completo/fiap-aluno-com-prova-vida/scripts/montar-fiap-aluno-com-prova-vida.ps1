# Monta PARA-PENDRIVE\fiap-aluno-com-prova-vida (+ ZIP).
# Uso: .\scripts\montar-fiap-aluno-com-prova-vida.ps1

$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$outParent = Join-Path $root 'PARA-PENDRIVE'
$dst = Join-Path $outParent 'fiap-aluno-com-prova-vida'
$zip = Join-Path $outParent 'fiap-aluno-com-prova-vida.zip'

Write-Host "Origem: $root"
Write-Host "Destino: $dst"

if (Test-Path $dst) { Remove-Item $dst -Recurse -Force }
New-Item -ItemType Directory -Path $dst -Force | Out-Null

$excludeDirs = @(
  '.git', 'node_modules', 'bin', 'obj', '.vs', 'dist',
  'playwright-report', 'test-results', 'TestResults',
  'PARA-PENDRIVE', '.cursor', '.expo', 'android', 'ios'
)
$xdArgs = @('/XD') + $excludeDirs
$rcArgs = @(
  $root, $dst, '/E', '/NFL', '/NDL', '/NJH', '/NJS', '/nc', '/ns', '/np'
) + $xdArgs + @(
  '/XF', 'guia-professor.html', '.env',
  'eas-log.txt', 'eas-log2.bin', 'eas-decoded.txt', 'eas-log-decoded.txt',
  'eas-build.json', 'qr.jpg'
)

& robocopy @rcArgs | Out-Null
if ($LASTEXITCODE -ge 8) { throw "robocopy falhou com codigo $LASTEXITCODE" }

foreach ($p in @(
  (Join-Path $dst 'docs\guia-professor.html'),
  (Join-Path $dst 'docs\deploy-nuvem.md')
)) {
  if (Test-Path $p) { Remove-Item $p -Force }
}

$leiaMe = @'
================================================================================
  LEIA-ME — pacote COM Prova de Vida · INSS Moçambique · FIAP
================================================================================

Evolução do projeto:
  fiap-aluno-sem-prova-vida  →  portal + Contribuintes + Benefícios
  fiap-aluno-com-prova-vida  →  o mesmo + ProvaVida.Api + app-mobile + webcam

Copie para o HD, ex.: C:\fiap-aluno-com-prova-vida
NÃO trabalhem no pen drive.

Comece por:
  PASSO-A-PASSO.txt
  docs\roteiro-prova-vida.html
  docs\slides-prova-vida.html

NUIT demo: 300400504 (António Idoso Sitoe)
================================================================================
'@

$passo = @'
================================================================================
  PASSO A PASSO — COM Prova de Vida
================================================================================

cd C:\fiap-aluno-com-prova-vida

1) Docker
   cd projeto\api
   docker compose up -d

2) Contribuintes :5001
   $env:ASPNETCORE_ENVIRONMENT="Development"
   dotnet run --project src\Contribuintes.Api --no-launch-profile --urls http://0.0.0.0:5001

3) ProvaVida :5003
   (outro terminal)
   dotnet run --project src\ProvaVida.Api --no-launch-profile --urls http://0.0.0.0:5003

4A) Webcam
   cd docs
   npx --yes serve . -p 5180 --no-clipboard
   http://localhost:5180/prova-vida-webcam.html

4B) APK Android 1.0.1+ · mesma Wi‑Fi · IP no app.json

Roteiro rico: docs\roteiro-prova-vida.html
================================================================================
'@

Set-Content -Path (Join-Path $dst 'LEIA-ME-ALUNOS.txt') -Value $leiaMe -Encoding UTF8
Set-Content -Path (Join-Path $dst 'PASSO-A-PASSO.txt') -Value $passo -Encoding UTF8

$readmePath = Join-Path $dst 'README.md'
if (Test-Path $readmePath) {
  $lines = Get-Content $readmePath | Where-Object { $_ -notmatch 'guia-professor' }
  Set-Content -Path $readmePath -Value $lines -Encoding UTF8
}

if (Test-Path $zip) { Remove-Item $zip -Force }
Compress-Archive -Path $dst -DestinationPath $zip -CompressionLevel Optimal

$sizeFolder = [math]::Round((Get-ChildItem $dst -Recurse -File -ErrorAction SilentlyContinue | Measure-Object Length -Sum).Sum / 1MB, 1)
$sizeZip = [math]::Round((Get-Item $zip).Length / 1MB, 1)
Write-Host "Pasta: $dst (~$sizeFolder MB)"
Write-Host "ZIP:   $zip (~$sizeZip MB)"
