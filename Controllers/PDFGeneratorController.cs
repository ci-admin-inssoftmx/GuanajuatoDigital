using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
//using Telerik.SvgIcons;
using GuanajuatoAdminUsuarios.Models.PDFModels;
using GuanajuatoAdminUsuarios.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class PDFGeneratorController : Controller
    {
        private readonly ICapturaAccidentesService _capturaAccidentesService;
        private readonly ICatAutoridadesDisposicionService _catAutoridadesDisposicionservice;
        private readonly ICatAutoridadesEntregaService _catAutoridadesEntregaService;
        private readonly IAppSettingsService _appSettingsService;
        private readonly IBusquedaAccidentesService _busquedaAccidentesService;
        private readonly IInfraccionesService _infraccionesService;
        private readonly IPdfGenerator _pdfService;
        private readonly ICatEntidadesService _catEntidadesService;
        private readonly IOficiales _oficialesService;
        private readonly ICatAgenciasMinisterioService _catAgenciasMinisterioService;
		private readonly IBitacoraService _bitacoraServices;

		public PDFGeneratorController(ICapturaAccidentesService capturaAccidentesService, ICatAutoridadesDisposicionService catAutoridadesDisposicionservice, IAppSettingsService appSettingService, IBusquedaAccidentesService busquedaAccidentesService, IPdfGenerator pdfGenerator, IInfraccionesService infraccionesService, ICatAutoridadesEntregaService catAutoridadesEntregaService, ICatEntidadesService catEntidadesService, IOficiales oficialesService, ICatAgenciasMinisterioService catAgenciasMinisterioService
            , IBitacoraService bitacoraServices)
        {
            _capturaAccidentesService = capturaAccidentesService;
            _catAutoridadesDisposicionservice = catAutoridadesDisposicionservice;
            _appSettingsService = appSettingService;
            _busquedaAccidentesService = busquedaAccidentesService;
            _pdfService = pdfGenerator;
            _infraccionesService = infraccionesService;
            _catAutoridadesEntregaService = catAutoridadesEntregaService;
            _catEntidadesService = catEntidadesService;
            _oficialesService = oficialesService;
            _catAgenciasMinisterioService = catAgenciasMinisterioService;
            _bitacoraServices = bitacoraServices;

		}

        [HttpGet]
        public async Task<FileResult> AccidentesDetallado(int idAccidente)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            // int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var AccidenteSeleccionado = _capturaAccidentesService.ObtenerAccidentePorId(idAccidente, idOficina);
            DatosAccidenteModel datosAccidente = _capturaAccidentesService.ObtenerDatosFinales(idAccidente);
            var ListVehiculosInvolucrados = _capturaAccidentesService.VehiculosInvolucrados(idAccidente);

            var ListClasificaciones = _capturaAccidentesService.ObtenerDatosGrid(idAccidente);
            var ListFactores = _capturaAccidentesService.ObtenerDatosGridFactor(idAccidente);
            var ListCausas = _capturaAccidentesService.ObtenerDatosGridCausa(idAccidente);
            var ListInfracciones = _capturaAccidentesService.InfraccionesDeAccidente(idAccidente);


            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var corporation = corp < 2 ? 1 : corp;


            var ListInvolucrados = _capturaAccidentesService.InvolucradosAccidente(idAccidente);
            foreach (var lst in ListInvolucrados)
            {
                lst.numeroConsecutivo = 0;
                lst.archivoInvolucradoPath = _capturaAccidentesService.GetArchivoInvolucradoPath(idAccidente, lst.IdPersona).value;
                lst.ArchivoInvolucradoBase64 = FileToBase64Parser.TryParse(lst.archivoInvolucradoPath, out string archivoInvolucradoBase64) ? archivoInvolucradoBase64 : null; ;
			}

            foreach (var invo in ListVehiculosInvolucrados)
            {
                foreach (var lst in ListInvolucrados.Where(r => r.IdVehiculo == invo.IdVehiculo))
                {
                    lst.numeroConsecutivo = invo.numeroConsecutivo;
                }

                foreach (var lst in ListInfracciones.Where(r => r.IdVehiculo == invo.IdVehiculo))
                {
                    lst.numeroConsecutivo = invo.numeroConsecutivo;
                }

                foreach (var lst in ListFactores.Where(r => r.IdVehiculo == invo.IdVehiculo))
                {
                    lst.numeroConsecutivo = invo.numeroConsecutivo;
                }

                foreach (var lst in ListCausas.Where(r => r.IdVehiculo == invo.IdVehiculo))
                {
                    lst.numeroConsecutivo = invo.numeroConsecutivo;
                }
            }

            var ParteNombre = _appSettingsService.GetAppSetting("ParteNombre", corporation).SettingValue;
            var PartePuesto = _appSettingsService.GetAppSetting("PartePuesto", corporation).SettingValue;
            List<PDFMotivosInfracciones> motivosInfraccion = ListInfracciones.Select(s => new PDFMotivosInfracciones { idInfraccion = s.IdInfraccion, Motivos = _infraccionesService.GetMotivosInfraccionByIdInfraccion((int)s.IdInfraccion).Select(ss => ss.Motivo + " (" + ss.Fundamento + ")").ToList() }).ToList();


            PDFAccidenteDetalladoModel model = new PDFAccidenteDetalladoModel();
            model.Involucrados = ListInvolucrados;
            model.MotivosInfraccion = motivosInfraccion;
            model.ParteAccidente = AccidenteSeleccionado;
            model.ParteAccidenteComplemento = datosAccidente;
            model.VehiculosInvolucrados = ListVehiculosInvolucrados;

            model.Clasificaciones = ListClasificaciones;
            model.Factores = ListFactores;
            model.CausasDeterminantes = ListCausas;
            model.Infracciones = ListInfracciones;


            model.ParteNombre = ParteNombre;
            model.PartePuesto = PartePuesto;
            model.ADisposicion = _catAutoridadesDisposicionservice.ObtenerAutoridadesActivas(corporation).Where(w => w.IdAutoridadDisposicion == datosAccidente.IdAutoridadDisposicion).Select(s => s.NombreAutoridadDisposicion).FirstOrDefault();
            model.entregadoA = _catAutoridadesEntregaService.ObtenerAutoridadesActivas(corporation).Where(w => w.IdAutoridadEntrega == datosAccidente.IdAutoridadEntrega).Select(s => s.AutoridadEntrega).FirstOrDefault();
            model.sede = _catEntidadesService.ObtenerEntidades(corporation).Where(w => w.idEntidad == datosAccidente.IdEntidadCompetencia).Select(s => s.nombreEntidad).FirstOrDefault();


            model.ElaboraConsignacion = _oficialesService.GetOficialesTodos().Where(w => w.IdOficial == datosAccidente.IdElaboraConsignacion)
                .Select(o => CultureInfo.InvariantCulture.TextInfo.ToTitleCase($"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}")).FirstOrDefault();
            model.NoOficio = datosAccidente.numeroOficio;
            model.AgenciaRecibe = _catAgenciasMinisterioService.ObtenerAgenciasActivas(corporation).Where(w => w.IdAgenciaMinisterio == datosAccidente.IdAgenciaMinisterio).Select(s => s.NombreAgencia).FirstOrDefault();
            model.recibe = datosAccidente.RecibeMinisterio;

            var oficiales = _oficialesService.GetOficialesTodos();
            var oficialElabora = oficiales.FirstOrDefault(w => w.IdOficial == datosAccidente.IdElabora);
			model.Elabora = oficialElabora == null ? null : CultureInfo.InvariantCulture.TextInfo.ToTitleCase($"{oficialElabora.Nombre} {oficialElabora.ApellidoPaterno} {oficialElabora.ApellidoMaterno}");
            model.ElaboraPuesto = oficialElabora?.Puesto;          
            model.ElaboraFirmaBase64 = FileToBase64Parser.TryParse(oficialElabora?.UrlFirma, out string elaboraFirmaB64) ? elaboraFirmaB64 : null;

            var oficialSupervisa = oficiales.FirstOrDefault(w => w.IdOficial == datosAccidente.IdSupervisa);
            model.Supervisor = oficialSupervisa == null ? null : CultureInfo.InvariantCulture.TextInfo.ToTitleCase($"{oficialSupervisa.Nombre} {oficialSupervisa.ApellidoPaterno} {oficialSupervisa.ApellidoMaterno}");
            model.SupervisorPuesto = oficialSupervisa?.Puesto;            
            model.SupervisorFirmaBase64 = FileToBase64Parser.TryParse(oficialSupervisa?.UrlFirma, out string supervisorFirmaB64) ? supervisorFirmaB64 : null;

            var oficialAutoriza = oficiales.FirstOrDefault(w => w.IdOficial == datosAccidente.IdAutoriza);
			model.Autoriza = oficialAutoriza == null ? null : CultureInfo.InvariantCulture.TextInfo.ToTitleCase($"{oficialAutoriza.Nombre} {oficialAutoriza.ApellidoPaterno} {oficialAutoriza.ApellidoMaterno}");
            model.AutorizaPuesto = oficialAutoriza?.Puesto;
            model.AutorizaFirmaBase64 = FileToBase64Parser.TryParse(oficialAutoriza?.UrlFirma, out string autorizaFirmaB64) ? autorizaFirmaB64 : null;

			model.ArchivoCroquisBase64 = FileToBase64Parser.TryParse(model.ParteAccidente.archivoCroquisPath, out string archivoCroquisBase64) ? archivoCroquisBase64 : null;

			model.ArchivoLugarBase64 = string.IsNullOrEmpty(model.ParteAccidente.archivoLugarPath)
	        ? null
	        : Directory.Exists(model.ParteAccidente.archivoLugarPath)
		        ? Directory.GetFiles(model.ParteAccidente.archivoLugarPath, "*.*", SearchOption.TopDirectoryOnly)
				          .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
				          .OrderByDescending(f => new FileInfo(f).LastWriteTime)
				          .Select(f =>
				          {
					          try
					          {
						          return Convert.ToBase64String(System.IO.File.ReadAllBytes(f));
					          }
					          catch
					          {
						          return null;
					          }
				          })
				          .FirstOrDefault()
		        : null;


			string jsonView = await this.RenderViewAsync("_AccidentesDetallado", model, false);
            var bootstrap = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", "bootstrap.min.css");
            bootstrap = System.IO.File.ReadAllText(bootstrap);
            string css = @".s1,.s2,.s4{text-decoration:none}.s1,.s2,.s4,p{color:#000;font-family:Arial,sans-serif;font-style:normal;font-weight:400}*,p{margin:0}h1,p{font-size:9pt}.lateral hr,.middle hr{margin-top:1rem;margin-bottom:1rem}table,table tr td p,td,th,thead,tr{padding:0!important;margin:0!important}table tr td p.s2,table tr td p.s3,table tr td p.s4,table tr td p.s5,table tr td p.s6{text-align:center}*{padding:0;text-indent:0}.s1{font-size:8pt}.s2,.s4{font-size:7pt}p{text-decoration:none}.s3,.s5,h1{font-weight:700;color:#000;font-family:Arial,sans-serif;font-style:normal;text-decoration:none}.s3{font-size:8pt}.s5{font-size:7pt}.s6{color:#000;font-family:Arial,sans-serif;font-style:normal;font-weight:400;text-decoration:none;font-size:2pt}table,tbody{vertical-align:top;overflow:visible;border-color:#000}.middle hr{border:0;width:50%;border-top:1pt solid #000}.lateral hr{border:0;width:60%;border-top:1pt solid #000}.table td{padding:1px!important;vertical-align:top;border-top:0}table,td,th,thead,tr{border-style:solid;border-width:.5pt;border-color:#000}table tr td p{font-size:5.5pt!important}table tr td p.s3{vertical-align:central}";
            bootstrap = bootstrap + css;
            ////(MapPath("~/css/test.css"));

            byte[] pdfBytes = _pdfService.CreatePDFByHTML(jsonView, bootstrap, iTextSharp.text.PageSize.LETTER);
            //BITACORA
			object data = new {idAccidente = idAccidente };
			_bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2048, data);
			//return Json(jsonView);
			return File(pdfBytes, "application/pdf");
            //return PartialView("_AccidentesDetallado", model);
        }

        [HttpPost]
        public ActionResult AccidentesGeneral(BusquedaAccidentesModel model)

        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            if (model.idMunicipio == null) model.idMunicipio = 0;
            if (model.IdOficialBusqueda == null) model.IdOficialBusqueda = 0;
            if (model.IdCarreteraBusqueda == null) model.IdCarreteraBusqueda = 0;
            if (model.IdTramoBusqueda == null) model.IdTramoBusqueda = 0;
            if (model.IdOficialBusqueda == null) model.IdOficialBusqueda = 0;
            
            if (model.folioBusqueda == null) model.folioBusqueda = string.Empty;
            if (model.placasBusqueda == null) model.placasBusqueda = string.Empty;
            if (model.propietarioBusqueda == null) model.propietarioBusqueda = string.Empty;
            if (model.serieBusqueda == null) model.serieBusqueda = string.Empty;
            if (model.conductorBusqueda == null) model.conductorBusqueda = string.Empty;


            //var modelList = _busquedaAccidentesService.GetAllAccidentes()
            //                                    .Where(w => w.idMunicipio == (model.idMunicipio > 0 ? model.idMunicipio : w.idMunicipio)
            //                                        && w.idSupervisa == (model.IdOficialBusqueda > 0 ? model.IdOficialBusqueda : w.idSupervisa)
            //                                        && w.idCarretera == (model.IdCarreteraBusqueda > 0 ? model.IdCarreteraBusqueda : w.idCarretera)
            //                                        && w.idTramo == (model.IdTramoBusqueda > 0 ? model.IdTramoBusqueda : w.idTramo)
            //                                        && w.idElabora == (model.IdOficialBusqueda > 0 ? model.IdOficialBusqueda : w.idElabora)
            //                                        && w.idAutoriza == (model.IdOficialBusqueda > 0 ? model.IdOficialBusqueda : w.idAutoriza)
            //                                        && w.idEstatusReporte == (model.IdEstatusAccidente > 0 ? model.IdEstatusAccidente : w.idEstatusReporte)
            //                                        && (string.IsNullOrEmpty(model.folioBusqueda) || w.numeroReporte.Contains(model.folioBusqueda))
            //                                        && (string.IsNullOrEmpty(model.placasBusqueda.ToUpper()) || w.placa.Contains(model.placasBusqueda.ToUpper()))
            //                                        && (string.IsNullOrEmpty(model.propietarioBusqueda) || w.propietario.Contains(model.propietarioBusqueda))
            //                                        && (string.IsNullOrEmpty(model.serieBusqueda) || w.serie.Contains(model.serieBusqueda))
            //                                        && (string.IsNullOrEmpty(model.conductorBusqueda) || w.conductor.Contains(model.conductorBusqueda))
            //                                        && ((!model.FechaInicio.HasValue && !model.FechaFin.HasValue) || (w.fecha >= model.FechaInicio && w.fecha <= model.FechaFin))
            //                                        ).ToList();

            Pagination pagination = new Pagination();
            pagination.PageIndex = 0;
            pagination.PageSize = 10000000;
            var modelList = _busquedaAccidentesService.GetAllAccidentesPagination(pagination, model, idOficina).ToList();
            Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
            {
            {"NumeroSecuencial","Número"},
            {"numeroReporte","Folio"},
            {"fecha","Fecha"},
            {"hora","Hora"},
            {"municipio","Municipio" },
            {"carretera","Carretera" },
            {"tramo","Tramo" },
            {"estatusReporte", "Estatus" }
            };

            //No se encontraron datos
            if (modelList.Count == 0)
                return Json(new { success = false, message = "No se encontraron registros con los parámetros de búsqueda" });

            var result = _pdfService.CreatePdf("ReporteAccidentesGeneral", "Reporte General de Accidentes", ColumnsNames, modelList, Array.Empty<float>());
            //return File(result.Item1, "application/pdf", result.Item2);
            byte[] bytes = result.Item1.ToArray();
            string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);
			//BITACORA
			_bitacoraServices.BitacoraAccidentes(0,CodigosAccidente.C2046,"");
			return Json(new { success = true, data = base64 });
        }

        [HttpPost]
        public ActionResult InfraccionesGeneral(InfraccionesBusquedaModel model)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            //Se valida si ambas fecha vienen nulas se limita la consulta a un rango de 45 dias
            /*if(model.FechaInicio.Year<=1900 && model.FechaFin.Year<=1900){
				model.FechaInicio=DateTime.Now.AddDays(-45);
				model.FechaFin=DateTime.Now;
			}*/
            var modelList = _infraccionesService.GetReporteInfracciones(model, idOficina, idDependencia);
            var pdfModel = modelList.Select(s => new InfraccionesGeneralPDFModel
            {
                folioInfraccion = s.folioInfraccion,
                conductor = s.PersonaInfraccion.nombreCompleto,
                propietario = s.Vehiculo.Persona.nombreCompleto,
                fechaAplicacion = s.fechaInfraccion,
                garantia = s.Garantia.garantia,
                vehiculo = s.Vehiculo.fullVehiculo,
                placas = s.Vehiculo.placas,
                delegacion = s.delegacion,
                estatusInfraccion = s.estatusInfraccion,
                aplicacion = s.aplicacion
            }).ToList();
            Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
            {
                //{"idInfraccion",""},
                {"folioInfraccion","Folio"},
                {"conductor","Conductor"},
                {"propietario","Propietario"},
                {"fechaAplicacion","Fecha Aplicación"},
                {"garantia","Garantía"},
                {"vehiculo","Vehículo"},
                {"placas","Placas"},
                {"delegacion","Delegación"},
                { "estatusInfraccion", "Estatus"},
                { "aplicacion","Aplicada a"}
            };

            //Si no se encontraron datos
            if (pdfModel.Count == 0)
                return Json(new { success = false, message = "No se encontraron registros con los parámetros de búsqueda" });

            float[] columnWidth = { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f };
            var result = _pdfService.CreatePdf("ReporteInfraccionesGeneral", "Reporte General de Infracciones", ColumnsNames, pdfModel, columnWidth);
            byte[] bytes = result.Item1.ToArray();
            string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);
            return Json(new { success = true, data = base64 }); ;
        }

    }
}
