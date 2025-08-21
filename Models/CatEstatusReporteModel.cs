using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatEstatusReporteModel
    {
        public int idEstatusReporte { get; set; }
        public string estatusReporte { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
    }
}
