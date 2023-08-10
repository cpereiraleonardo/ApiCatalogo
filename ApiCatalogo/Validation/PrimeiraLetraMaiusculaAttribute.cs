using System.ComponentModel.DataAnnotations;

namespace ApiCatalogo.Validation
{
    public class PrimeiraLetraMaiusculaAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            //Bypass da validação já usada no DataAnnotation [Requerid] -- Faz isso porque aqui é chamado antes de lá e lá já fará essa validação de valor obrígatório.
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var primeiraLertra = value.ToString()[0].ToString();

            if (primeiraLertra != primeiraLertra.ToUpper())
            {
                return new ValidationResult("A primeira letra do nome do produto dever ser maúscula!");
            }

            return ValidationResult.Success;
        }
    }
}
