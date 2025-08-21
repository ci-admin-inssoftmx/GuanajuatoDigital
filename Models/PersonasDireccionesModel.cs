using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class PersonasDireccionesModel
    {
        public int idPersonasDirecciones { get; set; }
        public int idEntidad { get; set; }
        public int idMunicipio { get; set; }
        public int codigoPostal { get; set; }
        public string colonia { get; set; }
        public string calle { get; set; }
        public string numero { get; set; }
        public int telefono { get; set; }
        public string correo { get; set; }
        public int idPersona { get; set; }
        public int actualizadoPor { get; set; }
        public DateTime fechaActualizacion { get; set; }
        public int estatus { get; set; }


    }
}
