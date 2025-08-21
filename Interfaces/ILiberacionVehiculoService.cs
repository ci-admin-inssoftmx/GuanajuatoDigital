using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ILiberacionVehiculoService
    {
        List<LiberacionVehiculoModel> GetAllTopDepositos(int idOficina);
        List<LiberacionVehiculoModel> GetDepositos(LiberacionVehiculoBusquedaModel model, int idOficina);
        LiberacionVehiculoModel GetDepositoByID(int Id, int idOficina);
        int UpdateDeposito(LiberacionVehiculoModel model);
        LiberacionVehiculoModel GetDepositoRutaFormatoManual(int Id);
    }
}
