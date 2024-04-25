using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.DTOs;

public record class UsuarioDTO
{
    [Display(Name = "Username")]    
    [Required(ErrorMessage = "{0} es requerido")]
    public string Login { get; set; } = null!;
    [Display(Name = "Rol")]
    [Required(ErrorMessage = "{0} es requerido")]
    public int IdRol { get; set; }
    [Display(Name = "Descripción")]
    [Required(ErrorMessage = "{0} es requerido")]
    public string? DescripcionRol { get; set; } = default;
    [Display(Name = "Contraseña")]
    [Required(ErrorMessage = "{0} es requerido")]
    public string? Password { get; set; } = default;
    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "{0} es requerido")]
    public string? Nombre { get; set; } = default;
    [Display(Name = "Apellido")]
    [Required(ErrorMessage = "{0} es requerido")]
    public string? Apellido { get; set; } = default;
    [Display(Name = "Estado")]
    public bool Estado { get; set; }
}
