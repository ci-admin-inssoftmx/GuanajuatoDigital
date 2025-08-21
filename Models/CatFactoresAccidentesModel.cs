using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatFactoresAccidentesModel
    {
        public int IdFactorAccidente { get; set; }

        public string FactorAccidente { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
        public bool ValorEstatusFactores { get; set; }
        public int? Corp { get; set; }


    }
}
