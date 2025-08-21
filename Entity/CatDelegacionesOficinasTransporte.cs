using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatDelegacionesOficinasTransporte
    {
        public int IdOficinaTransporte { get; set; }

        public int? IdDelegacion { get; set; }

        public string NombreOficina { get; set; }

        public string JefeOficina { get; set; }

        public int? IdMunicipio { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        //public CatMunicipios Municipio { get; set;}

        public CatDelegacione Delegacion { get; set; }
    }
}
