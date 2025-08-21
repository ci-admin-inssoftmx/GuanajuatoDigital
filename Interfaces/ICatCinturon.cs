using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatCinturon
    {
        List<CatCinturonModel> ObtenerCinturon();
        List<CatCascoModel> ObtenerCasco();

    }
}
