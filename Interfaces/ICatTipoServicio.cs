using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatTipoServicio
    {
        List<CatTipoServicioModel> ObtenerTiposActivos();
        public CatTipoServicioModel ObtenerTipoByID(int idTipoServicio);
        public int CrearTipo(CatTipoServicioModel model);
        public int EditarTipo(CatTipoServicioModel model);
    }
}
