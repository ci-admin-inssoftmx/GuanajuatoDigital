using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class FirmasModel
    {
        public int Id { get; set; }
		public string DependenciaNombre { get; set; }
		public int IdPuesto { get; set; }
		public string CargoNombre { get; set; }
        public string Nombre { get; set; }
		public string Estatus { get; set; }
		public int? EstatusNumero { get; set; }
		public IFormFile File { get; set; }
		public string FileUrl { get; set; }
		public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
    }

    public class FirmasListModel
    {
        public List<FirmasModel> firmasList { get; set; }
    }
}
