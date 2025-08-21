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
using System.Linq;
using System.Runtime.Intrinsics.Arm;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class SalidaVehiculosController : BaseController
    {
        private readonly ISalidaVehiculosService _salidaVehiculosService;
        private readonly ICatMarcasVehiculosService _catMarcasVehiculosService;
        private readonly IMarcasVehiculos _marcaServices;
        private readonly IPlacaServices _placaServices;
		private readonly IBitacoraService _bitacoraServices;



		public SalidaVehiculosController(ISalidaVehiculosService salidaVehiculosService, ICatMarcasVehiculosService catMarcasVehiculosService,
            IMarcasVehiculos marcaServices, IPlacaServices placaServices,IBitacoraService bitacoraServices)
        {
            _salidaVehiculosService = salidaVehiculosService;
            _catMarcasVehiculosService = catMarcasVehiculosService;
            _marcaServices = marcaServices;
            _placaServices = placaServices;
            _bitacoraServices = bitacoraServices;

		}
        public IActionResult Index()
        {
                return View();
        }
        public JsonResult Marcas_Read()
        {
            int idPension = HttpContext.Session.GetInt32("IdPension") ?? 0;
            var result = new SelectList(_marcaServices.GetMarcas(), "IdMarcaVehiculo", "MarcaVehiculo");
            return Json(result);
        }
        public JsonResult Placas_Read()
        {
            //int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int idPension = HttpContext.Session.GetInt32("IdPension") ?? 0;

            var result = new SelectList(_placaServices.GetPlacasSalidas(idPension), "IdDepositos", "Placa");
            return Json(result);
        }
        public IActionResult ajax_BusquedaIngresos(SalidaVehiculosModel model)
        {
           
                int idPension = HttpContext.Session.GetInt32("IdPension") ?? 0;

                var listaDepositos = _salidaVehiculosService.ObtenerIngresos(model, idPension);
                return Json(listaDepositos);
            }
           
            public IActionResult DatosDeposito(int iDp)
        {
            HttpContext.Session.SetInt32("idDeposito", iDp);
            int idPension = HttpContext.Session.GetInt32("IdPension") ?? 0;

            var infoDeposito = _salidaVehiculosService.DetallesDeposito(iDp, idPension);
			_bitacoraServices.BitacoraDepositos(iDp, "Salida de Vehiculo Tránsito y Transporte", CodigosDepositos.C3019, infoDeposito);

			return View(infoDeposito);
        }
        public IActionResult DatosServicio(int iDp)
        {
            HttpContext.Session.SetInt32("idDeposito", iDp);
            int idPension = HttpContext.Session.GetInt32("IdPension") ?? 0;

            var infoDeposito = _salidaVehiculosService.DetallesDepositoOtraDep(iDp, idPension);
			_bitacoraServices.BitacoraDepositos(iDp, "Salida de Vehiculo Otra Dependencia", CodigosDepositos.C3020, infoDeposito);

			return View(infoDeposito);
        }
        public JsonResult GetGruasAsignadas([DataSourceRequest] DataSourceRequest request)
        {
            int iDp = HttpContext.Session.GetInt32("idDeposito") ?? 0; 
            var ListFactores = _salidaVehiculosService.ObtenerDatosGridGruas(iDp);        
            return Json(ListFactores.ToDataSourceResult(request));
        }
      

        public ActionResult ModalCostosGrua(int idDeposito, int idGrua)
        {
            var DatosGruaSeleccionada = _salidaVehiculosService.CostosServicio(idDeposito, idGrua);

            return PartialView("_ModalCostosServicio", DatosGruaSeleccionada);
        }
        public ActionResult GuardarCostos(CostosServicioModel model)
        {
            int iDp = HttpContext.Session.GetInt32("idDeposito") ?? 0;

            var DatosGruaSeleccionada = _salidaVehiculosService.ActualizarCostos(model);
            List<SalidaVehiculosModel> gruas = _salidaVehiculosService.ObtenerTotal(iDp);
            float sumaCostoTotal = gruas.Sum(grua => grua.costoTotalPorGrua);

            var modelo = new SalidaVehiculosModel
            {
                costoTotalPorGrua = sumaCostoTotal, costoTotalTodasGruas = sumaCostoTotal

			};
            ViewBag.Total = sumaCostoTotal;
            return Json(modelo);
        }    

        public ActionResult GuardarDatosSalida(SalidaVehiculosModel model)
        {

            var DatosGruaSeleccionada = _salidaVehiculosService.GuardarInforSalida(model);
			object data = new { idDeposito = model.idDeposito };
			_bitacoraServices.BitacoraDepositos(model.idDeposito.HasValue?model.idDeposito.Value:0, "Salida de Vehiculo Tránsito y Transporte", CodigosDepositos.C3021, model);

			return PartialView("_ListadoGruas");
        }

        public ActionResult GuardarDatosSalidaDep(SalidaVehiculosModel model)
        {

            var DatosGruaSeleccionada = _salidaVehiculosService.GuardarInforSalidaOtrasDep(model);
			object data = new { idDeposito = model.idDeposito };
			_bitacoraServices.BitacoraDepositos(model.idDeposito.HasValue?model.idDeposito.Value:0, "Salida de Vehiculo Otra Dependencia", CodigosDepositos.C3022, model);
			return View("Index");
        }

    }
}
