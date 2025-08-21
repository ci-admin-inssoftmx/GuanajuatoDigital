using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatFactoresOpcionesAccidentesModel
    {
        public int IdFactorOpcionAccidente { get; set; }

        public string FactorOpcionAccidente { get; set; }

        public int IdFactorAccidente { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string FactorAccidente { get; set; }

        public string estatusDesc { get; set; }

        public bool ValorEstatusFactoresOpciones { get; set; }
        public int? Corp { get; set; }


    }
}
