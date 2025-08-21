using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El campo es Obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo es Obligatorio")]
        [Display(Name = "Primer Apellido")]
        public string Paterno { get; set; }
        [Display(Name = "Segundo Apellido")]
        public string Materno { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo es Obligatorio")]
        public int Estatus { get; set; }
        public string Login { get; set; }

        [Display(Name = "Contraseña")]
        public string Clave { get; set; }

        [Required(ErrorMessage = "El campo es Obligatorio")]
        public int UsrAlta { get; set; }

        [Required(ErrorMessage = "El campo es Obligatorio")]
        public DateTime FechaAlta { get; set; }

        public int UsrBaja { get; set; }

        public DateTime FechaBaja { get; set; }

        [Required(ErrorMessage = "El campo es Obligatorio")]
        public int UsrUmodif { get; set; }

        [Required(ErrorMessage = "El campo es Obligatorio")]
        public DateTime FechaUmodif { get; set; }

      
    }
}
