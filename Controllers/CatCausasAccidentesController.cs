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
    public class CatCausasAccidentesController : BaseController
    {
        private readonly ICatCausasAccidentesService _catCausasAccidentesService;

        public CatCausasAccidentesController(ICatCausasAccidentesService catCausasAccidentesService)
        {
            _catCausasAccidentesService = catCausasAccidentesService;
        }
        DBContextInssoft dbContext = new  DBContextInssoft();
        public IActionResult Index()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var ListCausasAccidentesModel = _catCausasAccidentesService.ObtenerCausasActivas(corp);

            return View(ListCausasAccidentesModel);
            }
 




        #region Modal Action
        public ActionResult IndexModal()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var ListCausasAccidentesModel = _catCausasAccidentesService.ObtenerCausasActivas(corp);
            return View("Index", ListCausasAccidentesModel);
            
        }

        [HttpPost]
        public ActionResult AgregarCausasAccidenteModal()
        {
   
                return PartialView("_Crear");
            }
  

        public ActionResult EditarCausasAccidenteModal(int IdCausaAccidente)
        {
        
                var causasAccidentesModel = _catCausasAccidentesService.ObtenerCausaByID(IdCausaAccidente);
            return PartialView("_Editar", causasAccidentesModel);
            }
   

        public ActionResult EliminarCausasAccidenteModal(int IdCausaAccidente)
        {
            var causasAccidentesModel = _catCausasAccidentesService.ObtenerCausaByID(IdCausaAccidente);
            return PartialView("_Eliminar", causasAccidentesModel);
        }



        [HttpPost]
        public ActionResult AgregarCausaAccidente(CatCausasAccidentesModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("CausaAccidente");
            if (ModelState.IsValid)
            {

				_catCausasAccidentesService.CrearCausa(model,(int)corp);
                var ListCausasAccidentesModel = _catCausasAccidentesService.ObtenerCausasActivas((int)corp);
                return Json(ListCausasAccidentesModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarCausaAccidenteMod(CatCausasAccidentesModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            bool switchCausasAccidentes = Request.Form["causaAccidenteSwitch"].Contains("true");
            model.Estatus = switchCausasAccidentes ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("CausaAccidente");
            if (ModelState.IsValid)
            {


				_catCausasAccidentesService.EditarCausa(model);
                var ListCausasAccidentesModel = _catCausasAccidentesService.ObtenerCausasActivas((int)corp);
                return Json(ListCausasAccidentesModel);
            }
            return PartialView("_Editar");
        }

      
        public JsonResult GetCausas([DataSourceRequest] DataSourceRequest request, int? idDependencia)
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

            if (corp < 4) corp = 1;

            var ListCausasAccidentesModel = _catCausasAccidentesService.ObtenerCausasActivas(corp);

            return Json(ListCausasAccidentesModel.ToDataSourceResult(request));
        }




        #endregion


    }
}
