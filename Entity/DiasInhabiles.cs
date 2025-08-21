using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class DiasInhabiles
    {
        public int idDiaInhabil { get; set; }

        public DateTime fecha { get; set; }

        public int idMunicipio { get; set; }

        public string todosMunicipiosDesc { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
    }
}

