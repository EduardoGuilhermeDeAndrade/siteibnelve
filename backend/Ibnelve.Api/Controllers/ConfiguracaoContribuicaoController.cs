using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Controllers;

/// <summary>Leitura pública (anônima) da configuração de contribuições — sem dado sensível,
/// reaproveita o mesmo DTO do endpoint de admin.</summary>
[ApiController]
[Route("api/configuracao-contribuicao")]
public class ConfiguracaoContribuicaoController(IbnelveDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ConfiguracaoContribuicaoDto>> Obter()
    {
        var configuracao = await db.ConfiguracoesContribuicao.FirstOrDefaultAsync();
        if (configuracao is null)
        {
            return NotFound();
        }

        return new ConfiguracaoContribuicaoDto(
            configuracao.Id,
            configuracao.ChavePix,
            configuracao.TipoChave.ToApiString(),
            configuracao.Favorecido,
            configuracao.DataAtualizacao);
    }
}
