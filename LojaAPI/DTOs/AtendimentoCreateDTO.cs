
namespace LojaAPI.DTOs
{
    public class AtendimentoCreateDTO
    {
        public int Idpessoa { get; set; }
        public DateOnly DataAtendimento { get; set; }
        public string StatusAtendimento { get; set; } = string.Empty;
    }
}
