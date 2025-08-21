using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]

    public class CatEstadoVictimaController : BaseController
    {
        private readonly ICatEstadoVictimaService _catEstadoVictimaService;

        public CatEstadoVictimaController(ICatEstadoVictimaService catEstadoVictimaService)
        {
            _catEstadoVictimaService = catEstadoVictimaService;

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
