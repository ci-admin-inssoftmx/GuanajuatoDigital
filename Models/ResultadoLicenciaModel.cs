using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class ResultadoLicenciaModel
    {
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }

        public string TipoLicencia { get; set; }
        public int tipoLicenciaVal { get; set; }
        
        public string NumeroLicencia { get; set; }

        public DateTime FechaExpedicion { get; set; }
        public DateTime FechaVigencia { get; set; }
    }
}
