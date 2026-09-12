// M13 — Front não fala com o banco: só HTTP com as APIs (.NET).
// Locais: Contribuintes :5001 · Benefícios :5002 (ou VITE_* na nuvem).
const CONTRIBUINTES_URL = import.meta.env.VITE_CONTRIBUINTES_API || 'http://localhost:5001';
const BENEFICIOS_URL = import.meta.env.VITE_BENEFICIOS_API || 'http://localhost:5002';

class ApiError extends Error {
  constructor(message, status) {
    super(message);
    this.status = status;
  }
}

async function request(baseUrl, path, options = {}) {
  // M14 — JWT: token no header Authorization em toda chamada autenticada
  const token = localStorage.getItem('inss_token');
  const headers = {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...options.headers
  };

  let response;
  try {
    response = await fetch(`${baseUrl}${path}`, { ...options, headers });
  } catch {
    throw new ApiError(
      'Não foi possível contactar a API. Se estiver na nuvem, aguarde o Render acordar (30–60 s) e tente de novo.',
      0
    );
  }

  // M14 — 401 = sessão inválida → limpa storage e manda para o login
  if (response.status === 401) {
    localStorage.removeItem('inss_token');
    localStorage.removeItem('inss_user');
    window.location.href = '/login';
    throw new ApiError('Sessão expirada', 401);
  }

  if (!response.ok) {
    const body = await response.json().catch(() => ({}));
    // DomainException → { mensagem }; ModelState ASP.NET → { errors: { Campo: [...] } }
    let mensagem = body.mensagem || body.title || body.detail;
    if (!mensagem && body.errors) {
      mensagem = Object.values(body.errors).flat().join(' ');
    }
    if (!mensagem) {
      mensagem = response.status === 0 || response.type === 'opaque'
        ? 'Não foi possível contactar a API. Verifique se o serviço está no ar.'
        : `Erro na requisição (${response.status})`;
    }
    throw new ApiError(mensagem, response.status);
  }

  if (response.status === 204) return null;
  return response.json();
}

export const authApi = {
  login: (email, senha) =>
    request(CONTRIBUINTES_URL, '/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, senha })
    })
};

export const contribuintesApi = {
  listar: () => request(CONTRIBUINTES_URL, '/api/contribuintes'),
  obter: (id) => request(CONTRIBUINTES_URL, `/api/contribuintes/${id}`),
  criar: (data) =>
    request(CONTRIBUINTES_URL, '/api/contribuintes', {
      method: 'POST',
      body: JSON.stringify(data)
    }),
  desativar: (id) =>
    request(CONTRIBUINTES_URL, `/api/contribuintes/${id}/desativar`, { method: 'PATCH' })
};

// 2º microsserviço — outro host, outro banco (não compartilha tabelas)
export const pedidosApi = {
  listar: () => request(BENEFICIOS_URL, '/api/pedidos'),
  criar: (data) =>
    request(BENEFICIOS_URL, '/api/pedidos', {
      method: 'POST',
      body: JSON.stringify(data)
    }),
  aprovar: (id) =>
    request(BENEFICIOS_URL, `/api/pedidos/${id}/aprovar`, { method: 'PATCH' }),
  rejeitar: (id, motivo) =>
    request(BENEFICIOS_URL, `/api/pedidos/${id}/rejeitar`, {
      method: 'PATCH',
      body: JSON.stringify({ motivo })
    }),
  listarPorContribuinte: (contribuinteId) =>
    request(BENEFICIOS_URL, `/api/pedidos/contribuinte/${contribuinteId}`)
};

export { ApiError };
