/*using GuanajuatoAdminUsuarios.Data;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Areas.Administracion.Controllers
{
    [Authorize]
    [Area("Administracion")]
    [Route("[area]/oficinas/[action]")]
    public class OficinaController : BaseController
    {
        private OficinaService _oficinaService;
        private EntidadService _entidadService;

            public OficinaController(OficinaService oficinaService,EntidadService entidadService) {
            _oficinaService = oficinaService;
            _entidadService = entidadService;
        }

        // GET: CatOficinasController
        [HttpGet]
        [Route("")]
        public IActionResult Inicio()
        {
             return View();
        }
        public ActionResult GetOficinas([DataSourceRequest] DataSourceRequest request)
        {
            List<Oficina> listOficinas = new List<Oficina>();

            listOficinas = _oficinaService.GetOficinas();


            DataSourceResult gridOficinas = listOficinas.ToDataSourceResult(request, cm => new
            {
                Id = cm.Id,
                Descripcion = cm.Descripcion,
                Estatus = cm.Estatus
            });
            return Json(gridOficinas);
        }

        // GET: CatOficinasController/Create
        [HttpGet]
        [Route("crear")]
        public IActionResult Crear()
        {
            return View();
        }

        // POST: CatOficinasController/Create
        [HttpPost("crear")]
        [ValidateAntiForgeryToken]
        public IActionResult CrearOficina([Bind("Descripcion,IdEntidad")] Oficina oficina)
        {
            if (ModelState.IsValid)
            {
                var idUsuario = User.FindFirst("IdUsuario").Value;
                _oficinaService.GuardaOficina(oficina.Descripcion, oficina.IdEntidad, Int32.Parse(idUsuario));
                return RedirectToAction(nameof(Inicio));
            }
            return View(oficina);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var catOficina = _oficinaService.GetOficinaById(id);

            return View(catOficina);
        }

        [HttpPost("editar")]
        [ValidateAntiForgeryToken]
        public IActionResult EditarOficina([Bind("Id,Descripcion,IdEntidad,Estatus")] Oficina oficina)
        {
            if (ModelState.IsValid)
            {
                var idUsuario = User.FindFirst("IdUsuario").Value;
                _oficinaService.ActualizaOficina(oficina.Id, oficina.Descripcion, oficina.IdEntidad, oficina.Estatus, idUsuario);
                return RedirectToAction(nameof(Inicio));
            }

            return View(oficina);
        }

        public JsonResult GetEntidadesAjax()
        {
            var entidades = _entidadService.GetEntidades();

            return Json(entidades, new Newtonsoft.Json.JsonSerializerSettings());
        }
    }
}*/
