using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IColores
    {
        public int obtenerIdPorColor(string colorLimpio);
        List<ColoresModel>ObtenerColoresActivosPorCorp(int corp);


    }
}
