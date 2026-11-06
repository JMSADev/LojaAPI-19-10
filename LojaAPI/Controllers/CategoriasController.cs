using LojaAPI.Data;
using LojaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly LojaContext _context;

    public CategoriasController(LojaContext context)
    {
        _context = context;
    }

    // GET: api/Categorias
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var categorias = await _context.Categoria
            .Select(c => new { c.Idcategoria, c.Categoria })
            .ToListAsync();

        return Ok(categorias);
    }

    // GET: api/Categorias/1
    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var categoria = await _context.Categoria.FindAsync(id);

        if (categoria == null)
            return NotFound(new { mensagem = "Categoria não encontrada." });

        return Ok(new { categoria.Idcategoria, categoria.Categoria });
    }

    // POST: api/Categorias
    [HttpPost]
    public async Task<IActionResult> Cadastrar(CategoriaSalvarDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Categoria))
            return BadRequest(new { mensagem = "Informe o nome da categoria." });

        var categoria = new Categorium { Categoria = dto.Categoria.Trim() };

        _context.Categoria.Add(categoria);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId),
            new { id = categoria.Idcategoria },
            new { categoria.Idcategoria, categoria.Categoria });
    }

    // PUT: api/Categorias/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, CategoriaSalvarDTO dto)
    {
        var categoria = await _context.Categoria.FindAsync(id);

        if (categoria == null)
            return NotFound(new { mensagem = "Categoria não encontrada." });

        if (string.IsNullOrWhiteSpace(dto.Categoria))
            return BadRequest(new { mensagem = "Informe o nome da categoria." });

        categoria.Categoria = dto.Categoria.Trim();
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Categorias/1
    // Não deixa excluir se algum produto ou serviço usa a categoria.
    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var categoria = await _context.Categoria.FindAsync(id);

        if (categoria == null)
            return NotFound(new { mensagem = "Categoria não encontrada." });

        if (await _context.Items.AnyAsync(i => i.Idcategoria == id))
            return Conflict(new { mensagem = "Não é possível excluir: existem produtos ou serviços nesta categoria." });

        _context.Categoria.Remove(categoria);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CategoriaSalvarDTO
{
    public string Categoria { get; set; } = string.Empty;
}
