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
    public class CatFactoresOpcionesAccidentesController : BaseController
    {
        private readonly ICatFactoresOpcionesAccidentesService _catFactoresOpcionesAccidentesService;

        public CatFactoresOpcionesAccidentesController(ICatFactoresOpcionesAccidentesService catFactoresOpcionesAccidentesService)
        {
            _catFactoresOpcionesAccidentesService = catFactoresOpcionesAccidentesService;
        }
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {

			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var ListFactoresOpcionesAccidentesModel = _catFactoresOpcionesAccidentesService.ObtenerOpcionesFactorAccidente(corp);

            return View(ListFactoresOpcionesAccidentesModel);
            }
      




        #region Modal Action
        public ActionResult IndexModal()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var ListFactoresOpcionesAccidentesModel = _catFactoresOpcionesAccidentesService.ObtenerOpcionesFactorAccidente(corp);
            return View("Index", ListFactoresOpcionesAccidentesModel);
        }

        [HttpPost]
        public ActionResult AgregarFactoresOpcionesAccidenteModal()
        {
          
                //Factores_Drop();
            return PartialView("_Crear");
            }
   

        public ActionResult EditarFactoresOpcionesAccidenteModal(int IdFactoropcionAccidente,int? Corp)
        {
            var corp = Corp;

            if (Corp.HasValue)
            {
                corp = Corp.Value;
            }
            else
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);
            }
            //Factores_Drop();
            var factoresOpcionesAccidentesModel = _catFactoresOpcionesAccidentesService.ObtenerOpcionesParaEditar(IdFactoropcionAccidente,(int)corp);
            return PartialView("_Editar", factoresOpcionesAccidentesModel);
        }


        public ActionResult EliminarFactoresOpcionesAccidenteModal(int IdFactoropcionAccidente)
        {
           // Factores_Drop();
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var factoresOpcionesAccidentesModel = _catFactoresOpcionesAccidentesService.ObtenerOpcionesParaEditar(IdFactoropcionAccidente,corp);
            return PartialView("_Eliminar", factoresOpcionesAccidentesModel);
        }



        [HttpPost]
        public ActionResult AgregarFactorOpcionAccidente(CatFactoresOpcionesAccidentesModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("FactorOpcionAccidente");
            if (ModelState.IsValid)
            {


				_catFactoresOpcionesAccidentesService.CrearOpcionFactor(model,(int) corp);
                var ListFactoresOpcionesAccidentesModel = _catFactoresOpcionesAccidentesService.ObtenerOpcionesFactorAccidente((int)corp);
                return Json(ListFactoresOpcionesAccidentesModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarFactorOpcionAccidenteMod(CatFactoresOpcionesAccidentesModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            bool switchFactoresOpciones = Request.Form["factoresOpcionesSwitch"].Contains("true");
            model.Estatus = switchFactoresOpciones ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("FactorOpcionAccidente");
            if (ModelState.IsValid)
            {


				_catFactoresOpcionesAccidentesService.EditarOpcionFactor(model);
                var ListFactoresOpcionesAccidentesModel = _catFactoresOpcionesAccidentesService.ObtenerOpcionesFactorAccidente((int)corp);
                return Json(ListFactoresOpcionesAccidentesModel);
            }
            return PartialView("_Editar");
        }

        public ActionResult EliminarFactorOpcionAccidenteMod(CatFactoresOpcionesAccidentesModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("FactorOpcionAccidente");
            if (ModelState.IsValid)
            {

				var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

				EliminaFactorOpcionAccidente(model);
                var ListFactoresAccidentesModel = _catFactoresOpcionesAccidentesService.ObtenerOpcionesFactorAccidente(corp);
                return Json(ListFactoresAccidentesModel);
            }
            return PartialView("_Eliminar");
        }
        public JsonResult GetFactoresOpciones([DataSourceRequest] DataSourceRequest request,int? idFactor, int? idDependencia)
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
            var ListFactoresOpcionesAccidentesModel = _catFactoresOpcionesAccidentesService.ObtenerOpcionesFactorAccidente(corp);

            if(idFactor != null)
            {
                ListFactoresOpcionesAccidentesModel = ListFactoresOpcionesAccidentesModel.Where(s => s.IdFactorAccidente == idFactor).ToList();
            }


            return Json(ListFactoresOpcionesAccidentesModel.ToDataSourceResult(request));
        }

       
        public JsonResult Factores_Drop(int? idDependencia)
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
            var corporation = corp < 2 ? 1 : corp;
			var result = new SelectList(dbContext.CatFactoresAccidentes.Where(s=>s.transito == corporation).ToList(), "IdFactorAccidente", "FactorAccidente");
            return Json(result);
        }


        #endregion


        #region Acciones a base de datos

        public void CrearFactorOpcionAccidente(CatFactoresOpcionesAccidentesModel model)
        {
            CatFactoresOpcionesAccidentes factorOpcion = new CatFactoresOpcionesAccidentes();
            factorOpcion.IdFactorOpcionAccidente = model.IdFactorOpcionAccidente;
            factorOpcion.FactorOpcionAccidente = model.FactorOpcionAccidente;
            factorOpcion.IdFactorAccidente = model.IdFactorAccidente;
            factorOpcion.Estatus = 1;
            factorOpcion.FechaActualizacion = DateTime.Now;
            dbContext.CatFactoresOpcionesAccidentes.Add(factorOpcion);
            dbContext.SaveChanges();
        }

        public void EditarFactorOpcionAccidente(CatFactoresOpcionesAccidentesModel model)
        {
            CatFactoresOpcionesAccidentes factorOpcion = new CatFactoresOpcionesAccidentes();
            factorOpcion.IdFactorOpcionAccidente = model.IdFactorOpcionAccidente;
            factorOpcion.FactorOpcionAccidente = model.FactorOpcionAccidente;
            factorOpcion.IdFactorAccidente = model.IdFactorAccidente;
            factorOpcion.Estatus = model.Estatus;
            factorOpcion.FechaActualizacion = DateTime.Now;
            dbContext.Entry(factorOpcion).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void EliminaFactorOpcionAccidente(CatFactoresOpcionesAccidentesModel model)
        {

            CatFactoresOpcionesAccidentes factorOpcion = new CatFactoresOpcionesAccidentes();
            factorOpcion.IdFactorOpcionAccidente = model.IdFactorOpcionAccidente;
            factorOpcion.FactorOpcionAccidente = model.FactorOpcionAccidente;
            factorOpcion.IdFactorAccidente = model.IdFactorAccidente;
            factorOpcion.Estatus = 0;
            factorOpcion.FechaActualizacion = DateTime.Now;
            dbContext.Entry(factorOpcion).State = EntityState.Modified;
            dbContext.SaveChanges();

        }




        public CatFactoresOpcionesAccidentesModel GetFactorOpcionAccidenteByID(int IdFactorOpcionAccidente)
        {

            var productEnitity = dbContext.CatFactoresOpcionesAccidentes.Find(IdFactorOpcionAccidente);

            var factoresOpcionesAccidentesModel = (from catFactoresOpcionesAccidentes in dbContext.CatFactoresOpcionesAccidentes.ToList()
                                                   select new CatFactoresOpcionesAccidentesModel

                                                   {
                                                       IdFactorOpcionAccidente = catFactoresOpcionesAccidentes.IdFactorOpcionAccidente,
                                                       FactorOpcionAccidente = catFactoresOpcionesAccidentes.FactorOpcionAccidente,
                                                       IdFactorAccidente = catFactoresOpcionesAccidentes.IdFactorAccidente,
                                                       Estatus = catFactoresOpcionesAccidentes.Estatus,


                                                   }).Where(w => w.IdFactorOpcionAccidente == IdFactorOpcionAccidente).FirstOrDefault();

            return factoresOpcionesAccidentesModel;
        }


        public List<CatFactoresOpcionesAccidentesModel> GetFactoresOpcionesAccidentes()
        {
            var ListFactoresOpcionesAccidentesModel = (from catFactoresOpcionesAccidentes in dbContext.CatFactoresOpcionesAccidentes.ToList()
                                                       join catFactoresAccidentes in dbContext.CatFactoresAccidentes.ToList()
                                                       on catFactoresOpcionesAccidentes.IdFactorAccidente equals catFactoresAccidentes.IdFactorAccidente
                                                       join estatus in dbContext.Estatus.ToList()
                                                       on catFactoresAccidentes.Estatus equals estatus.estatus
                                                       select new CatFactoresOpcionesAccidentesModel
                                                       {
                                                           IdFactorOpcionAccidente = catFactoresOpcionesAccidentes.IdFactorOpcionAccidente,
                                                           FactorOpcionAccidente = catFactoresOpcionesAccidentes.FactorOpcionAccidente,
                                                           IdFactorAccidente = catFactoresOpcionesAccidentes.IdFactorAccidente,
                                                           FactorAccidente = catFactoresAccidentes.FactorAccidente,
                                                           Estatus = catFactoresOpcionesAccidentes.Estatus,
                                                           estatusDesc = estatus.estatusDesc,

                                                       }).ToList();
            return ListFactoresOpcionesAccidentesModel;
        }
        #endregion



    }
}
