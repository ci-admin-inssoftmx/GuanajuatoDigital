using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Drawing;
using System;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography;
using Kendo.Mvc.Extensions;
using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using static GuanajuatoAdminUsuarios.RESTModels.AnulacionDocumentoRequestModel;
using GuanajuatoAdminUsuarios.RESTModels;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Authorization;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;
using Azure;
using static Kendo.Mvc.UI.UIPrimitives;
using DocumentFormat.OpenXml.Spreadsheet;

namespace GuanajuatoAdminUsuarios.Controllers
{


    [Authorize]
    public class CancelarInfraccionController : BaseController
    {

        private readonly ICancelarInfraccionService _cancelarInfraccionService;
        private readonly IAnulacionDocumentoService _anulacionDocumentoService;
		private readonly IBitacoraService _bitacoraServices;

		public CancelarInfraccionController(ICancelarInfraccionService cancelarInfraccionService, IAnulacionDocumentoService anulacionDocumentoService, IBitacoraService bitacoraService)
        {
            _cancelarInfraccionService = cancelarInfraccionService;
            _anulacionDocumentoService = anulacionDocumentoService;
            _bitacoraServices = bitacoraService;
        }

        public IActionResult Index(CancelarInfraccionModel cancelarInfraccionService)
        {
        
                return View("CancelarInfraccion");
        }
        public IActionResult Index2(CancelarInfraccionModel cancelarInfraccionService)//en finanzas
        {

            return View("CancelarInfraccionFinanzas");
        }



        [HttpPost]
        public ActionResult ObtenerInfracciones(CancelarInfraccionModel model, string FolioInfraccion)
        {

            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var ListInfraccionesModel = _cancelarInfraccionService.ObtenerInfraccionPorFolio(FolioInfraccion,corp);
            if (ListInfraccionesModel == null || ListInfraccionesModel.Count == 0)
            {
                TempData["ErrorNoCoinciencia"] = "No se encontraron infracciones con el folio especificado.";
                return PartialView("_ListadoCancelarInfraccion"); 
            }

            return PartialView("_ListadoCancelarInfraccion", ListInfraccionesModel);
        }

        [HttpPost]
        public ActionResult ObtenerInfracciones2(CancelarInfraccionModel model, string FolioInfraccion)
        {


            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;


            var ListInfraccionesModel = _cancelarInfraccionService.ObtenerInfraccionPorFolioFinanzas(FolioInfraccion, corp);

            if (ListInfraccionesModel == null || ListInfraccionesModel.Count == 0)
            {
                TempData["ErrorNoCoinciencia"] = "No se encontraron infracciones con el folio especificado.";
                return PartialView("_ListadoCancelarInfraccionFinanzas");
            }

            return PartialView("_ListadoCancelarInfraccionFinanzas", ListInfraccionesModel);
        }


        public ActionResult MostrarDetalle(CancelarInfraccionModel model, int Id)
        {
            var ListInfraccionesModel = _cancelarInfraccionService.ObtenerDetalleInfraccion(Id);
            return PartialView("_DetalleInfraccion",ListInfraccionesModel);

        }
        public ActionResult MostrarDetalleCanelar(CancelarInfraccionModel model, int Id) //en finanzas
        {
            var ListInfraccionesModel = _cancelarInfraccionService.ObtenerDetalleInfraccion(Id);
            return PartialView("_DetalleInfraccionCancelar", ListInfraccionesModel);

        }
        

        [HttpPost]
        public IActionResult IniciarCancelacion(CancelarInfraccionModel model, int IdInfraccion, string OficioRevocacion)
        {

            var ListInfraccionesModel = _cancelarInfraccionService.CancelarInfraccionBD(IdInfraccion, OficioRevocacion);

			var ip = HttpContext.Connection.RemoteIpAddress.ToString();
			var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(IdInfraccion, ip, string.Format("Se cancela la infraccion con folio {0}.", ListInfraccionesModel.FolioInfraccion), "Cancelacion de infraccion", "delete", user);

            return View("CancelarInfraccion");
        }

