using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IPadronDepositosGruasService
    {

        List<PadronDepositosGruasModel> GetAllPadronDepositosGruas(int idOficina);
        List<PadronDepositosGruasModel> GetPadronDepositosGruas(PadronDepositosGruasBusquedaModel model, int idOficina);
        List<PensionModel> GetPensiones(int idOficina);
        List<PensionModel> GetPensionesNoFilter();
    }
}
