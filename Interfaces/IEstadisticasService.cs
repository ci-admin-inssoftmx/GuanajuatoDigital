using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IEstadisticasService 
    {
        public List<EstadisticaInfraccionMotivosModel> GetAllInfraccionesEstadisticas(IncidenciasBusquedaModel modelBusqueda,int idDependencia);       
        // public List<EstadisticaInfraccionMotivosModel> GetAllMotivosPorInfraccionBusqueda(IncidenciasBusquedaModel modelBusqueda, int idDependencia);

    }
}
