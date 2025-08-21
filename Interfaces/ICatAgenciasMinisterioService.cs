using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatAgenciasMinisterioService
    {
        List<CatAgenciasMinisterioModel> ObtenerAgenciasActivas(int corp);
        List<CatAgenciasMinisterioModel> ObtenerAgencias(int corp);

		List<CatAgenciasMinisterioModel> ObtenerAgenciasActivasPorDelegacion(int idOficina, int corp);

        

    }
}
