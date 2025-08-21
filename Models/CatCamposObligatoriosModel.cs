using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatCamposObligatoriosModel
    {
        public int IdCampoObligatorio { get; set; }
        public int IdDelegacion { get; set; }

        public string Delegacion { get; set; }

        public int IdMunicipio { get; set; }

        public string Municipio { get; set; }
        
        public int IdCampo { get; set; }

        public string Campo { get; set; }

        public int? EstatusAccidente { get; set; }

        public int? EstatusInfracciones { get; set; }

        public int? EstatusDepositos { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

    }
}
