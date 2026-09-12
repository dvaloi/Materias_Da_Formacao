using ProvaVida.Domain.Entities;

namespace ProvaVida.Domain.Repositories;

public interface IPerfilBiometricoRepository
{
    Task<PerfilBiometrico?> ObterPorContribuinteAsync(Guid contribuinteId, CancellationToken ct = default);
    Task AdicionarAsync(PerfilBiometrico perfil, CancellationToken ct = default);
    Task SalvarAlteracoesAsync(CancellationToken ct = default);
}
