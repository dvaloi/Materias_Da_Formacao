import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  timeout: 30000,
  use: {
    baseURL: 'http://localhost:5173',
    headless: true
  },
  webServer: [
    {
      command: 'dotnet run --project ../api/src/Contribuintes.Api/Contribuintes.Api.csproj',
      url: 'http://localhost:5001/openapi/v1.json',
      reuseExistingServer: true,
      timeout: 60000
    },
    {
      command: 'dotnet run --project ../api/src/Beneficios.Api/Beneficios.Api.csproj',
      url: 'http://localhost:5002/openapi/v1.json',
      reuseExistingServer: true,
      timeout: 60000
    },
    {
      command: 'npm run dev',
      url: 'http://localhost:5173',
      reuseExistingServer: true,
      timeout: 30000
    }
  ]
});
