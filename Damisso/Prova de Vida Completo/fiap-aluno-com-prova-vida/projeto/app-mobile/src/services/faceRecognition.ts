/**
 * Biometria facial — camada atual (FIAP / open-source gratuito):
 * - face-api FaceRecognitionNet → embedding 128D
 * - liveness didático → 3 poses (frente / esquerda / direita)
 *
 * Identidade: NUIT → GUID → template no banco (NÃO salva arquivo de imagem).
 *
 * TODO (produção INSS Moçambique — SDK pago):
 * Trocar este módulo por FaceTec / iProov / AWS Rekognition Face Liveness etc.
 */
import '../polyfills/rnFaceEnv';
import {
  FakeCanvas,
  FakeCtx,
  FakeImage,
  FakeImageData,
  FakeVideo,
} from '../polyfills/rnFaceEnv';
import * as tf from '@tensorflow/tfjs';
import * as faceapi from '@vladmandic/face-api';
import { decode as decodeBase64 } from 'base-64';
import jpeg from 'jpeg-js';

const MODEL_BASE =
  'https://cdn.jsdelivr.net/npm/@vladmandic/face-api@1.7.15/model';

/** Score mínimo do detector (rejeita oclusão forte / mão no rosto). */
const SCORE_DETECCAO_MIN = 0.62;
/** Rosto deve ocupar pelo menos esta fração da largura da imagem. */
const FACE_LARGURA_MIN = 0.18;
/** Distância mínima entre olhos / largura do rosto (mão cobrindo um olho falha). */
const OLHOS_RATIO_MIN = 0.22;

export type AnaliseCaptura = {
  descriptor: number[];
  noseOffsetX: number;
  detectionScore: number;
  faceWidthRatio: number;
  eyesRatio: number;
};

let modelosProntos = false;
let envFaceApiOk = false;

function instalarEnvFaceApi() {
  if (envFaceApiOk) return;
  // RN não é browser nem Node → initialize() do face-api deixa environment=null.
  // setEnv DEVE rodar antes de qualquer nets.*.load / detect*.
  faceapi.env.setEnv({
    Canvas: FakeCanvas as unknown as typeof HTMLCanvasElement,
    CanvasRenderingContext2D: FakeCtx as unknown as typeof CanvasRenderingContext2D,
    Image: FakeImage as unknown as typeof HTMLImageElement,
    ImageData: FakeImageData as unknown as typeof ImageData,
    Video: FakeVideo as unknown as typeof HTMLVideoElement,
    createCanvasElement: () => new FakeCanvas() as unknown as HTMLCanvasElement,
    createImageElement: () => new FakeImage() as unknown as HTMLImageElement,
    createVideoElement: () => new FakeVideo() as unknown as HTMLVideoElement,
    fetch: (...args: Parameters<typeof fetch>) => fetch(...args),
    readFile: async () => {
      throw new Error('readFile não disponível no React Native.');
    },
  });
  envFaceApiOk = true;
}

// No load do módulo — não esperar o useEffect (evita corrida com getEnv).
instalarEnvFaceApi();

function garantirPlataformaTf() {
  try {
    tf.env().set('DEBUG', false);
    if (!tf.env().platform) {
      tf.env().setPlatform('react-native', {
        fetch: (path: string, init?: RequestInit) => fetch(path, init),
        now: () => Date.now(),
        encode: (text: string) => new TextEncoder().encode(text),
        decode: (bytes: Uint8Array) => new TextDecoder().decode(bytes),
        isTypedArray: (a: unknown) =>
          ArrayBuffer.isView(a) && !(a instanceof DataView),
      } as unknown as tf.Platform);
    }
  } catch {
    // ready() abaixo falha com mensagem clara se precisar
  }
}

export async function initFaceRecognition(): Promise<void> {
  if (modelosProntos) return;

  instalarEnvFaceApi();
  garantirPlataformaTf();
  await tf.ready();
  await tf.setBackend('cpu');
  await tf.ready();

  // Baixa manifests + shards via fetch (1ª vez).
  await faceapi.nets.tinyFaceDetector.loadFromUri(MODEL_BASE);
  await faceapi.nets.faceLandmark68TinyNet.loadFromUri(MODEL_BASE);
  await faceapi.nets.faceRecognitionNet.loadFromUri(MODEL_BASE);

  modelosProntos = true;
}

function base64ParaTensor(base64: string): { tensor: tf.Tensor3D; width: number; height: number } {
  const bin = decodeBase64(base64);
  const bytes = new Uint8Array(bin.length);
  for (let i = 0; i < bin.length; i++) bytes[i] = bin.charCodeAt(i);

  const decoded = jpeg.decode(bytes, { useTArray: true });
  const { width, height, data } = decoded;
  const rgb = new Uint8Array(width * height * 3);

  for (let i = 0, j = 0; i < data.length; i += 4, j += 3) {
    rgb[j] = data[i];
    rgb[j + 1] = data[i + 1];
    rgb[j + 2] = data[i + 2];
  }

  return { tensor: tf.tensor3d(rgb, [height, width, 3]), width, height };
}

