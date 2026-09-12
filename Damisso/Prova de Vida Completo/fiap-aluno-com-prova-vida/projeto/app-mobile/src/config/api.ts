import Constants from 'expo-constants';

/**
 * URLs das APIs locais.
 * - Emulador Android: 10.0.2.2 aponta para localhost do PC.
 * - Celular físico: troque pelo IP da máquina na rede (ex.: 192.168.1.10).
 * TODO: apontar para APIs publicadas na nuvem (Render/Railway).
 */
const extra = Constants.expoConfig?.extra ?? {};

export const API = {
  contribuintes: extra.contribuintesApiUrl as string,
  provaVida: extra.provaVidaApiUrl as string,
};
