using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class DatosLicenciaModel
    {
        public string NumeroLicencia { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }

        // Agrega la propiedad para almacenar las licencias encontradas
        public List<ResultadoLicenciaModel> LicenciasEncontradas { get; set; }
    }

}
