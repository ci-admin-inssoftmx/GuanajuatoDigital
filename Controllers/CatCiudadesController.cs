using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class CatCiudadesController : BaseController
        {

            private readonly ICatCiudadesService _catCiudadesService;

            public CatCiudadesController(ICatCiudadesService catCiudadesService)
            {
            _catCiudadesService = catCiudadesService;
            }
            public IActionResult Index()
        {
            return View();
        }
    }
}
