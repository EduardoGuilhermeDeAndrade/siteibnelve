using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Data.Entities;
using Ibnelve.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Controllers;

/// <summary>Leitura pública (anônima) da Agenda — soma as ocorrências de todas as séries
/// PUBLICO + PUBLICADO + Ativo numa janela de consulta, nunca gerando eventos fisicamente.</summary>
[ApiController]
[Route("api/eventos")]
public class EventosController(IbnelveDbContext db) : ControllerBase
{
    private const int JanelaPadraoDias = 180;
    private const int JanelaTetoDias = 366 * 3;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventoPublicoDto>>> Listar([FromQuery] int? limite)
    {
        var series = await db.SeriesEvento
            .Include(s => s.Local)
            .Include(s => s.Excecoes).ThenInclude(e => e.LocalSubstituto)
            .Where(s => s.Visibilidade == VisibilidadeEvento.Publico
                        && s.Status == StatusEvento.Publicado
                        && s.Ativo)
            .ToListAsync();

        var agora = DateTimeOffset.UtcNow;
        var hoje = OcorrenciaCalculator.HojeLocal(agora);

        var janelaDias = JanelaPadraoDias;
        List<(SerieEvento Serie, Ocorrencia Ocorrencia)> encontrados;

        while (true)
        {
            var fim = hoje.AddDays(janelaDias);
            encontrados = series
                .SelectMany(s => OcorrenciaCalculator.CalcularOcorrencias(s, hoje, fim)
                    .Where(o => !o.Cancelada && o.DataHoraInicio >= agora)
                    .Select(o => (Serie: s, Ocorrencia: o)))
                .OrderBy(x => x.Ocorrencia.DataHoraInicio)
                .ToList();

            var atingiuLimite = limite is > 0 && encontrados.Count >= limite.Value;
            if (atingiuLimite || limite is null || janelaDias >= JanelaTetoDias)
            {
                break;
            }

            janelaDias *= 2;
        }

        IEnumerable<(SerieEvento Serie, Ocorrencia Ocorrencia)> resultado = encontrados;
        if (limite is > 0)
        {
            resultado = resultado.Take(limite.Value);
        }

        return resultado.Select(x => ParaDto(x.Serie, x.Ocorrencia)).ToList();
    }

    private static EventoPublicoDto ParaDto(SerieEvento serie, Ocorrencia ocorrencia) => new(
        serie.Id,
        ocorrencia.Titulo,
        ocorrencia.Descricao,
        serie.Categoria.ToApiString(),
        ocorrencia.DataHoraInicio,
        ocorrencia.LocalNome ?? ocorrencia.LocalTexto,
        serie.TipoRecorrencia != TipoRecorrencia.Nenhuma);
}
