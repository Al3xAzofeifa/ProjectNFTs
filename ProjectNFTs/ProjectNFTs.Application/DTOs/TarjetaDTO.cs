using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.DTOs;

public class TarjetaDTO
{
    [Display(Name = "Código")]
    [Required(ErrorMessage = "{0} es requerido")]
    public int Id { get; set; }

    [Display(Name = "Descripción")]
    [Required(ErrorMessage = "{0} es requerido")]
    public string? Descripcion { get; set; }

    [Display(Name = "Estado")]    
    public bool Estado { get; set; }
}
