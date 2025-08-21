using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class CatClasificacionAccidentesController : BaseController
    {
        private readonly ICatClasificacionAccidentes _clasificacionAccidentesService;


        public CatClasificacionAccidentesController(ICatClasificacionAccidentes clasificacionAccidentesService)
        {
            _clasificacionAccidentesService = clasificacionAccidentesService;
        }

        public IActionResult Index()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var ListClasificacionAccidentesModel = _clasificacionAccidentesService.GetClasificacionAccidentes(corp);

            return View(ListClasificacionAccidentesModel);
        }

        public IActionResult OntenerParaDDL()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var ListClasificacionAccidentesModel = _clasificacionAccidentesService.ObtenerClasificacionesActivas(corp);

            return View(ListClasificacionAccidentesModel);

        }





        #region Modal Action
        public ActionResult IndexModal()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var ListClasificacionAccidentesModel = _clasificacionAccidentesService.GetClasificacionAccidentes(corp);
            return View("Index", ListClasificacionAccidentesModel);
        }

        [HttpPost]
        public ActionResult AgregarClasificacionAccidenteModal()
        {
    
                return PartialView("_Crear");
            }
            
    

        public ActionResult EditarClasificacionAccidenteModal(int IdClasificacionAccidente)
        {
      
                var clasificacionAccidentesModel = _clasificacionAccidentesService.GetClasificacionAccidenteByID(IdClasificacionAccidente);
            return PartialView("_Editar", clasificacionAccidentesModel);
            }
    

        public ActionResult EliminarClasificacionAccidenteModal(int IdClasificacionAccidente)
        {
            var clasificacionAccidentesModel = _clasificacionAccidentesService.GetClasificacionAccidenteByID(IdClasificacionAccidente);
            return PartialView("_Eliminar", clasificacionAccidentesModel);
        }



        [HttpPost]
        public ActionResult AgregarClasificacionAccidente(CatClasificacionAccidentesModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {
                

                _clasificacionAccidentesService.CrearClasificacionAccidente(model,(int)corp);
                var ListClasificacionAccidentesModel = _clasificacionAccidentesService.GetClasificacionAccidentes((int)corp);
                return Json(ListClasificacionAccidentesModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarClasificacionAccidenteMod(CatClasificacionAccidentesModel model)
        {
            bool switchClasificacion = Request.Form["clasificacionAccidentesSwitch"].Contains("true");
            model.Estatus = switchClasificacion ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreClasificacion");
            if (ModelState.IsValid)
            {

				var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

				_clasificacionAccidentesService.EditarClasificacionAccidente(model);
                var ListClasificacionAccidentesModel = _clasificacionAccidentesService.GetClasificacionAccidentes(corp);
                return Json(ListClasificacionAccidentesModel);
            }
            return PartialView("_Editar");
        }

        public JsonResult GetClasAccidentes([DataSourceRequest] DataSourceRequest request, int? idDependencia)
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

			var ListClasificacionAccidentesModel = _clasificacionAccidentesService.ObtenerClasificacionesActivas(corp);

            return Json(ListClasificacionAccidentesModel.ToDataSourceResult(request));
        }
    }
}




#endregion

