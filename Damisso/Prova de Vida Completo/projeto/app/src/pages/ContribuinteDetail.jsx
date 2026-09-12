import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { contribuintesApi, pedidosApi } from '../services/api';
import LoadingSpinner from '../components/LoadingSpinner';
import ErrorMessage from '../components/ErrorMessage';

export default function ContribuinteDetail() {
  // M13 — useParams(): lê o :id da rota /contribuintes/:id
  const { id } = useParams();
  const [contribuinte, setContribuinte] = useState(null);
  const [pedidos, setPedidos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState('');

  useEffect(() => {
    async function carregar() {
      try {
        // Duas APIs em paralelo: contribuinte (:5001) + pedidos desse ID (:5002)
        const [c, p] = await Promise.all([
          contribuintesApi.obter(id),
          pedidosApi.listarPorContribuinte(id)
        ]);
        setContribuinte(c);
        setPedidos(p);
      } catch (err) {
        setErro(err.message);
      } finally {
        setLoading(false);
      }
    }
    carregar();
  }, [id]);

  if (loading) return <LoadingSpinner />;
  if (erro) return <ErrorMessage mensagem={erro} />;
  if (!contribuinte) return <div className="empty-state">Contribuinte não encontrado.</div>;

  return (
    <section>
      <Link to="/contribuintes" className="back-link">← Voltar à lista</Link>
      <h1>{contribuinte.nome}</h1>

      <div className="detail-grid">
        <div className="detail-card">
          <h2>Dados do contribuinte</h2>
          <dl>
            <dt>NUIT</dt>
            <dd>{contribuinte.nuit}</dd>
            <dt>Salário (MZN)</dt>
            <dd>{contribuinte.salarioMensal.toLocaleString('pt-MZ')}</dd>
            <dt>Contribuição 3%</dt>
            <dd>{contribuinte.contribuicaoMensal.toLocaleString('pt-MZ')}</dd>
            <dt>Situação</dt>
            <dd>
              <span className={`badge badge-${contribuinte.situacao.toLowerCase()}`}>
                {contribuinte.situacao}
              </span>
            </dd>
            <dt>Nascimento</dt>
            <dd>{contribuinte.dataNascimento}</dd>
          </dl>
        </div>

        <div className="detail-card">
          <h2>Pedidos de benefício</h2>
          {pedidos.length === 0 ? (
            <p className="text-muted">Nenhum pedido para este contribuinte.</p>
          ) : (
            <ul className="pedidos-list">
              {pedidos.map((p) => (
                <li key={p.id}>
                  <strong>{p.tipo}</strong>
                  <span>{p.valorSolicitado.toLocaleString('pt-MZ')} MZN</span>
                  <span className={`badge badge-${p.status.toLowerCase()}`}>{p.status}</span>
                </li>
              ))}
            </ul>
          )}
          {/* Query string pré-preenche o select em PedidoForm */}
          <Link to={`/pedidos/novo?contribuinteId=${id}`} className="btn btn-primary">
            + Novo pedido
          </Link>
        </div>
      </div>
    </section>
  );
}
