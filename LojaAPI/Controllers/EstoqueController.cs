using LojaAPI.Data;
using LojaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstoqueController : ControllerBase
{
    private readonly LojaContext _context;

    public EstoqueController(LojaContext context)
    {
        _context = context;
    }

    // Monta os dados que a tela de estoque mostra.
    // Estoque -> Produto -> Item (e categoria)
    private static IQueryable<EstoqueDTO> Montar(IQueryable<Estoque> consulta) =>
        consulta.Select(e => new EstoqueDTO
        {
            Iditem = e.Iditem,
            Item = e.IditemNavigation.IditemNavigation.Item1,
            DescricaoItem = e.IditemNavigation.IditemNavigation.DescricaoItem,
            Preco = e.IditemNavigation.IditemNavigation.Preco,
            PrecoVenda = e.IditemNavigation.PrecoVenda,
            Unidade = e.IditemNavigation.Unidade,
            Categoria = e.IditemNavigation.IditemNavigation.IdcategoriaNavigation.Categoria,
            QuantidadeEstoque = e.QuantidadeEstoque,
            Ativo = e.IditemNavigation.IditemNavigation.AtivoProdutoServico == "S"
        });

    // GET: api/Estoque
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        return Ok(await Montar(_context.Estoques).ToListAsync());
    }

    // GET: api/Estoque/1   (1 = id do item)
    [HttpGet("{iditem}")]
    public async Task<IActionResult> BuscarPorId(int iditem)
    {
        var estoque = await Montar(_context.Estoques.Where(e => e.Iditem == iditem))
            .FirstOrDefaultAsync();

        if (estoque == null)
            return NotFound(new { mensagem = "Estoque não encontrado para este item." });

        return Ok(estoque);
    }
}

public class EstoqueDTO
{
    public int Iditem { get; set; }
    public string Item { get; set; } = string.Empty;
    public string DescricaoItem { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public decimal PrecoVenda { get; set; }
    public string Unidade { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int QuantidadeEstoque { get; set; }
    public bool Ativo { get; set; }
}
