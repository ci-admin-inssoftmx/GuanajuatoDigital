using GuanajuatoAdminUsuarios.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class AdministradorBlocksController : BaseController
    {
        private readonly IAdminBlocksService _adminBlocksService;

        public AdministradorBlocksController(IAdminBlocksService adminBlocksService)
        {
            _adminBlocksService = adminBlocksService;
        }

        #region Views

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult CrearServiceModal()
        {
            var corpsBlocks =  _adminBlocksService.GetDropDownCorporaciones();
            ViewBag.corpsBlocks = corpsBlocks;
            return PartialView("_Crear");
        }


        public ActionResult EditarAlertamientoModal(int idService)
        {
            var serviceModel = _adminBlocksService.Getdata(idService);
            return PartialView("_Editar", serviceModel);
        }





        #endregion

        #region Ajax

        public IActionResult GetDataGrid([DataSourceRequest] DataSourceRequest request)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var data = _adminBlocksService.GetdataGrid();
            return Json(data.ToDataSourceResult(request));

        }

        public IActionResult Ajax_CrearService(int corporacion, string prefijoInfracciones, bool infracciones, bool accidente, bool deposito)
        {
            _adminBlocksService.CrearBlockCorporaciones(corporacion,  prefijoInfracciones,  infracciones,  accidente, deposito);
            return Json(true);
        }

        public IActionResult Ajax_EditarAlertamiento(int Id,string prefijoInfracciones, bool infraccion, bool accidentes, bool depositos)
        {
            _adminBlocksService.EditarBlockCorporaciones( Id,  prefijoInfracciones, infraccion, accidentes, depositos);

            return Json(true);
        }

        #endregion
    }
}
