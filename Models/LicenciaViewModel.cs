using System.Collections.Generic;
using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class LicenciaViewModel
    {
        public string Nombre { get; set; }
        public string NumeroLicencia { get; set; }
        public DateTime FechaExpedicion { get; set; }
        public DateTime FechaVigencia { get; set; }
    }
}
