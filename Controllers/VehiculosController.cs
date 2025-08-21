using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Util;
using iTextSharp.text;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;
using static GuanajuatoAdminUsuarios.RESTModels.CotejarDatosResponseModel;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class VehiculosController : BaseController
    {
        private readonly ICatDictionary _catDictionary;
        private readonly IVehiculosService _vehiculosService;
        private readonly IPersonasService _personasService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ICotejarDocumentosClientService _cotejarDocumentosClientService;
        private readonly ICatTipoLicenciasService _catTipoLicenciasService;
        private readonly AppSettings _appSettings;
        private readonly ICatMunicipiosService _catMunicipiosService;
        private readonly ICatEntidadesService _catEntidadesService;
        private readonly IColores _coloresService;
        private readonly ICatMarcasVehiculosService _catMarcasVehiculosService;
        private readonly ICatSubmarcasVehiculosService _catSubmarcasVehiculosService;
        private readonly ICatTiposVehiculosService _catTiposVehiculosService;
        private readonly IRepuveService _repuveService;
        private readonly IBitacoraService _bitacoraServices;
        private readonly ICatSubtipoServicio _subtipoServicio;

        private readonly IVehiculoPlataformaService _vehiculoPlataformaService;
        //private readonly ISubmarcasVehiculos submarcasVehiculos;

        private string resultValue = string.Empty;
        public static VehiculoBusquedaModel vehModel = new VehiculoBusquedaModel();
        public VehiculosController(IVehiculosService vehiculosService, ICatDictionary catDictionary,
            IPersonasService personasService, HttpClient httpClientFactory, IConfiguration configuration,
           ICotejarDocumentosClientService cotejarDocumentosClientService, ICatTipoLicenciasService catTipoLicenciasService,
           IOptions<AppSettings> appSettings, ICatMunicipiosService catMunicipiosService, ICatEntidadesService catEntidadesService,
           IColores coloresService, ICatMarcasVehiculosService catMarcasVehiculosService, ICatSubmarcasVehiculosService catSubmarcasVehiculosService,
            ICatTiposVehiculosService catTiposVehiculosService
        , IRepuveService repuveService,
            IBitacoraService bitacoraService,
            ICatSubtipoServicio subtipoServicio
            //, ISubmarcasVehiculos submarcasVehiculos
            )
        {
            _vehiculosService = vehiculosService;
            _catDictionary = catDictionary;
            _personasService = personasService;
            _httpClient = httpClientFactory;
            _configuration = configuration;
            _cotejarDocumentosClientService = cotejarDocumentosClientService;
            _catTipoLicenciasService = catTipoLicenciasService;
            _appSettings = appSettings.Value;
            _catMunicipiosService = catMunicipiosService;
            _catEntidadesService = catEntidadesService;
            _coloresService = coloresService;
            _catMarcasVehiculosService = catMarcasVehiculosService;
            _catSubmarcasVehiculosService = catSubmarcasVehiculosService;
            _catTiposVehiculosService = catTiposVehiculosService;
            _repuveService = repuveService;
            _bitacoraServices = bitacoraService;
            _subtipoServicio = subtipoServicio;
            //_catSubmarcasVehiculosService = _submar
        }

        public IActionResult Index()
        {
            VehiculoBusquedaModel vehiculoBusquedaModel = new VehiculoBusquedaModel();
            vehiculoBusquedaModel.Vehiculo = new VehiculoModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
            //ViewBag.PersonasFisicas= _personasService.GetAllPersonas();
            HttpContext.Session.Remove("IdMarcaVehiculo");
            return View(vehiculoBusquedaModel);
        }

        public IActionResult Editar()
        {
            // var vehiculosModel = _vehiculosService.GetAllVehiculos();
            VehiculoBusquedaModel vehiculoBusquedaModel = new VehiculoBusquedaModel();
            vehiculoBusquedaModel.Vehiculo = new VehiculoModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
            // vehiculoBusquedaModel.ListVehiculo = vehiculosModel.ToList();
            return View(vehiculoBusquedaModel);
        }

        public ActionResult EditarVehiculo(int id)
        {

            var vehiculosModel = _vehiculosService.GetVehiculoById(id);
            VehiculoBusquedaModel vehiculoBusquedaModel = new VehiculoBusquedaModel();
            vehiculoBusquedaModel.Vehiculo = vehiculosModel;
            vehiculoBusquedaModel.Vehiculo.idSubmarcaUpdated = vehiculosModel.idSubmarca;
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
            vehiculoBusquedaModel.isFromUpdate = true;
            vehiculosModel.encontradoEn = (int)EstatusBusquedaVehiculo.Sitteg;


            vehiculoBusquedaModel.Vehiculo.showclose = false;
            vehiculoBusquedaModel.Vehiculo.showSubTipo = (vehiculoBusquedaModel.Vehiculo.idCatTipoServicio == 1 || vehiculoBusquedaModel.Vehiculo.idCatTipoServicio == 5) ? true : false;

            if (vehiculoBusquedaModel.Vehiculo.Persona.idCatTipoPersona == 2)
            {
                vehiculoBusquedaModel.Vehiculo.Persona.MoralNombre = vehiculoBusquedaModel.Vehiculo.Persona.nombre;
                vehiculoBusquedaModel.Vehiculo.Persona.moralrfc = vehiculoBusquedaModel.Vehiculo.Persona.RFC;

            }


            return View("EditarVehiculo", vehiculoBusquedaModel.Vehiculo);
        }


        public JsonResult Entidades_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult Municipios_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatMunicipios", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult TipoServicios_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatTipoServicio", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }
        public JsonResult Municipios_Drop(int entidadDDlValue)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catMunicipiosService.GetMunicipiosPorEntidad(entidadDDlValue, corp), "IdMunicipio", "Municipio");
            return Json(result);
        }
        public JsonResult Colores_Read()
        {
            int corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);
            //var catEntidades = _catDictionary.GetCatalog("CatColores", "0");
            var result = new SelectList(_coloresService.ObtenerColoresActivosPorCorp(corp), "IdColor", "color");
            return Json(result);
        }

        public JsonResult Marcas_Read()
        {
            var corp = 1;

            var result = new SelectList(_catMarcasVehiculosService.ObtenerMarcas(corp), "IdMarcaVehiculo", "MarcaVehiculo");
            return Json(result);
        }

        public JsonResult SubMarcas_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatSubmarcasVehiculos", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult SubMarcas_ReadPorMarca(int idMarca)
        {
            var corp = 1;

            List<CatSubmarcasVehiculosModel> vm = new List<CatSubmarcasVehiculosModel>();
            if (HttpContext.Session.GetInt32("IdMarcaVehiculo") != null || idMarca >= 0)
            {
                if (idMarca <= 0)
                {
                    vm = _catSubmarcasVehiculosService.GetSubmarcaByIDMarca(Convert.ToInt32(HttpContext.Session.GetInt32("IdMarcaVehiculo")), corp);
                    HttpContext.Session.Remove("IdMarcaVehiculo");
                }
                else
                {
                    vm = _catSubmarcasVehiculosService.GetSubmarcaByIDMarca(idMarca, corp);
                }
            }
            else
            {
                vm = _catSubmarcasVehiculosService.GetSubmarcaByIDMarca(idMarca, corp);
            }
            var result = new SelectList(vm, "IdSubmarca", "NombreSubmarca");
            return Json(result);
        }

        public JsonResult SubMarcasActive_ReadPorMarca(int idMarca)
        {
            List<CatSubmarcasVehiculosModel> vm = new List<CatSubmarcasVehiculosModel>();
            var corp = 1;

            if (HttpContext.Session.GetInt32("IdMarcaVehiculo") != null && idMarca<1)
            {
                if (idMarca <= 0)
                {
                    vm = _catSubmarcasVehiculosService.GetSubmarcaActiveByIDMarca(Convert.ToInt32(HttpContext.Session.GetInt32("IdMarcaVehiculo")), corp);
                    HttpContext.Session.Remove("IdMarcaVehiculo");
                }
                else
                {
                    vm = _catSubmarcasVehiculosService.GetSubmarcaActiveByIDMarca(idMarca, corp);
                }
            }
            else
            {
                vm = _catSubmarcasVehiculosService.GetSubmarcaActiveByIDMarca(idMarca, corp);
            }
            var result = new SelectList(vm, "IdSubmarca", "NombreSubmarca");
            return Json(result);
        }

        public JsonResult TiposVehiculo_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatTiposVehiculo", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }
        public JsonResult TipoLicencias_Drop()
        {
            var result = new SelectList(_catTipoLicenciasService.ObtenerTiposLicencia(), "idTipoLicencia", "tipoLicencia");
            return Json(result);
        }




        [HttpPost]
        public async Task<string> ajax_BuscarVehiculo([FromServices] IVehiculoPlataformaService vehiculoPlataformaService, VehiculoBusquedaModel model)
        {
			//Se busca el vehiculo en el padron RIAG, Finanzas y REPUVE
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			VehiculoModel vehiculo = await vehiculoPlataformaService.BuscarVehiculoEnPlataformas(model, corp);
            HttpContext.Session.SetInt32("IdMarcaVehiculo", vehiculo.idMarcaVehiculo);
            if (vehiculo.encontradoEn == 2)
            {
                return "VehiculoEncontradoEnRIAG";
            }
            return await this.RenderViewAsync("_Create", vehiculo, true);
        }


        [HttpPost]
        public ActionResult ajax_BuscarPersonasFiscasPagination([DataSourceRequest] DataSourceRequest request, int id = 0)
        {
            try
            {
                filterValue(request.Filters);
                Pagination pagination = new Pagination();
                pagination.PageIndex = request.Page - 1;
                pagination.PageSize = request.PageSize;
                pagination.Filter = resultValue;

                var personasFisicas = _personasService.GetAllPersonasFisicasPagination(pagination);
                request.PageSize = pagination.PageSize;
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
                var corp = 1;

                var idMarca = _catMarcasVehiculosService.obtenerIdPorMarca(marcaLimpio, corp);
                return idMarca;
            }

            return 0; // Valor predeterminado en caso de no encontrar el guión
        }

        private int ObtenerIdSubmarca(string submarca)
        {
            string[] partes = submarca.Split(new[] { '-' }, 2);
            var corp = 1;

            if (partes.Length > 1)
            {
                string submarcaLimpio = partes[1].Trim();

                var idSubmarca = _catSubmarcasVehiculosService.obtenerIdPorSubmarca(submarcaLimpio, corp);
                return idSubmarca;
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



        public IActionResult GetBuscarVehiculos([DataSourceRequest] DataSourceRequest request, VehiculoBusquedaModel model)
        {
            vehModel = model;
            return PartialView("_ListVehiculos", new List<VehiculoModel>());
        }



        [HttpPost]
        public IActionResult ajax_BuscarVehiculos([DataSourceRequest] DataSourceRequest request, VehiculoBusquedaModel model)
        {
            string listaIdsPermitidosJson = HttpContext.Session.GetString("Autorizaciones");
            List<int> listaIdsPermitidos = JsonConvert.DeserializeObject<List<int>>(listaIdsPermitidosJson);

            Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            pagination.PageSize = 10;
            pagination.Filter = resultValue;

            model = vehModel;

            var vehiculosModel = _vehiculosService.GetVehiculosPagination(model, pagination);
            VehiculoBusquedaModel vehiculoBusquedaModel = new VehiculoBusquedaModel();
            vehiculoBusquedaModel.Vehiculo = new VehiculoModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculoBusquedaModel.Vehiculo.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();
            vehiculoBusquedaModel.ListVehiculo = vehiculosModel.ToList();
            var total = 0;
            if (vehiculoBusquedaModel.ListVehiculo.Count() > 0)
                total = vehiculoBusquedaModel.ListVehiculo.ToList().FirstOrDefault().total;

            request.PageSize = 10;
            var result = new DataSourceResult()
            {
                Data = vehiculoBusquedaModel.ListVehiculo,
                Total = total
            };

            return Json(result);


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

        //public ActionResult ajax_BuscarVehiculos(VehiculoBusquedaModel model)
        //{
        //    var vehiculosModel = _vehiculosService.GetVehiculos(model);
        //    return PartialView("_ListVehiculos", vehiculosModel);
        //}



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
            var personasFisicas = _personasService.GetAllPersonasFisicas();
            return PartialView("_PersonasFisicas", personasFisicas);
        }


        [HttpPost]
        public ActionResult ajax_CrearPersonaMoral(PersonaModel Persona)
        {

            Persona.idCatTipoPersona = (int)TipoPersona.Moral;
            if (!string.IsNullOrEmpty(Persona.MoralNombre))
            {
                Persona.nombre = Persona.MoralNombre;
                Persona.RFC = Persona.moralrfc;
            }


            var IdPersonaMoral = _personasService.CreatePersonaMoral(Persona);
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);

            //_bitacoraServices.insertBitacora(IdPersonaMoral, ip, "VehiculosPersonaMoral", "Insertar", "Insert", user);
            //var personasMoralesModel = _personasService.GetAllPersonasMorales();
            object data = new { };
            _bitacoraServices.BitacoraGenerales(CodigosGeneral.C5016, data);
            var modelList = _personasService.ObterPersonaPorIDList(IdPersonaMoral); ;

            return PartialView("_ListPersonasMorales", modelList);
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
            Persona.idTipoLicencia = Persona.idTipoLicencia;
            Persona.vigenciaLicencia = Persona.vigenciaLicencia;
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
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);

            //_bitacoraServices.insertBitacora((int)Persona.idPersona, ip, "PersonaMoral", "Actualizar", "Update", user);
            object data = new { };
            _bitacoraServices.BitacoraGenerales(CodigosGeneral.C5018, data);
            return Json(new { data = personaEditada });
        }


        [HttpPost]
        public ActionResult ajax_CrearVehiculo(VehiculoModel model)
        {
            int IdVehiculo = 0;
            bool esEdicion = false;

            if (model.idVehiculo > 0)
            {
                model.idSubmarca = model.idSubmarcaUpdated;
                IdVehiculo = _vehiculosService.UpdateVehiculo(model);
                esEdicion = true;
            }
            else if (model.idVehiculo <= 0)
            {
                IdVehiculo = _vehiculosService.CreateVehiculo(model);
            }

            if (IdVehiculo != 0)
            {
                return Json(new { id = IdVehiculo, esEdicion = esEdicion });
            }
            else
            {
                return null;
            }
        }



        [HttpPost]
        public ActionResult ajax_CrearVehiculo2(VehiculoModel model)
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

        public JsonResult SubTipoServicios_Read()
        {
            var catEntidades = _catDictionary.GetCatalog("CatSubtipoServicio", "0");
            var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
            return Json(result);
        }

        public JsonResult GetSubtipoPorTipo(int idTipoServicio)
        {
            var result = new SelectList(_subtipoServicio.GetSubtipoPorTipo(idTipoServicio), "idSubTipoServicio", "subTipoServicio");
            return Json(result);
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




    }
}
