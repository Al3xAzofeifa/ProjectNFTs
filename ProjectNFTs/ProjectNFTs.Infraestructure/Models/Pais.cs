using System;
using System.Collections.Generic;

namespace ProjectNFTs.Infraestructure.Models;

public partial class Pais
{
    public int IdPais { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Cliente> Cliente { get; set; } = new List<Cliente>();
}
