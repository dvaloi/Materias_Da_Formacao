import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { contribuintesApi } from '../services/api';
import LoadingSpinner from '../components/LoadingSpinner';
import ErrorMessage from '../components/ErrorMessage';

export default function ContribuintesList() {
  // M13 — Quatro estados de UI: loading | erro | vazio | sucesso (com dados)
  const [contribuintes, setContribuintes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState('');

  async function carregar() {
    setLoading(true);
    setErro('');
    try {
      const data = await contribuintesApi.listar();
      setContribuintes(data);
    } catch (err) {
      setErro(err.message);
    } finally {
      setLoading(false);
    }
  }

  // M13 — Ao montar a tela, busca na API (useEffect). StrictMode pode disparar 2× em dev.
  useEffect(() => { carregar(); }, []);

  if (loading) return <LoadingSpinner texto="Carregando contribuintes..." />;

  return (
    <section>
      <div className="page-header">
        <div>
          <h1>Contribuintes INSS</h1>
          <p className="page-description">
            Contribuintes fictícios · NUIT · salários em Metical (MZN)
          </p>
        </div>
        <Link to="/contribuintes/novo" className="btn btn-primary">+ Novo Contribuinte</Link>
      </div>

      <ErrorMessage mensagem={erro} onRetry={carregar} />

      {contribuintes.length === 0 && !erro ? (
        <div className="empty-state">
          <p>Nenhum contribuinte registrado.</p>
          <Link to="/contribuintes/novo" className="btn btn-primary">Registrar primeiro contribuinte</Link>
        </div>
      ) : (
        <div className="table-responsive">
          <table className="table">
            <thead>
              <tr>
                <th>NUIT</th>
                <th>Nome</th>
                <th>Salário (MZN)</th>
                <th>Contribuição 3%</th>
                <th>Situação</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {contribuintes.map((c) => (
                <tr key={c.id}>
                  <td>{c.nuit}</td>
                  <td>{c.nome}</td>
                  <td>{c.salarioMensal.toLocaleString('pt-MZ')}</td>
                  <td>{c.contribuicaoMensal.toLocaleString('pt-MZ')}</td>
                  <td><span className={`badge badge-${c.situacao.toLowerCase()}`}>{c.situacao}</span></td>
                  <td><Link to={`/contribuintes/${c.id}`}>Detalhes</Link></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}
