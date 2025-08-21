using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatCausasAccidentes
    {
        public int IdCausaAccidente { get; set; }

        public string CausaAccidente { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
    }
}
