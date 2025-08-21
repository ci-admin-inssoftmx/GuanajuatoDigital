using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class DependenciasController : BaseController
    {
        private readonly IDependencias _catDependencias;

        public DependenciasController(IDependencias catDependencias)
        {
            _catDependencias = catDependencias;
        }
        public IActionResult Index()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var ListDependenciasModel = _catDependencias.GetDependencias(corp);

            return View(ListDependenciasModel);
            }
       



        #region Modal Action
        public ActionResult IndexModal()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var ListDependenciasModel = _catDependencias.GetDependencias(corp);
            //return View("IndexModal");
            return View("Index", ListDependenciasModel);
        }

        [HttpPost]
        public ActionResult AgregarPacial()
        {
           
                return PartialView("_Crear");
        }
 

        [HttpPost]
        public ActionResult EditarParcial(int IdDependencia)
        {
        
                var dependenciasModel = _catDependencias.GetDependenciaById(IdDependencia);
            return PartialView("_Editar", dependenciasModel);
            }
       

        [HttpPost]
        public ActionResult EliminarParcial(int IdDependencia)
        {
            var dependenciasModel = _catDependencias.GetDependenciaById(IdDependencia);
            return PartialView("_Eliminar", dependenciasModel);
        }


        [HttpPost]
        public ActionResult CreatePartialModal(DependenciasModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreDependencia");
            if (ModelState.IsValid)
            {
				//Crear el producto
				_catDependencias.SaveDependencia(model,(int)corp);
                var ListDependenciasModel = _catDependencias.GetDependencias((int)corp);
                return Json(ListDependenciasModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Crear");
        }

        [HttpPost]
        public ActionResult UpdatePartialModal(DependenciasModel model)
        {
            bool switchDependencias = Request.Form["switchDependencias"].Contains("true");
            model.Estatus = switchDependencias ? 1 : 0;
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreDependencia");
            if (ModelState.IsValid)
            {
				_catDependencias.UpdateDependencia(model);
                var ListDependenciasModel = _catDependencias.GetDependencias((int)corp);
                return Json(ListDependenciasModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Editar");
        }

       
        public JsonResult GetDeps([DataSourceRequest] DataSourceRequest request, int? idDependencia)
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
            var ListProuctModel = _catDependencias.GetDependencias(corp);

            return Json(ListProuctModel.ToDataSourceResult(request));
        }




        #endregion

      
    }
}
