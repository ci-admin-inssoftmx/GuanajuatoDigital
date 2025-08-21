using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatMunicipios
    {
        public int IdMunicipio { get; set; }

        public string Municipio { get; set; }

        public int? IdOficinaTransporte { get; set; }

        public int transito { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        //public CatDelegacionesOficinasTransporte OficinaTransporte { get; set; }
    }
}

