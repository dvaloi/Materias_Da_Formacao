import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import ErrorMessage from '../components/ErrorMessage';

export default function Login() {
  const { login, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  // Demo didática — usuário e senha simples para a aula
  const [email, setEmail] = useState('admin');
  const [senha, setSenha] = useState('123');
  const [erro, setErro] = useState('');
  const [loading, setLoading] = useState(false);

  if (isAuthenticated) {
    navigate('/contribuintes', { replace: true });
    return null;
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setErro('');
    setLoading(true);

    try {
      // M14 — login → API devolve JWT → AuthContext grava no localStorage
      await login(email, senha);
      navigate('/contribuintes');
    } catch (err) {
      setErro(err.message || 'Credenciais inválidas');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="login-page">
      <div className="flag-stripe" aria-hidden="true" />
      <div className="login-card">
        <h1>Portal INSS 🇲🇿</h1>
        <p className="login-subtitle">Instituto Nacional de Segurança Social</p>
        <p className="login-location">Maputo · Moçambique · Metical (MZN)</p>

        <ErrorMessage mensagem={erro} />

        <form onSubmit={handleSubmit} noValidate>
          <div className="form-group">
            <label htmlFor="email">Usuário</label>
            <input
              id="email"
              type="text"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              autoComplete="username"
            />
          </div>

          <div className="form-group">
            <label htmlFor="senha">Senha</label>
            <input
              id="senha"
              type="password"
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              required
              autoComplete="current-password"
            />
          </div>

          <button type="submit" className="btn btn-primary btn-block" disabled={loading}>
            {loading ? 'Entrando...' : 'Entrar no portal'}
          </button>
        </form>

        <p className="login-hint">
          Teste: admin / 123
        </p>
      </div>
    </div>
  );
}
