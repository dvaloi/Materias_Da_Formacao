import { useEffect, useRef, useState } from 'react';
import {
  ActivityIndicator,
  Pressable,
  StyleSheet,
  Text,
  View,
} from 'react-native';
import { CameraView, useCameraPermissions } from 'expo-camera';
import {
  Pensionista,
  StatusProvaVida,
  enviarProvaVida,
} from '../services/api';
import {
  AnaliseCaptura,
  analisarCaptura,
  calcularLiveness,
  escolherEmbeddingReferencia,
  initFaceRecognition,
} from '../services/faceRecognition';

type Props = {
  pensionista: Pensionista;
  status: StatusProvaVida;
  onSuccess: (resultado: { id: string; similaridadeFacial: number | null; tipoOperacao: string }) => void;
  onCancel: () => void;
};

const PASSOS = [
  {
    titulo: 'Olhe para a câmera (FRENTE)',
    dica: 'Rosto centralizado · depois toque Capturar',
  },
  {
    titulo: 'Vire o rosto à ESQUERDA',
    dica: 'Movimento de cabeça · mantenha e toque Capturar',
  },
  {
    titulo: 'Vire o rosto à DIREITA',
    dica: 'Movimento de cabeça · mantenha e toque Capturar',
  },
];

// TODO (produção INSS Moçambique): trocar desafios locais por SDK pago de liveness
// (FaceTec / iProov / AWS Rekognition Face Liveness). Manter o fluxo Cadastrar|Consultar|Renovar.
function resolverOperacao(status: StatusProvaVida): 'Cadastrar' | 'Renovar' | 'Consultar' {
  if (status.proximaAcao === 'Renovar') return 'Renovar';
  if (status.proximaAcao === 'Consultar' || status.provaValida) return 'Consultar';
  return 'Cadastrar';
}

