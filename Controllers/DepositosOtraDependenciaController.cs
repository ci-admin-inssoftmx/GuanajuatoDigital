/*
 * Descripción:
 * Proyecto: Sistema de Infracciones y Accidentes
 * Fecha de creación: Sunday, February 18th 2024 9:40:13 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Tue Feb 27 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */

using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class DepositosOtraDependenciaController : BusquedaVehiculoPropietarioController
    {
        #region Variables
        private readonly IBitacoraService _bitacoraService;
        #endregion

        #region Constructor
        public DepositosOtraDependenciaController(IBitacoraService bitacoraService, ICatDictionary catDictionary) : base(catDictionary) {
            _bitacoraService = bitacoraService;
        }

        #endregion
        public IActionResult Depositos()
        {
            return View("DepositosOtraDependencia");
        }
        /// <summary>
        /// Busca los datos de un vehiculo y muestra el componente de datos
        /// </summary>
        /// <param name="vehiculosService"></param>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
         [HttpGet]
        public IActionResult MostrarDatosVehiculo(int idVehiculo)
        {
            return ViewComponent("VehiculoPropietarioDatos", new { idVehiculo });
        }
        /// <summary>
        /// Guarda un registro de un deposito asociado a otra dependencia en la bd
        /// </summary>
        /// <param name="ingresarVehiculosService"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 

        [HttpPost]

        public ActionResult GuardarDeposito([FromServices] IIngresarVehiculosService ingresarVehiculosService,[FromServices] IVehiculosService vehiculoService, SolicitudDepositoOtraDependenciaModel model)
        {
            int idOficina = (int)HttpContext.Session.GetInt32("IdOficina");
            int idPension = (int)HttpContext.Session.GetInt32("IdPension");

            //Se busca el vehiculo y se asigna al objeto
            model.Vehiculo = vehiculoService.GetVehiculoById(model.Vehiculo.idVehiculo);

            int idDeposito = ingresarVehiculosService.GuardarDepositoOtraDependencia(model, idOficina, idPension);

            if (idDeposito < 0)
            {
                return Json(new { success = false });
            }
            _bitacoraService.BitacoraDepositos(idDeposito, "Otra Dependencia", CodigosDepositos.C3014, model);
			return Json(new { success = true,redirectTo=Url.Action("Index","IngresarVehiculo") });
        }

        #region Catalogos
        public JsonResult DependenciaEnvia_Drop([FromServices] ICatDependenciaEnviaService catDependenciaEnviaService)
        {
            var result = new SelectList(catDependenciaEnviaService.ObtenerDependenciasEnviaActivas(), "id", "nombre");
            return Json(result);
        }

        public JsonResult TipoMotivoIngreso_Drop([FromServices] ICatTipoMotivoIngresoService catTipoMotivoIngresoService)
        {
            var result = new SelectList(catTipoMotivoIngresoService.ObtenerTiposMotivoIngresoActivos(), "id", "nombre");
            return Json(result);
        }

        public JsonResult Municipios_Drop2([FromServices] ICatMunicipiosService catMunicipiosService)
        {
			var corp = User.FindFirst(CustomClaims.esPension).Value == "1" ? 1 : HttpContext.Session.GetInt32("IdDependencia").Value;

			var result = new SelectList(catMunicipiosService.GetMunicipiosPorEntidad(CatEntidadesModel.GUANAJUATO,corp), "IdMunicipio", "Municipio");
            return Json(result);
        }
		
        public JsonResult Municipios_Drop3Corporacion([FromServices] ICatMunicipiosService catMunicipiosService)
		{
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var result = new SelectList(catMunicipiosService.GetMunicipiosActivePorCorporacion(corp), "IdMunicipio", "Municipio");
			return Json(result);
		}
		public JsonResult Carreteras_Drop([FromServices] ICatCarreterasService catCarreterasService, int idMunicipio)
        {
            var result = new SelectList(catCarreterasService.GetCarreterasParaIngreso(idMunicipio), "IdCarretera", "Carretera");
            return Json(result);
        }


        public JsonResult Tramos_Drop([FromServices] ICatTramosService catTramosService, int carreteraDDValue)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(catTramosService.ObtenerTamosPorCarretera(carreteraDDValue, corp), "IdTramo", "Tramo");
            return Json(result);
        }
        #endregion
    }
}