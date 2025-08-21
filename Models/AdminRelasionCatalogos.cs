using DocumentFormat.OpenXml.Presentation;

namespace GuanajuatoAdminUsuarios.Models
{
    public class AdminRelasionCatalogos
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Corporacion { get; set; }
        public string Padre { get; set; }
        public int idPadre { get; set; }
        public int idcorporacion { get; set; }
        public int idCatalogo { get; set; }


    }
}
