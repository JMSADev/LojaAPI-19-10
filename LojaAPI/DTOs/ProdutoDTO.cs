namespace LojaAPI.Models.DTOs;

public class ProdutoDTO
{
    public int Iditem { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public decimal Preco { get; set; }

    public string? Categoria { get; set; }

    public decimal? PrecoVenda { get; set; }

    public string? Unidade { get; set; }

    public int? QuantidadeEstoque { get; set; }
}