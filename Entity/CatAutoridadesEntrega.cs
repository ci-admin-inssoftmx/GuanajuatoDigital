using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatAutoridadesEntrega
    {
        public int IdAutoridadEntrega { get; set; }

        public string AutoridadEntrega { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
    }
}
