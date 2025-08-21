using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IComparativoInfraccionesService
    {
        public List<ResultadoGeneral> BusquedaResultadosGenerales(ComparativoInfraccionesModel modelBusqueda);
        public List<DetallePorCausa> BusquedaDetallesPorCausas(ComparativoInfraccionesModel modelBusqueda);
        public List<DesgloseTotalInfraccion> DesgloseTotalesInfracciones(ComparativoInfraccionesModel modelBusqueda);
    }
}
