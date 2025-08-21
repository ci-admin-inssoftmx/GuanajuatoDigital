using ClosedXML.Excel;
using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Utils;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Controllers
{
	[Authorize]
	public class EstadisticasAccidentesController : BaseController
	{
		private readonly IEstatusInfraccionService _estatusInfraccionService;
		private readonly ITipoCortesiaService _tipoCortesiaService;
		private readonly IDependencias _dependeciaService;
		private readonly IDelegacionesService _delegacionesService;
		private readonly IGarantiasService _garantiasService;
		private readonly IInfraccionesService _infraccionesService;
		private readonly IPdfGenerator _pdfService;
		private readonly ICatDictionary _catDictionary;
		private readonly IVehiculosService _vehiculosService;
		private readonly IPersonasService _personasService;
		private readonly IBitacoraService _bitacoraServices;
		private readonly IEstadisticasAccidentesService _estadisticasAccidentesService;
		private readonly ICatTramosService _catTramosService;
		private string resultValue = string.Empty;
		public static BusquedaAccidentesModel vehModel = new BusquedaAccidentesModel();
		public static BusquedaAccidentesModel porVehiculoModel;
        private readonly IBusquedaAccidentesService _busquedaAccidentesService;

        public EstadisticasAccidentesController(
			IEstatusInfraccionService estatusInfraccionService, IDelegacionesService delegacionesService,
			ITipoCortesiaService tipoCortesiaService, IDependencias dependeciaService, IGarantiasService garantiasService,
			IInfraccionesService infraccionesService, IPdfGenerator pdfService,
			ICatDictionary catDictionary,
			IVehiculosService vehiculosService,
			IPersonasService personasService,
			IEstadisticasAccidentesService estadisticasAccidentesService,
			ICatTramosService catTramosService,
            IBusquedaAccidentesService busquedaAccidentesService,
			IBitacoraService bitacoraServices


		   )
		{
			_catDictionary = catDictionary;
			_estatusInfraccionService = estatusInfraccionService;
			_tipoCortesiaService = tipoCortesiaService;
			_dependeciaService = dependeciaService;
			_delegacionesService = delegacionesService;
			_garantiasService = garantiasService;
			_infraccionesService = infraccionesService;
			_pdfService = pdfService;
			_vehiculosService = vehiculosService;
			_personasService = personasService;
			_estadisticasAccidentesService = estadisticasAccidentesService;
			_catTramosService = catTramosService;
			_busquedaAccidentesService = busquedaAccidentesService;
			_bitacoraServices = bitacoraServices;


		}
		public IActionResult Index()
        {
            porVehiculoModel = new BusquedaAccidentesModel();

            var corp = HttpContext.Session.GetInt32("IdDependencia");
			// var modelList = _infraccionesService.GetAllAccidentes2()
			//.GroupBy(g => g.municipio)
			// .Select(s => new EstadisticaAccidentesMotivosModel() { Motivo = s.Key.ToString(), Contador = s.Count() }).ToList();

			var catMotivosInfraccion = _catDictionary.GetCatalog("CatAllMotivosInfraccion", "0");
			var catTipoServicio = _catDictionary.GetCatalog("CatTipoServicio", "0");
			var catTiposVehiculo = _catDictionary.GetCatalog("CatTiposVehiculo", "0");
			var catOficiales = _catDictionary.GetCatalog("CatOficiales", "0");


			var catMunicipios = _estadisticasAccidentesService.GetMunicipiosFilter(); //_catDictionary.GetCatalog("CatMunicipios", "0");
			var catCarreteras = _estadisticasAccidentesService.GetCarreterasFilter();
			var catTramos = _catDictionary.GetCatalog("CatTramos", "0");
			var catDelegaciones = _estadisticasAccidentesService.GetDelegacionesFilter();





			var catGarantias = _catDictionary.GetCatalog("CatGarantias", "0");
			var catTipoLicencia = _catDictionary.GetCatalog("CatTipoLicencia", "0");
			var catTipoPlaca = _catDictionary.GetCatalog("CatTipoPlaca", "0");
			var catClasificacionAccidentes = _catDictionary.GetCatalog("CatClasificacionAccidentes", "0");
			var catCausasAccidentes = _catDictionary.GetCatalog("CatCausasAccidentes", "0");
			var catFactoresAccidentes = _catDictionary.GetCatalog("CatFactoresAccidentes", "0");
			var catFactoresOpcionesAccidentes = _catDictionary.GetCatalog("CatFactoresOpcionesAccidentes", "0");
			var catSubtipoServicio = _catDictionary.GetCatalog("CatSubtipoServicioFilter", "0");

			ViewBag.CatMotivosInfraccion = new SelectList(catMotivosInfraccion.CatalogList, "Id", "Text");
			ViewBag.CatTipoServicio = new SelectList(catTipoServicio.CatalogList, "Id", "Text");
			ViewBag.CatTiposVehiculo = new SelectList(catTiposVehiculo.CatalogList, "Id", "Text");
			ViewBag.CatDelegaciones = new SelectList(catDelegaciones, "value", "text");
			ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
			ViewBag.CatTipoPlaca = new SelectList(catTipoPlaca.CatalogList, "Id", "Text");
			ViewBag.CatOficiales = new SelectList(catOficiales.CatalogList, "Id", "Text");

			ViewBag.CatMunicipios = new SelectList(catMunicipios, "value", "text");


			ViewBag.CatCarreteras = new SelectList(catCarreteras, "value", "text");
			ViewBag.CatTramos = new SelectList(catTramos.CatalogList, "value", "text");


			ViewBag.CatGarantias = new SelectList(catGarantias.CatalogList, "Id", "Text");
			ViewBag.CatClasificacionAccidentes = new SelectList(catClasificacionAccidentes.CatalogList, "Id", "Text");
			ViewBag.CatCausasAccidentes = new SelectList(catCausasAccidentes.CatalogList, "Id", "Text");
			ViewBag.CatFactoresAccidentes = new SelectList(catFactoresAccidentes.CatalogList, "Id", "Text");
			ViewBag.CatFactoresOpcionesAccidentes = new SelectList(catFactoresOpcionesAccidentes.CatalogList, "Id", "Text");
			ViewBag.CatSubtipoServicio = new SelectList(catSubtipoServicio.CatalogList, "Id", "Text");

			// ViewBag.Estadisticas = modelList;
			// ViewBag.ListadoAccidentesPorAccidente = _estadisticasAccidentesService.AccidentesPorAccidente();
			//ViewBag.ListadoAccidentesPorVehiculo = _estadisticasAccidentesService.AccidentesPorVehiculo();

			return View();
		}

		public IActionResult ajax_BusquedaAccidentes(BusquedaAccidentesModel model)
		{
			//int idOficina = (int)HttpContext.Session.GetInt32("IdOficina");

			var modelList = _estadisticasAccidentesService.ObtenerAccidentes();

			modelList = modelList.Where(w => w.idMunicipio == (model.idMunicipio > 0 ? model.idMunicipio : w.idMunicipio)
															   && w.idDelegacion == (model.idDelegacion > 0 ? model.idDelegacion : w.idDelegacion)
															   && w.IdOficial == (model.IdOficial > 0 ? model.IdOficial : w.IdOficial)
															   && w.idCarretera == (model.idCarretera > 0 ? model.idCarretera : w.idCarretera)
															   && w.idTramo == (model.idTramo > 0 ? model.idTramo : w.idTramo)
															   && w.idClasificacionAccidente == (model.idClasificacionAccidente > 0 ? model.idClasificacionAccidente : w.idClasificacionAccidente)
															   && w.idCausaAccidente == (model.idCausaAccidente > 0 ? model.idCausaAccidente : w.idCausaAccidente)
															   && w.idFactorAccidente == (model.idFactorAccidente > 0 ? model.idFactorAccidente : w.idFactorAccidente)
															   && w.IdTipoVehiculo == (model.IdTipoVehiculo > 0 ? model.IdTipoVehiculo : w.IdTipoVehiculo)
															   && w.IdSubtipoServicio == (model.IdSubtipoServicio > 0 ? model.IdSubtipoServicio : w.IdSubtipoServicio)
															   && w.IdTipoServicio == (model.IdTipoServicio > 0 ? model.IdTipoServicio : w.IdTipoServicio)
															   && w.idFactorOpcionAccidente == (model.idFactorOpcionAccidente > 0 ? model.idFactorOpcionAccidente : w.idFactorOpcionAccidente)
															   && ((model.FechaInicio == null && model.FechaFin == null) ||
																   (w.fecha >= (model.FechaInicio ?? DateTime.MinValue) &&
																	w.fecha <= (model.FechaFin ?? DateTime.MaxValue)))
																 &&(model.hora == TimeSpan.Zero && w.hora != TimeSpan.Zero) ||
																	(model.hora != TimeSpan.Zero && model.hora == w.hora)).ToList();

			var lista = modelList.GroupBy(g => g.municipio).ToList();

			var lista2 = lista.
				Select(
					s => new EstadisticaAccidentesMotivosModel()
					{
						Motivo = s.Key.ToString(),
						Delegacion = s.First().Delegacion,
						Contador = s.Count()

					}
					).ToList();
			//BITACORA
			_bitacoraServices.BitacoraAccidentes(0, CodigosAccidente.C2055, "");
			return PartialView("_EstadisticasAccidentes", lista2);
		}

		public IActionResult GetBuscaEsadisticasPorVehiculo([DataSourceRequest] DataSourceRequest request, BusquedaAccidentesModel model)
		{
			porVehiculoModel = model;
			return PartialView("_ListadoAccidentesPorVehiculo", new List<ListadoAccidentesPorVehiculoModel>());
		}

		public IActionResult GetBuscaEsadisticasPorAccidente([DataSourceRequest] DataSourceRequest request, BusquedaAccidentesModel model)
		{
			porVehiculoModel = model;
			return PartialView("_ListadoAccidentesPorAccidente", new List<ListadoAccidentesPorAccidenteModel>());
		}

		public IActionResult ajax_BusquedaParaTablas(BusquedaAccidentesModel model, [DataSourceRequest] DataSourceRequest request)
		  {
			filterValue(request.Filters);
			Pagination pagination = new Pagination();
			pagination.PageIndex = request.Page;
			pagination.PageSize = (request.PageSize != 0) ? request.PageSize : 10;
			pagination.Filter = resultValue;
            var t = request.Sorts;
            if (t.Count() > 0)
            {
                var q = t[0];
                pagination.Sort = q.SortDirection.ToString().Substring(0, 3);
                pagination.SortCamp = q.Member;
            }

            if (porVehiculoModel == null)
				porVehiculoModel = model;

			//var estadisticasPorVehiculo = _estadisticasAccidentesService.AccidentesPorVehiculo(porVehiculoModel, pagination);

			var estadisticasPorAccidente = _estadisticasAccidentesService.AccidentesPorAccidente(porVehiculoModel, pagination);

			request.PageSize = pagination.PageSize;

			var total = 0;
			
			if (estadisticasPorAccidente.Count() > 0)
				total = estadisticasPorAccidente.ToList().FirstOrDefault().Total;

			var result = new
			{
				Data = estadisticasPorAccidente,

				Total = total
			};

			return Json(result);
		}


		public IActionResult ajax_BusquedaParaTablas2(BusquedaAccidentesModel model, [DataSourceRequest] DataSourceRequest request)
		{
			filterValue(request.Filters);
			Pagination pagination = new Pagination();
			pagination.PageIndex = request.Page;
			pagination.PageSize = (request.PageSize != 0) ? request.PageSize : 10;
			pagination.Filter = resultValue;
			var t = request.Sorts;
			if (t.Count() > 0)
			{
				var q = t[0];
				pagination.Sort = q.SortDirection.ToString().Substring(0, 3);
				pagination.SortCamp = q.Member;
			}

			if (porVehiculoModel == null)
				porVehiculoModel = model;

			var estadisticasPorVehiculo = _estadisticasAccidentesService.AccidentesPorVehiculo(porVehiculoModel, pagination);


			request.PageSize = pagination.PageSize;

			var total = 0;

			if (estadisticasPorVehiculo.Count() > 0)
				total = estadisticasPorVehiculo.ToList().FirstOrDefault().Total;

			var result = new
			{
				Data = estadisticasPorVehiculo,

				Total = total
			};

			return Json(result);
		}

		public IActionResult GenerarExcel(BusquedaAccidentesModel model, string Reporte)
		{
			

			if (Reporte == "PorVehiculo")
			{
                Pagination pagination = new Pagination();
                pagination.PageIndex = 1;
                pagination.PageSize = 200000;
                pagination.Filter = resultValue;

                if (porVehiculoModel == null)
                    porVehiculoModel = model;

                var estadisticasPorVehiculo = _estadisticasAccidentesService.AccidentesPorVehiculo(porVehiculoModel, pagination);

                List<ListadoAccidentesPorVehiculoModel> modelList = new List<ListadoAccidentesPorVehiculoModel>();




                DataTable dataTable = new DataTable("Listado Accidentes por Vehículo");
                dataTable.Columns.AddRange(new DataColumn[]
                {
                new DataColumn("No"),
                new DataColumn("Num Reporte"),
				new DataColumn("Fecha"),
				new DataColumn("Hora"),
				
				new DataColumn("Depósito"),
				new DataColumn("Delegación"),
				new DataColumn("Municipio"),
				new DataColumn("Carretera"),
				new DataColumn("Tramo"),
				new DataColumn("Kilómetro"),
				new DataColumn("Latitud"),
				new DataColumn("Longitud"),
				new DataColumn("Nombre de oficial"),
				new DataColumn("Daños al camino"),
				new DataColumn("Daños a la carga"),
				new DataColumn("Daños a propietarios"),
				new DataColumn("Otros daños"),
				new DataColumn("Lesionados"),
				new DataColumn("Muertos"),
				new DataColumn("Causas"),
				new DataColumn("Descripción causas"),


                new DataColumn("Num vehículo [1]"),
                new DataColumn("Placas [1]"),
                new DataColumn("Serie [1]"),
                new DataColumn("Propietario [1]"),
                new DataColumn("Tipo vehículo [1]"),
                new DataColumn("Servicio [1]"),
                new DataColumn("Marca [1]"),
                new DataColumn("Submarca [1]"),
                new DataColumn("Modelo [1]"),
                new DataColumn("Conductor [1]"),

                new DataColumn("Num vehículo [2]"),
                new DataColumn("Placas [2]"),
                new DataColumn("Serie [2]"),
                new DataColumn("Propietario [2]"),
                new DataColumn("Tipo vehículo [2]"),
                new DataColumn("Servicio [2]"),
                new DataColumn("Marca [2]"),
                new DataColumn("Submarca [2]"),
                new DataColumn("Modelo [2]"),
                new DataColumn("Conductor [2]"),

                new DataColumn("Num vehículo [3]"),
                new DataColumn("Placas [3]"),
                new DataColumn("Serie [3]"),
                new DataColumn("Propietario [3]"),
                new DataColumn("Tipo vehículo [3]"),
                new DataColumn("Servicio [3]"),
                new DataColumn("Marca [3]"),
                new DataColumn("Submarca [3]"),
                new DataColumn("Modelo [3]"),
                new DataColumn("Conductor [3]"),

                new DataColumn("Num vehículo [4]"),
                new DataColumn("Placas [4]"),
                new DataColumn("Serie [4]"),
                new DataColumn("Propietario [4]"),
                new DataColumn("Tipo vehículo [4]"),
                new DataColumn("Servicio [4]"),
                new DataColumn("Marca [4]"),
                new DataColumn("Submarca [4]"),
                new DataColumn("Modelo [4]"),
                new DataColumn("Conductor [4]"),

                new DataColumn("Num vehículo [5]"),
                new DataColumn("Placas [5]"),
                new DataColumn("Serie [5]"),
                new DataColumn("Propietario [5]"),
                new DataColumn("Tipo vehículo [5]"),
                new DataColumn("Servicio [5]"),
                new DataColumn("Marca [5]"),
                new DataColumn("Submarca [5]"),
                new DataColumn("Modelo [5]"),
                new DataColumn("Conductor [5]"),

                new DataColumn("Num vehículo [6]"),
                new DataColumn("Placas [6]"),
                new DataColumn("Serie [6]"),
                new DataColumn("Propietario [6]"),
                new DataColumn("Tipo vehículo [6]"),
                new DataColumn("Servicio [6]"),
                new DataColumn("Marca [6]"),
                new DataColumn("Submarca [6]"),
                new DataColumn("Modelo [6]"),
                new DataColumn("Conductor [6]"),

                new DataColumn("Num vehículo [7]"),
                new DataColumn("Placas [7]"),
                new DataColumn("Serie [7]"),
                new DataColumn("Propietario [7]"),
                new DataColumn("Tipo vehículo [7]"),
                new DataColumn("Servicio [7]"),
                new DataColumn("Marca [7]"),
                new DataColumn("Submarca [7]"),
                new DataColumn("Modelo [7]"),
                new DataColumn("Conductor [7]"),

                new DataColumn("Num vehículo [8]"),
                new DataColumn("Placas [8]"),
                new DataColumn("Serie [8]"),
                new DataColumn("Propietario [8]"),
                new DataColumn("Tipo vehículo [8]"),
                new DataColumn("Servicio [8]"),
                new DataColumn("Marca [8]"),
                new DataColumn("Submarca [8]"),
                new DataColumn("Modelo [8]"),
                new DataColumn("Conductor [8]"),

                new DataColumn("Num vehículo [9]"),
                new DataColumn("Placas [9]"),
                new DataColumn("Serie [9]"),
                new DataColumn("Propietario [9]"),
                new DataColumn("Tipo vehículo [9]"),
                new DataColumn("Servicio [9]"),
                new DataColumn("Marca [9]"),
                new DataColumn("Submarca [9]"),
                new DataColumn("Modelo [9]"),
                new DataColumn("Conductor [9]"),

                new DataColumn("Num vehículo [10]"),
                new DataColumn("Placas [10]"),
                new DataColumn("Serie [10]"),
                new DataColumn("Propietario [10]"),
                new DataColumn("Tipo vehículo [10]"),
                new DataColumn("Servicio [10]"),
                new DataColumn("Marca [10]"),
                new DataColumn("Submarca [10]"),
                new DataColumn("Modelo [10]"),
                new DataColumn("Conductor [10]"),

                new DataColumn("Num vehículo [11]"),
                new DataColumn("Placas [11]"),
                new DataColumn("Serie [11]"),
                new DataColumn("Propietario [11]"),
                new DataColumn("Tipo vehículo [11]"),
                new DataColumn("Servicio [11]"),
                new DataColumn("Marca [11]"),
                new DataColumn("Submarca [11]"),
                new DataColumn("Modelo [11]"),
                new DataColumn("Conductor [11]"),

                new DataColumn("Num vehículo [12]"),
                new DataColumn("Placas [12]"),
                new DataColumn("Serie [12]"),
                new DataColumn("Propietario [12]"),
                new DataColumn("Tipo vehículo [12]"),
                new DataColumn("Servicio [12]"),
                new DataColumn("Marca [12]"),
                new DataColumn("Submarca [12]"),
                new DataColumn("Modelo [12]"),
                new DataColumn("Conductor [12]"),

                new DataColumn("Num vehículo [13]"),
                new DataColumn("Placas [13]"),
                new DataColumn("Serie [13]"),
                new DataColumn("Propietario [13]"),
                new DataColumn("Tipo vehículo [13]"),
                new DataColumn("Servicio [13]"),
                new DataColumn("Marca [13]"),
                new DataColumn("Submarca [13]"),
                new DataColumn("Modelo [13]"),
                new DataColumn("Conductor [13]"),


                new DataColumn("Num vehículo [14]"),
                new DataColumn("Placas [14]"),
                new DataColumn("Serie [14]"),
                new DataColumn("Propietario [14]"),
                new DataColumn("Tipo vehículo [14]"),
                new DataColumn("Servicio [14]"),
                new DataColumn("Marca [14]"),
                new DataColumn("Submarca [14]"),
                new DataColumn("Modelo [14]"),
                new DataColumn("Conductor [14]"),

                new DataColumn("Num vehículo [15]"),
                new DataColumn("Placas [15]"),
                new DataColumn("Serie [15]"),
                new DataColumn("Propietario [15]"),
                new DataColumn("Tipo vehículo [15]"),
                new DataColumn("Servicio [15]"),
                new DataColumn("Marca [15]"),
                new DataColumn("Submarca [15]"),
                new DataColumn("Modelo [15]"),
                new DataColumn("Conductor [15]")


                });

				var folio = "";
                ListadoAccidentesPorVehiculoModel accidentesPorVehiculoModel = new ListadoAccidentesPorVehiculoModel();
				int cont = 0;
				Boolean flagPrimeraVez = true;

                foreach (var row in estadisticasPorVehiculo)
                {
					if (row.Numreporteaccidente == folio)
					{
						cont++;

                        //set otro vehiculo
						if (cont == 1)
						{
                            accidentesPorVehiculoModel.NumVeh2 = row.NumVeh;
                            accidentesPorVehiculoModel.PlacasVeh2 = row.PlacasVeh;
                            accidentesPorVehiculoModel.SerieVeh2 = row.SerieVeh;
                            accidentesPorVehiculoModel.PropietarioVeh2 = row.PropietarioVeh;
                            accidentesPorVehiculoModel.TipoVeh2 = row.TipoVeh;
                            accidentesPorVehiculoModel.ServicioVeh2 = row.ServicioVeh;
                            accidentesPorVehiculoModel.Marca2 = row.Marca;
                            accidentesPorVehiculoModel.Submarca2 = row.Submarca;
                            accidentesPorVehiculoModel.Modelo2 = row.Modelo;
                            accidentesPorVehiculoModel.ConductorVeh2 = row.ConductorVeh;
                        }
                        if (cont == 2)
                        {
                            accidentesPorVehiculoModel.NumVeh3 = row.NumVeh;
                            accidentesPorVehiculoModel.PlacasVeh3 = row.PlacasVeh;
                            accidentesPorVehiculoModel.SerieVeh3 = row.SerieVeh;
                            accidentesPorVehiculoModel.PropietarioVeh3 = row.PropietarioVeh;
                            accidentesPorVehiculoModel.TipoVeh3 = row.TipoVeh;
                            accidentesPorVehiculoModel.ServicioVeh3 = row.ServicioVeh;
                            accidentesPorVehiculoModel.Marca3 = row.Marca;
                            accidentesPorVehiculoModel.Submarca3 = row.Submarca;
                            accidentesPorVehiculoModel.Modelo3 = row.Modelo;
                            accidentesPorVehiculoModel.ConductorVeh3 = row.ConductorVeh;
                        }
                        if (cont == 3)
                        {
                            accidentesPorVehiculoModel.NumVeh4 = row.NumVeh;
                            accidentesPorVehiculoModel.PlacasVeh4 = row.PlacasVeh;
                            accidentesPorVehiculoModel.SerieVeh4 = row.SerieVeh;
                            accidentesPorVehiculoModel.PropietarioVeh4 = row.PropietarioVeh;
                            accidentesPorVehiculoModel.TipoVeh4 = row.TipoVeh;
                            accidentesPorVehiculoModel.ServicioVeh4 = row.ServicioVeh;
                            accidentesPorVehiculoModel.Marca4 = row.Marca;
                            accidentesPorVehiculoModel.Submarca4 = row.Submarca;
                            accidentesPorVehiculoModel.Modelo4 = row.Modelo;
                            accidentesPorVehiculoModel.ConductorVeh4 = row.ConductorVeh;
                        }
                        if (cont == 4)
                        {
                            accidentesPorVehiculoModel.NumVeh5 = row.NumVeh;
                            accidentesPorVehiculoModel.PlacasVeh5 = row.PlacasVeh;
                            accidentesPorVehiculoModel.SerieVeh5 = row.SerieVeh;
                            accidentesPorVehiculoModel.PropietarioVeh5 = row.PropietarioVeh;
                            accidentesPorVehiculoModel.TipoVeh5 = row.TipoVeh;
                            accidentesPorVehiculoModel.ServicioVeh5 = row.ServicioVeh;
                            accidentesPorVehiculoModel.Marca5 = row.Marca;
                            accidentesPorVehiculoModel.Submarca5 = row.Submarca;
                            accidentesPorVehiculoModel.Modelo5 = row.Modelo;
                            accidentesPorVehiculoModel.ConductorVeh5 = row.ConductorVeh;
                        }
                        if (cont == 5)
                        {
                            accidentesPorVehiculoModel.NumVeh6 = row.NumVeh;
                            accidentesPorVehiculoModel.PlacasVeh6 = row.PlacasVeh;
                            accidentesPorVehiculoModel.SerieVeh6 = row.SerieVeh;
                            accidentesPorVehiculoModel.PropietarioVeh6 = row.PropietarioVeh;
                            accidentesPorVehiculoModel.TipoVeh6 = row.TipoVeh;
                            accidentesPorVehiculoModel.ServicioVeh6 = row.ServicioVeh;
                            accidentesPorVehiculoModel.Marca6 = row.Marca;
                            accidentesPorVehiculoModel.Submarca6 = row.Submarca;
                            accidentesPorVehiculoModel.Modelo6 = row.Modelo;
                            accidentesPorVehiculoModel.ConductorVeh6 = row.ConductorVeh;
                        }
                        if (cont == 6)
                        {
                            accidentesPorVehiculoModel.NumVeh7 = row.NumVeh;
                            accidentesPorVehiculoModel.PlacasVeh7 = row.PlacasVeh;
                            accidentesPorVehiculoModel.SerieVeh7 = row.SerieVeh;
                            accidentesPorVehiculoModel.PropietarioVeh7 = row.PropietarioVeh;
                            accidentesPorVehiculoModel.TipoVeh7 = row.TipoVeh;
                            accidentesPorVehiculoModel.ServicioVeh7 = row.ServicioVeh;
                            accidentesPorVehiculoModel.Marca7 = row.Marca;
                            accidentesPorVehiculoModel.Submarca7 = row.Submarca;
                            accidentesPorVehiculoModel.Modelo7 = row.Modelo;
                            accidentesPorVehiculoModel.ConductorVeh7 = row.ConductorVeh;
                        }
                        if (cont == 7)
                        {
                            accidentesPorVehiculoModel.NumVeh8 = row.NumVeh;
                            accidentesPorVehiculoModel.PlacasVeh8 = row.PlacasVeh;
                            accidentesPorVehiculoModel.SerieVeh8 = row.SerieVeh;
                            accidentesPorVehiculoModel.PropietarioVeh8 = row.PropietarioVeh;
                            accidentesPorVehiculoModel.TipoVeh8 = row.TipoVeh;
                            accidentesPorVehiculoModel.ServicioVeh8 = row.ServicioVeh;
                            accidentesPorVehiculoModel.Marca8 = row.Marca;
                            accidentesPorVehiculoModel.Submarca8 = row.Submarca;
                            accidentesPorVehiculoModel.Modelo8 = row.Modelo;
                            accidentesPorVehiculoModel.ConductorVeh8 = row.ConductorVeh;
                        }
                        if (cont == 8)
                        {
                            accidentesPorVehiculoModel.NumVeh9 = row.NumVeh;
                            accidentesPorVehiculoModel.PlacasVeh9 = row.PlacasVeh;
                            accidentesPorVehiculoModel.SerieVeh9 = row.SerieVeh;
                            accidentesPorVehiculoModel.PropietarioVeh9 = row.PropietarioVeh;
                            accidentesPorVehiculoModel.TipoVeh9 = row.TipoVeh;
                            accidentesPorVehiculoModel.ServicioVeh9 = row.ServicioVeh;
                            accidentesPorVehiculoModel.Marca9 = row.Marca;
                            accidentesPorVehiculoModel.Submarca9 = row.Submarca;
                            accidentesPorVehiculoModel.Modelo9 = row.Modelo;
                            accidentesPorVehiculoModel.ConductorVeh9 = row.ConductorVeh;
                        }
                        if (cont == 9)
                        {
                            accidentesPorVehiculoModel.NumVeh10 = row.NumVeh;
                            accidentesPorVehiculoModel.PlacasVeh10 = row.PlacasVeh;
                            accidentesPorVehiculoModel.SerieVeh10 = row.SerieVeh;
                            accidentesPorVehiculoModel.PropietarioVeh10 = row.PropietarioVeh;
                            accidentesPorVehiculoModel.TipoVeh10 = row.TipoVeh;
                            accidentesPorVehiculoModel.ServicioVeh10 = row.ServicioVeh;
                            accidentesPorVehiculoModel.Marca10 = row.Marca;
                            accidentesPorVehiculoModel.Submarca10 = row.Submarca;
                            accidentesPorVehiculoModel.Modelo10 = row.Modelo;
                            accidentesPorVehiculoModel.ConductorVeh10 = row.ConductorVeh;
                        }
                        if (cont == 10)
                        {
                            accidentesPorVehiculoModel.NumVeh11 = row.NumVeh;
                            accidentesPorVehiculoModel.PlacasVeh11 = row.PlacasVeh;
                            accidentesPorVehiculoModel.SerieVeh11 = row.SerieVeh;
                            accidentesPorVehiculoModel.PropietarioVeh11 = row.PropietarioVeh;
                            accidentesPorVehiculoModel.TipoVeh11 = row.TipoVeh;
                            accidentesPorVehiculoModel.ServicioVeh11 = row.ServicioVeh;
                            accidentesPorVehiculoModel.Marca11 = row.Marca;
                            accidentesPorVehiculoModel.Submarca11 = row.Submarca;
                            accidentesPorVehiculoModel.Modelo11 = row.Modelo;
                            accidentesPorVehiculoModel.ConductorVeh11 = row.ConductorVeh;
                        }
                        if (cont == 11)
                        {
                            accidentesPorVehiculoModel.NumVeh12 = row.NumVeh;
                            accidentesPorVehiculoModel.PlacasVeh12 = row.PlacasVeh;
                            accidentesPorVehiculoModel.SerieVeh12 = row.SerieVeh;
                            accidentesPorVehiculoModel.PropietarioVeh12 = row.PropietarioVeh;
                            accidentesPorVehiculoModel.TipoVeh12 = row.TipoVeh;
                            accidentesPorVehiculoModel.ServicioVeh12 = row.ServicioVeh;
                            accidentesPorVehiculoModel.Marca12 = row.Marca;
                            accidentesPorVehiculoModel.Submarca12 = row.Submarca;
                            accidentesPorVehiculoModel.Modelo12 = row.Modelo;
                            accidentesPorVehiculoModel.ConductorVeh12 = row.ConductorVeh;
                        }
                        if (cont == 12)
                        {
                            accidentesPorVehiculoModel.NumVeh13 = row.NumVeh;
                            accidentesPorVehiculoModel.PlacasVeh13 = row.PlacasVeh;
                            accidentesPorVehiculoModel.SerieVeh13 = row.SerieVeh;
                            accidentesPorVehiculoModel.PropietarioVeh13 = row.PropietarioVeh;
                            accidentesPorVehiculoModel.TipoVeh13 = row.TipoVeh;
                            accidentesPorVehiculoModel.ServicioVeh13 = row.ServicioVeh;
                            accidentesPorVehiculoModel.Marca13 = row.Marca;
                            accidentesPorVehiculoModel.Submarca13 = row.Submarca;
                            accidentesPorVehiculoModel.Modelo13 = row.Modelo;
                            accidentesPorVehiculoModel.ConductorVeh13 = row.ConductorVeh;
                        }
                        if (cont == 13)
                        {
                            accidentesPorVehiculoModel.NumVeh14 = row.NumVeh;
                            accidentesPorVehiculoModel.PlacasVeh14 = row.PlacasVeh;
                            accidentesPorVehiculoModel.SerieVeh14 = row.SerieVeh;
                            accidentesPorVehiculoModel.PropietarioVeh14 = row.PropietarioVeh;
                            accidentesPorVehiculoModel.TipoVeh14 = row.TipoVeh;
                            accidentesPorVehiculoModel.ServicioVeh14 = row.ServicioVeh;
                            accidentesPorVehiculoModel.Marca14 = row.Marca;
                            accidentesPorVehiculoModel.Submarca14 = row.Submarca;
                            accidentesPorVehiculoModel.Modelo14 = row.Modelo;
                            accidentesPorVehiculoModel.ConductorVeh14 = row.ConductorVeh;
                        }
                        if (cont == 14)
                        {
                            accidentesPorVehiculoModel.NumVeh15 = row.NumVeh;
                            accidentesPorVehiculoModel.PlacasVeh15 = row.PlacasVeh;
                            accidentesPorVehiculoModel.SerieVeh15 = row.SerieVeh;
                            accidentesPorVehiculoModel.PropietarioVeh15 = row.PropietarioVeh;
                            accidentesPorVehiculoModel.TipoVeh15 = row.TipoVeh;
                            accidentesPorVehiculoModel.ServicioVeh15 = row.ServicioVeh;
                            accidentesPorVehiculoModel.Marca15 = row.Marca;
                            accidentesPorVehiculoModel.Submarca15 = row.Submarca;
                            accidentesPorVehiculoModel.Modelo15 = row.Modelo;
                            accidentesPorVehiculoModel.ConductorVeh15 = row.ConductorVeh;
                        }

                    }
					else
					{
						
                            flagPrimeraVez = true;
                            modelList.Add(accidentesPorVehiculoModel);
                            cont = 0;
						

                        //nuevo modelo
                        folio = row.Numreporteaccidente;
                        accidentesPorVehiculoModel = new ListadoAccidentesPorVehiculoModel();

                        accidentesPorVehiculoModel.Numero = row.Numero;
						accidentesPorVehiculoModel.Numreporteaccidente = row.Numreporteaccidente;
						accidentesPorVehiculoModel.fecha = row.fecha;
						accidentesPorVehiculoModel.hora = row.hora;
                        /*accidentesPorVehiculoModel.NumVeh = row.NumVeh;
                        accidentesPorVehiculoModel.PlacasVeh = row.PlacasVeh;
                        accidentesPorVehiculoModel.SerieVeh = row.SerieVeh;
                        accidentesPorVehiculoModel.PropietarioVeh = row.PropietarioVeh;
                        accidentesPorVehiculoModel.TipoVeh = row.TipoVeh;
                        accidentesPorVehiculoModel.ServicioVeh = row.ServicioVeh;
                        accidentesPorVehiculoModel.Marca = row.Marca;
                        accidentesPorVehiculoModel.Submarca = row.Submarca;
                        accidentesPorVehiculoModel.Modelo = row.Modelo;
                        accidentesPorVehiculoModel.ConductorVeh = row.ConductorVeh;*/
                        accidentesPorVehiculoModel.DepositoVehículo = row.DepositoVehículo;
                        accidentesPorVehiculoModel.Delegacion = row.Delegacion;
                        accidentesPorVehiculoModel.Municipio = row.Municipio;
                        accidentesPorVehiculoModel.Carretera = row.Carretera;
                        accidentesPorVehiculoModel.Tramo = row.Tramo;
                        accidentesPorVehiculoModel.Kilómetro = row.Kilómetro;
                        accidentesPorVehiculoModel.Latitud = row.Latitud;
                        accidentesPorVehiculoModel.Longitud = row.Longitud;
                        accidentesPorVehiculoModel.NombredelOficial = row.NombredelOficial;
                        accidentesPorVehiculoModel.Dañosalcamino = row.Dañosalcamino;
                        accidentesPorVehiculoModel.DañosaCarga = row.DañosaCarga;
                        accidentesPorVehiculoModel.Dañosapropietario = row.Dañosapropietario;
                        accidentesPorVehiculoModel.Otrosdaños = row.Otrosdaños;
                        accidentesPorVehiculoModel.Lesionados = row.Lesionados;
                        accidentesPorVehiculoModel.Muertos = row.Muertos;
                        accidentesPorVehiculoModel.Causas = row.Causas;
                        accidentesPorVehiculoModel.CausasDescripcion = row.CausasDescripcion;

						if (flagPrimeraVez)
						{
							accidentesPorVehiculoModel.NumVeh1 = row.NumVeh;
							accidentesPorVehiculoModel.PlacasVeh1 = row.PlacasVeh;
							accidentesPorVehiculoModel.SerieVeh1 = row.SerieVeh;
							accidentesPorVehiculoModel.PropietarioVeh1 = row.PropietarioVeh;
							accidentesPorVehiculoModel.TipoVeh1 = row.TipoVeh;
							accidentesPorVehiculoModel.ServicioVeh1 = row.ServicioVeh;
							accidentesPorVehiculoModel.Marca1 = row.Marca;
							accidentesPorVehiculoModel.Submarca1 = row.Submarca;
							accidentesPorVehiculoModel.Modelo1 = row.Modelo;
							accidentesPorVehiculoModel.ConductorVeh1 = row.ConductorVeh;
						}

                    }

					flagPrimeraVez = false;


                }
				modelList.Add(accidentesPorVehiculoModel);//inserta el ultimo que quedo

                int contExcel = 0;
                modelList.RemoveAt(0);

                foreach (var row in modelList)
				{
                    contExcel++;
                    dataTable.Rows.Add(contExcel,
                            row.Numreporteaccidente,
                            row.fecha.ToString("dd/MM/yyyy"),
                            row.hora,                            
                            row.DepositoVehículo,
                            row.Delegacion,
                            row.Municipio,
                            row.Carretera,
                            row.Tramo,
                            row.Kilómetro,
                            row.Latitud,
                            row.Longitud,
                            row.NombredelOficial,
                            row.Dañosalcamino,
                            row.DañosaCarga,
                            row.Dañosapropietario,
                            row.Otrosdaños,
                            row.Lesionados,
                            row.Muertos,
                            row.Causas,
                            row.CausasDescripcion,
                            /*row.NumVeh,
                            row.PlacasVeh,
                            row.SerieVeh,
                            row.PropietarioVeh,
                            row.TipoVeh,
                            row.ServicioVeh,
                            row.Marca,
                            row.Submarca,
                            row.Modelo,
                            row.ConductorVeh,*/
                            row.NumVeh1,
							row.PlacasVeh1,
							row.SerieVeh1,
							row.PropietarioVeh1,
							row.TipoVeh1,
							row.ServicioVeh1,
							row.Marca1,
							row.Submarca1,
							row.Modelo1,
							row.ConductorVeh1,
							row.NumVeh2,
							row.PlacasVeh2,
							row.SerieVeh2,
							row.PropietarioVeh2,
							row.TipoVeh2,
							row.ServicioVeh2,
							row.Marca2,
							row.Submarca2,
							row.Modelo2,
							row.ConductorVeh2,
							row.NumVeh3,
							row.PlacasVeh3,
							row.SerieVeh3,
							row.PropietarioVeh3,
							row.TipoVeh3,
							row.ServicioVeh3,
							row.Marca3,
							row.Submarca3,
							row.Modelo3,
							row.ConductorVeh3,
							row.NumVeh4,
							row.PlacasVeh4,
							row.SerieVeh4,
							row.PropietarioVeh4,
							row.TipoVeh4,
							row.ServicioVeh4,
							row.Marca4,
							row.Submarca4,
							row.Modelo4,
							row.ConductorVeh4,
							row.NumVeh5,
							row.PlacasVeh5,
							row.SerieVeh5,
							row.PropietarioVeh5,
							row.TipoVeh5,
							row.ServicioVeh5,
							row.Marca5,
							row.Submarca5,
							row.Modelo5,
							row.ConductorVeh5,
                            row.NumVeh6,
                            row.PlacasVeh6,
                            row.SerieVeh6,
                            row.PropietarioVeh6,
                            row.TipoVeh6,
                            row.ServicioVeh6,
                            row.Marca6,
                            row.Submarca6,
                            row.Modelo6,
                            row.ConductorVeh6,
                            row.NumVeh7,
                            row.PlacasVeh7,
                            row.SerieVeh7,
                            row.PropietarioVeh7,
                            row.TipoVeh7,
                            row.ServicioVeh7,
                            row.Marca7,
                            row.Submarca7,
                            row.Modelo7,
                            row.ConductorVeh7,
                            row.NumVeh8,
                            row.PlacasVeh8,
                            row.SerieVeh8,
                            row.PropietarioVeh8,
                            row.TipoVeh8,
                            row.ServicioVeh8,
                            row.Marca8,
                            row.Submarca8,
                            row.Modelo8,
                            row.ConductorVeh8,
                            row.NumVeh9,
                            row.PlacasVeh9,
                            row.SerieVeh9,
                            row.PropietarioVeh9,
                            row.TipoVeh9,
                            row.ServicioVeh9,
                            row.Marca9,
                            row.Submarca9,
                            row.Modelo9,
                            row.ConductorVeh9,
                            row.NumVeh10,
                            row.PlacasVeh10,
                            row.SerieVeh10,
                            row.PropietarioVeh10,
                            row.TipoVeh10,
                            row.ServicioVeh10,
                            row.Marca10,
                            row.Submarca10,
                            row.Modelo10,
                            row.ConductorVeh10,
                            row.NumVeh11,
                            row.PlacasVeh11,
                            row.SerieVeh11,
                            row.PropietarioVeh11,
                            row.TipoVeh11,
                            row.ServicioVeh11,
                            row.Marca11,
                            row.Submarca11,
                            row.Modelo11,
                            row.ConductorVeh11,
                            row.NumVeh12,
                            row.PlacasVeh12,
                            row.SerieVeh12,
                            row.PropietarioVeh12,
                            row.TipoVeh12,
                            row.ServicioVeh12,
                            row.Marca12,
                            row.Submarca12,
                            row.Modelo12,
                            row.ConductorVeh12,
                            row.NumVeh13,
                            row.PlacasVeh13,
                            row.SerieVeh13,
                            row.PropietarioVeh13,
                            row.TipoVeh13,
                            row.ServicioVeh13,
                            row.Marca13,
                            row.Submarca13,
                            row.Modelo13,
                            row.ConductorVeh13,
                            row.NumVeh14,
                            row.PlacasVeh14,
                            row.SerieVeh14,
                            row.PropietarioVeh14,
                            row.TipoVeh14,
                            row.ServicioVeh14,
                            row.Marca14,
                            row.Submarca14,
                            row.Modelo14,
                            row.ConductorVeh14,
                            row.NumVeh15,
                            row.PlacasVeh15,
                            row.SerieVeh15,
                            row.PropietarioVeh15,
                            row.TipoVeh15,
                            row.ServicioVeh15,
                            row.Marca15,
                            row.Submarca15,
                            row.Modelo15,
                            row.ConductorVeh15);
                }



                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dataTable).Columns().AdjustToContents();

					using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
						stream.Position = 0;
						var content = stream.ToArray();

						return File(content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "ReporteAccidentePorVehiculo.xlsx");
                    }
                }

            }
            else if (Reporte == "PorAccidente")
			{
                Pagination pagination = new Pagination();
                pagination.PageIndex = 1;
                pagination.PageSize = 200000;
                pagination.Filter = resultValue;

                if (porVehiculoModel == null)
                    porVehiculoModel = model;

                var estadisticasPorAccidente = _estadisticasAccidentesService.AccidentesPorAccidente(porVehiculoModel, pagination);



                DataTable dataTable = new DataTable("Listado Accidentes");
                dataTable.Columns.AddRange(new DataColumn[]
                {
                new DataColumn("No"),
                new DataColumn("Num Reporte"),
				new DataColumn("Fecha"),
				new DataColumn("Hora"),
				new DataColumn("Delegación"),
				new DataColumn("Municipio"),
				new DataColumn("Carretera"),
				new DataColumn("Tramo"),
				new DataColumn("Kilómetro"),
				new DataColumn("Latitud"),
				new DataColumn("Longitud"),
				new DataColumn("Vehículo"),
				new DataColumn("Nombre de oficial"),
				new DataColumn("Daños al camino"),
				new DataColumn("Daños a la carga"),
				new DataColumn("Daños a propietarios"),
				new DataColumn("Otros daños"),
				new DataColumn("Lesionados"),
				new DataColumn("Factores/Opciones"),
				new DataColumn("Causas"),
				new DataColumn("Descripción causas")
				});

                foreach (var row in estadisticasPorAccidente)
                {
                    dataTable.Rows.Add(row.Numero,
                        row.Numreporteaccidente,
						row.Fecha,
						row.Hora,
						row.Delegacion,
						row.municipio,
						row.carretera,
						row.tramo,
						row.kilometro,
						row.latitud,
						row.longitud,
						row.Vehiculo,
						row.NombredelOficial,
						row.Dañosalcamino,
						row.Dañosacarga,
						row.Dañosapropietario,
						row.Otrosdaños,
						row.Lesionados,
						row.FactoresOpciones.TrimStart(' '),
						row.Causas.TrimStart(' '),

						row.CausasDescripcion);
                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dataTable).Columns().AdjustToContents();

					using (MemoryStream stream = new MemoryStream())
					{
						wb.SaveAs(stream);
						stream.Position = 0;
						var content = stream.ToArray();

						return File(content,
							"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "ReporteAccidentePorAccidente.xlsx");
                    }
                }
            }
			else
			{
				return null;
			}
		}

		public byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }



        public JsonResult Tramos_Drop(int carreteraDDValue)
		{
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catTramosService.ObtenerTamosPorCarretera(carreteraDDValue, corp), "IdTramo", "Tramo");
			return Json(result);
		}

		private void filterValue(IEnumerable<IFilterDescriptor> filters)
		{
			if (filters == null)
				return;

			if (filters.Any())
			{
				foreach (var filter in filters)
				{
					var descriptor = filter as FilterDescriptor;
					if (descriptor != null)
					{
						resultValue = descriptor.Value.ToString();
						break;
					}
					else if (filter is CompositeFilterDescriptor)
					{
						if (resultValue == "")
							filterValue(((CompositeFilterDescriptor)filter).FilterDescriptors);
					}
				}
			}
		}

	}
}
