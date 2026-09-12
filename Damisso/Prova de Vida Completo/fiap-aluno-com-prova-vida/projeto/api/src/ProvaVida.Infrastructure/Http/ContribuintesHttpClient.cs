using System.Net.Http.Json;
using ProvaVida.Application.DTOs;
using ProvaVida.Application.Interfaces;

namespace ProvaVida.Infrastructure.Http;

public class ContribuintesHttpClient(HttpClient httpClient) : IContribuintesClient
{
    public async Task<PensionistaExternoDto?> ObterPensionistaPorNuitAsync(string nuit, CancellationToken ct = default)
    {
        using var response = await httpClient.GetAsync($"api/contribuintes/pensionistas/por-nuit/{nuit}", ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PensionistaExternoDto>(cancellationToken: ct);
    }
}
