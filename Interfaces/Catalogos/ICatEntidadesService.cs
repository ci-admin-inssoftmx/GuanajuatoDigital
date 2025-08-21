using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatEntidadesService
    {

		List<CatEntidadesModel> ObtenerEntidadesActivas(int corp);
		List<CatEntidadesModel> ObtenerEntidades(int corp);
        public CatEntidadesModel ObtenerEntidadesByID(int idEntidad);
        public int CrearEntidad(CatEntidadesModel model,int corp);
        public int EditarEntidad(CatEntidadesModel model);
        public CatEntidadesModel ObtenerEntidadesByNombre(string nombre,int corp);
        public int obtenerIdPorEntidad(string entidad, int corp);
        


    }
}
