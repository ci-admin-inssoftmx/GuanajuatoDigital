using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class TiposCargaModel
    {
        public int IdTipoCarga { get; set; }

        public string TipoCarga { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string EstatusDesc { get; set; }
        
        public bool ValorEstatusTiposCarga { get; set; }


    }
}
