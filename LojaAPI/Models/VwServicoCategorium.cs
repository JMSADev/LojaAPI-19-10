using System;
using System.Collections.Generic;

namespace LojaAPI.Models;

public partial class VwServicoCategorium
{
    public int Iditem { get; set; }

    public string Item { get; set; } = null!;

    public string DescricaoItem { get; set; } = null!;

    public decimal Preco { get; set; }

    public string Categoria { get; set; } = null!;

    public string AtivoProdutoServico { get; set; } = null!;
}
