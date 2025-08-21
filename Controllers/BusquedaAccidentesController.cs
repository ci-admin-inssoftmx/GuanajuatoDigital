using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Utils;
using Kendo.Mvc.Infrastructure.Implementation;
using Microsoft.AspNetCore.Authorization;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Kendo.Mvc;


namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class BusquedaAccidentesController : BaseController
    {
        private readonly IBusquedaAccidentesService _busquedaAccidentesService;
        private readonly ICatCarreterasService _catCarreterasService;
        private readonly ICatTramosService _catTramosService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;
        private readonly IOficiales _oficialesService;
        private readonly ICatEstatusReporteService _catEstatusReporteService;
		private readonly IBitacoraService _bitacoraServices;

		private string resultValue = string.Empty;
        public static BusquedaAccidentesModel AccidentesNuevoModel = new BusquedaAccidentesModel();

        public BusquedaAccidentesController(IBusquedaAccidentesService busquedaAccidentesService, ICatCarreterasService catCarreterasService, ICatTramosService catTramosService,
            ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService, IOficiales oficialesService,
            ICapturaAccidentesService capturaAccidentesService, ICatDictionary catDictionary, ICatEstatusReporteService catEstatusReporteService, IBitacoraService bitacoraServices)
        {
            _busquedaAccidentesService = busquedaAccidentesService;
            _catCarreterasService = catCarreterasService;
            _catTramosService = catTramosService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
            _oficialesService = oficialesService;
            _catEstatusReporteService = catEstatusReporteService;
            _bitacoraServices = bitacoraServices;

		}
        #region DropDowns
        public IActionResult Index()
        {
            HttpContext.Session.Remove("isEditAccidente");
            HttpContext.Session.Remove("origen");
            HttpContext.Session.Remove("LastInsertedId");

            int? idOficina = HttpContext.Session.GetInt32("IdOficina");
            if (idOficina != 1)
            {
                BusquedaAccidentesModel modelo = new BusquedaAccidentesModel
                {
                    IdDelegacionBusqueda = idOficina ?? 0,
                };
                return View(modelo);
            }else
            {
                return View(new BusquedaAccidentesModel());
            }
               
            }

        public JsonResult GetAllAccidentes([DataSourceRequest] DataSourceRequest request)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var resultadoBusqueda = _busquedaAccidentesService.GetAllAccidentes();

            return Json(resultadoBusqueda.ToDataSourceResult(request));
        }
        public JsonResult Delegaciones_Drop()
        {
            var tipo = Convert.ToInt32(HttpContext.Session.GetInt32("IdDependencia").ToString());
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos().Where(x => x.Transito == tipo), "IdDelegacion", "Delegacion");
            //var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos(), "IdDelegacion", "Delegacion");
            return Json(result);
        }
        public JsonResult Carreteras_Drop()
        {
            var tipo = Convert.ToInt32(HttpContext.Session.GetInt32("IdDependencia").ToString());
            var result = new SelectList(_catCarreterasService.ObtenerCarreteras().Where(x=>x.Transito == tipo), "IdCarretera", "Carretera");
            return Json(result);
        }

        public JsonResult Tramos_Drop(int carreteraDDValue)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catTramosService.ObtenerTamosPorCarretera(carreteraDDValue, corp), "IdTramo", "Tramo");
            return Json(result);
        }
        public JsonResult Oficiales_Drop()
        {
            var tipo = Convert.ToInt32(HttpContext.Session.GetInt32("IdDependencia").ToString());
            var oficiales = _oficialesService.GetOficialesActivos().Where(x => x.transito == tipo)
                .Select(o => new
                {
                    IdOficial = o.IdOficial,
                    NombreCompleto = $"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}"
                });
            oficiales = oficiales.Skip(1);
            var result = new SelectList(oficiales, "IdOficial", "NombreCompleto");

            return Json(result);
        }
        public JsonResult EstatusAccidente_Drop()
        {
            var result = new SelectList(_catEstatusReporteService.ObtenerEstatusReporte(), "idEstatusReporte", "estatusReporte");
            return Json(result);
        }
        #endregion

        /* public ActionResult ajax_BuscarAccidente(BusquedaAccidentesModel model)
         {
             if (model.FechaInicio == null)
             {
                 model.FechaInicio = DateTime.MinValue;
             }

             if (model.FechaFin == null)
             {
                 model.FechaFin = DateTime.MinValue;
             }

             int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
             var resultadoBusqueda = _busquedaAccidentesService.BusquedaAccidentes(model, idOficina);
             return Json(resultadoBusqueda);
         }*/
     /*   public IActionResult ajax_BusquedaAccidentes(BusquedaAccidentesModel model)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var resultadoBusqueda = _busquedaAccidentesService.GetAllAccidentes()
                                                .Where(w => w.idMunicipio == (model.idMunicipio > 0 ? model.idMunicipio : w.idMunicipio)
                                                    && w.idSupervisa == (model.IdOficialBusqueda > 0 ? model.IdOficialBusqueda : w.idSupervisa)
                                                    && w.idCarretera == (model.IdCarreteraBusqueda > 0 ? model.IdCarreteraBusqueda : w.idCarretera)
                                                    && w.idTramo == (model.IdTramoBusqueda > 0 ? model.IdTramoBusqueda : w.idTramo)
                                                    && w.idElabora == (model.IdOficialBusqueda > 0 ? model.IdOficialBusqueda : w.idElabora)
                                                    && w.idAutoriza == (model.IdOficialBusqueda > 0 ? model.IdOficialBusqueda : w.idAutoriza)
                                                    && w.idEstatusReporte == (model.IdEstatusAccidente > 0 ? model.IdEstatusAccidente : w.idEstatusReporte)
                                                    && w.IdDelegacionBusqueda == (model.IdDelegacionBusqueda > 0 ? model.IdDelegacionBusqueda : w.IdDelegacionBusqueda)
                                                    && (string.IsNullOrEmpty(model.folioBusqueda) || w.numeroReporte.Contains(model.folioBusqueda))
                                                   && (string.IsNullOrEmpty(model.placasBusqueda) || w.placa.Contains(model.placasBusqueda))
                                                   && (string.IsNullOrEmpty(model.propietarioBusqueda) || w.propietario.Contains(model.propietarioBusqueda, StringComparison.OrdinalIgnoreCase))
                                                   && (string.IsNullOrEmpty(model.serieBusqueda) || w.serie.Contains(model.serieBusqueda))
                                                   && (string.IsNullOrEmpty(model.conductorBusqueda) || w.conductor.Contains(model.conductorBusqueda, StringComparison.OrdinalIgnoreCase))
                                                   && ((model.FechaInicio == default(DateTime) && model.FechaFin == default(DateTime)) || (w.fecha >= model.FechaInicio && w.fecha <= model.FechaFin))
                                                    ).ToList();
			for (int i = 0; i < resultadoBusqueda.Count; i++)
			{
				resultadoBusqueda[i].Numero = i + 1;
			}

			return Json(resultadoBusqueda);

        }*/
        public IActionResult ajax_BusquedaAccidentes([DataSourceRequest] DataSourceRequest request, BusquedaAccidentesModel model)
        {
           
                AccidentesNuevoModel = model;
            return PartialView("_ListaAccidentesBusqueda", new List<BusquedaAccidentesModel>());
          
        }
        public ActionResult GetAccidentesBusquedaPagination([DataSourceRequest] DataSourceRequest request, BusquedaAccidentesModel model)
        {
  
        Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page -1;
            pagination.PageSize = 10;
            if (AccidentesNuevoModel == null)
                AccidentesNuevoModel = model;



            int idOficina = (int)HttpContext.Session.GetInt32("IdOficina");
            var accidentesList = _busquedaAccidentesService.GetAllAccidentesPaginationWhitcode(pagination, AccidentesNuevoModel, idOficina);

            var total = 0;
            if (accidentesList.Count() > 0)
                total = accidentesList.ToList().FirstOrDefault().total;

            request.PageSize = 10;
            var result = new DataSourceResult()
            {
                Data = accidentesList,
                Total = total
            };
			//BITACORA
			_bitacoraServices.BitacoraAccidentes(0, CodigosAccidente.C2047,"");
			return Json(result);
            }


        private void filterValue(IEnumerable<IFilterDescriptor> filters)
        {
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
