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
  LEIA-ME — pacote alunos · INSS Mocambique · FIAP
================================================================================

Comece por este arquivo:
  PASSO-A-PASSO.txt

Resumo rapido:
  1) Copie esta pasta para o HD (ex.: C:\fiap-aluno) — nao use o pen drive
  2) Verifique: dotnet, node, npm, docker, git
  3) Docker Desktop aberto
  4) projeto\api → docker compose up -d
  5) Duas APIs: Contribuintes (:5001) e Beneficios (:5002)
  6) projeto\app → copy .env.example .env → npm install → npm run dev
  7) http://localhost:5173 · login admin / 123

Pastas:
  projeto\api   → Visual Studio (InssModernizacao.slnx)
  projeto\app   → VS Code + npm
  docs\         → slides no browser

Detalhes tecnicos: README.md
================================================================================
'@
Set-Content -Path (Join-Path $dst 'LEIA-ME-ALUNOS.txt') -Value $leiaMe -Encoding UTF8

$passo = @'
================================================================================
  PASSO A PASSO — a partir da SUA pasta do projeto
  INSS Mocambique · FIAP · G1 Full-Stack
================================================================================

ANTES DE TUDO
  - Extraia / copie o ZIP do pen drive para o HD, por exemplo:
      C:\fiap-aluno
  - Trabalhe SEMPRE nessa pasta do HD (nao no pen drive).
  - Abra o PowerShell e va ate a pasta:

      cd C:\fiap-aluno

  (Se a pasta tiver outro nome, use o caminho real dela.)


PASSO 0 — Verificar ferramentas (um comando de cada vez)
--------------------------------------------------------------------------------
  dotnet --version
  node --version
  npm --version
  docker --version
  wsl --status
  git --version

  OK = aparece o numero da versao.
  "nao e reconhecido" = falta instalar essa ferramenta (peca ajuda ao professor).

  Se o npm reclamar de "execucao de scripts desabilitada":
    Set-ExecutionPolicy -Scope CurrentUser RemoteSigned
  Feche e abra o PowerShell; teste npm --version de novo.


PASSO 1 — Abrir o Docker Desktop
--------------------------------------------------------------------------------
  Abra o Docker Desktop e espere ficar "Running" / pronto.


PASSO 2 — Subir o banco (Postgres)
--------------------------------------------------------------------------------
  cd C:\fiap-aluno\projeto\api
  docker compose up -d
  docker compose ps

  Deve aparecer o container em estado running.


PASSO 3 — Subir a API Contribuintes (porta 5001)
--------------------------------------------------------------------------------
  No PowerShell (pasta api):

  cd C:\fiap-aluno\projeto\api
  dotnet run --project src\Contribuintes.Api

  Deixe esse terminal ABERTO.
  Teste no browser: http://localhost:5001


PASSO 4 — Subir a API Beneficios (porta 5002)
--------------------------------------------------------------------------------
  Abra OUTRO PowerShell:

  cd C:\fiap-aluno\projeto\api
  dotnet run --project src\Beneficios.Api

  Deixe esse terminal ABERTO.
  Teste no browser: http://localhost:5002

  (No Visual Studio: abra InssModernizacao.slnx e rode as duas APIs.)


PASSO 5 — Subir o portal React (porta 5173)
--------------------------------------------------------------------------------
  Abra OUTRO PowerShell:

  cd C:\fiap-aluno\projeto\app
  copy .env.example .env
  npm install
  npm run dev

  Portal: http://localhost:5173
  Login:  admin   /   123


PASSO 6 — Conferir se esta tudo ok
--------------------------------------------------------------------------------
  [ ] Docker rodando
  [ ] http://localhost:5001 responde
  [ ] http://localhost:5002 responde
  [ ] http://localhost:5173 abre o portal
  [ ] Login admin / 123 funciona
  [ ] Lista de contribuintes aparece


SLIDES (opcional — no browser)
--------------------------------------------------------------------------------
  C:\fiap-aluno\docs\apresentacao.html
  C:\fiap-aluno\docs\aulas-m13-m16.html

  Setas ou PageDown / PageUp para trocar de slide.


ORDEM FIXA (nao inverta)
--------------------------------------------------------------------------------
  1) banco (Docker)  →  2) APIs  →  3) front (npm run dev)


PROBLEMAS COMUNS
--------------------------------------------------------------------------------
  Portal nao carrega dados
    → as APIs estao no ar? (:5001 e :5002)

  docker compose falha
    → Docker Desktop esta aberto?

  npm install lento / erro
    → precisa de internet na primeira vez

  Porta em uso
    → feche outro programa usando 5001, 5002 ou 5173


================================================================================
  Em sala: siga o que o professor pedir no checklist e nas demos.
================================================================================
'@
Set-Content -Path (Join-Path $dst 'PASSO-A-PASSO.txt') -Value $passo -Encoding UTF8

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
