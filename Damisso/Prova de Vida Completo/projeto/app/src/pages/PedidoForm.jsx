import { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { contribuintesApi, pedidosApi } from '../services/api';
import ErrorMessage from '../components/ErrorMessage';

const TIPOS = ['Reforma', 'Pensao', 'Doenca', 'Maternidade'];

/** Aceita "15000", "15000.50" ou "15.000,50" / "0,03" (pt) */
function parseValorMzn(texto) {
  if (texto == null || String(texto).trim() === '') return NaN;
  let s = String(texto).trim().replace(/\s/g, '');
  if (s.includes(',') && s.includes('.')) {
    // 15.000,50 → remove milhar, vírgula vira ponto
    s = s.replace(/\./g, '').replace(',', '.');
  } else if (s.includes(',')) {
    s = s.replace(',', '.');
  }
  return Number(s);
}

export default function PedidoForm() {
  const navigate = useNavigate();
  // M13 — ?contribuinteId= na URL (vindo do detalhe) pré-seleciona o contribuinte
  const [searchParams] = useSearchParams();
  const [contribuintes, setContribuintes] = useState([]);
  const [form, setForm] = useState({
    contribuinteId: searchParams.get('contribuinteId') || '',
    tipo: 'Reforma',
    valorSolicitado: ''
  });
  const [erro, setErro] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    contribuintesApi.listar().then(setContribuintes).catch(() => {});
  }, []);

  async function handleSubmit(e) {
    e.preventDefault();
    const valor = parseValorMzn(form.valorSolicitado);

    if (!form.contribuinteId) {
      setErro('Selecione o contribuinte.');
      return;
    }
    if (!Number.isFinite(valor) || valor <= 0) {
      setErro('Informe um valor em MZN maior que zero (ex.: 15000 ou 15000.50).');
      return;
    }

    setLoading(true);
    setErro('');

    try {
      await pedidosApi.criar({
        contribuinteId: form.contribuinteId,
        tipo: form.tipo,
        valorSolicitado: valor
      });
      navigate('/pedidos');
    } catch (err) {
      setErro(err.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <section>
      <h1>Novo Pedido de Benefício</h1>
      <p className="page-description">
        Pedido ao INSS Moçambique · integração com o microsserviço de Benefícios
      </p>

      <ErrorMessage mensagem={erro} />

      <form onSubmit={handleSubmit} className="form-card" noValidate>
        <div className="form-group">
          <label htmlFor="contribuinteId">Contribuinte</label>
          <select id="contribuinteId" value={form.contribuinteId}
            onChange={(e) => setForm({ ...form, contribuinteId: e.target.value })} required>
            <option value="">Selecionar...</option>
            {contribuintes.map((c) => (
              <option key={c.id} value={c.id}>{c.nome} — NUIT {c.nuit}</option>
            ))}
          </select>
        </div>

        <div className="form-group">
          <label htmlFor="tipo">Tipo de benefício</label>
          <select id="tipo" value={form.tipo}
            onChange={(e) => setForm({ ...form, tipo: e.target.value })}>
            {TIPOS.map((t) => <option key={t} value={t}>{t}</option>)}
          </select>
        </div>

        <div className="form-group">
          <label htmlFor="valor">Valor solicitado (MZN)</label>
          <input
            id="valor"
            type="text"
            inputMode="decimal"
            placeholder="Ex.: 15000"
            required
            value={form.valorSolicitado}
            onChange={(e) => setForm({ ...form, valorSolicitado: e.target.value })}
          />
        </div>

        <div className="form-actions">
          <button type="button" className="btn btn-outline" onClick={() => navigate('/pedidos')}>
            Cancelar
          </button>
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Enviando...' : 'Enviar pedido'}
          </button>
        </div>
      </form>
    </section>
  );
}
