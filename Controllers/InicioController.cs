using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Http;
using AdminUsuarios.Models.Commons;
using AdminUsuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using AdminUsuarios.Helpers;
using ReciboPago;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Runtime.Serialization.DataContracts;
using Org.BouncyCastle.Crypto.IO;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Text;
using System.Xml;
using System.Net.Http;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Route("")]
    [Authorize]
    public class InicioController : BaseController
    {

        private readonly ILogger<InicioController> _logger;

        public InicioController(ILogger<InicioController> logger)
        {
            _logger = logger;

        }

        //public RecibosPagoWSClient client { get; set; }

        [HttpGet("Inicio")]
        [Route("")]
        [AllowAnonymous]
        public IActionResult Index()
        {

            HttpContext.Session.Clear();
            HttpContext.SignOutAsync().Wait();

            return View("Marca");
        }

        [Route("/Principal")]
        [Authorize]
        public IActionResult Principal()
        {



            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewData["ErrorMessage"] = TempData["ErrorMessage"].ToString();
            }
            return View("Inicio");
        }


        private async Task SignInUser(int idUsuario, string nombre, string perfil)
        {
            var claims = new List<Claim>
            {
                new Claim(CustomClaims.IdUsuario, idUsuario.ToString()),
                 new Claim(CustomClaims.Nombre, nombre),
                new Claim(CustomClaims.Perfil, perfil)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }


        [Route("cerrar-sesion")]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete(".GtoAdminApp");
            HttpContext.Session.Clear();
            return Redirect("/login");
        }




        [Route("/MarcasVehiculos")]
        [Authorize]
        public IActionResult Inicio()
        {
            return View();
        }
        [Route("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
