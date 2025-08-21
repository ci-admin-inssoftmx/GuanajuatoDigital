using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.LoginController;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthMAnagerController : ControllerBase
    {
        ILogTraficoService _LogTraficoService;
        IBitacoraService _bit;

        [HttpPost]
        public IActionResult Post([FromBody] AuthModel data)
        {

            AuthManager.SingOutUser(data.id);

            return Ok(data);
        }
    }
}
