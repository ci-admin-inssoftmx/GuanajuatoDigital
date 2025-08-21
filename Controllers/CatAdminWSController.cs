using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class CatAdminWSController : BaseController
    {
        private readonly ICatAdminWsService _catAdminWSService;



        public CatAdminWSController(ICatAdminWsService catAdminWSService)
        {
            _catAdminWSService = catAdminWSService;
        }
        public IActionResult Index()
        {
            return View();
        }


        public JsonResult GetServices([DataSourceRequest] DataSourceRequest request)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var corporation = corp < 2 ? 1 : corp;
            var ListServicios = _catAdminWSService.ObtenerWebServices(corporation);

            return Json(ListServicios.ToDataSourceResult(request));
        }
        public ActionResult CrearServiceModal()
        {

            return PartialView("_Crear");
        }
        public ActionResult Ajax_CrearService(AppSettingsModel model)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var corporation = corp < 2 ? 1 : corp;
            _catAdminWSService.CrearService(model,corporation);
            var ListServicios = _catAdminWSService.ObtenerWebServices();

            return Json(ListServicios);
        }
        public ActionResult Ajax_EditarService(AppSettingsModel model)
        {
            bool switchEstatus = Request.Form["WsSwitch"].Contains("true");
            model.IsActive = switchEstatus;
            _catAdminWSService.EditarService(model);
            var ListServicios = _catAdminWSService.ObtenerWebServices();

            return Json(ListServicios);
        }
        public ActionResult EditarServiceModal(int idService)
        {
           
            var serviceModel = _catAdminWSService.GetServiceById(idService);

            return PartialView("_Editar", serviceModel);
        }

    }
}
