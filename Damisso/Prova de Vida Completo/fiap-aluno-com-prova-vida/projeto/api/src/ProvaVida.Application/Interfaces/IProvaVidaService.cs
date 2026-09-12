using ProvaVida.Application.DTOs;

namespace ProvaVida.Application.Interfaces;

public interface IProvaVidaService
{
    /// <summary>1ª vez — cria cadastro biométrico + registro único.</summary>
    Task<RegistroProvaVidaDto> CadastrarAsync(RegistrarProvaVidaRequest request, CancellationToken ct = default);

    /// <summary>
    /// Consulta com câmera (prova ainda válida) — liveness + match facial.
    /// Não cria cadastro nem renova validade.
    /// </summary>
    Task<RegistroProvaVidaDto> ConsultarAsync(RegistrarProvaVidaRequest request, CancellationToken ct = default);

    /// <summary>Renovação — compara com cadastro existente e atualiza o mesmo registro.</summary>
    Task<RegistroProvaVidaDto> RenovarAsync(RegistrarProvaVidaRequest request, CancellationToken ct = default);

    Task<RegistroProvaVidaDto?> ObterUltimaAsync(Guid contribuinteId, CancellationToken ct = default);
    Task<IReadOnlyList<RegistroProvaVidaDto>> ListarPorContribuinteAsync(Guid contribuinteId, CancellationToken ct = default);
    Task<StatusAcessoBeneficioDto> ObterStatusAcessoAsync(string nuit, CancellationToken ct = default);
    Task<StatusAcessoBeneficioDto> ValidarAcessoBeneficioAsync(string nuit, CancellationToken ct = default);
}

public interface IContribuintesClient
{
    Task<PensionistaExternoDto?> ObterPensionistaPorNuitAsync(string nuit, CancellationToken ct = default);
}
