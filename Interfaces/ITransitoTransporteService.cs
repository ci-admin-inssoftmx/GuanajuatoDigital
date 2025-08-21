using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ITransitoTransporteService
    {
        List<TransitoTransporteModel> GetAllTransitoTransporte(int idOficina);
        List<TransitoTransporteModel> GetTransitoTransportes(TransitoTransporteBusquedaModel model, Pagination pagination, int idOficina);
        TransitoTransporteModel GetTransitoTransporteById(int IdDeposito);       
        List<Pensiones> GetPensiones();
        int DeleteTransitoTransporte(int IdDeposito, int IdSolicitud);

    }
}
