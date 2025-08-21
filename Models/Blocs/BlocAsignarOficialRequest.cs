using Microsoft.AspNetCore.Http;

namespace GuanajuatoAdminUsuarios.Models.Blocs
{
    public class BlocAsignarOficialRequest
    {
        public IFormFile File { get; set; }
        public int RegistraId { get; set; }
        public int CatOficial { get; set; }
        public int FolioInicial { get; set; }
        public int FolioFinal { get; set; }
        public int iddelegacion { get; set; }
        public string delegacion { get; set; }
    }
}
