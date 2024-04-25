using System;
using System.Collections.Generic;

namespace ProjectNFTs.Infraestructure.Models;

public partial class CriptoWallet
{
    public string Id { get; set; } = null!;

    public decimal? Saldo { get; set; }

    public virtual ICollection<Cliente> Cliente { get; set; } = new List<Cliente>();
}
