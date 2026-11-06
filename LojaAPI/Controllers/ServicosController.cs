using LojaAPI.Data;
using LojaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicosController : ControllerBase
{
    private readonly LojaContext _context;

    public ServicosController(LojaContext context)
    {
        _context = context;
    }

    // Serviço = item do tipo "S" (produto é "P").
    private IQueryable<Item> Servicos() =>
        _context.Items.Where(i => i.TipoItem == "S" && i.AtivoProdutoServico == "S");

    // GET: api/Servicos?categoria=Facial
    [HttpGet]
    public async Task<IActionResult> Listar(string? categoria)
    {
        var consulta = Servicos();

        if (!string.IsNullOrWhiteSpace(categoria))
            consulta = consulta.Where(i => i.IdcategoriaNavigation.Categoria == categoria);

        var servicos = await consulta
            .Select(i => new
            {
                i.Iditem,
                Nome = i.Item1,
                Descricao = i.DescricaoItem,
                i.Preco,
                i.Idcategoria,
                Categoria = i.IdcategoriaNavigation.Categoria
            })
            .ToListAsync();

        return Ok(servicos);
    }

    // GET: api/Servicos/1
    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var servico = await Servicos()
            .Where(i => i.Iditem == id)
            .Select(i => new
            {
                i.Iditem,
                Nome = i.Item1,
                Descricao = i.DescricaoItem,
                i.Preco,
                i.Idcategoria,
                Categoria = i.IdcategoriaNavigation.Categoria
            })
            .FirstOrDefaultAsync();

        if (servico == null)
            return NotFound(new { mensagem = "Serviço não encontrado." });

        return Ok(servico);
    }

    // POST: api/Servicos
    [HttpPost]
    public async Task<IActionResult> Cadastrar(ServicoSalvarDTO dto)
    {
        var erro = await Validar(dto);
        if (erro != null) return BadRequest(new { mensagem = erro });

        var servico = new Item
        {
            Idcategoria = dto.Idcategoria,
            Item1 = dto.Nome.Trim(),
            DescricaoItem = dto.Descricao,
            Preco = dto.Preco,
            TipoItem = "S",
            AtivoProdutoServico = "S"
        };

        _context.Items.Add(servico);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId), new { id = servico.Iditem }, new { servico.Iditem });
    }

    // PUT: api/Servicos/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, ServicoSalvarDTO dto)
    {
        var servico = await Servicos().FirstOrDefaultAsync(i => i.Iditem == id);

        if (servico == null)
            return NotFound(new { mensagem = "Serviço não encontrado." });

        var erro = await Validar(dto);
        if (erro != null) return BadRequest(new { mensagem = erro });

        servico.Idcategoria = dto.Idcategoria;
        servico.Item1 = dto.Nome.Trim();
        servico.DescricaoItem = dto.Descricao;
        servico.Preco = dto.Preco;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Servicos/1
    // Não apaga: apenas desativa, para manter o histórico dos pedidos.
    [HttpDelete("{id}")]
    public async Task<IActionResult> Desativar(int id)
    {
        var servico = await Servicos().FirstOrDefaultAsync(i => i.Iditem == id);

        if (servico == null)
            return NotFound(new { mensagem = "Serviço não encontrado." });

        servico.AtivoProdutoServico = "N";
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Devolve a mensagem de erro, ou null se estiver tudo certo.
    private async Task<string?> Validar(ServicoSalvarDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome)) return "Informe o nome do serviço.";
        if (dto.Preco < 0) return "O preço não pode ser negativo.";
        if (!await _context.Categoria.AnyAsync(c => c.Idcategoria == dto.Idcategoria))
            return "A categoria informada não existe.";
        return null;
    }
}

public class ServicoSalvarDTO
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Idcategoria { get; set; }
}
