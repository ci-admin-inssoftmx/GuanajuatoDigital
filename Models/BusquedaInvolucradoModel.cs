using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class BusquedaInvolucradoModel
    {
        public int idPersona
        { get; set; }
        public string licencia { get; set; }
        public string tipoLicencia { get; set; }

        public string curp { get; set; }
        public string curpBusqueda { get; set; }

        public string rfc { get; set; }
        public string rfcBusqueda { get; set; }

        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string sexo { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public DateTime fechaExpedicion { get; set; }
        public DateTime fechaVigencia { get; set; }
        
        public string entidad { get; set; }
        public string municipio { get; set; }
        public string calle { get; set; }
        public string numero { get; set; }
        public string colonia { get; set; }
        public string telefono { get; set; }
        public string correo { get; set; }

        public List<CapturaAccidentesModel> ListCapturaAccidentesModel { get; set; }






    }
}
