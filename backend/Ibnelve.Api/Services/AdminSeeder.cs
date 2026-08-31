using Ibnelve.Api.Data;
using Microsoft.AspNetCore.Identity;

namespace Ibnelve.Api.Services;

/// <summary>
/// Garante o papel ADMIN e, se ainda não existir nenhum usuário nesse papel, cria o primeiro
/// a partir de Admin:Email/Admin:Password (config/user-secrets) — nunca há cadastro público
/// de administrador, conforme o CLAUDE.md.
/// </summary>
public static class AdminSeeder
{
    public const string PapelAdmin = "ADMIN";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("AdminSeeder");

        if (!await roleManager.RoleExistsAsync(PapelAdmin))
        {
            await roleManager.CreateAsync(new ApplicationRole(PapelAdmin));
        }

        var usuariosAdmin = await userManager.GetUsersInRoleAsync(PapelAdmin);
        if (usuariosAdmin.Count > 0)
        {
            return;
        }

        var email = configuration["Admin:Email"];
        var senha = configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            logger.LogWarning(
                "Nenhum usuário ADMIN existe e Admin:Email/Admin:Password não estão configurados — " +
                "defina via 'dotnet user-secrets set' para criar o primeiro administrador.");
            return;
        }

        var usuario = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            NomeCompleto = "Administrador"
        };

        var resultado = await userManager.CreateAsync(usuario, senha);
        if (resultado.Succeeded)
        {
            await userManager.AddToRoleAsync(usuario, PapelAdmin);
            logger.LogInformation("Usuário ADMIN inicial criado: {Email}", email);
        }
        else
        {
            logger.LogError(
                "Falha ao criar o usuário ADMIN inicial: {Erros}",
                string.Join("; ", resultado.Errors.Select(e => e.Description)));
        }
    }
}
