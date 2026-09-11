using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Data.Entities;
using Ibnelve.Api.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    IbnelveDbContext db,
    ITokenService tokenService,
    IWebHostEnvironment env) : ControllerBase
{
    private const string CookieRefresh = "ibnelve_refresh";

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var usuario = await userManager.FindByEmailAsync(request.Email);
        if (usuario is null || await userManager.IsLockedOutAsync(usuario))
        {
            return Unauthorized(new { message = "Credenciais inválidas." });
        }

        var senhaOk = await userManager.CheckPasswordAsync(usuario, request.Password);
        if (!senhaOk)
        {
            await userManager.AccessFailedAsync(usuario);
            return Unauthorized(new { message = "Credenciais inválidas." });
        }

        await userManager.ResetAccessFailedCountAsync(usuario);

        var roles = await userManager.GetRolesAsync(usuario);
        var (resposta, _) = await EmitirSessaoAsync(usuario, roles);
        return resposta;
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh()
    {
        if (!Request.Cookies.TryGetValue(CookieRefresh, out var tokenBruto) || string.IsNullOrEmpty(tokenBruto))
        {
            return Unauthorized(new { message = "Sessão expirada, faça login novamente." });
        }

        var hash = tokenService.HashRefreshToken(tokenBruto);
        var refreshToken = await db.RefreshTokens.FirstOrDefaultAsync(r => r.HashDoToken == hash);

        if (refreshToken is null || !refreshToken.Ativo)
        {
            Response.Cookies.Delete(CookieRefresh);
            return Unauthorized(new { message = "Sessão expirada, faça login novamente." });
        }

        var usuario = await userManager.FindByIdAsync(refreshToken.UsuarioId.ToString());
        if (usuario is null || await userManager.IsLockedOutAsync(usuario))
        {
            refreshToken.RevogadoEm = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync();
            Response.Cookies.Delete(CookieRefresh);
            return Unauthorized(new { message = "Sessão expirada, faça login novamente." });
        }

        var roles = await userManager.GetRolesAsync(usuario);
        var (resposta, novoRefreshTokenId) = await EmitirSessaoAsync(usuario, roles);

        refreshToken.RevogadoEm = DateTimeOffset.UtcNow;
        refreshToken.SubstituidoPorId = novoRefreshTokenId;
        await db.SaveChangesAsync();

        return resposta;
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue(CookieRefresh, out var tokenBruto) && !string.IsNullOrEmpty(tokenBruto))
        {
            var hash = tokenService.HashRefreshToken(tokenBruto);
            var refreshToken = await db.RefreshTokens.FirstOrDefaultAsync(r => r.HashDoToken == hash);
            if (refreshToken is not null && refreshToken.RevogadoEm is null)
            {
                refreshToken.RevogadoEm = DateTimeOffset.UtcNow;
                await db.SaveChangesAsync();
            }
        }

        Response.Cookies.Delete(CookieRefresh);
        return NoContent();
    }

    private async Task<(LoginResponse Resposta, Guid RefreshTokenId)> EmitirSessaoAsync(
        ApplicationUser usuario, IList<string> roles)
    {
        var (accessToken, expiraEm) = tokenService.GerarAccessToken(usuario, roles);

        var refreshBruto = tokenService.GerarRefreshTokenBruto();
        var novoRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            HashDoToken = tokenService.HashRefreshToken(refreshBruto),
            CriadoEm = DateTimeOffset.UtcNow,
            ExpiraEm = DateTimeOffset.UtcNow.Add(tokenService.DuracaoRefreshToken)
        };
        db.RefreshTokens.Add(novoRefreshToken);
        await db.SaveChangesAsync();

        // Front-end e API rodam em domínios diferentes fora de dev (ex.: Cloudflare Pages/Render
        // vs. Render da API) — SameSite=Strict nunca seria enviado nesse cenário. None+Secure é
        // obrigatório fora de dev; em dev local (http, mesmo "site" via localhost) Lax basta.
        var isProduction = !env.IsDevelopment();
        Response.Cookies.Append(CookieRefresh, refreshBruto, new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = isProduction ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = novoRefreshToken.ExpiraEm,
            Path = "/api/auth"
        });

        var resposta = new LoginResponse(
            accessToken,
            expiraEm,
            new UsuarioLogadoDto(usuario.Id, usuario.Email ?? string.Empty, usuario.NomeCompleto, roles.ToList()));

        return (resposta, novoRefreshToken.Id);
    }
}
