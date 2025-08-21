using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IIngresarVehiculosService
    {
        public List<IngresoVehiculosModel> ObtenerDepositos(IngresoVehiculosModel model, int idPension);
        // public int GuardarDeposito(int idDeposito);
        public IngresoVehiculosModel DetallesDeposito(int idDeposito);
        public int GuardarFechaIngreso(DatosIngresoModel model);
        public int GuardarDepositoOtraDependencia(SolicitudDepositoOtraDependenciaModel model, int idOficina, int idPension);

        
    }
}
