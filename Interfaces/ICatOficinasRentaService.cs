using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatOficinasRentaService
    {
        List<CatOficinasRentaModel> ObtenerOficinasActivas();

    }
}
