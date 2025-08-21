using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using GuanajuatoAdminUsuarios.Entity;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ISalidaVehiculosService
    {
        public List<SalidaVehiculosModel> ObtenerIngresos(SalidaVehiculosModel model, int idPension);
        public SalidaVehiculosModel DetallesDeposito(int iDp, int idPension);
        public SalidaVehiculosModel DetallesDepositoOtraDep(int iDp, int idPension);

        List<GruasSalidaVehiculosModel> ObtenerDatosGridGruas(int iDp);
        public CostosServicioModel CostosServicio(int idDeposito, int idGrua);

        public int ActualizarCostos(CostosServicioModel model); 
        public int GuardarInforSalida(SalidaVehiculosModel model);
        public int GuardarInforSalidaOtrasDep(SalidaVehiculosModel model);

        public List<SalidaVehiculosModel> ObtenerTotal(int iDp);

        public List<MarcasVehiculo> GetMarcasSalidaPension(int idPension);

    }
}
