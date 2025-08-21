using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IDelegacionesService
    {
        List<Delegaciones> GetDelegaciones();
        string getAbreviaturaMunicipio(int idDelegacion);
        CatalogModel GetExtensions();

	}
}
