using GuanajuatoAdminUsuarios.Common;
using GuanajuatoAdminUsuarios.Exceptions;
using GuanajuatoAdminUsuarios.ExternalServices.Interfaces;
using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Interfaces.Blocs;
using GuanajuatoAdminUsuarios.Interfaces.Catalogos;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Models.Catalogos.Turnos;
using GuanajuatoAdminUsuarios.Models.CatCampos;
using GuanajuatoAdminUsuarios.Models.Files;
using GuanajuatoAdminUsuarios.RESTModels;
using GuanajuatoAdminUsuarios.Services.Blocs;
using GuanajuatoAdminUsuarios.Util;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;

namespace GuanajuatoAdminUsuarios.Controllers
{

	[Authorize]
    public class CapturaAccidentesController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICatMunicipiosService _catMunicipiosService;
        private readonly ICatCarreterasService _catCarreterasService;
        private readonly ICatTramosService _catTramosService;
        private readonly ICapturaAccidentesService _capturaAccidentesService;
        private readonly ICatClasificacionAccidentes _clasificacionAccidentesService;
        private readonly ICatFactoresAccidentesService _catFactoresAccidentesService;
        private readonly ICatFactoresOpcionesAccidentesService _catFactoresOpcionesAccidentesService;
        private readonly ICatCausasAccidentesService _catCausasAccidentesService;
        private readonly ITiposCarga _tiposCargaService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;
        private readonly IPensionesService _pensionesService;
        private readonly ICatFormasTrasladoService _catFormasTrasladoService;
        private readonly ICatTipoInvolucradoService _catTipoInvolucradoService;
        private readonly ICatEstadoVictimaService _catEstadoVictimaService;
        private readonly ICatHospitalesService _catHospitalesService;
        private readonly ICatInstitucionesTrasladoService _catInstitucionesTraslado;
        private readonly ICatAsientoService _catAsientoservice;
        private readonly ICatCinturon _catCinturon;
        private readonly ICatAutoridadesDisposicionService _catAutoridadesDisposicionservice;
        private readonly ICatAutoridadesEntregaService _catAutoridadesEntregaService;
        private readonly IOficiales _oficialesService;
        private readonly ICatCiudadesService _catCiudadesService;
        private readonly ICatAgenciasMinisterioService _catAgenciasMinisterioService;
        private readonly ICatDictionary _catDictionary;
        private readonly IInfraccionesService _infraccionesService;
        private readonly ICotejarDocumentosClientService _cotejarDocumentosClientService;
        private readonly IPersonasService _personasService;
        private readonly IVehiculosService _vehiculosService;
        private readonly AppSettings _appSettings;
        private readonly ICatEntidadesService _catEntidadesService;
        private readonly IColores _coloresService;
        private readonly ICatMarcasVehiculosService _catMarcasVehiculosService;
        private readonly ICatSubmarcasVehiculosService _catSubmarcasVehiculosService;
        private readonly IRepuveService _repuveService;
        private readonly IBitacoraService _bitacoraServices;
        private readonly ICatSubtipoServicio _subtipoServicio;
        private readonly IVehiculoPlataformaService _vehiculoPlataformaService;
        private readonly ICatCamposObligService _catCamposObligService;
        private readonly ICadService _cadService;
        private readonly ICatTurnosService _turnosService;
        private readonly ICatAseguradorasService _aseguradorasService;
		private readonly IBlockPermisoAccidentes _canBlock;
		private readonly IBlocsService _Block;