        [HttpPost]
        public IActionResult IniciarCancelacionFinanzas(CancelarInfraccionModel model, int IdInfraccion)
        {
            HttpContext.Session.SetInt32("IdInfraccion", IdInfraccion);
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(IdInfraccion, ip, string.Format("Inicia proceso de cancelación en finanzas de la infracción con folio {0}.", model.FolioInfraccion + " con Estatus actual: " + model.descEstatusProceso), "Cancelacion de infraccion (Finanzas)", "Delete", user);

            //var ListInfraccionesModel = _cancelarInfraccionService.CancelarInfraccionFinanzas(IdInfraccion);

            //El final de insertar en bitacora esta en el proceso de cancelar inf en finanzas
            return View("CancelarInfraccionFinanzas");
        }

        public IActionResult AnulacionDocumento(string folio_infraccion, int idOficina)
        {



            string prefijo = (idOficina == 1) ? "TTO-PEC" : (idOficina == 2) ? "TTE-M" : "";
            RootAnulacionDocumentoRequest rootRequest = new RootAnulacionDocumentoRequest();
            MT_Consulta_documento mTConsultaDocumento = new MT_Consulta_documento(); 
            mTConsultaDocumento.DOCUMENTO = prefijo+folio_infraccion;
            mTConsultaDocumento.USUARIO = "INNSJACOB";
            mTConsultaDocumento.PASSWORD = "123456";
            rootRequest.MT_Consulta_documento = mTConsultaDocumento;
             
            var result = _anulacionDocumentoService.CancelarMultasTransitoFinanzas(rootRequest);
			var ip = HttpContext.Connection.RemoteIpAddress.ToString();
			var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);

			ViewBag.Pension = result;
            return Json(result);
        }

        public IActionResult EliminarFinanzas(string folio_infraccion, int idOficina)
        {
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();

            int IdInfraccion = HttpContext.Session.GetInt32("IdInfraccion") ?? 0;
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            _bitacoraServices.BitacoraWS("Inicia ws AnulacionDocumento", CodigosWs.C4023, new { idInfraccion= IdInfraccion, folio_infraccion = folio_infraccion});
            //string prefijo = (idOficina == 1) ? "TTO-PEC" : (idOficina == 2) ? "TTE-M" : "";
            string prefijo = (idDependencia == 1) ? "TTO-" : (idDependencia == 0) ? "TTE-" : "";
            RootAnulacionDocumentoRequest rootRequest = new RootAnulacionDocumentoRequest();
            MT_Consulta_documento mTConsultaDocumento = new MT_Consulta_documento();
            mTConsultaDocumento.DOCUMENTO = prefijo + folio_infraccion;
            mTConsultaDocumento.USUARIO = "INNSJACOB";
            mTConsultaDocumento.PASSWORD = "123456";
            rootRequest.MT_Consulta_documento = mTConsultaDocumento;
            var result = _anulacionDocumentoService.CancelarMultasTransitoFinanzas(rootRequest);
            var msn= result.MT_AnulacionDocumento_res.result.WMESSAGE;
            if (result.MT_AnulacionDocumento_res.result.WTYPE == "S")
            {
                _cancelarInfraccionService.CancelarInfraccionFinanzas(IdInfraccion);
                _bitacoraServices.insertBitacora(IdInfraccion, ip, string.Format("Termina proceso de cancelación en finanzas de la infracción con folio {0}.", folio_infraccion + " Estatus actual: Capturada"), "Cancelacion de infraccion (Finanzas)", "Delete", user);
                _bitacoraServices.BitacoraWS("Finaliza ws AnulacionDocumento", CodigosWs.C4024, new { idInfraccion = IdInfraccion, folio_infraccion = folio_infraccion, message = msn, result = result, success = true });
                _cancelarInfraccionService.CancelarInfraccionFinanzas(IdInfraccion);
            }
            else
            {
                _bitacoraServices.insertBitacora(IdInfraccion, ip, string.Format("Termina proceso de cancelación en finanzas de la infracción con folio {0}.", folio_infraccion + " No se actualiza el estatus, succes: false por parte de finanzas"), "Cancelacion de infraccion (Finanzas)", "Delete", user);

                _bitacoraServices.BitacoraWS("Finaliza ws AnulacionDocumento (con Error)", CodigosWs.C4024, new { idInfraccion = IdInfraccion, folio_infraccion = folio_infraccion, message = msn, result = result, success = false });
            }
            ViewBag.Pension = result;
            return Json(result);
        }

    }

}
