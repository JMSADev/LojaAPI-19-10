
namespace LojaAPI.DTOs
{
    public class AtendimentoItemCreateDTO
    {
        public int Idatendimento { get; set; }
        public int Iditem { get; set; }
        public int Idfuncionario { get; set; }
        public int QuantidadeItens { get; set; }
        public decimal ValorunitarioEntrada { get; set; }
    }
}
