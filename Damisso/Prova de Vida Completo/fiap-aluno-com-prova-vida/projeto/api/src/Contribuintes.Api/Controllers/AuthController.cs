using Contribuintes.Application.DTOs;
using Contribuintes.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Contribuintes.Api.Controllers;

// Login público (sem [Authorize]). Devolve JWT — o portal guarda e envia no Bearer.
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        // Demo aula: admin / 123 (ver AuthService)
        var result = await authService.AutenticarAsync(request, ct);
        return result is null ? Unauthorized(new { mensagem = "Credenciais inválidas." }) : Ok(result);
    }
}
