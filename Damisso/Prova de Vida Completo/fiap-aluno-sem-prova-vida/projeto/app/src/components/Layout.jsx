import { Link, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Layout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate('/login');
  }

  return (
    <div className="app-layout">
      <div className="flag-stripe" aria-hidden="true" />
      <header className="header">
        <div className="header-brand">
          <span className="logo">INSS 🇲🇿</span>
          <span className="subtitle">Instituto Nacional de Segurança Social</span>
          <span className="location">República de Moçambique · Maputo</span>
        </div>
        <nav className="nav" aria-label="Navegação principal">
          <Link to="/contribuintes">Contribuintes</Link>
          <Link to="/pedidos">Pedidos de Benefício</Link>
        </nav>
        <div className="header-user">
          <span>{user?.nome}</span>
          <button type="button" className="btn btn-outline" onClick={handleLogout}>
            Sair
          </button>
        </div>
      </header>

      <main className="main-content">
        <Outlet />
      </main>

      <footer className="footer">
        FIAP × INSS Moçambique — Portal em Metical (MZN) · NUIT · 2026
      </footer>
    </div>
  );
}
