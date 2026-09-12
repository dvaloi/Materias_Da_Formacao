# Consultas no PostgreSQL (como no SQL Server)

O projeto usa **PostgreSQL no Docker**. As consultas são SQL padrão — bem parecidas com SQL Server.

## Conectar (DBeaver, Azure Data Studio, pgAdmin, DataGrip)

| Campo | Valor |
|--------|--------|
| Host | `localhost` |
| Porta | `5432` |
| Usuário | `inss` |
| Senha | `inss123` |
| Banco Contribuintes | `inss_contribuintes` |
| Banco Benefícios | `inss_beneficios` |

## Exemplos — Contribuintes

```sql
-- Listar todos (parecido com SQL Server)
SELECT "Id", "Nuit", "Nome", "SalarioMensal", "Situacao"
FROM "Contribuintes"
ORDER BY "Nome";

-- Filtrar por NUIT
SELECT *
FROM "Contribuintes"
WHERE "Nuit" = '100200301';

-- Contribuição estimada 3% (só para demo)
SELECT
  "Nome",
  "SalarioMensal",
  ROUND("SalarioMensal" * 0.03, 2) AS contribuicao_estimada
FROM "Contribuintes";
```

## Exemplos — Benefícios

```sql
SELECT "Id", "ContribuinteId", "Tipo", "ValorSolicitado", "Status", "DataPedido"
FROM "PedidosBeneficio"
ORDER BY "DataPedido" DESC;
```

## SQL Server × PostgreSQL (diferenças rápidas na aula)

| SQL Server | PostgreSQL |
|------------|------------|
| `SELECT TOP 10 * FROM Tabela` | `SELECT * FROM "Tabela" LIMIT 10` |
| Identificadores sem aspas (costuma) | Aspas duplas se o EF criou com maiúsculas |
| `GETDATE()` | `NOW()` |
| `NVARCHAR` | `text` / `varchar` |

Para a aula: o importante é mostrar **SELECT / WHERE / ORDER BY** — a lógica é a mesma.

## Subir o banco

```powershell
docker compose up -d
```

As APIs aplicam as **Migrations** ao iniciar (`MigrateAsync`).
