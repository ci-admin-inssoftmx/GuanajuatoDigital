using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class DescripcionesEventoModel
    {
        public int idDescripcion { get; set; }

        public string descripcionEvento { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
    }
}
