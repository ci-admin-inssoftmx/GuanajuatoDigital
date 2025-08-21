using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class TipoVehiculos
    {
        public int IdTipoVehiculo { get; set; }

        public string TipoVehiculo { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
    }
}
