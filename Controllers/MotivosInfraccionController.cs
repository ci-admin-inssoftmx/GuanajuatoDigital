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
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;
using Microsoft.AspNetCore.Authorization;



namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class MotivosInfraccionController : BaseController
    {
        //DBContextInssoft dbContext = new DBContextInssoft();
        private readonly ICatDictionary _catDictionary;
        private readonly IMotivoInfraccionService _motivoInfraccionService;
        private readonly ICustomCatalogService _CustomCatalog;

       public MotivosInfraccionController(ICatDictionary catDictionary, IMotivoInfraccionService motivoInfraccionService,ICustomCatalogService customCatalog)
        {
            _catDictionary = catDictionary;
            _motivoInfraccionService = motivoInfraccionService;
            _CustomCatalog=customCatalog;
        }

        public IActionResult Index()
        {
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            CatMotivosInfraccionModel searchModel = new CatMotivosInfraccionModel();
            List<CatMotivosInfraccionModel> listMotivosInfraccion = _motivoInfraccionService.GetMotivos(idDependencia);

            searchModel.ListMotivosInfraccion = listMotivosInfraccion;
            return View(searchModel);
        }


        #region Modal Action
        public ActionResult IndexModal()
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            CatMotivosInfraccionModel searchModel = new CatMotivosInfraccionModel();
            List<CatMotivosInfraccionModel> listMotivosInfraccion = _motivoInfraccionService.GetMotivos(idDependencia);

            searchModel.ListMotivosInfraccion = listMotivosInfraccion;
            return View(searchModel);
        }


        [HttpPost]
        public ActionResult AgregarMotivoParcial()
        {

			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var catConcepto = _CustomCatalog.GetCatConceptoInfraccion(corp);
            ViewData["CatConcepto"] = new SelectList(catConcepto, "value", "text");

            return View("_Crear");
        }
        public ActionResult GetConceptoPorCorp(int? idDependencia)
        {
            var corp = idDependencia;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var catConcepto = _CustomCatalog.GetCatConceptoInfraccion((int)corp);


            return Json(catConcepto);
        }

        public ActionResult drop_GetcatSubConceptoInfraccion(string idConcepto,int? idDependencia)
		{
            var corp = idDependencia;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var catSubConcepto = _CustomCatalog.GetcatSubConceptoInfraccion((int)corp).Where(s=>s.aux==idConcepto).ToList();



			return Json(catSubConcepto);
		}

		public ActionResult EditarParcial(int IdCatMotivoInfraccion,int Corp)
        {
            int idDependencia = Corp;

            var motivosInfraccionsModel = _motivoInfraccionService.GetMotivoByID(IdCatMotivoInfraccion, idDependencia);

            var corp = Corp;

            var catConcepto = _CustomCatalog.GetCatConceptoInfraccion(corp);
            ViewData["CatConcepto"] = new SelectList(catConcepto, "value", "text");

            var catSubConcepto = _CustomCatalog.GetcatSubConceptoInfraccion(corp).Where(s => s.aux == motivosInfraccionsModel.idConcepto.ToString()).ToList();
            ViewData["CatSubConceptoInfraccion"] = new SelectList(catSubConcepto, "value", "text");
          
            
            
            return View("_Editar", motivosInfraccionsModel);
        }


        public ActionResult EliminarMotivoParcial(int IdCatMotivoInfraccion)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            var motivosInfraccionsModel = _motivoInfraccionService.GetMotivoByID(IdCatMotivoInfraccion, idDependencia);
            return View("_Eliminar", motivosInfraccionsModel);
        }
        public JsonResult Categories_Read()
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            var result = new SelectList(_motivoInfraccionService.GetCatMotivos(idDependencia), "IdCatMotivoInfraccion", "Nombre");
            return Json(result);
        }

        public JsonResult loadSubConceptos(int idConcepto)
        {
            if (idConcepto > 0)
            {
                var catSubConcepto = _catDictionary.GetCatalog("CatSubConceptoInfraccion", idConcepto.ToString());
                var result = new SelectList(catSubConcepto.CatalogList, "Id", "Text");
                return Json(result);
            } else
            {
                var catSubConcepto = _catDictionary.GetCatalog("CatSubConceptoInfraccion", "0");
                var result = new SelectList(catSubConcepto.CatalogList, "Id", "Text");
                return Json(result);
            }
        }
        

        public JsonResult ObtenerSubconceptos(int idConceptoValue)
        {
            var result = new SelectList(_motivoInfraccionService.GetSubconceptos(idConceptoValue), "idSubConcepto", "subConcepto");
            return Json(result);
        }

        public JsonResult AllMotivos_Drop(int idConcepto, int idSubconcepto)
        {
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            var result = new SelectList(_motivoInfraccionService.GetMotivosDropDown(idDependencia, idSubconcepto, idConcepto), "IdCatMotivoInfraccion", "Nombre");
            return Json(result);
        }


        [HttpPost]
        public ActionResult CreatePartialMotivoModal(CatMotivosInfraccionModel model)
        {

            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Nombre");
            if (ModelState.IsValid)
            {


                CreateMotivo(model);
                var ListMotivosInfraccionModel = _motivoInfraccionService.GetMotivos((int)corp);
                return Json(ListMotivosInfraccionModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_Crear");
        }

        [HttpPost]
        public ActionResult EditarParcialModal(CatMotivosInfraccionModel model)
        {

            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            bool switchMotivosInfraccion = Request.Form["motivosInfraccionSwitch"].Contains("true");
            model.estatus = switchMotivosInfraccion ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Nombre");
            if (ModelState.IsValid)
            {

                UpdateMotivo(model);
                var ListMotivosInfraccionModel = _motivoInfraccionService.GetMotivos((int)corp);
                return Json(ListMotivosInfraccionModel);
            }
            return PartialView("_Editar");
        }

        [HttpPost]
        public ActionResult EliminarMotivoParcialModal(CatMotivosInfraccionModel model)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("Nombre");
            if (ModelState.IsValid)
            {


                DeleteMotivo(model);
                var ListMotivosInfraccionModel = _motivoInfraccionService.GetMotivos(idDependencia);
                return Json(ListMotivosInfraccionModel);
            }
            return PartialView("_Eliminar");
        }

        [HttpGet]
        public ActionResult BuscarMotivoByID(int idCatMotivoInfraccion)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            CatMotivosInfraccionModel motivo = _motivoInfraccionService.GetMotivoByID(idCatMotivoInfraccion, idDependencia);
            return Json(motivo);
        }

        public JsonResult GetMotInf([DataSourceRequest] DataSourceRequest request, CatMotivosInfraccionModel model, int? idDep)
        {

            var idDependencia = idDep.HasValue ? Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value) : 0;
            if (idDep.HasValue)
            {
                idDependencia = idDep.Value;
            }
            else
            {
                idDependencia = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);

            }
            var ListMotivos = _motivoInfraccionService.GetMotivosBusqueda(model, idDependencia);

            return Json(ListMotivos.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CreateMotivo(CatMotivosInfraccionModel model)
        {

            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            _motivoInfraccionService.CrearMotivo(model, (int)corp);
        }

        public void UpdateMotivo(CatMotivosInfraccionModel model)
        {

            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            _motivoInfraccionService.UpdateMotivo(model, (int)corp);

        }

        public void DeleteMotivo(CatMotivosInfraccionModel model)
        {

            _motivoInfraccionService.DeleteMotivo(model);
        }

        /* private void SetDDLColores()
         {
             ///Espacio en memoria de manera temporal que solo existe en la petición bool, list, string ,clases , selectlist
             ViewBag.Categories = new SelectList(dbContext.Color.ToList(), "IdColor", "color");
         }*/


        #endregion
        public ActionResult ajax_BuscarMotivos(CatMotivosInfraccionModel model)
        {

            int idDependencia = HttpContext.Session.GetInt32("IdDependencia") ?? 0;
            var ListMotivos = _motivoInfraccionService.GetMotivosBusqueda(model, idDependencia);
            if (ListMotivos.Count == 0)
            {
                ViewBag.NoResultsMessage = "No se encontraron registros que cumplan con los criterios de búsqueda.";
            }

            return PartialView("_ListaMotivosInfraccion", ListMotivos);

        }


    }
}
