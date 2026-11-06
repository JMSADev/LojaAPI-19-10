using LojaAPI.Data;
using LojaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaAPI.Controllers;

// Pedido = Atendimento. Status no banco (1 letra):
// A = Aberto, P = Em processamento, C = Concluído, X = Cancelado
[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly LojaContext _context;

    public PedidosController(LojaContext context)
    {
        _context = context;
    }

    // Nomes aceitos na URL/JSON e a letra que cada um vira.
    private static readonly Dictionary<string, string> Codigos =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Aberto"] = "A",
            ["Em processamento"] = "P",
            ["Concluido"] = "C",
            ["Concluído"] = "C",
            ["Cancelado"] = "X"
        };

    // De qual status se pode ir para qual.
    private static readonly Dictionary<string, string[]> Transicoes = new()
    {
        ["A"] = new[] { "P", "X" },
        ["P"] = new[] { "C", "X" }
    };

    private static string Codigo(string status) =>
        Codigos.TryGetValue(status.Trim(), out var codigo) ? codigo : status.Trim().ToUpper();

    // Lista de pedidos com cliente, total e itens.
    private static async Task<List<object>> Montar(IQueryable<Atendimento> consulta)
    {
        var pedidos = await consulta
            .OrderByDescending(a => a.Idatendimento)
            .Select(a => new
            {
                a.Idatendimento,
                a.Idpessoa,
                Cliente = a.IdpessoaNavigation.Nome,
                a.DataAtendimento,
                a.StatusAtendimento,
                Total = a.Atendimentoitems.Sum(i => i.QuantidadeItens * i.ValorunitarioEntrada),
                Itens = a.Atendimentoitems.Select(i => new
                {
                    i.Iditem,
                    Item = i.IditemNavigation.Item1,
                    Quantidade = i.QuantidadeItens,
                    ValorUnitario = i.ValorunitarioEntrada,
                    Funcionario = i.IdfuncionarioNavigation.IdpessoaNavigation.Nome,
                    Cargo = i.IdfuncionarioNavigation.Cargo
                }).ToList()
            })
            .ToListAsync();

        return pedidos.Cast<object>().ToList();
    }

    // GET: api/Pedidos
    // GET: api/Pedidos?status=Aberto
    [HttpGet]
    public async Task<IActionResult> Listar(string? status)
    {
        var consulta = _context.Atendimentos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var codigo = Codigo(status);
            consulta = consulta.Where(a => a.StatusAtendimento == codigo);
        }

        return Ok(await Montar(consulta));
    }

    // GET: api/Pedidos/cliente/9
    // Só devolve os pedidos desse cliente.
    [HttpGet("cliente/{idPessoa}")]
    public async Task<IActionResult> ListarDoCliente(int idPessoa)
    {
        var consulta = _context.Atendimentos.Where(a => a.Idpessoa == idPessoa);

        return Ok(await Montar(consulta));
    }

    // POST: api/Pedidos
    // Cria o pedido e os itens de uma vez. Se algo falhar (ex.: sem estoque),
    // nada é salvo. O preço vem do banco, não do frontend.
    [HttpPost]
    public async Task<IActionResult> Criar(PedidoCreateDTO dto)
    {
        if (dto.Itens.Count == 0)
            return BadRequest(new { mensagem = "O pedido precisa ter pelo menos um item." });

        if (dto.Itens.Any(i => i.Quantidade <= 0))
            return BadRequest(new { mensagem = "A quantidade deve ser maior que zero." });

        if (!await _context.Pessoas.AnyAsync(p => p.Idpessoa == dto.Idpessoa && p.AtivoPessoa == "S"))
            return BadRequest(new { mensagem = "O cliente informado não existe." });

        if (!await _context.Funcionarios.AnyAsync(f => f.Idpessoa == dto.Idfuncionario))
            return BadRequest(new { mensagem = "O funcionário informado não existe." });

        // Se o mesmo item vier duas vezes, soma as quantidades.
        var pedidos = dto.Itens
            .GroupBy(i => i.Iditem)
            .Select(g => new { Iditem = g.Key, Quantidade = g.Sum(i => i.Quantidade) })
            .ToList();

        var ids = pedidos.Select(p => p.Iditem).ToList();

        var itens = await _context.Items
            .Include(i => i.Produto).ThenInclude(p => p!.Estoque)
            .Where(i => ids.Contains(i.Iditem) && i.AtivoProdutoServico == "S")
            .ToListAsync();

        var atendimento = new Atendimento
        {
            Idpessoa = dto.Idpessoa,
            DataAtendimento = DateOnly.FromDateTime(DateTime.Now),
            StatusAtendimento = "A"
        };

        foreach (var pedido in pedidos)
        {
            var item = itens.FirstOrDefault(i => i.Iditem == pedido.Iditem);

            if (item == null)
                return BadRequest(new { mensagem = $"O item {pedido.Iditem} não existe." });

            decimal valor;

            if (item.TipoItem == "P")
            {
                // Produto: respeita o estoque e usa o preço de venda.
                var estoque = item.Produto?.Estoque?.QuantidadeEstoque ?? 0;

                if (pedido.Quantidade > estoque)
                    return BadRequest(new { mensagem = $"Estoque insuficiente para {item.Item1}. Disponível: {estoque}." });

                valor = item.Produto!.PrecoVenda;
            }
            else
            {
                // Serviço: não tem estoque.
                valor = item.Preco;
            }

            atendimento.Atendimentoitems.Add(new Atendimentoitem
            {
                Iditem = item.Iditem,
                Idfuncionario = dto.Idfuncionario,
                QuantidadeItens = pedido.Quantidade,
                ValorunitarioEntrada = valor
            });
        }

        try
        {
            // Um único SaveChanges: pedido e itens são salvos juntos (ou nenhum).
            _context.Atendimentos.Add(atendimento);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(new { mensagem = ex.InnerException?.Message ?? ex.Message });
        }

        return Created($"/api/Atendimentos/{atendimento.Idatendimento}", new
        {
            mensagem = "Pedido registrado com sucesso!",
            idatendimento = atendimento.Idatendimento
        });
    }

    // PUT: api/Pedidos/1/status      corpo: { "status": "Em processamento" }
    [HttpPut("{id}/status")]
    public async Task<IActionResult> AtualizarStatus(int id, StatusPedidoDTO dto)
    {
        var pedido = await _context.Atendimentos.FindAsync(id);

        if (pedido == null)
            return NotFound(new { mensagem = "Pedido não encontrado." });

        var novo = Codigo(dto.Status);

        if (!Transicoes.TryGetValue(pedido.StatusAtendimento, out var permitidos) ||
            !permitidos.Contains(novo))
        {
            return BadRequest(new { mensagem = "Essa mudança de status não é permitida." });
        }

        pedido.StatusAtendimento = novo;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class PedidoCreateDTO
{
    public int Idpessoa { get; set; }
    public int Idfuncionario { get; set; } = 1;
    public List<PedidoItemDTO> Itens { get; set; } = new();
}

public class PedidoItemDTO
{
    public int Iditem { get; set; }
    public int Quantidade { get; set; }
}

public class StatusPedidoDTO
{
    public string Status { get; set; } = string.Empty;
}
