using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ILicenciasService
    { 
        public List<LicenciaPersonaDatos> ObtenerDatosPersona(string licencia, string curp, string rfc, string nombre, string primer_apellido, string segundo_apellido);
        public LicenciaPersonaDatos ObtenerDatosPersonaBD1(string licencia, string curp, string rfc, string nombre, string primer_apellido, string segundo_apellido);
        public LicenciaPersonaDatos ObtenerDatosPersonaBD2(string licencia, string curp, string rfc, string nombre, string primer_apellido, string segundo_apellido);
        public LicenciaPersonaDatos ObtenerDatosPersonaBD3(string licencia, string curp, string rfc, string nombre, string primer_apellido, string segundo_apellido);

    }
}
