using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ITiposCarga
    {
        List<TiposCargaModel> GetTiposCarga();
        List<TiposCargaModel> GetTiposCargaActivos();

    }
}
