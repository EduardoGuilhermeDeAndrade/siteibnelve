using System.Security.Claims;
using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Domain;
using Ibnelve.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Controllers.Admin;

/// <summary>Configuração de contribuições é singleton (1 linha só, semeada no startup) — por isso
/// só há GET/PUT, sem criar/listar/excluir.</summary>
[ApiController]
[Route("api/admin/configuracao-contribuicao")]
[Authorize(Roles = AdminSeeder.PapelAdmin)]
public class ConfiguracaoContribuicaoController(IbnelveDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ConfiguracaoContribuicaoDto>> Obter()
    {
        var configuracao = await db.ConfiguracoesContribuicao.FirstOrDefaultAsync();
        return configuracao is null ? NotFound() : ParaDto(configuracao);
    }

    [HttpPut]
    public async Task<ActionResult<ConfiguracaoContribuicaoDto>> Atualizar(ConfiguracaoContribuicaoUpsertRequest request)
    {
        var configuracao = await db.ConfiguracoesContribuicao.FirstOrDefaultAsync();
        if (configuracao is null)
        {
            return NotFound();
        }

        configuracao.ChavePix = request.ChavePix;
        configuracao.TipoChave = EnumMappings.ParseTipoChavePix(request.TipoChave);
        configuracao.Favorecido = request.Favorecido;
        configuracao.AtualizadoPorUsuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        configuracao.DataAtualizacao = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();
        return ParaDto(configuracao);
    }

    private static ConfiguracaoContribuicaoDto ParaDto(Data.Entities.ConfiguracaoContribuicao configuracao) => new(
        configuracao.Id,
        configuracao.ChavePix,
        configuracao.TipoChave.ToApiString(),
        configuracao.Favorecido,
        configuracao.DataAtualizacao);
}
