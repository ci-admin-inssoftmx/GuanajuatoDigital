using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatTramosService

    {

        List<CatTramosModel> ObtenerTamosPorCarretera(int carreteraDDValue,int corp);
        List<CatTramosModel> ObtenerTamosPorCarreteraActivos(int carreteraDDValue,int corp);

        List<CatTramosModel> ObtenerTramos(int corp);

        public CatTramosModel ObtenerTramoByID(int IdTramo);
        public int CrearTramo(CatTramosModel model, int corp);
        public int EditarTramo(CatTramosModel model);

    }
}
