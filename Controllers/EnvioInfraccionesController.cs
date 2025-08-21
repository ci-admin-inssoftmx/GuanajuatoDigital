using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;



namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class EnvioInfraccionesController : BaseController
    {
        private readonly IEnvioInfraccionesService _envioInfraccionesService;
        private readonly ICatOficinasRentaService _catOficinasRentaService;
        private readonly ICrearMultasTransitoClientService _crearMultasTransitoClientService;
        private readonly IInfraccionesService _infraccionesService;
        private readonly AppSettings _appSettings;


        public EnvioInfraccionesController(IEnvioInfraccionesService envioInfraccionesService, ICatOficinasRentaService catOficinasRentaService, ICrearMultasTransitoClientService crearMultasTransitoClientService, IInfraccionesService infraccionesService,
            IOptions<AppSettings> appSettings)
        {
            _envioInfraccionesService = envioInfraccionesService;
            _catOficinasRentaService = catOficinasRentaService;
            _crearMultasTransitoClientService = crearMultasTransitoClientService;
            _infraccionesService = infraccionesService;
            _appSettings = appSettings.Value;
        }
        public IActionResult Index()
        {         
                return View();
            }
          
        
        public ActionResult BusquedaInfracciones(EnvioInfraccionesModel model)
        {
            var resultadoBusqueda = _envioInfraccionesService.ObtenerInfracciones(model);
            return Json(resultadoBusqueda);
        }
        public JsonResult OficinasRenta_Drop()
        {
            var result = new SelectList(_catOficinasRentaService.ObtenerOficinasActivas(), "IdOficinaRenta", "NombreOficina");
            return Json(result);
        }
        public ActionResult MostrarModal(List<int> idInfracciones)
        {

            var modalModel = new ModalEnvioModel
            {
                SelectedIds = idInfracciones,
                Oficio = "",
                FechaEnvio = DateTime.Now,
                IdLugarEnvio = 0,
            };
            return PartialView("_ModalEnvioInfracciones", modalModel);
        }

        public ActionResult ajax_GuardarInfraccionesEnviadas(ModalEnvioModel model)
        {

			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
			if (_appSettings.AllowWebServices)
            {
                var guardarDatos = _envioInfraccionesService.GuardarEnvioInfracciones(model);
                //return PartialView("Index");


                List<int> successfulInfraccionIds = new List<int>();
                List<int> processedInfraccionIds = new List<int>();

                foreach (int infraccionId in model.SelectedIds)
                {
                    try
                    {
                        var infraccionBusqueda = _infraccionesService.GetInfraccionById(infraccionId, idDependencia);
                        var unicoMotivo = infraccionBusqueda.MotivosInfraccion.FirstOrDefault();
                        int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

                        if (infraccionBusqueda != null)
                        {
                            CrearMultasTransitoRequestModel crearMultasRequestModel = new CrearMultasTransitoRequestModel();
                            crearMultasRequestModel.CR1RFC = infraccionBusqueda.folioInfraccion;
                            //crearMultasRequestModel.CR1APAT = infraccionBusqueda.Persona.apellidoPaterno;
                            //crearMultasRequestModel.CR1AMAT = infraccionBusqueda.Persona.apellidoMaterno;
                            //crearMultasRequestModel.CR1NAME = infraccionBusqueda.Persona.nombre;
                            crearMultasRequestModel.CR2NAME = "";
                            crearMultasRequestModel.CR1RAZON = "";
                            crearMultasRequestModel.CR2RAZON = "";
                            crearMultasRequestModel.CR3RAZON = "";
                            crearMultasRequestModel.CR4RAZON = "";
                            crearMultasRequestModel.BIRTHDT = "";
                            crearMultasRequestModel.CR1CALLE = infraccionBusqueda.lugarCalle;
                            crearMultasRequestModel.CR1NEXT = infraccionBusqueda.lugarNumero;
                            crearMultasRequestModel.CR1NINT = "";
                            crearMultasRequestModel.CR1ENTRE = "";
                            crearMultasRequestModel.CR2ENTRE = "";
                            crearMultasRequestModel.CR1COLONIA = infraccionBusqueda.lugarColonia;
                            crearMultasRequestModel.CR1LOCAL = "";
                            crearMultasRequestModel.CR1MPIO = infraccionBusqueda.municipio;
                            crearMultasRequestModel.CR1CP = "00000";
                            crearMultasRequestModel.CR1TELE = "";
                            crearMultasRequestModel.CR1EDO = "GTO";
                            crearMultasRequestModel.CR1EMAIL = "";
                            crearMultasRequestModel.XSEXF = "";
                            crearMultasRequestModel.XSEXM = "X";
                            crearMultasRequestModel.LZONE = "";
                            crearMultasRequestModel.L_OFN_IOFICINA = "";
                            crearMultasRequestModel.IMPORTE_MULTA = infraccionBusqueda.totalInfraccion.ToString("F2");
                            crearMultasRequestModel.FEC_IMPOSICION = infraccionBusqueda.fechaInfraccion.ToString("yyyy-MM-dd");
                            crearMultasRequestModel.FEC_VENCIMIENTO = infraccionBusqueda.fechaVencimiento.ToString("yyyy-MM-dd");
                            crearMultasRequestModel.INF_PROP = "";
                            crearMultasRequestModel.NOM_INFRACTOR = infraccionBusqueda.PersonaInfraccion.nombreCompleto;
                            crearMultasRequestModel.DOM_INFRACTOR = "";
                            crearMultasRequestModel.NUM_PLACA = infraccionBusqueda.placasVehiculo;
                            crearMultasRequestModel.DOC_GARANTIA = "4";
                            crearMultasRequestModel.NOM_RESP_SOLI = "";
                            crearMultasRequestModel.DOM_RESP_SOLI = "";
                            if (infraccionBusqueda != null)
                            {
                                string prefijo = (idOficina == 1) ? "TTO-PEC" : (idOficina == 2) ? "TTE-M" : "";
                                crearMultasRequestModel.FOLIO_MULTA = prefijo + infraccionBusqueda.folioInfraccion;
                            }
                            crearMultasRequestModel.OBS_GARANT = "";
                            crearMultasRequestModel.ZMOTIVO1 = unicoMotivo.Motivo;
                            crearMultasRequestModel.ZMOTIVO2 = "";
                            crearMultasRequestModel.ZMOTIVO3 = "";
                            var result = _crearMultasTransitoClientService.CrearMultasTransitoCall(crearMultasRequestModel);
                            ViewBag.Pension = result;

                            if (result != null && result.MT_CrearMultasTransito_res.ZTYPE == "S")
                            {
                                _infraccionesService.GuardarReponse(result.MT_CrearMultasTransito_res, infraccionId);
                                successfulInfraccionIds.Add(infraccionId);

                            }
                            else if (result != null && result.MT_CrearMultasTransito_res.ZTYPE == "E")
                            {
                                processedInfraccionIds.Add(infraccionId);
                            }
                            else
                            {
                                processedInfraccionIds.Add(infraccionId);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = "Ha ocurrido un error, intenta más tarde" });
                    }
                }

                int totalInfracciones = model.SelectedIds.Count;
                int successCount = successfulInfraccionIds.Count;

                return Json(new
                {
                    success = true,
                    totalInfracciones = totalInfracciones,
                    successCount = successCount,
                    processedInfraccionIds = successfulInfraccionIds
                });
            }
            else   // USO DE SERVICIOS NO PERMITIDO
            {
                var guardarDatos = _envioInfraccionesService.GuardarEnvioInfracciones(model);
                return PartialView("Index");
            }


        }
    }
}
