// M13 — Estado de loading reutilizável (role="status" ajuda leitores de tela)
export default function LoadingSpinner({ texto = 'Carregando...' }) {
  return (
    <div className="loading" role="status" aria-live="polite">
      <div className="spinner" aria-hidden="true" />
      <span>{texto}</span>
    </div>
  );
}
