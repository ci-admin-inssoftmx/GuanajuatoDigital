using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatCiudadesService
    {
        List<CatCiudadesModel> ObtenerCiudadesActivas();

    }
}
