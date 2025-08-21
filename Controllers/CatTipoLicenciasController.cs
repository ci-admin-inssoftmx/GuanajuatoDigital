using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class CatTipoLicenciasController : BaseController
    {
  
            private readonly ICatTipoLicenciasService _catTipoLicenciasService;

            private int idOficina = 0;

            public CatTipoLicenciasController(ICatTipoLicenciasService catTipoLicenciasService)
            {
            _catTipoLicenciasService = catTipoLicenciasService;
               
            }

            public IActionResult Index()
        {
            return View();
        }
    }
}
