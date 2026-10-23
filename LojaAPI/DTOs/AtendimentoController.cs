
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using LojaAPI.Models;
using LojaAPI.DTOs;
using LojaAPI.Data;

namespace LojaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtendimentoController : ControllerBase
    {
        private readonly LojaContext _context;

        public AtendimentoController(LojaContext context)
        {
            _context = context;
        }

        // Listar ATENDIMENTOS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AtendimentoListDTO>>> GetAtendimentos()
        {
            var atendimentos = await _context.Atendimentos
                .Select(a => new AtendimentoListDTO
                {
                    IdAtendimento = a.Idatendimento,
                    IdPessoa = a.Idpessoa,
                    NomePessoa = a.IdpessoaNavigation.Nome,
                    DataAtendimento = a.DataAtendimento,
                    StatusAtendimento = a.StatusAtendimento,

                    Itens = a.Atendimentoitems
                        .Select(i => new AtendimentoItemListDTO
                        {
                            IdAtendimento = i.Idatendimento,
                            IdItem = i.Iditem,
                            Item = i.IditemNavigation.Item1,

                            // ID do funcionário (inteiro)
                            IdFuncionario = i.Idfuncionario,

                            Cargo = i.IdfuncionarioNavigation.Cargo,
                            Quantidade = i.QuantidadeItens,
                            ValorUnitario = i.ValorunitarioEntrada
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(atendimentos);
        }
    }
}
