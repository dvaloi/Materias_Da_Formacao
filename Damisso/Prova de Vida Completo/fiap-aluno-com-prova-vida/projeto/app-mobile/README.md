# INSS Prova de Vida — App Mobile (Expo)

App **standalone** com **reconhecimento facial real** (face-api 128D) + **liveness** por movimento de cabeça.

## Pré-requisitos

- Node 20+
- APIs locais: Contribuintes `:5001` e ProvaVida `:5003`
- Android Studio (emulador) **ou** celular Android na mesma Wi‑Fi do PC
- Java 17+ (para build local)

## 1. Subir as APIs no PC

```powershell
cd C:\fiap\projeto\api
docker compose up -d
dotnet run --project src\Contribuintes.Api
dotnet run --project src\ProvaVida.Api
```

## 2. Configurar IP do PC no celular

Edite `app.json` → `expo.extra` com o **IP da sua máquina** na rede Wi‑Fi:

```json
"contribuintesApiUrl": "http://SEU_IP:5001",
"provaVidaApiUrl": "http://SEU_IP:5003"
```

Descobrir IP: `ipconfig` → IPv4 (ex.: `192.168.0.10`).

| Ambiente | URL |
|----------|-----|
| Emulador Android | `http://10.0.2.2:5001` / `:5003` |
| Celular físico | `http://SEU_IP:5001` / `:5003` |

## 3. Instalar e rodar (dev)

```bash
cd projeto/app-mobile
npm install --legacy-peer-deps
npx expo start --dev-client
```

## Identidade: NUIT (não arquivo de imagem)

Fluxo alinhado ao diagrama (Contribuinte → GUID → biometria → Prova de Vida):

1. Usuário digita **NUIT** (documento em Moçambique — papel do CPF no Brasil)
2. App busca pensionista na API Contribuintes (`:5001`)
3. Câmera gera **embedding 128D** (template) — a foto **não** fica salva no disco
4. API ProvaVida (`:5003`) grava/compara template ligado ao **GUID** do contribuinte

| Hoje | Produção |
|------|----------|
| Match ≥ **72%** + qualidade (olhos visíveis, rosto grande) | SDK pago (TODO) |
| Liveness por movimento de cabeça | FaceTec / iProov / AWS Liveness |

## 4. Gerar APK Android

### Opção A — EAS (recomendado se não tiver Android SDK local)

```powershell
cd C:\fiap\projeto\app-mobile
npm install -g eas-cli
eas login
eas build -p android --profile preview
```

Baixe o `.apk` no link do Expo. No celular: mesma Wi‑Fi do PC · IP em `app.json`.

### Opção B — Android Studio (local)

1. Instale [Android Studio](https://developer.android.com/studio) + SDK + JDK 17  
2. Defina `ANDROID_HOME` = pasta do SDK  
3. Rode:

```powershell
cd C:\fiap\projeto\app-mobile
npm install --legacy-peer-deps
npx expo prebuild --platform android
cd android
.\gradlew assembleRelease
```

APK: `android\app\build\outputs\apk\release\app-release.apk`

### iOS

No Windows **não** gera IPA local. Use EAS (`eas build -p ios`) com conta Apple, ou Mac + Xcode.  
TODO: build iOS quando houver certificados.

## 5. Testar no celular (após APK)

1. APIs no PC (`:5001` e `:5003`)  
2. `app.json` com IP do PC (ex. `192.168.0.14`)  
3. Instale o APK · NUIT `300400504` (ou outro sem cadastro)  
4. Cadastro 1× com seu rosto → Consultar com você (passa) → outra pessoa (falha ≥72%)  
5. Mão no rosto / olhos tapados → deve **rejeitar** qualidade

## Tecnologia

- **face-api** — FaceRecognitionNet 128D
- **Liveness** — 3 poses + amplitude mínima endurecida
- **API** — `ComparadorFacial.LimiarMinimo = 0.72`

Roteiro: `docs/roteiro-prova-vida.html`
