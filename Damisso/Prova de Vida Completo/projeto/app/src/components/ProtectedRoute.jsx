import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

// M14 — Rota protegida: sem login → redireciona para /login
// No back, o equivalente é [Authorize] nos controllers.
export default function ProtectedRoute() {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
}
