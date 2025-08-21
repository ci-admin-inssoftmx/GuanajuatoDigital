using System;
namespace GuanajuatoAdminUsuarios.Entity
{
    public class Delegaciones
    {
        public int IdDelegacion { get; set; }

        public string Delegacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public bool Transito { get; set; }

        public int? IdMunicipio{ get; set; }

        public CatMunicipios Municipio { get; set; }        
    }
}