		private int idOficina = 0;
        private int lastInsertedId = 0;
        private int idVehiculoInsertado = 0;
        private string resultValue = string.Empty;
        private bool placaEncontrada = false;
        private List<CapturaAccidentesModel> capturaAccidenteModel = new List<CapturaAccidentesModel>();
        private static CapturaAccidentesModel perModel = new CapturaAccidentesModel();
        private readonly string _rutaArchivo;
		bool canPermiso;
        public CapturaAccidentesController(
            ICapturaAccidentesService capturaAccidentesService,
            ICatMunicipiosService catMunicipiosService,
            ICatCarreterasService catCarreterasService,
            ICatTramosService catTramosService,
            ICatClasificacionAccidentes catClasificacionAccidentesService,
            ICatFactoresAccidentesService catFactoresAccidentesService,
            ICatFactoresOpcionesAccidentesService catFactoresOpcionesAccidentesService,
            ICatCausasAccidentesService catCausasAccidentesService,
            ITiposCarga tiposCargaService,
            ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService,
            IPensionesService pensionesService,
            ICatFormasTrasladoService catFormasTrasladoService,
            ICatTipoInvolucradoService catTipoInvolucradoService,
            ICatEstadoVictimaService catEstadoVictimaService,
            ICatHospitalesService catHospitalesService,
            ICatInstitucionesTrasladoService catIsntitucionesTraslado,
            ICatAsientoService catAsientoservice,
            ICatCinturon catCinturon,
            ICatAutoridadesDisposicionService catAutoridadesDisposicionservice,
            ICatAutoridadesEntregaService catAutoridadesEntregaService,
            IOficiales oficialesService,
            ICatCiudadesService catCiudadesService,
            ICatAgenciasMinisterioService catAgenciasMinisterioService,
            ICatDictionary catDictionary,
            IInfraccionesService infraccionesService,
            IHttpClientFactory httpClientFactory,
            ICotejarDocumentosClientService cotejarDocumentosClientService,
            IPersonasService personasService,
            IVehiculosService vehiculosService,
            IOptions<AppSettings> appSettings,
            ICatEntidadesService catEntidadesService,
            IColores coloresService,
            ICatMarcasVehiculosService catMarcasVehiculosService,
            ICatSubmarcasVehiculosService catSubmarcasVehiculosService,
            IRepuveService repuveService,
            IBitacoraService bitacoraService,
            ICatSubtipoServicio subtipoServicio,
            IVehiculoPlataformaService vehiculoPlataformaService,
            ICatCamposObligService catCamposObligService,
            ICadService cadService,
            ICatTurnosService turnosService,
            ICatAseguradorasService aseguradorasService,
            IConfiguration configuration,
            IBlockPermisoAccidentes block,
            IBlocsService servicebloc


            )
        {
            _capturaAccidentesService = capturaAccidentesService;
            _catMunicipiosService = catMunicipiosService;
            _catCarreterasService = catCarreterasService;
            _catTramosService = catTramosService;
            _clasificacionAccidentesService = catClasificacionAccidentesService;
            _catFactoresAccidentesService = catFactoresAccidentesService;
            _catFactoresOpcionesAccidentesService = catFactoresOpcionesAccidentesService;
            _catCausasAccidentesService = catCausasAccidentesService;
            _tiposCargaService = tiposCargaService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
            _pensionesService = pensionesService;
            _catFormasTrasladoService = catFormasTrasladoService;
            _catTipoInvolucradoService = catTipoInvolucradoService;
            _catEstadoVictimaService = catEstadoVictimaService;
            _catHospitalesService = catHospitalesService;
            _catAsientoservice = catAsientoservice;
            _catInstitucionesTraslado = catIsntitucionesTraslado;
            _catCinturon = catCinturon;
            _catAutoridadesDisposicionservice = catAutoridadesDisposicionservice;
            _catAutoridadesEntregaService = catAutoridadesEntregaService;
            _oficialesService = oficialesService;
            _catCiudadesService = catCiudadesService;
            _catAgenciasMinisterioService = catAgenciasMinisterioService;
            _catDictionary = catDictionary;
            _infraccionesService = infraccionesService;
            _httpClientFactory = httpClientFactory;
            _cotejarDocumentosClientService = cotejarDocumentosClientService;
            _personasService = personasService;
            _vehiculosService = vehiculosService;
            _appSettings = appSettings.Value;
            _catEntidadesService = catEntidadesService;
            _coloresService = coloresService;
            _catMarcasVehiculosService = catMarcasVehiculosService;
            _catSubmarcasVehiculosService = catSubmarcasVehiculosService;
            _repuveService = repuveService;
            _bitacoraServices = bitacoraService;
            _subtipoServicio = subtipoServicio;
            _vehiculoPlataformaService = vehiculoPlataformaService;
            _catCamposObligService = catCamposObligService;
            _cadService = cadService;
            _turnosService = turnosService;
            _aseguradorasService = aseguradorasService;
            _rutaArchivo = configuration.GetValue<string>("AppSettings:RutaArchivosAccidente");
            _canBlock = block;
            canPermiso = _canBlock.getdate();
            _Block = servicebloc;

        }
        /// <summary>
        /// //PRIMERA SECCION DE CAPTURA ACCIDENTE//////////
        /// </summary>
        public IActionResult BuscarAccidentesLista([DataSourceRequest] DataSourceRequest request)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var ListAccidentesModel = _capturaAccidentesService.ObtenerAccidentes(idOficina);
            return Json(ListAccidentesModel.ToDataSourceResult(request));
        }


        public IActionResult BuscarAccidentesListaPagination([DataSourceRequest] DataSourceRequest request)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

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

            var ListAccidentesModel = _capturaAccidentesService.ObtenerAccidentesPagination(idOficina, pagination);
            request.PageSize = 10;
            var total = 0;
            if (ListAccidentesModel.Count() > 0)
                total = ListAccidentesModel.ToList().FirstOrDefault().Total;

            var result = new DataSourceResult()
            {
                Data = ListAccidentesModel,
                Total = total
            };

            return Json(result);
        }

        public IActionResult Index(CapturaAccidentesModel capturaAccidentesService, [DataSourceRequest] DataSourceRequest request)
        {
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Captura Accidente"), "");

            //filterValue(request.Filters);
            HttpContext.Session.Remove("isEditAccidente");
            Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            pagination.PageSize = 1;
            pagination.Filter = resultValue;
            HttpContext.Session.Remove("IdMarcaVehiculo");

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var ListAccidentesModel = _capturaAccidentesService.ObtenerAccidentesPagination(idOficina, pagination);
            if (ListAccidentesModel.Count == 0)
            {
                ViewBag.canBlock = canPermiso;

                return View("AgregarAccidente");

            }
            else
            {
                return View("CapturaAccidentes", ListAccidentesModel);
            }
        }


        private void filterValue(IEnumerable<IFilterDescriptor> filters)
        {
            if (filters == null)
                return;

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

        public ActionResult NuevoAccidente()
        {

            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Agregar Accidente"), "");
            ViewBag.canBlock = canPermiso;

            return View("AgregarAccidente");
        }

        public ActionResult AgregarAccidente()
        {
            ViewBag.canBlock = canPermiso;


            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Agregar Accidente"), "");

            return View("AgregarAccidente");
        }


        #region Dropdown sources

        public JsonResult Entidades_Drop()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catEntidadesService.ObtenerEntidades(corp), "idEntidad", "nombreEntidad");
            return Json(result);
        }
        public JsonResult Municipios_Drop()
        {
            var corp = 1;

            var result = new SelectList(_catMunicipiosService.GetMunicipios(corp), "IdMunicipio", "Municipio");
            return Json(result);
        }
        public JsonResult Municipios_Drop2()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catMunicipiosService.GetMunicipios2(corp), "IdMunicipio", "Municipio");
            return Json(result);
        }

        public JsonResult Municipios_Drop3()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catMunicipiosService.GetMunicipios3(corp), "IdMunicipio", "Municipio");
            return Json(result);
        }



        public JsonResult Municipios_Por_Delegacion_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catMunicipiosService.GetMunicipiosPorDelegacion(idOficina, corp), "IdMunicipio", "Municipio");
            return Json(result);
        }

        public JsonResult Municipios_Por_Delegacion_DropActivos()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catMunicipiosService.GetMunicipiosPorDelegacionActivos(idOficina, corp), "IdMunicipio", "Municipio");
            return Json(result);
        }

        public JsonResult Municipios_Por_Delegacion_DropTodos()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catMunicipiosService.GetMunicipiosPorDelegacionTodos(idOficina, corp), "IdMunicipio", "Municipio");
            return Json(result);
        }

        public JsonResult CarreterasPorDelegacion()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_catCarreterasService.GetCarreterasPorDelegacion(idOficina), "IdCarretera", "Carretera");
            return Json(result);
        }
        public JsonResult Carreteras_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_catCarreterasService.GetCarreterasPorDelegacion(idOficina), "IdCarretera", "Carretera");
            return Json(result);
        }

        public JsonResult Carreteras_DropTodos()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_catCarreterasService.GetCarreterasPorDelegacionTodos(idOficina), "IdCarretera", "Carretera");
            return Json(result);
        }

        public JsonResult Tramos_Drop(int carreteraDDValue)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var tramos = _catTramosService.ObtenerTamosPorCarretera(carreteraDDValue, corp);

            var result = new List<SelectListItem>();

            result.AddRange(new SelectList(tramos, "IdTramo", "Tramo"));

            result.Add(new SelectListItem { Value = "1", Text = "No aplica" });
            result.Add(new SelectListItem { Value = "2", Text = "No especificado" });

            return Json(result);
        }

        public JsonResult Tramos_DropActivos(int carreteraDDValue)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var tramos = _catTramosService.ObtenerTamosPorCarreteraActivos(carreteraDDValue, corp);

            var result = new List<SelectListItem>();

            result.AddRange(new SelectList(tramos, "IdTramo", "Tramo"));

            return Json(result);
        }


        public JsonResult TramosTodos_Drop(int carreteraDDValue)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catTramosService.ObtenerTramos(corp), "IdTramo", "Tramo");
            return Json(result);
        }

        public JsonResult Municipios_Por_Entidad(int entidadDDlValue)
        {
            var corp = 1;// HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catMunicipiosService.GetMunicipiosPorEntidad(entidadDDlValue, corp), "IdMunicipio", "Municipio");
            return Json(result);
        }
        public JsonResult Clasificacion_Drop()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_clasificacionAccidentesService.ObtenerClasificacionesActivas(corp), "IdClasificacionAccidente", "NombreClasificacion");
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

        #endregion Dropdown sources

        [HttpPost]
        public async Task<ActionResult> GuardarUbicacionAccidente(CapturaAccidentesModel model,int foliobAccidente,int idOficial2)
        {
            var iddeleg = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value);
            var idblockfolio = 0;

            if (canPermiso)
            {
                idblockfolio = _Block.ExistFolio(foliobAccidente, iddeleg, idOficial2, 2);

                if (idblockfolio == 0)
                {
                    return Json(new { success = false, message = "Folio de bloc no existe", errorFolio = 1 });
                }

            }


            if (!string.IsNullOrEmpty(model.HoraStr))
            {
                TimeSpan hora;
                if (TimeSpan.TryParse(model.HoraStr, out hora))
                {
                    // Asignar la hora convertida al modelo
                    model.Hora = hora;
                }
                else
                {
                    ModelState.AddModelError("HoraComoString", "La hora proporcionada no es válida.");
                }
            }

            var Mpio = _catMunicipiosService.GetMunicipioByID(model.IdMunicipio.Value);
            if (model.FolioEmergencia > 0)
            {

                string message = "", httpError = "";
                bool isValid;

                if (User.FindFirst(CustomClaims.TipoOficina).Value.toInt() < 2)
                {
                    (isValid, message, httpError) = await _cadService.FolioCadAsync(model.FolioEmergencia.ToString(), "C5i" );
                }
                else
                {
                    (isValid, message, httpError) = await _cadService.FolioCadAsync(model.FolioEmergencia.ToString(), Mpio.Municipio);
                }

                
                if (!isValid)
                {
                    return Json(new { success = false, message, httpError });
                }
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Se presentaron errores al momento del guardado." });
            }
            else
            {

                var oficina = User.FindFirst(CustomClaims.Oficina).Value;
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
                string abreviaturaMunicipio = User.FindFirst(CustomClaims.AbreviaturaMunicipio).Value;
                int dependencia = Convert.ToInt32(HttpContext.Session.GetInt32("IdDependencia"));

                if (abreviaturaMunicipio.IsNullOrEmpty())
                {
                    return Json(new { success = false, message = "La delegación del usuario no tiene asociado un municipio, no se puede generar el folio de la solicitud de depósito" });

                }
                lastInsertedId = _capturaAccidentesService.GuardarParte1(model, idOficina, abreviaturaMunicipio, DateTime.Now.Year, oficina);

                if (canPermiso)
                {
                    _Block.UpdateFolio(idblockfolio, lastInsertedId, 2);
                }

                if (model.FolioEmergencia > 0)
                {
                    await _capturaAccidentesService.GuardarAccidenteEmergencia((int)model.IdMunicipio, lastInsertedId, idOficina, model.FolioEmergencia);
                }
                HttpContext.Session.SetInt32("LastInsertedId", lastInsertedId);
                await GuardarArchivoCroquis(model.archivoCroquis, lastInsertedId);
                _bitacoraServices.BitacoraAccidentes(lastInsertedId, string.Format(CodigosAccidente.C2001, lastInsertedId), model);
                return Json(new { success = true, id = lastInsertedId });

            }
        }

        public ActionResult CapturaAaccidente(bool? showE, int? lstId)
        {
            if (lstId.HasValue)
            {
                HttpContext.Session.SetInt32("LastInsertedId", lstId.Value);
            }
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var AccidenteSeleccionado = _capturaAccidentesService.ObtenerAccidentePorId(idAccidente, idOficina);

            var isedit = HttpContext.Session.GetInt32("isEditAccidente");

            ViewBag.EsEdicion = isedit.HasValue ? isedit.ToString() : "0";

            ViewBag.EsSoloLectura = showE.HasValue && showE.Value;
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Captura 2da Parte Accidente"), "");
            _bitacoraServices.BitacoraAccidentes(lstId.HasValue ? lstId.Value : 0, CodigosAccidente.C2050, AccidenteSeleccionado);
            //  _bitacoraServices.BitacoraAccidentes(lstId.HasValue ? lstId.Value : 0, CodigosAccidente.C2053, AccidenteSeleccionado);

            return View("CapturaAaccidente", AccidenteSeleccionado);
        }
        [HttpGet]
        public IActionResult GetFile(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(fileName);
            return File(fileBytes, "image/png", fileName); // Cambia el MIME type según el tipo de archivo
        }

        public ActionResult ModalAgregarVehiculo()
        {
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Agregar Vehiculo"), "");

            return PartialView("_ModalVehiculo");
        }
        public ActionResult ModalDetallesVehiculo(int IdVehiculoInvolucrado, int IdPropietarioInvolucrado)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var vehiculoInvolucrado = _capturaAccidentesService.InvolucradoSeleccionado(idAccidente, IdVehiculoInvolucrado, IdPropietarioInvolucrado);



            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2005, vehiculoInvolucrado);
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Detalle Accidente"), "");

            return PartialView("_ModalDetalleVehiculos", vehiculoInvolucrado);
        }
        public ActionResult ModalBorraRegistro(int IdVehiculoInvolucrado, int IdPropietarioInvolucrado, int IdAccidente)
        {
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Eliminar Involucrado"), "");

            return PartialView("_ModalEliminarInvolucrado");
        }
        public ActionResult ModalBorraRegistroPersona(int IdPersona, int IdAccidente, int IdInvolucrado)
        {
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Borrar Persona Involucrada"), "");

            return PartialView("_ModalEliminarPersonaInvolucrada");
        }

        public ActionResult MostrarModalConductor(int IdPersona, int IdVehiculo)
        {
            ViewBag.IdVehiculo = IdVehiculo;
            var ListConductor = _capturaAccidentesService.ObtenerConductorPorId(IdPersona);
            HttpContext.Session.SetInt32("idVehiculoInsertado", IdVehiculo);


            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Mostrar Datos Conductor"), "");


            return PartialView("_ModalConductor", ListConductor);
        }

        public ActionResult ModalClasificacionAccidente()
        {
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Mostrar Clasificacion"), "");

            return PartialView("_ModalClasificacion");
        }
        public ActionResult ModalEliminarClasificacion(int IdAccidente, int idClasif)
        {
            List<CapturaAccidentesModel> clasificacionesModel = _capturaAccidentesService.AccidentePorID(IdAccidente);

            clasificacionesModel[0].IdClasificacionAccidente = idClasif;

            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Eliminar clasificacion"), "");

            return PartialView("_ModalEliminarClasificacion");
        }
        public ActionResult ModalAnexo2()
        {
            var vehiculoEncontrado = new VehiculoModel();
            vehiculoEncontrado.idSubmarcaUpdated = vehiculoEncontrado.idSubmarca;
            vehiculoEncontrado.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculoEncontrado.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
            vehiculoEncontrado.encontradoEn = 3;
            return PartialView("_Create", vehiculoEncontrado);
        }

        public IActionResult EliminarInvolucradoAccidente(int IdVehiculoInvolucrado, int IdPropietarioInvolucrado, int IdAccidente)
        {
            var EliminarVehiculo = _capturaAccidentesService.BorrarVehiculoAccidente(IdVehiculoInvolucrado, IdAccidente);
            var involucradoEliminado = _capturaAccidentesService.EliminarInvolucradoAcc(IdVehiculoInvolucrado, IdPropietarioInvolucrado, IdAccidente);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            // _bitacoraServices.insertBitacora(involucradoEliminado, ip, "CapturaAccidente_EliminarInvolucrado", "Eliminar", "delete", user);

            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            object data = new { IdVehiculo = IdVehiculoInvolucrado, idPropietario = IdPropietarioInvolucrado, idAccidente = IdAccidente };

            _bitacoraServices.BitacoraAccidentes(IdAccidente, CodigosAccidente.C2032, data);


            var ListVehiculosInvolucrados = _capturaAccidentesService.VehiculosInvolucrados(idAccidente);

            return Json(ListVehiculosInvolucrados);

        }

        [HttpPost]
        public async Task<ActionResult> BuscarVehiculo2(string Placa, string Serie, string folio)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            if (_appSettings.AllowWebServices)
            {
                var SeleccionVehiculo = _capturaAccidentesService.BuscarPorParametro(Placa, Serie, folio, corp);

                if (SeleccionVehiculo.Count == 0)
                {
                    //string jsonPartialVehiculosByWebServices = await AbrirModalVehiculo(Placa, Serie);
                    //return Json(new { noResults = true, data = jsonPartialVehiculosByWebServices });
                }
                return Json(new { noResults = false, data = SeleccionVehiculo, porWebService = false });
            }
            else
            {
                var SeleccionVehiculo = _capturaAccidentesService.BuscarPorParametro(Placa, Serie, folio, corp);
                return Json(new { noResults = true, data = SeleccionVehiculo, porWebService = false });

            }
        }
        public JsonResult Entidades_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public async Task<ActionResult> BuscarVehiculo(VehiculoBusquedaModel model)
        {
            HttpContext.Session.Remove("NumerosEconomicos");
            HttpContext.Session.Remove("IdConcesionario");
            HttpContext.Session.Remove("IdPension");
            HttpContext.Session.Remove("IdDelegacionPension");
            HttpContext.Session.Remove("IdGruas");
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
                if (!string.IsNullOrEmpty(model.FolioBusqueda))
                {
                    model.FolioBusqueda = model.FolioBusqueda.ToUpper();
                }
                var SeleccionVehiculo = _capturaAccidentesService.BuscarPorParametro(model.PlacasBusqueda, model.SerieBusqueda, model.FolioBusqueda, corp);

                if (SeleccionVehiculo.Count > 0)
                {
                    // Almacenar valores en variables de sesión si están disponibles
                    if (!string.IsNullOrEmpty(SeleccionVehiculo[0].NumerosEconomicos))
                    {
                        HttpContext.Session.SetString("NumerosEconomicos", SeleccionVehiculo[0].NumerosEconomicos);
                    }
                    if (!string.IsNullOrEmpty(SeleccionVehiculo[0].IdGruas))
                    {
                        HttpContext.Session.SetString("IdGruas", SeleccionVehiculo[0].IdGruas);
                    }

                    if (SeleccionVehiculo[0].IdConcesionario != 0)
                    {
                        HttpContext.Session.SetInt32("IdConcesionario", SeleccionVehiculo[0].IdConcesionario);
                    }

                    if (SeleccionVehiculo[0].IdPension != 0)
                    {
                        HttpContext.Session.SetInt32("IdPension", SeleccionVehiculo[0].IdPension);
                    }
                    if (SeleccionVehiculo[0].IdDelegacionPension != 0)
                    {
                        HttpContext.Session.SetInt32("IdDelegacionPension", SeleccionVehiculo[0].IdDelegacionPension);

                    }
                    if (!string.IsNullOrEmpty(SeleccionVehiculo[0].IdGruas))
                    {
                        HttpContext.Session.SetString("idGruas", SeleccionVehiculo[0].IdGruas);

                    }
                    return Json(new { noResults = false, data = SeleccionVehiculo });
                }
                else
                {

                    var jsonPartialVehiculosByWebServices = await AbrirModalVehiculo(model);

                    if (jsonPartialVehiculosByWebServices != null)
                    {
                        return Json(new { noResults = true, data = jsonPartialVehiculosByWebServices });
                    }
                    else
                    {
                        return Json(new { noResults = true, data = new { } });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { noResults = true, error = "Se produjo un error al procesar la solicitud" });
            }
        }

        public async Task<string> AbrirModalVehiculo(VehiculoBusquedaModel model)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            try
            {
                // Se busca el vehículo en el padrón RIAG, Finanzas y REPUVE
                VehiculoModel vehiculo = await _vehiculoPlataformaService.BuscarVehiculoEnPlataformas(model, corp);

                // Verificar si el servicio está desactivado
               if (vehiculo.ServicioDesactivado)
                {
                 
                    vehiculo.Persona = new PersonaModel();
                    vehiculo.Persona.PersonaDireccion = new PersonaDireccionModel();
                    vehiculo.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
                    return await this.RenderViewAsync("_Create", vehiculo, true); 

                }

                if (vehiculo == null)
                {
                    return JsonConvert.SerializeObject(new
                    {
                        servicioDesactivado = false,
                        mensaje = "No se encontró el vehículo."
                    });
                }

                HttpContext.Session.SetInt32("IdMarcaVehiculo", vehiculo.idMarcaVehiculo);

                return await this.RenderViewAsync("_Create", vehiculo, true); 
            }
            catch (Exception ex)
            {
                VehiculoModel vehiculo = new VehiculoModel();
                Logger.Error("CapturaAccidentes - AbrirModalVehiculo: " + ex.Message);
                return await this.RenderViewAsync("_Create", vehiculo, true); 

            }
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



        public JsonResult ObtVehiculosInvol([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListVehiculosInvolucrados = _capturaAccidentesService.VehiculosInvolucrados(idAccidente);

            return Json(ListVehiculosInvolucrados.ToDataSourceResult(request));
        }

        public JsonResult ObtVehiculosInvol2([DataSourceRequest] DataSourceRequest request)
        {
            filterValue2(request.Filters);
            Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            pagination.PageSize = 10;

            pagination.Filter = resultValue;
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListVehiculosInvolucrados = _capturaAccidentesService.VehiculosInvolucradosFiltro(idAccidente, pagination);
            var total = 0;
            if (ListVehiculosInvolucrados.Count() > 0)
                total = ListVehiculosInvolucrados.Count();


            if (request.Sorts.Count() > 0)
            {
                var direction = request.Sorts[0].SortDirection;
                var member = request.Sorts[0].Member;
                var model = new CapturaAccidentesModel();
                var key = model.GetType().GetMember("OrdenVehiculo");

                if (direction == Kendo.Mvc.ListSortDirection.Descending)
                {
                    ListVehiculosInvolucrados = ListVehiculosInvolucrados.OrderByDescending(s => s.GetType().GetProperty(member).GetValue(s, null)).ToList();

                }
                else
                {
                    ListVehiculosInvolucrados = ListVehiculosInvolucrados.OrderBy(s => s.GetType().GetProperty(member).GetValue(s, null)).ToList();
                }


            }



            request.PageSize = pagination.PageSize;
            var result = new DataSourceResult()
            {
                Data = ListVehiculosInvolucrados,
                Total = total
            };


            return Json(result);
        }
        private void filterValue2(IEnumerable<IFilterDescriptor> filters)
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
        public JsonResult ObtVehiculosInvolInfracciones([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListVehiculosInvolucrados = _capturaAccidentesService.VehiculosInvolucradosParaInfracciones(idAccidente);

            return Json(ListVehiculosInvolucrados.ToDataSourceResult(request));
        }

        public IActionResult ActualizarAccidenteConVehiculo(int IdVehiculo, int IdPersona, string Placa = "", string Serie = "")
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;

            // Persistencia del vehículo seleccionado; se resetean al momento del guardado.
            HttpContext.Session.SetInt32("idPersonaInvolucrado", IdPersona);
            HttpContext.Session.SetString("placaInvolucrado", Placa);
            HttpContext.Session.SetString("serieInvolucrado", Serie);

            var can = _capturaAccidentesService.ActualizarConVehiculo(IdVehiculo, idAccidente, IdPersona, Placa, Serie, true);
            if (can == 0)
            {
                return Json(new { IdPersona = IdPersona, IdVehiculoH = IdVehiculo, error = 1 });
            }

            var idVehiculoInsertado = IdVehiculo;

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(idVehiculoInsertado, ip, "CapturaAccidente_AccidenteConVehiculo", "Actualizar", "update", user);

            object data = new { IdVehiculo = idVehiculoInsertado, idAccidente = idAccidente };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2004, data);

            HttpContext.Session.SetInt32("idVehiculoInsertado", idVehiculoInsertado);
            //return Json(IdPersona);
            return Json(new { IdPersona = IdPersona, IdVehiculoH = IdVehiculo });
        }
        public IActionResult RegresarModalVehiculo()
        {
            int IdVehiculo = HttpContext.Session.GetInt32("idVehiculoInsertado") ?? 0;
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var idVehiculoBorrado = _capturaAccidentesService.BorrarVehiculoAccidente(IdVehiculo, idAccidente);
            return Json(idVehiculoBorrado);
        }


        [HttpPost]
        public IActionResult ActualizarConConductor(int IdVehiculo, int IdPersona)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0; // Obtener el valor de lastInsertedId desde la variable de sesión

            var idVehiculoInsertado = _capturaAccidentesService.InsertarConductor(IdVehiculo, idAccidente, IdPersona);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(idVehiculoInsertado, ip, "CapturaAccidente_ConConductor", "Actualizar", "update", user);

            object data = new { idAccidente = idAccidente, IdPersona = IdPersona };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2005, data);

            return Json(idVehiculoInsertado);
        }

        [HttpPost]
        public IActionResult ActualizarSinConductor(int IdVehiculo)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0; // Obtener el valor de lastInsertedId desde la variable de sesión
            var personamodel = new PersonaModel();
            personamodel.nombre = "Se ignora";
            personamodel.idCatTipoPersona = 1;
            personamodel.PersonaDireccion = new PersonaDireccionModel();

            var IdPersona = _personasService.CreatePersona(personamodel);
            var idVehiculoInsertado = _capturaAccidentesService.InsertarConductor(IdVehiculo, idAccidente, IdPersona);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(idVehiculoInsertado, ip, "CapturaAccidente_ConConductor", "Actualizar", "update", user);

            object data = new { idAccidente = idAccidente, IdPersona = IdPersona };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2005, data);

            return Json(idVehiculoInsertado);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarInfoAccidente(DateTime Fecha, string HoraStr, int IdMunicipio, int IdCarretera, int IdTramo, long? IdTurno, string Kilometro, int? IdEmergencia, int? FolioEmergencia, string Latitud, string Longitud, IFormFile archivoCroquis,string observacionesTramo,string lugarCalle,string lugarNumero,string lugarColonia)
        {
            TimeSpan Hora = TimeSpan.Zero;
            if (!string.IsNullOrEmpty(HoraStr))
            {
                TimeSpan hora;
                if (TimeSpan.TryParse(HoraStr, out hora))
                {
                    // Asignar la hora convertida al modelo
                    Hora = hora;
                }
                else
                {
                    ModelState.AddModelError("HoraComoString", "La hora proporcionada no es válida.");
                }
            }

            if (FolioEmergencia > 0)
            {
                var Mpio = _catMunicipiosService.GetMunicipioByID(IdMunicipio);

                string message = "", httpError = "";
                bool isValid;

                if (User.FindFirst(CustomClaims.TipoOficina).Value.toInt() < 2)
                {
                    (isValid, message, httpError) = await _cadService.FolioCadAsync(FolioEmergencia.ToString(), "C5i");
                }
                else
                {
                    (isValid, message, httpError) = await _cadService.FolioCadAsync(FolioEmergencia.ToString(), Mpio.Municipio);
                }


                if (!isValid)
                {
                    Response.StatusCode = 400;
                    return Json(new { errorMessage = message });
                }
            }

            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0; // Obtener el valor de lastInsertedId desde la variable de sesión
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var idAccidenteActualizado = await _capturaAccidentesService.ActualizaInfoAccidente(idAccidente, Fecha, Hora, IdMunicipio, IdCarretera, IdTramo, IdTurno, Kilometro, idOficina, IdEmergencia, FolioEmergencia, Latitud, Longitud,observacionesTramo,lugarCalle,lugarNumero,lugarColonia);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(idVehiculoInsertado, ip, "CapturaAccidente_InfoAccidente", "Actualizar", "update", user);

            object data = new { idAccidente = idAccidente };
            await GuardarArchivoCroquis(archivoCroquis, idAccidente);
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2002, data);

            return Json(idAccidenteActualizado);
        }

        public IActionResult GuardarConductorVehiculo(int IdPersona, int idAuto)
        {
            int IdVehiculoI = HttpContext.Session.GetInt32("idVehiculoInsertado") ?? 0; // Obtener el valor de idVehiculoInsertado desde la variable de sesión
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.InsertarConductor(IdVehiculoI, idAccidente, IdPersona);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(RegistroSeleccionado, ip, "CapturaAccidente_ConductorVehiculo", "Insertar", "insert", user);

            object data = new { idAccidente = idAccidente, IdPersona = IdPersona, idVehiculo = IdVehiculoI };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2005, data);

            return Json(RegistroSeleccionado);
        }
        public ActionResult BuscarConductor(BusquedaInvolucradoModel model)
        {

            var ListInvolucradoModel = _capturaAccidentesService.BusquedaPersonaInvolucrada(model);
            return Json(ListInvolucradoModel);
        }

        [HttpPost]

        public IActionResult GuardarComplementoVehiculo(CapturaAccidentesModel model)
        {
            int IdVehiculo = HttpContext.Session.GetInt32("idVehiculoInsertado") ?? 0; // Obtener el valor de idVehiculoInsertado desde la variable de sesión
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0; // Obtener el valor de lastInsertedId desde la variable de sesión

            string placa = HttpContext.Session.GetString("placaInvolucrado") ?? "";
            string serie = HttpContext.Session.GetString("serieInvolucrado") ?? "";
            int idPersona = HttpContext.Session.GetInt32("idPersonaInvolucrado") ?? 0;

            var insert = _capturaAccidentesService.ActualizarConVehiculo(IdVehiculo, idAccidente, idPersona, placa, serie, false);

            HttpContext.Session.Remove("placaInvolucrado");
            HttpContext.Session.Remove("serieInvolucrado");
            HttpContext.Session.Remove("idPersonaInvolucrado");


            //Valores adicionales cuando fue busqueda de vehiculo pro folio G1//

            string numerosEconomicos = HttpContext.Session.GetString("NumerosEconomicos") ?? "";
            int idConcesionario = HttpContext.Session.GetInt32("IdConcesionario") ?? 0;
            string idGruas = HttpContext.Session.GetString("IdGruas") ?? "";


            var RegistroSeleccionado = _capturaAccidentesService.GuardarComplementoVehiculo(model, IdVehiculo, idAccidente, numerosEconomicos, idConcesionario, idGruas);

            //Vaciar variables de sesion despues del guardado en GuardarComplementoVehiculo//



            HttpContext.Session.Remove("NumerosEconomicos");
            HttpContext.Session.Remove("IdConcesionario");
            HttpContext.Session.Remove("IdGruas");
            HttpContext.Session.Remove("IdPension");
            HttpContext.Session.Remove("IdDelegacionPension");

            HttpContext.Session.SetInt32("idVehiculoInvolucrado", IdVehiculo);


            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(RegistroSeleccionado, ip, "CapturaAccidente_ComplementoVehiculo", "Insertar", "insert", user);

            object data = new { idAccidente = idAccidente, idVehiculo = IdVehiculo };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2064, data);


            return RedirectToAction("ObtVehiculosInvol", "CapturaAccidentes");
        }
        [HttpPost]
        public IActionResult AgregarClasificacion(int IdClasificacionAccidente)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.AgregarValorClasificacion(IdClasificacionAccidente, idAccidente);
            var datosGrid = _capturaAccidentesService.ObtenerDatosGrid(idAccidente);
            if (RegistroSeleccionado == 1)
            {
                // La inserción fue exitosa//
                //Bitacora
                object data = new { idAccidente = idAccidente, idClasificacionAccidente = IdClasificacionAccidente };
                _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2006, data);

                return Json(datosGrid);
            }

            else
            {
                return Json(new { error = "La clasificación seleccionada ya esta registrada en este accidente." });
            }
        }
        public JsonResult ObtClasificacionAccidente([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListClasificaciones = _capturaAccidentesService.ObtenerDatosGrid(idAccidente);

            return Json(ListClasificaciones.ToDataSourceResult(request));
        }
        [HttpPost]
        public IActionResult EliminaClasificacion(int IdAccidente, int IdClasificacionAccidente)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var clasificacionEliminada = _capturaAccidentesService.ClasificacionEliminar(IdAccidente, IdClasificacionAccidente);
            var datosGrid = _capturaAccidentesService.ObtenerDatosGrid(idAccidente);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(clasificacionEliminada, ip, "CapturaAccidente_Clasificacion", "Eliminar", "delete", user);

            object data = new { idAccidente = idAccidente, IdClasificacionAccidente = IdClasificacionAccidente };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2034, data);

            return Json(datosGrid);

        }
        ///////////////
        ///SEGUNDA SECCION CAPTURA ACCIDENTE///////////
        ///

        public ActionResult CapturaBAccidente(bool? esSoloLectura)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            string descripcionCausa = _capturaAccidentesService.ObtenerDescripcionCausaDesdeBD(idAccidente);
            ViewData["DescripcionCausa"] = descripcionCausa;

            var isedit = HttpContext.Session.GetInt32("isEditAccidente");

            ViewBag.EsEdicion = isedit.HasValue ? isedit.ToString() : "0";

            ViewBag.EsSoloLectura = esSoloLectura ?? false;

            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Captura  parte B accidentes"), "");

            return View("CapturaBAccidente");
        }

        public ActionResult ModalFactorAccidente()
        {

            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "factor aciidente"), "");

            return PartialView("_ModalFactor");
        }
        public ActionResult ModalEditarFactorAccidente(int IdFactorAccidente, int IdFactorOpcionAccidente, int IdAccidenteFactorOpcion)

        {
            EditarFactorOpcionModel modelo = new EditarFactorOpcionModel
            {
                IdFactorAccidente = IdFactorAccidente,
                IdFactorOpcionAccidente = IdFactorOpcionAccidente,
                IdAccidenteFactorOpcion = IdAccidenteFactorOpcion
            };

            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Editar factor"), "");

            return PartialView("_ModalEditarFactor", modelo);
        }

        public ActionResult ModalEliminarFactorAccidente(string FactorAccidente, string FactorOpcionAccidente, int IdAccidenteFactorOpcion)
        {
            ViewBag.FactorAccidente = FactorAccidente;
            ViewBag.FactorOpcionAccidente = FactorOpcionAccidente;
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Eliminar factor"), "");

            return PartialView("_ModalEliminarFactor");
        }
        public ActionResult ModalCausaAccidente()
        {
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "causa accidente"), "");

            return PartialView("_ModalCausa");
        }
        public ActionResult ModalCapturaConductor()
        {
            var catTipoPersona = _catDictionary.GetCatalog("CatTipoPersona", "0");
            var catTipoLicencia = _catDictionary.GetCatalog("CatTipoLicencia", "0");
            //var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            var catGeneros = _catDictionary.GetCatalog("CatGeneros", "0");
            //var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");

            //ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            ViewBag.CatGeneros = new SelectList(catGeneros.CatalogList, "Id", "Text");
            ///ViewBag.CatEntidades = new SelectList(catEntidades.CatalogList, "Id", "Text");
            ViewBag.CatTipoPersona = new SelectList(catTipoPersona.CatalogList, "Id", "Text");
            ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
            var isedit = HttpContext.Session.GetInt32("isEditAccidente");

            ViewBag.EsEdicion = isedit.HasValue ? isedit.ToString() : "0";

            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Captura conductor"), "");

            return PartialView("_ModalCapturarConductor");
        }
        public ActionResult ModalEditarCausaAccidente(int IdCausaAccidente, string CausaAccidente, int idAccidenteCausa)
        {
            ViewBag.IdCausaAccidente = IdCausaAccidente;
            ViewBag.CausaAccidente = CausaAccidente;
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Editar causa"), "");

            return PartialView("_ModalEditarCausa");
        }
        public ActionResult ModalEliminarCausas(int IdCausaAccidente, string CausaAccidente, int IdAccidenteCausa)
        {
            ViewBag.IdCausaAccidente = IdCausaAccidente;
            ViewBag.CausaAccidente = CausaAccidente;
            ViewBag.IdAccidenteCausa = IdAccidenteCausa;
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Eliminar causa"), "");

            return PartialView("_ModalEliminarCausa");
        }

        public ActionResult ActualizaIndiceCausaAccidente(int idAccidenteCausa, int indice, int idAccidenteCausaParent, int indiceParent)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2065, new { indice = indice, idCaudaAnterior = idAccidenteCausaParent, indiceAnterior = indiceParent });
            _capturaAccidentesService.ActualizaIndiceCuasa(idAccidenteCausa, indice);
            _capturaAccidentesService.ActualizaIndiceCuasa(idAccidenteCausaParent, indiceParent);
            return Json(new { success = true });
        }

        public ActionResult ModalAgregarInvolucrado()
        {
            var isedit = HttpContext.Session.GetInt32("isEditAccidente");

            ViewBag.EsEdicion = isedit.HasValue ? isedit.ToString() : "0";
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Involucrado"), "");
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;

            return PartialView("_ModalInvolucrado-Vehiculo");
        }

        //public ActionResult MostrarDetalle(string Id)
        //{
        //	var ListInfraccionesModel = _CortesiasNoAplicadasService.ObtenerDetalleCortesiasNoAplicada(Id);
        //	return PartialView("_DetalleCortesiasNoAplicadas", ListInfraccionesModel);

        //}


        public ActionResult ModalAgregarInvolucradoPersona(int IdPersona)
        {
            var listPersonasModel = _capturaAccidentesService.ObtenerDetallePersona(IdPersona);
            var isedit = HttpContext.Session.GetInt32("isEditAccidente");

            ViewBag.EsEdicion = isedit.HasValue ? isedit.ToString() : "0";
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Involucrado"), "");

            return PartialView("_ModalInvolucrado-Vehiculo-Persona", listPersonasModel);
        }
        public ActionResult NuevoInvolucradoPersona(int IdPersona, int idAccidente, int IdInvolucrado)
        {


            var listPersonasModel = _capturaAccidentesService.DatosInvolucradoEdicion(IdPersona, idAccidente, IdInvolucrado);
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Involucrado"), "");

            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2005, new { idpersona = IdPersona, idinvolucrado = IdInvolucrado });
            return PartialView("_ModalEditarInvolucrado", listPersonasModel);
        }

        [HttpGet]
        public IActionResult SubmodalBuscarInvolucrado()
        {
            BusquedaInvolucradoModel model = new BusquedaInvolucradoModel();
            //var ListInvolucradoModel = _personasService.GetAllPersonasFisicasPagination(pagination);
            //ViewBag.ModeInvolucrado = ListInvolucradoModel;

            return PartialView("_ModalAgregarInvolucrado");
        }

        [HttpGet]
        public IActionResult SubmodalBuscarInvolucradoPersona()
        {
            BusquedaInvolucradoModel model = new BusquedaInvolucradoModel();
            var ListInvolucradoModel = _capturaAccidentesService.BusquedaPersonaInvolucrada(model);
            ViewBag.ModeInvolucrado = ListInvolucradoModel;

            return PartialView("_ModalAgregarInvolucradoPersona");
        }


        public IActionResult AbrirModalBuscarInvolucrado()
        {
            BusquedaInvolucradoModel model = new BusquedaInvolucradoModel();



            return PartialView("BusquedaPersonaFisica");
        }
        public IActionResult SinInvolucrado()
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0; // Obtener el valor de lastInsertedId desde la variable de sesión
            var personamodel = new PersonaModel();
            personamodel.nombre = "Se ignora";
            personamodel.idCatTipoPersona = 1;
            personamodel.PersonaDireccion = new PersonaDireccionModel();

            var nuevaPersona = _personasService.CreatePersona(personamodel);
            object data = new { idAccidente = idAccidente, idPersona = nuevaPersona };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2061, data);
            personamodel.idPersona = nuevaPersona;

            return Json(personamodel);
        }

        public ActionResult ModalAgregarComplemeto()
        {
            int idPension = HttpContext.Session.GetInt32("IdPension") ?? 0;
            int idDelegacionPension = HttpContext.Session.GetInt32("IdDelegacionPension") ?? 0;
            ViewBag.deFolio = idPension != 0 ? "1" : "0";
            ViewData["IdPension"] = idPension;
            ViewData["IdDelegacionPension"] = idDelegacionPension;

            return PartialView("_ModalComplementoVehiculo");
        }
        public JsonResult Factores_Drop()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var result = new SelectList(_catFactoresAccidentesService.GetFactoresAccidentesActivos(corp), "IdFactorAccidente", "FactorAccidente");
            return Json(result);
        }

        public JsonResult FactoresOpciones_Drop(int factorDDValue)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var result = new SelectList(_catFactoresOpcionesAccidentesService.ObtenerOpcionesPorFactor(factorDDValue, corp), "IdFactorOpcionAccidente", "FactorOpcionAccidente");
            return Json(result);
        }
        public JsonResult FactoresOpciones_Drop2()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var result = new SelectList(_catFactoresOpcionesAccidentesService.ObtenerOpcionesPorCorporacion(corp), "IdFactorOpcionAccidente", "FactorOpcionAccidente");
            return Json(result);
        }

        public JsonResult Causas_Drop()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catCausasAccidentesService.ObtenerCausasActivas(corp), "IdCausaAccidente", "CausaAccidente");
            return Json(result);
        }
        [HttpPost]
        public IActionResult AgregarFactorNuevo(int IdFactorAccidente, int IdFactorOpcionAccidente, int IdAccidente)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.AgregarValorFactorYOpcion(IdFactorAccidente, IdFactorOpcionAccidente, idAccidente);

            if (RegistroSeleccionado == 2)
            {
                return Json(new { error = "Esa opción ya está registrada en el accidente." });
            }

            object data = new { idAccidente = idAccidente, idFactor = IdFactorAccidente };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2007, data);
            object data2 = new { idAccidente = idAccidente, idFactor = IdFactorAccidente, idFactorOpcion = IdFactorOpcionAccidente };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2008, data2);

            var datosGrid = _capturaAccidentesService.ObtenerDatosGridFactor(idAccidente);
            return Json(datosGrid);
        }


        public JsonResult ObtFactorOpcionAccidente([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListFactores = _capturaAccidentesService.ObtenerDatosGridFactor(idAccidente);

            return Json(ListFactores.ToDataSourceResult(request));
        }
        [HttpPost]
        public IActionResult EditarValorFactorYOpcion(int IdFactorAccidente, int IdFactorOpcionAccidente, int IdAccidenteFactorOpcion)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.EditarFactorOpcion(IdFactorAccidente, IdFactorOpcionAccidente, IdAccidenteFactorOpcion, idAccidente);

            if (RegistroSeleccionado == 2)
            {
                return Json(new { error = "Esa opción ya está registrada en el accidente." });
            }
            //BITACORA//
            object data = new { idAccidente = idAccidente, idFactor = IdFactorAccidente };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2021, data);
            object data2 = new { idAccidente = idAccidente, idFactor = IdFactorAccidente, idFactorOpcion = IdFactorOpcionAccidente };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2022, data2);
            var datosGrid = _capturaAccidentesService.ObtenerDatosGridFactor(idAccidente);
            return Json(datosGrid);
        }
        [HttpPost]
        public IActionResult EliminarValorFactorYOpcion(int IdAccidenteFactorOpcion)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.EliminarValorFactorYOpcion(IdAccidenteFactorOpcion);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(RegistroSeleccionado, ip, "CapturaAccidente_ValorFactorYOpcion", "Eliminar", "delete", user);

            object data = new { idAccidente = idAccidente, IdAccidenteFactorOpcion = IdAccidenteFactorOpcion };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2035, data);

            var datosGrid = _capturaAccidentesService.ObtenerDatosGridFactor(idAccidente);

            return Json(datosGrid);
        }

        public IActionResult AgregarCausaNuevo(int IdCausaAccidente)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.AgregarValorCausa(IdCausaAccidente, idAccidente);
            if (RegistroSeleccionado == 2)
            {
                return Json(new { error = "Esa causa ya está registrada en el accidente." });
            }
            var datosGrid = _capturaAccidentesService.ObtenerDatosGridCausa(idAccidente);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(RegistroSeleccionado, ip, "CapturaAccidente_CausaNuevo", "Insertar", "insert", user);

            object data = new { idAccidente = idAccidente, IdCausaAccidente = IdCausaAccidente };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2009, data);

            return Json(datosGrid);
        }
        public IActionResult EditarCausa(int IdCausaAccidente, int idAccidenteCausa)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.EditarValorCausa(IdCausaAccidente, idAccidenteCausa);
            if (RegistroSeleccionado == 2)
            {
                return Json(new { error = "Esa causa ya está registrada en el accidente." });
            }
            var datosGrid = _capturaAccidentesService.ObtenerDatosGridCausa(idAccidente);

            object data = new { idAccidente = idAccidente, IdCausaAccidente = IdCausaAccidente };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2023, data);

            return Json(datosGrid);
        }
        public IActionResult EliminarCausaAccidente(int IdCausaAccidente, int idAccidenteCausa)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.EliminarCausaBD(IdCausaAccidente, idAccidente, idAccidenteCausa);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(RegistroSeleccionado, ip, "CapturaAccidente_CausaAccidente", "Eliminar", "delete", user);
            object data = new { idAccidente = idAccidente, IdCausaAccidente = IdCausaAccidente };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2037, data);

            _capturaAccidentesService.RecalcularIndex(idAccidente);
            var datosGrid = _capturaAccidentesService.ObtenerDatosGridCausa(idAccidente);

            return Json(datosGrid);
        }
        public JsonResult ObtCausasAccidente([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListCausas = _capturaAccidentesService.ObtenerDatosGridCausa(idAccidente);

            return Json(ListCausas.ToDataSourceResult(request));
        }
        [HttpGet]
        public ActionResult BuscarInvolucrado(BusquedaInvolucradoModel model)
        {
            var ListInvolucradoModel = _capturaAccidentesService.BusquedaPersonaInvolucrada(model);
            return Json(ListInvolucradoModel);
        }
        public IActionResult GuardarInvolucrado(int idPersonaInvolucrado)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.AgregarPersonaInvolucrada(idPersonaInvolucrado, idAccidente);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(RegistroSeleccionado, ip, "CapturaAccidente_PersonaInvolucrada", "Insertar", "insert", user);
            object data = new { idAccidente = idAccidente, Idpersona = idPersonaInvolucrado };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2005, data);

            return PartialView("_ModalConductor");
        }
        public JsonResult ObtenerVehiculosInvolucrados([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListVehiculosInvolucrados = _capturaAccidentesService.VehiculosInvolucrados(idAccidente);

            return Json(ListVehiculosInvolucrados.ToDataSourceResult(request));
        }
        public IActionResult SeleccionInvolucrado(int idPersonaInvolucrado, string nombre)
        {
            ViewBag.nombre = nombre;
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Agregar Involucrado"), "");

            return RedirectToAction("ModalAgregarInvolucrado");
        }
        public JsonResult Carga_Drop()
        {
            var result = new SelectList(_tiposCargaService.GetTiposCarga(), "IdTipoCarga", "TipoCarga");
            return Json(result);
        }
        public JsonResult Delegaciones_Drop()
        {
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos(), "IdDelegacion", "Delegacion");
            return Json(result);
        }

        public JsonResult Delegaciones_Drop2()
        {
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos().Where(X => X.Transito == 1), "IdDelegacion", "Delegacion");
            return Json(result);
        }

        public JsonResult Pensiones_Drop(int delegacionDDValue)
        {
            var result = new SelectList(_pensionesService.GetPensionesByDelegacion(delegacionDDValue), "IdPension", "Pension");
            return Json(result);
        }
        public JsonResult PensionesTodas_Drop()
        {
            var result = new SelectList(_pensionesService.GetAllPensiones2(), "IdPension", "Pension");
            return Json(result);
        }
        public JsonResult Traslados_Drop()
        {
            var result = new SelectList(_catFormasTrasladoService.ObtenerFormasActivas(), "idFormaTraslado", "formaTraslado");
            return Json(result);
        }

        public JsonResult Tipos_Drop()
        {
            var result = new SelectList(_catTipoInvolucradoService.ObtenerTipos(), "IdTipoInvolucrado", "TipoInvolucrado");
            return Json(result);
        }

        public JsonResult EstadoVictima_Drop()
        {
            var result = new SelectList(_catEstadoVictimaService.ObtenerEstados(), "IdEstadoVictima", "EstadoVictima");
            return Json(result);
        }

        public async Task<JsonResult> Aseguradora_Drop()
        {
            var aseguradoras = _aseguradorasService.GetAllListAsync();

            var result = aseguradoras.Select(a => new
            {
                Value = a.IdAseguradora,
                Text = a.NombreAseguradora
            });

            return Json(result);
        }

        public JsonResult Hospitales_Drop()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var corporation = corp < 2 ? 1 : corp;
            var result = new SelectList(_catHospitalesService.GetHospitales(corp), "IdHospital", "NombreHospital");
            return Json(result);
        }
        public JsonResult InstTraslado_Drop()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var corporation = corp < 2 ? 1 : corp;
            var result = new SelectList(_catInstitucionesTraslado.ObtenerInstitucionesActivas(corp), "IdInstitucionTraslado", "InstitucionTraslado");
            return Json(result);
        }
        public JsonResult InstTraslado_DropTodas()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var corporation = corp < 2 ? 1 : corp;
            var result = new SelectList(_catInstitucionesTraslado.ObtenerInstituciones(corp), "IdInstitucionTraslado", "InstitucionTraslado");
            return Json(result);
        }
        
        public JsonResult Asiento_Drop()
        {
            var result = new SelectList(_catAsientoservice.ObtenerAsientos(), "IdAsiento", "Asiento");
            return Json(result);
        }
        public JsonResult Cinturon_Drop()
        {
            var result = new SelectList(_catCinturon.ObtenerCinturon(), "IdCinturon", "Cinturon");
            return Json(result);
        }
        public JsonResult Casco_Drop()
        {
            var result = new SelectList(_catCinturon.ObtenerCasco(), "IdCasco", "Casco");
            return Json(result);
        }
        public JsonResult Disposicion_Drop()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catAutoridadesDisposicionservice.ObtenerAutoridadesActivas(corp), "IdAutoridadDisposicion", "NombreAutoridadDisposicion");
            return Json(result);
        }
        public JsonResult Entrega_Drop()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var result = new SelectList(_catAutoridadesEntregaService.ObtenerAutoridadesActivas(corp), "IdAutoridadEntrega", "AutoridadEntrega");
            return Json(result);
        }

        public JsonResult Ciudades_Drop()
        {
            var result = new SelectList(_catCiudadesService.ObtenerCiudadesActivas(), "IdCiudad", "Ciudad");
            return Json(result);
        }
        public JsonResult AgMinisterio_Drop()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var result = new SelectList(_catAgenciasMinisterioService.ObtenerAgenciasActivas(corp), "IdAgenciaMinisterio", "NombreAgencia");
            return Json(result);
        }
        public JsonResult AgMinisterioDelegacion_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var result = new SelectList(_catAgenciasMinisterioService.ObtenerAgenciasActivasPorDelegacion(idOficina, corp), "IdAgenciaMinisterio", "NombreAgencia");
            return Json(result);
        }

        public JsonResult AgMinisterioDelegacion_DropTodas()
        {
            //int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var result = new SelectList(_catAgenciasMinisterioService.ObtenerAgenciasActivas(corp), "IdAgenciaMinisterio", "NombreAgencia");
            return Json(result);
        }
        public JsonResult Oficiales_Drop()
        {
			int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

			var result = Oficiales_DropSource(() => _oficialesService.GetOficialesActivosFiltrados(idOficina, idDependencia));			
            return Json(result);
        }

        public JsonResult Oficiales_DropTodos()
        {
			int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
			int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

			var result = Oficiales_DropSource(() => _oficialesService.GetOficialesFiltrados(idOficina, idDependencia));           
            return Json(result);
        }

		private SelectList Oficiales_DropSource(Func<List<CatOficialesModel>> fecthFunc)
		{
			List<CatOficialesModel> source = fecthFunc();
            var oficiales = source.Select(o => new
			{
				IdOficial = o.IdOficial,
				NombreCompleto = (CultureInfo.InvariantCulture.TextInfo.ToTitleCase($"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}".ToLower()))
			});

			oficiales = oficiales.Skip(1);
			var result = new SelectList(oficiales, "IdOficial", "NombreCompleto");

			return result;
		}

		public JsonResult Oficiales_DropCorpTodos()
        {
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            var oficiales = _oficialesService.GetOficialesByCorporacion(idDependencia)// .GetOficialesFiltrados(idOficina, idDependencia)
                .Select(o => new
                {
                    IdOficial = o.IdOficial,
                    NombreCompleto = (CultureInfo.InvariantCulture.TextInfo.ToTitleCase($"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}".ToLower()))
                });
            oficiales = oficiales.Skip(1);
            var result = new SelectList(oficiales, "IdOficial", "NombreCompleto");

            return Json(result);
        }


        public JsonResult CambiosDDLOficiales()
        {
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");


            var oficiales = _oficialesService.GetOficialesPorDependencia(idDependencia)
                .Select(o => new
                {
                    IdOficial = o.IdOficial,
                    NombreCompleto = (CultureInfo.InvariantCulture.TextInfo.ToTitleCase($"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}".ToLower()))
                });
            oficiales = oficiales.Skip(1);
            var result = new SelectList(oficiales, "IdOficial", "NombreCompleto");

            return Json(result);
        }



        public ActionResult CapturaAccidenteC(string descripcionCausa, bool rOy, int? pet)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;

            var estatusAccidente = _capturaAccidentesService.GetEstatusAccidente(idAccidente);

            _capturaAccidentesService.GuardarDescripcion(idAccidente, descripcionCausa, estatusAccidente);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(idAccidente, ip, "CapturaAccidente_Descripcion", "Insertar", "insert", user);
            object data = new { idAccidente = idAccidente, DescripcionCausa = descripcionCausa };

            if (pet > 0)
                _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2028, data);

            DatosAccidenteModel datosAccidente = _capturaAccidentesService.ObtenerDatosFinales(idAccidente);

            var isedit = HttpContext.Session.GetInt32("isEditAccidente");

            ViewBag.EsEdicion = isedit.HasValue ? isedit.ToString() : "0";

            ViewBag.EsSoloLectura = rOy;
            ViewBag.Origen = HttpContext.Session.GetString("origen");
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Captura C Accidentes"), "");

            return View("CapturaCAccidente", datosAccidente);

        }

        public ActionResult CapturaCr(int IdVehiculo, int IdInfraccion)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var InfraccionAccidente = _capturaAccidentesService.RelacionAccidenteInfraccion(IdVehiculo, idAccidente, IdInfraccion);
            var AccidenteSeleccionado = _capturaAccidentesService.ObtenerAccidentePorId(idAccidente, idOficina);
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Captura C Accidentes"), "");

            return View("CapturaCAccidente");
        }
        public ActionResult MostrarModalCrearInfraccion(int IdAccidente, int IdVehiculo, int IdConductor, int IdPropietario, string Placa, string Tarjeta, int IdDelegacion)
        {
            var modelo = new NuevaInfraccionModel
            {
                IdAccidente = IdAccidente,
                IdPersonaInfraccion = IdConductor,
                IdVehiculo = IdVehiculo,
                IdPersona = IdPropietario,
                Placa = Placa,
                Tarjeta = Tarjeta,
                idDelegacion = IdDelegacion,

            };
            _bitacoraServices.BitacoraGenerales(string.Format(CodigosGeneral.C5003, "Accidentes", "Crear Infraccion"), "");

            return PartialView("_ModalCrearInfraccion", modelo);
        }
        public ActionResult MostrarModalAgregarMonto(int IdAccidente, int IdVehiculoInvolucrado, int IdPropietarioInvolucrado)
        {
            var modelo = new MontoModel
            {
                IdAccidente = IdAccidente,
                IdVehiculoInvolucrado = IdVehiculoInvolucrado,
                IdPropietarioInvolucrado = IdPropietarioInvolucrado,
            };

            return PartialView("_ModalAgregarMonto", modelo);
        }


        [HttpPost]
        public ActionResult ajax_CrearInfraccion(NuevaInfraccionModel model)
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var DatosAccidente = _capturaAccidentesService.ObtenerAccidentePorId(idAccidente, idOficina);
            model.IdMunicipio = (int)DatosAccidente.IdMunicipio;
            model.IdCarretera = (int)DatosAccidente.IdCarretera;
            model.IdTramo = (int)DatosAccidente.IdTramo;
            model.Kilometro = DatosAccidente.Kilometro;

            DateTime fechaInfraccion = (DateTime)DatosAccidente.Fecha;
            //     string horaInfraccion = DatosAccidente.Hora.ToString("hhmm");



            model.fechaInfraccion = fechaInfraccion;
            model.fechaVencimiento = getFechaVencimiento(model.fechaInfraccion, (int)model.IdMunicipio);

            var errors = ModelState.Values.SelectMany(s => s.Errors);
            if (ModelState.IsValid)
            {
                var ExistenciaInfraccion = _capturaAccidentesService.VerificarExistenciaInfraccion(model.IdVehiculo, idAccidente);
                if (ExistenciaInfraccion == 2)
                {
                    // Si la infracción ya existe, enviar un JSON con un mensaje de error
                    return Json(new { error = "El vehículo ya cuenta con una infracción en el accidente." });
                }
                else
                {
                    var idInfraccion = _capturaAccidentesService.RegistrarInfraccion(model, idDependencia);
                    var idPersonaConductor = _infraccionesService.CrearPersonaInfraccion((int)idInfraccion, (int)model.IdPersona);
                    var InfraccionAccidente = _capturaAccidentesService.RelacionAccidenteInfraccion(model.IdVehiculo, idAccidente, idInfraccion);
                    //BITACORA
                    object data = new { idAccidente = idAccidente, idVehiculo = model.IdVehiculo };
                    _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2011, model);

                    //var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    //var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                    //_bitacoraServices.insertBitacora(idPersonaConductor, ip, "CapturaAccidente_PersonaInfraccion", "Insertar", "insert", user);

                    return Json(new { id = idInfraccion });
                }
            }
            else
            {
                // Manejar los errores de validación si es necesario
                return PartialView("_ModalCrearInfraccion");
            }
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






        [HttpPost]
        public ActionResult AgregarMontoVehiculo(MontoModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {

                _capturaAccidentesService.AgregarMontoV(model);
                int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;

                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                //_bitacoraServices.insertBitacora(idAccidente, ip, "CapturaAccidente_Monto", "Insertar", "insert", user);
                object data = new { idAccidente = idAccidente, idVehiculo = model.IdVehiculoInvolucrado };
                _bitacoraServices.BitacoraAccidentes(idAccidente, string.Format(CodigosAccidente.C2013), data);
                _bitacoraServices.BitacoraAccidentes(idAccidente, string.Format(CodigosAccidente.C2010), data);

                var ListVehiculos = _capturaAccidentesService.VehiculosInvolucrados(idAccidente);
                return PartialView("_ListaVehiculosDaños", ListVehiculos);
            }
            return PartialView("_ModalAgregarMonto");

        }

        public JsonResult ObtenerInfraccionesVehiculos([DataSourceRequest] DataSourceRequest request)
        {
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListVehiculosInfracciones = _capturaAccidentesService.InfraccionesVehiculosAccidete(idAccidente);

            return Json(ListVehiculosInfracciones.ToDataSourceResult(request));
        }
        public ActionResult ModalInfraccionesVehiculos(string montoCamino, string montoCarga, string montoPropietarios, string montoOtros)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var DatosMontos = _capturaAccidentesService.GuardarDatosPrevioInfraccion(idAccidente, montoCamino, montoCarga, montoPropietarios, montoOtros);

            return PartialView("_ModalAsignarInfracciones");
        }
        public IActionResult VincularInfraccionAccidente(int IdVehiculo, int IdInfraccion)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var InfraccionAccidente = _capturaAccidentesService.RelacionAccidenteInfraccion(IdVehiculo, idAccidente, IdInfraccion);
            return Json(InfraccionAccidente);
        }

        public IActionResult VincularInfraccionAccidente2(int IdVehiculo, int IdInfraccion)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var InfraccionAccidente = _capturaAccidentesService.RelacionAccidenteInfraccion2(IdVehiculo, idAccidente, IdInfraccion);
            if (InfraccionAccidente == 2)
            {
                return Json(new { error = "El vehículo ya cuenta con una infracción en el accidente." });
            }
            object data = new { idAccidente = idAccidente, idVehiculo = IdVehiculo };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2011, data);
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2012, data);
            return Json(InfraccionAccidente);
        }
        public JsonResult ObtInfraccionesAccidente([DataSourceRequest] DataSourceRequest request)
        {
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListInfracciones = _capturaAccidentesService.InfraccionesDeAccidente(idAccidente);

            return Json(ListInfracciones.ToDataSourceResult(request));
        }
        public IActionResult GuardarRelacionPersonaVehiculo(int IdPersona, int IdVehiculoInvolucrado, int IdInvolucrado)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var PersonaVehiculo = _capturaAccidentesService.RelacionPersonaVehiculo(IdPersona, idAccidente, IdVehiculoInvolucrado, IdInvolucrado);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(PersonaVehiculo, ip, "CapturaAccidente_RelacionPersonaVehiculo", "Insertar", "insert", user);
            object data = new { idAccidente = idAccidente, IdPersona = IdPersona, IdVehiculoInvolucrado = IdVehiculoInvolucrado };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2056, data);

            return Json(PersonaVehiculo);
        }
        public async Task<IActionResult> ActualizarInfoInvolucradoAsync(CapturaAccidentesModel model, IFormFile archivoInvolucrado)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroActualizado = _capturaAccidentesService.ActualizarInvolucrado2(model, idAccidente);

            if (archivoInvolucrado != null && archivoInvolucrado.Length > 0)
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await archivoInvolucrado.CopyToAsync(memoryStream);
                        var fileData = memoryStream.ToArray();

                        var fileName = $"{idAccidente}_{archivoInvolucrado.FileName}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(archivoInvolucrado.FileName)}";
                        var filePath = Path.Combine(_rutaArchivo, fileName);

                        System.IO.File.WriteAllBytes(filePath, fileData);

                        var datosAccidente = new DatosAccidenteModel
                        {
                            archivoInvolucradoPath = filePath,
                            archivoInvolucradoStr = fileName,
                            idInvolucrado = model.idPersona
                        };

                        _capturaAccidentesService.AnexarArchivoInvolucrado(datosAccidente, idAccidente);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error al guardar el archivo: {ex.Message}" });
                }

            }
            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(RegistroActualizado, ip, "CapturaAccidente_Involucrado", "Actualizar", "update", user);
            object data = new { idAccidente = idAccidente, idPersona = model.IdPersona, obj = model };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2005, data);
            return Json(RegistroActualizado);
        }

        public IActionResult ActualizarRelacionVehiculoInvolucrado(CapturaAccidentesModel model)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroActualizado = _capturaAccidentesService.ActualizarRelacionVehiculoPersona(model, idAccidente);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(RegistroActualizado, ip, "CapturaAccidente_Involucrado", "Actualizar", "update", user);
            object data = new { idAccidente = idAccidente, obj = model };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2019, data);

            return Json(RegistroActualizado);
        }
        public JsonResult ObtInvolucradosAccidente([DataSourceRequest] DataSourceRequest request)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListInvolucrados = _capturaAccidentesService.InvolucradosAccidente(idAccidente);

            ListInvolucrados = ListInvolucrados.Select(x =>
            {
                var temp = x;
                temp.FolioDetencion = _capturaAccidentesService.GetAccidenteFolioDetencion(x.IdAccidente.Value, x.IdPersona);

                if (EsFormatoImagen(temp.rutaArchivo))
				{
                    temp.EsImagen = true;
					if (System.IO.File.Exists(temp.rutaArchivo))
                    {
                        try
                        {
                            byte[] data = System.IO.File.ReadAllBytes(temp.rutaArchivo);

                            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                            if (!provider.TryGetContentType(temp.rutaArchivo, out var mimeType))
                            {
                                mimeType = "application/octet-stream";
                            }

                            string b64 = Convert.ToBase64String(data);

                            temp.UrlImagen = "data:" + mimeType + ";base64," + b64;

						}
						catch (Exception ex)
                        {
                            temp.UrlImagen = null;
                        }
                    }
                    else
                    {
                        temp.UrlImagen = null; 
                    }
                }
                else
                {
                    temp.UrlImagen = null;
                }

                return temp;
            }).ToList();

            return Json(ListInvolucrados.ToDataSourceResult(request));
        }


        private bool EsFormatoImagen(string rutaArchivo)
		{
			var extensionesImagen = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };
			return extensionesImagen.Any(ext => rutaArchivo.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
		}


		public IActionResult EliminaInvolucrado(int IdAccidente, int idPersona, int IdInvolucrado)
        {
            var eliminarInvolucrado = _capturaAccidentesService.EliminarInvolucrado(IdInvolucrado);
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var ListInvolucrados = _capturaAccidentesService.InvolucradosAccidente(idAccidente);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(eliminarInvolucrado, ip, "CapturaAccidente_Involucrado", "Eliminar", "delete", user);
            object data = new { idAccidente = idAccidente, idPersona = idPersona };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2033, data);

            return Json(ListInvolucrados);

        }
        public IActionResult EditarInvolucrado(CapturaAccidentesModel model)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.EditarInvolucrado(model);
            object data = new { idAccidente = idAccidente, idPersona = model.idPersona, obj = model };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2019, data);
            var datosGrid = _capturaAccidentesService.ObtenerDatosGridFactor(idAccidente);

            return Json(datosGrid);
        }

        public ActionResult MostrarModalFechaHora(int IdPersona, string FechaIngreso,string Folio,string Hora)
        {

            DateTime Fecha = DateTime.MinValue;
            DateTime.TryParseExact(FechaIngreso, "dd/MM/yyyy", CultureInfo.InvariantCulture,DateTimeStyles.None, out Fecha);
               

            var model = new FechaHoraIngresoModel
            {
                IdPersona = IdPersona,
                FechaIngreso = Fecha== DateTime.MinValue?null:Fecha,
                HoraIngresoStr = Hora,
                FolioDetencionStr=Folio
            };


            return PartialView("_ModalFechaHora", model);
        }
        [HttpPost]
        public ActionResult AgregarFechaHora(FechaHoraIngresoModel model)
        {
            if (!string.IsNullOrEmpty(model.HoraIngresoStr))
            {
                TimeSpan hora;
                if (TimeSpan.TryParse(model.HoraIngresoStr, out hora))
                {
                    // Asignar la hora convertida al modelo
                    model.HoraIngreso = hora;
                }
                else
                {
                    ModelState.AddModelError("HoraComoString", "La hora proporcionada no es válida.");
                }
            }
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {
                int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
                _capturaAccidentesService.AgregarFechaHoraIngreso(model, idAccidente);
                _capturaAccidentesService.AgregarAccidenteFolioDetencion(model, idAccidente);
                var ListInvolucrados = _capturaAccidentesService.InvolucradosAccidente(idAccidente);
                ListInvolucrados = ListInvolucrados.Select(x =>
                {
                    var temp = x;
                    temp.FolioDetencion = _capturaAccidentesService.GetAccidenteFolioDetencion(x.IdAccidente.Value, x.IdPersona);
                    return temp;
                }).ToList();
                return PartialView("_ListaInvolucradosFechaYHora", ListInvolucrados);
            }
            return Json(new { success = false });
        }
        [HttpPost]

        public IActionResult InsertarDatos(DatosAccidenteModel datosAccidente, int armasValue, int drogasValue, int valoresValue, int prendasValue, int otrosValue, int convenioValue)
        {
            try
            {
				int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;

				var accidente = _capturaAccidentesService.AccidentePorID(idAccidente).FirstOrDefault() ?? throw new NotFoundException("accidente no encontrado");
				if (!accidente.Fecha.HasValue) throw new Exception("No se ha registrado la fecha del accidente");
                var oficina = User.FindFirst(CustomClaims.Oficina).Value;
                var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                int IdMpio = _catMunicipiosService.obtenerIdPorNombre(oficina, corp);
                var DatRequerido = _catCamposObligService.CamposPermitido(2, 15, null, "Validacion Parte");

                if (DatRequerido.Accidentes == 1)
                {

                    _oficialesService.ValidateFirmaDeOficialOrThrow(datosAccidente.IdElabora, accidente.Fecha.GetValueOrDefault());
                    _oficialesService.ValidateFirmaDeOficialOrThrow(datosAccidente.IdSupervisa, accidente.Fecha.GetValueOrDefault());
                    _oficialesService.ValidateFirmaDeOficialOrThrow(datosAccidente.IdAutoriza, accidente.Fecha.GetValueOrDefault());
                }
                _capturaAccidentesService.AgregarDatosFinales(datosAccidente, armasValue, drogasValue, valoresValue, prendasValue, otrosValue, idAccidente, convenioValue);
				object data = new { idAccidente = idAccidente, datosAccidente = datosAccidente, armasValue = armasValue, drogasValue = drogasValue, valoresValue = valoresValue, prendasValue = prendasValue, otrosValue = otrosValue, convenioValue = convenioValue };
				_bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2014, data);


				return Json(datosAccidente);
			}
            catch (Exception ex) when (ex is NotFoundException || ex is FirmaException)
            {
                return Json(new { success = false, message = ex.Message });
            }            
        }
        public async Task<IActionResult> GuardarBoleta(IFormFile boleta)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;

            if (boleta != null && boleta.Length > 0)
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await boleta.CopyToAsync(memoryStream);
                        var fileData = memoryStream.ToArray();

                        var fileName = $"{idAccidente}_{boleta.FileName}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(boleta.FileName)}";
                        var filePath = Path.Combine(_rutaArchivo, fileName);

                        System.IO.File.WriteAllBytes(filePath, fileData);

                        var datosAccidente = new DatosAccidenteModel
                        {
                            boletaPath = filePath,
                            boletaStr = fileName
                        };

                        _capturaAccidentesService.AnexarBoleta(datosAccidente, idAccidente);

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

        public IActionResult ObtenerBoleta()
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;


            var path = _capturaAccidentesService.GetBoletaPath(idAccidente);

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
        public async Task<IActionResult> GuardarArchivoParte(IFormFile parteFisico)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;

            if (parteFisico != null && parteFisico.Length > 0)
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await parteFisico.CopyToAsync(memoryStream);
                        var fileData = memoryStream.ToArray();

                        var fileName = $"{idAccidente}_{parteFisico.FileName}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(parteFisico.FileName)}";
                        var filePath = Path.Combine(_rutaArchivo, fileName);

                        System.IO.File.WriteAllBytes(filePath, fileData);

                        var datosAccidente = new DatosAccidenteModel
                        {
                            archivoPartePath = filePath,
                            archivoParteStr = fileName
                        };

                        _capturaAccidentesService.AnexarArchivoParte(datosAccidente, idAccidente);

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

        public IActionResult ObtenerArchivoParte()
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;


            var path = _capturaAccidentesService.GetArchivoPartePath(idAccidente);

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

        public IActionResult ObtenerArchivoInvolucrado(int IdInvolucrado)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var path = _capturaAccidentesService.GetArchivoInvolucradoPath(idAccidente, IdInvolucrado);

            if (System.IO.File.Exists(path.value) && !string.IsNullOrEmpty(path.text))
            {
                try
                {
                    // Leer el archivo en un arreglo de bytes
                    byte[] data = System.IO.File.ReadAllBytes(path.value);

                    // Determinar el tipo MIME del archivo
                    var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                    if (!provider.TryGetContentType(path.value, out var mimeType))
                    {
                        // Fallback al tipo MIME genérico si no se puede determinar
                        mimeType = "application/octet-stream";
                    }

                    // Convertir el archivo a una cadena base64
                    string b64 = Convert.ToBase64String(data);

                    // Devolver la respuesta en formato JSON
                    return Json(new { file = b64, app = mimeType, name = path.text });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al leer el archivo: " + ex.Message);
                    return Json(new { file = "" });
                }
            }
            else
            {
                Console.WriteLine("Archivo no encontrado o path vacío");
                return Json(new { file = "" });
            }
        }


        public async Task<IActionResult> GuardarArchivoInvolucrado(IFormFile archivoInvolucrado, int IdInvolucrado)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;

            if (archivoInvolucrado != null && archivoInvolucrado.Length > 0)
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await archivoInvolucrado.CopyToAsync(memoryStream);
                        var fileData = memoryStream.ToArray();

                        var fileName = $"{idAccidente}_{archivoInvolucrado.FileName}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(archivoInvolucrado.FileName)}";
                        var filePath = Path.Combine(_rutaArchivo, fileName);

                        System.IO.File.WriteAllBytes(filePath, fileData);

                        var datosAccidente = new DatosAccidenteModel
                        {
                            archivoInvolucradoPath = filePath,
                            archivoInvolucradoStr = fileName,
                            idInvolucrado = IdInvolucrado
                        };

                        _capturaAccidentesService.AnexarArchivoInvolucrado(datosAccidente, idAccidente);

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

        public async Task<IActionResult> GuardarArchivoCroquis(IFormFile archivoCroquis, int idAccidente)
        {
            idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? idAccidente;

            if (archivoCroquis != null && archivoCroquis.Length > 0)
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await archivoCroquis.CopyToAsync(memoryStream);
                        var fileData = memoryStream.ToArray();

                        var fileName = $"{idAccidente}-{DateTime.Now:ddMMyyyy}-{Guid.NewGuid().ToString("D")}{Path.GetExtension(archivoCroquis.FileName)}";
                        var croquisDirectory = Path.Combine(_rutaArchivo, "Accidentes", idAccidente.ToString(), "Croquis");
                        if (!Directory.Exists(croquisDirectory))
                        {
                            Directory.CreateDirectory(croquisDirectory);
                        }
                        var filePath = Path.Combine(croquisDirectory, fileName);

                        System.IO.File.WriteAllBytes(filePath, fileData);

                        var datosAccidente = new DatosAccidenteModel
                        {
                            archivoInvolucradoPath = filePath,
                            archivoInvolucradoStr = fileName
                        };

                        _capturaAccidentesService.AnexarArchivoCroquis(datosAccidente, idAccidente);

                        return Json(new { success = true, message = "Archivo croquis guardado correctamente." });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error al guardar el archivo croquis: {ex.Message}" });
                }
            }

            return Json(new { success = false, message = "No se ha seleccionado ningún archivo." });
        }

        public ActionResult SetLastInsertedIdEdit(bool modoSoloLectura, int idAccidente, string origen)
        {
            ViewBag.ModoSoloLectura = modoSoloLectura;

            const int maxRetries = 10;
            const int delay = 50; // 50 ms
            bool sessionSet = false;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    // Eliminar el valor anterior si existe
                    HttpContext.Session.Remove("LastInsertedId");

                    HttpContext.Session.SetInt32("LastInsertedId", idAccidente);
                    HttpContext.Session.SetInt32("isEditAccidente", 1);
                    sessionSet = true;
                    break;
                }
                catch (Exception ex)
                {
                    System.Threading.Thread.Sleep(delay);
                }
            }

            if (!sessionSet)
            {
                return StatusCode(500, "No se pudo establecer la sesión después de varios intentos.");
            }

            while (HttpContext.Session.GetInt32("LastInsertedId") != idAccidente)
            {
                System.Threading.Thread.Sleep(delay);
            }

            if (origen != null)
            {
                HttpContext.Session.SetString("origen", origen);
            }
            else
            {
                HttpContext.Session.SetString("origen", "");
            }
            object data = new { idAccidente = idAccidente };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2053, data);
            return Json(new { LastInsertedId = idAccidente });

        }



        public ActionResult SetLastInsertedIdCapt(bool modoSoloLectura, int idAccidente)
        {
            ViewBag.ModoSoloLectura = modoSoloLectura;
            HttpContext.Session.SetInt32("LastInsertedId", idAccidente);
            System.Threading.Thread.Sleep(1000);

            return Ok();
        }

        public IActionResult ConsultaAccidente(bool modoSoloLectura, int idAccidente)
        {
            HttpContext.Session.SetInt32("LastInsertedId", idAccidente);
            ViewBag.ModoSoloLectura = modoSoloLectura;
            return Ok();
        }
        [HttpPost]
        public ActionResult ajax_CrearPersonaMoral(PersonaModel Persona)
        {
            Persona.idCatTipoPersona = (int)TipoPersona.Moral;
            if (Persona.PersonaDireccion != null)
            {
                if (Persona.PersonaDireccion.idMunicipioMoral > 0)
                {
                    Persona.PersonaDireccion.idMunicipio = Persona.PersonaDireccion.idMunicipioMoral;
                    Persona.PersonaDireccion.idMunicipioFisico = Persona.PersonaDireccion.idMunicipioMoral;
                }
            }
            var IdPersonaMoral = _personasService.CreatePersonaMoral(Persona);
            //var personasMoralesModel = _personasService.GetAllPersonasMorales();
            var modelList = _personasService.ObterPersonaPorIDList(IdPersonaMoral); ;

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(IdPersonaMoral, ip, "CapturaAccidente_PersonaMoral", "Insertar", "insert", user);
            object data = new { idPersonaMoral = IdPersonaMoral };
            _bitacoraServices.BitacoraAccidentes(0, CodigosAccidente.C2057, data);


            return PartialView("_ListPersonasMorales", modelList);
        }


        [HttpPost]
        public ActionResult ajax_BuscarPersonasFiscas()
        {
            var personasFisicas = _personasService.GetAllPersonasFisicas();
            return PartialView("_PersonasFisicas", personasFisicas);
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
                    total = personasFisicas.ToList().FirstOrDefault().total;

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



        public IActionResult pruebapartial()
        {

            var q = PartialView("_PersonasFisicas");
            return q;
        }



        [HttpPost]
        public ActionResult ajax_BuscarPersonaMoral(PersonaMoralBusquedaModel PersonaMoralBusquedaModel)
        {
            PersonaMoralBusquedaModel.IdTipoPersona = (int)TipoPersona.Moral;
            var personasMoralesModel = _personasService.GetAllPersonasMorales(PersonaMoralBusquedaModel);
            return PartialView("_ListPersonasMorales", personasMoralesModel);
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
            //Persona.idTipoLicencia = Persona.idTipoLicencia;
            //Persona.vigenciaLicencia = Persona.vigenciaLicencia;
            Persona.PersonaDireccion.idEntidad = Persona.PersonaDireccion.idEntidadFisico;
            Persona.PersonaDireccion.idMunicipio = Persona.PersonaDireccion.idMunicipioFisico;
            Persona.PersonaDireccion.correo = Persona.PersonaDireccion.correoFisico;
            Persona.PersonaDireccion.telefono = Persona.PersonaDireccion.telefonoFisico;
            Persona.PersonaDireccion.colonia = Persona.PersonaDireccion.coloniaFisico;
            Persona.PersonaDireccion.calle = Persona.PersonaDireccion.calleFisico;
            Persona.PersonaDireccion.numero = Persona.PersonaDireccion.numeroFisico;
            Persona.idCatTipoPersona = (int)TipoPersona.Fisica;
            var IdPersonaFisica = _personasService.CreatePersona(Persona);
            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(IdPersonaFisica, ip, "CapturaAccidente_PersonaFisica", "Insertar", "insert", user);
            object data = new { IdPersonaFisica = IdPersonaFisica };
            _bitacoraServices.BitacoraAccidentes(0, CodigosAccidente.C2058, Persona);

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


        public ActionResult ajax_CrearVehiculo_Ejemplo(VehiculoModel model)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

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



            var resultados = _capturaAccidentesService.BuscarPorParametro(model.placas, model.serie, model.placas, corp);

            return Json(new { data = resultados });
        }

        public ActionResult ajax_CrearVehiculo_Ejemplo2(VehiculoModel model)
        {
            // creamos a la persona sin datos
            if (model.Persona.idPersona == -1)
            {

                PersonaDireccionModel direccion = new PersonaDireccionModel();
                direccion.idEntidad = 35;
                direccion.colonia = "Se ignora";
                direccion.calle = "Se ignora";
                direccion.numero = "Se ignora";
                PersonaModel persona = new PersonaModel();
                persona.nombre = "Se ignora";
                persona.idCatTipoPersona = 0;
                persona.PersonaDireccion = direccion;
                persona.idGenero = 0;
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
                var resultados = _capturaAccidentesService.BuscarPorParametroid(IdVehiculo.ToString());
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

                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                //_bitacoraServices.insertBitacora(IdVehiculo, ip, "CapturaAccidente_Vehiculo", "Actualizar", "update", user);
                object data = new { IdVehiculo = IdVehiculo };
                _bitacoraServices.BitacoraAccidentes(0, CodigosAccidente.C2059, data);
            }
            else if (model.encontradoEn == (int)EstatusBusquedaVehiculo.NoEncontrado)
            {
                IdVehiculo = _vehiculosService.CreateVehiculo(model);

                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                //_bitacoraServices.insertBitacora(IdVehiculo, ip, "CapturaAccidente_Vehiculo", "Insertar", "insert", user);
                object data = new { IdVehiculo = IdVehiculo };
                _bitacoraServices.BitacoraAccidentes(0, CodigosAccidente.C2060, data);
            }

            if (IdVehiculo != 0)
            {
                var resultados = _vehiculosService.GetAllVehiculos();
                return Json(new { data = resultados });
            }
            else
            {
                return null;
            }
        }

        public async Task<IActionResult> BuscarPorParametro(PersonaModel model)
        {
            // Realizar la búsqueda de personas
            var builder = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();


            string UrlHost = configuration.GetSection("AppSettings").GetSection("UrlHost").Value.ToString();
            var personasList = _personasService.BusquedaPersona(model);

            var personasListFormato = personasList.Select(persona => new
            {
                IdPersona = persona.idPersona,
                nombre = persona.nombre,
                apellidoPaterno = persona.apellidoPaterno,
                apellidoMaterno = persona.apellidoMaterno,
                CURP = persona.CURP,
                RFC = persona.RFC,
                fechaNacimiento = persona.fechaNacimiento,
                numeroLicencia = persona.numeroLicencia
            }).ToList();
            if (personasList.Any())
            {
                return Json(new { encontrada = true, data = personasListFormato });
            }
            string parametros = "";
            parametros += string.IsNullOrEmpty(model.numeroLicenciaBusqueda) ? "" : "licencia=" + model.numeroLicenciaBusqueda;
            parametros += string.IsNullOrEmpty(model.CURPBusqueda) ? "" : "curp=" + model.CURPBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.RFCBusqueda) ? "" : "rfc=" + model.RFCBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.nombreBusqueda) ? "" : "nombre=" + model.nombreBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.apellidoPaternoBusqueda) ? "" : "primer_apellido=" + model.apellidoPaternoBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.apellidoMaternoBusqueda) ? "" : "segundo_apellido=" + model.apellidoMaternoBusqueda;

            string ultimo = parametros.Substring(parametros.Length - 1);
            if (ultimo.Equals("&"))
                parametros = parametros.Substring(0, parametros.Length - 1);

            try
            {
                //string urlServ = Request.GetDisplayUrl();
                //Uri uri = new Uri(urlServ);
                //string requested = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

                var url = UrlHost + $"/api/Licencias/datos_generales?" + parametros;

                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    LicenciaRespuestaPersona respuesta = JsonConvert.DeserializeObject<LicenciaRespuestaPersona>(content);

                    return Json(respuesta);
                }
            }
            catch (Exception ex)
            {
                // En caso de errores, devolver una respuesta JSON con licencia no encontrada
                return Json(new { encontrada = false, data = personasListFormato, message = "Ocurrió un error al obtener los datos. " + ex.Message + "; " + ex.InnerException });
            }

            // Si no se cumple la condición anterior, devolver una respuesta JSON indicando que no se encontraron resultados
            return Json(new { encontrada = false, data = personasListFormato, message = "No se encontraron resultados." });
        }

        [HttpPost]
        public ActionResult GuardaDesdeServicio(LicenciaPersonaDatos personaDatos)
        {
            try
            {
                var id = _personasService.InsertarDesdeServicio(personaDatos);
                var datosTabla = _personasService.BuscarPersonaSoloLicencia(personaDatos.NUM_LICENCIA == null ? "sin numero licencia" : personaDatos.NUM_LICENCIA);

                CapturaAccidentesModel involucrado = new CapturaAccidentesModel();
                involucrado.IdPersona = (int)datosTabla.idPersona;
                involucrado.nombre = datosTabla.nombre;
                involucrado.apellidoPaterno = datosTabla.apellidoPaterno;
                involucrado.apellidoMaterno = datosTabla.apellidoMaterno;
                involucrado.RFC = datosTabla.RFC;
                involucrado.CURP = datosTabla.CURP;
                involucrado.numeroLicencia = datosTabla.numeroLicencia;

                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                //_bitacoraServices.insertBitacora(id, ip, "CapturaAccidente_DesdeServicio", "Insertar", "insert", user);
                object data = new { IdPersona = involucrado.IdPersona, obj = involucrado };
                _bitacoraServices.BitacoraAccidentes(0, CodigosAccidente.C2061, data);

                return Json(involucrado);
            }
            catch (Exception ex)
            {
                // Maneja el error de manera adecuada
                return Json(new { error = "Error al guardar en la base de datos: " + ex.Message });
            }
        }


        [HttpPost]
        public IActionResult ajax_CrearPersona(PersonaModel model)
        {
            int id = _personasService.CreatePersona(model);

            var modelList = _capturaAccidentesService.ObtenerConductorPorId(id);

            if (modelList != null)
            {
                modelList.idPersona = modelList.IdPersona;

                string formatoFecha = "dd/MM/yyyy";
                DateTime fechaNacimiento;

                if (DateTime.TryParseExact(modelList.FormatDateNacimiento, formatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaNacimiento))
                {
                    modelList.fechaNacimiento = fechaNacimiento;
                }
                else
                {
                    modelList.fechaNacimiento = null;
                }

                var jsonSettings = new JsonSerializerSettings
                {
                    DateFormatString = "dd/MM/yyyy",
                };

                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                //_bitacoraServices.insertBitacora(id, ip, "CapturaAccidente_Persona", "Insertar", "insert", user);
                object data = new { IdPersona = id };
                _bitacoraServices.BitacoraAccidentes(0, CodigosAccidente.C2061, data);

                return new JsonResult(modelList, jsonSettings);
            }
            else
            {

                return NotFound();
            }
        }




        public JsonResult test()
        {
            var catGeneros = _catDictionary.GetCatalog("CatGeneros", "0");
            var result = new SelectList(catGeneros.CatalogList, "Id", "Text");
            return Json(result);
        }

        [HttpGet]
        public IActionResult ajax_ModalEditarPersona(int idPersona)
        {
            var model = _personasService.GetPersonaById(idPersona);
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
            return PartialView("_EditarPersona", model);
        }

        [HttpGet]
        public IActionResult ajax_ModalEditarPersona2(int idPersona, int esFisico)
        {
            var model = _personasService.GetPersonaById(idPersona);
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
            ViewBag.EsFisico = esFisico;

            return PartialView("_EditarPersona", model);
        }

        // TODO revisar nombre del method
        public IActionResult ajax_ModalEditarPersonaFromVehiculoSection(int id, int esConductor, int UpdatePermanencia, int Ope)
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

            ViewBag.NavigationFlow = NavigationFlow.Accidentes;
            return PartialView("_EditarPersonaReturnId", model);
        }

        // TODO revisar nombre del method
        public IActionResult ajax_ModalEditarPersonaHistoricoFromVehiculoSection(int id, int esConductor, int idAccidente, int UpdatePermanencia, int Ope)
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

            var model = _personasService.GetPersonaByIdHistorico(id, idAccidente, esConductor == 1 ? 1 : 2);
            var personaFolioDetencion = _infraccionesService.GetPersonaFolioDetencion(id);
            model.folioDetencion = personaFolioDetencion;

            //validacion
            model.PersonaDireccion.telefono = StringFunctions.CleanNullString(model.PersonaDireccion.telefono);
            model.PersonaDireccion.correo = StringFunctions.CleanNullString(model.PersonaDireccion.correo);
            model.PersonaDireccion.numero = StringFunctions.CleanNullString(model.PersonaDireccion.numero);
            model.PersonaDireccion.calle = StringFunctions.CleanNullString(model.PersonaDireccion.calle);
            model.PersonaDireccion.colonia = StringFunctions.CleanNullString(model.PersonaDireccion.colonia);

            ViewBag.NavigationFlow = NavigationFlow.Accidentes;
            return PartialView("_EditarPersonaReturnId", model);
        }

        // TODO revisar nombre del method, se mantine signature por compatibilidad con el front
        public IActionResult ajax_EditarPersonaFromVehiculoSection(PersonaModel model, int percan, int Ope, int IsCom)
        {
            int idAccidente = Ope;
            model.PersonaDireccion.telefono = StringFunctions.CleanNullString(model.PersonaDireccion.telefono);
            model.PersonaDireccion.correo = StringFunctions.CleanNullString(model.PersonaDireccion.correo);
            model.PersonaDireccion.numero = StringFunctions.CleanNullString(model.PersonaDireccion.numero);
            model.PersonaDireccion.calle = StringFunctions.CleanNullString(model.PersonaDireccion.calle);
            model.PersonaDireccion.colonia = StringFunctions.CleanNullString(model.PersonaDireccion.colonia);

            if (model.PersonaDireccion.idPersona == null || model.PersonaDireccion.idPersona <= 0)
            {
                model.PersonaDireccion.idPersona = model.idPersona;
                int idDireccion = _personasService.CreatePersonaDireccion(model.PersonaDireccion);

                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                //_bitacoraServices.insertBitacora(idDireccion, ip, "CapturaAccidente_PersonaDireccion", "Insertar", "insert", user);
                object data = new { IdPersona = model.IdPersona, idDireccion = idDireccion };
                _bitacoraServices.BitacoraAccidentes(0, CodigosAccidente.C2062, data);

            }
            else
            {
                int idDireccion = _personasService.UpdatePersonaDireccion(model.PersonaDireccion);
                int historicoDireccion = _personasService.UpdateHistoricoDireccionPersonaAccidente((int)model.idPersona, idAccidente);

                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                //_bitacoraServices.insertBitacora(idDireccion, ip, "CapturaAccidente_PersonaDireccion", "Actualizar", "update", user);
                object data = new { IdPersona = model.IdPersona, idDireccion = idDireccion };
                _bitacoraServices.BitacoraAccidentes(0, CodigosAccidente.C2062, data);
            }

            int id = (model.idPersona != null && model.idPersona > 0)
                ? _personasService.UpdatePersona(model)
                : _personasService.CreatePersona(model);

            return Json(new { data = id, id = model.PersonaDireccion.idPersona });
        }

        [HttpPost]
        public IActionResult ajax_EditarPersona(PersonaModel model)
        {
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            //var model = json.ToObject<Gruas2Model>();
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {
                if (model.PersonaDireccion.idPersona == null || model.PersonaDireccion.idPersona <= 0)
                {
                    model.PersonaDireccion.idPersona = model.idPersona;
                    int idDireccion = _personasService.CreatePersonaDireccion(model.PersonaDireccion);

                    //BITACORA
                    var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                    //_bitacoraServices.insertBitacora(idDireccion, ip, "CapturaAccidente_PersonaDireccion", "Insertar", "insert", user);
                    object data = new { IdPersona = model.IdPersona };
                    _bitacoraServices.BitacoraAccidentes(0, CodigosAccidente.C2063, model);

                }
                else
                {
                    int idDireccion = _personasService.UpdatePersonaDireccion(model.PersonaDireccion);
                    int historicoDireccion = _personasService.UpdateHistoricoDireccionPersonaAccidente((int)model.idPersona, idAccidente);

                    //BITACORA
                    var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                    //_bitacoraServices.insertBitacora(idDireccion, ip, "CapturaAccidente_PersonaDireccion", "Actualizar", "update", user);
                    object data = new { IdPersona = model.IdPersona, idDireccion = idDireccion };
                    _bitacoraServices.BitacoraAccidentes(0, CodigosAccidente.C2062, model);
                }
                int id = _personasService.UpdatePersona(model);
                var modelList = _personasService.GetPersonaById((int)model.idPersona);
                var formattedModelList = new
                {
                    IdPersona = modelList.idPersona,
                    idPersona = modelList.idPersona,
                    nombre = modelList.nombre,
                    apellidoPaterno = modelList.apellidoPaterno,
                    apellidoMaterno = modelList.apellidoMaterno,
                    RFC = modelList.RFC,
                    CURP = modelList.CURP,
                    fechaNacimiento = modelList.fechaNacimiento != null ? ((DateTime)modelList.fechaNacimiento).ToString("dd/MM/yyyy") : "",
                    numeroLicencia = modelList.numeroLicencia,
                    entidad = modelList.PersonaDireccion.entidad,
                    municipio = modelList.PersonaDireccion.municipio,
                    calle = modelList.PersonaDireccion.calle,
                    numero = modelList.PersonaDireccion.numero,
                    colonia = modelList.PersonaDireccion.colonia,
                    telefono = modelList.PersonaDireccion.telefono,
                    correo = modelList.PersonaDireccion.correo,
                    genero = modelList.genero
                };
                return Json(new { data = formattedModelList });
            }
            return RedirectToAction("Index");
        }
        public ActionResult ModalEliminarInfraccionDelAccidente(int IdInfraccion, string FolioInfraccion)
        {
            ViewBag.FolioInfraccion = FolioInfraccion;
            return PartialView("_ModalEliminarInfraccion");
        }
        [HttpPost]
        public IActionResult ajax_EliminarRegistroInfraccion(int IdInfraccion)
        {
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            int idAccidente = HttpContext.Session.GetInt32("LastInsertedId") ?? 0;
            var RegistroSeleccionado = _capturaAccidentesService.EliminarRegistroInfraccion(IdInfraccion);
            object data = new { idAccidente = idAccidente, IdInfraccion = IdInfraccion };
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2039, data);
            _bitacoraServices.BitacoraAccidentes(idAccidente, CodigosAccidente.C2040, data);
            var ListInfracciones = _capturaAccidentesService.InfraccionesDeAccidente(idAccidente);

            return Json(ListInfracciones);
        }



        [HttpPost]
        public async Task<IActionResult> BuscarPorParametroPaginado([DataSourceRequest] DataSourceRequest request, CapturaAccidentesModel capturaModel)
        {
            // Realizar la búsqueda de personas
            // if (model != null)
            //    perModel = model;
            //else
            //   model = perModel;
            var builder = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();


            string UrlHost = configuration.GetSection("AppSettings").GetSection("UrlHost").Value.ToString();
            BusquedaPersonaModel model = new BusquedaPersonaModel();
            PersonaModel personaM = new PersonaModel();
            personaM.CURPBusqueda = capturaModel.CURPBusqueda;
            personaM.RFCBusqueda = capturaModel.RFCBusqueda;
            personaM.nombreBusqueda = capturaModel.nombreBusqueda;
            personaM.apellidoPaternoBusqueda = capturaModel.apellidoPaternoBusqueda;
            personaM.apellidoMaternoBusqueda = capturaModel.apellidoMaternoBusqueda;
            personaM.numeroLicenciaBusqueda = capturaModel.numeroLicenciaBusqueda;

            model.PersonaModel = personaM;

            var findAll = false;
            var personas = new BusquedaPersonaModel();
            Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            if (model != null)
            {

                if (model.PersonaModel.apellidoMaternoBusqueda == null &&
                    model.PersonaModel.apellidoPaternoBusqueda == null &&
                    model.PersonaModel.CURPBusqueda == null &&
                    model.PersonaModel.RFCBusqueda == null &&
                    model.PersonaModel.numeroLicenciaBusqueda == null &&
                    model.PersonaModel.nombreBusqueda == null)
                {
                    pagination.PageSize = (request.PageSize > 0) ? request.PageSize : 10;
                }
                else
                {
                    findAll = true;
                    pagination.PageSize = 1000000;
                }
            }
            else
            {
                pagination.PageSize = (request.PageSize > 0) ? request.PageSize : 10;
            }

            //model = perModel;
            var personasList = _personasService.BusquedaPersonaPagination(model, pagination);

            if (personasList.Any())
            {
                personas.ListadoPersonas = personasList;
                var total = 0;
                if (personasList.Count() > 0)
                    total = personasList.ToList().FirstOrDefault().total;

                //if (findAll)
                request.PageSize = pagination.PageSize;

                var result = new DataSourceResult()
                {
                    Data = personas.ListadoPersonas,
                    Total = total
                };
                return Json(result);
            }

            if (model.PersonaModel.tipoPersona == "2")
            {
                return Json(new { encontrada = false, Data = "2", tipo = "sin datos", message = "No busca en licencias por ser moral" });

            }
            else
            {
                // Si no se encontraron resultados en la búsqueda de personas, realizar la búsqueda por licencia
                return Json(new { encontrada = false, Data = "1", tipo = "sin datos", message = "busca en licencias" });
            }
        }
           /* string parametros = "";
            parametros += string.IsNullOrEmpty(model.PersonaModel.numeroLicenciaBusqueda) ? "" : "licencia=" + model.PersonaModel.numeroLicenciaBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.PersonaModel.CURPBusqueda) ? "" : "curp=" + model.PersonaModel.CURPBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.PersonaModel.RFCBusqueda) ? "" : "rfc=" + model.PersonaModel.RFCBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.PersonaModel.nombreBusqueda) ? "" : "nombre=" + model.PersonaModel.nombreBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.PersonaModel.apellidoPaternoBusqueda) ? "" : "primer_apellido=" + model.PersonaModel.apellidoPaternoBusqueda + "&";
            parametros += string.IsNullOrEmpty(model.PersonaModel.apellidoMaternoBusqueda) ? "" : "segundo_apellido=" + model.PersonaModel.apellidoMaternoBusqueda;
            string ultimo = parametros.Substring(parametros.Length - 1);
            if (ultimo.Equals("&"))
                parametros = parametros.Substring(0, parametros.Length - 1);

            try
            {
                //string urlServ = Request.GetDisplayUrl();
                //Uri uri = new Uri(urlServ);
                //string requested = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

                var url = UrlHost + $"/api/Licencias/datos_generales?" + parametros;

                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    //    LicenciaRespuestaPersona respuesta = JsonConvert.DeserializeObject<LicenciaRespuestaPersona>(content);
                    // var personasEncontradas = content.
                    //  return Json(new { encontrada = false, Data = respuesta.datos, tipo = respuesta.datos == null ? "sin datos" : "success", message = respuesta.mensaje });


                    List<LicenciaPersonaDatos> respuesta = JsonConvert.DeserializeObject<List<LicenciaPersonaDatos>>(content);

                    List<CapturaAccidentesModel> resultado = new List<CapturaAccidentesModel>();
                    foreach (LicenciaPersonaDatos pivote in respuesta)
                    {
                        CapturaAccidentesModel capAcc = new CapturaAccidentesModel();
                        capAcc.nombre = pivote.NOMBRE;
                        capAcc.apellidoPaterno = pivote.PRIMER_APELLIDO;
                        capAcc.apellidoMaterno = pivote.SEGUNDO_APELLIDO;
                        capAcc.TipoLicencia = pivote.TIPOLICENCIA; ;
                        capAcc.numeroLicencia = pivote.NUM_LICENCIA;
                        capAcc.CURP = pivote.CURP;
                        capAcc.Calle = pivote.CALLE;
                        capAcc.Numero = pivote.NUM_EXT;
                        capAcc.Colonia = pivote.COLONIA;
                        resultado.Add(capAcc);
                    }



                    //  return Json(pEncontradas);
                    return Json(new { encontrada = true, Data = resultado, tipo = "success", message = "busqueda exitosa" });

                }
            }
            catch (Exception ex)
            {
                // En caso de errores, devolver una respuesta JSON con licencia no encontrada
                return Json(new { encontrada = false, Data = "", message = "Ocurrió un error al obtener los datos. " + ex.Message + "; " + ex.InnerException });
            }


            //                

            return Json(new { encontrada = false, Data = "1", tipo = "sin datos", message = "busca en licencias" });


        }*/
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
            var result = await this.RenderViewAsync2("CapturaAccidentes", models);
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> FolioEmePermitido()
        {
            return Json(FolioObligatorio());
        }
        [HttpGet]
        public async Task<IActionResult> PartePermitido()
        {
            return Json(ParteObligatorio());
        }

        // TODO cambiar nombre ver que hace
        [HttpGet]
        public ActionResult ajax_detalleVehiculo(int idVehiculo, int idAccidente)
        {

            var model = _vehiculosService.GetVehiculoById(idVehiculo);

            model.cargaTexto = (model.carga == true) ? "Si" : "No";
            return PartialView("_DetalleVehiculo", model);
        }

        // TODO cambiar nombre ver que hace
        public ActionResult ajax_detalleVehiculo2(int idVehiculo, int idAccidente)
        {
            var model = _vehiculosService.GetVehiculoHistoricoByIdAndIdinfraccion(idVehiculo, idAccidente);

            model.cargaTexto = (model.carga == true) ? "Si" : "No";
            return PartialView("_DetalleVehiculo", model);
        }

        // TODO cambiar nombre ver que hace
        public IActionResult ajax_ModalEditarVehiculo(int id, int idAccidente)
        {
            var model = _vehiculosService.GetVehiculoId(id.ToString());
            model.IdAccidente = idAccidente;
            return PartialView("_EditarVehiculoReturnId", model);
        }

        // TODO cambiar nombre ver que hace
        public IActionResult ajax_ModalEditarVehiculoHistorico(int id, int IdAccidente, int DoEditar)
        {
            var model = _vehiculosService.GetVehiculoIdHistoricoAccidente(id, IdAccidente);
            model.IdAccidente = IdAccidente;
            ViewBag.DoEditar = DoEditar;
            return PartialView("_EditarVehiculoReturnId", model);
        }

        // TODO cambiar nombre ver que hace
        public IActionResult ajax_EditarVehiculoAcc(VehiculoEditViewModel data, int idAccidente)
        {
            _ = _vehiculosService.UpdateFromEditVehiculo(data);
            int tipoEventoAccidente = 2;
            try
            {
                _vehiculosService.CrearHistoricoVehiculo(idAccidente, Convert.ToInt32(data.id), tipoEventoAccidente);
            }
            catch (Exception e) { }


            var tt = data;

            return Json(new { data = "1", id = tt });
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

        public IActionResult UpdatePropietario(int idPersona, int idVehiculo)
        {
            var t = _vehiculosService.UpdatePropietario(idPersona, idVehiculo);
            return Json(new { data = "1" });
        }

        public ActionResult ModalAgregarConductor()
        {
            BusquedaPersonaModel model = new BusquedaPersonaModel();
            return View("_ModalBusquedaPersonas", model);
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
            return PartialView("_CrearPersonaConductor", new PersonaModel());
        }

        [HttpPost]
        public IActionResult ajax_CrearPersonaFromVehiculoSection(PersonaModel model)
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
            model.idCatTipoPersona = (int)TipoPersona.Fisica;

            //validacion
            model.PersonaDireccion.telefono = StringFunctions.CleanNullString(model.PersonaDireccion.telefono);
            model.PersonaDireccion.correo = StringFunctions.CleanNullString(model.PersonaDireccion.correo);
            model.PersonaDireccion.numero = StringFunctions.CleanNullString(model.PersonaDireccion.numero);
            model.PersonaDireccion.calle = StringFunctions.CleanNullString(model.PersonaDireccion.calle);
            model.PersonaDireccion.colonia = StringFunctions.CleanNullString(model.PersonaDireccion.colonia);

            int id = _personasService.CreatePersona(model);
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
        public async Task<IActionResult> UploadFiles(List<IFormFile> files, int accidenteId)
        {
            try
            {
                if (files == null || files.Count == 0)
                    return BadRequest("no files received.");

                _capturaAccidentesService.GuardarArchivosLugarAccidente(accidenteId, files);

                return Ok(new { success = true });
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadTemporaryFiles(IFormFile[] files)
        {
            // dummy endpoint only for uploading files in browser session
            if (files == null || files.Length == 0)
                return BadRequest("no files received.");

            return Ok(new { success = true });
        }

        [HttpDelete]
        public IActionResult DeleteFiles(string[] files, int accidenteId)
        {
            if (files == null || files.Length == 0)
                return BadRequest("No files specified for deletion.");

            _capturaAccidentesService.EliminarArchivosLugarAccidente(accidenteId, files);

            return Ok(new { success = true });
        }

        [HttpGet]
        public IActionResult GetFiles(int accidenteId)
        
        {
            IEnumerable<FileData> files = _capturaAccidentesService.GetArchivosLugarAccidente(accidenteId);

            foreach (var file in files)
            {
                HttpContext.Session.Set(file.FileName, file.Content);
            }

            return Json(files.Select(f => f.FileName));
        }

        [HttpGet]
        public IActionResult DownloadFile(int accidenteId, string filename)
        {
            IEnumerable<FileData> files = _capturaAccidentesService.GetArchivosLugarAccidente(accidenteId);
            FileData file = files.FirstOrDefault(f => f.FileName == filename);
            if (file == default)
                return NotFound("file not found.");

            var provider = new FileExtensionContentTypeProvider();
            var contentType = provider.TryGetContentType(file.FileName, out string value)
                ? value
                : "application/octet-stream";

            var fileStream = new FileStream(file.FilePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, contentType, file.FileName);
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
                Requerido = DatRequerido != null ? DatRequerido.Accidentes : 0
            };
            return campoValido;
        }

        private CampoValidoDto ParteObligatorio()
        {
            var oficina = User.FindFirst(CustomClaims.Oficina).Value;
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int IdMpio = _catMunicipiosService.obtenerIdPorNombre(oficina, corp);
            var DatRequerido = _catCamposObligService.CamposPermitido(idOficina, IdMpio, null, "Validacion Parte");
            CampoValidoDto campoValido = new()
            {
                Requerido = DatRequerido != null ? DatRequerido.Accidentes : 0
            };
            return campoValido;
        }
    }
}
