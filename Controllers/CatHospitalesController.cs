using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Framework;
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
//using Telerik.SvgIcons;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class CatHospitalesController : BaseController
    {
        private readonly ICatHospitalesService _catHospitalesService;
        private readonly ICatMunicipiosService _catMunicipiosService;
        private readonly ICatDictionary _catDictionary;
        private readonly ICatEntidadesService _catEntidadesService;

        public CatHospitalesController(ICatHospitalesService catHospitalesService,
                                       ICatMunicipiosService catMunicipiosService,
                                       ICatDictionary catDictionary,
                                       ICatEntidadesService catEntidadesService)
        {
            _catHospitalesService = catHospitalesService;
            _catMunicipiosService = catMunicipiosService;
            _catDictionary = catDictionary;
            _catEntidadesService = catEntidadesService;
        }
        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {

            var catHospitales = new CatHospitalesDTO();
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var catEntidades = _catEntidadesService.ObtenerEntidadesActivas(corp);
            ViewBag.Entidades = new SelectList(catEntidades, "idEntidad", "nombreEntidad");



            var ListHospitalesModel = _catHospitalesService.GetHospitales(corp);
            catHospitales.HospitalesModel = ListHospitalesModel;

            ViewBag.ListaHospitales = catHospitales.HospitalesModel;
            return View(catHospitales);
        }


        public IActionResult BusquedaHospitales(CatHospitalesDTO model)
        {
            var catHospitales = new CatHospitalesDTO();
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var corporation = corp < 2 ? 1 : corp;

			var ListHospitalesModel = _catHospitalesService.GetHospitales(corp).Where(x=>x.IdMunicipio == model.idMunicipio2).ToList();
            catHospitales.HospitalesModel = ListHospitalesModel;

            //ViewBag.ListaHospitales = catHospitales.HospitalesModel;
            return PartialView("_ListaHospitales", ListHospitalesModel);
        }




        #region Modal Action
        public ActionResult IndexModal()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var corporation = corp < 2 ? 1 : corp;
			var ListHospitalesModel = _catHospitalesService.GetHospitales(corp);
            return View("Index", ListHospitalesModel);
        }

        [HttpPost]
        public ActionResult AgregarHospitalModal()
        {
			//Municipios_Drop();
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var corporation = corp < 2 ? 1 : corp;
			var catEntidades = _catEntidadesService.ObtenerEntidadesActivas(corp);
			ViewBag.Entidades = new SelectList(catEntidades, "idEntidad", "nombreEntidad");

			return PartialView("_Crear");
        }


        public ActionResult EditarHospitalModal(int IdHospital,int? Corp)
        {
            var corp = Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var corporation = corp < 2 ? 1 : corp;
			var catEntidades = _catEntidadesService.ObtenerEntidadesActivas((int)corp);
			ViewBag.Entidades = new SelectList(catEntidades, "idEntidad", "nombreEntidad");

            var hospitalesModel = GetHospitalByID(IdHospital);
			
			hospitalesModel.idEntidad = _catMunicipiosService.GetMunicipios3((int)corp).ToList().Where(x => x.IdMunicipio == hospitalesModel.IdMunicipio).FirstOrDefault().IdEntidad;

            return PartialView("_Editar", hospitalesModel);
        }


        public ActionResult EliminarHospitalModal(int IdHospital)
        {
            //Municipios_Drop();
            var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            ViewBag.Entidades = new SelectList(catEntidades.CatalogList, "Id", "Text");
            var hospitalesModel = GetHospitalByID(IdHospital);
            return PartialView("_Eliminar", hospitalesModel);
        }



        [HttpPost]
        public ActionResult AgregarHospital(CatHospitalesModel model)
        {
            var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            ViewBag.Entidades = new SelectList(catEntidades.CatalogList, "Id", "Text");

            model.Municipio = "";
            
            var errors = ModelState.Values.Select(s => s.Errors);
            //ModelState.Remove("NombreHospital");
            //if (ModelState.IsValid)
            //{
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var corporation = corp < 2 ? 1 : corp;
			CrearHospital(model);
                var ListHospitalesModel = _catHospitalesService.GetHospitales((int)corp);
                return Json(ListHospitalesModel);
            //}

            //return PartialView("_Crear");
        }

        public ActionResult EditarHospitalMod(CatHospitalesModel model)
        {
            bool switchHospitales = Request.Form["hospitalesSwitch"].Contains("true");
            model.Estatus = switchHospitales ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreHospital");
            if (ModelState.IsValid)
            {
                var corp = model.Corp;

                if (corp == null)
                {
                    corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
                }
                var corporation = corp < 2 ? 1 : corp;
				EditarHospital(model);
                var ListHospitalesModel = _catHospitalesService.GetHospitales((int)corp);
                return Json(ListHospitalesModel);
            }
            return PartialView("_Editar");
        }

        public ActionResult EliminarHospitalMod(CatHospitalesModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreHospital");
            if (ModelState.IsValid)
            {
				var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
				var corporation = corp < 2 ? 1 : corp;
				EliminarHospital(model);
                var ListHospitalesModel = _catHospitalesService.GetHospitales(corp);
                return Json(ListHospitalesModel);
            }
            return PartialView("_Eliminar");
        }
        public JsonResult GetHospitalesLista([DataSourceRequest] DataSourceRequest request, int idMunicipio, int? idDependencia)
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
			var ListHospitalesModel = _catHospitalesService.GetHospitales(corp);
            if (idMunicipio > 0)
                ListHospitalesModel = ListHospitalesModel.Where(z => z.IdMunicipio == idMunicipio).ToList();

            return Json(ListHospitalesModel.ToDataSourceResult(request));
        }
        public JsonResult Municipios_Drop(int? idDependencia)
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
            var result = new SelectList(dbContext.CatMunicipios.Where(s=>s.transito== corporation).ToList(), "IdMunicipio", "Municipio");
            return Json(result);
        }

        public JsonResult Municipios_DropChange(int idEntidad,int? CORP)
        {

            var corp = CORP;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var tt = _catMunicipiosService.GetMunicipiosPorEntidad(idEntidad,(int)corp);

            //tt.Add(new CatMunicipiosModel() { IdMunicipio = 1, Municipio = "No aplica" });
            //tt.Add(new CatMunicipiosModel() { IdMunicipio = 2, Municipio = "No especificado" });

            var result = new SelectList(tt, "IdMunicipio", "Municipio");

            return Json(result);
        }




        #endregion


        #region Acciones a base de datos

        public void CrearHospital(CatHospitalesModel model)
        {
            //{
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var corporation = corp < 2 ? 1 : corp;

			CatHospitales hospital = new CatHospitales();
            hospital.IdHospital = model.IdHospital;
            hospital.NombreHospital = model.NombreHospital;
            hospital.IdMunicipio = model.IdMunicipio;
            hospital.Estatus = 1;
            hospital.transito = (int)corporation;
            hospital.FechaActualizacion = DateTime.Now;
            dbContext.CatHospitales.Add(hospital);
            dbContext.SaveChanges();
        }

        public void EditarHospital(CatHospitalesModel model)
        {
            var corp = model.Corp;

            if (corp == null)
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            }
            var corporation = corp < 2 ? 1 : corp; 
			CatHospitales hospital = new CatHospitales();
            hospital.IdHospital = model.IdHospital;
            hospital.NombreHospital = model.NombreHospital;
            hospital.IdMunicipio = model.IdMunicipio;
            hospital.Estatus = model.Estatus;
            hospital.transito= (int)corporation;
            hospital.FechaActualizacion = DateTime.Now;
            dbContext.Entry(hospital).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void EliminarHospital(CatHospitalesModel model)
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var corporation = corp < 2 ? 1 : corp;
			CatHospitales hospital = new CatHospitales();
            hospital.IdHospital = model.IdHospital;
            hospital.NombreHospital = model.NombreHospital;
            hospital.IdMunicipio = model.IdMunicipio;
            hospital.Estatus = 0;
            hospital.transito = corporation;
            hospital.FechaActualizacion = DateTime.Now;
            dbContext.Entry(hospital).State = EntityState.Modified;
            dbContext.SaveChanges();

        }




        public CatHospitalesModel GetHospitalByID(int IdHospital)
        {
            try
            {
                var hospital = (from catHospitales in dbContext.CatHospitales
                                where catHospitales.IdHospital == IdHospital
                                select new CatHospitalesModel
                                {
                                    IdHospital = catHospitales.IdHospital,
                                    NombreHospital = catHospitales.NombreHospital,
                                    IdMunicipio = catHospitales.IdMunicipio,
                                    Estatus = catHospitales.Estatus,
                                }).FirstOrDefault();

                if (hospital == null)
                {
                    // Manejar el caso en que el hospital no se encuentra
                    throw new Exception("Hospital no encontrado.");
                }

                return hospital;
            }
            catch (Exception ex)
            {
                // Manejar la excepción y registrar el error
                Console.WriteLine("Error al obtener el hospital: " + ex.Message);
                throw;
            }
        }



        public List<CatHospitalesModel> GetHospitales()
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var corporation = corp < 2 ? 1 : corp;
			var ListHospitalesModel = (from catHospitales in dbContext.CatHospitales.ToList()
                                       join Municipios in dbContext.CatMunicipios.ToList()
                                       on catHospitales.IdMunicipio equals Municipios.IdMunicipio
                                       join estatus in dbContext.Estatus.ToList()
                                       on catHospitales.Estatus equals estatus.estatus
                                       where catHospitales.transito==corporation
                                       select new CatHospitalesModel
                                       {
                                           IdHospital = catHospitales.IdHospital,
                                           NombreHospital = catHospitales.NombreHospital,
                                           IdMunicipio = catHospitales.IdMunicipio,
                                           Municipio = Municipios.Municipio,
                                           Estatus = catHospitales.Estatus,
                                           estatusDesc = estatus.estatusDesc,

                                       }).ToList();
            return ListHospitalesModel;
        }
        #endregion



    }
}
