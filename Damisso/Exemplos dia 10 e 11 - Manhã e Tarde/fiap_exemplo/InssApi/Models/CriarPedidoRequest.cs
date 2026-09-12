namespace InssApi.Models;

/// <summary>DTO do POST — só o que o cidadão manda no cadastro.</summary>
public class CriarPedidoRequest
{
    public int ContribuinteId { get; set; }
    public string Tipo { get; set; } = string.Empty;
}
