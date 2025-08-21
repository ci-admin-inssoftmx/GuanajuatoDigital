using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatSubtipoServicio
    {
        List<CatSubtipoServicioModel> GetSubtipoPorTipo(int tipoServicioDDlValue);
        List<CatSubtipoServicioModel> ObtenerSubtiposActivos();
        public CatSubtipoServicioModel ObtenerSubtipoByID(int idSubtipoServicio);
        public int CrearSubtipo(CatSubtipoServicioModel model);
        public int EditarSubtipo(CatSubtipoServicioModel model);
    }
}
