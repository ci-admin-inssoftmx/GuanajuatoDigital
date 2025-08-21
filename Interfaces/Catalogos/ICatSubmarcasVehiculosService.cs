using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatSubmarcasVehiculosService
    {
        List<CatSubmarcasVehiculosModel> ObtenerSubarcas(int corp);

        public CatSubmarcasVehiculosModel GetSubmarcaByID(int IdSubmarca); 
        public List<CatSubmarcasVehiculosModel> GetSubmarcaByIDMarca(int IdMarca, int corp);
        public List<CatSubmarcasVehiculosModel> GetSubmarcaActiveByIDMarca(int IdMarca, int corp);
        

        bool ValidarExistenciaSubmarca(int idMarca, string descripcion, int corp);

		public int GuardarSubmarca(CatSubmarcasVehiculosModel model, int corp);
        public int UpdateSubmarca(CatSubmarcasVehiculosModel model);
        public int obtenerIdPorSubmarca(string submarcaLimpio, int corp);
        
    }
}
