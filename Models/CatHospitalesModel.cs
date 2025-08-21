using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatHospitalesModel
    {
        public int IdHospital { get; set; }

        public string NombreHospital { get; set; }

        public int IdMunicipio { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string Municipio { get; set; }


        public string estatusDesc { get; set; }

        public bool ValorEstatusHospitales { get; set; }
        public int idEntidad { get; set; }
        public int? Corp { get; set; }

    }
}
