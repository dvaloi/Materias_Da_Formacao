namespace InssApi.Models;

/// <summary>Ficha do pedido de benefício (mesmo padrão do Contribuinte).</summary>
public class PedidoBeneficio
{
    public int Id { get; set; }
    public int ContribuinteId { get; set; }
    public string Tipo { get; set; } = string.Empty; // ex.: Velhice, Invalidez
    public string Status { get; set; } = "Aberto";
}
