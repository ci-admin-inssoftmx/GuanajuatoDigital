using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class MotivosInfraccion
    {
        public int IdMotivoInfraccion { get; set; }

        public string Nombre { get; set; }

        public int? CalificacionMinima { get; set; }

        public int? CalificacionMaxima { get; set; }

        public int? Calificacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
    }
}
