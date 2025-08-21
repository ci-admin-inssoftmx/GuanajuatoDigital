using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
//using Telerik.SvgIcons;
using System;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class CatTramosController : BaseController
    {
        private readonly ICatTramosService _catTramosService;
        private readonly ICatCarreterasService _catCarreterasService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;


        public CatTramosController(ICatTramosService catTramosService, ICatCarreterasService catCarreterasService, ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService)
        {
            _catTramosService = catTramosService;
            _catCarreterasService = catCarreterasService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
        }
        public IActionResult Index()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var ListTramosModel = _catTramosService.ObtenerTramos(corp);
            ViewBag.ListadoTramos = ListTramosModel;

            return View();
        }
      

        public JsonResult Carreteras_Drop(int idDelegacion)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var tipo = corp < 3 ? 1 : corp;
            var result = new SelectList(_catCarreterasService.GetCarreterasPorDelegacion(idDelegacion).Where(x=>x.Transito == tipo), "IdCarretera", "Carretera");
            return Json(result);
        }


        [HttpPost]
        public ActionResult MostrarModalAgregarTramo()
        {
     
                return PartialView("_Crear");
            }
     

        public ActionResult EditarTramoModal(int IdTramo)
        {
  
                var TramosModel = _catTramosService.ObtenerTramoByID(IdTramo);
            return PartialView("_Editar", TramosModel);
            }


        [HttpPost]
        public ActionResult CrearTramoMod(CatTramosModel model)
        {

          
                var corp = model.Corp;

                if (corp == null)
                {
                    corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
                }

                _catTramosService.CrearTramo(model,(int)corp);
                var TramosModel = _catTramosService.ObtenerTramos((int)corp);
                return Json(TramosModel);
            

        }

        public ActionResult EditarTramoBD(CatTramosModel model)
        {
            bool switchTramoss = Request.Form["tramosSwitch"].Contains("true");
            model.Estatus = switchTramoss ? 1 : 0;
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {


                _catTramosService.EditarTramo(model);
                var TramosModel = _catTramosService.ObtenerTramos((int)corp);
                return Json(TramosModel);
            }
            return PartialView("_Editar");
        }

        public JsonResult GetTra([DataSourceRequest] DataSourceRequest request, int? idDependencia)
        {
            var corp = idDependencia.HasValue ? Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value) : 0;
            if (idDependencia.HasValue)
            {
                corp = idDependencia.Value;
            }
            else
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);

            }
            var ListtramosModel = _catTramosService.ObtenerTramos(corp);

            return Json(ListtramosModel.ToDataSourceResult(request));
        }

        public JsonResult DelegacionesOficinas_Drop()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var tipo = corp < 3 ? 1 : corp;
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos().Where(x=>x.Transito == tipo), "IdDelegacion", "Delegacion");
            return Json(result);
        }

        [HttpGet]
        public ActionResult ajax_BuscarTramos(int idCarreteraFiltro, int idDelegacionFiltro)
        {
            List<CatTramosModel> ListTramos = new List<CatTramosModel>();
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;


            ListTramos = (from tramos in _catTramosService.ObtenerTramos(corp).ToList()
                          join carretera in _catCarreterasService.ObtenerCarreteras()
                            on tramos.IdCarretera equals carretera.IdCarretera

                          select new CatTramosModel
                                      {
                                          IdTramo = tramos.IdTramo,
                                          Tramo = tramos.Tramo,
                                          Carretera = tramos.Carretera,
                                          IdCarretera = tramos.IdCarretera,
                                          IdDelegacion = carretera.idOficinaTransporte,
                                          estatusDesc = tramos.estatusDesc,

                                        // = diasInhabiles.Municipio
                                      }).ToList();

            if (idCarreteraFiltro > 0 && idDelegacionFiltro > 0)
            {
                ListTramos = ListTramos.Where(s =>
                    s.IdCarretera == idCarreteraFiltro && s.IdDelegacion == idDelegacionFiltro
                ).ToList();
            }
            else if (idCarreteraFiltro > 0)
            {
                ListTramos = ListTramos.Where(s =>
                    s.IdCarretera == idCarreteraFiltro
                ).ToList();
            }
            else if (idDelegacionFiltro > 0)
            {
                ListTramos = ListTramos.Where(s =>
                    s.IdDelegacion == idDelegacionFiltro
                ).ToList();
            }

            return Json(ListTramos);
        }
    }
}
