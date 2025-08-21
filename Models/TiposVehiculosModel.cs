using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class TiposVehiculosModel
    {

            public int IdTipoVehiculo { get; set; }

            public string TipoVehiculo { get; set; }

            public DateTime? FechaActualizacion { get; set; }

            public int? ActualizadoPor { get; set; }

            public int? Estatus { get; set; }
        public int Corp { get; set; }

        public string EstatusDesc { get; set; }

           public bool ValorEstatusTiposVehiculo { get; set; }



    }
}

