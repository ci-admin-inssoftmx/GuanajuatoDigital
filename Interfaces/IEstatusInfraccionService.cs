using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IEstatusInfraccionService
    {
        List<EstatusInfraccionModel> GetEstatusInfracciones();        
        List<EstatusInfraccionModel> GetEstatusInvisibleInfracciones();
        int GetEstatusId(string estatusName);
    }
}
