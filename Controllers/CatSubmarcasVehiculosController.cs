using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;


namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class CatSubmarcasVehiculosController : BaseController
    {
        private readonly ICatSubmarcasVehiculosService _catSubmarcasVehiculosService;
        private readonly ICatMarcasVehiculosService _catMarcasVehiculosService;

        public CatSubmarcasVehiculosController(ICatSubmarcasVehiculosService catSubmarcasVehiculosService, ICatMarcasVehiculosService catMarcasVehiculosService)
        {
            _catSubmarcasVehiculosService = catSubmarcasVehiculosService;
            _catMarcasVehiculosService = catMarcasVehiculosService;
        }
       
        public IActionResult Index()
        {
            var corp = 1;

            var ListSubmarcasModel = _catSubmarcasVehiculosService.ObtenerSubarcas(corp);
            return View(ListSubmarcasModel);
         }
    


        [HttpPost]
        public ActionResult AgregarSubmarcaParcial()
        {
    
                Marcas_Drop();
            return PartialView("_Crear");
            }
    

        public ActionResult EditarSubmarcaParcial(int IdSubmarca)
        {   
            Marcas_Drop();
            var submarcasModel = _catSubmarcasVehiculosService.GetSubmarcaByID(IdSubmarca);
            return View("_Editar", submarcasModel);
        }


        public ActionResult EliminarSubmarcaParcial(int IdSubmarca)
        {
            Marcas_Drop();
            var submarcasModel = _catSubmarcasVehiculosService.GetSubmarcaByID(IdSubmarca);
            return View("_Eliminar", submarcasModel);
        }

        public JsonResult Categories_Read()
        {
            var corp = 1;

            var result = new SelectList(_catSubmarcasVehiculosService.ObtenerSubarcas(corp), "IdSubmarca", "NombreSubmarca");
            return Json(result);
        }



        [HttpPost]
        public ActionResult CreatePartialModal(CatSubmarcasVehiculosModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            ModelState.Remove("NombreSubmarca");
            if (ModelState.IsValid)
            {
                //Crear el producto

                var can = _catSubmarcasVehiculosService.ValidarExistenciaSubmarca(model.IdMarcaVehiculo.Value, model.NombreSubmarca,(int)corp);

                if(can) {
					_catSubmarcasVehiculosService.GuardarSubmarca(model,(int)corp);
				}
            }

			var ListSubmarcasModel2 = _catSubmarcasVehiculosService.ObtenerSubarcas((int)corp);
			return Json(ListSubmarcasModel2);

		}

        [HttpPost]
        public ActionResult CrearSubMarcaCatalogo(CatSubmarcasVehiculosModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            ModelState.Remove("NombreSubmarca");
            if (ModelState.IsValid)
            {
               
                    _catSubmarcasVehiculosService.GuardarSubmarca(model, (int)corp);              
            }

            var ListSubmarcasModel2 = _catSubmarcasVehiculosService.ObtenerSubarcas((int)corp);
            return Json(ListSubmarcasModel2);

        }

        [HttpPost]
        public ActionResult EditarSubmarca(CatSubmarcasVehiculosModel model)
        {
            bool switchSubmarcas = Request.Form["submarcasSwitch"].Contains("true");
            var corp = 1;

            model.Estatus = switchSubmarcas ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreSubmarca");
            if (ModelState.IsValid)
            {
                //Crear el producto

                _catSubmarcasVehiculosService.UpdateSubmarca(model);
                var ListSubmarcasModel = _catSubmarcasVehiculosService.ObtenerSubarcas(corp);
                return Json(ListSubmarcasModel);
            }
            Marcas_Drop();
            //return View("Create");
            return PartialView("_Editar");
        }

   


        public JsonResult GetSubs([DataSourceRequest] DataSourceRequest request, int idMarca,int? idDependencia)
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
            var ListSubmarcasModel = _catSubmarcasVehiculosService.ObtenerSubarcas(corp);
           if(idMarca!=0)
            ListSubmarcasModel = ListSubmarcasModel.Where(s => s.IdMarcaVehiculo == idMarca).ToList();
            return Json(ListSubmarcasModel.ToDataSourceResult(request));
        }

      

        public JsonResult Marcas_Drop()
        {
			var corp = 1;

			var result = new SelectList(_catMarcasVehiculosService.ObtenerMarcas(corp), "IdMarcaVehiculo", "MarcaVehiculo");
            return Json(result);
        }

        [HttpGet]
        public ActionResult ajax_BuscarPorMarca(int idMarcaFiltro)
        {
            List<CatSubmarcasVehiculosModel> ListAgencias = new List<CatSubmarcasVehiculosModel>();
            var corp = 1;


            ListAgencias = (from catSubmarcasVehiculos in _catSubmarcasVehiculosService.ObtenerSubarcas(corp).ToList()
                                //join municipio in _catMunicipiosService.GetMunicipios().ToList()
                                //on diasInhabiles.idMunicipio equals municipio.IdMunicipio
                                // join estatus in dbContext.Estatus.ToList()
                                //on diasInhabiles.Estatus equals estatus.estatus

                            select new CatSubmarcasVehiculosModel
                            {
                                IdSubmarca = catSubmarcasVehiculos.IdSubmarca,
                                NombreSubmarca = catSubmarcasVehiculos.NombreSubmarca,
                                IdMarcaVehiculo = catSubmarcasVehiculos.IdMarcaVehiculo,
                                MarcaVehiculo = catSubmarcasVehiculos.MarcaVehiculo,
                                Estatus = catSubmarcasVehiculos.Estatus,
                                estatusDesc = catSubmarcasVehiculos.estatusDesc,
                                // EstatusDesc = estatus.estatusDesc,
                            }).ToList();


            if (idMarcaFiltro > 0)
            {
                ListAgencias = (from s in ListAgencias
                                where s.IdMarcaVehiculo == idMarcaFiltro
                                select s).ToList();
            }

            return Json(ListAgencias);
        }

    }
}
