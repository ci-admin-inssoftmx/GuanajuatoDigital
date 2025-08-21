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
    public class CatAutoridadesEntregaController : BaseController
    {
        private readonly ICatAutoridadesEntregaService _catAutoridadesEntregaService;

        public CatAutoridadesEntregaController(ICatAutoridadesEntregaService catAutoridadesEntregaService)
        {
            _catAutoridadesEntregaService = catAutoridadesEntregaService;
        }
        public IActionResult Index()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var ListAutoridadesEntregaModel = _catAutoridadesEntregaService.ObtenerAutoridadesActivas(corp);

            return View(ListAutoridadesEntregaModel);
            }
   




        #region Modal Action
        public ActionResult IndexModal()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var ListAutoridadesEntregaModel = _catAutoridadesEntregaService.ObtenerAutoridadesActivas(corp);
            return View("Index", ListAutoridadesEntregaModel);
        }

        [HttpPost]
        public ActionResult AgregarAutoridadEntregaModal()
        {

                return PartialView("_Crear");
            }
     

        public ActionResult EditarAutoridadEntregaModal(int IdAutoridadEntrega)
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var autoridadesEntregaModel = _catAutoridadesEntregaService.GetAutoridadesByID(IdAutoridadEntrega,corp);
                return PartialView("_Editar", autoridadesEntregaModel);
            }
  

        public ActionResult EliminarAutoridadEntregaModal(int IdAutoridadEntrega)
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var autoridadesEntregaModel = _catAutoridadesEntregaService.GetAutoridadesByID(IdAutoridadEntrega, corp);
            return PartialView("_Eliminar", autoridadesEntregaModel);
        }



        [HttpPost]
        public ActionResult AgregarAutoridadEntrega(CatAutoridadesEntregaModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadEntrega");
            if (ModelState.IsValid)
            {

                _catAutoridadesEntregaService.GuardarAutoridad(model,(int) corp);
                var ListAutoridadesEntregaModel = _catAutoridadesEntregaService.ObtenerAutoridadesActivas((int)corp);
                return PartialView("_ListaAutoridadesEntrega", ListAutoridadesEntregaModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarAutoridadEntregaMod(CatAutoridadesEntregaModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            bool switchAutEntrega = Request.Form["autEntregaSwitch"].Contains("true");
            model.Estatus = switchAutEntrega ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadEntrega");
            if (ModelState.IsValid)
            {

				_catAutoridadesEntregaService.UpdateAutoridad(model, (int)corp);
                var ListAutoridadesEntregaModel = _catAutoridadesEntregaService.ObtenerAutoridadesActivas((int)corp);
                return PartialView("_ListaAutoridadesEntrega", ListAutoridadesEntregaModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Editar");
        }

      /*  public ActionResult EliminarAutoridadEntregaMod(CatAutoridadesEntregaModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadEntrega");
            if (ModelState.IsValid)
            {


                EliminaAutoridadEntrega(model);
                var ListAutoridadesEntregaModel = GetAutoridadesEntrega();
                return PartialView("_ListaAutoridadesEntrega", ListAutoridadesEntregaModel);
            }
            return PartialView("_Eliminar");
        }*/
        public JsonResult GetAutEntrega([DataSourceRequest] DataSourceRequest request, int? idDependencia)
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
            var ListAutoridadesEntregaModel = _catAutoridadesEntregaService.ObtenerAutoridadesActivas(corp);

            return Json(ListAutoridadesEntregaModel.ToDataSourceResult(request));
        }




        #endregion

     
    }
}
