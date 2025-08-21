using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.IO;

namespace GuanajuatoAdminUsuarios.Interfaces
{
	public interface IEstadisticasAccidentesService
	{
		public List<InfraccionesModel> GetAllInfracciones2();
		public List<BusquedaAccidentesModel> ObtenerAccidentes();
		//public List<ListadoAccidentesPorAccidenteModel> AccidentesPorAccidente();
		public IEnumerable<ListadoAccidentesPorAccidenteModel> AccidentesPorAccidente(BusquedaAccidentesModel model,Pagination pagination);

		public List<CatalogModel> GetMunicipiosFilter();
		List<CatalogModel> GetCarreterasFilter();
		List<CatalogModel> GetDelegacionesFilter();

		List<CatalogModel> GetTramosFilter();
        
                    public Stream AccidentesPorVehiculoExcel(BusquedaAccidentesModel model); 
 public Stream AccidentesPorAccidenteExcel(BusquedaAccidentesModel model);
		public IEnumerable<ListadoAccidentesPorVehiculoModel> AccidentesPorVehiculo(BusquedaAccidentesModel model, Pagination pagination);




		//public List<ListadoAccidentesPorVehiculoModel> AccidentesPorVehiculo();
	}
}
