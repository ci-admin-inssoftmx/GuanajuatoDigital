using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IAdminCatalogosService
    {
        public List<AdminCatalogosModel> ObtieneListaCatalogos();
        public List<AdminCatalogosModel> BusquedaPorCatalogo(int? idCatalogo, int? idDependencia);


    }
}
