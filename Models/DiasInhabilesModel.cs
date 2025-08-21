using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class DiasInhabilesModel
    {
        public int idDiaInhabil { get; set; }

        public DateTime fecha { get; set; }

        public int idMunicipio { get; set; }
          
        public string todosMunicipiosDesc { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string Municipio { get; set; }

        public string EstatusDesc { get; set; }

         public bool ValorEstatusDiasInhabiles { get; set; }
        public int? Corp { get; set; }


    }

}
