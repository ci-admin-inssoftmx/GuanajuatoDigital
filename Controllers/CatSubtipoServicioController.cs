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

    public class CatSubtipoServicioController : BaseController
    {
        private readonly ICatSubtipoServicio _catSubtipoServicio;
        public CatSubtipoServicioController(
           ICatSubtipoServicio catSubtipoServicio)
        {
            _catSubtipoServicio = catSubtipoServicio;
        }

        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetSubtipos([DataSourceRequest] DataSourceRequest request, int idTipoServicio)
        {
            var ListSubtipoModel = _catSubtipoServicio.ObtenerSubtiposActivos();
            if (idTipoServicio != 0)
                ListSubtipoModel = ListSubtipoModel.Where(s => s.idTipoServicio == idTipoServicio).ToList();
            return Json(ListSubtipoModel.ToDataSourceResult(request));
        }


        #region Modal Action


        [HttpPost]
        public ActionResult AgregarSubTipoServicioModal()
        {

            return PartialView("_Crear");
        }


        public ActionResult EditarSubtipoServicioModal(int idSubtipoServicio)
        {

            var subtipoServicioModel = _catSubtipoServicio.ObtenerSubtipoByID(idSubtipoServicio);
            return PartialView("_Editar", subtipoServicioModel);
        }


        [HttpPost]
        public ActionResult AgregarSubTipoServicio(CatSubtipoServicioModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("servicio");
            if (ModelState.IsValid)
            {


                _catSubtipoServicio.CrearSubtipo(model);
                var ListTiposServicio = _catSubtipoServicio.ObtenerSubtiposActivos();
                return Json(ListTiposServicio);
            }

            return PartialView("_Crear");
        }

        public ActionResult EditarSubtipoServicioBD(CatSubtipoServicioModel model)
        {
            bool switchSubtipoServicio = Request.Form["subtipoServicioSwitch"].Contains("true");
            model.estatus = switchSubtipoServicio ? 1 : 0;
            var errors = ModelState.Values.Select(s => s.Errors);
            ModelState.Remove("servicio");
            if (ModelState.IsValid)
            {


                _catSubtipoServicio.EditarSubtipo(model);
                var ListTiposServicio = _catSubtipoServicio.ObtenerSubtiposActivos();
                return Json(ListTiposServicio);
            }
            return PartialView("_Editar");
        }
        #endregion


    }
}

