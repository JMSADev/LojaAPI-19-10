
using LojaAPI.Data;
using LojaAPI.DTOs;
using LojaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtendimentoItemsController : ControllerBase
    {
        private readonly LojaContext _context;

        public AtendimentoItemsController(LojaContext context)
        {
            _context = context;
        }

        // GET: api/AtendimentoItems
        // Lista todos os itens dos atendimentos
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var itens = await _context.Atendimentoitems
                .ToListAsync();

            return Ok(itens);
        }

        // POST: api/AtendimentoItems
        // Adiciona um item ao atendimento
        // A trigger do banco valida e desconta o estoque
        [HttpPost]
        public async Task<IActionResult> Cadastrar(
            [FromBody] AtendimentoItemCreateDTO dto)
        {
            if (dto.QuantidadeItens <= 0)
            {
                return BadRequest(new
                {
                    mensagem = "A quantidade deve ser maior que zero."
                });
            }

            if (dto.ValorunitarioEntrada < 0)
            {
                return BadRequest(new
                {
                    mensagem = "O valor unitário não pode ser negativo."
                });
            }

            var atendimento = await _context.Atendimentos
                .FindAsync(dto.Idatendimento);

            if (atendimento == null)
            {
                return BadRequest(new
                {
                    mensagem = "O atendimento informado não existe."
                });
            }

            var item = await _context.Items
                .FindAsync(dto.Iditem);

            if (item == null)
            {
                return BadRequest(new
                {
                    mensagem = "O produto ou serviço informado não existe."
                });
            }

            var funcionario = await _context.Funcionarios
                .FindAsync(dto.Idfuncionario);

            if (funcionario == null)
            {
                return BadRequest(new
                {
                    mensagem = "O funcionário informado não existe."
                });
            }

            var atendimentoItem = new Atendimentoitem
            {
                Idatendimento = dto.Idatendimento,
                Iditem = dto.Iditem,
                Idfuncionario = dto.Idfuncionario,
                QuantidadeItens = dto.QuantidadeItens,
                ValorunitarioEntrada = dto.ValorunitarioEntrada
            };

            _context.Atendimentoitems.Add(atendimentoItem);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Item adicionado ao atendimento com sucesso!"
            });
        }

        // GET: api/AtendimentoItems/1/1
        // Busca um item pela chave composta
        [HttpGet("{idatendimento}/{iditem}")]
        public async Task<IActionResult> BuscarPorId(
            int idatendimento,
            int iditem)
        {
            var registro = await _context.Atendimentoitems
                .FirstOrDefaultAsync(a =>
                    a.Idatendimento == idatendimento &&
                    a.Iditem == iditem);

            if (registro == null)
            {
                return NotFound(new
                {
                    mensagem = "Item do atendimento não encontrado."
                });
            }

            return Ok(registro);
        }

        // PUT: api/AtendimentoItems/1/1
        // Atualiza o item e ajusta a diferença no estoque
        [HttpPut("{idatendimento}/{iditem}")]
        public async Task<IActionResult> Atualizar(
            int idatendimento,
            int iditem,
            [FromBody] AtendimentoItemCreateDTO dto)
        {
            var registro = await _context.Atendimentoitems
                .FirstOrDefaultAsync(a =>
                    a.Idatendimento == idatendimento &&
                    a.Iditem == iditem);

            if (registro == null)
            {
                return NotFound(new
                {
                    mensagem = "Item do atendimento não encontrado."
                });
            }

            if (dto.Idatendimento != idatendimento ||
                dto.Iditem != iditem)
            {
                return BadRequest(new
                {
                    mensagem = "Os IDs do corpo devem corresponder aos IDs da URL."
                });
            }

            if (dto.QuantidadeItens <= 0)
            {
                return BadRequest(new
                {
                    mensagem = "A quantidade deve ser maior que zero."
                });
            }

            if (dto.ValorunitarioEntrada < 0)
            {
                return BadRequest(new
                {
                    mensagem = "O valor unitário não pode ser negativo."
                });
            }

            var funcionario = await _context.Funcionarios
                .FindAsync(dto.Idfuncionario);

            if (funcionario == null)
            {
                return BadRequest(new
                {
                    mensagem = "O funcionário informado não existe."
                });
            }

            var item = await _context.Items
                .FindAsync(iditem);

            if (item == null)
            {
                return BadRequest(new
                {
                    mensagem = "O produto ou serviço não existe."
                });
            }

            int diferenca =
                dto.QuantidadeItens - registro.QuantidadeItens;

            bool ehProduto = item.TipoItem == "P";

            Estoque? estoque = null;

            if (ehProduto)
            {
                estoque = await _context.Estoques
                    .FindAsync(iditem);

                if (estoque == null)
                {
                    return BadRequest(new
                    {
                        mensagem = "Não existe estoque cadastrado para esse produto."
                    });
                }

                // Se a quantidade aumentou, verifica o saldo disponível
                if (diferenca > 0 &&
                    estoque.QuantidadeEstoque < diferenca)
                {
                    return Conflict(new
                    {
                        mensagem = "Estoque insuficiente para aumentar a quantidade do atendimento."
                    });
                }
            }

            await using var transacao =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Aumentar a quantidade do atendimento consome mais estoque.
                // Diminuir a quantidade devolve a diferença ao estoque.
                if (ehProduto && estoque != null)
                {
                    estoque.QuantidadeEstoque -= diferenca;
                }

                registro.Idfuncionario = dto.Idfuncionario;
                registro.QuantidadeItens = dto.QuantidadeItens;
                registro.ValorunitarioEntrada =
                    dto.ValorunitarioEntrada;

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

        // DELETE: api/AtendimentoItems/1/1
        // Exclui o item e devolve ao estoque a quantidade removida
        [HttpDelete("{idatendimento}/{iditem}")]
        public async Task<IActionResult> Excluir(
            int idatendimento,
            int iditem)
        {
            var registro = await _context.Atendimentoitems
                .FirstOrDefaultAsync(a =>
                    a.Idatendimento == idatendimento &&
                    a.Iditem == iditem);

            if (registro == null)
            {
                return NotFound(new
                {
                    mensagem = "Item do atendimento não encontrado."
                });
            }

            var item = await _context.Items
                .FindAsync(iditem);

            if (item == null)
            {
                return BadRequest(new
                {
                    mensagem = "O produto ou serviço não existe."
                });
            }

            Estoque? estoque = null;

            if (item.TipoItem == "P")
            {
                estoque = await _context.Estoques
                    .FindAsync(iditem);

                if (estoque == null)
                {
                    return BadRequest(new
                    {
                        mensagem = "Não existe estoque cadastrado para esse produto."
                    });
                }
            }

            await using var transacao =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Serviços não possuem quantidade de estoque para devolver.
                // Produtos devolvem a quantidade retirada pelo atendimento.
                if (item.TipoItem == "P" && estoque != null)
                {
                    estoque.QuantidadeEstoque +=
                        registro.QuantidadeItens;
                }

                _context.Atendimentoitems.Remove(registro);

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
