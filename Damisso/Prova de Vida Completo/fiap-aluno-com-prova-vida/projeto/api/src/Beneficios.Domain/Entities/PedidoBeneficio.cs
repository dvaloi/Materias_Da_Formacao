using Beneficios.Domain.Enums;
using Beneficios.Domain.Exceptions;

namespace Beneficios.Domain.Entities;

/// <summary>
/// Aggregate Root — pedido de benefício do INSS.
/// Referencia contribuinte por ID (sem acoplar ao outro microsserviço — Bounded Context).
/// </summary>
public class PedidoBeneficio
{
    public Guid Id { get; private set; }
    public Guid ContribuinteId { get; private set; }
    public TipoBeneficio Tipo { get; private set; }
    public decimal ValorSolicitado { get; private set; }
    public StatusPedido Status { get; private set; }
    public string? MotivoRejeicao { get; private set; }
    public DateTime DataPedido { get; private set; }

    private PedidoBeneficio() { }

    public static PedidoBeneficio Criar(Guid contribuinteId, TipoBeneficio tipo, decimal valorSolicitado)
    {
        if (contribuinteId == Guid.Empty)
            throw new DomainException("ContribuinteId é obrigatório.");

        if (valorSolicitado <= 0)
            throw new DomainException("Valor solicitado deve ser maior que zero.");

        return new PedidoBeneficio
        {
            Id = Guid.NewGuid(),
            ContribuinteId = contribuinteId,
            Tipo = tipo,
            ValorSolicitado = valorSolicitado,
            Status = StatusPedido.Pendente,
            DataPedido = DateTime.UtcNow
        };
    }

    public void Aprovar()
    {
        if (Status is StatusPedido.Aprovado or StatusPedido.Rejeitado)
            throw new DomainException("Pedido já foi finalizado.");

        Status = StatusPedido.Aprovado;
    }

    public void Rejeitar(string motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new DomainException("Motivo de rejeição é obrigatório.");

        if (Status is StatusPedido.Aprovado or StatusPedido.Rejeitado)
            throw new DomainException("Pedido já foi finalizado.");

        Status = StatusPedido.Rejeitado;
        MotivoRejeicao = motivo.Trim();
    }

    public void IniciarAnalise()
    {
        if (Status != StatusPedido.Pendente)
            throw new DomainException("Apenas pedidos pendentes podem ser analisados.");

        Status = StatusPedido.EmAnalise;
    }
}
