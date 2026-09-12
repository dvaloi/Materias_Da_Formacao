# InssApi — exemplo da TARDE (M6)

Mesmo balcão da manhã, agora com **SQLite + EF Core**.

## Abrir
Visual Studio → abrir `InssApi.csproj` nesta pasta.

## Prova no Swagger
1. F5 → `/swagger`
2. POST contribuinte Ana → 201
3. Shift+F5 → F5 de novo
4. GET lista → a Ana **continua** (gravou em `inss.db`)

## Estrutura
- `Data/AppDbContext.cs`
- `Controllers/` com `_db` + `SaveChangesAsync`
- `appsettings.json` → `Data Source=inss.db`
