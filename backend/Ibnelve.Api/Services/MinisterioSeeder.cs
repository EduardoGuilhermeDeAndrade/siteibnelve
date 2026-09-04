using Ibnelve.Api.Data;
using Ibnelve.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Services;

/// <summary>Semeia os 5 ministérios reais do CLAUDE.md, só se a tabela estiver vazia.</summary>
public static class MinisterioSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<IbnelveDbContext>();

        if (await db.Ministerios.AnyAsync())
        {
            return;
        }

        db.Ministerios.AddRange(
            new Ministerio { Id = Guid.NewGuid(), Nome = "Ministério Infantil", Lideres = "Silvani", ImagemUrl = "/img/ministerio-infantil.jpg", Ativo = true, Ordem = 1 },
            new Ministerio { Id = Guid.NewGuid(), Nome = "Mocidade", Lideres = "Wendell e Hudson", Ativo = true, Ordem = 2 },
            new Ministerio { Id = Guid.NewGuid(), Nome = "Mulheres", Lideres = "Sildeni e Claudiene", Ativo = true, Ordem = 3 },
            new Ministerio { Id = Guid.NewGuid(), Nome = "Louvor", Lideres = "Marcus", Ativo = true, Ordem = 4 },
            new Ministerio { Id = Guid.NewGuid(), Nome = "Diáconos", Lideres = "Ronildo", Ativo = true, Ordem = 5 }
        );

        await db.SaveChangesAsync();
    }
}
