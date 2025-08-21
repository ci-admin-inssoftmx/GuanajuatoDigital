using GuanajuatoAdminUsuarios.Controllers;
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
using System.Diagnostics;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class CatAgenciasMinisterioController : BaseController
    {
        private readonly ICatAgenciasMinisterioService _catAgenciasMinisterioService;
		private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;

		public CatAgenciasMinisterioController(ICatAgenciasMinisterioService catAgenciasMinisterioService,ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService
)
        {
            _catAgenciasMinisterioService = catAgenciasMinisterioService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;

		}
        
            DBContextInssoft dbContext = new DBContextInssoft();
            public IActionResult Index()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var ListAgenciasMinisterioModel = _catAgenciasMinisterioService.ObtenerAgencias(corp);

                return View(ListAgenciasMinisterioModel);
            }
 




        #region Modal Action


        [HttpPost]
            public ActionResult AgregarAgenciaMinisterioModal()
             {
    
                Delegaciones_Drop();
                return PartialView("_Crear");
              }
   

            public ActionResult EditarAgenciaMinisterioModal(int IdAgenciaMinisterio)
            {
			    var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			    Delegaciones_Drop();
                var agenciasMinisterioModel = GetAgenciaMinisterioByID(IdAgenciaMinisterio,corp);
                return PartialView("_Editar", agenciasMinisterioModel);
            }


            public ActionResult EliminarAgenciaMinisterioModal(int IdAgenciaMinisterio)
            {
			    var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			    Delegaciones_Drop();
                var agenciasMinisterioModel = GetAgenciaMinisterioByID(IdAgenciaMinisterio,corp);
                return PartialView("_Eliminar", agenciasMinisterioModel);
            }



            [HttpPost]
            public ActionResult AgregarAgenciaMinisterio(CatAgenciasMinisterioModel model)
            {
                var errors = ModelState.Values.Select(s => s.Errors);
                ModelState.Remove("NombreAgencia");
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            if (ModelState.IsValid)
                {


                    CrearAgenciaMinisterio(model);
                    var ListAgenciasMinisterioModel = _catAgenciasMinisterioService.ObtenerAgencias((int)corp);
				return Json(ListAgenciasMinisterioModel);
                }
			Delegaciones_Drop();
                return PartialView("_Crear");
            }

            public ActionResult EditarAgenciaMinisterioMod(CatAgenciasMinisterioModel model)
            {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            bool switchAgenciasMinisterio = Request.Form["agenciasSwitch"].Contains("true");
                model.Estatus = switchAgenciasMinisterio ? 1 : 0;
                var errors = ModelState.Values.Select(s => s.Errors);
                ModelState.Remove("NombreAgencia");
			if (ModelState.IsValid)
                {


                    EditarAgenciaMinisterio(model);
                    var ListAgenciasMinisterioModel = _catAgenciasMinisterioService.ObtenerAgencias((int)corp);
                    return Json(ListAgenciasMinisterioModel);
                }
                return PartialView("_ListaAgenciasMinisterio");
            }

            public ActionResult EliminarAgenciaMinisterioMod(CatAgenciasMinisterioModel model)
            {
                var errors = ModelState.Values.Select(s => s.Errors);
			    var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			ModelState.Remove("NombreAgencia");
                if (ModelState.IsValid)
                {


                    EliminarAgenciaMinisterio(model);
                    var ListAgenciasMinisterioModel = _catAgenciasMinisterioService.ObtenerAgencias(corp);
                    return PartialView("_ListaAgenciasMinisterio", ListAgenciasMinisterioModel);
                }
			Delegaciones_Drop();
                return PartialView("_Eliminar");
            }

            public JsonResult GetAgenciasM([DataSourceRequest] DataSourceRequest request, int idDelegacion,int? idDependencia)
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

            var ListAgenciasMinisterioModel = _catAgenciasMinisterioService.ObtenerAgencias(corp);
			if (idDelegacion != 0)
                ListAgenciasMinisterioModel = ListAgenciasMinisterioModel.Where(s => s.IdDelegacion == idDelegacion).ToList();
            return Json(ListAgenciasMinisterioModel.ToDataSourceResult(request));
            }

		public JsonResult Delegaciones_Drop()
		{
			var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos(), "IdDelegacion", "Delegacion");
			return Json(result);
		}




		#endregion


		#region Acciones a base de datos

		public void CrearAgenciaMinisterio(CatAgenciasMinisterioModel model)
            {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var corporation = corp < 2 ? 1 : corp;


			CatAgenciasMinisterio agencia = new CatAgenciasMinisterio();
                agencia.IdAgenciaMinisterio = model.IdAgenciaMinisterio;
                agencia.NombreAgencia = model.NombreAgencia;
                agencia.IdDelegacion = model.IdDelegacion;
                agencia.Estatus = 1;
                agencia.transito = (int)corporation;
                agencia.ActualizadoPor = Convert.ToInt32(User.FindFirst(CustomClaims.IdUsuario).Value);
                agencia.FechaActualizacion = DateTime.Now;
                dbContext.CatAgenciasMinisterio.Add(agencia);
                dbContext.SaveChanges();
            }

            public void EditarAgenciaMinisterio(CatAgenciasMinisterioModel model)
            {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var corporation = corp < 2 ? 1 : corp;


			CatAgenciasMinisterio agencia = new CatAgenciasMinisterio();
                agencia.IdAgenciaMinisterio = model.IdAgenciaMinisterio;
                agencia.NombreAgencia = model.NombreAgencia;
                agencia.IdDelegacion = model.IdDelegacion;
                agencia.Estatus = model.Estatus;
			agencia.ActualizadoPor = Convert.ToInt32(User.FindFirst(CustomClaims.IdUsuario).Value);
			agencia.transito = (int)corporation;
			agencia.FechaActualizacion = DateTime.Now;
                dbContext.Entry(agencia).State = EntityState.Modified;
                dbContext.SaveChanges();

            }

            public void EliminarAgenciaMinisterio(CatAgenciasMinisterioModel model)
            {
                CatAgenciasMinisterio agencia = new CatAgenciasMinisterio();
                agencia.IdAgenciaMinisterio = model.IdAgenciaMinisterio;
                agencia.NombreAgencia = model.NombreAgencia;
                agencia.IdDelegacion = model.IdDelegacion;
                agencia.Estatus = 0;
                agencia.FechaActualizacion = DateTime.Now;
			agencia.ActualizadoPor = Convert.ToInt32(User.FindFirst(CustomClaims.IdUsuario).Value);

			dbContext.Entry(agencia).State = EntityState.Modified;
                dbContext.SaveChanges();


            }

            private void SetDDLDelegaciones()
            {
                ///Espacio en memoria de manera temporal que solo existe en la petición bool, list, string ,clases , selectlist
                ViewBag.Delegaciones = new SelectList(dbContext.Delegaciones.ToList(), "IdDelegacion", "Delegacion");
            }



            public CatAgenciasMinisterioModel GetAgenciaMinisterioByID(int IdAgenciaMinisterio,int corp)
            {

                var productEnitity = dbContext.CatAgenciasMinisterio.Find(IdAgenciaMinisterio);

                var agenciasMinisterioModel = (from catAgenciasMinisterio in dbContext.CatAgenciasMinisterio.ToList()
                                               select new CatAgenciasMinisterioModel

                                               {
                                                   IdAgenciaMinisterio = catAgenciasMinisterio.IdAgenciaMinisterio,
                                                   NombreAgencia = catAgenciasMinisterio.NombreAgencia,
                                                   IdDelegacion = catAgenciasMinisterio.IdDelegacion,
                                                   Estatus = catAgenciasMinisterio.Estatus,

                                               }).Where(w => w.IdAgenciaMinisterio == IdAgenciaMinisterio).FirstOrDefault();

                return agenciasMinisterioModel;
            }

            /// <summary>
            /// Linq es una tecnologia de control de datos (excel, txt,EF,sqlclient etc)
            /// para la gestion un mejor control de la info
            /// </summary>
            /// <returns></returns>
            public List<CatAgenciasMinisterioModel> GetAgenciasministerio()
            {
                var ListAgenciasMinisterioModel = (from catAgenciasMinisterio in dbContext.CatAgenciasMinisterio.ToList()
                                                   join delegaciones in dbContext.Delegaciones.ToList()
                                                   on catAgenciasMinisterio.IdDelegacion equals delegaciones.IdDelegacion
                                                   join estatus in dbContext.Estatus.ToList()
                                                   on catAgenciasMinisterio.Estatus equals estatus.estatus
                                                   select new CatAgenciasMinisterioModel
                                                   {
                                                       IdDelegacion = catAgenciasMinisterio.IdDelegacion,
                                                       IdAgenciaMinisterio = catAgenciasMinisterio.IdAgenciaMinisterio,
                                                       NombreAgencia = catAgenciasMinisterio.NombreAgencia,
                                                       DelegacionDesc = delegaciones.Delegacion,
                                                       estatusDesc = estatus.estatusDesc,
                                                   }).OrderBy(s=>s.IdAgenciaMinisterio).ToList();
                return ListAgenciasMinisterioModel;
            }
        #endregion

        [HttpGet]
        public ActionResult ajax_BuscarAgencias( int idDelegacionFiltro)
        {
            List<CatAgenciasMinisterioModel> ListAgencias = new List<CatAgenciasMinisterioModel>();


            ListAgencias = (from catAgenciasMinisterio in GetAgenciasministerio().ToList()
                                          //join municipio in _catMunicipiosService.GetMunicipios().ToList()
                                          //on diasInhabiles.idMunicipio equals municipio.IdMunicipio
                                          // join estatus in dbContext.Estatus.ToList()
                                          //on diasInhabiles.Estatus equals estatus.estatus

                                      select new CatAgenciasMinisterioModel
                                      {
                                          IdAgenciaMinisterio = catAgenciasMinisterio.IdAgenciaMinisterio,
                                          NombreAgencia = catAgenciasMinisterio.NombreAgencia,
                                          IdDelegacion = catAgenciasMinisterio.IdDelegacion,
                                          DelegacionDesc = catAgenciasMinisterio.DelegacionDesc,
                                          Estatus = catAgenciasMinisterio.Estatus,
                                          // EstatusDesc = estatus.estatusDesc,
                                      }).ToList();

           
           if (idDelegacionFiltro > 0)
            {
                ListAgencias = (from s in ListAgencias
                                where s.IdDelegacion == idDelegacionFiltro
                                select s).ToList();
            }

            return Json(ListAgencias);
        }

    }
}

    
