
namespace LojaAPI.DTOs
{
    public class AtendimentoDetalheDTO
    {
        public int Idatendimento { get; set; }
        public DateOnly DataAtendimento { get; set; }
        public string StatusAtendimento { get; set; } = string.Empty;

        public PessoaResumoDTO? Pessoa { get; set; }

        public List<AtendimentoItemDetalheDTO> Itens { get; set; } = new();
    }

    public class PessoaResumoDTO
    {
        public int Idpessoa { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    public class AtendimentoItemDetalheDTO
    {
        public int Iditem { get; set; }
        public string NomeItem { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }

        public FuncionarioResumoDTO? FuncionarioResponsavel { get; set; }
    }

    public class FuncionarioResumoDTO
    {
        public int Idfuncionario { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
    }
}
