# INSS MoÃ§ambique â€” ModernizaÃ§Ã£o Full-Stack

Projeto didÃ¡tico para o curso **G1 â€” Programadores Full-Stack (INSS MoÃ§ambique)**, FIAP 2026.

## Estrutura (API Ã— App)

```
fiap/
â”œâ”€â”€ docs/                         # ApresentaÃ§Ãµes das aulas
â”œâ”€â”€ scripts/
â”œâ”€â”€ projeto/
â”‚   â”œâ”€â”€ api/                      # BACKEND â†’ abrir no Visual Studio
â”‚   â”‚   â”œâ”€â”€ InssModernizacao.slnx
â”‚   â”‚   â”œâ”€â”€ src/                  # MicrosserviÃ§os .NET + DDD
â”‚   â”‚   â”œâ”€â”€ tests/
â”‚   â”‚   â”œâ”€â”€ docker-compose.yml    # PostgreSQL
â”‚   â”‚   â””â”€â”€ docker/
â”‚   â””â”€â”€ app/                      # FRONTEND â†’ abrir no VS Code
â”‚       â”œâ”€â”€ src/                  # React (Vite)
â”‚       â”œâ”€â”€ e2e/                  # Playwright
â”‚       â””â”€â”€ package.json
â””â”€â”€ README.md
```

| Pasta | Ferramenta | ConteÃºdo |
|--------|------------|----------|
| `projeto/api` | **Visual Studio** | .NET, microsserviÃ§os, testes, Postgres |
| `projeto/app` | **VS Code** | React, formulÃ¡rios, rotas, auth UI |
| `docs/` | Browser | Comparativo legado Ã— moderno (apresentaÃ§Ã£o) |

O comparativo com MVC legado fica **sÃ³ nos slides** (`docs/apresentacao.html`) â€” a solution traz apenas o sistema moderno.

## PrÃ©-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download) + Visual Studio
- [Node.js 20+](https://nodejs.org/) + VS Code
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (banco)

## Como abrir

1. **Backend:** abra `projeto/api/InssModernizacao.slnx` no Visual Studio  
2. **Frontend:** no VS Code â†’ File â†’ Open Folder â†’ `projeto/app`

## Subir o ambiente

```powershell
# Na pasta projeto/api â€” banco
docker compose up -d

# Visual Studio / terminal na api
dotnet run --project src/Contribuintes.Api    # :5001
dotnet run --project src/Beneficios.Api       # :5002

# VS Code / terminal no app
npm install
npm run dev                                   # :5173
```

Ou na raiz do `fiap`: `.\scripts\iniciar-ambiente.ps1`

**Portal:** http://localhost:5173  
**Login:** `admin` / `123`

## Deploy na nuvem (opcional)

Stack completa grÃ¡tis: **Vercel** (front) + **Render** (APIs) + **Neon** (Postgres).  
Guia passo a passo: [`docs/deploy-nuvem.md`](docs/deploy-nuvem.md)

## Testes

```powershell
# Backend (com Postgres no ar)
cd projeto/api
dotnet test

# Frontend e2e
cd projeto/app
npm run test:e2e
```

## ApresentaÃ§Ãµes

| Arquivo | Para quem | Para quÃª |
|---------|-----------|----------|
| [`docs/aulas-m13-m16.html`](docs/aulas-m13-m16.html) | Alunos | Conceitos M13â€“M16 (o que Ã© cada tema) |
| [`docs/apresentacao.html`](docs/apresentacao.html) | Alunos | Projeto INSS / arquitetura / DDD |

Abra no browser. As de aluno: setas â† â†’. O guia do professor: menu Ã  esquerda.
