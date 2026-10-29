using LojaAPI.Data;
using LojaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly LojaContext _context;

    public ProdutosController(LojaContext context)
    {
        _context = context;
    }

    // GET: api/Produtos
    // Lista os produtos cadastrados e suas informações.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetProdutos()
    {
        var produtos = await _context.Items
            .Include(i => i.Produto)
            .Where(i => i.TipoItem == "P")
            .ToListAsync();

        return Ok(produtos);
    }

    // GET: api/Produtos/1
    // Busca um produto específico pelo seu identificador.
    [HttpGet("{id}")]
    public async Task<ActionResult<Item>> GetProduto(int id)
    {
        var produto = await _context.Items
            .Include(i => i.Produto)
            .FirstOrDefaultAsync(i => i.Iditem == id && i.TipoItem == "P");

        if (produto == null)
        {
            return NotFound(new
            {
                mensagem = "Produto não encontrado."
            });
        }

        return Ok(produto);
    }
}