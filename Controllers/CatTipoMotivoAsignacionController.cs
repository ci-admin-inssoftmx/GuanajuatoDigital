using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class CatTipoMotivoAsignacionController : BaseController
    {
        private readonly ICatTipoMotivoAsignacionService _catTipoMotivoAsignacionService;


        public CatTipoMotivoAsignacionController(ICatTipoMotivoAsignacionService catTipoMotivoAsignacionService)
        {
            _catTipoMotivoAsignacionService = catTipoMotivoAsignacionService;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
