using System.Security.Claims;
using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ibnelve.Api.Controllers.Admin;

/// <summary>Ações sobre a própria conta do usuário logado — qualquer usuário autenticado, não só ADMIN
/// (hoje só existe o papel ADMIN, mas trocar a própria senha não é uma ação administrativa sobre outros).</summary>
[ApiController]
[Route("api/admin/conta")]
[Authorize]
public class ContaController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpPost("trocar-senha")]
    public async Task<IActionResult> TrocarSenha(TrocarSenhaRequest request)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var usuario = await userManager.FindByIdAsync(usuarioId);
        if (usuario is null)
        {
            return Unauthorized();
        }

        var resultado = await userManager.ChangePasswordAsync(usuario, request.SenhaAtual, request.NovaSenha);
        if (!resultado.Succeeded)
        {
            return BadRequest(new { message = string.Join(" ", resultado.Errors.Select(e => e.Description)) });
        }

        return NoContent();
    }
}
