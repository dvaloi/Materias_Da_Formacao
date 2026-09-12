using ProvaVida.Domain.Entities;

namespace ProvaVida.Domain.Repositories;

public interface IRegistroProvaVidaRepository
{
    Task AdicionarAsync(RegistroProvaVida registro, CancellationToken ct = default);
    Task SalvarAlteracoesAsync(CancellationToken ct = default);
    Task<RegistroProvaVida?> ObterUltimoPorContribuinteAsync(Guid contribuinteId, CancellationToken ct = default);
    Task<IReadOnlyList<RegistroProvaVida>> ListarPorContribuinteAsync(Guid contribuinteId, CancellationToken ct = default);
    Task<RegistroProvaVida?> ObterUltimoPorNuitAsync(string nuit, CancellationToken ct = default);
    Task<RegistroProvaVida?> ObterCadastroPorContribuinteAsync(Guid contribuinteId, CancellationToken ct = default);
}
