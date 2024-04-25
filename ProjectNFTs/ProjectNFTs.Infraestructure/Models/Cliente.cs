using System;
using System.Collections.Generic;

namespace ProjectNFTs.Infraestructure.Models;

public partial class Cliente
{
    public string? IdCriptoWallet { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido1 { get; set; }

    public string Apellido2 { get; set; } = null!;

    public string? Email { get; set; }

    public string? Sexo { get; set; }

    public DateTime? FechaDeNacimiento { get; set; }

    public int? IdPais { get; set; }

    public Guid IdCliente { get; set; }

    public virtual ICollection<EncabezadoFactura> EncabezadoFactura { get; set; } = new List<EncabezadoFactura>();

    public virtual Pais? IdPaisNavigation { get; set; }
}