function validarQualidadeFacial(
  detection: faceapi.WithFaceDescriptor<faceapi.WithFaceLandmarks<{ detection: faceapi.FaceDetection }>>,
  imageWidth: number,
): { faceWidthRatio: number; eyesRatio: number } {
  const box = detection.detection.box;
  const faceWidthRatio = box.width / imageWidth;

  if (detection.detection.score < SCORE_DETECCAO_MIN) {
    throw new Error(
      'Rosto pouco nítido ou muito tapado. Remova a mão, óculos escuros e centralize o rosto.',
    );
  }

  if (faceWidthRatio < FACE_LARGURA_MIN) {
    throw new Error('Aproxime o rosto da câmera (rosto pequeno demais).');
  }

  const leftEye = detection.landmarks.getLeftEye();
  const rightEye = detection.landmarks.getRightEye();
  const leftCx = leftEye.reduce((s, p) => s + p.x, 0) / leftEye.length;
  const rightCx = rightEye.reduce((s, p) => s + p.x, 0) / rightEye.length;
  const eyesRatio = Math.abs(rightCx - leftCx) / box.width;

  if (eyesRatio < OLHOS_RATIO_MIN) {
    throw new Error(
      'Olhos não visíveis o suficiente (possível mão/objeto no rosto). Mostre o rosto inteiro.',
    );
  }

  return { faceWidthRatio, eyesRatio };
}

export async function analisarCaptura(base64: string): Promise<AnaliseCaptura> {
  await initFaceRecognition();

  const { tensor, width } = base64ParaTensor(base64);
  try {
    const input = tensor as unknown as faceapi.TNetInput;
    const detection = await faceapi
      .detectSingleFace(input, new faceapi.TinyFaceDetectorOptions({ inputSize: 320, scoreThreshold: 0.45 }))
      .withFaceLandmarks(true)
      .withFaceDescriptor();

    if (!detection) {
      throw new Error('Rosto não detectado. Centralize o rosto na câmera com boa iluminação.');
    }

    const qualidade = validarQualidadeFacial(detection, width);
    const box = detection.detection.box;
    const nose = detection.landmarks.getNose()[3];
    const faceCenterX = box.x + box.width / 2;
    const noseOffsetX = (nose.x - faceCenterX) / box.width;

    return {
      descriptor: Array.from(detection.descriptor),
      noseOffsetX,
      detectionScore: detection.detection.score,
      faceWidthRatio: qualidade.faceWidthRatio,
      eyesRatio: qualidade.eyesRatio,
    };
  } finally {
    tensor.dispose();
  }
}

/** Usa a captura mais frontal e nítida para o match 1:1. */
export function escolherEmbeddingReferencia(capturas: AnaliseCaptura[]): number[] {
  const ranqueadas = [...capturas].sort((a, b) => {
    const scoreA = a.detectionScore - Math.abs(a.noseOffsetX) * 0.5;
    const scoreB = b.detectionScore - Math.abs(b.noseOffsetX) * 0.5;
    return scoreB - scoreA;
  });
  return ranqueadas[0].descriptor;
}

export function calcularLiveness(capturas: AnaliseCaptura[]): {
  scoreLiveness: number;
  movimentosDetectados: boolean;
} {
  if (capturas.length < 3) {
    return { scoreLiveness: 0, movimentosDetectados: false };
  }

  const offsets = capturas.map((c) => c.noseOffsetX);
  const amplitude = Math.max(...offsets) - Math.min(...offsets);
  const mediaScore = capturas.reduce((s, c) => s + c.detectionScore, 0) / capturas.length;

  // Endurecido: exige virada mais clara da cabeça
  const movimentoOk = amplitude >= 0.12;
  const qualidadeOk = capturas.every((c) => c.detectionScore >= SCORE_DETECCAO_MIN);
  const scoreLiveness = Math.min(
    98,
    Math.round((mediaScore * 45 + amplitude * 350 + (movimentoOk ? 25 : 0)) * 10) / 10,
  );

  return {
    scoreLiveness,
    movimentosDetectados: movimentoOk && qualidadeOk && scoreLiveness >= 75,
  };
}

export function similaridadeLocal(a: number[], b: number[]): number {
  let dot = 0;
  let normA = 0;
  let normB = 0;
  for (let i = 0; i < a.length; i++) {
    dot += a[i] * b[i];
    normA += a[i] * a[i];
    normB += b[i] * b[i];
  }
  if (normA === 0 || normB === 0) return 0;
  return dot / (Math.sqrt(normA) * Math.sqrt(normB));
}
