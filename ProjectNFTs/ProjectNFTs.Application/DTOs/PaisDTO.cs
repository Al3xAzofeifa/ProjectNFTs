using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.DTOs;

public class PaisDTO
{
    [Display(Name = "Código")]
    [Required(ErrorMessage = "{0} es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "{0} debe ser mayor o igual a 1")]
    public int IdPais { get; set; }

    [Display(Name = "Descripción")]
    [Required(ErrorMessage = "{0} es requerida")]
    public string? Descripcion { get; set; }

}
