using GuanajuatoAdminUsuarios.Entity;
using System;

namespace GuanajuatoAdminUsuarios.Models.Catalogos.Turnos
{
    public class TurnoDetailsModel
    {
        public long IdTurno { get; set; }
        public string Nombre { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        public int IdDelegacion { get; set; }

        public string Delegacion { get; set; }

        public static TurnoDetailsModel FromEntity(CatTurno entity)
        {
            return new TurnoDetailsModel
            {
                IdTurno = entity.IdTurno,
                Nombre = entity.NombreTurno,
                HoraInicio = entity.InicioTurno,
                HoraFin = entity.FinTurno,
                IdDelegacion = entity.IdDelegacion,
                Delegacion = entity.Delegacion?.Delegacion,
            };
        }
    }
}
