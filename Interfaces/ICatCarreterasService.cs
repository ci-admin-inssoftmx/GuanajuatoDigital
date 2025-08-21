using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatCarreterasService
    {

        List<CatCarreterasModel> ObtenerCarreteras();
        List<CatCarreterasModel> ObtenerCarreteras(int delegacion);
        public CatCarreterasModel ObtenerCarreteraByID(int IdCarretera);
        public int CrearCarretera(CatCarreterasModel model);
       public int EditarCarretera(CatCarreterasModel model);
        List<CatCarreterasModel> GetCarreterasPorDelegacion(int idOficina);
        List<CatCarreterasModel> GetCarreterasPorDelegacionTodos(int idOficina);
        List<CatCarreterasModel> GetCarreterasPorDelegacion2(int idOficina);

        List<CatCarreterasModel> GetCarreterasParaIngreso(int idMunicipio);

    }
}
