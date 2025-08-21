using GuanajuatoAdminUsuarios.Framework;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class CatResponsablesPensionesController : BaseController
    {
        private readonly ICatResponsablesPensiones _catResponsablesPensiones;
        public CatResponsablesPensionesController(ICatResponsablesPensiones catResponsablesPensiones)
        {
            _catResponsablesPensiones = catResponsablesPensiones;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
