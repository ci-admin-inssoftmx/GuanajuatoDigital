using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class TiposCarga
    {
        public int IdTipoCarga { get; set; }

        public string TipoCarga { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
    }
}
