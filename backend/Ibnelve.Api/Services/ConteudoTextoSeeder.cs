using Ibnelve.Api.Data;
using Ibnelve.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Services;

/// <summary>
/// Semeia os textos institucionais provisórios já aprovados no CLAUDE.md, só se a tabela
/// estiver vazia — depois disso o conteúdo passa a ser gerido pelo Portal.
/// </summary>
public static class ConteudoTextoSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<IbnelveDbContext>();

        if (await db.ConteudosTexto.AnyAsync())
        {
            return;
        }

        var agora = DateTimeOffset.UtcNow;

        db.ConteudosTexto.AddRange(
            new ConteudoTexto
            {
                Id = Guid.NewGuid(),
                Chave = "quem-somos",
                Titulo = "Quem Somos",
                Conteudo = "Há 29 anos, a Igreja Batista Nacional da Esperança do Liberdade e Vereda é uma " +
                    "comunidade cristã em Ribeirão das Neves comprometida com a Palavra de Deus, a comunhão, " +
                    "o discipulado e a proclamação do Evangelho de Jesus Cristo. Com presença nos bairros " +
                    "Vereda e Liberdade, buscamos ser uma igreja acolhedora, onde pessoas e famílias possam " +
                    "conhecer a Cristo, crescer na fé e servir a Deus e ao próximo.",
                DataAtualizacao = agora
            },
            new ConteudoTexto
            {
                Id = Guid.NewGuid(),
                Chave = "historia",
                Titulo = "Nossa História",
                Conteudo = "[Texto de exemplo — espaço reservado para a história completa da igreja. " +
                    "A ser preenchido pela liderança com o relato real da fundação e da trajetória da IBNELVE.]",
                DataAtualizacao = agora
            },
            new ConteudoTexto
            {
                Id = Guid.NewGuid(),
                Chave = "missao",
                Titulo = "Missão",
                Conteudo = "Glorificar a Deus, proclamando o Evangelho de Jesus Cristo, fazendo discípulos, " +
                    "ensinando a Palavra e servindo pessoas e famílias por meio de uma igreja viva, acolhedora " +
                    "e comprometida com a Grande Comissão.",
                DataAtualizacao = agora
            },
            new ConteudoTexto
            {
                Id = Guid.NewGuid(),
                Chave = "visao",
                Titulo = "Visão",
                Conteudo = "Ser uma igreja bíblica, relevante e acolhedora, que cresce em comunhão, maturidade " +
                    "espiritual e compromisso missionário, alcançando vidas e famílias para Cristo em nossa " +
                    "comunidade e além dela.",
                DataAtualizacao = agora
            },
            new ConteudoTexto
            {
                Id = Guid.NewGuid(),
                Chave = "valores",
                Titulo = "Valores",
                Conteudo = "Palavra de Deus\nComunhão\nDiscipulado\nFamília\nServiço\nMissões",
                DataAtualizacao = agora
            },
            new ConteudoTexto
            {
                Id = Guid.NewGuid(),
                Chave = "no-que-cremos",
                Titulo = "No que cremos",
                Conteudo = "A Bíblia como Palavra de Deus e regra de fé e prática.\n" +
                    "Deus — Pai, Filho e Espírito Santo.\n" +
                    "Jesus Cristo como Senhor e Salvador.\n" +
                    "Salvação pela graça de Deus mediante a fé em Jesus Cristo.\n" +
                    "A Igreja como comunidade de discípulos, chamada à comunhão, ao serviço e à missão.\n" +
                    "A missão de anunciar o Evangelho e fazer discípulos.",
                DataAtualizacao = agora
            }
        );

        await db.SaveChangesAsync();
    }
}
