// M13 — Estado de erro reutilizável (lista, form, detalhe)
export default function ErrorMessage({ mensagem, onRetry }) {
  if (!mensagem) return null;

  return (
    <div className="alert alert-error" role="alert">
      <strong>Erro:</strong> {mensagem}
      {onRetry && (
        <button type="button" className="btn-link" onClick={onRetry}>
          Tentar novamente
        </button>
      )}
    </div>
  );
}
