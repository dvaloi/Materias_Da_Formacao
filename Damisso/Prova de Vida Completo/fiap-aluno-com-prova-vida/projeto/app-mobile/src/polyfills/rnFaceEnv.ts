/**
 * Face-api exige "browser env" completo (Canvas, Image, ImageData…).
 * No React Native isso não existe → getEnv falha.
 * Este arquivo DEVE ser o 1º import do App.
 *
 * Em Hermes, propriedades em `global` viram identificadores globais
 * (necessários para isBrowser() do face-api).
 */
const g = (typeof global !== 'undefined' ? global : globalThis) as any;

class FakeCanvas {
  width = 0;
  height = 0;
  getContext() {
    return {
      fillRect() {},
      clearRect() {},
      getImageData: (_x: number, _y: number, w: number, h: number) =>
        new FakeImageData(new Uint8ClampedArray(w * h * 4), w, h),
      putImageData() {},
      drawImage() {},
      measureText: () => ({ width: 0 }),
      fillText() {},
      strokeRect() {},
      beginPath() {},
      moveTo() {},
      lineTo() {},
      stroke() {},
      arc() {},
      fill() {},
      canvas: null as unknown,
    };
  }
  toDataURL() {
    return '';
  }
}

class FakeImage {
  width = 0;
  height = 0;
  naturalWidth = 0;
  naturalHeight = 0;
  complete = true;
  src = '';
  onload: ((ev?: unknown) => void) | null = null;
  onerror: ((ev?: unknown) => void) | null = null;
  addEventListener() {}
  removeEventListener() {}
}

class FakeVideo extends FakeImage {
  videoWidth = 0;
  videoHeight = 0;
  readyState = 4;
  play() {
    return Promise.resolve();
  }
}

class FakeImageData {
  data: Uint8ClampedArray;
  width: number;
  height: number;
  constructor(data: Uint8ClampedArray, width: number, height: number) {
    this.data = data;
    this.width = width;
    this.height = height;
  }
}

class FakeCtx {}

if (!g.window) g.window = g;
if (!g.document) {
  g.document = {
    createElement: (tag: string) => {
      if (tag === 'canvas') return new FakeCanvas();
      if (tag === 'img') return new FakeImage();
      if (tag === 'video') return new FakeVideo();
      return {};
    },
    getElementById: () => null,
  };
}

g.HTMLCanvasElement = FakeCanvas;
g.HTMLImageElement = FakeImage;
g.HTMLVideoElement = FakeVideo;
g.ImageData = FakeImageData;
g.CanvasRenderingContext2D = FakeCtx;
g.Canvas = FakeCanvas;
g.Image = FakeImage;
g.Video = FakeVideo;

if (!g.navigator) {
  g.navigator = { userAgent: 'ReactNative', product: 'ReactNative' };
}

export {
  FakeCanvas,
  FakeImage,
  FakeVideo,
  FakeImageData,
  FakeCtx,
};
