
using LojaAPI.Data;
using LojaAPI.DTOs;
using LojaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtendimentosController : ControllerBase
    {
        private readonly LojaContext _context;

        public AtendimentosController(LojaContext context)
        {
            _context = context;
        }

        // GET: api/Atendimentos
        // Lista todos os atendimentos
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var atendimentos = await _context.Atendimentos
                .Include(a => a.IdpessoaNavigation)
                .ToListAsync();

            return Ok(atendimentos);
        }

        // GET: api/Atendimentos/1
        // Busca um atendimento com pessoa, itens e funcionários
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var atendimento = await _context.Atendimentos
                .Include(a => a.IdpessoaNavigation)
                .Include(a => a.Atendimentoitems)
                    .ThenInclude(ai => ai.IditemNavigation)
                .Include(a => a.Atendimentoitems)
                    .ThenInclude(ai => ai.IdfuncionarioNavigation)
                        .ThenInclude(f => f.IdpessoaNavigation)
                .FirstOrDefaultAsync(a => a.Idatendimento == id);

            if (atendimento == null)
            {
                return NotFound(new
                {
                    mensagem = "Atendimento não encontrado."
                });
            }

            var resultado = new AtendimentoDetalheDTO
            {
                Idatendimento = atendimento.Idatendimento,
                DataAtendimento = atendimento.DataAtendimento,
                StatusAtendimento = atendimento.StatusAtendimento,

                Pessoa = new PessoaResumoDTO
                {
                    Idpessoa = atendimento.IdpessoaNavigation.Idpessoa,
                    Nome = atendimento.IdpessoaNavigation.Nome
                },

                Itens = atendimento.Atendimentoitems.Select(ai =>
                    new AtendimentoItemDetalheDTO
                    {
                        Iditem = ai.Iditem,
                        NomeItem = ai.IditemNavigation.Item1,
                        Quantidade = ai.QuantidadeItens,
                        ValorUnitario = ai.ValorunitarioEntrada,

                        FuncionarioResponsavel = new FuncionarioResumoDTO
                        {
                            Idfuncionario = ai.IdfuncionarioNavigation.Idpessoa,
                            Nome = ai.IdfuncionarioNavigation.IdpessoaNavigation.Nome,
                            Cargo = ai.IdfuncionarioNavigation.Cargo
                        }
                    }).ToList()
            };

            return Ok(resultado);
        }

        // POST: api/Atendimentos
        // Cadastra um atendimento
        [HttpPost]
        public async Task<IActionResult> Cadastrar(
            [FromBody] AtendimentoCreateDTO dto)
        {
            var pessoa = await _context.Pessoas
                .FindAsync(dto.Idpessoa);

            if (pessoa == null)
            {
                return BadRequest(new
                {
                    mensagem = "A pessoa informada não existe."
                });
            }

            var atendimento = new Atendimento
            {
                Idpessoa = dto.Idpessoa,
                DataAtendimento = dto.DataAtendimento,
                StatusAtendimento = dto.StatusAtendimento
            };

            _context.Atendimentos.Add(atendimento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id = atendimento.Idatendimento },
                atendimento
            );
        }

        // PUT: api/Atendimentos/1
        // Atualiza um atendimento
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(
            int id,
            [FromBody] AtendimentoCreateDTO dto)
        {
            var atendimento = await _context.Atendimentos
                .FindAsync(id);

            if (atendimento == null)
            {
                return NotFound(new
                {
                    mensagem = "Atendimento não encontrado."
                });
            }

            var pessoa = await _context.Pessoas
                .FindAsync(dto.Idpessoa);

            if (pessoa == null)
            {
                return BadRequest(new
                {
                    mensagem = "A pessoa informada não existe."
                });
            }

            atendimento.Idpessoa = dto.Idpessoa;
            atendimento.DataAtendimento = dto.DataAtendimento;
            atendimento.StatusAtendimento = dto.StatusAtendimento;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Atendimentos/1
        // Exclui um atendimento
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var atendimento = await _context.Atendimentos
                .FindAsync(id);

            if (atendimento == null)
            {
                return NotFound(new
                {
                    mensagem = "Atendimento não encontrado."
                });
            }

            // Verifica se existem itens vinculados
            bool possuiItens = await _context.Atendimentoitems
                .AnyAsync(a => a.Idatendimento == id);

            if (possuiItens)
            {
                return Conflict(new
                {
                    mensagem = "Não é possível excluir o atendimento enquanto houver itens vinculados."
                });
            }

            _context.Atendimentos.Remove(atendimento);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
