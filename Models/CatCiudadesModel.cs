using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatCiudadesModel
    {
        public int IdCiudad { get; set; }
        public int IdEntidad { get; set; }

        public string Ciudad { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
    }
}
