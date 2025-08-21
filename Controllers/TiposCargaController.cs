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
    public class TiposCargaController : BaseController
    {
        private readonly ITiposCarga _tiposCargaService;

        DBContextInssoft dbContext = new DBContextInssoft();

        public TiposCargaController(ITiposCarga tiposCargaService)
        {
            _tiposCargaService = tiposCargaService;
        }
        public IActionResult Index()
        {
           
                var ListTiposCargaModel = _tiposCargaService.GetTiposCarga();

            return View(ListTiposCargaModel);
            }



        #region Modal Action
        public ActionResult IndexModal()
        {
   
                var ListTiposCargaModel = GetTiposCarga();
            //return View("IndexModal");
            return View("Index", ListTiposCargaModel);
        }
 

        [HttpPost]
        public ActionResult AgregarTipoCargaParcial()
        {
          
                return PartialView("_Crear");
            }
 

        [HttpPost]
        public ActionResult EditarParcial(int IdTipoCarga)
        {
           
                var tiposCargaModel = GetTipoCargaByID(IdTipoCarga);
                return View("_Editar", tiposCargaModel);
            }


        [HttpPost]
        public ActionResult EliminarTipoCargaParcial(int IdTipoCarga)
        {
            var tiposCargaModel = GetTipoCargaByID(IdTipoCarga);
            return View("_Eliminar", tiposCargaModel);
        }
        public JsonResult Categories_Read()
        {
            var result = new SelectList(dbContext.TiposCarga.ToList(), "IdTipoCarga", "TipoCarga");
            return Json(result);
        }



        [HttpPost]
        public ActionResult CreatePartialModal(TiposCargaModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("TipoCarga");
            if (ModelState.IsValid)
            {


                CreateTipoCarga(model);
                var ListTiposCargaModel = GetTiposCarga();
                return Json(ListTiposCargaModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Crear");
        }

        [HttpPost]
        public ActionResult EditarParcialModal(TiposCargaModel model)
        {
            bool switchTiposCarga = Request.Form["tiposCargaSwitch"].Contains("true");
            model.Estatus = switchTiposCarga ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("TipoCarga");
            if (ModelState.IsValid)
            {


                UpdateTipoCarga(model);
                var ListTiposCargaModel = GetTiposCarga();
                return Json(ListTiposCargaModel);
            }

            return PartialView("_Editar");
        }

        [HttpPost]
        public ActionResult EditarTipoCargaParcialModal(TiposCargaModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("TipoCarga");
            if (ModelState.IsValid)
            {


                DeleteTipoCarga(model);
                var ListTiposCargaModel = GetTiposCarga();
                return Json(ListTiposCargaModel);
            }
            return PartialView("_Eliminar");
        }

        public JsonResult GetTipos([DataSourceRequest] DataSourceRequest request)
        {
            var ListTiposCargaModel = GetTiposCarga();

            return Json(ListTiposCargaModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CreateTipoCarga(TiposCargaModel model)
        {
            TiposCarga tipo = new TiposCarga();
            tipo.IdTipoCarga = model.IdTipoCarga;
            tipo.TipoCarga = model.TipoCarga;
            tipo.Estatus = 1;
            tipo.FechaActualizacion = DateTime.Now;
            dbContext.TiposCarga.Add(tipo);
            dbContext.SaveChanges();
        }
        public void UpdateTipoCarga(TiposCargaModel model)
        {
            TiposCarga tipo = new TiposCarga();
            tipo.IdTipoCarga = model.IdTipoCarga;
            tipo.TipoCarga = model.TipoCarga;
            tipo.Estatus = model.Estatus;
            tipo.FechaActualizacion = DateTime.Now;
            dbContext.Entry(tipo).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void DeleteTipoCarga(TiposCargaModel model)
        {
            TiposCarga tipo = new TiposCarga();
            tipo.IdTipoCarga = model.IdTipoCarga;
            tipo.TipoCarga = model.TipoCarga;
            tipo.Estatus = 0;
            tipo.FechaActualizacion = DateTime.Now;
            dbContext.Entry(tipo).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        private void SetDDLTiposCarga()
        {
            ///Espacio en memoria de manera temporal que solo existe en la petición bool, list, string ,clases , selectlist
            ViewBag.Tiposcarga = new SelectList(dbContext.TiposCarga.ToList(), "IdTipoCarga", "TipoCarga");
        }


        public TiposCargaModel GetTipoCargaByID(int IdTipoCarga)
        {

            var productEnitity = dbContext.TiposCarga.Find(IdTipoCarga);

            var tipoCargaModel = (from tiposCarga in dbContext.TiposCarga.ToList()
                                  select new TiposCargaModel

                                  {
                                      IdTipoCarga = tiposCarga.IdTipoCarga,
                                      TipoCarga = tiposCarga.TipoCarga,
                                      Estatus = tiposCarga.Estatus,


                                  }).Where(w => w.IdTipoCarga == IdTipoCarga).FirstOrDefault();

            return tipoCargaModel;
        }

        /// <summary>
        /// Linq es una tecnologia de control de datos (excel, txt,EF,sqlclient etc)
        /// para la gestion un mejor control de la info
        /// </summary>
        /// <returns></returns>
        public List<TiposCargaModel> GetTiposCarga()
        {
            var ListTiposcargaModel = (from tiposCarga in dbContext.TiposCarga.ToList()
                                       join estatus in dbContext.Estatus.ToList()
                                       on tiposCarga.Estatus equals estatus.estatus
                                       

                                       select new TiposCargaModel
                                       {
                                           IdTipoCarga = tiposCarga.IdTipoCarga,
                                           TipoCarga = tiposCarga.TipoCarga,
                                           Estatus = tiposCarga.Estatus,
                                           EstatusDesc = estatus.estatusDesc

                                       }).ToList();
            return ListTiposcargaModel;
        }
        #endregion



    }
}
