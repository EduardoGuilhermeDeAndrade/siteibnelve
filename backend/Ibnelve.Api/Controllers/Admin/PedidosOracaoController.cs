using Ibnelve.Api.Contracts;
using Ibnelve.Api.Data;
using Ibnelve.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Controllers.Admin;

/// <summary>Consulta dos pedidos de oração pelo Portal — quem cria é sempre o formulário público
/// (ver PedidosOracaoController), então aqui só há leitura, marcação de lido e exclusão.</summary>
[ApiController]
[Route("api/admin/pedidos-oracao")]
[Authorize(Roles = AdminSeeder.PapelAdmin)]
public class PedidosOracaoController(IbnelveDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PedidoOracaoAdminDto>>> Listar()
    {
        var pedidos = await db.PedidosOracao
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync();

        return pedidos.Select(p => new PedidoOracaoAdminDto(
            p.Id, p.Nome, p.Anonimo, p.Contato, p.DesejaFalarComPastor, p.Mensagem, p.Lido, p.DataCriacao)).ToList();
    }

    [HttpPost("{id:guid}/marcar-lido")]
    public async Task<IActionResult> MarcarLido(Guid id) => await AtualizarLido(id, true);

    [HttpPost("{id:guid}/marcar-nao-lido")]
    public async Task<IActionResult> MarcarNaoLido(Guid id) => await AtualizarLido(id, false);

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var pedido = await db.PedidosOracao.FirstOrDefaultAsync(p => p.Id == id);
        if (pedido is null)
        {
            return NotFound();
        }

        db.PedidosOracao.Remove(pedido);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<IActionResult> AtualizarLido(Guid id, bool lido)
    {
        var pedido = await db.PedidosOracao.FirstOrDefaultAsync(p => p.Id == id);
        if (pedido is null)
        {
            return NotFound();
        }

        pedido.Lido = lido;
        await db.SaveChangesAsync();
        return NoContent();
    }
}
