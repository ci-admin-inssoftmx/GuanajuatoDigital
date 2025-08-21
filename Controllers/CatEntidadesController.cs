using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class CatEntidadesController : BaseController
    {

        private readonly ICatEntidadesService _catEntidadesService;

        public CatEntidadesController(ICatEntidadesService catEntidadesService)
        {
            _catEntidadesService = catEntidadesService;
        }
        public IActionResult Index()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var ListEntidadesModel = _catEntidadesService.ObtenerEntidades(corp);

            return View(ListEntidadesModel);
        }
        [HttpPost]
        public ActionResult MostrarModalAgregarEntidad()
        {
       
                return PartialView("_Crear");
            }
     

        public ActionResult EditarEntidadModal(int idEntidad)
        {
          
                var EntidadesModel = _catEntidadesService.ObtenerEntidadesByID(idEntidad);
            return PartialView("_Editar", EntidadesModel);
            }
    

        [HttpPost]
        public ActionResult AgregarEntidad(CatEntidadesModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }

            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("idEntidad");
            if (ModelState.IsValid)
            {

              
                _catEntidadesService.CrearEntidad(model,(int)corp);
                var ListEntidadesModel = _catEntidadesService.ObtenerEntidades((int)corp);
                return Json(ListEntidadesModel);
            }

            return PartialView("_Crear");
        
        
        }

        public ActionResult EditarEntidadBD(CatEntidadesModel model)
        {
            bool switchEntidades = Request.Form["entidadesSwitch"].Contains("true");
            model.estatus = switchEntidades ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {

				var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

				_catEntidadesService.EditarEntidad(model);
                var ListEntidadesModel = _catEntidadesService.ObtenerEntidades(corp);
                return Json(ListEntidadesModel);
            }
            return PartialView("_Editar");
        }
        public JsonResult GetEnt([DataSourceRequest] DataSourceRequest request,int?idDependencia)
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
            var ListEntidadesModel = _catEntidadesService.ObtenerEntidades(corp);

            return Json(ListEntidadesModel.ToDataSourceResult(request));
        }
    }
}
