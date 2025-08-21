using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Util;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        IPagosInfraccionesService _PagosInfraccionesService;
        ILogTraficoService _LogTraficoService;
        IBitacoraService _bit;
        public PagosController(IPagosInfraccionesService PagosInfraccionesService, ILogTraficoService LogTraficoService,IBitacoraService bit)
        {
            _PagosInfraccionesService = PagosInfraccionesService;
            _LogTraficoService = LogTraficoService;
            _bit = bit;
        }
        // POST api/<PagosController>
        [HttpPost]
        public IActionResult Post([FromBody] InfoPagoModel InfoPago)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                _bit.BitacoraWS("InfraccionPagoWS", CodigosWs.C4005, InfoPago,"0",ip,"0");
                Logger.Info("4005-Se inicia la invocación al WS InfraccionPagoWS: (Envio: " + JsonConvert.SerializeObject(InfoPago, Formatting.Indented) + " )");
                var resp = _PagosInfraccionesService.Pagar(InfoPago);
                _bit.BitacoraWS("finaliza InfraccionPagoWS:", CodigosWs.C4006, resp,"0",ip,"0");
                Logger.Info("4006-Se finaliza la generación del WS InfraccionPagoWS: (Envio: " + JsonConvert.SerializeObject(resp, Formatting.Indented) + " )");
                LogTraficoModel LogModel = new LogTraficoModel();
                LogModel.jsonRequest = JsonConvert.SerializeObject(InfoPago);
                LogModel.jsonResponse = JsonConvert.SerializeObject(resp);
                LogModel.fecha = DateTime.Now;
                LogModel.valor = InfoPago.FolioInfraccion;
                LogModel.api = nameof(PagosController) + "/Post";
                _LogTraficoService.CreateLog(LogModel);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponsePagoModel() { CodigoRespuesta = 6, HasError = true, Mensaje = "Error al pagar: " + ex.Message });
            }
        }

        // PUT api/<PagosController>/5
        [HttpDelete]
        public IActionResult Delete([FromBody] ReversaPagoModel ReversaPago)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                _bit.BitacoraWS("AnulacionDocumento", CodigosWs.C4023, ReversaPago);
                Logger.Info("4023-Se inicia la invocación al WS AnulacionDocumento:-(Envio: " + JsonConvert.SerializeObject(ReversaPago, Formatting.Indented) + " )");
                var resp = _PagosInfraccionesService.ReversaDePago(ReversaPago);
                _bit.BitacoraWS("AnulacionDocumento", CodigosWs.C4024, resp);
                Logger.Info("4024-Se finaliza la generación del WS AnulacionDocumento:-(Envio: " + JsonConvert.SerializeObject(resp, Formatting.Indented) + " )");
                LogTraficoModel LogModel = new LogTraficoModel();
                LogModel.jsonRequest = JsonConvert.SerializeObject(ReversaPago);
                LogModel.jsonResponse = JsonConvert.SerializeObject(resp);
                LogModel.fecha = DateTime.Now;
                LogModel.api = nameof(PagosController) + "/Delete";
                _LogTraficoService.CreateLog(LogModel);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponsePagoModel() { CodigoRespuesta = 6, HasError = true, Mensaje = "Error al eliminar el pago: " + ex.Message });
            }
        }
    }
}
