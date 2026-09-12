import { test, expect } from '@playwright/test';

// M15 — E2E (topo da pirâmide): poucos testes, fluxo real no browser.
// Cobre M13 (rotas/lista) + M14 (login) sem abrir o código React linha a linha.
test.describe('Portal INSS — E2E', () => {
  test('login e listagem de contribuintes', async ({ page }) => {
    await page.goto('/login');

    await page.getByLabel('Usuário').fill('admin');
    await page.getByLabel('Senha').fill('123');
    await page.getByRole('button', { name: 'Entrar' }).click();

    await expect(page.getByRole('heading', { name: 'Contribuintes' })).toBeVisible();
    await expect(page.getByText('Ana Celeste Mabunda')).toBeVisible();
    await expect(page.getByText('Graça Mussá Tembe')).toBeVisible();
    await expect(page.getByText('Tomás Viola Matola')).toBeVisible();
  });

  test('navegação para formulário de novo contribuinte', async ({ page }) => {
    await page.goto('/login');
    await page.getByLabel('Usuário').fill('admin');
    await page.getByLabel('Senha').fill('123');
    await page.getByRole('button', { name: 'Entrar' }).click();

    await page.getByRole('link', { name: '+ Novo Contribuinte' }).click();
    await expect(page.getByRole('heading', { name: 'Novo Contribuinte' })).toBeVisible();
  });
});
