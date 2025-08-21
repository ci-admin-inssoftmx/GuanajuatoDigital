using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class CatEstatusReporteController : Controller
    {
        public class BusquedaAccidentesController : BaseController
        {
            private readonly ICatEstatusReporteService _catEstatusReporteService;

            public BusquedaAccidentesController(ICatEstatusReporteService catEstatusReporteService)
            {
                _catEstatusReporteService = catEstatusReporteService;
            }

            public IActionResult Index()
            {
                return View();
            }
        }
    }
}
