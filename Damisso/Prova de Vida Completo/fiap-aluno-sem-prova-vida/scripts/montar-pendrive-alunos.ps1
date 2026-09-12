# Monta pasta + ZIP para pen drive (alunos).
# Uso: .\scripts\montar-pendrive-alunos.ps1

$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$outParent = Join-Path $root 'PARA-PENDRIVE'
$dst = Join-Path $outParent 'INSS-Mocambique-FIAP-2026'
$zip = Join-Path $outParent 'INSS-Mocambique-FIAP-2026.zip'

Write-Host "Origem: $root"
Write-Host "Destino: $dst"

if (Test-Path $outParent) {
  Remove-Item $outParent -Recurse -Force
}
New-Item -ItemType Directory -Path $dst -Force | Out-Null

$excludeDirs = @(
  '.git', 'node_modules', 'bin', 'obj', '.vs', 'dist',
  'playwright-report', 'test-results', 'TestResults',
  'PARA-PENDRIVE', '.cursor'
)

$xdArgs = @('/XD') + $excludeDirs
$rcArgs = @(
  $root, $dst, '/E', '/NFL', '/NDL', '/NJH', '/NJS', '/nc', '/ns', '/np'
) + $xdArgs + @('/XF', 'guia-professor.html', '.env')

& robocopy @rcArgs | Out-Null
if ($LASTEXITCODE -ge 8) {
  throw "robocopy falhou com codigo $LASTEXITCODE"
}

foreach ($p in @(
  (Join-Path $dst 'docs\guia-professor.html'),
  (Join-Path $dst 'docs\deploy-nuvem.md')
)) {
  if (Test-Path $p) { Remove-Item $p -Force }
}

$leiaMe = @'
================================================================================
  INSS Mocambique — projeto FIAP (G1 Full-Stack) · pacote para alunos
================================================================================

O que tem nesta pasta
  - projeto/api     → backend .NET (abrir no Visual Studio)
  - projeto/app     → frontend React (abrir no VS Code)
  - docs/           → apresentacoes (abra no browser)
  - scripts/        → iniciar ambiente (opcional)
  - README.md       → detalhes tecnicos

Como comecar (resumo)
  1) Copie ESTA pasta inteira do pen drive para o HD, por exemplo:
       C:\fiap-aluno
     (nao rode npm/dotnet de dentro do pen drive — e lento e falha facil)

  2) Pre-requisitos: .NET 10 SDK, Node.js 20+, Docker Desktop, VS + VS Code

  3) Banco:
       cd projeto\api
       docker compose up -d

  4) APIs (dois terminais ou multiplos startups no Visual Studio):
       dotnet run --project src\Contribuintes.Api    → http://localhost:5001
       dotnet run --project src\Beneficios.Api       → http://localhost:5002

  5) Front:
       cd projeto\app
       copy .env.example .env
       npm install
       npm run dev                                   → http://localhost:5173

  Login de teste: admin / 123

Slides (abra no Chrome/Edge)
  docs\apresentacao.html     → projeto / arquitetura
  docs\aulas-m13-m16.html    → conceitos M13 a M16
  (Use as setas ou PageDown/PageUp)

Importante
  - Este pacote NAO inclui node_modules nem bin/obj (voce gera com npm install / dotnet).
  - Precisa de internet na 1a vez (npm install e imagens Docker).
  - Demo na nuvem (se o professor indicar): https://fiap-kappa.vercel.app

================================================================================
'@
Set-Content -Path (Join-Path $dst 'LEIA-ME-ALUNOS.txt') -Value $leiaMe -Encoding UTF8

$readmePath = Join-Path $dst 'README.md'
if (Test-Path $readmePath) {
  $lines = Get-Content $readmePath
  $filtered = $lines | Where-Object { $_ -notmatch 'guia-professor' }
  $filtered = $filtered | ForEach-Object {
    if ($_ -match 'Abra no browser') {
      'Abra no browser. Use setas ou PageDown/PageUp.'
    } else {
      $_
    }
  }
  Set-Content -Path $readmePath -Value $filtered -Encoding UTF8
}

if (Test-Path $zip) { Remove-Item $zip -Force }
Compress-Archive -Path $dst -DestinationPath $zip -CompressionLevel Optimal

$sizeFolder = [math]::Round((Get-ChildItem $dst -Recurse -File -ErrorAction SilentlyContinue | Measure-Object Length -Sum).Sum / 1MB, 1)
$sizeZip = [math]::Round((Get-Item $zip).Length / 1MB, 1)

Write-Host ""
Write-Host "Pronto para o pen drive:"
Write-Host "  Pasta: $dst  (~$sizeFolder MB)"
Write-Host "  ZIP:   $zip  (~$sizeZip MB)"
Write-Host ""
Write-Host "Copie a pasta OU o ZIP para o pen drive."
Write-Host "Peca aos alunos: extrair/copiar para o HD antes de npm install."
