using System;
using System.Collections.Generic;

namespace ProjectNFTs.Infraestructure.Models;

public partial class Tarjeta
{
    public int Id { get; set; }

    public string? Descripcion { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<EncabezadoFactura> EncabezadoFactura { get; set; } = new List<EncabezadoFactura>();
}
