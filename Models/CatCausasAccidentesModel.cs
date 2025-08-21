using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatCausasAccidentesModel
    {
        public int IdCausaAccidente { get; set; }

        public string CausaAccidente { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
        public bool SwitchEstatusCausaAccidente { get; set; }
        public int? Corp { get; set; }


    }
}
