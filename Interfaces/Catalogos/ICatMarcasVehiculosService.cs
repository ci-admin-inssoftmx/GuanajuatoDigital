using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatMarcasVehiculosService
    {
        List<CatMarcasVehiculosModel> ObtenerMarcas(int corp);
        public List<CatMarcasVehiculosModel> ObtenerMarcasTodas(int corp);

        public CatMarcasVehiculosModel GetMarcaByID(int IdMarcaVehiculo);
       public int GuardarMarca(CatMarcasVehiculosModel model, int corp);
        public int UpdateMarca(CatMarcasVehiculosModel marca);
        public int obtenerIdPorMarca( string marcaLimpio, int corp);
        
    }
}
