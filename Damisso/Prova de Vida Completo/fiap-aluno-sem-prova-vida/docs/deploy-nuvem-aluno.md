# Deploy na nuvem (stack completa — plano gratuito)

Guia para publicar **front + 2 APIs + Postgres** antes da aula.

| Camada | Serviço | Plano gratuito |
|--------|---------|----------------|
| **PostgreSQL** | [Neon](https://neon.tech) | Sim (com limites) |
| **API Contribuintes + Benefícios** | [Render](https://render.com) | Sim (cold start ~1 min) |
| **React (Vite)** | [Vercel](https://vercel.com) | Hobby grátis |

> O Vercel **não** hospeda .NET nem Postgres. Por isso usamos Render + Neon.

---

## O que você vai ter no final

- `https://seu-app.vercel.app` — portal React  
- `https://inss-contribuintes.onrender.com` — API :5001  
- `https://inss-beneficios.onrender.com` — API :5002  
- Postgres gerenciado no Neon  

**Login:** `admin` / `123`

---

## Pré-requisitos

1. Projeto no **GitHub** (veja **Passo 0** abaixo)
2. Contas grátis: Neon, Render, Vercel (login com GitHub facilita)

---

## Passo 0 — Subir o projeto no GitHub (você está aqui)

No GitHub, logado como `gitfernandolopes`:

### 0.1 Criar o repositório

1. Clique no **+** (canto superior direito) → **New repository**
2. Preencha:
   - **Repository name:** `inss-mocambique-modernizacao` (ou manter `inss-modernizacao`)
   - **Description:** `Modernização Full-Stack INSS Moçambique — FIAP G1 2026 (.NET, React, Postgres)`
   - **Visibility:** Private (recomendado para aula) ou Public
   - **Não** marque “Add a README” (o projeto já tem arquivos)
3. **Create repository**

### 0.2 Enviar a pasta `C:\fiap` para o GitHub

Escolha **uma** opção:

**Opção A — GitHub Desktop** (mais fácil se `git` não estiver no PATH)

1. Instale [GitHub Desktop](https://desktop.github.com/)
2. File → Add Local Repository → `C:\fiap`
3. Se pedir, “create a repository”
4. Publish repository → escolha `gitfernandolopes/inss-modernizacao`
5. Commit message: `INSS Moçambique — modernização Full-Stack — FIAP G1` → **Push**

**Opção B — Visual Studio**

1. Abra `projeto\api\InssModernizacao.slnx`
2. Git → **Create Git Repository** (se ainda não existir)
3. Git → **Push** → conecte ao repositório remoto que você criou no GitHub

**Opção C — PowerShell** (se `git` estiver instalado)

```powershell
cd C:\fiap
git init
git add .
git commit -m "INSS Moçambique — modernização Full-Stack — FIAP G1"
git branch -M main
git remote add origin https://github.com/gitfernandolopes/inss-modernizacao.git
git push -u origin main
```

### 0.3 Conferir o CI (M16 — GitHub Actions)

Depois do push, no GitHub:

1. Abra o repositório → aba **Actions**
2. Deve rodar o workflow **“CI — INSS Modernização”** (arquivo `.github/workflows/ci.yml`)

O pipeline já cobre o que vocês ministram:

| Módulo | O que o CI faz |
|--------|----------------|
| **M15** | `dotnet test` — unitário + integração (WebApplicationFactory) |
| **M15** | `npm run build` — front compila sem erro |
| **M16** | Quality gate: testes precisam passar para o job do front rodar |
| **M16** | Artefatos: `test-results` (`.trx`) + `frontend-dist` |

Se falhar, clique no job vermelho → veja o log (Postgres, .NET 10, testes).

> **Na aula M16:** mostre essa aba Actions ao vivo após um commit.

---

1. Acesse [neon.tech](https://neon.tech) → **New Project** (ex.: `inss-aula`)
2. Anote a connection string do banco padrão (formato `postgresql://...`)
3. No **SQL Editor** do Neon, rode:

```sql
CREATE DATABASE inss_beneficios;
```

4. Monte **duas** connection strings Npgsql (uma por microsserviço):

**Contribuintes** (`inss_contribuintes` ou o DB principal do Neon):

```
Host=SEU_HOST.neon.tech;Database=inss_contribuintes;Username=SEU_USER;Password=SUA_SENHA;SSL Mode=Require;Trust Server Certificate=true
```

**Benefícios:**

```
Host=SEU_HOST.neon.tech;Database=inss_beneficios;Username=SEU_USER;Password=SUA_SENHA;SSL Mode=Require;Trust Server Certificate=true
```

> No Neon, o host/user/senha são os mesmos; só muda o nome do `Database`.

---

## Passo 2 — APIs no Render (Docker)

Repita para **cada** API (Contribuintes e Benefícios).

### 2a) API Contribuintes

1. [dashboard.render.com](https://dashboard.render.com) → **New +** → **Web Service**
2. Conecte o repositório GitHub
3. Configuração:

| Campo | Valor |
|-------|--------|
| Name | `inss-contribuintes` |
| Root Directory | `projeto/api` |
| Runtime | **Docker** |
| Dockerfile Path | `docker/Dockerfile.contribuintes` |
| Instance Type | **Free** |

4. **Environment Variables:**

| Key | Value |
|-----|--------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__Default` | *(connection string Contribuintes do Neon)* |
| `Jwt__Key` | `InssMozambique2026ChaveSecretaMin32Chars!` |
| `Jwt__Issuer` | `InssContribuintes` |
| `Jwt__Audience` | `InssPortal` |
| `Cors__Origins` | *(deixe vazio por agora; preenche no passo 4)* |

5. **Create Web Service** — aguarde o build (5–10 min na 1ª vez)

6. Teste: `https://inss-contribuintes.onrender.com/openapi/v1.json` (deve abrir JSON)

### 2b) API Benefícios

Mesmo processo, com:

| Campo | Valor |
|-------|--------|
| Name | `inss-beneficios` |
| Dockerfile Path | `docker/Dockerfile.beneficios` |
| `ConnectionStrings__Default` | *(connection string Benefícios / inss_beneficios)* |

Teste: `https://inss-beneficios.onrender.com/openapi/v1.json`

> **Cold start:** no plano free o Render “dorme”. A 1ª requisição pode levar **30–60 s**. Normal na demo — avise a turma.

---

## Passo 3 — Front na Vercel

1. [vercel.com](https://vercel.com) → **Add New Project** → importe o GitHub
2. Configuração:

| Campo | Valor |
|-------|--------|
| Root Directory | `projeto/app` |
| Framework Preset | Vite |
| Build Command | `npm run build` |
| Output Directory | `dist` |

3. **Environment Variables** (Production):

| Key | Value |
|-----|--------|
| `VITE_CONTRIBUINTES_API` | `https://inss-contribuintes.onrender.com` |
| `VITE_BENEFICIOS_API` | `https://inss-beneficios.onrender.com` |

4. **Deploy**

5. Anote a URL: `https://seu-projeto.vercel.app`

---

## Passo 4 — CORS (obrigatório)

Volte no **Render** → cada API → **Environment** → edite:

```
Cors__Origins=https://seu-projeto.vercel.app
```

Salve (o Render redeploya sozinho).

> Se tiver preview da Vercel, pode adicionar várias URLs separadas por vírgula:
> `https://seu-projeto.vercel.app,https://seu-projeto-git-main.vercel.app`

---

## Passo 5 — Testar antes da aula

1. Abra a URL da Vercel
2. Login: `admin` / `123`
3. Lista de contribuintes (seed com nomes fictícios)
4. **+ Novo Contribuinte**
5. **Pedidos de Benefício** → **+ Novo Pedido**

Se falhar:
- F12 → Network → veja se a API retornou erro ou demorou (cold start)
- Confira `Cors__Origins` com a URL exata da Vercel (sem barra no final)
- Confira as variáveis `VITE_*` na Vercel (precisa **redeploy** após mudar)

---

## Checklist rápido

- [ ] Neon: 2 bancos (`inss_contribuintes` + `inss_beneficios`)
- [ ] Render: `inss-contribuintes` no ar
- [ ] Render: `inss-beneficios` no ar
- [ ] Vercel: front com `VITE_*` apontando para Render
- [ ] CORS nas 2 APIs com URL da Vercel
- [ ] Login + listagem + criar pedido testados

---

## Limitações do plano grátis (fale na aula)

| Limitação | Impacto |
|-----------|---------|
| Render free dorme | 1ª request lenta |
| Neon free | Limite de storage/conexões — ok para demo |
| Vercel hobby | Só o front; APIs ficam no Render |

**Plano B na sala:** ambiente local (Docker + `dotnet run` + `npm run dev`) como você já testou.

---

## Arquivos do projeto relacionados

- `projeto/api/docker/Dockerfile.contribuintes`
- `projeto/api/docker/Dockerfile.beneficios`
- `projeto/app/vercel.json`
- `projeto/app/.env.example`
