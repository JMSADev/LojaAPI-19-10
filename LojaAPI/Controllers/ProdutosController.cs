
using LojaAPI.Data;
using LojaAPI.Models;
using LojaAPI.Models.DTOs;
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
    // Lista os produtos ativos utilizando a ProdutoDTO.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutos(string? categoria)
    {
        var produtos = await _context.Items
            .Where(i =>
                i.TipoItem == "P" &&
                i.AtivoProdutoServico == "S" &&
                (categoria == null || i.IdcategoriaNavigation.Categoria == categoria))
            .Select(i => new ProdutoDTO
            {
                Iditem = i.Iditem,
                Nome = i.Item1,
                Descricao = i.DescricaoItem,
                Preco = i.Preco,
                Categoria = i.IdcategoriaNavigation != null
                    ? i.IdcategoriaNavigation.Categoria
                    : null,
                PrecoVenda = i.Produto != null
                    ? i.Produto.PrecoVenda
                    : null,
                Unidade = i.Produto != null
                    ? i.Produto.Unidade
                    : null,
                QuantidadeEstoque = i.Produto != null &&
                                    i.Produto.Estoque != null
                    ? i.Produto.Estoque.QuantidadeEstoque
                    : null
            })
            .ToListAsync();

        return Ok(produtos);
    }

    // GET: api/Produtos/1
    // Busca um produto específico pelo identificador.
    [HttpGet("{id}")]
    public async Task<ActionResult<ProdutoDTO>> GetProduto(int id)
    {
        var produto = await _context.Items
            .Where(i =>
                i.Iditem == id &&
                i.TipoItem == "P" &&
                i.AtivoProdutoServico == "S")
            .Select(i => new ProdutoDTO
            {
                Iditem = i.Iditem,
                Nome = i.Item1,
                Descricao = i.DescricaoItem,
                Preco = i.Preco,
                Categoria = i.IdcategoriaNavigation != null
                    ? i.IdcategoriaNavigation.Categoria
                    : null,
                PrecoVenda = i.Produto != null
                    ? i.Produto.PrecoVenda
                    : null,
                Unidade = i.Produto != null
                    ? i.Produto.Unidade
                    : null,
                QuantidadeEstoque = i.Produto != null &&
                                    i.Produto.Estoque != null
                    ? i.Produto.Estoque.QuantidadeEstoque
                    : null
            })
            .FirstOrDefaultAsync();

        if (produto == null)
        {
            return NotFound(new
            {
                mensagem = "Produto não encontrado."
            });
        }

        return Ok(produto);
    }

    // POST: api/Produtos
    // Cadastra o item, o produto e já cria o estoque zerado.
    [HttpPost]
    public async Task<IActionResult> PostProduto(ProdutoSalvarDTO dto)
    {
        var erro = await Validar(dto);
        if (erro != null) return BadRequest(new { mensagem = erro });

        var item = new Item
        {
            Idcategoria = dto.Idcategoria,
            Item1 = dto.Nome.Trim(),
            DescricaoItem = dto.Descricao,
            Preco = dto.Preco,
            TipoItem = "P",
            AtivoProdutoServico = "S",
            Produto = new Produto
            {
                CodigoPreco = dto.CodigoPreco,
                PrecoVenda = dto.PrecoVenda,
                Unidade = dto.Unidade
            }
        };

        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        // Se o banco já criou o estoque (trigger), não cria de novo.
        if (!await _context.Estoques.AnyAsync(e => e.Iditem == item.Iditem))
        {
            _context.Estoques.Add(new Estoque { Iditem = item.Iditem, QuantidadeEstoque = 0 });
            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(GetProduto), new { id = item.Iditem }, new { item.Iditem });
    }

    // PUT: api/Produtos/1
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduto(int id, ProdutoSalvarDTO dto)
    {
        var item = await _context.Items
            .Include(i => i.Produto)
            .FirstOrDefaultAsync(i => i.Iditem == id && i.TipoItem == "P" && i.AtivoProdutoServico == "S");

        if (item == null || item.Produto == null)
            return NotFound(new { mensagem = "Produto não encontrado." });

        var erro = await Validar(dto);
        if (erro != null) return BadRequest(new { mensagem = erro });

        item.Idcategoria = dto.Idcategoria;
        item.Item1 = dto.Nome.Trim();
        item.DescricaoItem = dto.Descricao;
        item.Preco = dto.Preco;
        item.Produto.CodigoPreco = dto.CodigoPreco;
        item.Produto.PrecoVenda = dto.PrecoVenda;
        item.Produto.Unidade = dto.Unidade;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Produtos/1
    // Não apaga: apenas desativa, para manter o histórico de pedidos e entradas.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DesativarProduto(int id)
    {
        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.Iditem == id && i.TipoItem == "P" && i.AtivoProdutoServico == "S");

        if (item == null)
            return NotFound(new { mensagem = "Produto não encontrado." });

        item.AtivoProdutoServico = "N";
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Devolve a mensagem de erro, ou null se estiver tudo certo.
    private async Task<string?> Validar(ProdutoSalvarDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome)) return "Informe o nome do produto.";
        if (string.IsNullOrWhiteSpace(dto.Unidade)) return "Informe a unidade do produto.";
        if (dto.Preco < 0 || dto.PrecoVenda < 0) return "Os preços não podem ser negativos.";
        if (!await _context.Categoria.AnyAsync(c => c.Idcategoria == dto.Idcategoria))
            return "A categoria informada não existe.";
        return null;
    }
}

public class ProdutoSalvarDTO
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Idcategoria { get; set; }
    public int CodigoPreco { get; set; } = 1;
    public decimal PrecoVenda { get; set; }
    public string Unidade { get; set; } = "UN";
}
