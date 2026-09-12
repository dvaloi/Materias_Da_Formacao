namespace InssApi.Models;

public class PedidoBeneficio
{
    public int Id { get; set; }
    public int ContribuinteId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Status { get; set; } = "Aberto";
}
