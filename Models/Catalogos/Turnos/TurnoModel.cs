using System.ComponentModel.DataAnnotations;
using GuanajuatoAdminUsuarios.Entity;

namespace GuanajuatoAdminUsuarios.Models.Catalogos.Turnos
{
    public class TurnoModel : TurnoBaseModel
    {
        [Required(ErrorMessage = TurnoErrors.RequiredMessage)]
        public long IdTurno { get; set; }

        public override CatTurno ToEntity()
        {
            return new CatTurno
            {
                IdTurno = IdTurno,
                NombreTurno = Nombre,
                InicioTurno = HoraInicio.GetValueOrDefault(),
                FinTurno = HoraFin.GetValueOrDefault(),
                IdDelegacion = IdDelegacion.GetValueOrDefault(),
            };
        }

        public static TurnoModel FromEntity(CatTurno entity)
        {
            return new TurnoModel
            {
                IdTurno = entity.IdTurno,
                Nombre = entity.NombreTurno,
                HoraInicio = entity.InicioTurno,
                HoraFin = entity.FinTurno,
                IdDelegacion = entity.IdDelegacion,
            };
        }
    }
}
