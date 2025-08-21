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
    public class CatAutoridadesDisposicionController : BaseController
    {
        private readonly ICatAutoridadesDisposicionService _catAutoridadesDisposicionservice;

        public CatAutoridadesDisposicionController(ICatAutoridadesDisposicionService catAutoridadesDisposicionservice)
        {
            _catAutoridadesDisposicionservice = catAutoridadesDisposicionservice;
        }
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
                

			var ListAutoridadesDisposicionModel = _catAutoridadesDisposicionservice.ObtenerAutoridadesActivas(corp);

            return View(ListAutoridadesDisposicionModel);
            }
 


        [HttpPost]
        public IActionResult Agregar(CatAutoridadesDisposicionModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadDisposicion");
            if (ModelState.IsValid)
            {
                //Crear el producto
              
                _catAutoridadesDisposicionservice.GuardarAutoridad(model,(int)corp);
                var ListAutoridadesDisposicionModel = _catAutoridadesDisposicionservice.ObtenerAutoridadesActivas((int)corp);

                return Json(ListAutoridadesDisposicionModel);
            }
            return View("_Agregar");
        }


        [HttpGet]
        public IActionResult Editar(int IdAutoridadDisposicion)
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var autoridadesDisposicionModel = _catAutoridadesDisposicionservice.GetAutoridadesByID(IdAutoridadDisposicion, corp);
            return View(autoridadesDisposicionModel);
        }


        [HttpPost]
        public ActionResult Update(CatAutoridadesDisposicionModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            bool switchAutoridadesDisposicion = Request.Form["autoridadesDisposicionSwitch"].Contains("true");
            model.Estatus = switchAutoridadesDisposicion ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadDisposicion");
            if (ModelState.IsValid)
            {


				_catAutoridadesDisposicionservice. UpdateAutoridad(model,(int) corp);
                var ListAutoridadesDisposicionModel = _catAutoridadesDisposicionservice.ObtenerAutoridadesActivas((int)corp);
                return Json(ListAutoridadesDisposicionModel);
            }

            return PartialView("_Editar");
        }

        [HttpGet]
        public IActionResult Eliminar(int IdAutoridadDisposicion)
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var autoridadesDisposicionModel = _catAutoridadesDisposicionservice.GetAutoridadesByID(IdAutoridadDisposicion, corp);
            return View(autoridadesDisposicionModel);
        }


        [HttpPost]
        public IActionResult Eliminar(CatAutoridadesDisposicionModel autoridadesDisposicionModel)
        {
            ModelState.Remove("AutoridadDisposicion");
            if (ModelState.IsValid)
            {
                //Modificiacion del registro
               // EliminarAutoridadDisp(autoridadesDisposicionModel);
                return RedirectToAction("Index");
            }
            return View("Delete");
        }



        ///Crear metodo de update (post)


        #region Modal Action
       

        [HttpPost]
        public ActionResult AgregarAutoridadDisposicionPacial()
        {
        
                //SetDDLDependencias();
                return PartialView("_Crear");
            }
   

        public ActionResult EditarAutoridadDisposicionParcial(int IdAutoridadDisposicion)
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var autoridadesDisposicionModel = _catAutoridadesDisposicionservice.GetAutoridadesByID(IdAutoridadDisposicion, corp);
            return PartialView("_Editar", autoridadesDisposicionModel);
            }
  

        public ActionResult EliminarAutoridadDisposicionParcial(int IdAutoridadDisposicion)
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var autoridadesDisposicionModel = _catAutoridadesDisposicionservice.GetAutoridadesByID(IdAutoridadDisposicion, corp);
            return PartialView("_Eliminar", autoridadesDisposicionModel);
        }







        [HttpPost]
        public ActionResult CrearAutoridadDisposicionParcialModal(CatAutoridadesDisposicionModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadDisposicion");
            if (ModelState.IsValid)
            {

                
                _catAutoridadesDisposicionservice.GuardarAutoridad(model, (int)corp);
                var ListAutoridadesDisposicionModel = _catAutoridadesDisposicionservice.ObtenerAutoridadesActivas((int)corp);
                return PartialView("_ListaAutoridadesDisposicion", ListAutoridadesDisposicionModel);
            }

            return PartialView("_Crear");
        }

        public JsonResult GetAutDisp([DataSourceRequest] DataSourceRequest request, int? idDependencia)
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
            var ListAutoridadesDisposicionModel = _catAutoridadesDisposicionservice.ObtenerAutoridadesActivas(corp);

            return Json(ListAutoridadesDisposicionModel.ToDataSourceResult(request));
        }




        #endregion

    }
}
