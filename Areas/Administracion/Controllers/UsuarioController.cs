/*using GuanajuatoAdminUsuarios.Controllers;
using GuanajuatoAdminUsuarios.Data;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Areas.Administracion.Controllers
{
    [Authorize]
    [Area("Administracion")]
    [Route("[area]/usuarios/[action]")]
    public class UsuarioController : BaseController
    {

        private readonly ILogger<InicioController> _logger;
        private UsuarioService _usuarioService;


        public UsuarioController(ILogger<InicioController> logger, UsuarioService usuarioService)
        {
            _logger = logger;
            _usuarioService = usuarioService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Inicio()
        {

            return View();
        }


        public ActionResult GetUsuarios([DataSourceRequest] DataSourceRequest request)
        {
            List<Usuario> listUsuarios = new List<Usuario>();

            listUsuarios = _usuarioService.ListarUsuarios();


            DataSourceResult gridUsuarios = listUsuarios.ToDataSourceResult(request, cm => new
            {
                cm.IdUsuario,
                cm.Nombre,
                cm.Paterno,
                cm.Materno,
                cm.Estatus


            });
            return Json(gridUsuarios);
        }
        [HttpGet]
        [Route("crear")]
        public IActionResult Crear()
        {
            return View();
        }


        [HttpPost("crear")]
        [ValidateAntiForgeryToken]
        public IActionResult CrearUsuario([Bind("Nombre,Paterno,Materno,Email,Login,Clave")] Usuario usuario)
        {

            if (ModelState.IsValid)
            {
                var idUsuario = User.FindFirst("IdUsuario").Value;

                _usuarioService.GuardaUsuario(usuario.Nombre, usuario.Paterno, usuario.Materno, usuario.Email,
                                                            usuario.Login, usuario.Clave, idUsuario);


                return RedirectToAction(nameof(Inicio));
            }
            return View( usuario);

        }


        [HttpGet]
        [Route("{id}")]
        public IActionResult Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catUsuario = _usuarioService.GetUsuario(id);

            return View(catUsuario);
        }


        [HttpPost("editar")]
        [ValidateAntiForgeryToken]
        public IActionResult EditarUsuario([Bind("IdUsuario,Nombre,Paterno,Materno,Email,Estatus,Login")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var idUsuario = User.FindFirst("IdUsuario").Value;

                 _usuarioService.ActualizaUsuario(usuario.IdUsuario, usuario.Nombre, usuario.Paterno, usuario.Materno, usuario.Email,
                                                                usuario.Login, usuario.Estatus, idUsuario);
                return RedirectToAction(nameof(Inicio));
            }

            return View(usuario);
        }

    }
}*/
