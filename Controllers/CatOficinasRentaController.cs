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
    public class CatOficinasRentaController : BaseController
    {
        private readonly ICatOficinasRentaService _catOficinasRentaService;
        private readonly IDelegacionesService _delegacionesService;

        public CatOficinasRentaController(ICatOficinasRentaService catOficinasRentaService, IDelegacionesService delegacionesService)
        {
            _catOficinasRentaService = catOficinasRentaService;
            _delegacionesService = delegacionesService;
        }
        DBContextInssoft dbContext = new DBContextInssoft();






        #region Modal Action
        public ActionResult Index()
        {
            var catOficinasDTO = new CatOficinasRentaDTO();
            var ListOficinasRentaModel = GetOficinas();
            catOficinasDTO.OficinasRentaModel = ListOficinasRentaModel;
            return View("Index", catOficinasDTO);
        }


        public ActionResult Index2(int idDelegacion)
        {
            var ListOficinasRentaModel = ObtenerPorDel(idDelegacion);
            return View("Index2", ListOficinasRentaModel);
        }

        [HttpPost]
        public ActionResult AgregarOficinaRentaModal()
        {

            Delegaciones_Drop();
            return PartialView("_Crear");
        }



        public ActionResult EditarOficinaRentaModal(int IdOficinaRenta, int IdDelegacion)
        {

            Delegaciones_Drop();
            var oficinasRentaModel = GetOficinaRentaByID(IdOficinaRenta);
            return PartialView("_Editar", oficinasRentaModel);
        }


        public ActionResult EliminarOficinaRentaModal(int IdOficinaRenta)
        {
            Delegaciones_Drop();
            var oficinasRentaModel = GetOficinaRentaByID(IdOficinaRenta);
            return PartialView("_Eliminar", oficinasRentaModel);
        }



        [HttpPost]
        public ActionResult AgregarOficinaRenta(CatOficinasRentaModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreOficina");
            if (ModelState.IsValid)
            {


                CrearOficinaRenta(model);
                var ListOficinasRentaModel = GetOficinas();
                return Json(ListOficinasRentaModel);
            }
            Delegaciones_Drop();
            return PartialView("_Crear");
        }

        public ActionResult EditarOficinaRentaMod(CatOficinasRentaModel model)
        {
            bool switchOficinasRenta = Request.Form["oficinasRentaSwitch"].Contains("true");
            model.Estatus = switchOficinasRenta ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreOficina");
            if (ModelState.IsValid)
            {


                EditarOficinaRenta(model);
                var ListOficinasRentaModel = GetOficinas();
                return Json(ListOficinasRentaModel);
            }
            Delegaciones_Drop();
            return PartialView("_Editar");
        }

      /*  public ActionResult EliminarOficinasRentaMod(CatOficinasRentaModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("NombreOficina");
            if (ModelState.IsValid)
            {


                EliminarOficinaRenta(model);
                var ListOficinasRentaModel = GetOficinas();
                return Json(ListOficinasRentaModel);
            }
            Delegaciones_Drop();
            return PartialView("_Eliminar");
        }*/

        public JsonResult GetOfiRent([DataSourceRequest] DataSourceRequest request, int idDelegacion,int? idDependencia)
        {
            var corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);

       
            var ListOficinasRentaModel = GetOficinas();

            if (idDelegacion > 0)
                ListOficinasRentaModel = ListOficinasRentaModel.Where(x => x.IdDelegacion == idDelegacion).ToList();

            return Json(ListOficinasRentaModel.ToDataSourceResult(request));
        }

        public JsonResult GetOfiPorDel([DataSourceRequest] DataSourceRequest request, int idDelegacion)
        {
            var ListOficinasRentaModel = ObtenerPorDel(idDelegacion);

            return Json(ListOficinasRentaModel.ToDataSourceResult(request));
        }




        #endregion


        #region Acciones a base de datos

        public void CrearOficinaRenta(CatOficinasRentaModel model)
        {
            CatOficinasRenta oficina = new CatOficinasRenta();
            oficina.IdOficinaRenta = model.IdOficinaRenta;
            oficina.NombreOficina = model.NombreOficina;
            oficina.IdDelegacion = model.IdDelegacion;
            oficina.Estatus = 1;
            oficina.FechaActualizacion = DateTime.Now;
            dbContext.CatOficinasRenta.Add(oficina);
            dbContext.SaveChanges();
        }

        public void EditarOficinaRenta(CatOficinasRentaModel model)
        {
            CatOficinasRenta oficina = new CatOficinasRenta();
            oficina.IdOficinaRenta = model.IdOficinaRenta;
            oficina.NombreOficina = model.NombreOficina;
            oficina.IdDelegacion = model.IdDelegacion;
            oficina.Estatus = model.Estatus;
            oficina.FechaActualizacion = DateTime.Now;
            dbContext.Entry(oficina).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void EliminarOficinaRenta(CatOficinasRentaModel model)
        {

            CatOficinasRenta oficina = new CatOficinasRenta();
            oficina.IdOficinaRenta = model.IdOficinaRenta;
            oficina.NombreOficina = model.NombreOficina;
            oficina.IdDelegacion = model.IdDelegacion;
            oficina.Estatus = 0;
            oficina.FechaActualizacion = DateTime.Now;
            dbContext.Entry(oficina).State = EntityState.Modified;
            dbContext.SaveChanges();


        }



        public JsonResult Delegaciones_Drop()
        {
            var tipo = 1; //Convert.ToInt32(HttpContext.Session.GetInt32("IdDependencia").ToString());
            var result = new SelectList(_delegacionesService.GetDelegaciones().Where(x => x.Transito == (tipo == 1) ? true : false), "IdDelegacion", "Delegacion");
            return Json(result);
        }
        public CatOficinasRentaModel GetOficinaRentaByID(int IdOficinaRenta)
        {

            var productEnitity = dbContext.CatOficinasRenta.Find(IdOficinaRenta);

            var oficinasRentaModel = (from catOficinasRenta in dbContext.CatOficinasRenta.ToList()
                                      select new CatOficinasRentaModel

                                      {
                                          IdOficinaRenta = catOficinasRenta.IdOficinaRenta,
                                          NombreOficina = catOficinasRenta.NombreOficina,
                                          IdDelegacion = catOficinasRenta.IdDelegacion,
                                          Estatus = catOficinasRenta.Estatus


                                      }).Where(w => w.IdOficinaRenta == IdOficinaRenta).FirstOrDefault();

            return oficinasRentaModel;
        }

        /// <summary>
        /// Linq es una tecnologia de control de datos (excel, txt,EF,sqlclient etc)
        /// para la gestion un mejor control de la info
        /// </summary>
        /// <returns></returns>
        public List<CatOficinasRentaModel> GetOficinas()
        {

           
            var tipo = 1;
            var deleg = _delegacionesService.GetDelegaciones().Where(x => x.Transito == (tipo == 1) ? true : false);
            var ListOficinasRentaModel = (from catOficinasRenta in dbContext.CatOficinasRenta.ToList()
                                          join delegaciones in deleg.ToList()
                                          on catOficinasRenta.IdDelegacion equals delegaciones.IdDelegacion
                                          join estatus in dbContext.Estatus.ToList()
                                          on catOficinasRenta.Estatus equals estatus.estatus
                                          select new CatOficinasRentaModel
                                          {
                                              IdOficinaRenta = catOficinasRenta.IdOficinaRenta,
                                              NombreOficina = catOficinasRenta.NombreOficina,
                                              DelegacionDesc = delegaciones.Delegacion,
                                              IdDelegacion = delegaciones.IdDelegacion,
                                              estatusDesc = estatus.estatusDesc,
                                              Transito = delegaciones.Transito ? 1 : 0
                                          }).ToList();
            return ListOficinasRentaModel;
        }

        public List<CatOficinasRentaModel> ObtenerPorDel(int IdDelegacion)
        {
            var tipo = 1;
            var deleg = _delegacionesService.GetDelegaciones().Where(x => x.Transito == (tipo == 1) ? true : false);
            var ListaPorDelegacion = (from catOficinasRenta in dbContext.CatOficinasRenta.ToList()
                                      join delegaciones in deleg.ToList()
                                      on catOficinasRenta.IdDelegacion equals delegaciones.IdDelegacion
                                      select new CatOficinasRentaModel
                                      {
                                          IdOficinaRenta = catOficinasRenta.IdOficinaRenta,
                                          NombreOficina = catOficinasRenta.NombreOficina,
                                          DelegacionDesc = delegaciones.Delegacion,
                                          IdDelegacion = delegaciones.IdDelegacion,
                                          Transito = delegaciones.Transito ? 1 : 0

                                      }).Where(w => w.IdDelegacion == IdDelegacion).ToList();
            return ListaPorDelegacion;
        }

    }
}
#endregion