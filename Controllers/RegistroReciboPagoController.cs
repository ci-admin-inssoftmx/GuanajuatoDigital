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
using GuanajuatoAdminUsuarios.Services;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoRequestModel;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;


namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class RegistroReciboPagoController : BaseController
    {

        private readonly IRegistroReciboPagoService _registroReciboPagoService;
        private readonly IConsultarDocumentoService _consultarDocumentoService;
        private readonly IBitacoraService _bitacoraServices;
        private readonly IAppSettingsService _appSettingsService;

        private readonly AppSettings _appSettings;


        public RegistroReciboPagoController(IRegistroReciboPagoService registroReciboPagoService, IConsultarDocumentoService consultarDocumentoService,
            IOptions<AppSettings> appSettings, IBitacoraService bitacoraService,IAppSettingsService appSettingsService
)
        {
            _registroReciboPagoService = registroReciboPagoService;
            _consultarDocumentoService = consultarDocumentoService;
            _appSettings = appSettings.Value;
            _bitacoraServices = bitacoraService;
            _appSettingsService = appSettingsService;


    }

        public IActionResult Index()
        {

            return View("RegistroReciboDePago");
        }



        [HttpPost]
        public ActionResult ObtenerInfracciones(RegistroReciboPagoModel model, string FolioInfraccion)
        {

            var q = User.FindFirst(CustomClaims.TipoOficina).Value;

            var ListInfraccionesModel = _registroReciboPagoService.ObtInfracciones(FolioInfraccion, q);
            return Json(ListInfraccionesModel);

        }

        public ActionResult Detalleinfraccion(RegistroReciboPagoModel model, int Id)
        {
            var ListInfraccionesModel = _registroReciboPagoService.ObtenerDetallePorId(Id);



            return PartialView("_DetalleRegistroDePago", ListInfraccionesModel);

        }
        public ActionResult GuardarReciboPago(string ReciboPago, float Monto, string FechaPago, string LugarPago, int IdInfraccion, float MontoCalculado)
        {

            //var date = DateTime.ParseExact(FechaPago, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var date = DateTime.Parse(FechaPago);

            var datosGuardados = _registroReciboPagoService.GuardarRecibo(ReciboPago, Monto, date, LugarPago, IdInfraccion, MontoCalculado);

            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(IdInfraccion, ip, "Infraccion", "Pagar", "WS", user);
            object data = new { };
            _bitacoraServices.BitacoraWS("GuardarReciboPago", CodigosWs.C4011, data);
            _bitacoraServices.insertBitacora(IdInfraccion, ip, string.Format("Se captura pago de infraccion por un monto de {0}", Monto), "Pagar de infraccion", "WS", user);



            return PartialView("RegistroReciboDePago");

        }

        public IActionResult ConsultarDocumento(string recibo, string idInfracc)
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
			var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            var endPointName = "ConsultarDocumentoEndPoint";
            var isActive = _appSettingsService.VerificarActivo(endPointName,corp);
            if (isActive)
            {
                _bitacoraServices.insertBitacora(Convert.ToInt32(idInfracc), ip, string.Format("Consulta de pago para la infraccion con documento {0}", recibo), "Consulta Documento por WS Finanzas", "WS", user);


                RootConsultarDocumentoRequest rootRequest = new RootConsultarDocumentoRequest();
                MTConsultaDocumento mTConsultaDocumento = new MTConsultaDocumento();
                mTConsultaDocumento.PROCESO = "GENERAL";
                mTConsultaDocumento.DOCUMENTO = recibo;
                mTConsultaDocumento.USUARIO = "INNSJACOB";
                mTConsultaDocumento.PASSWORD = "123456";
                rootRequest.MT_Consulta_documento = mTConsultaDocumento;

                object data = new { };
                _bitacoraServices.BitacoraWS("Consulta Documento", CodigosWs.C4021, rootRequest);
                var result = _consultarDocumentoService.ConsultarDocumento(rootRequest, endPointName);
                ViewBag.Pension = result;
                _bitacoraServices.BitacoraWS("Consulta Documento", CodigosWs.C4022, result);
                //var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                //var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                //_bitacoraServices.insertBitacora( Convert.ToInt32(idInfracc), ip, "Infraccion", "ConsultaP", "WS", user);


                _bitacoraServices.insertBitacora(Convert.ToInt32(idInfracc), ip, string.Format("El WS responde Monto {0} para el folio {1}", result.MT_ConsultarDocumento_res.e_doc_pago.importe, result.MT_ConsultarDocumento_res.result.FOL_MULTA), "Consulta Documento por WS Finanzas", "WS", user);

                return Json(result);
            }
            else
            {
                _bitacoraServices.insertBitacora(Convert.ToInt32(idInfracc), ip, string.Format("No esta habilitado el ws de consulta de pago"), "Consulta Documento por WS Finanzas", "WS", user);

                return Json(new { hasError = true, message = "Los servicios web no están habilitados." });
            }
        }

    }

}
