using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IBusquedaAccidentesService
    {
        List<BusquedaAccidentesModel> BusquedaAccidentes(BusquedaAccidentesModel model, int idOficina);
       List<BusquedaAccidentesModel> GetAllAccidentes();
        public IEnumerable<BusquedaAccidentesModel> GetAllAccidentesPagination(Pagination pagination, BusquedaAccidentesModel model, int idOficina);
        public IEnumerable<BusquedaAccidentesModel> GetAllAccidentesPaginationWhitcode(Pagination pagination, BusquedaAccidentesModel model, int IdOficina);

        List<BusquedaAccidentesPDFModel> BusquedaAccidentes(BusquedaAccidentesPDFModel model, int idOficina);
        public BusquedaAccidentesPDFModel ObtenerAccidentePorId(int idAccidente);
        List<BusquedaAccidentesModel> ObtenerAccidentes(BusquedaAccidentesModel model);
    }
}
