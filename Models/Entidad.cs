using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class Entidad
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        public string Descripcion { get; set; }
    }
}
