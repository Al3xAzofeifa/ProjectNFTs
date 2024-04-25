using ProjectNFTs.Application.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.DTOs;

public class NftDTO
{
    [DisplayName("ID NFT")]
    public Guid Id { get; set; }

    [DisplayName("Nombre")]
    [Required(ErrorMessage = "{0} es requerido")]
    public string? Nombre { get; set; }

    [DisplayName("Autor")]
    [Required(ErrorMessage = "{0} es requerido")]
    public string? Autor { get; set; }

    [DisplayName("Valor")]
    [Required(ErrorMessage = "{0} es requerido")]
    public decimal Valor { get; set; }

    [DisplayName("Unidades")]
    [Required(ErrorMessage = "{0} es requerido")]
    [RegularExpression("^[1]$", ErrorMessage = "La cantidad en inventario debe ser igual a 1")]
    public int? CantidadInventario { get; set; }

    [DisplayName("Imagen")]
    [Required(ErrorMessage = "{0} es requerida")]
    public byte[] Imagen { get; set; } = null!;


}
