using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatAsientoModel
    {
        public int IdAsiento { get; set; }

        public string Asiento { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
    }
}
