using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatMunicipiosModel
    {
        public int IdMunicipio { get; set; }
        public int IdEntidad { get; set; }
        public int IdOficinaTransporte{ get; set; }

        public string Municipio { get; set; }
        public string nombreOficina { get; set; }
        public string nombreEntidad { get; set; }
        public string abreviatura { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
        public int? Corp { get; set; }

        public string estatusDesc { get; set; }
        public bool ValorEstatusMunicipio { get; set; }



    }
}
