using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
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
using System.Drawing;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class ColoresController : BaseController
    {
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
       
            //var ListColoresModel = GetColores();
            return View();
        }
       

        /// <summary>
        /// Accion que redirige a la vista
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            SetDDLColores();
            return View();
        }





        #region Modal Action
        public ActionResult IndexModal()
        {
            //var ListColoresModel = GetColores();
            //return View("IndexModal");
            return View("IndexModal");
        }

        [HttpPost]
        public ActionResult AgregarPacial()
        {
           
                return PartialView("_Crear");
            }
     

        public ActionResult EditarParcial(int Id)
        {
          
            var coloresModel = GetColorByID(Id);
            coloresModel.ValorEstatusColores = (coloresModel.Estatus == 1) ? true : false;
            return View("_Editar", coloresModel);
        }


        public ActionResult EliminarColorParcial(int IdColor)
        {
            var coloresModel = GetColorByID(IdColor);
            return View("_Eliminar", coloresModel);
        }

        public JsonResult Categories_Read()
        {
            var result = new SelectList(dbContext.Colores.ToList(), "IdColor", "color");
            return Json(result);
        }




        [HttpPost]
        public ActionResult CreatePartialModal(ColoresModel model)
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

			var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("color");
           

                CreateColor(model);
                var ListColoresModel = GetColores((int)corp);
                return Json(ListColoresModel);
            
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Crear");
        }

        public ActionResult UpdatePartialModal(ColoresModel model)
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

            bool switchColores = Request.Form["coloresSwitch"].Contains("true");
            model.Estatus = switchColores ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("idColor");
            if (ModelState.IsValid)
            {


                UpdateColor(model);
                var ListColoresModel = GetColores((int)corp);
                return Json(ListColoresModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Editar");
        }

       /* public ActionResult EliminarPartialModal(ColoresModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("color");
            if (ModelState.IsValid)
            {


                DeleteColor(model);
                var ListColoresModel = GetColores();
                return Json(ListColoresModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Eliminar");
        }*/
        public JsonResult GetCols([DataSourceRequest] DataSourceRequest request, int? idDependencia)
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

          /*  if (corp > 3)
            {
                ListCarreterasModel = _catCarreterasService.ObtenerCarreteras(corp);
            }
            else
            {
                ListCarreterasModel = _catCarreterasService.ObtenerCarreteras();
            }
          */
            var ListColoresModel = GetColores(corp);

            return Json(ListColoresModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CreateColor(ColoresModel model)
        {

			var corp = model.Corp;
			var corporation = corp < 2 ? 1 : corp;

			CatColores color = new CatColores();
            color.IdColor = model.IdColor;
            color.color = model.color;
            color.Estatus = 1;
            color.transito = corporation;
            color.FechaActualizacion = DateTime.Now;
            dbContext.Colores.Add(color);
            dbContext.SaveChanges();
        }

        public void UpdateColor(ColoresModel model)
        {
			var corp = model.Corp;
			var corporation = corp < 2 ? 1 : corp;

			CatColores color = new CatColores();
            color.IdColor = model.IdColor;
            color.color = model.color;
            color.Estatus = model.Estatus;
            color.transito= corporation;
            color.FechaActualizacion = DateTime.Now;
            dbContext.Entry(color).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void DeleteColor(ColoresModel model)
        {
			var corp = 1;
			var corporation = corp < 2 ? 1 : corp;

			CatColores color = new CatColores();
            color.IdColor = model.IdColor;
            color.color = model.color;
            color.transito = corporation;
            color.Estatus = 0;
            color.FechaActualizacion = DateTime.Now;
            dbContext.Entry(color).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        private void SetDDLColores()
        {
			///Espacio en memoria de manera temporal que solo existe en la petición bool, list, string ,clases , selectlist
			var corp = 1;
			var corporation = corp < 2 ? 1 : corp;
			ViewBag.Categories = new SelectList(dbContext.Colores.Where(s=>s.transito==corporation).ToList(), "IdColor", "color");
        }


        public ColoresModel GetColorByID(int Id)
        {

            var productEnitity = dbContext.Colores.Find(Id);

            var colorModel = (from colores in dbContext.Colores.ToList()
                              select new ColoresModel

                              {
                                  IdColor = colores.IdColor,
                                  color = colores.color,
                                  Estatus = colores.Estatus,
                                  ValorEstatusColores = (colores.Estatus == 1) ? true : false

                              }).Where(w => w.IdColor == Id).FirstOrDefault();

            return colorModel;
        }

        public List<ColoresModel> GetColores(int corp)
        {
		
			var corporation = corp < 2 ? 1 : corp;
			var ListColoresModel = (from colores in dbContext.Colores.ToList()
                                    join estatus in dbContext.Estatus.ToList()
                                    on colores.Estatus equals estatus.estatus
                                    where colores.transito == corporation
                                    select new ColoresModel
                                    {
                                        IdColor = colores.IdColor,
                                        color = colores.color,
                                        estatusDesc = estatus.estatusDesc

                                    }).ToList();
            return ListColoresModel;
        }
        #endregion



    }
}
