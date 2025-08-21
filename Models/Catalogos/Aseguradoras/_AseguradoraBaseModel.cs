using GuanajuatoAdminUsuarios.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace GuanajuatoAdminUsuarios.Models.Catalogos.Aseguradoras
{
    public abstract class AseguradoraBaseModel : IValidatableObject
    {
        [Required(ErrorMessage = AseguradoraErrors.RequiredMessage)]
        public string NombreAseguradora { get; set; }

        [Required(ErrorMessage = AseguradoraErrors.RequiredMessage)]
        public int? Estatus { get; set; }

        public abstract CatAseguradoras ToEntity();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Estatus < 0)
            {
                yield return new ValidationResult(
                    "El valor no puede ser negativo."
                );
            }
        }
    }
}
