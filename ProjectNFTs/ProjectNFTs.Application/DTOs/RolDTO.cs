using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.DTOs;

public class RolDTO
{
    [Display(Name = "Código")]
    public int Id { get; set; }
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }
}
