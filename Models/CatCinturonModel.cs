using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatCinturonModel
    {
        public int IdCinturon { get; set; }

        public string Cinturon { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
    }
}
