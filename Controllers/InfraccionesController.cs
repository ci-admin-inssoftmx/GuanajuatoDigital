using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using GuanajuatoAdminUsuarios.Framework.Catalogs;
using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Interfaces.Catalogos;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Models.CatCampos;
using GuanajuatoAdminUsuarios.RESTModels;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Services.CustomReportsService;
using GuanajuatoAdminUsuarios.Util;
using iTextSharp.text.pdf.parser;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GuanajuatoAdminUsuarios.RESTModels.CotejarDatosResponseModel;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;
using static GuanajuatoAdminUsuarios.Helpers.MethodsExtensions;
using GuanajuatoAdminUsuarios.ExternalServices.Interfaces;
using Path = System.IO.Path;
using GuanajuatoAdminUsuarios.Models.Catalogos.Turnos;

using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Bibliography;
using static GuanajuatoAdminUsuarios.Services.InfraccionesService;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Org.BouncyCastle.Crypto;
using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Services.Blocs;
using GuanajuatoAdminUsuarios.Interfaces.Blocs;
using System.ServiceModel.Channels;
//using Telerik.SvgIcons;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class InfraccionesController : BaseController
    {
        private readonly IEstatusInfraccionService _estatusInfraccionService;
        private readonly ITipoCortesiaService _tipoCortesiaService;
        private readonly IDependencias _dependeciaService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;
        private readonly IGarantiasService _garantiasService;
        private readonly IInfraccionesService _infraccionesService;
        private readonly ICatDictionary _catDictionary;
        private readonly IVehiculosService _vehiculosService;
        private readonly IPersonasService _personasService;
        private readonly ICapturaAccidentesService _capturaAccidentesService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICrearMultasTransitoClientService _crearMultasTransitoClientService;
        private readonly ICotejarDocumentosClientService _cotejarDocumentosClientService;
        private readonly ICatMunicipiosService _catMunicipiosService;
        private readonly ICatEntidadesService _catEntidadesService;
        private readonly IColores _coloresService;
        private readonly ICatMarcasVehiculosService _catMarcasVehiculosService;
        private readonly ICatSubmarcasVehiculosService _catSubmarcasVehiculosService;
        private readonly IRepuveService _repuveService;
        private readonly ICatCarreterasService _catCarreterasService;
        private readonly IBitacoraService _bitacoraServices;
        private readonly IVehiculoPlataformaService _vehiculoPlataformaService;
        private readonly ICustomCatalogService _CustomCatalog;
        private readonly ICatCamposObligService _catCamposObligService;
        private readonly ICadService _cadService;
        private readonly ICatTurnosService _turnosService;
        private readonly IAppSettingsService _appSettingsService;
        private readonly IBlockPermisoInfraccion _canBlock;
        private readonly IBlocsService _Block;

        private readonly string _rutaArchivo;

        private readonly AppSettings _appSettings;
        private string resultValue = string.Empty;
        public static bool findValue { get; set; } = true;
        public static InfraccionesBusquedaModel infraModel;
        public int _idInfraccion = 0;

        bool canPermiso;
        string prefijo;


        public InfraccionesController(
            IHttpClientFactory httpClientFactory,
            IEstatusInfraccionService estatusInfraccionService,
            ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService,
            ITipoCortesiaService tipoCortesiaService,
            IDependencias dependeciaService,
            IGarantiasService garantiasService,
            IInfraccionesService infraccionesService,
            ICatDictionary catDictionary,
            IVehiculosService vehiculosService,
            IPersonasService personasService,
            ICrearMultasTransitoClientService crearMultasTransitoClientService,
            ICapturaAccidentesService capturaAccidentesService,
            ICotejarDocumentosClientService cotejarDocumentosClientService,
            ICatMunicipiosService catMunicipiosService,
            ICatEntidadesService catEntidadesService,
            IColores coloresService,
            ICatMarcasVehiculosService catMarcasVehiculosService,
            ICatSubmarcasVehiculosService catSubmarcasVehiculosService,
            IRepuveService repuveService,
            ICatCarreterasService catCarreterasService,
            IBitacoraService bitacoraService,
            IVehiculoPlataformaService vehiculoPlataformaService,
            ICustomCatalogService customCatalog,
            ICatCamposObligService catCamposObligService,
            ICadService cadService,
            ICatTurnosService turnosService,
            IConfiguration configuration,
            IOptions<AppSettings> appSettings,
            IAppSettingsService appSettingsService,
            IBlockPermisoInfraccion canBlock,
            IBlocsService blocs)
        {
            _httpClientFactory = httpClientFactory;

            _catDictionary = catDictionary;
            _estatusInfraccionService = estatusInfraccionService;
            _tipoCortesiaService = tipoCortesiaService;
            _dependeciaService = dependeciaService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
            _garantiasService = garantiasService;
            _infraccionesService = infraccionesService;
            _vehiculosService = vehiculosService;
            _personasService = personasService;
            _capturaAccidentesService = capturaAccidentesService;
            _cotejarDocumentosClientService = cotejarDocumentosClientService;
            // Configurar el cliente HTTP con la URL base del servicio
            _catCarreterasService = catCarreterasService;
            _crearMultasTransitoClientService = crearMultasTransitoClientService;
            _catMunicipiosService = catMunicipiosService;
            _catEntidadesService = catEntidadesService;
            _coloresService = coloresService;
            _catMarcasVehiculosService = catMarcasVehiculosService;
            _catSubmarcasVehiculosService = catSubmarcasVehiculosService;
            _repuveService = repuveService;
            _bitacoraServices = bitacoraService;
            _vehiculoPlataformaService = vehiculoPlataformaService;
            _CustomCatalog = customCatalog;
            _catCamposObligService = catCamposObligService;
            _cadService = cadService;
            _turnosService = turnosService;
            _appSettings = appSettings.Value;
            _appSettingsService = appSettingsService;
            _rutaArchivo = configuration.GetValue<string>("AppSettings:RutaArchivoInventarioInfracciones");
            _canBlock = canBlock;
            _Block = blocs;
            (canPermiso, prefijo) = _canBlock.getdate();
        }

        public IActionResult Index()
        {

            infraModel = new InfraccionesBusquedaModel();
            int idOficina = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value);
            var x = User.FindFirst(CustomClaims.IdUsuario).Value;
            infraModel.IdDelegacion = idOficina;
            InfraccionesBusquedaModel searchModel = new InfraccionesBusquedaModel();
            List<InfraccionesModel> listInfracciones = new List<InfraccionesModel>();
            //_infraccionesService.GetAllInfracciones(idOficina);
            var catTipoServicio = _catDictionary.GetCatalog("CatTipoServicio", "0");
            var catTiposVehiculo = _catDictionary.GetCatalog("CatTiposVehiculo", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipiosByEntidad", "11");
            var catAplicada = _catDictionary.GetCatalog("CatAplicadoA", "0");

            ViewBag.CatTipoServicio = new SelectList(catTipoServicio.CatalogList, "Id", "Text");
            ViewBag.CatTiposVehiculo = new SelectList(catTiposVehiculo.CatalogList, "Id", "Text");
            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            ViewBag.CatAplicada = new SelectList(catAplicada.CatalogList, "Id", "Text");

            searchModel.ListInfracciones = listInfracciones;
            searchModel.IdDelegacion = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value);
            HttpContext.Session.Remove("IdMarcaVehiculo");
            return View(searchModel);
        }

        public IActionResult InfraccionesInvisibles()
        {
            infraModel = new InfraccionesBusquedaModel();
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var x = User.FindFirst(CustomClaims.IdUsuario).Value;
            infraModel.IdDelegacion = idOficina;
            infraModel.IdEstatus = _estatusInfraccionService.GetEstatusId("Invisible");
            InfraccionesBusquedaModel searchModel = new InfraccionesBusquedaModel();

            HttpContext.Session.Remove("IdMarcaVehiculo");
            return View(searchModel);
        }

        [HttpGet]
        public FileResult CreatePdfUnRegistro(int IdInfraccion)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            Dictionary<string, string> ColumnsNames = new Dictionary<string, string>()
            {
            {"folioInfraccion","Folio"},
            {"NombreConductor","Conductor"},
            {"NombrePropietario","Propietario"},
            {"fechaInfraccion","Fecha Aplicada a"},
            {"NombreGarantia","Garantía"},
            {"delegacion","Delegación/Oficina"}
            };
            var InfraccionModel = _infraccionesService.GetInfraccionReportById(IdInfraccion, idDependencia);
            var uma = _infraccionesService.getUMAValue(InfraccionModel.fechaInfraccion);
            InfraccionModel.Uma = uma;
            var report = new InfraccionReportService("Infracción", "INFRACCIÓN").CreatePdf(InfraccionModel);
            return File(report.File.ToArray(), "application/pdf", report.FileName);
        }

        [HttpGet]
        public ActionResult Detail(int IdInfraccion)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Update(int IdInfraccion)
        {
            return View();
        }


        #region DropDown sources

        public JsonResult Estatus_Read()
        {
            var result = new SelectList(_estatusInfraccionService.GetEstatusInfracciones(), "idEstatusInfraccion", "estatusInfraccion");
            return Json(result);
        }

        public JsonResult Estatus_Invisible()
        {
            var result = new SelectList(_estatusInfraccionService.GetEstatusInvisibleInfracciones(), "idEstatusInfraccion", "estatusInfraccion");
            return Json(result);
        }

        public JsonResult Municipios_Read()
        {
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");
            var result = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            //var selected = result.Where(x => x.Value == Convert.ToString(idSubmarca)).First();
            //selected.Selected = true;
            return Json(result);
        }

        public JsonResult MunicipiosActive_Read()
        {
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");
            var result = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            //var selected = result.Where(x => x.Value == Convert.ToString(idSubmarca)).First();
            //selected.Selected = true;
            return Json(result);
        }

        public JsonResult Municipios_Por_Delegacion_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var tt = _catMunicipiosService.GetMunicipiosPorDelegacion2(idOficina);

            tt.Add(new CatMunicipiosModel() { IdMunicipio = 1, Municipio = "No aplica".ToUpper() });
            tt.Add(new CatMunicipiosModel() { IdMunicipio = 2, Municipio = "No especificado".ToUpper() });

            var aux = tt.Select(t => new { IdMunicipio = t.IdMunicipio, Municipio = t.Municipio.ToUpper() });

            var result = new SelectList(tt, "IdMunicipio", "Municipio");
            return Json(result);
        }

        public JsonResult Municipios_Por_Entidad(int entidadDDlValue)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catMunicipiosService.GetMunicipiosPorEntidad(entidadDDlValue, corp), "IdMunicipio", "Municipio");
            return Json(result);
        }

        public JsonResult CarreterasPorDelegacion()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_catCarreterasService.GetCarreterasPorDelegacion(idOficina), "idCarretera", "carretera");
            return Json(result);
        }

        public async Task<JsonResult> GetTurnosDropDownList(int municipioId)
		{
            List<TurnoDetailsModel> turnos = (await _turnosService.GetAllByMunicipioAsync(municipioId))
                    .Select(TurnoDetailsModel.FromEntity)
                    .ToList();

			var dropDownList = turnos.Select(d => new
			{
				Value = d.IdTurno,
				Text = d.Nombre,
			}).ToList();

			return Json(dropDownList);
		}

        #endregion DropDown sources

        public IActionResult ajax_OmitirConductor()
        {
            var personamodel = new PersonaModel();
            personamodel.nombre = "Se ignora";
            personamodel.idCatTipoPersona = 1;
            personamodel.PersonaDireccion = new PersonaDireccionModel();

            var result = _personasService.CreatePersona(personamodel);

            return Json(result);
        }

        public IActionResult ajax_OmitirConductor2(int idInfraccion)
        {
            var personamodel = new PersonaModel();
            personamodel.nombre = "Se ignora";
            personamodel.idCatTipoPersona = 1;
            personamodel.PersonaDireccion = new PersonaDireccionModel();

            var result = _personasService.CreatePersona(personamodel);
            _infraccionesService.CrearPersonaInfraccion(idInfraccion, result);

            var persona = _personasService.GetPersonaByIdInfraccion(result, idInfraccion);
            //_infraccionesService.ActualizConductor(idInfraccion, result);

            ViewBag.EsSoloLectura = false;

            var q = PartialView("_EditarDetallePersona", persona);

            return q;
        }


        public IActionResult AgregarPersonasConductor(int idInfraccion)
        {
            HttpContext.Session.SetInt32("IdInfra", idInfraccion);
            return PartialView("_AgregarEditarConductor");
        }

        public IActionResult CambiarVehiculo()
        {
            return PartialView("_CambiarVehiculo");
        }


        public IActionResult ajax_PropietarioConductor2(int idInfraccion, int idConductor)
        {

            var persona = _personasService.GetPersonaByIdInfraccion(idConductor, idInfraccion);

            if (persona.idCatTipoPersona == 2)
            {
                return Json(new { Error = 1 });
            }

            _infraccionesService.ActualizConductor(idInfraccion, idConductor);

            ViewBag.EsSoloLectura = false;

            HttpContext.Session.SetInt32("IdInfraConductor", idConductor);
            persona = _personasService.GetPersonaByIdInfraccion(idConductor, idInfraccion);
            var q = PartialView("_EditarDetallePersona", persona);

            return q;
        }


        public JsonResult Cortesias_Read()
        {
            //catTipoCortesia
            var result = new SelectList(_tipoCortesiaService.GetTipoCortesias(), "idTipoCortesia", "tipoCortesia");
            return Json(result);
        }

        public JsonResult Dependencias_Read()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_dependeciaService.GetDependencias(corp), "IdDependencia", "NombreDependencia");
            return Json(result);
        }

        public JsonResult Garantias_Read()
        {
            //catGarantias
            var result = new SelectList(_garantiasService.GetGarantias(), "idGarantia", "garantia");
            return Json(result);
        }
    
        public JsonResult Delegaciones_Read()
        {

            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var result2 = new List<CatDelegacionesOficinasTransporteModel>();


            if (corp < 3)
            {
                result2 = _catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos();
                result2 = result2.Where(s => !s.Delegacion.Contains("Centro de Gobierno")).ToList();
            }

            else
                result2 = _catDelegacionesOficinasTransporteService.GetDelegacionesOficinasFiltrado(corp);

            // else
            //     result2 = _catDelegacionesOficinasTransporteService.GetDelegacionesOficinasFiltrado(corp);



            var result = new SelectList(result2, "IdDelegacion", "Delegacion");



            return Json(result);
        }

        public async Task<ActionResult> Crear()
        {
            HttpContext.Session.SetInt32("modoMostrar", 0);
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            ViewBag.CatCarreteras = new SelectList(_catCarreterasService.GetCarreterasPorDelegacion2(idOficina), "IdCarretera", "Carretera");
            ViewBag.EditarVehiculo = false;
            ViewBag.Regreso = 1;
            ViewBag.canBlock = canPermiso;

            var q = View(new InfraccionesModel());
            return q;
        }


        public ActionResult GetAllVehiculosPagination([DataSourceRequest] DataSourceRequest request)

        {

            filterValue(request.Filters);

            Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            pagination.PageSize = (request.PageSize != 0) ? request.PageSize : 10;
            pagination.Filter = resultValue;
            var t = request.Sorts;
            if (t.Count() > 0)
            {
                var q = t[0];
                pagination.Sort = q.SortDirection.ToString().Substring(0, 3);
                pagination.SortCamp = q.Member;
            }

            var vehiculosList = _vehiculosService.GetAllVehiculosPagination(pagination);
            var total = 0;
            if (vehiculosList.Count() > 0)
                total = vehiculosList.ToList().FirstOrDefault().total;

            request.PageSize = pagination.PageSize;
            var result = new DataSourceResult()
            {
                Data = vehiculosList,
                Total = total
            };

            return Json(result);
        }




        public IActionResult ajax_OpenPropietarioList()
        {
            return PartialView("_listadoPersonasPropietarios");
        }



        public ActionResult GetAllPersonasPagination([DataSourceRequest] DataSourceRequest request)
        {
            filterValue(request.Filters);

            Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            pagination.PageSize = (request.PageSize != 0) ? request.PageSize : 10;
            pagination.Filter = resultValue;

            var personasList = _personasService.GetAllPersonasPagination(pagination);
            var total = 0;
            if (personasList.Count() > 0)
                total = personasList.ToList().FirstOrDefault().total;

            request.PageSize = pagination.PageSize;
            var result = new DataSourceResult()
            {
                Data = personasList,
                Total = total
            };

            return Json(result);
        }

        public IActionResult GetBuscarInfraccionesNormal([DataSourceRequest] DataSourceRequest request, InfraccionesBusquedaModel model)
        {
            infraModel = model;
            return PartialView("_ListadoInfracciones", new List<InfraccionesModel>());
        }
		public IActionResult GetBuscarInfraccionesInvisibles([DataSourceRequest] DataSourceRequest request, InfraccionesBusquedaModel model)
		{
			infraModel = model;
			return PartialView("_ListadoInfraccionesInvisibles", new List<InfraccionesModel>());
		}
		public IActionResult GetAllBuscarInfraccionesPagination([DataSourceRequest] DataSourceRequest request, InfraccionesBusquedaModel model)

        {
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
   
			//int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
			int idOficina = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value) ;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("Autorizaciones");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);


            Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            pagination.PageSize = (request.PageSize != 0) ? request.PageSize : 10;
            pagination.Filter = resultValue;

            if (infraModel == null)
                infraModel = model;

           // var listReporteAsignacion = _infraccionesService.GetAllInfraccionesPagination(infraModel, idOficina, idDependencia, pagination);
            var listReporteAsignacion = _infraccionesService.GetAllInfraccionesPaginationWhitcode(infraModel, idOficina, idDependencia, pagination);
            var total = 0;
            if (listReporteAsignacion.Count() > 0)
                total = listReporteAsignacion.ToList().FirstOrDefault().Total;

            request.PageSize = pagination.PageSize;
            var result = new DataSourceResult()
            {
                Data = listReporteAsignacion,
                Total = total
            };
            return Json(result);

        }
		public IActionResult GetAllBuscarInfraccionesInvisiblesPagination([DataSourceRequest] DataSourceRequest request, InfraccionesBusquedaModel model)

		{
			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

			//int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
			int idOficina = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value);
			string listaIdsPermitidosJson = HttpContext.Session.GetString("Autorizaciones");
			List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);


			Pagination pagination = new Pagination();
			pagination.PageIndex = request.Page - 1;
			pagination.PageSize = (request.PageSize != 0) ? request.PageSize : 10;
			pagination.Filter = resultValue;

			if (infraModel == null)
				infraModel = model;

			var listReporteAsignacion = _infraccionesService.GetAllInfraccionesPaginationInv(infraModel, idOficina, idDependencia, pagination);
			var total = 0;
			if (listReporteAsignacion.Count() > 0)
				total = listReporteAsignacion.ToList().FirstOrDefault().Total;

			request.PageSize = pagination.PageSize;
			var result = new DataSourceResult()
			{
				Data = listReporteAsignacion,
				Total = total
			};
			return Json(result);

		}

		private void filterValue(IEnumerable<IFilterDescriptor> filters)
        {
            if (filters.Any())
            {
                foreach (var filter in filters)
                {
                    var descriptor = filter as FilterDescriptor;
                    if (descriptor != null)
                    {
                        resultValue = descriptor.Value.ToString();
                        break;
                    }
                    else if (filter is CompositeFilterDescriptor)
                    {
                        if (resultValue == "")
                            filterValue(((CompositeFilterDescriptor)filter).FilterDescriptors);
                    }
                }
            }
        }


        public IActionResult DescargarItem(string IdInfraccion)
        {

            var path = _infraccionesService.GetPathFile(IdInfraccion);

            byte[] data = new byte[] { };

            if (System.IO.File.Exists(path.value) && !string.IsNullOrEmpty(path.text))
            {
                Console.WriteLine("La condición se cumple");

                data = System.IO.File.ReadAllBytes(path.value);
                MemoryStream stream = new MemoryStream(data);

                var contentType = "application/octet-stream";
                var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                if (provider.TryGetContentType(path.value, out var mimeType))
                {
                    contentType = mimeType;
                }



                string b64 = Convert.ToBase64String(stream.ToArray());
                return Json(new { file = b64, app = contentType, name = path.text });
            }
            else
            {
                Console.WriteLine("NO SE cumple");

                return Json(new { file = "" });
            }





        }

        public IActionResult ajax_guardarDetenidos([FromBody] dynamic datos)
        {
            int idInfraccion = datos.idInfraccion;
            foreach (var dato in datos.datos)
            {

                int idPersona = dato.IdPersona;
                string folioDetenido = dato.FolioDetenido;

                // Llamar al servicio para guardar o actualizar los datos
                int id = _infraccionesService.CreateUpdatePersonaFolioDetencionPersona(idInfraccion, folioDetenido, idPersona);
            }

            return Json(new { success = true });
        }

        public static string CleanNullString(string input)
        {
            return input?.Replace("NULL", "").Trim();
        }

        public IActionResult ajax_EditarPersona(PersonaModel model, int percan, int Ope, int IsCom)
        {
            //var model = json.ToObject<Gruas2Model>();
            //validacion
            model.PersonaDireccion.telefono = CleanNullString(model.PersonaDireccion.telefono);
            model.PersonaDireccion.correo = CleanNullString(model.PersonaDireccion.correo);
            model.PersonaDireccion.numero = CleanNullString(model.PersonaDireccion.numero);
            model.PersonaDireccion.calle = CleanNullString(model.PersonaDireccion.calle);
            model.PersonaDireccion.colonia = CleanNullString(model.PersonaDireccion.colonia);

            var errors = ModelState.Values.Select(s => s.Errors);

            if (model.PersonaDireccion.idPersona == null || model.PersonaDireccion.idPersona <= 0)
            {
                model.PersonaDireccion.idPersona = model.idPersona;
                int idDireccion = _personasService.CreatePersonaDireccion(model.PersonaDireccion);

                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                if (IsCom == 1)
                {
                    _bitacoraServices.insertBitacora(Ope, ip, string.Format("Se agrega Conductor {0} {1}", model.nombre, model.apellidoMaterno), "Agregar Conductor", "insert", user);
                }
                else
                {
                    _bitacoraServices.insertBitacora(Ope, ip, string.Format("Se agrega Propietario {0} {1}", model.nombre, model.apellidoMaterno), "Agregar propietario", "insert", user);
                }

            }
            else
            {
                int idDireccion = _personasService.UpdatePersonaDireccion(model.PersonaDireccion);
                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                if (IsCom == 1)
                {
                    _bitacoraServices.insertBitacora(Ope, ip, string.Format("Se actualiza Conductor {0} {1}", model.nombre, model.apellidoMaterno), "Actualizar Conductor", "insert", user);
                }
                else
                {
                    _bitacoraServices.insertBitacora(Ope, ip, string.Format("Se actualiza Propietario {0} {1}", model.nombre, model.apellidoMaterno), "Actualizar propietario", "insert", user);
                }
            }
            int id = 0;
            if (model.idPersona != null && model.idPersona > 0)
                id = _personasService.UpdatePersona(model);
            else
                id = _personasService.CreatePersona(model);

            if (Ope > 0)
            {
                _infraccionesService.CreateUpdatePersonaFolioDetencionPersona(Ope, model.folioDetencion, (int)model.idPersona);
            }
            else
            {
                _infraccionesService.CreateUpdatePersonaFolioDetencionPersona(Ope, model.folioDetencion, (int)model.idPersona);
            }

            if (percan == 1)
            {
                if (IsCom == 1)
                {
                    _personasService.UpdateHistoricoPersonas(model.idPersona.Value, Ope);
                }
                else
                {
                    _personasService.UpdateHistoricoPersonasProp(Ope, model.idPersona.Value);
                }
            }


            return Json(new { data = id, id = model.PersonaDireccion.idPersona });


        }




        public IActionResult UpdatePropietario(int idPersona, int idVehiculo)
        {


            var t = _vehiculosService.UpdatePropietario(idPersona, idVehiculo);

            HttpContext.Session.SetInt32("EditPropietario", 1);

            return Json(new { data = "1" });
        }


        public IActionResult ajax_ModalEditarPersona(int id, int esConductor, int UpdatePermanencia, int Ope)
        {




            var catTipoPersona = _catDictionary.GetCatalog("CatTipoPersona", "0");
            var catTipoLicencia = _catDictionary.GetCatalog("CatTipoLicencia", "0");
            var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            var catGeneros = _catDictionary.GetCatalog("CatGeneros", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");

            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            ViewBag.CatGeneros = new SelectList(catGeneros.CatalogList, "Id", "Text");
            ViewBag.CatEntidades = new SelectList(catEntidades.CatalogList, "Id", "Text");
            ViewBag.CatTipoPersona = new SelectList(catTipoPersona.CatalogList, "Id", "Text");
            ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
            ViewBag.esConductor = esConductor;
            ViewBag.UpdatePermanencia = UpdatePermanencia;

            ViewBag.Ope = Ope;
            if (id == 0)
            {
                var model2 = new PersonaModel();
                return PartialView("_EditarPersonaReturnId", model2);
            }

            var model = _personasService.GetPersonaById(id);
            var personaFolioDetencion = _infraccionesService.GetPersonaFolioDetencion(id);
            model.folioDetencion = personaFolioDetencion;

            return PartialView("_EditarPersonaReturnId", model);
        }


        public IActionResult ajax_ModalEditarPersonaHistorico(int id, int esConductor, int idinfraccion, int UpdatePermanencia, int Ope)
        {
            var catTipoPersona = _catDictionary.GetCatalog("CatTipoPersona", "0");
            var catTipoLicencia = _catDictionary.GetCatalog("CatTipoLicencia", "0");
            var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            var catGeneros = _catDictionary.GetCatalog("CatGeneros", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");

            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            ViewBag.CatGeneros = new SelectList(catGeneros.CatalogList, "Id", "Text");
            ViewBag.CatEntidades = new SelectList(catEntidades.CatalogList, "Id", "Text");
            ViewBag.CatTipoPersona = new SelectList(catTipoPersona.CatalogList, "Id", "Text");
            ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
            ViewBag.esConductor = esConductor;
            ViewBag.UpdatePermanencia = UpdatePermanencia;
            ViewBag.Ope = Ope;
            if (id == 0)
            {
                var model2 = new PersonaModel();
                return PartialView("_EditarPersonaReturnId", model2);
            }

            var model = _personasService.GetPersonaByIdHistorico(id, idinfraccion, esConductor == 1 ? 1 : 2);
            var personaFolioDetencion = _infraccionesService.GetPersonaFolioDetencion(id);
            model.folioDetencion = personaFolioDetencion;
            //validacion
            model.PersonaDireccion.telefono = CleanNullString(model.PersonaDireccion.telefono);
            model.PersonaDireccion.correo = CleanNullString(model.PersonaDireccion.correo);
            model.PersonaDireccion.numero = CleanNullString(model.PersonaDireccion.numero);
            model.PersonaDireccion.calle = CleanNullString(model.PersonaDireccion.calle);
            model.PersonaDireccion.colonia = CleanNullString(model.PersonaDireccion.colonia);

            return PartialView("_EditarPersonaReturnId", model);
        }


        public IActionResult ajax_ModalEditarVehiculo(int id, int idInfraccion)
        {
            var model = _vehiculosService.GetVehiculoId(id.ToString());
            model.idInfraccion = idInfraccion;
            return PartialView("_EditarVehiculoReturnId", model);
        }
        public IActionResult ajax_ModalEditarVehiculoHistorico(int id, int idInfraccion, int DoEditar)
        {
            var model = _vehiculosService.GetVehiculoIdHistorico(id, idInfraccion);
            model.idInfraccion = idInfraccion;
            ViewBag.DoEditar = DoEditar;
            return PartialView("_EditarVehiculoReturnId", model);
        }


        public IActionResult ajax_EditarVehiculoInf(VehiculoEditViewModel data, int idInfraccion)
        {
            var id = _vehiculosService.UpdateFromEditVehiculo(data);

            try
            {
                _vehiculosService.CrearHistoricoVehiculo(idInfraccion, Convert.ToInt32(data.id), 1);
            }
            catch (Exception e) { }


            var tt = data;

            return Json(new { data = "1", id = tt });
        }



        public ActionResult Editar(int idInfraccion, int id, string regreso, bool? showE = false, bool? modoSoloLectura = false, int? edit = 0)
        {

            HttpContext.Session.SetInt32("IDOpeInfracciones", id);

            ViewBag.ModoSoloLectura = modoSoloLectura;

            int modoMostrar = 0;
            if (ViewBag.ModoSoloLectura)
            {
                modoMostrar = 1;
                HttpContext.Session.SetInt32("modoMostrar", modoMostrar);
            }

             if (edit == 1) //Permitira entrar al modo editar despues de haber entrado al modo mostrar
            {
                HttpContext.Session.SetInt32("modoMostrar", 0);
            }

            if (edit != 1) //Permitira entrar al modo editar despues de haber entrado al modo mostrar
            {
                HttpContext.Session.SetInt32("modoMostrar", 0);
            }

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            int ids = id != 0 ? id : idInfraccion;

            var model = _infraccionesService.GetInfraccion2ById(ids, idDependencia);
            var (latitud, longitud) = _infraccionesService.GetLatitudLongitudPorInfraccion(ids);
            model.Latitud = latitud;
            model.Longitud = longitud;            
            model.isPropietarioConductor = model.Vehiculo != null ? model.Vehiculo.idPersona == model.idPersonaConductor : false;
            model.Vehiculo.cargaTexto = model.Vehiculo == null ? "No" : (model.Vehiculo.carga == true) ? "Si" : "No";
            model.Persona = model.Persona ?? new PersonaModel();

            var ToDepositos = _infraccionesService.ExitDesposito(id);
            HttpContext.Session.SetInt32("LastInfCapturada", id);
            ViewBag.GoDEpositos = ToDepositos;
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            ViewBag.CatCarreteras = new SelectList(_catCarreterasService.GetCarreterasPorDelegacion(idOficina), "IdCarretera", "Carretera");
            model.Persona.PersonaDireccion = model.Persona.PersonaDireccion ?? new PersonaDireccionModel();
            model.Persona.folioDetencion = model.Persona.folioDetencion ?? _infraccionesService.GetPersonaFolioDetencion((int)model.Persona.idPersona);
            model.PersonaInfraccion2.folioDetencion = model.Persona.folioDetencion ?? _infraccionesService.GetPersonaFolioDetencion((int)model.Persona.idPersona);

            var catTramos = _catDictionary.GetCatalog("CatTramosByFilter", model.idCarretera.ToString());

            var catOficiales = _catDictionary.GetCatalog("CatOficiales", "0");
            //var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var catMunicipios = _catMunicipiosService.GetMunicipiosPorDelegacion2(corp);
            catMunicipios.Add(new CatMunicipiosModel() { IdMunicipio = 1, Municipio = "No aplica" });
            catMunicipios.Add(new CatMunicipiosModel() { IdMunicipio = 2, Municipio = "No especificado" });

            var catCarreteras = _catDictionary.GetCatalog("CatCarreteras", "0");
            var catGarantias = _catDictionary.GetCatalog("CatGarantias", "0");
            var catTipoLicencia = _catDictionary.GetCatalog("CatTipoLicencia", "0");
            var catTipoPlaca = _catDictionary.GetCatalog("CatTipoPlaca", "0");
            var CatAplicadoA = _catDictionary.GetCatalog("CatAplicadoA", "0");
            ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
            ViewBag.CatTipoPlaca = new SelectList(catTipoPlaca.CatalogList, "Id", "Text");
            ViewBag.CatTramos = new SelectList(catTramos.CatalogList, "Id", "Text");
            ViewBag.CatOficiales = new SelectList(catOficiales.CatalogList, "Id", "Text");
            ViewBag.CatMunicipios = new SelectList(catMunicipios, "IdMunicipio", "Municipio");
            ViewBag.CatGarantias = new SelectList(catGarantias.CatalogList, "Id", "Text");
            ViewBag.CatAplicadoA = new SelectList(CatAplicadoA.CatalogList, "Id", "Text");      
            ViewBag.EsSoloLectura = showE.HasValue && showE.Value;
            ViewBag.EditarVehiculo = true;
            ViewBag.Regreso = regreso == null ? "0" : regreso;

            if ((model.MotivosInfraccion == null || model.MotivosInfraccion.Count() == 0) || (model.idGarantia == null || model.idGarantia == 0))
            {
                HttpContext.Session.SetString("isedition", "0");
            }
            else
            {
                HttpContext.Session.SetString("isedition", "1");
            }
            ViewBag.totalUMAS = model.MotivosInfraccion.Sum(s => (int)s.calificacion);

            //validacion
            model.Persona.PersonaDireccion.telefono = CleanNullString(model.Persona.PersonaDireccion.telefono);
            model.Persona.PersonaDireccion.correo = CleanNullString(model.Persona.PersonaDireccion.correo);
            model.Persona.PersonaDireccion.numero = CleanNullString(model.Persona.PersonaDireccion.numero);
            model.Persona.PersonaDireccion.calle = CleanNullString(model.Persona.PersonaDireccion.calle);
            model.Persona.PersonaDireccion.colonia = CleanNullString(model.Persona.PersonaDireccion.colonia);

            model.PersonaInfraccion.PersonaDireccion.telefono = CleanNullString(model.PersonaInfraccion.PersonaDireccion.telefono);
            model.PersonaInfraccion.PersonaDireccion.correo = CleanNullString(model.PersonaInfraccion.PersonaDireccion.correo);
            model.PersonaInfraccion.PersonaDireccion.numero = CleanNullString(model.PersonaInfraccion.PersonaDireccion.numero);
            model.PersonaInfraccion.PersonaDireccion.calle = CleanNullString(model.PersonaInfraccion.PersonaDireccion.calle);
            model.PersonaInfraccion.PersonaDireccion.colonia = CleanNullString(model.PersonaInfraccion.PersonaDireccion.colonia);

            model.PersonaInfraccion2.PersonaDireccion.telefono = CleanNullString(model.PersonaInfraccion2.PersonaDireccion.telefono);
            model.PersonaInfraccion2.PersonaDireccion.correo = CleanNullString(model.PersonaInfraccion2.PersonaDireccion.correo);
            model.PersonaInfraccion2.PersonaDireccion.numero = CleanNullString(model.PersonaInfraccion2.PersonaDireccion.numero);
            model.PersonaInfraccion2.PersonaDireccion.calle = CleanNullString(model.PersonaInfraccion2.PersonaDireccion.calle);
            model.PersonaInfraccion2.PersonaDireccion.colonia = CleanNullString(model.PersonaInfraccion2.PersonaDireccion.colonia);
            TempData["Ids"] = ids;
            var cortesias = _infraccionesService.GetCortesias(ids);
            ViewBag.Cortesias = cortesias;

            return View(model);

        }
        public IActionResult ObtieneDatosInfracciones([DataSourceRequest] DataSourceRequest request)
        {
            if (TempData["Ids"] is int ids)
            {
                var ListaInfracciones = _infraccionesService.GetCortesias(ids);
                return Json(ListaInfracciones.ToDataSourceResult(request));
            }
            return Json(new { success = false, message = "ID no encontrado." });
        }

        public ActionResult EditarA(int idInfraccion, int id)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            int ids = id != 0 ? id : idInfraccion;

            int count = ("MONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\n").Length;
            var model = _infraccionesService.GetInfraccionAccidenteById(id, idDependencia);
            model.isPropietarioConductor = model.Vehiculo.idPersona == model.IdPersona;


            var ToDepositos = _infraccionesService.ExitDesposito(idInfraccion);

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            ViewBag.CatCarreteras = new SelectList(_catCarreterasService.GetCarreterasPorDelegacion(idOficina), "IdCarretera", "Carretera");


            var catTramos = _catDictionary.GetCatalog("CatTramosByFilter", model.IdCarretera.ToString());
            var catOficiales = _catDictionary.GetCatalog("CatOficiales", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");
            var catCarreteras = _catDictionary.GetCatalog("CatCarreteras", "0");
            var catGarantias = _catDictionary.GetCatalog("CatGarantias", "0");
            var catTipoLicencia = _catDictionary.GetCatalog("CatTipoLicencia", "0");
            var catTipoPlaca = _catDictionary.GetCatalog("CatTipoPlaca", "0");
            ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
            ViewBag.CatTipoPlaca = new SelectList(catTipoPlaca.CatalogList, "Id", "Text");
            ViewBag.CatTramos = new SelectList(catTramos.CatalogList, "Id", "Text");
            ViewBag.CatOficiales = new SelectList(catOficiales.CatalogList, "Id", "Text");
            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            ViewBag.CatGarantias = new SelectList(catGarantias.CatalogList, "Id", "Text");

            return View("Editar2", model);
        }


        [HttpPost]
        public async Task<ActionResult> ajax_editarInfraccion(InfraccionesModel model,string havecortesias,string FechaVencimientoAux , string ObsCortesia)
        {
            var propietario = model.Persona.idPersona;
            if ((bool)model.infraccionCortesia)
            {
                model.cortesiaInt = 2;
            }
            else
            {
                model.cortesiaInt = 1;
            }

            HttpContext.Session.SetInt32("Cortesia", model.cortesiaInt);


            var motivos = _infraccionesService.ConteoMotivos(model.idInfraccion);

            bool tieneMotivos = motivos > 0;

            if (!tieneMotivos)
            {
                return Json(new { success = false, message = "La infracción debe tener al menos un motivo asignado." });
            }

            var Mpio = _catMunicipiosService.GetMunicipioByID(model.idMunicipio.Value);

            if (!model.folioEmergencia.IsNullOrEmpty())
            {

                string message="", httpError="";
                bool isValid;

                if (User.FindFirst(CustomClaims.TipoOficina).Value.toInt() <2)
                {
                    (isValid, message, httpError) = await _cadService.FolioCadAsync(model.folioEmergencia, "C5i");
                }
                else
                {
                    (isValid, message, httpError) = await _cadService.FolioCadAsync(model.folioEmergencia, Mpio.Municipio);
                }


                
                if (!isValid)
                {
                    return Json(new { success = false, message, httpError });

                }
            }

            var isedition = HttpContext.Session.GetString("isedition");


            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            int idGarantia = 0;
            int idInf = model.idInfraccion;
            if (model.idGarantia == null || model.idGarantia == 0)
            {
                model.Garantia.numPlaca = model.Vehiculo == null ? "" : model.Vehiculo.placas.ToUpper();
                model.Garantia.numLicencia = model.PersonaInfraccion2?.numeroLicencia ?? model.Garantia.numLicencia?.ToUpper()??"";
                model.Garantia.idTipoLicencia = model.PersonaInfraccion2?.idTipoLicencia ?? model.Garantia.idTipoLicencia;
                idGarantia = _infraccionesService.CrearGarantiaInfraccion(model.Garantia, idInf);
                model.idGarantia = idGarantia;
            }
            else
            {
                model.Garantia.idGarantia = model.idGarantia;
                var result = _infraccionesService.ModificarGarantiaInfraccion(model, idInf);
            }

            int municipioid = model.idMunicipio ?? 0;

            model.idDelegacion = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value); //HttpContext.Session.GetInt32("IdOficina") ?? 0;
            //model.fechaVencimiento = getFechaVencimiento(model.fechaInfraccion, municipioid);
            int conteoCortesias;
            if ( int.TryParse(havecortesias,out conteoCortesias))
            {
                if (conteoCortesias > 0)
                {
                    DateTime datevencimiento;

                    DateTime date = model.fechaVencimiento;


                    if (DateTime.TryParseExact(FechaVencimientoAux,"dd/MM/yyyy",CultureInfo.InvariantCulture,DateTimeStyles.None,out datevencimiento))
                    {

                        if(date == datevencimiento)
                        {
                            model.fechaVencimiento = date;

                        }
                        else
                        {
                            var i = _infraccionesService.guardarInfraccionCortesia(model.idInfraccion, model.fechaVencimiento, ObsCortesia);
                            model.fechaVencimiento = date;
                        }

                    }
                        
                }
                else
                {
                    var i = _infraccionesService.guardarInfraccionCortesia(model.idInfraccion, model.fechaVencimiento, ObsCortesia);
                }

            }


            if (HttpContext.Session.GetInt32("IdInfraConductor") != null)
            {
                if (Convert.ToInt32(HttpContext.Session.GetInt32("IdInfraConductor")) > 0)
                    model.idPersonaConductor = Convert.ToInt32(HttpContext.Session.GetInt32("IdInfraConductor"));
            }
            model.monto = _infraccionesService.MontoMotivos(model.idInfraccion);
            var idInfraccion = _infraccionesService.ModificarInfraccion(model);
            HttpContext.Session.Remove("IdInfraConductor");


            var modelVehiculo = new VehiculoModel();
            var modelPersona = new PersonaModel();
            var modelConductor = new PersonaModel();
            if (model.idVehiculo.HasValue)
                modelVehiculo = _vehiculosService.GetVehiculoById(model.idVehiculo.Value);
            if (model.idPersonaConductor > 0)
                modelConductor = _personasService.GetPersonaById(model.idPersonaConductor);
            if (modelVehiculo.idPersona > 0)
                modelPersona = _personasService.GetPersonaById(modelVehiculo.idPersona.Value);


            var EditVehiculo = HttpContext.Session.GetInt32("EditVehiculo");
            var EditPropietario = HttpContext.Session.GetInt32("EditPropietario");
            var EditConductor = HttpContext.Session.GetInt32("EditConductor");

            if (EditVehiculo.HasValue)
                _bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("Se Selecciona vehiculo con placas  {0}", modelVehiculo.placas), "Editar 1ra parte", "create 2 infraccion", user, modelVehiculo);
            if (model.idPersonaConductor > 0)
                _bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("Se Selecciona Persona Conductor  {0}", modelConductor.nombre + " " + modelConductor.apellidoPaterno), "Editar 1ra parte", "create 2 infraccion", user, modelConductor);
            if (EditPropietario.HasValue)
                _bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("Se Selecciona Persona Propietario  {0}", modelPersona.nombre + " " + modelPersona.apellidoPaterno), "Editar 1ra parte", "create 2 infraccion", user, modelPersona);


            HttpContext.Session.Remove("EditVehiculo");
            HttpContext.Session.Remove("EditPropietario");
            HttpContext.Session.Remove("EditConductor");

            if (isedition == "0")
            {
                _bitacoraServices.insertBitacora(model.idInfraccion, ip, string.Format("Se capturó la infracción con folio: {0} {1} con {2} motivos . La segunda parte de la captura", model.folioInfraccion, (model.cortesiaInt == 1 ? "" : " de cortesia"), motivos), "Crear 2da parte", "create 2 infraccion", user, model);

            }
            else
            {
                _bitacoraServices.insertBitacora(model.idInfraccion, ip, string.Format("Se modificó la infracción con folio: {0}. La primera parte de la captura", model.folioInfraccion), "Modificar 2da parte", "create 2 infraccion", user);


            }

            var idVehiculo = model.idVehiculo;
            Logger.Info("401-Infraccion 2da Parte-ajax_editarInfraccion-(Folio:  " + model.folioInfraccion + ")");

            return Json(new { success = true, idInfraccion = idInfraccion, idVehiculo = idVehiculo });
        }



        public IActionResult ModalEditarFolio(int id, string folio)
        {
            var q = new EditarFolioModel();
            q.NumeroReporte = folio;
            q.IdAccidente = id;

            return PartialView("_EditarFolio", q);
        }

        public IActionResult UpdateFolioS(string id, string folios)
        {

            string folio = folios.Replace(" ", "");

            var t = _infraccionesService.validarFolio(folio);
            if (t)
            {
                var fol = _infraccionesService.UpdateFolioS(id, folio);
            }
            return Json(t);

        }



        /// <summary>
        /// HMG
        /// 10-04-2024
        /// MÉTODO PARA INSERTAR LA INFRACCIÓN, SE CREA UN SOLO MÉTODO Y UN STORE PROCEDURE, 
        /// REGRESANDO LA EXCEPCIÓN EN CASO DE QUE EL FOLIO EXISTA O EXISTA ALGÚN ERROR.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="requestMode"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ajax_crearInfraccion(InfraccionesModel model, CrearMultasTransitoRequestModel requestMode, IFormFile boletaInfraccion)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            int municipioid = model.idMunicipio ?? 0;
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var iddeleg = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value);
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            var idblockfolio = 0;

            var cantContinue = _infraccionesService.ValidarFolio(model.folioInfraccion, idDependencia);

            if (cantContinue)
            {
                return Json(new { success = false, message = "Folio ya registrado en RIAG", errorFolio = 1 });
            }



            if (canPermiso)
            {

                var sufijo = model.folioInfraccion.ToUpper().Substring(0, prefijo.Length); //PEC
                var folioBlock = model.folioInfraccion.Substring(prefijo.Length);  // 135
                var folioBlockInt = 0;//135

                if (sufijo != prefijo.ToUpper()) // KAR   PEC
                {
                    return Json(new { success = false, message = "Folio de infraccion no tiene el formato correcto ("+prefijo+")", errorFolio = 1 });
                }

                int.TryParse(folioBlock, out folioBlockInt);  //  135 es numero???

                if (folioBlockInt == 0) {
                    return Json(new { success = false, message = "Folio de infraccion no tiene el formato correcto (Digito)", errorFolio = 1 });
                }

                idblockfolio = _Block.ExistFolio(folioBlockInt, iddeleg, model.idOficial.Value, 1);

                if(idblockfolio == 0)
                {
                    return Json(new { success = false, message = "Folio de bloc no esta asignado" , errorFolio=1 });
                }
            }



            Logger.Info("400-Creacion de Infraccion-ajax_crearInfraccion-(folio: " + model.folioInfraccion + " )");

            try
            {

                if (!model.folioEmergencia.IsNullOrEmpty())
                {
                    ////C5i para pec y mov  C5i
                    string message = "", httpError = "";
                    bool isValid;
                    var Mpio = _catMunicipiosService.GetMunicipioByID(model.idMunicipio.Value);

                    if (User.FindFirst(CustomClaims.TipoOficina).Value.toInt() < 2)
                    {
                        (isValid, message, httpError) = await _cadService.FolioCadAsync(model.folioEmergencia, "C5i");
                    }
                    else
                    {
                        (isValid, message, httpError) = await _cadService.FolioCadAsync(model.folioEmergencia, Mpio.Municipio);
                    }

                    if (!isValid)
                    {

                        return Json(new { success = false, message, httpError });

                    }
                }


                model.idEstatusInfraccion = (int)CatEnumerator.catEstatusInfraccion.EnProceso;

                model.idDelegacion = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value);     //HttpContext.Session.GetInt32("IdOficina") ?? 0;
                model.fechaVencimiento = getFechaVencimiento(model.fechaInfraccion, idDependencia);

                //validacion


                if (model?.Persona?.PersonaDireccion != null)
                {
                    model.Persona.PersonaDireccion.telefono = CleanNullString(model.Persona.PersonaDireccion.telefono);
                    model.Persona.PersonaDireccion.correo = CleanNullString(model.Persona.PersonaDireccion.correo);
                    model.Persona.PersonaDireccion.numero = CleanNullString(model.Persona.PersonaDireccion.numero);
                    model.Persona.PersonaDireccion.calle = CleanNullString(model.Persona.PersonaDireccion.calle);
                    model.Persona.PersonaDireccion.colonia = CleanNullString(model.Persona.PersonaDireccion.colonia);
                }

                if (model?.PersonaInfraccion?.PersonaDireccion != null)
                {
                    model.PersonaInfraccion.PersonaDireccion.telefono = CleanNullString(model.PersonaInfraccion.PersonaDireccion.telefono);
                    model.PersonaInfraccion.PersonaDireccion.correo = CleanNullString(model.PersonaInfraccion.PersonaDireccion.correo);
                    model.PersonaInfraccion.PersonaDireccion.numero = CleanNullString(model.PersonaInfraccion.PersonaDireccion.numero);
                    model.PersonaInfraccion.PersonaDireccion.calle = CleanNullString(model.PersonaInfraccion.PersonaDireccion.calle);
                    model.PersonaInfraccion.PersonaDireccion.colonia = CleanNullString(model.PersonaInfraccion.PersonaDireccion.colonia);
                }

                if (model?.PersonaInfraccion2?.PersonaDireccion != null)
                {
                    model.PersonaInfraccion2.PersonaDireccion.telefono = CleanNullString(model.PersonaInfraccion2.PersonaDireccion.telefono);
                    model.PersonaInfraccion2.PersonaDireccion.correo = CleanNullString(model.PersonaInfraccion2.PersonaDireccion.correo);
                    model.PersonaInfraccion2.PersonaDireccion.numero = CleanNullString(model.PersonaInfraccion2.PersonaDireccion.numero);
                    model.PersonaInfraccion2.PersonaDireccion.calle = CleanNullString(model.PersonaInfraccion2.PersonaDireccion.calle);
                    model.PersonaInfraccion2.PersonaDireccion.colonia = CleanNullString(model.PersonaInfraccion2.PersonaDireccion.colonia);
                }
                var modelVehiculo = new VehiculoModel();
                
                
                
                var idInfraccion = _infraccionesService.CrearInfraccion(model, idDependencia);




                var modelPersona = new PersonaModel();
                var modelConductor = new PersonaModel();
                if (model.idVehiculo.HasValue)
                    modelVehiculo = _vehiculosService.GetVehiculoById(model.idVehiculo.Value);
                if (model.idPersona.HasValue)
                    modelPersona = _personasService.GetPersonaById(model.idPersona.Value);
                if (modelVehiculo.idPersona > 0)
                    modelConductor = _personasService.GetPersonaById(modelVehiculo.idPersona.Value);

                _bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("Se capturó la infracción con folio: {0} con fecha: {1} a las placas {2}. La primera parte de la captura", model.folioInfraccion, model.fechaInfraccion.ToString("dd/MM/yyyy"), model.placaInfraccion), "Captura de infracción (Inicial)", "insert", user, model);
                _bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("Se Selecciona vehiculo con placas  {0}", modelVehiculo.placas), "Crear 1ra parte", "create 2 infraccion", user, modelVehiculo);
                _bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("Se Selecciona Persona Conductor  {0}", modelPersona.nombre + " " + modelPersona.apellidoPaterno), "Crear 1ra parte", "create 2 infraccion", user, modelPersona);
                _bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("Se Selecciona Persona Propietario  {0}", modelConductor.nombre + " " + modelConductor.apellidoPaterno), "Crear 1ra parte", "create 2 infraccion", user, modelConductor);

                if (boletaInfraccion != null && boletaInfraccion.Length > 0)
                {
                    try
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await boletaInfraccion.CopyToAsync(memoryStream);
                            var fileData = memoryStream.ToArray();

                            var fileName = $"{idInfraccion}_{boletaInfraccion.FileName}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(boletaInfraccion.FileName)}";
                            var filePath = Path.Combine(_rutaArchivo, fileName);

                            System.IO.File.WriteAllBytes(filePath, fileData);

                            var arhivoBoleta = new InfraccionesModel
                            {
                                boletaFisicaPath = filePath,
                                nombreBoletaStr = fileName,
                            };

                            _infraccionesService.AnexarBoletaFisica(arhivoBoleta, idInfraccion);
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = $"Error al guardar el archivo: {ex.Message}" });
                    }
                }
				if (canPermiso)
					_Block.UpdateFolio(idblockfolio, idInfraccion, 1);
				// Devuelve el ID de la infracción si todo ha ido bien
				return Json(new { id = idInfraccion });
            }
            catch (Exception ex)
            {
                return Json(new { id = 0, validacion = false, message = "Ocurrió un error al procesar la solicitud." });
            }
        }


        [HttpGet]
        public JsonResult obtener_ajax_involucrados(int idInfraccion)
        {
            var personasInvolucradas = _infraccionesService.GetPersonasInvolucradas(idInfraccion);
            return Json(personasInvolucradas);
        }

        [HttpPost]
        public ActionResult ajax_ValidarFolio(InfraccionesModel model)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            bool idInfraccion = _infraccionesService.ValidarFolio(model.folioInfraccion, idDependencia);

            return Json(new { id = idInfraccion });

        }
        public ActionResult ModalAgregarVehiculo()
        {
            return PartialView("_ModalVehiculo");
        }





        private async Task<bool> ValidarRobo(RepuveConsgralRequestModel repuveGralModel)
        {
            var estatus = false;
            _bitacoraServices.BitacoraWS("4015-Se inicia la invocación al WS REPUVE Robo:", CodigosWs.C4015, new { token = repuveGralModel.token });

            var repuveConsRoboResponse = (await _repuveService.ConsultaRobo(repuveGralModel))?.FirstOrDefault() ?? new RepuveRoboModel();

            estatus = repuveConsRoboResponse.EsRobado;

            return estatus;
        }


        public VehiculoModel GetVEiculoModelFromFinanzas(RootCotejarDatosRes result)
        {
            var vehiculoEncontradoData = result.MT_CotejarDatos_res.tb_vehiculo[0];
            var vehiculoDireccionData = result.MT_CotejarDatos_res.tb_direccion[0];
            var vehiculoInterlocutorData = result.MT_CotejarDatos_res;
            var idMunicipio = !string.IsNullOrEmpty(vehiculoDireccionData.municipio)
                  ? ObtenerIdMunicipioDesdeBD(vehiculoDireccionData.municipio)
                  : 0;

            var idEntidad = !string.IsNullOrEmpty(vehiculoDireccionData.entidadreg)
                ? ObtenerIdEntidadDesdeBD(vehiculoDireccionData.entidadreg)
                : 0;


            var idColor = !string.IsNullOrEmpty(vehiculoEncontradoData.color)
                ? ObtenerIdColor(vehiculoEncontradoData.color)
                : 0;

            var idMarca = !string.IsNullOrEmpty(vehiculoEncontradoData.marca)
                ? ObtenerIdMarca(vehiculoEncontradoData.marca)
                : 0;

            var idSubmarca = !string.IsNullOrEmpty(vehiculoEncontradoData.linea)
                ? ObtenerIdSubmarca(vehiculoEncontradoData.linea)
                : 0;
            var submarcaLimpio = !string.IsNullOrEmpty(vehiculoEncontradoData.linea)
                ? ObtenerSubmarca(vehiculoEncontradoData.linea)
                : "NA";
            var telefonoValido = !string.IsNullOrEmpty(vehiculoDireccionData.telefono)
                ? LimpiarValorTelefono(vehiculoDireccionData.telefono)
                : 0;
            var cargaBool = ConvertirBool(vehiculoEncontradoData.carga);
            var generoBool = ConvertirGeneroBool(vehiculoInterlocutorData.es_per_fisica?.sexo);

            var idTipo = !string.IsNullOrEmpty(vehiculoEncontradoData.categoria)
             ? ObtenerIdTipoVehiculo(vehiculoEncontradoData.categoria)
             : 0;
            var idTipoServicio = !string.IsNullOrEmpty(vehiculoEncontradoData.servicio)
            ? ObtenerIdTipoServicio(vehiculoEncontradoData.servicio)
            : 0;
            var vehiculoEncontrado = new VehiculoModel
            {
                placas = vehiculoEncontradoData.no_placa,
                serie = vehiculoEncontradoData.no_serie,
                tarjeta = vehiculoEncontradoData.no_tarjeta,
                motor = vehiculoEncontradoData.no_motor,
                otros = vehiculoEncontradoData.otros,
                idColor = idColor,
                idEntidad = idEntidad,
                idMarcaVehiculo = idMarca,
                idSubmarca = idSubmarca,
                submarca = submarcaLimpio,
                idTipoVehiculo = idTipo,
                modelo = vehiculoEncontradoData.modelo,
                capacidad = vehiculoEncontradoData.numpersona,
                carga = cargaBool,
                idCatTipoServicio = idTipoServicio,
                idTipoPersona = vehiculoInterlocutorData.es_per_fisica != null ? 1 : 2,

                Persona = new PersonaModel
                {
                    nombreFisico = vehiculoInterlocutorData.es_per_fisica?.Nombre,
                    apellidoPaternoFisico = vehiculoInterlocutorData.es_per_fisica?.Ape_paterno,
                    apellidoMaternoFisico = vehiculoInterlocutorData.es_per_fisica?.Ape_materno,
                    fechaNacimiento = vehiculoInterlocutorData.es_per_fisica?.Fecha_nacimiento,
                    CURPFisico = vehiculoInterlocutorData.es_per_fisica?.Nro_curp,
                    generoBool = generoBool,
                    nombre = vehiculoInterlocutorData.es_per_moral?.name_org1,
                    RFC = vehiculoInterlocutorData.Nro_rfc,


                    PersonaDireccion = new PersonaDireccionModel
                    {

                        telefono = vehiculoInterlocutorData.es_per_moral != null ? telefonoValido.ToString() : null,
                        telefonoFisico = vehiculoInterlocutorData.es_per_fisica != null ? telefonoValido.ToString() : null,
                        colonia = vehiculoInterlocutorData.es_per_moral != null ? vehiculoDireccionData.colonia : null,
                        coloniaFisico = vehiculoInterlocutorData.es_per_fisica != null ? vehiculoDireccionData.colonia : null,
                        calle = vehiculoInterlocutorData.es_per_moral != null ? vehiculoDireccionData.calle : null,
                        calleFisico = vehiculoInterlocutorData.es_per_fisica != null ? vehiculoDireccionData.calle : null,
                        numero = vehiculoInterlocutorData.es_per_moral != null ? vehiculoDireccionData.nro_exterior : null,
                        numeroFisico = vehiculoInterlocutorData.es_per_fisica != null ? vehiculoDireccionData.nro_exterior : null,
                        idMunicipio = vehiculoInterlocutorData.es_per_moral != null ? idMunicipio : null,
                        idMunicipioFisico = vehiculoInterlocutorData.es_per_fisica != null ? idMunicipio : null,
                        idEntidad = vehiculoInterlocutorData.es_per_moral != null ? idEntidad : null,
                        idEntidadFisico = vehiculoInterlocutorData.es_per_fisica != null ? idEntidad : null,
                    }
                },

                PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel
                {
                    PersonasMorales = new List<PersonaModel>()
                }
            };

            return vehiculoEncontrado;

        }
        private int ObtenerIdMarcaRepuve(string marca)
        {

            string marcaLimpio = marca.Trim();
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var idMarca = _catMarcasVehiculosService.obtenerIdPorMarca(marcaLimpio, corp);
            return idMarca;


        }
        private int ObtenerIdSubmarcaRepuve(string submarca)
        {

            string submarcaLimpio = submarca.Trim();
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var idMarca = _catSubmarcasVehiculosService.obtenerIdPorSubmarca(submarcaLimpio, corp);
            return idMarca;


        }
        private int ObtenerIdTipoServicioRepuve(string servicio)
        {
            int TipoServicio = 0;

            var Tipo = _catDictionary.GetCatalog("CatTipoServicio", "0");

            TipoServicio = Tipo.CatalogList.Where(w => servicio.ToLower().Contains(w.Text.ToLower())).Select(s => s.Id).FirstOrDefault();

            return (int)TipoServicio;
        }
        private int ObtenerIdEntidadRepuve(string entidad)
        {
            int idEntidad = 0;
            var Entidad = _catDictionary.GetCatalog("CatEntidades", "0");
            idEntidad = Entidad.CatalogList
                .Where(w => RemoveDiacritics(w.Text.ToLower()).Contains(RemoveDiacritics(entidad.ToLower())))
                .Select(s => s.Id)
                .FirstOrDefault();
            return (idEntidad);
        }
        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }



        public async Task<IActionResult> CrearvehiculoSinPlaca()
        {
            try
            {
                //var SeleccionVehiculo = _capturaAccidentesService.BuscarPorParametro(model.PlacasBusqueda, model.SerieBusqueda, model.FolioBusqueda);




                var jsonPartialVehiculosByWebServices = await ajax_CrearVehiculoSinPlacasVehiculo();

                return Json(new { noResults = true, data = jsonPartialVehiculosByWebServices });


            }
            catch (Exception ex)
            {
                return Json(new { noResults = true, error = "Se produjo un error al procesar la solicitud", data = "" });
            }
        }


        private async Task<string> ajax_CrearVehiculoSinPlacasVehiculo()
        {

            var models = new VehiculoModel();
            models.Persona = new PersonaModel();
            models.Persona.PersonaDireccion = new PersonaDireccionModel();
            models.PersonasFisicas = new List<PersonaModel>();
            models.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            models.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
            models.placas = "";
            models.serie = "";
            models.RepuveRobo = new RepuveRoboModel();
            var result = await this.RenderViewAsync2("", models);
            return result;
        }



        public async Task<ActionResult> BuscarVehiculo(VehiculoBusquedaModel model)
        {
            try
            {
                //var SeleccionVehiculo = _capturaAccidentesService.BuscarPorParametro(model.PlacasBusqueda, model.SerieBusqueda, model.FolioBusqueda);
                var SeleccionVehiculo = _vehiculosService.BuscarPorParametro(model.PlacasBusqueda ?? "", model.SerieBusqueda ?? "", model.FolioBusqueda);

                if (SeleccionVehiculo > 0)
                {
                    var text = "";
                    var value = "";

                    if (!string.IsNullOrEmpty(model.SerieBusqueda))
                    {
                        text = "serie";
                        value = model.SerieBusqueda;

                    }
                    else if (!string.IsNullOrEmpty(model.PlacasBusqueda))
                    {
                        text = "placas";
                        value = model.PlacasBusqueda.ToUpper();
                    }



                    return Json(new { noResults = false, data = value, field = text });
                }
                else
                {
                    var jsonPartialVehiculosByWebServices = await ajax_BuscarVehiculo(model);

                    if (jsonPartialVehiculosByWebServices != null)
                    {
                        return Json(new { noResults = false, data = jsonPartialVehiculosByWebServices,
                            servicioDesactivado = true,
                            mensaje = "El servicio de consulta de vehículos en REPUVE está desactivado."

                        });
                    }
                    else
                    {
                        return Json(new { noResults = true, data = "" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { noResults = true, error = "Se produjo un error al procesar la solicitud", data = "" });
            }
        }

        [HttpPost]
        public async Task<string> ajax_BuscarVehiculo(VehiculoBusquedaModel model)
        {
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			try
			{
                if (!string.IsNullOrEmpty(model.PlacasBusqueda))
                {
                    model.PlacasBusqueda = model.PlacasBusqueda.ToUpper();
                }
                if (!string.IsNullOrEmpty(model.SerieBusqueda))
                {
                    model.SerieBusqueda = model.SerieBusqueda.ToUpper();
                }


                var models = await _vehiculoPlataformaService.BuscarVehiculoEnPlataformas(model,corp);

                if (!string.IsNullOrEmpty(model.PlacasBusqueda) && !string.IsNullOrEmpty(models.serie))
                    models.idVehiculo = _vehiculosService.BuscarvehiculoSerie(models.serie);


                HttpContext.Session.SetInt32("IdMarcaVehiculo", models.idMarcaVehiculo);


                var test = await this.RenderViewAsync2("", models);


                return test;
            }
            catch (Exception ex)
            {
                Logger.Error("Infracciones - ajax_BuscarVehiculo: " + ex.Message);
                return null;
            }
        }







        private int ObtenerIdMunicipioDesdeBD(string municipio)
        {
            int idMunicipio = 0;

            var municipioStr = _catDictionary.GetCatalog("CatMunicipios", "0");

            idMunicipio = municipioStr.CatalogList
                            .Where(w => RemoveDiacritics(w.Text.ToLower()).Contains(RemoveDiacritics(municipio.ToLower())))
                            .Select(s => s.Id)
                            .FirstOrDefault();
            return (idMunicipio);
        }


        private int ObtenerIdEntidadDesdeBD(string entidad)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var idEntidad = _catEntidadesService.obtenerIdPorEntidad(entidad, corp);
            return (idEntidad);
        }
        private string ObtenerSubmarca(string submarca)
        {
            string[] partes = submarca.Split(new[] { '-' }, 2);

            if (partes.Length > 1)
            {
                string submarcaLimpio = partes[1].Trim();

                return submarcaLimpio;
            }

            return "NA"; // Valor predeterminado en caso de no encontrar el guión
        }
        private bool ConvertirBool(string carga)
        {
            bool cargaBool = false;

            if (carga.Trim() == "1.00")
            {
                cargaBool = true;
            }
            else if (carga.Trim() == "0.00")
            {
                cargaBool = false;
            }
            return (cargaBool);
        }


        private int ObtenerIdColor(string color)
        {
            string colorLimpio = Regex.Replace(color, "[0-9-]", "").Trim();
            var idColor = _coloresService.obtenerIdPorColor(colorLimpio);
            return (idColor);
        }
        private int ObtenerIdMarca(string marca)
        {
            string[] partes = marca.Split(new[] { '-' }, 2);

            if (partes.Length > 1)
            {
                string marcaLimpio = partes[1].Trim();
                var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

                var idMarca = _catMarcasVehiculosService.obtenerIdPorMarca(marcaLimpio, corp);
                return idMarca;
            }

            return 0; // Valor predeterminado en caso de no encontrar el guión
        }





        private int ObtenerIdSubmarca(string submarca)
        {
            string[] partes = submarca.Split(new[] { '-' }, 2);
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;


            if (partes.Length > 1)
            {
                string submarcaLimpio = partes[1].Trim();

                var idMarca = _catSubmarcasVehiculosService.obtenerIdPorSubmarca(submarcaLimpio, corp);
                return idMarca;
            }

            return 0; // Valor predeterminado en caso de no encontrar el guión
        }
        private int ObtenerIdTipoVehiculo(string categoria)
        {

            int idTipo = 0;

            var tipoVehiculo = _catDictionary.GetCatalog("CatTiposVehiculo", "0");

            idTipo = tipoVehiculo.CatalogList.Where(w => categoria.ToLower().Contains(w.Text.ToLower())).Select(s => s.Id).FirstOrDefault();

            return (idTipo);

        }
        private int ObtenerIdTipoServicio(string servicio)
        {
            int servicioNumero = int.Parse(servicio.TrimStart('0'));
            var idTipoVehiculo = _catDictionary.GetCatalog("CatTipoServicio", "0");

            var tipoServicio = idTipoVehiculo.CatalogList.FirstOrDefault(item => item.Id == servicioNumero)?.Id;

            return (int)tipoServicio;
        }



        private bool ConvertirGeneroBool(string sexo)
        {
            if (sexo == "2")
            {
                return true;
            }
            else if (sexo == "1")
            {
                return false;
            }
            else
            {
                return false;
            }

        }

        private long LimpiarValorTelefono(string telefono)
        {
            telefono = telefono.Replace(" ", "");

            long telefonoValido;

            if (long.TryParse(telefono, out telefonoValido))
            {
                return telefonoValido;
            }
            else
            {
                return 0; // O algún otro valor que indique que no es válido
            }
        }





        [HttpGet]
        public ActionResult ajax_ModalCrearMotivo()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var catConcepto = _CustomCatalog.GetCatConceptoInfraccion(corp);
            ViewData["CatConcepto"] = new SelectList(catConcepto, "value", "text");

            //ViewBag.CatConcepto = catConcepto;
            return PartialView("_CrearMotivo", new MotivoInfraccionModel());
        }

        [HttpPost]
        public ActionResult ajax_CrearMotivos(MotivoInfraccionModel model)
        {
            var id = _infraccionesService.CrearMotivoInfraccion(model);
            var modelList = _infraccionesService.GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
            var fecha = _infraccionesService.GetDateInfraccion(model.idInfraccion);

            var umas = _infraccionesService.GetUmas(fecha);
            ViewBag.Umas = umas;
            ViewBag.totalUMAS = modelList.Sum(s => s.calificacion);
            ViewBag.Totales = modelList.Sum(s => s.calificacion) * umas;
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            var isedition = HttpContext.Session.GetString("isedition");
            if (isedition == "1")
            {
                _bitacoraServices.insertBitacora(model.idInfraccion, ip, string.Format("Se agrega motivo de infraccion con calificacion {0} con calificaciontotal de {1}, con una Uma de {2} y un total de {3}", model.calificacion, ViewBag.totalUMAS, ViewBag.Umas, ViewBag.Totales), "Agregar motivos", "insert", user, model);

            }

            //}
            object data = new { idMotivoInfraccion = id };
            _bitacoraServices.BitacoraGenerales(CodigosGeneral.C5015, data);

            _bitacoraServices.insertBitacora(model.idInfraccion, ip, string.Format("Se agrega motivo de infraccion con calificacion {0} con calificaciontotal de {1}, con una Uma de {2} y un total de {3}", model.calificacion, ViewBag.totalUMAS, ViewBag.Umas, ViewBag.Totales), "Agregar motivos", "insert", user, model);

            Logger.Info("401-Infraccion 2da Parte-Motivos-ajax_CrearMotivos-(Motivo" + model.idMotivoInfraccion + ")");

            return PartialView("_ListadoMotivos", modelList);
        }

        [HttpGet]
        public ActionResult ajax_detalleVehiculo(int idVehiculo, int idInfraccion)
        {

            var model = _vehiculosService.GetVehiculoById(idVehiculo);

            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);

            if (idInfraccion != 0)
                _bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("Se Selecciona vehiculo con placas  {0}", model.placas), "Crear 2da parte", "create 2 infraccion", user, model);

            model.cargaTexto = (model.carga == true) ? "Si" : "No";

            HttpContext.Session.SetInt32("EditVehiculo", 1);


            return PartialView("_DetalleVehiculo", model);
        }

        public ActionResult ajax_detalleVehiculo2(int idVehiculo, int idInfraccion)
        {
            var model = _vehiculosService.GetVehiculoHistoricoByIdAndIdinfraccion(idVehiculo, idInfraccion);

            model.cargaTexto = (model.carga == true) ? "Si" : "No";
            return PartialView("_DetalleVehiculo", model);
        }

        [HttpGet]
        public ActionResult ajax_detallePersona(int idPersona, int prop)
        {
            var infraccion = HttpContext.Session.GetInt32("IDOpeInfracciones");
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);

            HttpContext.Session.Remove("IdInfraConductor");
            var model = _personasService.GetPersonaById(idPersona);
            var personaFolioDetencion = _infraccionesService.GetPersonaFolioDetencion(idPersona);
            model.folioDetencion = personaFolioDetencion;


            if (infraccion.HasValue && infraccion != 0)
            {
                if (prop != 0)
                    _bitacoraServices.insertBitacora(infraccion.Value, ip, string.Format("Se Selecciona Persona Conductor  {0}", model.nombre + " " + model.apellidoPaterno), "Crear 2da parte", "create 2 infraccion", user, model);
                else
                    _bitacoraServices.insertBitacora(infraccion.Value, ip, string.Format("Se Selecciona Persona Conductor  {0}", model.nombre + " " + model.apellidoPaterno), "Crear 2da parte", "create 2 infraccion", user, model);

            }


            //validacion
            HttpContext.Session.SetInt32("EditConductor", 1);
            if (model.PersonaDireccion != null)
            {
                model.PersonaDireccion.telefono = CleanNullString(model.PersonaDireccion.telefono);
                model.PersonaDireccion.correo = CleanNullString(model.PersonaDireccion.correo);
                model.PersonaDireccion.numero = CleanNullString(model.PersonaDireccion.numero);
                model.PersonaDireccion.calle = CleanNullString(model.PersonaDireccion.calle);
                model.PersonaDireccion.colonia = CleanNullString(model.PersonaDireccion.colonia);
            }
            model.folioDetencion = "";

            return PartialView("_DetallePersona", model);
        }

        [HttpGet]
        public ActionResult ajax_listadoVehiculoInfracciones()
        {
            var modelList = _vehiculosService.GetAllVehiculos();
            return PartialView("_ListadoVehiculos", modelList);
        }

        [HttpGet]
        public ActionResult ajax_listadoPersonasInfracciones()
        {
            var modelList = _personasService.GetAllPersonas();
            return PartialView("_ListadoPersonas", modelList);
        }

        [HttpGet]
        public ActionResult ajax_listadoMotivosInfracciones(int idInfraccion)
        {
            var modelList = _infraccionesService.GetMotivosInfraccionByIdInfraccion(idInfraccion);
            return PartialView("_ListadoMotivos", modelList);
        }

        public JsonResult InfraccionesEstatus_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatEstatusInfraccion", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            //var selected = result.Where(x => x.Value == Convert.ToString(idSubmarca)).First();
            //selected.Selected = true;
            return Json(result);
        }

        [HttpGet]
        public ActionResult ajax_CortesiaInfraccion(int id)
        {
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            //var model = _vehiculosService.GetVehiculoById(id);
            var model = _infraccionesService.GetInfraccion2ById(id, idDependencia);
            return PartialView("_Cortesia", model);
        }

        [HttpPost]
        public ActionResult ajax_UpdateCortesiaInfraccion(InfraccionesModel model)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);

            var modelInf = _infraccionesService.ModificarInfraccionPorCortesia(model);
            if (modelInf == 1)
            {

                _bitacoraServices.insertBitacora(model.idInfraccion, ip, string.Format("Se Cambia la Cortesia {0}", (model.cortesiaInt == 3 ? "a no aplicada" : model.cortesiaInt == 4 ? "a Aplicada" : "")), "Crear 2da parte", "create 2 infraccion", user, model);


                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var listInfracciones = _infraccionesService.GetAllInfracciones(idOficina, idDependencia);
                return PartialView("_ListadoInfracciones", listInfracciones);
                //return Json(listInfracciones);
            }
            else
            {
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(null);
            }

        }
        public ActionResult MostrarModalAnexar()
        {
            return PartialView("_ModalAnexar");
        }
        [HttpPost]
        public async Task<IActionResult> SubirImagen(IFormFile file, int idInfraccion)
        {
            if (file != null && file.Length > 0)
            {
                //Se crea el nombre del archivo de la garantia
                string nombreArchivo = _rutaArchivo + "/" + idInfraccion + "_" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").Replace("/", "").Replace(":", "").Replace(" ", "") + System.IO.Path.GetExtension(file.FileName);
                string nombreArchivostr = file.FileName;
                try
                {
                    //Se escribe el archivo en disco
                    using (Stream fileStream = new FileStream(nombreArchivo, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    // Llamar al método del servicio para guardar la imagen
                    int resultado = _infraccionesService.InsertarImagenEnInfraccion(nombreArchivo, idInfraccion, nombreArchivostr);
                    if (resultado == 0)
                        return Json(new { success = false, message = "Ocurrió un error al actualizar infracción" });

                }
                catch (Exception e)
                {
                    Logger.Error("Ocurrió un error al cargar el archivo a la infracción: " + e);
                    return Json(new { success = false, message = "Ocurrió un error al guardar el archivo" });
                }




                /*using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    imageData = memoryStream.ToArray();
                }*/

                return Json(new { success = true, message = "El archivó se agregó exitosamente" });
            }
            else
            {
                return Json(new { success = false, message = "Selecciona una imagen antes de continuar" });
            }

        }

        public IActionResult ServiceCrearInfraccion(int idInfraccion)
        {
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

			var Cortesia = HttpContext.Session.GetInt32("Cortesia") ?? 1;

            _bitacoraServices.BitacoraWS("Inicia ws CrearMultasTransito", CodigosWs.C4009, new { idInfraccion = idInfraccion });
            Logger.Info("410-Envio a finanzas-ServiceCrearInfraccion-(AllowWS: " + _appSettings.AllowWebServices.ToString() + " )");
            var endPointName = "CrearMultasTransito";
            var isActive = _appSettingsService.VerificarActivo(endPointName,idDependencia);
            var Corp = HttpContext.Session.GetInt32("IdDependencia");

            if (isActive && Cortesia == 1 && Corp < 3)
            {
                try
                {
                    var infraccionBusqueda = _infraccionesService.GetInfraccionById(idInfraccion, idDependencia);
                    if (infraccionBusqueda == null)
                    {
                        _bitacoraServices.BitacoraWS("Finaliza ws CrearMultasTransito", CodigosWs.C4010, new { idInfraccion = idInfraccion, message = "Infracción guardada, no enviada a finanzas" });
                        return Json(new { success = false, message = "Infracción guardada, no enviada a finanzas", id = idInfraccion });
                    }

                    var unicoMotivo = infraccionBusqueda.MotivosInfraccion?.FirstOrDefault();
                    int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                    if (infraccionBusqueda.cortesiaInt == 2)
                    {
                        _infraccionesService.ModificarEstatusInfraccion(idInfraccion, (int)CatEnumerator.catEstatusInfraccion.Capturada);
                        _bitacoraServices.BitacoraWS("Finaliza ws CrearMultasTransito", CodigosWs.C4010, new { idInfraccion = idInfraccion, message = "Infracción guardada, no enviada a finanzas" });
                        return Json(new { success = false, message = "Infracción guardada, no enviada a finanzas", id = idInfraccion });
                    }
                    else
                    {
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
                            if (count == 1) { crearMultasRequestModel.ZMOTIVO1 = enumeration.Motivo.Cut(250); }
                            if (count == 2) { crearMultasRequestModel.ZMOTIVO2 = enumeration.Motivo.Cut(250); }
                            if (count == 3) { crearMultasRequestModel.ZMOTIVO3 = enumeration.Motivo.Cut(250); }
                            count++;
                        }
                        Logger.Info("410-Envio a finanzas-ServiceCrearInfraccion-(Envio: " + JsonConvert.SerializeObject(crearMultasRequestModel, Formatting.Indented) + " )");

                        _bitacoraServices.BitacoraWS("Envia Modelo wsCrearMultasTransito", CodigosWs.C4010, new { idInfraccion = idInfraccion, modelo = crearMultasRequestModel });

                        var result = _crearMultasTransitoClientService.CrearMultasTransitoCall(crearMultasRequestModel);

                        ViewBag.Pension = result;
                        var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                        var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                        if (result != null && result.MT_CrearMultasTransito_res != null && "S".Equals(result.MT_CrearMultasTransito_res.ZTYPE, StringComparison.OrdinalIgnoreCase))
                        {
                            _infraccionesService.ModificarEstatusInfraccion(idInfraccion, (int)CatEnumerator.catEstatusInfraccion.Enviada);
                            _infraccionesService.GuardarReponse(result.MT_CrearMultasTransito_res, idInfraccion);
                            _bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("Se registró en finanzas la infracción folio: {0} se obtuvo No. de documento = {1}; a través de WS", infraccionBusqueda.folioInfraccion, result.MT_CrearMultasTransito_res.DOCUMENTNUMBER), "Registro Finanzas", "WS", user);
                            _bitacoraServices.BitacoraWS("Finaliza ws CrearMultasTransito", CodigosWs.C4010, new { idInfraccion = idInfraccion, result = result, success = true });
                            return Json(new { success = true });
                        }
                        else if (result != null && result.MT_CrearMultasTransito_res != null && "E".Equals(result.MT_CrearMultasTransito_res.ZTYPE, StringComparison.OrdinalIgnoreCase))
                        {
                            _bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("Se registró en RIAG la infracción folio: {0} no se pudo enviar  finanzas", infraccionBusqueda.folioInfraccion), "Registro Finanzas", "WS", user);
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
                        }
                        else
                        {
                            _bitacoraServices.BitacoraWS("Finaliza ws CrearMultasTransito", CodigosWs.C4010, new { idInfraccion = idInfraccion, message = "Infraccion Guardada, no enviada" });
                            return Json(new { success = false, message = "Infraccion Guardada, no enviada" });
                        }
                    }
                }
                catch (SqlException ex)
                {
                    return Json(new { success = false, message = "Ha ocurrido un error intenta más tarde" });
                }
            }


            if (Corp > 3)
            {
                _bitacoraServices.BitacoraWS("Finaliza ws CrearMultasTransito", CodigosWs.C4010, new { idInfraccion = idInfraccion, message = "Infraccion Guardada, Municipio" });
                return Json(new { success = true, message = "Infracción guardada, no enviada a finanzas", id = idInfraccion });
            }

            _bitacoraServices.BitacoraWS("Finaliza ws CrearMultasTransito", CodigosWs.C4010, new { idInfraccion = idInfraccion, message = "Infraccion Guardada, no enviada a finanzas" });
            return Json(new { success = false, message = "Infracción guardada, no enviada a finanzas", id = idInfraccion });

        }

        public ActionResult ModalAgregarConductor(int x)
        {
            HttpContext.Session.SetInt32("conduc", x);
            BusquedaPersonaModel model = new BusquedaPersonaModel();
            return View("_ModalBusquedaPersonas", model);
        }


        [HttpPost]
        public ActionResult ajax_BuscarVehiculos(VehiculoBusquedaModel model)
        {
            var vehiculosModel = _vehiculosService.GetVehiculos(model);
            return PartialView("_ListVehiculos", vehiculosModel);
        }


        [HttpPost]
        public ActionResult ajax_BuscarPersonaMoral(PersonaMoralBusquedaModel PersonaMoralBusquedaModel)
        {
            PersonaMoralBusquedaModel.IdTipoPersona = (int)TipoPersona.Moral;
            var personasMoralesModel = _personasService.GetAllPersonasMorales(PersonaMoralBusquedaModel);
            return PartialView("_ListPersonasMorales", personasMoralesModel);
        }

        [HttpPost]
        public ActionResult ajax_BuscarPersonasFiscas()
        {
            var personasFisicas = _personasService.GetAllPersonas();
            return PartialView("_PersonasFisicas", personasFisicas);
        }


        [HttpPost]
        public ActionResult ajax_CrearPersonaMoral(PersonaModel Persona)
        {
            Persona.idCatTipoPersona = (int)TipoPersona.Moral;
            Persona.PersonaDireccion.telefono = System.String.IsNullOrEmpty(Persona.telefono) ? null : Persona.telefono;
            var IdPersonaMoral = _personasService.CreatePersonaMoral(Persona);
            if (IdPersonaMoral == 0)
                return Json(new { success = false, message = "Ocurrió un error al procesar su solicitud." });
            else
            {
                var modelList = _personasService.ObterPersonaPorIDList(IdPersonaMoral);
                return PartialView("_ListPersonasMorales", modelList);
            }


            //var personasMoralesModel = _personasService.GetAllPersonasMorales();

        }

        [HttpGet]
        public IActionResult ajax_ModalCrearPersona()
        {
            var catTipoPersona = _catDictionary.GetCatalog("CatTipoPersona", "0");
            var catTipoLicencia = _catDictionary.GetCatalog("CatTipoLicencia", "0");
            var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            var catGeneros = _catDictionary.GetCatalog("CatGeneros", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");

            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            ViewBag.CatGeneros = new SelectList(catGeneros.CatalogList, "Id", "Text");
            ViewBag.CatEntidades = new SelectList(catEntidades.CatalogList, "Id", "Text");
            ViewBag.CatTipoPersona = new SelectList(catTipoPersona.CatalogList, "Id", "Text");
            ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
            return PartialView("_CrearPersona", new PersonaModel());
        }

        [HttpGet]
        public IActionResult ajax_ModalCrearPersonaConductor()
        {
            var conductor = HttpContext.Session.GetInt32("conduc");
            var catTipoPersona = _catDictionary.GetCatalog("CatTipoPersona", "0");
            var catTipoLicencia = _catDictionary.GetCatalog("CatTipoLicencia", "0");
            var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            var catGeneros = _catDictionary.GetCatalog("CatGeneros", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");

            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            ViewBag.CatGeneros = new SelectList(catGeneros.CatalogList, "Id", "Text");
            ViewBag.CatEntidades = new SelectList(catEntidades.CatalogList, "Id", "Text");
            ViewBag.CatTipoPersona = new SelectList(catTipoPersona.CatalogList, "Id", "Text");
            ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
            ViewBag.CatTipoConductor = conductor;
            return PartialView("_CrearPersonaConductor", new PersonaModel());
        }
        [HttpPost]
        public IActionResult ajax_CrearPersona(PersonaModel model)
        {

            if (!string.IsNullOrEmpty(model.numeroLicenciaFisico)) model.numeroLicencia = model.numeroLicenciaFisico;
            if (model.idTipoLicenciaInfraccion != null) model.idTipoLicencia = model.idTipoLicenciaInfraccion;
            if (!string.IsNullOrEmpty(model.telefonoInfraccion))
            {
                model.PersonaDireccion.telefono = model.telefonoInfraccion;
            }
            if (!string.IsNullOrEmpty(model.correoInfraccion)) model.PersonaDireccion.correo = model.correoInfraccion;
            if (model.vigenciaLicenciaFisico != null)
            {
                if (Convert.ToDateTime(model.vigenciaLicenciaFisico).Year > 2000)
                    model.vigenciaLicencia = model.vigenciaLicenciaFisico;
                else
                    model.vigenciaLicencia = null;
            }
            //model.idCatTipoPersona = (int)TipoPersona.Fisica;

            //validacion
            model.PersonaDireccion.telefono = CleanNullString(model.PersonaDireccion.telefono);
            model.PersonaDireccion.correo = CleanNullString(model.PersonaDireccion.correo);
            model.PersonaDireccion.numero = CleanNullString(model.PersonaDireccion.numero);
            model.PersonaDireccion.calle = CleanNullString(model.PersonaDireccion.calle);
            model.PersonaDireccion.colonia = CleanNullString(model.PersonaDireccion.colonia);

            int id = _personasService.CreatePersona(model);
            HttpContext.Session.Remove("IdInfra");

            if (id == -1)
            {
                // El registro ya existe, muestra un mensaje de error al usuario
                return Json(new { success = false, message = "El registro ya existe, revise los datos ingresados." });
            }
            else if (id == 0)
                return Json(new { success = false, message = "Ocurrió un error al procesar su solicitud." });
            else
            {
                // La inserción se realizó correctamente
                model.PersonaDireccion.idPersona = id;

                // NO APLICA YA QUE PREVIAMENTE SE HABIA INSERTADO.
                //int idDireccion = _personasService.CreatePersonaDireccion(model.PersonaDireccion);


                // var modelList = _personasService.GetPersonaByIdInfraccion(id, 0);
                var modelList = _personasService.GetPersonaById(id);
                ViewBag.EditarVehiculo = true;
                return Json(modelList);
            }
        }


        [HttpPost]
        public ActionResult ajax_CrearPersonaFisica(PersonaModel Persona)
        {
            Persona.nombre = Persona.nombreFisico;
            Persona.apellidoMaterno = Persona.apellidoMaternoFisico;
            Persona.apellidoPaterno = Persona.apellidoPaternoFisico;
            Persona.CURP = Persona.CURPFisico;
            Persona.RFC = Persona.RFCFisico;
            Persona.numeroLicencia = Persona.numeroLicenciaFisico;

            Persona.PersonaDireccion.idEntidad = Persona.PersonaDireccion.idEntidadFisico;
            Persona.PersonaDireccion.idMunicipio = Persona.PersonaDireccion.idMunicipioFisico;
            Persona.PersonaDireccion.correo = Persona.PersonaDireccion.correoFisico;
            Persona.PersonaDireccion.telefono = Persona.PersonaDireccion.telefonoFisico;
            Persona.PersonaDireccion.colonia = Persona.PersonaDireccion.coloniaFisico;
            Persona.PersonaDireccion.calle = Persona.PersonaDireccion.calleFisico;
            Persona.PersonaDireccion.numero = Persona.PersonaDireccion.numeroFisico;
            Persona.idCatTipoPersona = (int)TipoPersona.Fisica;
            var IdPersonaFisica = _personasService.CreatePersona(Persona);
            if (IdPersonaFisica == 0)
            {
                throw new Exception("Ocurrio un error al dar de alta la persona");
            }
            var modelList = _personasService.ObterPersonaPorIDList(IdPersonaFisica); ;
            return PartialView("_PersonasFisicas", modelList);
        }
        [HttpGet]
        public ActionResult ajax_GetPersonaMoral(int id)
        {
            var personaModel = _personasService.GetPersonaTypeById(id);
            return PartialView("_UpdatePersonaMoral", personaModel);
        }

        [HttpPost]
        public ActionResult ajax_UpdatePersonaMoral(PersonaModel Persona)
        {
            Persona.idCatTipoPersona = (int)TipoPersona.Moral;
            var personaModel = _personasService.UpdatePersonaMoral(Persona);
            var personaEditada = _personasService.GetPersonaTypeById((int)Persona.idPersona);

            return Json(new { data = personaEditada });
        }

        //TODO: ejemplo crear vehiculo por service de guanajuato
        [HttpPost]
        public ActionResult ajax_CrearVehiculo3(VehiculoModel model)
        {
            var IdVehiculo = _vehiculosService.CreateVehiculo(model);

            if (IdVehiculo != 0)
            {
                var resultados = _vehiculosService.GetAllVehiculos();
                return PartialView("_ListadoVehiculos", resultados);
            }
            else
            {
                return null;
            }
        }



        public ActionResult ajax_CrearVehiculo_Ejemplo(VehiculoModel model)
        {
            int IdVehiculo = 0;
            if (model.idVehiculo > 0)
            {
                model.idSubmarca = model.idSubmarcaUpdated;
                IdVehiculo = _vehiculosService.UpdateVehiculo(model);
            }
            else if (model.idVehiculo <= 0)
            {
                IdVehiculo = _vehiculosService.CreateVehiculo(model);
            }




            var resultados = _vehiculosService.GetAllVehiculos();
            return PartialView("_ListadoVehiculos", resultados);
        }
        public ActionResult ajax_CrearVehiculo_Ejemplo2(VehiculoModel model)
        {

            model.idEdntidad2 = model.idEntidad;

            if (model.Persona.idPersona == -1)
            {

                PersonaDireccionModel direccion = new PersonaDireccionModel();
                direccion.idEntidad = 35;
                direccion.colonia = "Se ignora";
                direccion.calle = "Se ignora";
                direccion.numero = "Se ignora";
                PersonaModel persona = new PersonaModel();
                persona.nombre = "Se ignora";
                persona.idCatTipoPersona = (int)TipoPersona.Fisica;
                persona.PersonaDireccion = direccion;
                persona.idGenero = 1;
                var IdPersonaFisica = _personasService.CreatePersona(persona);
                model.idPersona = IdPersonaFisica;
                model.Persona.idPersona = IdPersonaFisica;
                model.propietario = IdPersonaFisica.ToString();
            }



            var IdVehiculo = 0;

            if (model.idVehiculo > 0)
                IdVehiculo = _vehiculosService.UpdateVehiculo2(model);
            else
                IdVehiculo = _vehiculosService.CreateVehiculo(model);


            if (IdVehiculo > 0)
            {

                var Placa = model.placas;
                var Serie = model.serie;
                var folio = "";
                var resultados = _vehiculosService.GetVehiculoById(IdVehiculo);
                return Json(new { success = true, data = resultados });
            }
            else if (IdVehiculo == -1)
            {
                return Json(new { success = false, duplicate = true });
            }
            else
            {
                return Json(new { success = false, error = "Error al guardar el vehículo." });
            }
        }

        [HttpPost]
        public ActionResult ajax_CrearVehiculo(VehiculoModel model)
        {
            int IdVehiculo = 0;
            if (model.encontradoEn == (int)EstatusBusquedaVehiculo.Sitteg)
            {
                model.idSubmarca = model.idSubmarcaUpdated;
                IdVehiculo = _vehiculosService.UpdateVehiculo(model);
            }
            else if (model.encontradoEn == (int)EstatusBusquedaVehiculo.NoEncontrado)
            {
                IdVehiculo = _vehiculosService.CreateVehiculo(model);
            }

            if (IdVehiculo != 0)
            {
                var resultados = _vehiculosService.GetAllVehiculos();
                return Json(new { id = IdVehiculo, data = resultados });
            }
            else
            {
                return null;
            }
        }


        [HttpPost]
        public IActionResult ajax_EditarConductor(PersonaModel model)
        {
            //int id = _personasService.UpdatePersona(model);
            //int idDireccion = _personasService.UpdatePersonaDireccion(model.PersonaDireccion);

            int id = _personasService.UpdateConductor(model);


            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult ajax_BuscarPersonasFiscasPagination([DataSourceRequest] DataSourceRequest request, int id = 0)
        {
            try
            {
                filterValue(request.Filters);
                Pagination pagination = new Pagination();
                pagination.PageIndex = request.Page - 1;
                pagination.PageSize = 10;
                pagination.Filter = resultValue;
                var t = request.Sorts;
                if (t.Count() > 0)
                {
                    var q = t[0];
                    pagination.Sort = q.SortDirection.ToString().Substring(0, 3);
                    pagination.SortCamp = q.Member;
                }


                var personasFisicas = _personasService.GetAllPersonasFisicasPagination(pagination);
                request.PageSize = 10;
                var total = 0;
                if (personasFisicas.Count() > 0)
                {
                    total = personasFisicas.ToList().FirstOrDefault().total;
                    personasFisicas = personasFisicas.Select(x =>
                    {
                        var newPersona = x;
                        newPersona.folioDetencion = _infraccionesService.GetPersonaFolioDetencion(x.idPersona.Value);
                        return newPersona;
                    });
                }

                var result = new DataSourceResult()
                {
                    Data = personasFisicas,
                    Total = total
                };

                return Json(result);
            }
            catch
            {
                return null;
            }
        }
        #region Budqueda
        /************************************************************************************************/
        //BusquedaEspecial

        public IActionResult BusquedaEspecial()
        {

            var t = User.FindFirst(CustomClaims.Nombre).Value;

            return View("BusquedaEspecial");
        }



        public JsonResult Overview_GetTerritories()
        {


            var Options = new List<CatalogModel>();


            Options.Add(new CatalogModel { value = "1", text = "  En proceso" });
            Options.Add(new CatalogModel { value = "2", text = "Capturada" });
            Options.Add(new CatalogModel { value = "3", text = "Pagada" });
            Options.Add(new CatalogModel { value = "4", text = "Pagada con descuento" });
            Options.Add(new CatalogModel { value = "5", text = "Solventada" });
            Options.Add(new CatalogModel { value = "6", text = "Pagada con recargo" });
            Options.Add(new CatalogModel { value = "7", text = "Enviada" });



            return Json(Options);

        }

        public IActionResult GetDataBusquedaEspecialBit(string id)
        {

            var nombre = HttpContext.Session.GetString("Nombre");
            var result = _bitacoraServices.getBitacoraData(id, nombre);




            return Json(result);

        }
        public IActionResult GetDataBusquedaEspecial(InfraccionesBusquedaEspecialModel data)
        {

            return PartialView("_ListadoInfraccionesBusquedaEspecial");
        }




        public IActionResult test([DataSourceRequest] DataSourceRequest request, InfraccionesBusquedaEspecialModel model)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            string listaIdsPermitidosJson = HttpContext.Session.GetString("Autorizaciones");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);

            Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            pagination.PageSize = (request.PageSize != 0) ? request.PageSize : 10;
            pagination.Filter = resultValue;

            var listReporteAsignacion = _infraccionesService.GetAllInfraccionesBusquedaEspecialPagination(model, idOficina, idDependencia, pagination);
            var total = 0;
            if (listReporteAsignacion.Count() > 0)
                total = listReporteAsignacion.ToList().FirstOrDefault().Total;

            request.PageSize = pagination.PageSize;
            var result = new DataSourceResult()
            {
                Data = listReporteAsignacion,
                Total = total
            };

            return Json(result);

        }


        public IActionResult Mostrar(string id)
        {

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            int ids = Convert.ToInt32(id);

            int count = ("MONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\nMONOETILENGLICOL G F (GRANEL) MONOETILENGLICOL G F\r\n(GRANEL) MONOETILENGLICOL G F (GRANEL)\r\n").Length;
            var model = _infraccionesService.GetInfraccion2ByIdMostrar(ids, idDependencia);



            return View(model);
        }

        public IActionResult ComplementoMostrar(int id)
        {
            ViewBag.EsSoloLectura = true;

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var model = _infraccionesService.GetInfraccion2ByIdMostrar(id, idDependencia);
            return View(model);
        }




        public IActionResult RemoveData(string id)
        {

            var model = _infraccionesService.CancelTramite(id);

            var idinf = Convert.ToInt32(id);
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);


            _bitacoraServices.insertBitacora(idinf, ip, string.Format("Se elimina la infraccion con ID:{0} ", id), "Eliminar Infraccion", "Delete", user);

            return Json(true);
        }
        public IActionResult ModalEditarCortesia(int idInfraccion)
        {

            var viewCortesiaModel = new EditarCortesiaModel
            {
                idInfraccion = idInfraccion,
            };

            return PartialView("_ModalCambiarCortesia", viewCortesiaModel);
        }
        public ActionResult UpdateCortesia(int idInfraccion, int cortesiaInt, string ObsevacionesApl)
        {


            var cambioCortesia = _infraccionesService.ActualizarEstatusCortesia(idInfraccion, cortesiaInt, ObsevacionesApl);
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("Se Cambia la Cortesia {0}", (cortesiaInt == 3 ? "a no aplicada" : cortesiaInt == 4 ? "a Aplicada" : "")), "Crear 2da parte", "create 2 infraccion", user);



            if (cortesiaInt == 4)
            {
                ServiceCrearInfraccion(idInfraccion);
            }

            return Json(cambioCortesia);
        }

        /*****************************************************************************************************/
        #endregion

        public JsonResult Entidades_Drop()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catEntidadesService.ObtenerEntidades(corp), "idEntidad", "nombreEntidad");
            return Json(result);
        }
        public JsonResult Municipios_Drop(int entidadDDlValue)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catMunicipiosService.GetMunicipiosPorEntidad(entidadDDlValue, corp), "IdMunicipio", "Municipio");
            return Json(result);
        }


        public ActionResult UpdateFolio(string id, string folio)
        {

            var t = _infraccionesService.UpdateFolio(id, folio);

            return Json(true);
        }

        public ActionResult ModalEliminarMotivo(int idMotivoInfraccion, int idInfraccion, string Nombre)
        {
            ViewBag.idMotivoInfraccion = idMotivoInfraccion;
            ViewBag.idInfraccion = idInfraccion;
            ViewBag.Nombre = Nombre;

            return PartialView("_ModalEliminarMotivo");
        }
        [HttpPost]
        public IActionResult ajax_EliminarMotivo(int idMotivoInfraccion, int idInfraccion)
        {

            var calificacion = _infraccionesService.GetMotivosCal(idMotivoInfraccion);
            var MotivoEliminar = _infraccionesService.EliminarMotivoInfraccion(idMotivoInfraccion);
            var fecha = _infraccionesService.GetDateInfraccion(idInfraccion);
            var umas = _infraccionesService.GetUmas(fecha);

            ViewBag.Umas = umas;
            var datosGrid = _infraccionesService.GetMotivosInfraccionByIdInfraccion(idInfraccion);

            ViewBag.Totales = datosGrid.Sum(s => s.calificacion) * umas;
            ViewBag.totalUMAS = datosGrid.Sum(s => s.calificacion);
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            _bitacoraServices.insertBitacora(idInfraccion, ip, string.Format("Se Elimina motivo de infraccion con calificacion {0} con calificaciontotal de {1}, con una Uma de {2} y un total de {3}", calificacion, ViewBag.totalUMAS, ViewBag.Umas, ViewBag.Totales), "Agregar motivos", "insert", user, new { idMotivoInfraccion = idMotivoInfraccion });


            return Json(new { DatosGrid = datosGrid, Totales = ViewBag.Totales });
        }
        public ActionResult MostrarInfraccion(bool modoSoloLectura, int Id)
        {


            return RedirectToAction("Editar", new { modoSoloLectura = true, idInfraccion = Id });
        }


        public ActionResult GetCatalogTramoFilter(FilterCatalogTramoModel filter)
        {

            if (filter.idCarretera == null || filter.idCarretera == 0)
            {
                return Json(new List<SystemCatalogListModel>());
            }


            var dat = _infraccionesService.GetFilterCatalog(filter);

            //dat.Add(new SystemCatalogListModel() { Id = 1, Text = "No aplica" });
            //dat.Add(new SystemCatalogListModel() { Id = 2, Text = "No especificado" });



            return Json(dat);

        }


        public DateTime getFechaVencimiento(DateTime fechaInfraccion, int idOficina)
        {
            int contador = 0;
            DateTime fechavigencia = fechaInfraccion;
            while (contador < 10)
            {
                fechavigencia = fechaInfraccion.AddDays(1);
                Console.WriteLine(fechavigencia.ToString("dddd"));
                if (fechavigencia.DayOfWeek.ToString() == "Saturday" || fechavigencia.DayOfWeek.ToString() == "Sabado" || fechavigencia.DayOfWeek.ToString() == "Sábado" || fechavigencia.DayOfWeek.ToString() == "Sunday" || fechavigencia.DayOfWeek.ToString() == "Domingo")

                    Console.WriteLine(fechavigencia.ToString("dddd"));
                else
                {
                    if (_infraccionesService.GetDiaFestivo(idOficina, fechavigencia) == 0)
                        contador++;
                }
                //else { 
                //contador++;
                //}
                fechaInfraccion = fechavigencia;
            }


            return fechaInfraccion;
        }

        [HttpGet]
        public async Task<IActionResult> FolioEmergenciaObligatorio()
        {
            return Json(FolioObligatorio());
        }

        public async Task<IActionResult> GuardarBoletaFisica(IFormFile boletaInfraccion, int idInfraccion)
        {

            if (boletaInfraccion != null && boletaInfraccion.Length > 0)
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await boletaInfraccion.CopyToAsync(memoryStream);
                        var fileData = memoryStream.ToArray();

                        var fileName = $"{idInfraccion}_{boletaInfraccion.FileName}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(boletaInfraccion.FileName)}";
                        var filePath = Path.Combine(_rutaArchivo, fileName);

                        System.IO.File.WriteAllBytes(filePath, fileData);

                        var datosBoleta = new InfraccionesModel
                        {
                            boletaFisicaPath = filePath,
                            nombreBoletaStr = fileName
                        };

                        _infraccionesService.AnexarBoletaFisica(datosBoleta, idInfraccion);

                        return Json(new { success = true, message = "Archivo guardado correctamente." });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error al guardar el archivo: {ex.Message}" });
                }
            }

            return Json(new { success = false, message = "No se ha seleccionado ningún archivo." });
        }

        public IActionResult ObtenerBoletaFisica(int idInfraccion)
        {

            var path = _infraccionesService.GetBoletaFisicaPath(idInfraccion);

            byte[] data = new byte[] { };

            if (System.IO.File.Exists(path.value) && !string.IsNullOrEmpty(path.text))
            {
                Console.WriteLine("La condición se cumple");

                data = System.IO.File.ReadAllBytes(path.value);
                MemoryStream stream = new MemoryStream(data);

                var contentType = "application/octet-stream";
                var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                if (provider.TryGetContentType(path.value, out var mimeType))
                {
                    contentType = mimeType;
                }



                string b64 = Convert.ToBase64String(stream.ToArray());
                return Json(new { file = b64, app = contentType, name = path.text });
            }
            else
            {
                Console.WriteLine("NO SE cumple");

                return Json(new { file = "" });
            }
        }

        public IActionResult GetAlertasInfraccion(int idInfraccion, int idOficina, int idAplicacion, int idPersonaAplicacion)
        {
            int corp = User.FindFirst(CustomClaims.TipoOficina).Value.toInt();

            try
            {
                int counter = _infraccionesService.GetAlertasInfraccion(idInfraccion, corp, idAplicacion, idPersonaAplicacion);
                return Json(new { success = true, counter = counter });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, counter = 0 });
            }



        }
        private CampoValidoDto FolioObligatorio()
        {
            var oficina = User.FindFirst(CustomClaims.Oficina).Value;
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int IdMpio = _catMunicipiosService.obtenerIdPorNombre(oficina, corp);
            var DatRequerido = _catCamposObligService.CamposPermitido(idOficina, IdMpio, null, "Folio Emergencias");
            CampoValidoDto campoValido = new()
            {
                Requerido = DatRequerido != null ? DatRequerido.Infracciones : 0
            };
            return campoValido;
        }


    }
}

