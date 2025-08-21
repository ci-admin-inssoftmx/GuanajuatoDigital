using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class CatFormastrasladoController : BaseController
    {
        private readonly ICatFormasTrasladoService _catFormasTrasladoService;

        public CatFormastrasladoController(ICatFormasTrasladoService catFormasTrasladoService)
        {
            _catFormasTrasladoService = catFormasTrasladoService;
        }
        public IActionResult Index()
        {
            _catFormasTrasladoService.ObtenerFormasActivas();
            return View();
        }
    }
}
