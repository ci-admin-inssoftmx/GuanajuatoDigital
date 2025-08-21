using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatResponsablesPensionesModel
    {
        public int IdResponsable { get; set; }

        public string Responsable { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
    }
}
