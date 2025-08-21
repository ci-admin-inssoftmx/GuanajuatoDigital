using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
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
    public class CatTipoServicioController : BaseController
    {
        private readonly ICatTipoServicio _catTipoServicio;

        public CatTipoServicioController(ICatTipoServicio catTipoServicio)
        {
            _catTipoServicio = catTipoServicio;
        }
        public IActionResult Index()
        {

            //var ListTipoServicio = _catTipoServicio.ObtenerTiposActivos();

            return View();
        }


        public JsonResult GetTipos([DataSourceRequest] DataSourceRequest request)
        {
            var ListCausasAccidentesModel = _catTipoServicio.ObtenerTiposActivos();

            return Json(ListCausasAccidentesModel.ToDataSourceResult(request));
        }


        #region Modal Action


        [HttpPost]
        public ActionResult AgregarTipoServicioModal()
        {

            return PartialView("_Crear");
        }


        public ActionResult EditarCausasAccidenteModal(int idTipoServicio)
        {

            var tipoServicioModel = _catTipoServicio.ObtenerTipoByID(idTipoServicio);
            return PartialView("_Editar", tipoServicioModel);
        }
   

        [HttpPost]
        public ActionResult AgregarTipoServicio(CatTipoServicioModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("tipoServicio");
            if (ModelState.IsValid)
            {


                _catTipoServicio.CrearTipo(model);
                var ListTiposServicio = _catTipoServicio.ObtenerTiposActivos();
                return Json(ListTiposServicio);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarTipoServicioBD(CatTipoServicioModel model)
        {
            bool switchTipoServicio = Request.Form["tipoServicioSwitch"].Contains("true");
            model.Estatus = switchTipoServicio ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("CausaAccidente");
            if (ModelState.IsValid)
            {


                _catTipoServicio.EditarTipo(model);
                var ListTiposServicio = _catTipoServicio.ObtenerTiposActivos();
                return Json(ListTiposServicio);
            }
            return PartialView("_Editar");
        }
        #endregion


    }
}
