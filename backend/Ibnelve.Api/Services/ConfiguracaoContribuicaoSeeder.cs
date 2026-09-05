using Ibnelve.Api.Data;
using Ibnelve.Api.Data.Entities;
using Ibnelve.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Services;

/// <summary>Semeia a configuração de contribuições (singleton) com os dados confirmados do CLAUDE.md, só se a tabela estiver vazia.</summary>
public static class ConfiguracaoContribuicaoSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<IbnelveDbContext>();

        if (await db.ConfiguracoesContribuicao.AnyAsync())
        {
            return;
        }

        db.ConfiguracoesContribuicao.Add(new ConfiguracaoContribuicao
        {
            Id = Guid.NewGuid(),
            ChavePix = "11.080.185/0001-08",
            TipoChave = TipoChavePix.Cnpj,
            Favorecido = "Igreja Batista Nacional da Esperança do Liberdade e Vereda"
        });

        await db.SaveChangesAsync();
    }
}
