using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
        public interface IVehiculosService
        {
                public IEnumerable<VehiculoModel> GetAllVehiculos();
                public IEnumerable<VehiculoModel> GetAllVehiculosPagination(Pagination pagination);
                public IEnumerable<VehiculoModel> GetAllVehiculosPaginationByFilter(Pagination pagination, VehiculoBusquedaModel filtro); 
                List<VehiculoModel> GetVehiculos(VehiculoBusquedaModel modelSearch);
                List<VehiculoModel> GetVehiculosPagination(VehiculoBusquedaModel modelSearch, Pagination pagination);
                public VehiculoModel GetVehiculoById(int idVehiculo);
                public VehiculoModel GetVehiculoByIdHistorico(int idVehiculo, int idinfraccion ,int op);
                public int UpdateFromEditVehiculo(VehiculoEditViewModel data);

                public VehiculoModel GetVehiculoHistoricoByIdAndIdinfraccion(int idVehiculo, int idInfraccion);
                public VehiculoModel GetVehiculoByIdSubMArcaAll(int idVehiculo);
                int BuscarPorParametro(string Placa, string Serie, string Folio);
                int BuscarvehiculoSerie(string Serie);
                public VehiculoModel GetVehiculoToAnexo(VehiculoBusquedaModel model);
                public int CreateVehiculo(VehiculoModel model);
                int UpdateVehiculo2(VehiculoModel model);
                public int UpdateVehiculo(VehiculoModel model);
                public int UpdatePropietario(int idPersona, int idVehiculo);

                VehiculoModel GetVehiculoId(string id);
                VehiculoModel GetVehiculoIdHistorico(int id ,int idInfraccion);
                VehiculoModel GetVehiculoIdHistoricoAccidente(int id, int idAccidente);
                
                public List<VehiculoModel> GetVehiculoPropietario(VehiculoBusquedaModel model);

                public int CrearHistoricoVehiculo(int idEvento, int idVehiculo, int tipoEvento);
        }
}
