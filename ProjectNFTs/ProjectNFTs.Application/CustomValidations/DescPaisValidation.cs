using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.CustomValidations;

public class DescPaisValidation : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Verifica si el valor es nulo o una cadena vacía
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            // Si es nulo o una cadena vacía, es válido
            return ValidationResult.Success!;
        }

        // Si no es nulo o una cadena vacía, es inválido
        return new ValidationResult(ErrorMessage);
    }
}
