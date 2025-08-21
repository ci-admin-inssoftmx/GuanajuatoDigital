using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatColores
    {
        public int IdColor { get; set; }

        public string color { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? transito { get; set; }

        public int? Estatus { get; set; }
    }
}
