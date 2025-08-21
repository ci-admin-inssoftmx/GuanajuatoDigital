using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatAutoridadesEntregaModel
    {
        public int IdAutoridadEntrega { get; set; }

        public string AutoridadEntrega { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }

        public bool ValorEstatusAutEntrega { get; set; }
        public int? Corp { get; set; }

    }
}
