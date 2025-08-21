using GuanajuatoAdminUsuarios.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace GuanajuatoAdminUsuarios.Models.Catalogos.Turnos
{
    public abstract class TurnoBaseModel : IValidatableObject
    {
        [Required(ErrorMessage = TurnoErrors.RequiredMessage)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = TurnoErrors.RequiredMessage)]
        public TimeSpan? HoraInicio { get; set; }

        [Required(ErrorMessage = TurnoErrors.RequiredMessage)]
        public TimeSpan? HoraFin { get; set; }

        [Required(ErrorMessage = TurnoErrors.RequiredMessage)]
        public int? IdDelegacion { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HoraInicio.HasValue && HoraFin.HasValue && HoraInicio >= HoraFin)
            {
                yield return new ValidationResult(
                    "La hora de finalización debe ser mayor que la hora de inicio.",
                    new[] { nameof(HoraFin) }
                );
            }
        }

        public abstract CatTurno ToEntity();
    }
}
