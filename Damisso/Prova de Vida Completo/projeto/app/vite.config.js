import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api/contribuintes': 'http://localhost:5001',
      '/api/auth': 'http://localhost:5001',
      '/api/pedidos': 'http://localhost:5002'
    }
  }
});
