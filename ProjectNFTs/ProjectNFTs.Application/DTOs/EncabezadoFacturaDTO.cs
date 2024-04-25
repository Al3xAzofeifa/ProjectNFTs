using ProjectNFTs.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.DTOs;

public record EncabezadoFacturaDTO
{
    [Display(Name = "No Factura")]
    //[ValidateNever]
    public int IdFactura { get; set; }
    
    [Display(Name = "Cliente")]
    [Required(ErrorMessage = "{0} es requerido")]
    public Guid? IdCliente { get; set; }

    [Display(Name = "Tipo Tarjeta")]
    [Required(ErrorMessage = "{0} es requerido")]
    public int? IdTarjeta { get; set; }

    [Required(ErrorMessage = "{0} es requerido")]
    [Display(Name = "No Tarjeta")]
    public string? NumeroTarjeta { get; set; }

    [Display(Name = "Total")]
    public decimal? Total { get; set; }

    [Display(Name = "Estado")]
    public bool EstadoFactura { get; set; }

    public List<DetalleFacturaDTO> ListFacturaDetalle = null!;

    public string? DescripcionTarjeta { get; set; }

    public string? NombreCliente { get; set; }

}
