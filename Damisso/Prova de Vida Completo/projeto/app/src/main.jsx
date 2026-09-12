import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import App from './App';
import './styles/index.css';

// M13 — StrictMode (só em desenvolvimento): monta efeitos 2× de propósito.
// No DevTools → Network, a mesma API pode aparecer duas vezes. Em produção é 1×.
createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
      {/* M14 — AuthProvider: sessão (token/user) disponível em toda a árvore */}
      <AuthProvider>
        <App />
      </AuthProvider>
    </BrowserRouter>
  </StrictMode>
);
