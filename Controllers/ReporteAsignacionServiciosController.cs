using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class ReporteAsignacionServiciosController : BaseController
    {
        private readonly IPadronDepositosGruasService _padronDepositosGruasService;
        private readonly IGruasService _gruasService;
        private readonly IEventoService _eventoService;
        private readonly IReporteAsignacionService _reporteAsignacionService;
        private readonly IPdfGenerator _pdfService;
        private readonly ICatDictionary _catDictionary;
        private readonly ITransitoTransporteService _transitoTransporteService;
        private readonly IConcesionariosService _concesionariosService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;


        public ReporteAsignacionServiciosController(
            ITransitoTransporteService transitoTransporteService,
            IPadronDepositosGruasService padronDepositosGruasService,
             IGruasService gruasService, IEventoService eventoService,
             IReporteAsignacionService reporteAsignacionService, IPdfGenerator pdfService
            , ICatDictionary catDictionary,
             IConcesionariosService concesionariosService,
             ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService


            )
        {
            _padronDepositosGruasService = padronDepositosGruasService;
            _gruasService = gruasService;
            _eventoService = eventoService;
            _reporteAsignacionService = reporteAsignacionService;
            _pdfService = pdfService;
            _catDictionary = catDictionary;
            _transitoTransporteService = transitoTransporteService;
            _concesionariosService = concesionariosService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
        }
        public IActionResult Index()
        {        
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            ReporteAsignacionBusquedaModel searchModel = new ReporteAsignacionBusquedaModel();
            // List<ReporteAsignacionModel> listReporteAsignacion = _reporteAsignacionService.GetAllReporteAsignaciones(idOficina);
            //searchModel.ListReporteAsignacion = listReporteAsignacion;
            return View(searchModel);         
        }

        [HttpPost]
        public ActionResult ajax_BuscarReporte(ReporteAsignacionBusquedaModel model)
        {
      
                // Verifica si las fechas son nulas y asigna DateTime.MinValue si es el caso
                if (model.FechaInicio == null)
                {
                    model.FechaInicio = DateTime.MinValue;
                }

                if (model.FechaFin == null)
                {
                    model.FechaFin = DateTime.MaxValue;
                }
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

                var listReporteAsignacion = _reporteAsignacionService.GetAllReporteAsignaciones(model, idOficina);
                foreach(ReporteAsignacionModel item in listReporteAsignacion)
                {
                    if (item.vehiculoKm.Trim()!="")
                        item.vehiculoKm = Convert.ToDecimal(item.vehiculoKm).ToString("G29");
                item.fechaSolicitud2 = Convert.ToDateTime(item.fechaSolicitud) == DateTime.MinValue  ?"" : Convert.ToDateTime(item.fechaSolicitud).ToString("dd/MM/yyyy");
                item.fechaLiberacion2 = Convert.ToDateTime(item.fechaLiberacion) == DateTime.MinValue?"": Convert.ToDateTime(item.fechaLiberacion).ToString("dd/MM/yyyy");
                }

                if (listReporteAsignacion.Count == 0)
                {
                    ViewBag.NoResultsMessage = "No se encontraron registros que cumplan con los criterios de búsqueda.";
                }
                return PartialView("_ListadoReporteAsignacion", listReporteAsignacion);
               
            }
   

    

    [HttpGet]
        public FileResult CreatePdf(string data)
        {
           var model2 = JsonConvert.DeserializeObject<ReporteAsignacionBusquedaModel2>(data);

            var model = new ReporteAsignacionBusquedaModel();

            model.IdGrua = model2.IdGrua;
            model.IdPension = model2.IdPension;
            model.IdEvento = model2.IdEvento;
            model.idDelegacion = model2.idDelegacion;
            model.Evento = model2.Evento;

            if (String.IsNullOrEmpty(model2.FechaInicio))
            {
                model.FechaInicio = DateTime.MinValue;
            }
            else
            {
                var axudate1 = DateTime.ParseExact(model2.FechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                model.FechaInicio = axudate1;
            }

            if (String.IsNullOrEmpty(model2.FechaFin))
            {

                model.FechaFin = DateTime.MaxValue;
            }
            else
            {
                var axudate2 = DateTime.ParseExact(model2.FechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                model.FechaFin = axudate2;
            }


            //if (model.FechaInicio == null)
            //{
            //    model.FechaInicio = DateTime.MinValue;
            //}

            //if (model.FechaFin == null)
            //{
            //    model.FechaFin = DateTime.MaxValue;
            //}

            //if (String.IsNullOrEmpty( model2.FechaFin ))
            //{

            //    model.FechaFin = DateTime.MaxValue;
            //}
            //else
            //{
            //    var axudate2 = DateTime.ParseExact(model2.FechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            //    model.FechaFin = axudate2;
            //}


            model.Evento = model2.Evento == string.Empty ? null : model2.Evento;

            Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
            {
            {"vehiculoCarretera","#"},
            {"vehiculoTramo","Servicio"},
            {"vehiculoKm","Lugar"},
            {"fechaSolicitud","Solicitante"},
            {"fechaLiberacion","Otros"},
            {"evento","Grúa / Pensión"},
            //{"fullName","Nombre Solicitante"}
            };
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var listReporteAsignacion = _reporteAsignacionService.GetAllReporteAsignaciones(model,idOficina);
            var result = _pdfService.CreatePdfReporteAsignacion("ReporteAsignacionServicios", "Reporte de Asignación de Servicios", ColumnsNames, listReporteAsignacion,Array.Empty<float>());
            return File(result.Item1, "application/pdf", result.Item2);
        }

        public JsonResult Evento_Read()
        {
            var result = new SelectList(_eventoService.GetEventos(), "IdEvento", "Evento");
            return Json(result);
        }

        public JsonResult Pension_Read()
        {
            var result = new SelectList(_transitoTransporteService.GetPensiones(), "IdPension", "Pension");
            return Json(result);
        }


        public JsonResult Grua_Read()
        {
            var result = new SelectList(_concesionariosService.GetAllConcesionarios(), "IdConcesionario", "Concesionario");
            return Json(result);
        }
        public JsonResult Delegaciones_Read()
        {
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinas(), "IdOficinaTransporte", "NombreOficina");
            return Json(result);
        }

    }
}
