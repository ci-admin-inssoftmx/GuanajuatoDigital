using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatHospitales
    {
        public int IdHospital { get; set; }

        public string NombreHospital { get; set; }

        public int IdMunicipio { get; set; }
        public int transito { get; set; }   

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
    }
}
