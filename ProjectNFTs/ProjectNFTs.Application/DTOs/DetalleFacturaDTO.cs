using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.DTOs;

public record DetalleFacturaDTO
{

    public int IdFactura { get; set; }

    [Display(Name = "No")]
    public int IdDetalle { get; set; }

    [Display(Name = "Código")]
    public Guid? IdNft { get; set; }

    [Display(Name = "Nombre NFT")]
    public string DescripcionNFT { get; set; } = default!;

    [Display(Name = "Cantidad")]
    public int Cantidad { get; set; }

    [DisplayFormat(DataFormatString = "{0:n2}")]
    [Display(Name = "Precio")]
    public decimal Precio { get; set; }

    public string ImagenUrl { get; set; }

    [DisplayFormat(DataFormatString = "{0:n2}")]
    [Display(Name = "Total")]
    public decimal TotalLinea { get; set; }
}
