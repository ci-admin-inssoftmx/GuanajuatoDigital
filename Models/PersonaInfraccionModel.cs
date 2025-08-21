using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class PersonaInfraccionModel : EntityModel
    {
        public int idPersona { get; set; }
        public int idPersonaConductor { get; set; }

        public int idGenero { get; set; }
        public int idTipoLicencia { get; set; }       
        public string numeroLicencia { get; set; }
        public string CURP { get; set; }
        public string RFC { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public int idCatTipoPersona { get; set; }
        public string tipoPersona { get; set; }
        public string tipoLicencia { get; set; }
        public string genero { get; set; }
        public bool generoBool { get; set; }

        public DateTime vigenciaLicencia { get; set; }
        public DateTime? fechaNacimiento { get; set; }

        public string nombreCompleto { get { return (nombre ?? "-") + " " + (apellidoPaterno ?? "-") + " " + (apellidoMaterno ?? "-"); } }
        public virtual PersonaDireccionModel PersonaDireccion { get; set; }

    }
}
