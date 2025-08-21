using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
//using Telerik.SvgIcons;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class CatCarreterasController : BaseController
    {
        private readonly ICatCarreterasService _catCarreterasService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;

        public CatCarreterasController(ICatCarreterasService catCarreterasService, ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService)
        {
            _catCarreterasService = catCarreterasService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;

        }
        public IActionResult Index()
        {
   
                var ListCarreterassModel = new List<CatCarreterasModel>();
                var corp =Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);

            if (corp>3)
                ListCarreterassModel = _catCarreterasService.ObtenerCarreteras(corp);
            else
            ListCarreterassModel = _catCarreterasService.ObtenerCarreteras();
            return View(ListCarreterassModel);
            }
  
        public JsonResult Delegaciones_Drop()
        {

            var ListDelegaciones = new List<CatDelegacionesOficinasTransporteModel>();
            ListDelegaciones = _catDelegacionesOficinasTransporteService.GetDelegacionesOficinas();

            var result = new SelectList(ListDelegaciones, "IdOficinaTransporte", "NombreOficina");
            return Json(result);
        }
        [HttpPost]
        public ActionResult MostrarModalAgregarCarretera()
        {
  
                return PartialView("_Crear");
            }


        public ActionResult EditarCarreteraModal(int IdCarretera)
        {
     
                var CarreterasModel = _catCarreterasService.ObtenerCarreteraByID(IdCarretera);
            return PartialView("_Editar", CarreterasModel);
            }
   

        [HttpPost]
        public ActionResult CrearCarreteraMod(CatCarreterasModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {
                var corp = model.Corp;

                if (corp == null)
                {
                    corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
                }
                if (corp > 3)
                    model.Corp = corp;
                else
                    model.Corp = 1;


                _catCarreterasService.CrearCarretera(model);
                var CarreterasModel = _catCarreterasService.ObtenerCarreteras();
                return Json(CarreterasModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarCarreteraBD(CatCarreterasModel model)
        {
            bool switchCarreteras = Request.Form["carreterasSwitch"].Contains("true");
            model.Estatus = switchCarreteras ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {
                _catCarreterasService.EditarCarretera(model);
                var ListCarreterasModel = _catCarreterasService.ObtenerCarreteras();
                return Json(ListCarreterasModel);
            }
            return PartialView("_Editar");
        }

      public JsonResult GetCarr([DataSourceRequest] DataSourceRequest request, int idDelegacion, int? idDependencia)
{
    var ListCarreterasModel = new List<CatCarreterasModel>();

    var corp = idDependencia.HasValue ? Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value) : 0;

    if (idDependencia.HasValue)
    {
        corp = idDependencia.Value;
    }
    else
    {
        corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);
    }

    if (corp > 3)
    {
        ListCarreterasModel = _catCarreterasService.ObtenerCarreteras(corp);
    }
    else
    {
        ListCarreterasModel = _catCarreterasService.ObtenerCarreteras();
    }

    if (idDelegacion != 0)
    {
        ListCarreterasModel = ListCarreterasModel.Where(s => s.idOficinaTransporte == idDelegacion).ToList();
    }

    return Json(ListCarreterasModel.ToDataSourceResult(request));
}

        public JsonResult CarreterasPorDelegacion_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_catCarreterasService.GetCarreterasPorDelegacion(idOficina), "IdCarretera", "Carretera");
            return Json(result);
        }

		public JsonResult CarreterasPorDelegacion_DropTodos()
		{
			int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

			var result = new SelectList(_catCarreterasService.GetCarreterasPorDelegacionTodos(idOficina), "IdCarretera", "Carretera");
			return Json(result);
		}

		public JsonResult Carreteras_Drop()
        {

            var result = new SelectList(_catCarreterasService.ObtenerCarreteras(), "IdCarretera", "Carretera");
            return Json(result);
        }

        [HttpGet]
        public ActionResult ajax_BuscarCarreteras(int idDelegacionFiltro)
        {
            List<CatCarreterasModel> ListAgencias = new List<CatCarreterasModel>();


            ListAgencias = (from catCarreteras in _catCarreterasService.ObtenerCarreteras().ToList()
                                //join municipio in _catMunicipiosService.GetMunicipios().ToList()
                                //on diasInhabiles.idMunicipio equals municipio.IdMunicipio
                                // join estatus in dbContext.Estatus.ToList()
                                //on diasInhabiles.Estatus equals estatus.estatus

                            select new CatCarreterasModel
                            {
                                IdCarretera = catCarreteras.IdCarretera,
                                Carretera = catCarreteras.Carretera,
                                idOficinaTransporte = catCarreteras.idOficinaTransporte,
                                nombreOficina = catCarreteras.nombreOficina,
                                Estatus = catCarreteras.Estatus,
                                estatusDesc = catCarreteras.estatusDesc,
                                // EstatusDesc = estatus.estatusDesc,
                            }).ToList();


            if (idDelegacionFiltro > 0)
            {
                ListAgencias = (from s in ListAgencias
                                where s.idOficinaTransporte == idDelegacionFiltro
                                select s).ToList();
            }

            return Json(ListAgencias);
        }
    }
}
