using System;
using System.Collections.Generic;

namespace ProjectNFTs.Infraestructure.Models;

public partial class DetalleFactura
{
    public int? Cantidad { get; set; }

    public decimal? Precio { get; set; }

    public int IdFactura { get; set; }

    public int IdDetalle { get; set; }

    public Guid IdNft { get; set; }

    public virtual EncabezadoFactura IdFacturaNavigation { get; set; } = null!;

    public virtual Nft IdNftNavigation { get; set; } = null!;
}
