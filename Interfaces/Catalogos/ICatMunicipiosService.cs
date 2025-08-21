using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatMunicipiosService
    {
        List<CatMunicipiosModel> GetMunicipios(int corp);
        List<CatMunicipiosModel> GetMunicipiosCatalogo(int corp);
		List<CatMunicipiosModel> GetMunicipiosActivePorCorporacion(int corp);

		List<CatMunicipiosModel> GetMunicipiosCatalogoFiltro(string nombre, int idEntidad, int idDelegacion, int corp);

        List<CatMunicipiosModel> GetMunicipios2(int corp);
        List<CatMunicipiosModel> GetMunicipios3(int corp);
        List<CatMunicipiosModel> GetMunicipiosPorEntidad(int entidadDDlValue, int corp);

        List<CatMunicipiosModel> GetMunicipiosActivePorEntidad(int entidadDDlValue, int corp);

        List<CatMunicipiosModel> GetMunicipiosPorDelegacion(int idOficina, int corp);
        List<CatMunicipiosModel> GetMunicipiosPorDelegacionActivos(int idOficina, int corp);
        List<CatMunicipiosModel> GetMunicipiosPorDelegacionTodos(int idOficina, int corp);

        List<CatMunicipiosModel> GetMunicipiosPorDelegacion2(int idOficina, int corp);
        List<CatMunicipiosModel> GetMunicipiosPorDelegacion2(int corp);


        List<CatMunicipiosModel> GetMunicipiosGuanajuato(int corp);
        List<CatMunicipiosModel> GetMunicipiosGuanajuatoActivos(int corp);


        public CatMunicipiosModel GetMunicipioByID(int IdMunicipio);
        public int AgregarMunicipio(CatMunicipiosModel model, int corp);
        public int EditarMunicipio(CatMunicipiosModel model);
        public int obtenerIdPorNombre(string municipio, int corp);
        public CatMunicipiosModel ObtenerMunicipiosByNombre(string nombre, int corp);

    }
}
