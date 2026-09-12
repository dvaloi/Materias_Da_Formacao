import { API } from '../config/api';

export type Pensionista = {
  id: string;
  nuit: string;
  nome: string;
  dataNascimento: string;
};

export type StatusProvaVida = {
  contribuinteId: string;
  nuit: string;
  nome: string;
  podeAcessarBeneficio: boolean;
  provaValida: boolean;
  possuiCadastroBiometrico: boolean;
  requerCadastroInicial: boolean;
  requerRenovacao: boolean;
  proximaAcao: 'Cadastrar' | 'Renovar' | 'Consultar';
  validaAte: string | null;
  diasRestantes: number;
  motivoBloqueio: string | null;
  acaoRequerida: string | null;
};

export type RegistroProvaVida = {
  id: string;
  contribuinteId: string;
  nuit: string;
  realizadoEm: string;
  scoreLiveness: number;
  movimentosDetectados: boolean;
  status: string;
  tipoOperacao: string;
  identidadeConfirmada: boolean;
  similaridadeFacial: number | null;
};

export async function buscarPensionistaPorNuit(nuit: string): Promise<Pensionista | null> {
  const response = await fetch(`${API.contribuintes}/api/contribuintes/pensionistas/por-nuit/${nuit}`);
  if (response.status === 404) return null;
  if (!response.ok) {
    const body = await response.json().catch(() => ({}));
    throw new Error(body.mensagem ?? 'Erro ao buscar pensionista.');
  }
  return response.json();
}

export async function consultarStatusProvaVida(nuit: string): Promise<StatusProvaVida> {
  const response = await fetch(`${API.provaVida}/api/prova-vida/status/por-nuit/${nuit}`);
  const body = await response.json().catch(() => ({}));
  if (!response.ok) throw new Error(body.mensagem ?? 'Erro ao consultar status.');
  return body;
}

export async function enviarProvaVida(
  operacao: 'Cadastrar' | 'Renovar' | 'Consultar',
  payload: {
    nuit: string;
    scoreLiveness: number;
    movimentosDetectados: boolean;
    imagemBase64: string;
    faceEmbedding: number[];
  },
): Promise<RegistroProvaVida> {
  const endpoint =
    operacao === 'Renovar' ? 'renovar' : operacao === 'Consultar' ? 'consultar' : 'cadastrar';

  const response = await fetch(`${API.provaVida}/api/prova-vida/${endpoint}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });

  const body = await response.json().catch(() => ({}));
  if (!response.ok) {
    throw new Error(body.mensagem ?? 'Erro ao enviar prova de vida.');
  }
  return body;
}
