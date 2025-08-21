using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.Extensions;
using System;
using GuanajuatoAdminUsuarios.Models;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.RESTModels;
using Microsoft.Extensions.Options;
using System.Linq;
using GuanajuatoAdminUsuarios.Framework.Catalogs;
using GuanajuatoAdminUsuarios.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class InfraccionesCortesiaController : BaseController
    {
        private readonly IInfraccionesCortesiaService _infraccionesCortesiaServices;
        private readonly IInfraccionesService _infraccionesService;
        private readonly ICrearMultasTransitoClientService _crearMultasTransitoClientService;
        private readonly IBitacoraService _bitacoraServices;
        private readonly AppSettings _appSettings;

        public InfraccionesCortesiaController(IInfraccionesCortesiaService infraccionesCortesiaServices, IOptions<AppSettings> appSettings, IInfraccionesService infraccionesService,
            ICrearMultasTransitoClientService crearMultasTransitoClientService, IBitacoraService bitacoraServices

           )
        {
            _infraccionesCortesiaServices = infraccionesCortesiaServices;
            _infraccionesService = infraccionesService;
            _crearMultasTransitoClientService = crearMultasTransitoClientService;
            _bitacoraServices = bitacoraServices;
            _appSettings = appSettings.Value;

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ObtieneInfracciones([DataSourceRequest] DataSourceRequest request)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int corp = (int)HttpContext.Session.GetInt32("IdDependencia");

            var ListaInfracciones = _infraccionesCortesiaServices.ObtenerTodasInfraccionesCortesia(idOficina, corp);
            return Json(ListaInfracciones.ToDataSourceResult(request));
        }

        public ActionResult ModalModificar(int IdInfraccion, string fechaInfraccion, string fecha,string folioInfraccion)
        {
            DateTime fechaVencimiento;
            DateTime fechaInfraccion1;

            bool isDateValid = DateTime.TryParse(fecha, out fechaVencimiento);

            if (!isDateValid)
            {
                fechaVencimiento = DateTime.MinValue;
            }

            bool isDateValid1 = DateTime.TryParse(fechaInfraccion, out fechaInfraccion1);

            if (!isDateValid1)
            {
                fechaInfraccion1 = DateTime.MinValue;
            }
            var model = new InfraccionesCortesiaModel
            {
                IdInfraccion = IdInfraccion,
                FechaVencimiento = fechaVencimiento,
                FechaInfraccionDt = fechaInfraccion1,
                folioInfraccion = folioInfraccion

            };
            return PartialView("_ModalModificar", model);


        }

		public ActionResult EditarFechaVencimiento(InfraccionesCortesiaModel model)
		{
			if (model.FechaInfraccionDt > model.NuevaFechaVencimiento)
			{
				return Json(new { success = false, message = "La nueva fecha de vencimiento no puede ser menor a la fecha de la infracción" });
			}

			int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

			if (EsFechaInvalida(model.NuevaFechaVencimiento, idOficina))
			{
				return Json(new { success = false, message = "La fecha de vencimiento no puede ser un sábado, domingo o día festivo." });
			}

			int corp = (int)HttpContext.Session.GetInt32("IdDependencia");
			var ip = HttpContext.Connection.RemoteIpAddress.ToString();
			var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);

			int modificar = _infraccionesCortesiaServices.GuardarNuevaFecha(model);
			_bitacoraServices.insertBitacora((decimal)model.IdInfraccion, ip, string.Format("Se modificó la fecha de vigencia de cortesía de la infracción con folio: {0}.", model.folioInfraccion), "Cambio fecha cortesía", "Update", user);

			var ListaInfracciones = _infraccionesCortesiaServices.ObtenerTodasInfraccionesCortesia(idOficina, corp);

			return Json(new { success = true, data = ListaInfracciones });
		}

		private bool EsFechaInvalida(DateTime fecha, int idOficina)
		{
			if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
			{
				return true;
			}

			// Consultar si la fecha está en el listado de días festivos
			if (_infraccionesService.GetDiaFestivo(idOficina, fecha) != 0)
			{
				return true;
			}

			return false;
		}


		public IActionResult EnviarFinanzas(int idInfraccion)
        {
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            _bitacoraServices.BitacoraWS("Inicia ws CrearMultasTransito", CodigosWs.C4009, new
            {
                idInfraccion = idInfraccion
            });

            if (_appSettings.AllowWebServices)
            {
                try
                {
                    var infraccionBusqueda = _infraccionesService.GetInfraccionById(idInfraccion, idDependencia);
                    if (infraccionBusqueda == null)
                    {
                        return Json(new { success = false, message = "Error en la busuqeda de infracción", id = idInfraccion });
                    }

                    var unicoMotivo = infraccionBusqueda.MotivosInfraccion?.FirstOrDefault();
                    int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                                   
                        CrearMultasTransitoRequestModel crearMultasRequestModel = new CrearMultasTransitoRequestModel();

                        PersonaModel Persona = infraccionBusqueda.PersonaInfraccion2;
                        var validrfc = 13;

                        if (infraccionBusqueda.idAplicacion == 1)
                        {

                            Persona = infraccionBusqueda.PersonaInfraccion2;
                        }
                        if (infraccionBusqueda.idAplicacion == 2 || infraccionBusqueda.idAplicacion == 4)
                        {
                            Persona = infraccionBusqueda.Persona;
                        }
                        if (infraccionBusqueda.idAplicacion == 3)
                        {
                            if (infraccionBusqueda.PersonaInfraccion2?.nombre.ToLower() != "se ignora")
                            {
                                Persona = infraccionBusqueda.PersonaInfraccion2;
                            }
                            else
                            {
                                Persona = infraccionBusqueda.Persona;
                            }
                        }
                        if (Persona?.tipoPersona == "1")
                        {
                            validrfc = 13;
                        }
                        else
                        {
                            validrfc = 12;
                        }
                        string prefijo = (idDependencia == 1) ? "TTO-" : (idDependencia == 0) ? "TTE-" : "";

                        crearMultasRequestModel.CR1RFC = (Persona?.RFC ?? "").Length == validrfc ? Persona?.RFC : (prefijo + infraccionBusqueda.folioInfraccion.ToUpper());


                        if ((Persona?.tipoPersona ?? "Persona física") == "Persona física")
                        {
                            crearMultasRequestModel.CR1APAT = (Persona?.apellidoPaterno ?? "").Cut(40);
                            crearMultasRequestModel.CR1AMAT = (Persona?.apellidoMaterno ?? "").Cut(40);
                            crearMultasRequestModel.CR1NAME = (Persona?.nombre ?? "").Cut(40);
                        }
                        else
                        {
                            crearMultasRequestModel.CR1RAZON = Persona?.nombre ?? "";
                        }



                        crearMultasRequestModel.BIRTHDT = Persona?.fechaNacimiento?.ToString("yyyy-MM-dd") ?? "1900-01-01";


                        crearMultasRequestModel.CR1CALLE = (Persona?.PersonaDireccion?.calle ?? "" ?? "").Cut(60);
                        crearMultasRequestModel.CR1NEXT = (Persona?.PersonaDireccion?.numero ?? "" ?? "").Cut(10);
                        crearMultasRequestModel.CR1NINT = "";
                        crearMultasRequestModel.CR1ENTRE = "";
                        crearMultasRequestModel.CR2ENTRE = "";
                        crearMultasRequestModel.CR1COLONIA = (Persona?.PersonaDireccion?.colonia ?? "" ?? "").Cut(40);
                        crearMultasRequestModel.CR1LOCAL = "";
                        crearMultasRequestModel.CR1MPIO = (Persona?.PersonaDireccion?.municipio ?? "" ?? "").Cut(40);
                        crearMultasRequestModel.CR1CP = (Persona?.PersonaDireccion?.codigoPostal ?? "" ?? "").Cut(10);
                        crearMultasRequestModel.CR1TELE = (Persona?.PersonaDireccion?.telefono ?? "" ?? "").Cut(30);
                        crearMultasRequestModel.CR1EDO = ("GTO").Cut(3);
                        crearMultasRequestModel.CR1EMAIL = (Persona?.PersonaDireccion?.correo ?? "" ?? "").Cut(241);

                        if ((Persona?.genero ?? "0") == "MASCULINO")
                        {
                            crearMultasRequestModel.XSEXM = "1";
                        }
                        else
                        {
                            crearMultasRequestModel.XSEXF = "1";
                        }

                        var placaStr = infraccionBusqueda.placasVehiculo.Replace("-", "");
                        var nombreinfra = (Persona?.nombreCompleto ?? "").Length < 61 ? (Persona?.nombreCompleto ?? "") : (Persona?.nombreCompleto ?? "").Substring(0, 60);
                        var nombreResp = (infraccionBusqueda.Persona?.nombreCompleto ?? "").Length < 61 ? (infraccionBusqueda.Persona?.nombreCompleto ?? "") : (infraccionBusqueda.Persona?.nombreCompleto ?? "").Substring(0, 60);


                        crearMultasRequestModel.IMPORTE_MULTA = infraccionBusqueda.totalInfraccion.ToString("F2");
                        crearMultasRequestModel.FEC_IMPOSICION = infraccionBusqueda.fechaInfraccion.ToString("yyyy-MM-dd");
                        crearMultasRequestModel.FEC_VENCIMIENTO = infraccionBusqueda.fechaVencimiento.ToString("yyyy-MM-dd");
                        crearMultasRequestModel.NOM_INFRACTOR = nombreinfra;
                        crearMultasRequestModel.DOM_INFRACTOR = (Persona?.PersonaDireccion.calle ?? "" + " " + Persona?.PersonaDireccion.numero ?? "" + ", " + Persona?.PersonaDireccion.colonia ?? "").Cut(60);
                        crearMultasRequestModel.NUM_PLACA = (placaStr).Cut(7);
                        crearMultasRequestModel.DOC_GARANTIA = infraccionBusqueda.idGarantia.ToString();
                        crearMultasRequestModel.NOM_RESP_SOLI = nombreResp;
                        crearMultasRequestModel.DOM_RESP_SOLI = ((infraccionBusqueda.Persona?.PersonaDireccion.calle ?? "") + " " + (infraccionBusqueda.Persona?.PersonaDireccion.numero ?? "") + ", " + (infraccionBusqueda.Persona?.PersonaDireccion.colonia ?? "")).Cut(150);
                        crearMultasRequestModel.FOLIO_MULTA = (prefijo + infraccionBusqueda.folioInfraccion.ToUpper()).Cut(20);
                        crearMultasRequestModel.OBS_GARANT = (infraccionBusqueda.NombreGarantia + " " + (infraccionBusqueda.idGarantia == 1 ? infraccionBusqueda.Garantia.numPlaca : infraccionBusqueda.idGarantia == 2 ? infraccionBusqueda.Garantia.numLicencia : infraccionBusqueda.idGarantia == 3 ? "-" : infraccionBusqueda.Vehiculo.placas)).Cut(100);


                        int count = 1;
                        foreach (var enumeration in infraccionBusqueda.MotivosInfraccion)
                        {
                            if (count == 1) { crearMultasRequestModel.ZMOTIVO1 = enumeration.Motivo; }
                            if (count == 2) { crearMultasRequestModel.ZMOTIVO2 = enumeration.Motivo; }
                            if (count == 3) { crearMultasRequestModel.ZMOTIVO3 = enumeration.Motivo; }
                            count++;
                        }

                        _bitacoraServices.BitacoraWS("Envia Modelo wsCrearMultasTransito", CodigosWs.C4010, new { idInfraccion = idInfraccion, modelo = crearMultasRequestModel });

                        var result = _crearMultasTransitoClientService.CrearMultasTransitoCall(crearMultasRequestModel);

                        ViewBag.Pension = result;
                        var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                        var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                        if (result != null && result.MT_CrearMultasTransito_res != null && "S".Equals(result.MT_CrearMultasTransito_res.ZTYPE, StringComparison.OrdinalIgnoreCase))
                        {
                            _infraccionesService.ModificarEstatusInfraccion(idInfraccion, (int)CatEnumerator.catEstatusInfraccion.Enviada);
                            _infraccionesService.GuardarReponse(result.MT_CrearMultasTransito_res, idInfraccion);
                            _infraccionesCortesiaServices.ModificarEstatusCortesia(idInfraccion);
                            _bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("Se registró en finanzas la infracción folio: {0} se obtuvo No. de documento = {1}; a través de WS", infraccionBusqueda.folioInfraccion, result.MT_CrearMultasTransito_res.DOCUMENTNUMBER), "Registro Finanzas", "WS", user);
                            _bitacoraServices.BitacoraWS("Finaliza ws CrearMultasTransito", CodigosWs.C4010, new { idInfraccion = idInfraccion, result = result, success = true });
                            return Json(new { success = true });
                        }
                        /*else if (result != null && result.MT_CrearMultasTransito_res != null && "E".Equals(result.MT_CrearMultasTransito_res.ZTYPE, StringComparison.OrdinalIgnoreCase))
                        {
                         //_bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("Se registró en RIAG la infracción folio: {0} no se pudo enviar  finanzas", infraccionBusqueda.folioInfraccion), "Registro Finanzas", "WS", user);
                         _bitacoraServices.BitacoraWS("Finaliza ws CrearMultasTransito", CodigosWs.C4010, new { idInfraccion = idInfraccion, message = "Registro actualizado en RIAG" });
                         return Json(new { success = false, message = "Registro actualizado en RIAG", id = idInfraccion });


                     }
                     else if (result != null && result.MT_CrearMultasTransito_res != null && "A".Equals(result.MT_CrearMultasTransito_res.ZTYPE, StringComparison.OrdinalIgnoreCase))
                     {

                         if (string.IsNullOrEmpty(infraccionBusqueda.documento) && infraccionBusqueda.idEstatusInfraccion == 2)
                         {
                             var documento = result.MT_CrearMultasTransito_res.ZMESSAGE.Split("el");
                             var document = documento[1].Substring(9, 12);
                             _infraccionesService.GuardarReponse2(result.MT_CrearMultasTransito_res, idInfraccion);
                             _bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("Se registró en finanzas la infracción folio: {0} se obtuvo No. de documento = {1}; a través de WS por la opcion A", infraccionBusqueda.folioInfraccion, document), "Registro Finanzas", "WS", user);

                             return Json(new { success = true, message = "Infraccion anteriormente registrada en finanzas", id = idInfraccion });

                         }
                         _bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("La infraccion con folio {0} ya existe en Finanzas y tiene {0}.", infraccionBusqueda.folioInfraccion, infraccionBusqueda.documento), "Registro Finanzas", "WS", user);
                         _bitacoraServices.BitacoraWS("Finaliza ws CrearMultasTransito", CodigosWs.C4010, new { idInfraccion = idInfraccion, message = "Infraccion anteriormente registrada en finanzas" });
                         return Json(new { success = false, message = "Infraccion anteriormente registrada en finanzas", id = idInfraccion });
                     }*/
                    else
                    {
                            _bitacoraServices.BitacoraWS("Finaliza ws CrearMultasTransito", CodigosWs.C4010, new { idInfraccion = idInfraccion, message = "Infraccion no enviada" });
                            return Json(new { success = false, message = "Infraccion no enviada" });
                        }
                    
                }
                catch (SqlException ex)
                {
                    return Json(new { success = false, message = "Ha ocurrido un error intenta más tarde" });
                }
            }
            _bitacoraServices.BitacoraWS("Finaliza ws CrearMultasTransito", CodigosWs.C4010, new { idInfraccion = idInfraccion, message = "Infraccion no enviada a finanzas" });
            return Json(new { success = false, message = "Infracción no enviada a finanzas", id = idInfraccion });


        }
    }
}

    
