using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Controllers
{


    [Authorize]
    public class BusquedaDepositoController : BaseController
    {
        private readonly IBusquedaDepositoService _busquedaDepositoService;
        private readonly IGruasService _gruasService;



        public BusquedaDepositoController(IBusquedaDepositoService busquedaDepositoService, IGruasService gruasService)
        {
            _busquedaDepositoService = busquedaDepositoService;
            _gruasService = gruasService;
        }
        public IActionResult Index()
        {

                return View();
            }
        public JsonResult GetAllDepositos([DataSourceRequest] DataSourceRequest request)
        {
            int idPension = HttpContext.Session.GetInt32("IdPension") ?? 0;

            var listaDepositos = _busquedaDepositoService.ObtenerTodosDepositos(idPension);

            return Json(listaDepositos.ToDataSourceResult(request));
        }
        public IActionResult ajax_BusquedaDepositos(BusquedaDepositoModel model)
        {

                int idPension = HttpContext.Session.GetInt32("IdPension") ?? 0;

            var listaDepositos = _busquedaDepositoService.ObtenerDepositos(model, idPension);
            return Json(listaDepositos);
            }   

public ActionResult ModalDetalleGrua(int Id)
        {
            var model = _gruasService.GetGruasConcesionariosByIdCocesionario(Id);

            return PartialView("_DetallesGrua", model);
        }
    }
}
