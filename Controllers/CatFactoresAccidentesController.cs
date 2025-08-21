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
    public class CatFactoresAccidentesController : BaseController
    {
        private readonly ICatFactoresAccidentesService _catFactoresAccidentesService;

        public CatFactoresAccidentesController(ICatFactoresAccidentesService catFactoresAccidentesService)
        {
            _catFactoresAccidentesService = catFactoresAccidentesService;
        }
        public IActionResult Index()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var ListFactoresAccidentesModel = _catFactoresAccidentesService.GetFactoresAccidentes(corp);

            return View(ListFactoresAccidentesModel);
        }




        #region Modal Action
        public ActionResult IndexModal()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var ListFactoresAccidentesModel = _catFactoresAccidentesService.GetFactoresAccidentes(corp);
            return View("Index", ListFactoresAccidentesModel);
        }

        [HttpPost]
        public ActionResult AgregarFactoresAccidenteModal()
        {
          
                return PartialView("_Crear");
            }
      

        public ActionResult EditarFactoresAccidenteModal(int IdFactorAccidente)
        {
          
                var factoresAccidentesModel = _catFactoresAccidentesService.GetFactorByID(IdFactorAccidente);
            return PartialView("_Editar", factoresAccidentesModel);
        }


        public ActionResult EliminarFactoresAccidenteModal(int IdFactorAccidente)
        {
            var factoresAccidentesModel = _catFactoresAccidentesService.GetFactorByID(IdFactorAccidente);
            return PartialView("_Eliminar", factoresAccidentesModel);
        }



        [HttpPost]
        public ActionResult AgregarFactornAccidente(CatFactoresAccidentesModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("FactorAccidente");
            if (ModelState.IsValid)
            {

               
                _catFactoresAccidentesService.GuardarFactor(model,(int)corp);
                var ListFactoresAccidentesModel = _catFactoresAccidentesService.GetFactoresAccidentes((int)corp);
                return Json(ListFactoresAccidentesModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarFactorAccidenteMod(CatFactoresAccidentesModel model)
        {
            bool switchFactores = Request.Form["factoresSwitch"].Contains("true");
            model.Estatus = switchFactores ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("FactorAccidente");
            if (ModelState.IsValid)
            {

				var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

				_catFactoresAccidentesService.UpdateFactor(model);
                var ListFactoresAccidentesModel = _catFactoresAccidentesService.GetFactoresAccidentes(corp);
                return Json(ListFactoresAccidentesModel);
            }
            return PartialView("_Editar");
        }

       
        public JsonResult GetFactores([DataSourceRequest] DataSourceRequest request, int? idDependencia)
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
            var ListFactoresAccidentesModel = _catFactoresAccidentesService.GetFactoresAccidentes(corp);

            return Json(ListFactoresAccidentesModel.ToDataSourceResult(request));
        }




        #endregion


       


    }
}
