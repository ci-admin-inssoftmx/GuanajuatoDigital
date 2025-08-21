using System.Collections.Generic;
using GuanajuatoAdminUsuarios.Models.Components;

namespace GuanajuatoAdminUsuarios.Models
{
    public class BusquedaPersonaModel
    {
        public int idVigencia { get; set; }

		public string NombreBusqueda { get; set; }
        public string ApellidoPaternoBusqueda { get; set; }
        public string ApellidoMaternoBusqueda { get; set; }
        public string RFCBusqueda { get; set; }
        public string CURPBusqueda { get; set; }
        public int IdTipoPersona { get; set; }


        public string NumeroLicenciaBusqueda { get; set; }
        public PersonaModel PersonaModel { get; set; }

        public Pagination Pagination { get; set; }

        public BusquedaPersonaFisicaConfig Config { get; set; }
        public int Total { get; set; }
        public List<PersonaModel> ListadoPersonas { get; set; } = new();

        public List<PersonaModel> ListadoPersonasOtras { get; set; } = new();

        public override string ToString()
        {
            return $"[Nombre:{NombreBusqueda},Paterno:{ApellidoPaternoBusqueda},Materno:{ApellidoMaternoBusqueda},RFC:{RFCBusqueda},CURP:{CURPBusqueda},No.Licencia:{NumeroLicenciaBusqueda}]";
        }
        public string numeroLicenciaBusqueda { get; set; }
        public string nombreBusqueda { get; set; }
        public string apellidoPaternoBusqueda { get; set; }
        public string apellidoMaternoBusqueda { get; set; }

    }

}
