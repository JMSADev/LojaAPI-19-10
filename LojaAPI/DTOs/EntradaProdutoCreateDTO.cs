
namespace LojaAPI.DTOs
{
    public class EntradaProdutoCreateDTO
    {
        public int Iditem { get; set; }
        public DateOnly DataEntradaProduto { get; set; }
        public int QuantidadeEntrada { get; set; }
        public string Unidade { get; set; } = string.Empty;
        public decimal ValorUnitarioEntrada { get; set; }
    }
}
