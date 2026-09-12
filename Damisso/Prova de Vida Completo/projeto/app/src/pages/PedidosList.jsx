import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { pedidosApi } from '../services/api';
import LoadingSpinner from '../components/LoadingSpinner';
import ErrorMessage from '../components/ErrorMessage';

export default function PedidosList() {
  const [pedidos, setPedidos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState('');
  const [acao, setAcao] = useState('');

  async function carregar() {
    setLoading(true);
    setErro('');
    try {
      // M13 — Demo Network (F12): GET /api/pedidos no 2º microsserviço (:5002).
      // Em dev o StrictMode pode mostrar a chamada 2× — não é bug da API.
      setPedidos(await pedidosApi.listar());
    } catch (err) {
      setErro(err.message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { carregar(); }, []);

  async function aprovar(id) {
    setAcao(id);
    try {
      await pedidosApi.aprovar(id);
      await carregar();
    } catch (err) {
      setErro(err.message);
    } finally {
      setAcao('');
    }
  }

  async function rejeitar(id) {
    const motivo = prompt('Motivo da rejeição:');
    if (!motivo) return;
    setAcao(id);
    try {
      await pedidosApi.rejeitar(id, motivo);
      await carregar();
    } catch (err) {
      setErro(err.message);
    } finally {
      setAcao('');
    }
  }

  if (loading) return <LoadingSpinner texto="Carregando pedidos..." />;

  return (
    <section>
      <div className="page-header">
        <div>
          <h1>Pedidos de Benefício</h1>
          <p className="page-description">Reforma, pensão, doença e maternidade · valores em MZN</p>
        </div>
        <Link to="/pedidos/novo" className="btn btn-primary">+ Novo Pedido</Link>
      </div>

      <ErrorMessage mensagem={erro} onRetry={carregar} />

      {/* Seed não cria pedidos — lista vazia é normal até registrar o primeiro */}
      {pedidos.length === 0 && !erro ? (
        <div className="empty-state"><p>Nenhum pedido registrado.</p></div>
      ) : (
        <div className="table-responsive">
          <table className="table">
            <thead>
              <tr>
                <th>Tipo</th>
                <th>Valor (MZN)</th>
                <th>Status</th>
                <th>Data</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {pedidos.map((p) => (
                <tr key={p.id}>
                  <td>{p.tipo}</td>
                  <td>{p.valorSolicitado.toLocaleString('pt-MZ')}</td>
                  <td><span className={`badge badge-${p.status.toLowerCase()}`}>{p.status}</span></td>
                  <td>{new Date(p.dataPedido).toLocaleDateString('pt-MZ')}</td>
                  <td className="actions">
                    {(p.status === 'Pendente' || p.status === 'EmAnalise') && (
                      <>
                        <button className="btn btn-sm btn-success" disabled={acao === p.id}
                          onClick={() => aprovar(p.id)}>Aprovar</button>
                        <button className="btn btn-sm btn-danger" disabled={acao === p.id}
                          onClick={() => rejeitar(p.id)}>Rejeitar</button>
                      </>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}
