
using LojaAPI.Data;
using LojaAPI.DTOs;
using LojaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionariosController : ControllerBase
    {
        private readonly LojaContext _context;

        public FuncionariosController(LojaContext context)
        {
            _context = context;
        }

        // GET: api/Funcionarios
        // Lista todos os funcionários
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var funcionarios = await _context.Funcionarios
                .Include(f => f.IdpessoaNavigation)
                .ToListAsync();

            return Ok(funcionarios);
        }

        // GET: api/Funcionarios/1
        // Busca um funcionário pelo ID da pessoa
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var funcionario = await _context.Funcionarios
                .Include(f => f.IdpessoaNavigation)
                    .ThenInclude(p => p.Enderecos)
                .FirstOrDefaultAsync(f => f.Idpessoa == id);

            if (funcionario == null)
            {
                return NotFound(new
                {
                    mensagem = "Funcionário não encontrado."
                });
            }

            return Ok(funcionario);
        }

        // POST: api/Funcionarios
        // Cadastra uma pessoa, um funcionário e seu endereço
        [HttpPost]
        public async Task<IActionResult> Cadastrar(
            [FromBody] FuncionarioCreateDTO dto)
        {
            await using var transacao =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Cadastra os dados pessoais
                var pessoa = new Pessoa
                {
                    Nome = dto.Nome,
                    Cpf = dto.Cpf,
                    DataNascimento = dto.DataNascimento,
                    Sexo = dto.Sexo,
                    Email = dto.Email,
                    Telefone = dto.Telefone,
                    AtivoPessoa = dto.AtivoPessoa
                };

                _context.Pessoas.Add(pessoa);
                await _context.SaveChangesAsync();

                // 2. Cadastra os dados profissionais
                var funcionario = new Funcionario
                {
                    Idpessoa = pessoa.Idpessoa,
                    Cargo = dto.Cargo,
                    DataAdmissao = dto.DataAdmissao
                };

                _context.Funcionarios.Add(funcionario);

                // 3. Cadastra o endereço
                var endereco = new Endereco
                {
                    Idpessoa = pessoa.Idpessoa,
                    Cep = dto.Cep,
                    Numero = dto.Numero,
                    Complemento = dto.Complemento
                };

                _context.Enderecos.Add(endereco);

                await _context.SaveChangesAsync();
                await transacao.CommitAsync();

                return Created(
                    "/api/Funcionarios",
                    new
                    {
                        mensagem = "Funcionário cadastrado com sucesso!",
                        idPessoa = pessoa.Idpessoa,
                        nome = pessoa.Nome,
                        cargo = funcionario.Cargo
                    });
            }
            catch
            {
                await transacao.RollbackAsync();
                throw;
            }
        }

        // PUT: api/Funcionarios/1
        // Atualiza os dados pessoais, profissionais e endereço
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(
            int id,
            [FromBody] FuncionarioCreateDTO dto)
        {
            var funcionario = await _context.Funcionarios
                .Include(f => f.IdpessoaNavigation)
                    .ThenInclude(p => p.Enderecos)
                .FirstOrDefaultAsync(f => f.Idpessoa == id);

            if (funcionario == null)
            {
                return NotFound(new
                {
                    mensagem = "Funcionário não encontrado."
                });
            }

            // Atualiza os dados pessoais
            var pessoa = funcionario.IdpessoaNavigation;

            pessoa.Nome = dto.Nome;
            pessoa.Cpf = dto.Cpf;
            pessoa.DataNascimento = dto.DataNascimento;
            pessoa.Sexo = dto.Sexo;
            pessoa.Email = dto.Email;
            pessoa.Telefone = dto.Telefone;
            pessoa.AtivoPessoa = dto.AtivoPessoa;

            // Atualiza os dados profissionais
            funcionario.Cargo = dto.Cargo;
            funcionario.DataAdmissao = dto.DataAdmissao;

            // Atualiza o primeiro endereço ou cadastra um novo
            var endereco = pessoa.Enderecos.FirstOrDefault();

            if (endereco != null)
            {
                endereco.Cep = dto.Cep;
                endereco.Numero = dto.Numero;
                endereco.Complemento = dto.Complemento;
            }
            else
            {
                pessoa.Enderecos.Add(new Endereco
                {
                    Idpessoa = pessoa.Idpessoa,
                    Cep = dto.Cep,
                    Numero = dto.Numero,
                    Complemento = dto.Complemento
                });
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Funcionarios/1
        // Exclui um funcionário
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var funcionario = await _context.Funcionarios
                .FirstOrDefaultAsync(f => f.Idpessoa == id);

            if (funcionario == null)
            {
                return NotFound(new
                {
                    mensagem = "Funcionário não encontrado."
                });
            }

            // Verifica se existem itens de atendimento vinculados
            bool possuiAtendimentos = await _context.Atendimentoitems
                .AnyAsync(a => a.Idfuncionario == id);

            if (possuiAtendimentos)
            {
                return Conflict(new
                {
                    mensagem = "Não é possível excluir este funcionário, pois ele possui atendimentos vinculados."
                });
            }

            _context.Funcionarios.Remove(funcionario);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
