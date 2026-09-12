# Baixa modelos face-api (TinyFaceDetector + landmarks + recognition) para o app mobile.
$ErrorActionPreference = "Stop"
$dest = Join-Path $PSScriptRoot "..\projeto\app-mobile\assets\models"
New-Item -ItemType Directory -Force -Path $dest | Out-Null

$base = "https://raw.githubusercontent.com/vladmandic/face-api/master/model"
$files = @(
  "tiny_face_detector_model-weights_manifest.json",
  "tiny_face_detector_model-shard1",
  "face_landmark_68_tiny_model-weights_manifest.json",
  "face_landmark_68_tiny_model-shard1",
  "face_recognition_model-weights_manifest.json",
  "face_recognition_model-shard1",
  "face_recognition_model-shard2"
)

foreach ($f in $files) {
  $out = Join-Path $dest $f
  if (-not (Test-Path $out)) {
    Write-Host "Baixando $f..."
    Invoke-WebRequest -Uri "$base/$f" -OutFile $out
  } else {
    Write-Host "OK $f"
  }
}

Write-Host "Modelos em $dest"
