
namespace LojaAPI.DTOs
{
    public class FuncionarioCreateDto
    {
        // Dados pessoais
        public string Nome { get; set; } = string.Empty;
        public ulong Cpf { get; set; }
        public DateOnly DataNascimento { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ulong Telefone { get; set; }
        public string AtivoPessoa { get; set; } = string.Empty;

        // Dados profissionais
        public string Cargo { get; set; } = string.Empty;
        public DateOnly DataAdmissao { get; set; }

        // Dados do endereço
        public string Cep { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
    }
}
