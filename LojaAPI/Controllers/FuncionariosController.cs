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

        // POST: api/Funcionarios
        // Cadastra uma pessoa, um funcionário e seu endereço
        [HttpPost]
        public async Task<IActionResult> Cadastrar(
            FuncionarioCreateDto dto)
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

                // Salva para gerar o ID da pessoa
                await _context.SaveChangesAsync();

                // 2. Cadastra os dados do funcionário
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

                // Salva o funcionário e o endereço
                await _context.SaveChangesAsync();

                // Confirma todas as alterações
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
                // Desfaz as alterações caso ocorra um erro
                await transacao.RollbackAsync();
                throw;
            }
        }
    }
}
