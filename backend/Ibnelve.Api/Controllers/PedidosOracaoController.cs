using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ibnelve.Api.Controllers;

/// <summary>Recebe pedidos de oração do formulário público de Contato — escrita anônima, sem
/// leitura pública (só o Portal Admin consulta). Rate limiting porque é um endpoint público de
/// escrita, mesma cautela já aplicada a login/refresh.</summary>
[ApiController]
[Route("api/pedidos-oracao")]
public class PedidosOracaoController(IbnelveDbContext db) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("pedido-oracao")]
    public async Task<IActionResult> Criar(PedidoOracaoCriarRequest request)
    {
        if (request.DesejaFalarComPastor && string.IsNullOrWhiteSpace(request.Contato))
        {
            return BadRequest(new { message = "Informe um contato para que um pastor possa retornar." });
        }

        var pedido = new PedidoOracao
        {
            Id = Guid.NewGuid(),
            // O backend decide o Nome a partir de Anonimo — não confia só no que o front manda.
            Nome = request.Anonimo ? null : request.Nome,
            Anonimo = request.Anonimo,
            Contato = request.Contato,
            DesejaFalarComPastor = request.DesejaFalarComPastor,
            Mensagem = request.Mensagem,
            DataCriacao = DateTimeOffset.UtcNow
        };

        db.PedidosOracao.Add(pedido);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
