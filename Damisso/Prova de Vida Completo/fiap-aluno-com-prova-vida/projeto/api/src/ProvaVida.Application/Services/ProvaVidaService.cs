using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using ProvaVida.Application.DTOs;
using ProvaVida.Application.Interfaces;
using ProvaVida.Application.Mappings;
using ProvaVida.Domain.Entities;
using ProvaVida.Domain.Enums;
using ProvaVida.Domain.Exceptions;
using ProvaVida.Domain.Repositories;
using ProvaVida.Domain.Services;

namespace ProvaVida.Application.Services;

public class ProvaVidaService(
    IRegistroProvaVidaRepository repository,
    IPerfilBiometricoRepository perfilRepository,
    IContribuintesClient contribuintesClient,
    IConfiguration configuration) : IProvaVidaService
{
    private const decimal ScoreMinimoLiveness = 70m;

    private int MesesValidade =>
        int.TryParse(configuration["ProvaVida:ValidadeMeses"], out var meses) && meses > 0
            ? meses
            : ValidadeProvaVida.MesesPadrao;

    public async Task<RegistroProvaVidaDto> CadastrarAsync(RegistrarProvaVidaRequest request, CancellationToken ct = default)
    {
        var pensionista = await ObterPensionistaAsync(request.Nuit, ct);
        var perfil = await perfilRepository.ObterPorContribuinteAsync(pensionista.Id, ct);
        var registro = await repository.ObterCadastroPorContribuinteAsync(pensionista.Id, ct);

        if (perfil is not null || registro is not null)
            throw new DomainException(
                "Cadastro biométrico já existe. Use Consultar (prova válida) ou Renovar (expirada).");

        ValidarLiveness(request);
        var embedding = ValidarEmbedding(request.FaceEmbedding, obrigatorio: true);
        var faceHash = GerarFaceHash(request.ImagemBase64, pensionista.Id, embedding!);

        perfil = PerfilBiometrico.Criar(pensionista.Id, pensionista.Nuit, faceHash, embedding!);
        await perfilRepository.AdicionarAsync(perfil, ct);

        registro = RegistroProvaVida.Registrar(
            pensionista.Id,
            pensionista.Nuit,
            request.ScoreLiveness,
            faceHash,
            request.MovimentosDetectados,
            ScoreMinimoLiveness);

        await repository.AdicionarAsync(registro, ct);
        await perfilRepository.SalvarAlteracoesAsync(ct);

        return ProvaVidaMapper.ParaDto(registro, TipoOperacaoProvaVida.Cadastro, identidadeConfirmada: true, 100m);
    }

    public async Task<RegistroProvaVidaDto> ConsultarAsync(RegistrarProvaVidaRequest request, CancellationToken ct = default)
    {
        var pensionista = await ObterPensionistaAsync(request.Nuit, ct);
        ValidarLiveness(request);

        var perfil = await perfilRepository.ObterPorContribuinteAsync(pensionista.Id, ct)
            ?? throw new DomainException("Nenhum cadastro biométrico. Faça o cadastro inicial (1ª vez).");

        var registro = await repository.ObterCadastroPorContribuinteAsync(pensionista.Id, ct)
            ?? throw new DomainException("Nenhuma Prova de Vida cadastrada. Faça o cadastro inicial.");

        var agora = DateTime.UtcNow;
        if (!ValidadeProvaVida.EstaValida(registro.RealizadoEm, agora, MesesValidade))
            throw new DomainException(
                "Prova expirada. Use Renovar (câmera + comparação facial) para liberar o benefício.");

        var possuiTemplate = perfil.ObterEmbedding() is { Count: > 0 };
        decimal? similaridade = null;
        var identidadeOk = false;

        if (possuiTemplate)
        {
            var embedding = ValidarEmbedding(request.FaceEmbedding, obrigatorio: true)!;
            similaridade = perfil.VerificarIdentidade(embedding, request.ScoreLiveness);
            identidadeOk = true;
        }
        else
        {
            // Cadastro antigo só com hash (ex.: webcam) — consulta autentica por liveness.
            identidadeOk = request.MovimentosDetectados && request.ScoreLiveness >= ScoreMinimoLiveness;
            if (!identidadeOk)
                throw new DomainException("Liveness insuficiente na consulta.");
        }

        // Não altera RealizadoEm / validade — só autentica como apps de banco/gov.
        return ProvaVidaMapper.ParaDto(
            registro,
            TipoOperacaoProvaVida.Consulta,
            identidadeConfirmada: identidadeOk,
            similaridade);
    }

    public async Task<RegistroProvaVidaDto> RenovarAsync(RegistrarProvaVidaRequest request, CancellationToken ct = default)
    {
        var pensionista = await ObterPensionistaAsync(request.Nuit, ct);
        ValidarLiveness(request);
        var embedding = ValidarEmbedding(request.FaceEmbedding, obrigatorio: true)!;
        var faceHash = GerarFaceHash(request.ImagemBase64, pensionista.Id, embedding);

        var perfil = await perfilRepository.ObterPorContribuinteAsync(pensionista.Id, ct)
            ?? throw new DomainException("Nenhum cadastro biométrico encontrado. Faça o cadastro inicial pelo app mobile.");

        var registro = await repository.ObterCadastroPorContribuinteAsync(pensionista.Id, ct)
            ?? throw new DomainException("Nenhum cadastro de Prova de Vida encontrado. Faça o cadastro inicial primeiro.");

        var agora = DateTime.UtcNow;
        if (ValidadeProvaVida.EstaValida(registro.RealizadoEm, agora, MesesValidade))
            throw new DomainException(
                $"Prova ainda válida por {ValidadeProvaVida.DiasRestantes(registro.RealizadoEm, agora, MesesValidade)} dias. " +
                "Use Consultar (câmera) para autenticar — renovação só quando expirar.");

        var similaridade = perfil.ConfirmarIdentidadeNaRenovacao(embedding, request.ScoreLiveness);
        registro.Renovar(request.ScoreLiveness, faceHash, request.MovimentosDetectados, ScoreMinimoLiveness);

        await perfilRepository.SalvarAlteracoesAsync(ct);

        return ProvaVidaMapper.ParaDto(registro, TipoOperacaoProvaVida.Renovacao, identidadeConfirmada: true, similaridade);
    }

    public async Task<StatusAcessoBeneficioDto> ObterStatusAcessoAsync(string nuit, CancellationToken ct = default) =>
        await MontarStatusAsync(nuit, lancarSeInvalido: false, ct);

    public async Task<StatusAcessoBeneficioDto> ValidarAcessoBeneficioAsync(string nuit, CancellationToken ct = default) =>
        await MontarStatusAsync(nuit, lancarSeInvalido: true, ct);

    public async Task<RegistroProvaVidaDto?> ObterUltimaAsync(Guid contribuinteId, CancellationToken ct = default)
    {
        var entity = await repository.ObterCadastroPorContribuinteAsync(contribuinteId, ct);
        return entity is null
            ? null
            : ProvaVidaMapper.ParaDto(entity, TipoOperacaoProvaVida.Cadastro, true);
    }

    public async Task<IReadOnlyList<RegistroProvaVidaDto>> ListarPorContribuinteAsync(Guid contribuinteId, CancellationToken ct = default)
    {
        var entity = await repository.ObterCadastroPorContribuinteAsync(contribuinteId, ct);
        return entity is null
            ? []
            : [ProvaVidaMapper.ParaDto(entity, TipoOperacaoProvaVida.Cadastro, true)];
    }

    private async Task<StatusAcessoBeneficioDto> MontarStatusAsync(string nuit, bool lancarSeInvalido, CancellationToken ct)
    {
        var pensionista = await ObterPensionistaAsync(nuit, ct);
        var registro = await repository.ObterCadastroPorContribuinteAsync(pensionista.Id, ct);
        var perfil = await perfilRepository.ObterPorContribuinteAsync(pensionista.Id, ct);
        var agora = DateTime.UtcNow;
        var possuiTemplateFacial = perfil?.ObterEmbedding() is { Count: > 0 };

        if (registro is null)
        {
            var semProva = new StatusAcessoBeneficioDto(
                pensionista.Id, pensionista.Nuit, pensionista.Nome,
                PodeAcessarBeneficio: false, ProvaValida: false,
                PossuiProvaRegistrada: false,
                PossuiCadastroBiometrico: false,
                RequerCadastroInicial: true,
                RequerRenovacao: false,
                ProximaAcao: "Cadastrar",
                UltimaProvaEm: null, ValidaAte: null, DiasRestantes: 0,
                MesesValidade: MesesValidade,
                MotivoBloqueio: "Nenhuma Prova de Vida cadastrada.",
                AcaoRequerida: "Abrir câmera — cadastro biométrico (1ª vez).");

            if (lancarSeInvalido) throw new DomainException(semProva.MotivoBloqueio!);
            return semProva;
        }

        var valida = ValidadeProvaVida.EstaValida(registro.RealizadoEm, agora, MesesValidade);
        var validoAte = ValidadeProvaVida.CalcularValidoAte(registro.RealizadoEm, MesesValidade);
        var diasRestantes = ValidadeProvaVida.DiasRestantes(registro.RealizadoEm, agora, MesesValidade);

        string? motivo = null;
        string? acao;
        string proximaAcao;

        if (valida)
        {
            proximaAcao = "Consultar";
            acao = "Abrir câmera — autenticar rosto no cadastro (não renova validade).";
        }
        else
        {
            motivo = $"Prova de Vida expirada (validade: {MesesValidade} meses).";
            acao = possuiTemplateFacial
                ? "Abrir câmera — renovar comparando com o cadastro."
                : "Abrir câmera — refazer cadastro biométrico (template ausente).";
            proximaAcao = possuiTemplateFacial ? "Renovar" : "Cadastrar";
        }

        var dto = new StatusAcessoBeneficioDto(
            pensionista.Id, pensionista.Nuit, pensionista.Nome,
            PodeAcessarBeneficio: valida, ProvaValida: valida,
            PossuiProvaRegistrada: true,
            PossuiCadastroBiometrico: possuiTemplateFacial || perfil is not null,
            RequerCadastroInicial: false,
            RequerRenovacao: !valida && possuiTemplateFacial,
            ProximaAcao: proximaAcao,
            UltimaProvaEm: registro.RealizadoEm,
            ValidaAte: validoAte,
            DiasRestantes: diasRestantes,
            MesesValidade: MesesValidade,
            MotivoBloqueio: motivo,
            AcaoRequerida: acao);

        if (lancarSeInvalido && !valida)
            throw new DomainException(motivo!);

        return dto;
    }

    private async Task<PensionistaExternoDto> ObterPensionistaAsync(string nuit, CancellationToken ct) =>
        await contribuintesClient.ObterPensionistaPorNuitAsync(nuit, ct)
        ?? throw new DomainException("NUIT não pertence a um pensionista cadastrado.");

    private static void ValidarLiveness(RegistrarProvaVidaRequest request)
    {
        if (!request.MovimentosDetectados)
            throw new DomainException("Liveness não confirmado — movimentos faciais não detectados.");
        if (request.ScoreLiveness < ScoreMinimoLiveness)
            throw new DomainException($"Score de liveness insuficiente ({request.ScoreLiveness:F0}%). Mínimo: {ScoreMinimoLiveness:F0}%.");
        if (string.IsNullOrWhiteSpace(request.ImagemBase64))
            throw new DomainException("Imagem biométrica é obrigatória.");
    }

    private static float[]? ValidarEmbedding(float[]? embedding, bool obrigatorio)
    {
        if (embedding is null || embedding.Length < 64)
        {
            if (obrigatorio)
                throw new DomainException(
                    "Embedding facial obrigatório. Use o app com reconhecimento facial (face-api).");
            return null;
        }

        return embedding;
    }

    private static string GerarFaceHash(string imagemBase64, Guid contribuinteId, float[] embedding)
    {
        var embeddingPart = string.Join(",", embedding.Select(v => v.ToString("F4")));
        var payload = Encoding.UTF8.GetBytes($"{contribuinteId}:{imagemBase64.Trim()}:{embeddingPart}");
        return Convert.ToHexString(SHA256.HashData(payload));
    }
}
