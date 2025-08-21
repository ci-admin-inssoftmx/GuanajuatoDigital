using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class CatAsientoController : BaseController
    {
        private readonly ICatAsientoService _catAsientoservice;

        public CatAsientoController(ICatAsientoService catAsientoservice)
        {
            _catAsientoservice = catAsientoservice;

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
