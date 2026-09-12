using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Contribuintes.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Contribuintes.IntegrationTests;

/// <summary>
/// Testes de integração com WebApplicationFactory — M15.
/// Testam a API completa (HTTP → Controller → Service → DB).
/// </summary>
public class ContribuintesApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ContribuintesApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> ObterTokenAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(
            "admin", "123"));

        response.EnsureSuccessStatusCode();
        var login = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return login!.Token;
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_DeveRetornarToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(
            "admin", "123"));

        response.EnsureSuccessStatusCode();
        var login = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(login?.Token);
    }

    [Fact]
    public async Task Login_ComCredenciaisInvalidas_DeveRetornar401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(
            "admin", "SenhaErrada"));

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ListarContribuintes_ComToken_DeveRetornar200()
    {
        var token = await ObterTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/contribuintes");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var lista = JsonSerializer.Deserialize<List<ContribuinteDto>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(lista);
        Assert.NotEmpty(lista);
    }

    [Fact]
    public async Task CriarContribuinte_ComDadosValidos_DeveRetornar201()
    {
        var token = await ObterTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CriarContribuinteRequest(
            "111222333", "Teste Integração", new DateOnly(1992, 5, 10), 25000m);

        var response = await _client.PostAsJsonAsync("/api/contribuintes", request);

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }
}
