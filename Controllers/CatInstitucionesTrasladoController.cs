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
    public class CatInstitucionesTrasladoController : BaseController
    {
        private readonly ICatInstitucionesTrasladoService _catInstitucionesTrasladoService;

        public CatInstitucionesTrasladoController(ICatInstitucionesTrasladoService catInstitucionesTrasladoService)
        {
            _catInstitucionesTrasladoService = catInstitucionesTrasladoService;
        }
    
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            var corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);

            //var ListInstitucionesTrasladoModel = GetInstitucionesTraslado(corp);

                return View();
            }
      





            #region Modal Action
            public ActionResult IndexModal()
        {
            var corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);

            var ListInstitucionesTrasladoModel = GetInstitucionesTraslado(corp);
            return View("Index", ListInstitucionesTrasladoModel);
        }

        [HttpPost]
        public ActionResult AgregarInstitucionTrasladoModal()
        {
      
                return PartialView("_Crear");
            }
   

        public ActionResult EditarInstitucionTrasladoModal(int IdInstitucionTraslado)
        {
       
                var institucionesTrasladoModel = GetInstitucionTrasladoByID(IdInstitucionTraslado);
            return PartialView("_Editar", institucionesTrasladoModel);
            }
   

        public ActionResult EliminarInstitucionTrasladoModal(int IdInstitucionTraslado)
        {
            var institucionesTrasladoModel = GetInstitucionTrasladoByID(IdInstitucionTraslado);
            return PartialView("_Eliminar", institucionesTrasladoModel);
        }



        [HttpPost]
        public ActionResult AgregarInstitucionTraslado(CatInstitucionesTrasladoModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("InstitucionTraslado");
            if (ModelState.IsValid)
            {


                CrearInstitucionTraslado(model);
                var ListInstitucionesTrasladoModel = GetInstitucionesTraslado((int)corp);
                return Json(ListInstitucionesTrasladoModel);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarInstitucionTrasladoMod(CatInstitucionesTrasladoModel model,int? Corp)
        {
            var corp = Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            bool switchInstTraslado = Request.Form["instTrasladoSwitch"].Contains("true");
            model.Estatus = switchInstTraslado ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("InstitucionTraslado");
            if (ModelState.IsValid)
            {


                EditarInstitucionTraslado(model);
                var ListInstitucionesTrasladoModel = GetInstitucionesTraslado((int)corp);
                return Json(ListInstitucionesTrasladoModel);
            }
            return PartialView("_Editar");
        }

        public ActionResult EliminarInstitucionTrasladoMod(CatInstitucionesTrasladoModel model)
        {
           var corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);

            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("AutoridadEntrega");
            if (ModelState.IsValid)
            {


                EliminaInstitucionTraslado(model);
                var ListInstitucionesTrasladoModel = GetInstitucionesTraslado(corp);
                return Json(ListInstitucionesTrasladoModel);
            }
            return PartialView("_Eliminar");
        }
        public JsonResult GetInstTraslado([DataSourceRequest] DataSourceRequest request,int? idDependencia)
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
            var ListInstitucionesTrasladoModel = GetInstitucionesTraslado(corp);

            return Json(ListInstitucionesTrasladoModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CrearInstitucionTraslado(CatInstitucionesTrasladoModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var corporation = corp < 2 ? 1 : corp;
			CatInstitucionesTraslado institucion = new CatInstitucionesTraslado();
            institucion.IdInstitucionTraslado = model.IdInstitucionTraslado;
            institucion.InstitucionTraslado = model.InstitucionTraslado;
            institucion.Estatus = 1;
            institucion.transito = (int)corporation;
            institucion.FechaActualizacion = DateTime.Now;
            dbContext.CatInstitucionesTraslado.Add(institucion);
            dbContext.SaveChanges();
        }

        public void EditarInstitucionTraslado(CatInstitucionesTrasladoModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var corporation = corp < 2 ? 1 : corp;
			CatInstitucionesTraslado institucion = new CatInstitucionesTraslado();
            institucion.IdInstitucionTraslado = model.IdInstitucionTraslado;
            institucion.InstitucionTraslado = model.InstitucionTraslado;
            institucion.Estatus = model.Estatus;
            institucion.transito=(int)corporation;
            institucion.FechaActualizacion = DateTime.Now;
            dbContext.Entry(institucion).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void EliminaInstitucionTraslado(CatInstitucionesTrasladoModel model)
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var corporation = corp < 2 ? 1 : corp;
			CatInstitucionesTraslado institucion = new CatInstitucionesTraslado();
            institucion.IdInstitucionTraslado = model.IdInstitucionTraslado;
            institucion.InstitucionTraslado = model.InstitucionTraslado;
            institucion.Estatus = 0;
            institucion.transito = corporation;
            institucion.FechaActualizacion = DateTime.Now;
            dbContext.Entry(institucion).State = EntityState.Modified;
            dbContext.SaveChanges();

        }



		public CatInstitucionesTrasladoModel GetInstitucionTrasladoByID(int IdInstitucionTraslado)
		{
			var institucionesTrasladoModel = (from catInstitucionesTraslado in dbContext.CatInstitucionesTraslado
											  where catInstitucionesTraslado.IdInstitucionTraslado == IdInstitucionTraslado
											  select new CatInstitucionesTrasladoModel
											  {
												  IdInstitucionTraslado = catInstitucionesTraslado.IdInstitucionTraslado,
												  InstitucionTraslado = catInstitucionesTraslado.InstitucionTraslado,
												  Estatus = catInstitucionesTraslado.Estatus ?? default(int?),
												  Corp = (int?)catInstitucionesTraslado.transito
											  })
											  .FirstOrDefault();
			return institucionesTrasladoModel;
		}






		/// <summary>
		/// Linq es una tecnologia de control de datos (excel, txt,EF,sqlclient etc)
		/// para la gestion un mejor control de la info
		/// </summary>
		/// <returns></returns>
		public List<CatInstitucionesTrasladoModel> GetInstitucionesTraslado(int corp)
        {
            var corporation = corp < 2 ? 1 : corp;

            // Consulta para manejar valores nulos en la base de datos.
            var listInstitucionesTrasladoModel = (from catInstitucionesTraslado in dbContext.CatInstitucionesTraslado
                                                  join estatus in dbContext.Estatus
                                                  on catInstitucionesTraslado.Estatus equals estatus.estatus into estatusJoin
                                                  from estatus in estatusJoin.DefaultIfEmpty()
                                                  where catInstitucionesTraslado.transito == corporation
                                                  select new CatInstitucionesTrasladoModel
                                                  {
                                                      IdInstitucionTraslado = catInstitucionesTraslado.IdInstitucionTraslado,
                                                      InstitucionTraslado = catInstitucionesTraslado.InstitucionTraslado,
                                                      estatusDesc = estatus != null ? estatus.estatusDesc : "Desconocido", // Manejo de valores nulos
                                                      Corp = catInstitucionesTraslado.transito
                                                  }).ToList();

            return listInstitucionesTrasladoModel;
        }

        #endregion



    }
}
