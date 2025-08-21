using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IReporteAsignacionService
    {
        List<ReporteAsignacionModel> GetAllReporteAsignaciones(int idOficina);
        List<ReporteAsignacionModel> GetAllReporteAsignaciones(ReporteAsignacionBusquedaModel model,int idOficina);
    }
}
