using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using GuanajuatoAdminUsuarios.Services;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class ComparativoInfraccionesController : BaseController
    {
        private readonly ICatDictionary _catDictionary;
        private readonly IComparativoInfraccionesService _comparativoInfracciones;
        private readonly ICatSubtipoServicio _catSubtipoServicio;
        private readonly ICatTramosService _catTramosService;




        public ComparativoInfraccionesController(ICatDictionary catDictionary, IComparativoInfraccionesService comparativoInfracciones,
             IEstadisticasService estadisticasService, ICatSubtipoServicio catSubtipoServicio, ICatTramosService catTramosService
)
        {
            _catDictionary = catDictionary;
            _comparativoInfracciones = comparativoInfracciones;
            _catSubtipoServicio = catSubtipoServicio;
            _catTramosService = catTramosService;



        }
        // GET: ComparativoInfraccionesController
        public ActionResult Index()
        {

           
            var catTipoServicio = _catDictionary.GetCatalog("CatTipoServicio", "0");
            var catTiposVehiculo = _catDictionary.GetCatalog("CatTiposVehiculo", "0");
            var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            //var catTramos = _catDictionary.GetCatalog("CatTramos", "0");
            var catOficiales = _catDictionary.GetCatalog("CatOficiales", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipiosByEntidad", "11");
            //var catCarreteras = _catDictionary.GetCatalog("CatCarreteras", "0");
            var catGarantias = _catDictionary.GetCatalog("CatGarantias", "0");
            var catTipoLicencia = _catDictionary.GetCatalog("CatTipoLicencia", "0");
            var catTipoPlaca = _catDictionary.GetCatalog("CatTipoPlaca", "0");

            var selectList = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem {Text = "Motivos de tránsito", Value = "1"},
                         new SelectListItem {Text = "Motivos de transporte", Value = "2"},
                    }, "Value", "Text");

            ViewBag.TiposMotivos = new SelectList(selectList, "Value", "Text");
            //ViewBag.CatMotivosInfraccion = new SelectList(catMotivosInfraccion.CatalogList, "Id", "Text");
            ViewBag.CatTipoServicio = new SelectList(catTipoServicio.CatalogList, "Id", "Text");
            ViewBag.CatTiposVehiculo = new SelectList(catTiposVehiculo.CatalogList, "Id", "Text");
            ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
            ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
            ViewBag.CatTipoPlaca = new SelectList(catTipoPlaca.CatalogList, "Id", "Text");
            // ViewBag.CatTramos = new SelectList(catTramos.CatalogList, "Id", "Text");
            ViewBag.CatOficiales = new SelectList(catOficiales.CatalogList, "Id", "Text");
            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            //ViewBag.CatCarreteras = new SelectList(catCarreteras.CatalogList, "Id", "Text");
            ViewBag.CatGarantias = new SelectList(catGarantias.CatalogList, "Id", "Text");
            ViewBag.ComparativoInfraccionesResumen = new ComparativoInfraccionesResumenModel();
            return View(new ComparativoInfraccionesModel()
            {
                año1 = DateTime.Now.AddYears(-1).Year,
                año2 = DateTime.Now.Year
            });
        }


        public IActionResult ajax_ComparativoInfracciones(ComparativoInfraccionesModel model)
        {
            var resumen = new ComparativoInfraccionesResumenModel();
            resumen.año1 = model.año1;
            resumen.año2 = model.año2;
            resumen.resultadosGenerales = _comparativoInfracciones.BusquedaResultadosGenerales(model);
            resumen.detallesPorCausa = _comparativoInfracciones.BusquedaDetallesPorCausas(model);
            resumen.desgloseTotalDeInfracciones = _comparativoInfracciones.DesgloseTotalesInfracciones(model);
            return PartialView("_ComparativoInfraccionesResultados", resumen);
        }

        public JsonResult SubtipoServicio_Drop(int tipoServicioDDlValue)
        {
            var result = new SelectList(_catSubtipoServicio.GetSubtipoPorTipo(tipoServicioDDlValue), "idSubTipoServicio", "subTipoServicio");
            return Json(result);
        }

        public JsonResult Tramos_Drop(int carreteraDDValue)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catTramosService.ObtenerTamosPorCarretera(carreteraDDValue, corp), "IdTramo", "Tramo");
            return Json(result);
        }

        public JsonResult TiposMotivos_Drop(int idTipo)
        {
            var catMotivosInfraccion = _catDictionary.GetCatalog("CatAllMotivosInfraccion", "0");

            SelectList result = null;
            if (idTipo ==0)
                result = new SelectList(catMotivosInfraccion.CatalogList, "Id", "Text");
            else if (idTipo == 1)
            {
                result = new SelectList(catMotivosInfraccion.CatalogList.Where(x=>x.Transito == true), "Id", "Text");
            }
            else if (idTipo == 2)
            {
                result = new SelectList(catMotivosInfraccion.CatalogList.Where(x => x.Transito == false), "Id", "Text");
            }
            return Json(result);
        }
    }
}
