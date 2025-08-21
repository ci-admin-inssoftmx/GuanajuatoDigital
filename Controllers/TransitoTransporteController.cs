using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;
using System.Linq;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Kendo.Mvc;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class TransitoTransporteController : BaseController
    {
        private readonly ITransitoTransporteService _transitoTransporteService;
        private readonly IDependencias _dependeciaService;
        private readonly IGruasService _gruasService;
        private readonly IPdfGenerator _pdfService;
        private readonly ICatDictionary _catDictionary;
		private readonly IBitacoraService _bitacoraServices;
        private readonly ILiberacionVehiculoService _liberacionVehiculoService;
        private string resultValue = string.Empty;

        public TransitoTransporteController(ITransitoTransporteService transitoTransporteService,
            IDependencias dependeciaService, IGruasService gruasService,
            IPdfGenerator pdfService, ICatDictionary catDictionary,IBitacoraService bitacoraServices,
            ILiberacionVehiculoService liberacionVehiculoService
            )
        {
            _transitoTransporteService = transitoTransporteService;
            _dependeciaService = dependeciaService;
            _gruasService = gruasService;
            _pdfService = pdfService;
            _catDictionary = catDictionary;
            _bitacoraServices = bitacoraServices;
            _liberacionVehiculoService = liberacionVehiculoService;

        }

        public IActionResult Index()
        {
            HttpContext.Session.Remove("Busqueda");
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            TransitoTransporteBusquedaModel searchModel = new TransitoTransporteBusquedaModel();
            //List<TransitoTransporteModel> listTransitoTransporte = _transitoTransporteService.GetAllTransitoTransporte(idOficina);
            //searchModel.ListTransitoTransporte = listTransitoTransporte;

            return View(searchModel);

        }

        [HttpGet]
        public FileResult CreatePdf(string data)
        {
            var model = JsonConvert.DeserializeObject<TransitoTransporteBusquedaModel>(data);
            /* if (model.FechaIngreso == null)
             {
                 model.FechaIngreso = DateTime.MinValue;
             }

             if (model.FechaIngresoFin == null)
             {
                 model.FechaIngresoFin = DateTime.MinValue;
             }*/
            model.FolioInfraccion = model.FolioInfraccion == string.Empty ? null : model.FolioInfraccion;
            model.FolioSolicitud = model.FolioSolicitud == string.Empty ? null : model.FolioSolicitud;
            model.NumeroEconomico = model.NumeroEconomico == string.Empty ? null : model.NumeroEconomico;
            model.Placas = model.Placas == string.Empty ? null : model.Placas;
            model.Serie = model.Serie == string.Empty ? null : model.Serie;
            model.Propietario = model.Propietario == string.Empty ? null : model.Propietario;

            Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
            {
            {"fullSolicitudfolioInfraccion","Fecha_evento/Folio_Solicitud/Folio_Infracción"},
            {"fullVehiculo","Vehículo"},
            {"FechaIngreso","Fecha Ingreso"},
            {"FechaLiberacion","Fecha Liberación"},
            };
            int idOficina = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value); //HttpContext.Session.GetInt32("IdOficina") ?? 0;
            Pagination pagination = new Pagination();
            pagination.PageIndex = 0;
            pagination.PageSize = 100000000;
            var ListTransitoModel = _transitoTransporteService.GetTransitoTransportes(model, pagination, idOficina);
            var result = _pdfService.CreatePdf("ReporteTransitoTransporte", "Tránsito Transporte", ColumnsNames, ListTransitoModel, Array.Empty<float>());
            _bitacoraServices.BitacoraDepositos(0, "Busqueda", CodigosDepositos.C3008, model);
            
            return File(result.Item1, "application/pdf", result.Item2);
        }

        [HttpGet]
        public FileResult CreatePdfUnRegistro(int IdDeposito)
        {
            Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
            {
            {"fullSolicitudfolioInfraccion","Fecha_evento/Folio_Solicitud/Folio_Infracción"},
            {"fullVehiculo","Vehículo"},
            {"UbicacionVehiculo","Ubicación"},
            {"DatosGrua","Grua"},
            {"Tiempos","Tiempo Servicio"},

            };
            var TransitoModel = _transitoTransporteService.GetTransitoTransporteById(IdDeposito);
            //var result = _pdfService.CreatePdf("ReporteTransitoTransporte", "Tránsito Transporte", 5, ColumnsNames, TransitoModel);
            var report = new TransitoTransporteReportService("ServicioGruaDetalle", "SERVICIO DE GRÚA").CreatePdf(TransitoModel);
			object data = new { IdDeposito = IdDeposito };
			_bitacoraServices.BitacoraDepositos(IdDeposito, "Busqueda", string.Format(CodigosDepositos.C3010,IdDeposito), data);

			return File(report.File.ToArray(), "application/pdf", report.FileName);
        }


        [HttpPost]
        public ActionResult ajax_BuscarTransito(TransitoTransporteBusquedaModel model)
        {
            Pagination pagination = new Pagination();
            pagination.PageIndex = 0;
            pagination.PageSize = 100000000;
            int idOficina = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value);   //HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var ListTransitoModel = _transitoTransporteService.GetTransitoTransportes(model, pagination, idOficina);
			_bitacoraServices.BitacoraDepositos(0, "Búsqueda", CodigosDepositos.C3007, model);

			if (ListTransitoModel.Count == 0)
            {
                ViewBag.NoResultsMessage = "No se encontraron registros que cumplan con los criterios de búsqueda.";
            }

            return PartialView("_ListadoTransitoTransporte", ListTransitoModel);

        }


        public JsonResult Delegacion_Read()
        {
            var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            var result = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult Pension_Read()
        {
            var result = new SelectList(_transitoTransporteService.GetPensiones(), "IdPension", "Pension");
            return Json(result);
        }


        public JsonResult DependenciaEnvia_Read()
        {
            var CatDependencias = _catDictionary.GetCatalog("DependenciaEnvia_Read", "0");
            var result = new SelectList(CatDependencias.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult Estatus_Read()
        {
            /* var directions = from EstatusTransitoTransporte d in Enum.GetValues(typeof(EstatusTransitoTransporte))
                              select new { ID = (int)d, Name = d.ToString() };
             var result = new SelectList(directions, "ID", "Name");*/
            var CatEstatusTransitoTransporte = _catDictionary.GetCatalog("CatEstatusTransitoTransporte", "0");
            var result = new SelectList(CatEstatusTransitoTransporte.CatalogList, "Id", "Text");

            //var result = new SelectList(_transitoTransporteService.GetPensiones(), "IdPension", "Pension");
            return Json(result);
        }


        [HttpGet]
        public ActionResult ajax_DetailGruas(int Id)
        {
            var model = _gruasService.GetGruasConcesionariosByIdCocesionario(Id);
			object data = new { IdDeposito = Id };
			_bitacoraServices.BitacoraDepositos(Id, "Busqueda", CodigosDepositos.C3009, data);
			return PartialView("_GruasConcesionarioDetail", model);

        }

        [HttpPost]
        public ActionResult ajax_DeleteTransito(string ids, [DataSourceRequest] DataSourceRequest request)
        {
            if (ids.IsNullOrEmpty())
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return null;
            }

            string[] idsList = ids.Split(",");
            var result = _transitoTransporteService.DeleteTransitoTransporte(Convert.ToInt32(idsList[0]), Convert.ToInt32(idsList[1]));
            if (result > 0)
            {

                Pagination pagination = new Pagination();
                pagination.PageIndex = 0;
                pagination.PageSize = 100000000;
                pagination.Filter = "";

                int idOficina = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value); //HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var aux = new TransitoTransporteBusquedaModel();
                var ListTransitoModel = _transitoTransporteService.GetTransitoTransportes(aux, pagination, idOficina);
                return PartialView("_ListadoTransitoTransporte", ListTransitoModel);
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return null;
            }
        }


        public JsonResult GetListado([DataSourceRequest] DataSourceRequest request,
                                        string placas,
                                        string serie,
                                        string folioSolicitud,
                                        string folioInfraccion,
                                        string propietario,
                                        string numeroEconomico,
                                        string delegacion,
                                        string pension,
                                        string estatus,
                                        string dependenciaNoTransito,
                                        string dependenciaTransito,
                                        string fechaIngreso,
                                        string fechaIngresoFin,
                                        string evento)

        {
            int idOficina = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value);
            var transitoTransporte = new TransitoTransporteBusquedaModel();
            filterValue(request.Filters);

            transitoTransporte.Placas = String.IsNullOrEmpty(placas) ? null : placas;
            transitoTransporte.Serie = String.IsNullOrEmpty(serie) ? null : serie;
            transitoTransporte.FolioSolicitud = String.IsNullOrEmpty(folioSolicitud) ? null : folioSolicitud;
            transitoTransporte.Propietario = String.IsNullOrEmpty(propietario) ? null : propietario;
            transitoTransporte.NumeroEconomico = String.IsNullOrEmpty(numeroEconomico) ? null : numeroEconomico;
            transitoTransporte.IdDelegacion = String.IsNullOrEmpty(delegacion) ? null : Convert.ToInt32(delegacion);
            transitoTransporte.Evento = String.IsNullOrEmpty(evento) ? null : evento;


            if (transitoTransporte.IdDelegacion != null)
            {
                if (transitoTransporte.IdDelegacion == -1)
                    transitoTransporte.IdDelegacion = null;
            }
            transitoTransporte.IdPension = String.IsNullOrEmpty(pension) ? null : Convert.ToInt32(pension);
            if (transitoTransporte.IdPension != null)
            {
                if (transitoTransporte.IdPension == -1)
                    transitoTransporte.IdPension = null;
            }
            transitoTransporte.IdEstatus = String.IsNullOrEmpty(estatus) ? null : Convert.ToInt32(estatus);
            if (transitoTransporte.IdEstatus != null)
            {
                if (transitoTransporte.IdEstatus == -1)
                    transitoTransporte.IdEstatus = null;
            }
            transitoTransporte.FechaIngreso = String.IsNullOrEmpty(fechaIngreso) ? null : Convert.ToDateTime(fechaIngreso);
            transitoTransporte.FechaIngresoFin = String.IsNullOrEmpty(fechaIngresoFin) ? null : Convert.ToDateTime(fechaIngresoFin);
            transitoTransporte.IdDependenciaNoTransito = String.IsNullOrEmpty(dependenciaNoTransito) ? null : Convert.ToInt32(dependenciaNoTransito);
            if (transitoTransporte.IdDependenciaNoTransito != null)
            {
                if (transitoTransporte.IdDependenciaNoTransito == -1)
                    transitoTransporte.IdDependenciaNoTransito = null;
            }
                transitoTransporte.FolioInfraccion = String.IsNullOrEmpty(folioInfraccion) ? null : folioInfraccion;
            transitoTransporte.IdDependenciaTransito = String.IsNullOrEmpty(dependenciaTransito) ? null : Convert.ToInt32(dependenciaTransito);
            if (transitoTransporte.IdDependenciaTransito != null)
            {
                if (transitoTransporte.IdDependenciaTransito == -1)
                    transitoTransporte.IdDependenciaTransito = null;
            }

            Pagination pagination = new Pagination();
            pagination.Filter = resultValue;
            var t = request.Sorts;
            if (t.Count() > 0)
            {
                var q = t[0];
                pagination.Sort = q.SortDirection.ToString().Substring(0, 3);
                pagination.SortCamp = q.Member;
            }

            if (HttpContext.Session.GetInt32("Busqueda") != null)
            {
                if (HttpContext.Session.GetInt32("Busqueda") == 1)
                {
                    request.Page = 1;
                }
                pagination.PageIndex = request.Page - 1;
                HttpContext.Session.Remove("Busqueda");
            } else
            {
                pagination.PageIndex = request.Page - 1;
            }
            pagination.PageSize = (request.PageSize != 0) ? request.PageSize : 10;
            var total = 0;

            var ListTransitoModel = _transitoTransporteService.GetTransitoTransportes(transitoTransporte, pagination, idOficina);
            if (ListTransitoModel.Count() > 0)
                total = ListTransitoModel.ToList().FirstOrDefault().total;

            request.PageSize = pagination.PageSize;
            var result = new DataSourceResult()
            {
                Data = ListTransitoModel,
                Total = total
            };
            if (ListTransitoModel.Count == 0)
            {
                //ViewBag.NoResultsMessage = "No se encontraron registros que cumplan con los criterios de búsqueda.";
                throw new Exception("No se encontraron registros que cumplan con los criterios de búsqueda.");
            }
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


        public JsonResult SetBuscar()

        {
            HttpContext.Session.SetInt32("Busqueda",1);
            return Json(new { ok = false });
        }

        [HttpGet]
        public IActionResult DownloadLiberacionPdf(int idDeposito)
        {
            try
            {
                int idOficina = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value); //HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var modelData = _liberacionVehiculoService.GetDepositoRutaFormatoManual(idDeposito);

                if (!System.IO.File.Exists(modelData.FormatoManualPath))
                {
                    return NotFound("No se encontró el archivo PDF.");
                }

                var fileBytes = System.IO.File.ReadAllBytes(modelData.FormatoManualPath);
                string pdfBase64 = Convert.ToBase64String(fileBytes);
                return Json(new
                {
                    success = true,
                    message = "Descarga Exitosa",
                    pdfBase64,
                    fileName = $"LiberacionVehiculo_{idDeposito}.pdf"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al intentar descargar el archivo PDF", details = ex.Message });
            }
        }
    }
}
