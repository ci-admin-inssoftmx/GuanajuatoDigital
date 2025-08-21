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
    public class CatMarcasVehiculosController : BaseController
    {
        private readonly ICatMarcasVehiculosService _catMarcasVehiculosService;

        public CatMarcasVehiculosController(ICatMarcasVehiculosService catMarcasVehiculosService)
        {
            _catMarcasVehiculosService = catMarcasVehiculosService;
        }
        public IActionResult Index()
		{
            var corp = 1;

			var ListMarcasModel = _catMarcasVehiculosService.ObtenerMarcasTodas(corp);
            return View(ListMarcasModel);
         }      
        [HttpPost]
        public ActionResult AgregarPacial()
        {         
                return PartialView("_Crear");
            }
   

        [HttpPost]
        public ActionResult EditarParcial(int IdMarcaVehiculo)
        {
       
                var marcasVehiculosModel = _catMarcasVehiculosService.GetMarcaByID(IdMarcaVehiculo);
            return PartialView("_Editar", marcasVehiculosModel);
        }

        [HttpPost]
        public ActionResult EliminarMarcaParcial(int IdMarcaVehiculo)
        {
            var marcasVehiculosModel = _catMarcasVehiculosService.GetMarcaByID(IdMarcaVehiculo);
            return View("_Eliminar", marcasVehiculosModel);
        }

        [HttpPost]
        public IActionResult GetUpdate(int IdMarcaVehiculo)
        {
            var marcaVehiculoModel = _catMarcasVehiculosService.GetMarcaByID(IdMarcaVehiculo);
            return View(marcaVehiculoModel);
        }


        public JsonResult Categories_Read()
        {
            var corp = 1;

			var result = new SelectList(_catMarcasVehiculosService.ObtenerMarcas(corp), "IdMarcaVehiculo", "MarcaVehiculo");
            return Json(result);
        }



        [HttpPost]
        public ActionResult CreatePartialModal(CatMarcasVehiculosModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("MarcaVehiculo");
            if (ModelState.IsValid)
            {
              

                _catMarcasVehiculosService.GuardarMarca(model,(int)corp);
                var ListMarcasModel = _catMarcasVehiculosService.ObtenerMarcasTodas((int)corp);
                return Json(ListMarcasModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Crear");
        }

        public ActionResult UpdatePartialModal(CatMarcasVehiculosModel model)
        {
            bool switchMarcas = Request.Form["MarcasSwitch"].Contains("true");
            model.Estatus = switchMarcas ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("MarcaVehiculo");
            if (ModelState.IsValid)
            {
                //Crear el producto
                var corp = 1;

				_catMarcasVehiculosService.UpdateMarca(model);
                var ListMarcasModel = _catMarcasVehiculosService.ObtenerMarcasTodas(corp);
                return Json(ListMarcasModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Editar");
        }



        public JsonResult GetMarca2([DataSourceRequest] DataSourceRequest request,int? idDependencia)
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
            var ListMarcasModel = _catMarcasVehiculosService.ObtenerMarcasTodas(corp);

            return Json(ListMarcasModel.ToDataSourceResult(request));
        }

    }
}
