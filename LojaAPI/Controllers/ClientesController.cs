using LojaAPI.Data;
using LojaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly LojaContext _context;

    public ClientesController(LojaContext context)
    {
        _context = context;
    }

    // GET: api/Clientes
    // Lista os clientes ativos.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pessoa>>> GetClientes()
    {
        var clientes = await _context.Pessoas
            .Where(p => p.AtivoPessoa == "S")
            .ToListAsync();

        return Ok(clientes);
    }

    // GET: api/Clientes/9
    // Busca um cliente pelo ID e mostra seus endereços.
    [HttpGet("{id}")]
    public async Task<ActionResult<Pessoa>> GetCliente(int id)
    {
        var cliente = await _context.Pessoas
            .Include(p => p.Enderecos)
            .FirstOrDefaultAsync(p =>
                p.Idpessoa == id &&
                p.AtivoPessoa == "S");

        if (cliente == null)
            return NotFound(new { mensagem = "Cliente não encontrado." });

        return Ok(cliente);
    }

    // POST: api/Clientes
    // Cadastra um novo cliente.
    [HttpPost]
    public async Task<ActionResult<Pessoa>> PostCliente(Pessoa cliente)
    {
        var erro = await Validar(cliente, null);
        if (erro != null) return BadRequest(new { mensagem = erro });

        cliente.AtivoPessoa = "S";

        _context.Pessoas.Add(cliente);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCliente), new { id = cliente.Idpessoa }, cliente);
    }

    // PUT: api/Clientes/9
    // Atualiza os dados pessoais de um cliente.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCliente(int id, Pessoa dados)
    {
        var cliente = await _context.Pessoas
            .FirstOrDefaultAsync(p => p.Idpessoa == id && p.AtivoPessoa == "S");

        if (cliente == null)
            return NotFound(new { mensagem = "Cliente não encontrado." });

        var erro = await Validar(dados, id);
        if (erro != null) return BadRequest(new { mensagem = erro });

        cliente.Nome = dados.Nome;
        cliente.Cpf = dados.Cpf;
        cliente.DataNascimento = dados.DataNascimento;
        cliente.Sexo = dados.Sexo;
        cliente.Email = dados.Email;
        cliente.Telefone = dados.Telefone;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Clientes/9
    // Não apaga: apenas desativa, para manter o histórico de pedidos.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DesativarCliente(int id)
    {
        var cliente = await _context.Pessoas
            .FirstOrDefaultAsync(p => p.Idpessoa == id && p.AtivoPessoa == "S");

        if (cliente == null)
            return NotFound(new { mensagem = "Cliente não encontrado." });

        cliente.AtivoPessoa = "N";
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Clientes/9/Endereco
    // Cadastra um endereço para um cliente.
    [HttpPost("{id}/Endereco")]
    public async Task<ActionResult<Endereco>> PostEndereco(int id, EnderecoCreateDTO dados)
    {
        var cliente = await _context.Pessoas
            .FirstOrDefaultAsync(p => p.Idpessoa == id && p.AtivoPessoa == "S");

        if (cliente == null)
            return NotFound(new { mensagem = "Cliente não encontrado." });

        var endereco = new Endereco
        {
            Idpessoa = id,
            Cep = dados.Cep,
            Numero = dados.Numero,
            Complemento = dados.Complemento ?? string.Empty
        };

        _context.Enderecos.Add(endereco);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCliente), new { id = id }, endereco);
    }

    // Valida os campos e não deixa repetir CPF ou e-mail.
    // idAtual = id do cliente que está sendo editado (null no cadastro).
    // Devolve a mensagem de erro, ou null se estiver tudo certo.
    private async Task<string?> Validar(Pessoa cliente, int? idAtual)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nome)) return "Informe o nome.";
        if (cliente.Cpf == 0 || cliente.Cpf > 99999999999) return "CPF inválido.";
        if (string.IsNullOrWhiteSpace(cliente.Email) || !cliente.Email.Contains('@')) return "E-mail inválido.";

        if (await _context.Pessoas.AnyAsync(p => p.Cpf == cliente.Cpf && p.Idpessoa != idAtual))
            return "Já existe uma pessoa cadastrada com este CPF.";

        if (await _context.Pessoas.AnyAsync(p => p.Email == cliente.Email && p.Idpessoa != idAtual))
            return "Já existe uma pessoa cadastrada com este e-mail.";

        return null;
    }
}

// DTO usado somente para cadastrar endereço.
public class EnderecoCreateDTO
{
    public string Cep { get; set; } = string.Empty;

    public string Numero { get; set; } = string.Empty;

    public string? Complemento { get; set; }
}