export function LivenessScreen({ pensionista, status, onSuccess, onCancel }: Props) {
  const cameraRef = useRef<CameraView>(null);
  const [permission, requestPermission] = useCameraPermissions();
  const [passo, setPasso] = useState(0);
  const [analises, setAnalises] = useState<AnaliseCaptura[]>([]);
  const [capturasBase64, setCapturasBase64] = useState<string[]>([]);
  const [enviando, setEnviando] = useState(false);
  const [carregandoModelos, setCarregandoModelos] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  const operacao = resolverOperacao(status);

  useEffect(() => {
    if (!permission?.granted) requestPermission();
  }, [permission, requestPermission]);

  useEffect(() => {
    initFaceRecognition()
      .catch((e) => setErro(e instanceof Error ? e.message : 'Falha ao carregar modelos faciais.'))
      .finally(() => setCarregandoModelos(false));
  }, []);

  async function capturarPasso() {
    if (!cameraRef.current || carregandoModelos) return;

    setErro(null);
    const foto = await cameraRef.current.takePictureAsync({
      base64: true,
      quality: 0.6,
    });

    if (!foto?.base64) {
      setErro('Não foi possível capturar a imagem. Tente novamente.');
      return;
    }

    try {
      const analise = await analisarCaptura(foto.base64);
      const novasAnalises = [...analises, analise];
      const novasCapturas = [...capturasBase64, foto.base64];
      setAnalises(novasAnalises);
      setCapturasBase64(novasCapturas);

      if (passo < PASSOS.length - 1) {
        setPasso(passo + 1);
        return;
      }

      await enviarProva(novasAnalises, novasCapturas);
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Erro no reconhecimento facial.');
    }
  }

  async function enviarProva(analisesCaptura: AnaliseCaptura[], fotos: string[]) {
    setEnviando(true);
    setErro(null);

    try {
      const { scoreLiveness, movimentosDetectados } = calcularLiveness(analisesCaptura);
      if (!movimentosDetectados) {
        throw new Error(
          'Liveness não confirmado. Vire mais o rosto à esquerda e à direita.',
        );
      }

      const embedding = escolherEmbeddingReferencia(analisesCaptura);
      const registro = await enviarProvaVida(operacao, {
        nuit: pensionista.nuit,
        scoreLiveness,
        movimentosDetectados,
        imagemBase64: fotos.join('|'),
        faceEmbedding: embedding,
      });

      onSuccess({
        id: registro.id,
        similaridadeFacial: registro.similaridadeFacial,
        tipoOperacao: registro.tipoOperacao,
      });
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Falha ao registrar prova de vida.');
    } finally {
      setEnviando(false);
    }
  }

  if (!permission) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color="#0d47a1" />
      </View>
    );
  }

  if (!permission.granted) {
    return (
      <View style={styles.center}>
        <Text style={styles.texto}>Precisamos da câmera para a Prova de Vida.</Text>
        <Pressable style={styles.botao} onPress={requestPermission}>
          <Text style={styles.botaoTexto}>Permitir câmera</Text>
        </Pressable>
      </View>
    );
  }

  const titulo =
    operacao === 'Consultar'
      ? 'Consultar — autenticar com câmera'
      : operacao === 'Renovar'
        ? 'Renovar — validar rosto'
        : 'Cadastrar rosto (1ª vez)';

  const botaoFinal =
    operacao === 'Consultar'
      ? 'Confirmar identidade'
      : operacao === 'Renovar'
        ? 'Renovar prova de vida'
        : 'Finalizar cadastro';

  return (
    <View style={styles.container}>
      <Text style={styles.titulo}>{titulo}</Text>
      <Text style={styles.subtitulo}>{pensionista.nome}</Text>
      {operacao === 'Consultar' ? (
        <Text style={styles.info}>
          Prova válida · {status.diasRestantes} dias restantes — autentica sem renovar
        </Text>
      ) : null}
      <Text style={styles.passo}>Passo {passo + 1} de {PASSOS.length} — liveness por movimento</Text>
      <Text style={styles.instrucao}>{PASSOS[passo].titulo}</Text>
      <Text style={styles.dicaPasso}>{PASSOS[passo].dica}</Text>
      <Text style={styles.tecnico}>
        NUIT → template no banco (sem gravar foto) · match ≥ 72% · 3 poses obrigatórias
      </Text>

      {carregandoModelos ? (
        <View style={styles.centerBox}>
          <ActivityIndicator size="large" color="#0d47a1" />
          <Text style={styles.texto}>Baixando modelos ML (1ª vez)…</Text>
        </View>
      ) : (
        <View style={styles.cameraBox}>
          <CameraView ref={cameraRef} style={styles.camera} facing="front" />
        </View>
      )}

      {erro ? <Text style={styles.erro}>{erro}</Text> : null}

      <Pressable
        style={[styles.botao, (enviando || carregandoModelos) && styles.botaoDisabled]}
        onPress={capturarPasso}
        disabled={enviando || carregandoModelos}
      >
        <Text style={styles.botaoTexto}>
          {enviando
            ? 'Validando identidade…'
            : passo < PASSOS.length - 1
              ? 'Capturar e continuar'
              : botaoFinal}
        </Text>
      </Pressable>

      <Pressable style={styles.link} onPress={onCancel} disabled={enviando}>
        <Text style={styles.linkTexto}>Cancelar</Text>
      </Pressable>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, padding: 20, backgroundColor: '#f5f7fb' },
  center: { flex: 1, justifyContent: 'center', alignItems: 'center', padding: 24 },
  centerBox: { marginTop: 16, height: 360, justifyContent: 'center', alignItems: 'center' },
  titulo: { fontSize: 22, fontWeight: '700', color: '#0d47a1' },
  subtitulo: { marginTop: 4, color: '#333' },
  info: { marginTop: 8, fontSize: 13, color: '#1b5e20', fontWeight: '600' },
  passo: { marginTop: 16, fontWeight: '600' },
  instrucao: { marginTop: 8, fontSize: 16, color: '#444', fontWeight: '600' },
  dicaPasso: { marginTop: 4, fontSize: 13, color: '#666' },
  tecnico: { marginTop: 4, fontSize: 11, color: '#666' },
  cameraBox: {
    marginTop: 16,
    borderRadius: 12,
    overflow: 'hidden',
    height: 360,
    backgroundColor: '#000',
  },
  camera: { flex: 1 },
  botao: {
    marginTop: 16,
    backgroundColor: '#0d47a1',
    padding: 14,
    borderRadius: 8,
    alignItems: 'center',
  },
  botaoDisabled: { opacity: 0.6 },
  botaoTexto: { color: '#fff', fontWeight: '700' },
  link: { marginTop: 12, alignItems: 'center' },
  linkTexto: { color: '#555' },
  erro: { marginTop: 12, color: '#b00020' },
  texto: { textAlign: 'center', marginBottom: 12, color: '#333' },
});
