using System;
using System.Collections.Generic;

namespace ProjectNFTs.Infraestructure.Models;

public partial class EncabezadoFactura
{
    public int? IdTarjeta { get; set; }

    public int? EstadoFactura { get; set; }

    public DateTime? FechaFacturacion { get; set; }

    public string? NumeroTarjeta { get; set; }

    public decimal? Total { get; set; }

    public int IdFactura { get; set; }

    public Guid? IdCliente { get; set; }

    public virtual ICollection<DetalleFactura> DetalleFactura { get; set; } = new List<DetalleFactura>();

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual Tarjeta? IdTarjetaNavigation { get; set; }
}
