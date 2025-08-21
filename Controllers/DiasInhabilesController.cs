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
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class DiasInhabilesController : BaseController
    {
        private readonly IDiasInhabiles _diasInhabiles;
        private readonly ICatMunicipiosService _catMunicipiosService;


        public DiasInhabilesController(IDiasInhabiles diasInhabiles, ICatMunicipiosService catMunicipiosService)
        {
            _diasInhabiles = diasInhabiles;
            _catMunicipiosService = catMunicipiosService;
        }
        public IActionResult Index()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var ListDiasInhabilesModel = _diasInhabiles.GetDiasInhabiles(corp);

            return View(ListDiasInhabilesModel);
            }
        

        ///Crear metodo de update (post)


        #region Modal Action
       
        [HttpPost]
        public ActionResult AgregarParcialDiaInhabil()
        {

            return PartialView("_Crear");
            }
     

        public ActionResult EditarParcial(int idDiaInhabil)
        {
          
             var diasInhabilesModel = _diasInhabiles.GetDiasById(idDiaInhabil);

            


            return View("_Editar", diasInhabilesModel);
         }
       

/*public ActionResult EliminarDiaInhabilParcial(int IdDiaInhabil)
        {
            var diasInhabilesModel = GetDiaInhabilByID(IdDiaInhabil);
            Municipios_Drop();
            return View("_Eliminar", diasInhabilesModel);
        }*/

        public JsonResult Municipios_Read(int? idDependencia) {

            var corp = idDependencia;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var q = _catMunicipiosService.GetMunicipiosGuanajuato((int)corp);

            var data = q.GroupBy(s => s.Municipio).Select(s => s.FirstOrDefault()).ToList();

            var result = new SelectList(data, "IdMunicipio", "Municipio");
            return Json(result);
        }



        [HttpPost]
        public ActionResult CreatePartialModal(DiasInhabilesModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("fecha");
            if (ModelState.IsValid)
            {              
                  
                
                    _diasInhabiles.CrearDiaInhabil(model,(int)corp);
                var ListDiasInhabilesModel = _diasInhabiles.GetDiasInhabiles((int)corp);
                return PartialView("_ListaDiasInhabiles", ListDiasInhabilesModel);
            }
            Municipios_Drop();
            return PartialView("_Crear");
        }

        [HttpPost]
        public ActionResult EditarParcialModal(DiasInhabilesModel model)
        {
            bool switchDiasinhabiles = Request.Form["diasInhabilesSwitch"].Contains("true");
            model.Estatus = switchDiasinhabiles ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("fecha");
            if (ModelState.IsValid)
            {
				var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
				_diasInhabiles.EditDia(model);
                var ListDiasInhabilesModel = _diasInhabiles.GetDiasInhabiles(corp);
                return PartialView("_ListaDiasInhabiles", ListDiasInhabilesModel);
            }
            Municipios_Drop();
            return PartialView("_Editar");
        }

        [HttpPost]
        public ActionResult EliminarDiaParcialModal(DiasInhabilesModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("fecha");
            if (ModelState.IsValid)
            {

				var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
				DeleteDiaInhabil(model);
                var ListDiasInhabilesModel = _diasInhabiles.GetDiasInhabiles(corp);
                return Json(ListDiasInhabilesModel);
            }
            Municipios_Drop();
            //return View("Create");
            return PartialView("_Eliminar");
        }
        public JsonResult GetDiasIn([DataSourceRequest] DataSourceRequest request, string fecha, int idMunicipio, int? idDependencia)
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
            if (idDependencia.HasValue)
            {
                corp = idDependencia.Value;
            }
            else
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);

            }
            var ListDiasInhabilesModel = _diasInhabiles.GetDiasInhabiles(corp);

            if (!String.IsNullOrEmpty(fecha) && idMunicipio > 0)
            {
                ListDiasInhabilesModel = (from s in ListDiasInhabilesModel
                                          where Convert.ToDateTime(s.fecha) == Convert.ToDateTime(fecha)
                                          &&
                                          s.idMunicipio == idMunicipio
                                          select s).ToList();
            }
            else if (!String.IsNullOrEmpty(fecha) && idMunicipio == 0)
            {
                ListDiasInhabilesModel = (from s in ListDiasInhabilesModel
                                          where Convert.ToDateTime(s.fecha) == Convert.ToDateTime(fecha)
                                          select s).ToList();
            }
            else if (String.IsNullOrEmpty(fecha) && idMunicipio > 0)
            {
                ListDiasInhabilesModel = (from s in ListDiasInhabilesModel
                                          where s.idMunicipio == idMunicipio
                                          select s).ToList();
            }


            return Json(ListDiasInhabilesModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CreateDiaInhabil(DiasInhabilesModel model)
        {
            DiasInhabiles diaInhabil = new DiasInhabiles();
            diaInhabil.idDiaInhabil = model.idDiaInhabil;
            diaInhabil.fecha = model.fecha;
            diaInhabil.idMunicipio = model.idMunicipio;
            diaInhabil.todosMunicipiosDesc = model.todosMunicipiosDesc;
            diaInhabil.Estatus = 1;
            diaInhabil.FechaActualizacion = DateTime.Now;
            //dbContext.DiasInhabiles.Add(diaInhabil);
            //dbContext.SaveChanges();
        }

        public void UpdateDiaInhabil(DiasInhabilesModel model)
        {
            DiasInhabiles diaInhabil = new DiasInhabiles();
            diaInhabil.idDiaInhabil = model.idDiaInhabil;
            diaInhabil.fecha = model.fecha;
            diaInhabil.idMunicipio = model.idMunicipio;
            diaInhabil.todosMunicipiosDesc = model.todosMunicipiosDesc;
            diaInhabil.Estatus = model.Estatus;
            diaInhabil.FechaActualizacion = DateTime.Now;

            //dbContext.Entry(diaInhabil).State = EntityState.Modified;
            //dbContext.SaveChanges();

        }

        public void DeleteDiaInhabil(DiasInhabilesModel model)
        {
            DiasInhabiles diaInhabil = new DiasInhabiles();
            diaInhabil.idDiaInhabil = model.idDiaInhabil;
            diaInhabil.fecha = model.fecha;
            diaInhabil.idMunicipio = model.idMunicipio;
            diaInhabil.todosMunicipiosDesc = model.todosMunicipiosDesc;
            diaInhabil.Estatus = 0;
            diaInhabil.FechaActualizacion = DateTime.Now;
           // dbContext.Entry(diaInhabil).State = EntityState.Modified;
            //dbContext.SaveChanges();

        }

      

        public JsonResult Municipios_Drop()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var result = new SelectList(_catMunicipiosService.GetMunicipios(corp), "IdMunicipio", "Municipio");
            ViewBag.CatMunicipios = result;
            return Json(result);
        }

        /*  public DiasInhabilesModel GetDiaInhabilByID(int IdDiaInhabil)
          {

              var productEnitity = dbContext.DiasInhabiles.Find(IdDiaInhabil);

              var diaInhabilModel = (from diasInhabiles in dbContext.DiasInhabiles.ToList()
                                     select new DiasInhabilesModel

                                     {
                                         idDiaInhabil = diasInhabiles.idDiaInhabil,
                                         fecha =  diasInhabiles.fecha,
                                         idMunicipio = diasInhabiles.idMunicipio,
                                         todosMunicipiosDesc = diasInhabiles.todosMunicipiosDesc,



                                     }).Where(w => w.idDiaInhabil == IdDiaInhabil).FirstOrDefault();

              return diaInhabilModel;
          }

          public List<DiasInhabilesModel> GetDiasInhabiles()
          {
              var ListDiasInhabilesModel = (from diasInhabiles in dbContext.DiasInhabiles.ToList()
                                            join municipio in dbContext.CatMunicipios.ToList()
                                            on diasInhabiles.idMunicipio equals municipio.IdMunicipio
                                            join estatus in dbContext.Estatus.ToList()
                                            on diasInhabiles.Estatus equals estatus.estatus



                                            select new DiasInhabilesModel
                                            {
                                                idDiaInhabil = diasInhabiles.idDiaInhabil,
                                                fecha = diasInhabiles.fecha,
                                                idMunicipio = diasInhabiles.idMunicipio,
                                                todosMunicipiosDesc = diasInhabiles.todosMunicipiosDesc,
                                                Estatus = diasInhabiles.Estatus,
                                                EstatusDesc = estatus.estatusDesc,
                                                Municipio = municipio.Municipio

                                            }).ToList();
              return ListDiasInhabilesModel;
          }
          #
        */
        #endregion
        [HttpGet]
        public ActionResult ajax_BuscarDiasInhabiles(string fecha, int idMunicipio)
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			List<DiasInhabilesModel> ListDiasInhabilesModel = new List<DiasInhabilesModel>();
            if (!String.IsNullOrEmpty(fecha))
            {
                if (fecha.ToUpper().Contains("DAY"))
                    fecha = null;
            }

            ListDiasInhabilesModel = (from diasInhabiles in _diasInhabiles.GetDiasInhabiles(corp).ToList()
                                          //join municipio in _catMunicipiosService.GetMunicipios().ToList()
                                          //on diasInhabiles.idMunicipio equals municipio.IdMunicipio
                                         // join estatus in dbContext.Estatus.ToList()
                                          //on diasInhabiles.Estatus equals estatus.estatus

                                          select new DiasInhabilesModel
                                          {
                                              idDiaInhabil = diasInhabiles.idDiaInhabil,
                                              fecha = diasInhabiles.fecha,
                                              idMunicipio = diasInhabiles.idMunicipio,
                                              todosMunicipiosDesc = diasInhabiles.todosMunicipiosDesc,
                                              Estatus = diasInhabiles.Estatus,
                                             // EstatusDesc = estatus.estatusDesc,
                                              Municipio = diasInhabiles.Municipio
                                          }).ToList();

            if (!String.IsNullOrEmpty(fecha) && idMunicipio > 0)
            {
                ListDiasInhabilesModel = (from s in ListDiasInhabilesModel
                                          where Convert.ToDateTime(s.fecha) == Convert.ToDateTime(fecha)
                                          &&
                                          s.idMunicipio == idMunicipio
                                          select s).ToList();
            } else if (!String.IsNullOrEmpty(fecha) && idMunicipio == 0)
            {
                ListDiasInhabilesModel = (from s in ListDiasInhabilesModel
                                          where Convert.ToDateTime(s.fecha) == Convert.ToDateTime(fecha)
                                          select s).ToList();
            }
            else if (String.IsNullOrEmpty(fecha) && idMunicipio > 0)
            {
                ListDiasInhabilesModel = (from s in ListDiasInhabilesModel
                                          where s.idMunicipio == idMunicipio
                                          select s).ToList();
            }

            return Json(ListDiasInhabilesModel);
        }

    }
}
