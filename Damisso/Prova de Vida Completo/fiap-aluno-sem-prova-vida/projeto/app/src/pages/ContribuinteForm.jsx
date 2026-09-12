import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { contribuintesApi } from '../services/api';
import ErrorMessage from '../components/ErrorMessage';

// M13 — Validação no front (UX). A API / Domain valida de novo (segurança).
function validarFormulario(form) {
  const erros = {};

  if (!/^\d{9}$/.test(form.nuit))
    erros.nuit = 'NUIT deve ter exatamente 9 dígitos';

  if (form.nome.trim().length < 3)
    erros.nome = 'Nome deve ter pelo menos 3 caracteres';

  if (!form.dataNascimento)
    erros.dataNascimento = 'Data de nascimento é obrigatória';

  if (Number(form.salarioMensal) <= 0)
    erros.salarioMensal = 'Salário deve ser maior que zero';

  return erros;
}

export default function ContribuinteForm() {
  const navigate = useNavigate();

  // M13 — Formulário controlado: o estado (form) é a fonte da verdade dos inputs
  const [form, setForm] = useState({
    nuit: '',
    nome: '',
    dataNascimento: '',
    salarioMensal: ''
  });
  const [erros, setErros] = useState({});
  const [erroApi, setErroApi] = useState('');
  const [sucesso, setSucesso] = useState(false);
  const [loading, setLoading] = useState(false);

  function handleChange(e) {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
    setErros((prev) => ({ ...prev, [name]: undefined }));
    setSucesso(false);
  }

  async function handleSubmit(e) {
    e.preventDefault();
    const validacao = validarFormulario(form);
    setErros(validacao);
    if (Object.keys(validacao).length > 0) return; // bloqueia POST se inválido

    setLoading(true);
    setErroApi('');

    try {
      // M13 — POST na API Contribuintes → depois navega para o detalhe (/:id)
      const criado = await contribuintesApi.criar({
        nuit: form.nuit,
        nome: form.nome,
        dataNascimento: form.dataNascimento,
        salarioMensal: Number(form.salarioMensal)
      });
      setSucesso(true);
      setTimeout(() => navigate(`/contribuintes/${criado.id}`), 1000);
    } catch (err) {
      setErroApi(err.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <section>
      <h1>Novo Contribuinte</h1>
      <p className="page-description">
        Registro com NUIT (9 dígitos) · salário em Metical (MZN) — ex.: Maputo, Beira, Nampula
      </p>

      <ErrorMessage mensagem={erroApi} />
      {sucesso && <div className="alert alert-success" role="status">Contribuinte registrado com sucesso!</div>}

      <form onSubmit={handleSubmit} className="form-card" noValidate>
        <div className="form-group">
          <label htmlFor="nuit">NUIT (9 dígitos)</label>
          <input
            id="nuit"
            name="nuit"
            value={form.nuit}
            onChange={handleChange}
            maxLength={9}
            inputMode="numeric"
            aria-invalid={!!erros.nuit}
            aria-describedby={erros.nuit ? 'nuit-erro' : undefined}
          />
          {erros.nuit && <span id="nuit-erro" className="field-error">{erros.nuit}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="nome">Nome completo</label>
          <input id="nome" name="nome" value={form.nome} onChange={handleChange}
            aria-invalid={!!erros.nome} />
          {erros.nome && <span className="field-error">{erros.nome}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="dataNascimento">Data de nascimento</label>
          <input id="dataNascimento" name="dataNascimento" type="date"
            value={form.dataNascimento} onChange={handleChange} />
          {erros.dataNascimento && <span className="field-error">{erros.dataNascimento}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="salarioMensal">Salário mensal (MZN)</label>
          <input id="salarioMensal" name="salarioMensal" type="number" step="0.01" min="0"
            value={form.salarioMensal} onChange={handleChange} />
          {erros.salarioMensal && <span className="field-error">{erros.salarioMensal}</span>}
        </div>

        <div className="form-actions">
          <button type="button" className="btn btn-outline" onClick={() => navigate('/contribuintes')}>
            Cancelar
          </button>
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Salvando...' : 'Registrar'}
          </button>
        </div>
      </form>
    </section>
  );
}
