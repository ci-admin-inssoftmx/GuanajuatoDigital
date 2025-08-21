using System;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class PersonaDireccionModel : EntityModel
    {
        public int idPersonasDirecciones { get; set; }
        public int? idEntidad { get; set; }
        public int? idEntidadFisico { get; set; }

        public string entidad { get; set; }
        public int? idMunicipio { get; set; }
        public int? idMunicipioFisico { get; set; }
        public int? idMunicipioMoral { get; set; }
        public int? idTipoPersona { get; set; }
        public string municipio { get; set; }
        public string codigoPostal { get; set; }
        public string colonia { get; set; }
        public string coloniaFisico { get; set; }

        public string calle { get; set; }
        public string calleFisico { get; set; }

        public string numero { get; set; }
        public string numeroFisico { get; set; }

        public string telefono { get; set; }
        public string telefonoString { get; set; }

        public string telefonoFisico { get; set; }
       
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string correo { get; set; }
        public string correoFisico { get; set; }

        public int? idPersona { get; set; }

    }
}
