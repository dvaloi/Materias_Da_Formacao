using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Contribuintes.Application.DTOs;
using Contribuintes.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Contribuintes.Application.Services;

public class AuthService(IConfiguration configuration) : IAuthService
{
    // Demo aula — usuários fixos (em produção viriam do banco). Login: admin / 123
    private static readonly Dictionary<string, (string Senha, string Nome)> Usuarios = new()
    {
        ["admin"] = ("123", "Administrador INSS — Maputo"),
        ["operador"] = ("123", "Operador INSS — Matola")
    };

    public Task<LoginResponse?> AutenticarAsync(LoginRequest request, CancellationToken ct = default)
    {
        if (!Usuarios.TryGetValue(request.Email.ToLowerInvariant(), out var user))
            return Task.FromResult<LoginResponse?>(null);

        if (user.Senha != request.Senha)
            return Task.FromResult<LoginResponse?>(null);

        var expiraEm = DateTime.UtcNow.AddHours(8);
        var token = GerarToken(request.Email, user.Nome, expiraEm);

        return Task.FromResult<LoginResponse?>(new LoginResponse(token, user.Nome, expiraEm));
    }

    private string GerarToken(string email, string nome, DateTime expiraEm)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "InssMozambique2026ChaveSecretaMin32Chars!"));

        var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, nome),
            new Claim(ClaimTypes.Role, "Operador")
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"] ?? "InssContribuintes",
            audience: configuration["Jwt:Audience"] ?? "InssPortal",
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
