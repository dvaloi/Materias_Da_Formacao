import { Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout';
import ProtectedRoute from './components/ProtectedRoute';
import Login from './pages/Login';
import ContribuintesList from './pages/ContribuintesList';
import ContribuinteForm from './pages/ContribuinteForm';
import ContribuinteDetail from './pages/ContribuinteDetail';
import PedidosList from './pages/PedidosList';
import PedidoForm from './pages/PedidoForm';

// M13 — React Router: cada path = uma tela. :id = parâmetro dinâmico.
// M14 — ProtectedRoute envolve as rotas privadas (precisa estar logado).
export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />

      <Route element={<ProtectedRoute />}>
        <Route element={<Layout />}>
          <Route index element={<Navigate to="/contribuintes" replace />} />
          <Route path="contribuintes" element={<ContribuintesList />} />
          <Route path="contribuintes/novo" element={<ContribuinteForm />} />
          <Route path="contribuintes/:id" element={<ContribuinteDetail />} />
          <Route path="pedidos" element={<PedidosList />} />
          <Route path="pedidos/novo" element={<PedidoForm />} />
        </Route>
      </Route>

      <Route path="*" element={<Navigate to="/contribuintes" replace />} />
    </Routes>
  );
}
