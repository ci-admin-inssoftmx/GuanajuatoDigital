using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatalogosService
    {
        public List<Dictionary<string, string>> GetGenericCatalogos(string tabla, string[] campos);
        public List<Dictionary<string, string>> GetGenericCatalogosByFilter(string tabla, string[] campos, string campoFiltro, int idFiltro);
        public List<Dictionary<string, string>> GetGenericCatalogosByFilters(string tabla, string[] campos, string campoFiltro, int idFiltro, string campoFiltro2, int idFiltro2);
        public List<Dictionary<string, string>> GetGenericCatalogosByFilter(string tabla, string[] campos, string[] campoFiltro, int[] idFiltro);
        public int UpdateRelaciones(RelacionModel model);

    }
}
