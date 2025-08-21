using System;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class PensionModel
    {

        public int IdPension { get; set; }
        public int? Indicador { get; set; }
        public string Pension { get; set; }
        public string Permiso { get; set; }
        public int IdDelegacion { get; set; }
        public int IdResponsable { get; set; }
        public int IdMunicipio { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string estatusDesc { get { if (this.estatus == 1) { return "Activo"; } else { return "Inactivo"; } } }

        

        [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$", ErrorMessage = "Ingresa un correo electrónico válido.")]
        public string Correo { get; set; }
        public string Longitud { get; set; }
        public string Latitud { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int ActualizadoPor { get; set; }
        public int estatus { get; set; }
        public string responsable { get; set; }
        public string delegacion { get; set; }
        public string municipio { get; set; }
        public string Asociados { get; set; }
        public string concesionario { get; set; }
        public string placas { get; set; }
        public string strIdGruas { get; set; }
        public string descripcionEstatus { get; set; }

        
    }
}
