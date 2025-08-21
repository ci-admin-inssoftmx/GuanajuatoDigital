using System;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class Oficina
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        public string Descripcion { get; set; }

        [Display(Name = "Entidad")]
        [Required(ErrorMessage = "Debes seleccionar una entidad")]
        public int IdEntidad { get; set; }

        public int Estatus { get; set; }
    }
}
