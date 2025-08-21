using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatEstadoVictimaModel
    {
        public int IdEstadoVictima { get; set; }

        public string EstadoVictima { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
    }
}
