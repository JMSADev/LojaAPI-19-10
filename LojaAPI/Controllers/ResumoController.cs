using LojaAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResumoController : ControllerBase
{
    private readonly LojaContext _context;

    public ResumoController(LojaContext context)
    {
        _context = context;
    }

    // GET: api/Resumo
    // Totais mostrados na tela inicial do painel administrativo.
    [HttpGet]
    public async Task<IActionResult> Obter()
    {
        return Ok(new
        {
            clientes = await _context.Pessoas.CountAsync(p => p.AtivoPessoa == "S"),
            produtos = await _context.Items.CountAsync(i => i.TipoItem == "P" && i.AtivoProdutoServico == "S"),
            servicos = await _context.Items.CountAsync(i => i.TipoItem == "S" && i.AtivoProdutoServico == "S"),
            pedidos = await _context.Atendimentos.CountAsync(),
            pedidosEmAberto = await _context.Atendimentos.CountAsync(a => a.StatusAtendimento == "A"),
            unidadesEmEstoque = await _context.Estoques.SumAsync(e => e.QuantidadeEstoque)
        });
    }
}
