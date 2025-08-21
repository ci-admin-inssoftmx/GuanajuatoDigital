using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class ReportesController : BaseController
    {
        private readonly IReportes _reportesService;
        public ReportesController(IReportes reportesService
)
        {
            _reportesService = reportesService;
        }

            public IActionResult Index()
        {
            var reportes = new List<SelectListItem>
    {
        new SelectListItem { Text = "Infracciones por tipo de  licencia", Value = "1" },
        new SelectListItem { Text = "Infracciones por Corporacion", Value = "2" },
        new SelectListItem { Text = "Infracciones por licencia", Value = "3" },
        new SelectListItem { Text = "Municipios con mas infracciones", Value = "4" },
        new SelectListItem { Text = "Municipio/Colonia con mas infracciones", Value = "5" },
        new SelectListItem { Text = "Infracciones por día de la semana y hora", Value = "6" }

    };

            ViewBag.Reportes = reportes;
            return View();
        }
        public IActionResult Accidentes()
        {
            var reportes = new List<SelectListItem>
    {
        new SelectListItem { Text = "Accidentes por corporación", Value = "1" },
        new SelectListItem { Text = "Municipios con mas accidentes", Value = "2" },
        new SelectListItem { Text = "Municipios/Colonias con mas accidentes", Value = "3" },
        new SelectListItem { Text = "Daños por accidentes", Value = "4" },


    };
            ViewBag.ReportesAccidentes = reportes;
            return View("Accidentes");
        }
        public IActionResult Otros()
        {
            var reportes = new List<SelectListItem>
        {
        new SelectListItem { Text = "Infracciones/Accidentes por municipio", Value = "1" },

        };

            ViewBag.ReportesOtros = reportes;
            return View("Otros");
        }

        [HttpPost]
        public IActionResult ajax_BuscarReporte(int idReporte)
        {

            switch (idReporte)
            {
                case 1:
                    return PartialView("_InfraccionesPorTipoLicencia");
                case 2:
                    return PartialView("_LIstaInfraccionsCorporacion");
                case 3:
                    return PartialView("_ListaInfraccionesPorLicencia");
                case 4:
                    return PartialView("_ListaMunicipiosMasInfracciones");
                case 5:
                    return PartialView("_ListaMunicipiosColoniaMasInfracciones");
                case 6:
                    return PartialView("_ListaInfraccionesPorDiaYHora");
                default:
                    return Json(new { success = false, message = "Catálogo no encontrado." });
            }
        }
        public IActionResult ajax_BuscarReporteAccidentes(int idReporte)
        {

            switch (idReporte)
            {
                case 1:
                    return PartialView("_AccidentesPorCorporacion");
                case 2:
                    return PartialView("_MunicipiosMasAccidentes");
                case 3:
                    return PartialView("_MunicipiosColoniasMasAccidentes");
                case 4:
                    return PartialView("_ListaDañosAccidentes");
                default:
                    return Json(new { success = false, message = "Catálogo no encontrado." });
            }
        }
        public IActionResult ajax_BuscarReporteOtros(int idReporte)
        {

            switch (idReporte)
            {
                case 1:
                    return PartialView("_InfraccionesAccidentesMunicipio");
               
                default:
                    return Json(new { success = false, message = "Catálogo no encontrado." });
            }
        }
        public JsonResult GetInfraccionesPorTipoLicencia([DataSourceRequest] DataSourceRequest request)
        {
            var corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);
            var listaDatos = _reportesService.BusquedaInfPorTipoLicencia(corp);
            return Json(listaDatos.ToDataSourceResult(request));
        }

        public JsonResult GetInfraccionesCorporacion([DataSourceRequest] DataSourceRequest request)
        {
            var listaDatos = _reportesService.BusquedaInfTodasCorporaciones();
            return Json(listaDatos.ToDataSourceResult(request));
        }
        public JsonResult GetAccidentesCorporacion([DataSourceRequest] DataSourceRequest request)
        {
            var listaDatos = _reportesService.BusquedaAccidentesTodasCorporaciones();
            return Json(listaDatos.ToDataSourceResult(request));
        }
        public JsonResult GetMuncipiosMasAccidentes([DataSourceRequest] DataSourceRequest request)
        {
            var corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);
            var listaDatos = _reportesService.BusquedaMunicipiosMasAccidentes(corp);
            return Json(listaDatos.ToDataSourceResult(request));
        }
        public JsonResult GetInfraccionesAccidentesMunicipio([DataSourceRequest] DataSourceRequest request)
        {
            var corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);
            var listaDatos = _reportesService.BusquedaInfraccionesAccidentesMunicipio(corp);
            return Json(listaDatos.ToDataSourceResult(request));
        }

        public JsonResult GetInfraccionesPorLicencia([DataSourceRequest] DataSourceRequest request, string txtLicencia)
        {
            var listaDatos = _reportesService.BusquedaInfraccionesPorLicencia(txtLicencia);
            return Json(listaDatos.ToDataSourceResult(request));
        }
        public JsonResult GetMunicipiosConMasInfracciones([DataSourceRequest] DataSourceRequest request, string txtLicencia)
        {
            var corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);
            var listaDatos = _reportesService.BusquedaMunicipiosMasInfracciones(corp);
            return Json(listaDatos.ToDataSourceResult(request));
        }

        public JsonResult GetMunicipiosConMasAccidentes([DataSourceRequest] DataSourceRequest request)
        {
            var listaDatos = _reportesService.BusquedaMunicipiosMasAccidentesB();
            return Json(listaDatos.ToDataSourceResult(request));
        }

        public JsonResult GetMunicipiosColoniasConMasInfracciones([DataSourceRequest] DataSourceRequest request)
        {
            var corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);
            var listaDatos = _reportesService.BusquedaMunicipiosColoniasMasInfracciones(corp);
            return Json(listaDatos.ToDataSourceResult(request));
        }

        public JsonResult GetMunicipiosColoniasConMasAccidentes([DataSourceRequest] DataSourceRequest request)
        {
            var listaDatos = _reportesService.BusquedaMunicipiosColoniasMasAccidentes();
            return Json(listaDatos.ToDataSourceResult(request));
        }

        public JsonResult GetDanosAccidentes([DataSourceRequest] DataSourceRequest request)
        {
            var listaDatos = _reportesService.BusquedaDanosAccidentes();
            return Json(listaDatos.ToDataSourceResult(request));

        }

        public JsonResult GetInfraccionesDiaYHora([DataSourceRequest] DataSourceRequest request)
        {
            var listaDatos = _reportesService.BusquedaInfraccionesProDiayHora();
            return Json(listaDatos.ToDataSourceResult(request));

        }
    }
}
