
using LojaAPI.DTOs;
using LojaAPI.Data;
using LojaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntradasProdutosController : ControllerBase
    {
        private readonly LojaContext _context;

        public EntradasProdutosController(LojaContext context)
        {
            _context = context;
        }

        // GET: api/EntradasProdutos
        // Lista todas as entradas
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var entradas = await _context.EntradaProdutos
                .ToListAsync();

            return Ok(entradas);
        }

        // GET: api/EntradasProdutos/1
        // Busca uma entrada pelo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var entrada = await _context.EntradaProdutos
                .FirstOrDefaultAsync(e => e.IdentradaProduto == id);

            if (entrada == null)
            {
                return NotFound(new
                {
                    mensagem = "Entrada de produto não encontrada."
                });
            }

            return Ok(entrada);
        }

        // POST: api/EntradasProdutos
        // Cadastra uma entrada de produto.
        // A trigger do banco atualiza o estoque e os preços.
        [HttpPost]
        public async Task<IActionResult> Cadastrar(
            [FromBody] EntradaProdutoCreateDTO dto)
        {
            var estoque = await _context.Estoques
                .FindAsync(dto.Iditem);

            if (estoque == null)
            {
                return BadRequest(new
                {
                    mensagem = "Não existe estoque cadastrado para esse item."
                });
            }

            if (dto.QuantidadeEntrada <= 0)
            {
                return BadRequest(new
                {
                    mensagem = "A quantidade deve ser maior que zero."
                });
            }

            if (dto.ValorUnitarioEntrada < 0)
            {
                return BadRequest(new
                {
                    mensagem = "O valor unitário não pode ser negativo."
                });
            }

            var entrada = new EntradaProduto
            {
                Iditem = dto.Iditem,
                DataEntradaProduto = dto.DataEntradaProduto,
                QuantidadeEntrada = dto.QuantidadeEntrada,
                Unidade = dto.Unidade,
                ValorUnitarioEntrada = dto.ValorUnitarioEntrada,
                IditemNavigation = estoque
            };

            _context.EntradaProdutos.Add(entrada);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id = entrada.IdentradaProduto },
                new
                {
                    mensagem = "Entrada cadastrada com sucesso!",
                    idEntrada = entrada.IdentradaProduto,
                    idItem = entrada.Iditem,
                    quantidade = entrada.QuantidadeEntrada
                });
        }

        // PUT: api/EntradasProdutos/1
        // Atualiza uma entrada e ajusta a diferença no estoque
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(
            int id,
            [FromBody] EntradaProdutoCreateDTO dto)
        {
            var entrada = await _context.EntradaProdutos
                .FirstOrDefaultAsync(e => e.IdentradaProduto == id);

            if (entrada == null)
            {
                return NotFound(new
                {
                    mensagem = "Entrada de produto não encontrada."
                });
            }

            if (dto.Iditem != entrada.Iditem)
            {
                return BadRequest(new
                {
                    mensagem = "Não é permitido trocar o produto de uma entrada existente."
                });
            }

            if (dto.QuantidadeEntrada <= 0)
            {
                return BadRequest(new
                {
                    mensagem = "A quantidade deve ser maior que zero."
                });
            }

            if (dto.ValorUnitarioEntrada < 0)
            {
                return BadRequest(new
                {
                    mensagem = "O valor unitário não pode ser negativo."
                });
            }

            var estoque = await _context.Estoques
                .FindAsync(entrada.Iditem);

            if (estoque == null)
            {
                return BadRequest(new
                {
                    mensagem = "Não existe estoque cadastrado para esse item."
                });
            }

            int diferenca =
                dto.QuantidadeEntrada - entrada.QuantidadeEntrada;

            await using var transacao =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Ajusta o estoque somente pela diferença
                estoque.QuantidadeEstoque += diferenca;

                entrada.DataEntradaProduto = dto.DataEntradaProduto;
                entrada.QuantidadeEntrada = dto.QuantidadeEntrada;
                entrada.Unidade = dto.Unidade;
                entrada.ValorUnitarioEntrada = dto.ValorUnitarioEntrada;

                await _context.SaveChangesAsync();
                await transacao.CommitAsync();

                return NoContent();
            }
            catch
            {
                await transacao.RollbackAsync();
                throw;
            }
        }

        // DELETE: api/EntradasProdutos/1
        // Exclui uma entrada e desconta sua quantidade do estoque
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var entrada = await _context.EntradaProdutos
                .FirstOrDefaultAsync(e => e.IdentradaProduto == id);

            if (entrada == null)
            {
                return NotFound(new
                {
                    mensagem = "Entrada de produto não encontrada."
                });
            }

            var estoque = await _context.Estoques
                .FindAsync(entrada.Iditem);

            if (estoque == null)
            {
                return BadRequest(new
                {
                    mensagem = "Não existe estoque cadastrado para esse item."
                });
            }

            if (estoque.QuantidadeEstoque < entrada.QuantidadeEntrada)
            {
                return Conflict(new
                {
                    mensagem = "O estoque atual é menor que a quantidade dessa entrada. Não é possível excluí-la com segurança."
                });
            }

            await using var transacao =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Retira do estoque a quantidade da entrada excluída
                estoque.QuantidadeEstoque -= entrada.QuantidadeEntrada;

                _context.EntradaProdutos.Remove(entrada);

                await _context.SaveChangesAsync();
                await transacao.CommitAsync();

                return NoContent();
            }
            catch
            {
                await transacao.RollbackAsync();
                throw;
            }
        }
    }
}
