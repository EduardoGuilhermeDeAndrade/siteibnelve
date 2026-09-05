using System.Security.Claims;
using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ibnelve.Api.Controllers.Admin;

/// <summary>
/// Gerencia usuários do papel ADMIN (perfil amplo do MVP — ver CLAUDE.md). Não permite exclusão
/// definitiva, só ativar/desativar (via lockout do Identity), e nunca deixa desativar a própria
/// conta logada nem o último ADMIN ativo restante, pra ninguém ficar trancado pra fora do Portal.
/// </summary>
[ApiController]
[Route("api/admin/usuarios")]
[Authorize(Roles = AdminSeeder.PapelAdmin)]
public class UsuariosController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UsuarioAdminDto>>> Listar()
    {
        var usuarios = await userManager.GetUsersInRoleAsync(AdminSeeder.PapelAdmin);

        var resultado = new List<UsuarioAdminDto>();
        foreach (var usuario in usuarios.OrderBy(u => u.NomeCompleto))
        {
            resultado.Add(await ParaDtoAsync(usuario));
        }

        return resultado;
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioAdminDto>> Criar(UsuarioCriarRequest request)
    {
        var usuario = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true,
            NomeCompleto = request.NomeCompleto
        };

        var resultado = await userManager.CreateAsync(usuario, request.Senha);
        if (!resultado.Succeeded)
        {
            return BadRequest(new { message = string.Join(" ", resultado.Errors.Select(e => e.Description)) });
        }

        await userManager.AddToRoleAsync(usuario, AdminSeeder.PapelAdmin);
        return CreatedAtAction(nameof(Listar), await ParaDtoAsync(usuario));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UsuarioAdminDto>> Atualizar(Guid id, UsuarioAtualizarRequest request)
    {
        var usuario = await userManager.FindByIdAsync(id.ToString());
        if (usuario is null)
        {
            return NotFound();
        }

        usuario.NomeCompleto = request.NomeCompleto;
        await userManager.UpdateAsync(usuario);
        return await ParaDtoAsync(usuario);
    }

    [HttpPost("{id:guid}/desativar")]
    public async Task<IActionResult> Desativar(Guid id)
    {
        if (id == ObterUsuarioIdAtual())
        {
            return BadRequest(new { message = "Você não pode desativar sua própria conta." });
        }

        var usuario = await userManager.FindByIdAsync(id.ToString());
        if (usuario is null)
        {
            return NotFound();
        }

        var administradores = await userManager.GetUsersInRoleAsync(AdminSeeder.PapelAdmin);
        var ativosRestantes = new List<ApplicationUser>();
        foreach (var admin in administradores)
        {
            if (admin.Id != id && !await userManager.IsLockedOutAsync(admin))
            {
                ativosRestantes.Add(admin);
            }
        }

        if (ativosRestantes.Count == 0)
        {
            return BadRequest(new { message = "Não é possível desativar o último administrador ativo." });
        }

        await userManager.SetLockoutEnabledAsync(usuario, true);
        await userManager.SetLockoutEndDateAsync(usuario, DateTimeOffset.MaxValue);
        return NoContent();
    }

    [HttpPost("{id:guid}/ativar")]
    public async Task<IActionResult> Ativar(Guid id)
    {
        var usuario = await userManager.FindByIdAsync(id.ToString());
        if (usuario is null)
        {
            return NotFound();
        }

        await userManager.SetLockoutEndDateAsync(usuario, null);
        return NoContent();
    }

    [HttpPost("{id:guid}/redefinir-senha")]
    public async Task<IActionResult> RedefinirSenha(Guid id, RedefinirSenhaRequest request)
    {
        var usuario = await userManager.FindByIdAsync(id.ToString());
        if (usuario is null)
        {
            return NotFound();
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(usuario);
        var resultado = await userManager.ResetPasswordAsync(usuario, token, request.NovaSenha);
        if (!resultado.Succeeded)
        {
            return BadRequest(new { message = string.Join(" ", resultado.Errors.Select(e => e.Description)) });
        }

        return NoContent();
    }

    private async Task<UsuarioAdminDto> ParaDtoAsync(ApplicationUser usuario) => new(
        usuario.Id,
        usuario.NomeCompleto,
        usuario.Email ?? string.Empty,
        !await userManager.IsLockedOutAsync(usuario));

    private Guid ObterUsuarioIdAtual() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
