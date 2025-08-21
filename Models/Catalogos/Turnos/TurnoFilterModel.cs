using System;

namespace GuanajuatoAdminUsuarios.Models.Catalogos.Turnos
{
    public class TurnoFilterModel
    {
        public string Nombre { get; set; }

        public TimeSpan? HoraInicio { get; set; }

        public TimeSpan? HoraFin { get; set; }

        public int? IdDelegacion { get; set; }

        public bool IsEmpty()
        {
            return 
                string.IsNullOrWhiteSpace(Nombre) && 
                HoraInicio is null && 
                HoraFin is null && 
                IdDelegacion is null;
        }

    }
}
