namespace InssApi.Models;

public class Contribuinte
{
    public int Id { get; set; }
    public string Nuit { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public DateOnly? DataNascimento { get; set; }
    public string Status { get; set; } = "Ativo";
}
