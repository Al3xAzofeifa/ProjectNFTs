using ProjectNFTs.Application.CustomValidations;
using ProjectNFTs.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.DTOs;

public record ClienteDTO
{
    //Id Cliente
    [Display(Name = "Identificación")]    
    public Guid IdCliente { get; set; }

    // Nombre con DisplayName y validación de longitud
    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "{0} es requerido")]
    [StringLength(50, ErrorMessage = "{0} debe tener como máximo {1} caracteres.")]
    public string? Nombre { get; set; }

    // Apellido1 con DisplayName y validación de longitud
    [Display(Name = "1º Apellido")]
    [Required(ErrorMessage = "{0} es requerido")]
    [StringLength(50, ErrorMessage = "{0} debe tener como máximo {1} caracteres.")]
    public string? Apellido1 { get; set; }

    // Apellido2 con DisplayName y validación de longitud
    [Display(Name = "2º Apellido")]
    [Required(ErrorMessage = "{0} es requerido")]
    [StringLength(50, ErrorMessage = "{0} debe tener como máximo {1} caracteres.")]
    public string? Apellido2 { get; set; }

    // Email con DisplayName y validación de formato
    [Display(Name = "Correo Electrónico")]
    [Required(ErrorMessage = "{0} es requerido")]
    [EmailAddress(ErrorMessage = "Ingrese un {0} válido.")]
    public string? Email { get; set; }

    // Sexo con DisplayName y validación de longitud
    [Display(Name = "Sexo")]
    [Required(ErrorMessage = "{0} es requerido")]
    [StringLength(1, ErrorMessage = "{0} debe tener como máximo {1} selección.")]
    public string? Sexo { get; set; }

    // FechaDeNacimiento con DisplayName y validación de rango
    [Display(Name = "Fecha Nac.")]
    [Required(ErrorMessage = "{0} es requerida")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]    
    public DateTime? FechaDeNacimiento { get; set; }

    // IdPais con DisplayName y validación de rango
    [Display(Name = "País")]
    [Required(ErrorMessage = "{0} es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un {0} válido.")]
    public int? IdPais { get; set; }

    [DescPaisValidation(ErrorMessage = "La descripción del país no es válida.")]
    public string? DescripcionPais { get; set; }

}
