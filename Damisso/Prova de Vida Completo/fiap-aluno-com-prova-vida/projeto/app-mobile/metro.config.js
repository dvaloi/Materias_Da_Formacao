const { getDefaultConfig } = require('expo/metro-config');
const path = require('path');

/** @type {import('expo/metro-config').MetroConfig} */
const config = getDefaultConfig(__dirname);

const faceApiEsm = path.resolve(
  __dirname,
  'node_modules/@vladmandic/face-api/dist/face-api.esm.js',
);

// Metro usa "main" (Node). Face-api no Node espera canvas nativo → getEnv quebra no APK.
// Forçamos o build browser/ESM e injetamos env via faceapi.env.setEnv.
const previousResolveRequest = config.resolver.resolveRequest;
config.resolver.resolveRequest = (context, moduleName, platform) => {
  if (moduleName === '@vladmandic/face-api') {
    return { filePath: faceApiEsm, type: 'sourceFile' };
  }
  if (previousResolveRequest) {
    return previousResolveRequest(context, moduleName, platform);
  }
  return context.resolveRequest(context, moduleName, platform);
};

module.exports = config;
