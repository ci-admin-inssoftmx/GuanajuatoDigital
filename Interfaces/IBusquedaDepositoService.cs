using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IBusquedaDepositoService
    {
        public List<BusquedaDepositoModel> ObtenerTodosDepositos(int idPension);

        public List<BusquedaDepositoModel> ObtenerDepositos(BusquedaDepositoModel model, int idPension);
        public BusquedaDepositoModel ObtenerDetalles(int idDeposito);

    }
}

