using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using Kendo.Mvc.UI;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;
using static GuanajuatoAdminUsuarios.Models.LicenciaTipoRespuesta;
using Kendo.Mvc.Extensions;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Http;

namespace GuanajuatoAdminUsuarios.Controllers
{
	[Authorize]
	public class PersonasController : BaseController
	{
		private readonly ICatDictionary _catDictionary;
		private readonly IPersonasService _personasService;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ICatEntidadesService _catEntidadesService;
		private readonly ICatMunicipiosService _catMunicipiosService;
		private readonly IBitacoraService _bitacoraServices;
		private readonly ISqlClientConnectionBD _connectionBD;
		private readonly ILicenciasService _licenciasService;
		private static BusquedaPersonaModel perModel = new BusquedaPersonaModel();
		public PersonasController(ICatDictionary catDictionary, IPersonasService personasService, IHttpClientFactory httpClientFactory, ICatEntidadesService catEntidadesService
			, ICatMunicipiosService catMunicipiosService
, IBitacoraService bitacoraServices, ISqlClientConnectionBD connectionBD, ILicenciasService licenciasService)
		{
			_catDictionary = catDictionary;
			_personasService = personasService;
			_httpClientFactory = httpClientFactory;
			_catEntidadesService = catEntidadesService;
			_catMunicipiosService = catMunicipiosService;
			_bitacoraServices = bitacoraServices;
			_connectionBD = connectionBD;
			_licenciasService = licenciasService;
		}
		public IActionResult Index()
		{
			perModel = new BusquedaPersonaModel();
			var catTipoPersona = _catDictionary.GetCatalog("CatTipoPersona", "0");
			var personaModel = new BusquedaPersonaModel();
			personaModel.ListadoPersonas = new List<PersonaModel>();

			ViewBag.CatTipoPersona = new SelectList(catTipoPersona.CatalogList, "Id", "Text");
			return View(personaModel);
		}
		public IActionResult DetallesLicencia()
		{

			return View("_DetalleLicencia");
		}
		public async Task<IActionResult> BuscarPorParametro([FromServices] ICatEntidadesService catEntidadesService, PersonaModel model, int tipoper = 0)
		{
			//buscarConductorPaginado
			// Realizar la búsqueda de personas
			//SqlConnection connection = new SqlConnection(_connectionBD.GetConnection3());
			//connection.Open();
			//SqlCommand command;
			//command = new SqlCommand(
			//           @"SELECT TOP 1 ID_PERSONA FROM PERSONAS", connection);
			//command.CommandType = System.Data.CommandType.Text;
			//var result = 0;
			//var reader = command.ExecuteReader();
			//while (reader.Read())
			//{
			//    result = (int)reader["ID_PERSONA"];
			//}

			var builder = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
			IConfiguration configuration = builder.Build();


			string UrlHost = configuration.GetSection("AppSettings").GetSection("UrlHost").Value.ToString();
			var personasList = _personasService.BusquedaPersona(model);

			if (tipoper == 1)
				personasList = personasList.Where(s => s.idCatTipoPersona == 1).ToList();

			// Verificar si se encontraron resultados en la búsqueda de personas
			if (personasList.Any())
			{
				return Json(new { encontrada = true, data = personasList, type = "Riag" });
			}

			// Si no se encontraron resultados en la búsqueda de personas, realizar la búsqueda por licencia
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
				_bitacoraServices.BitacoraWS(" Licencias", CodigosWs.C4017, parametros);

				var url = UrlHost + $"/api/Licencias/datos_generales?" + parametros;

				var httpClient = _httpClientFactory.CreateClient();
				var response = await httpClient.GetAsync(url);
				_bitacoraServices.BitacoraWS(" Licencias", CodigosWs.C4018, response);

				if (response.IsSuccessStatusCode)
				{
					//var content = await response.Content.ReadAsStringAsync();
					//LicenciaRespuestaPersona respuesta = JsonConvert.DeserializeObject<LicenciaRespuestaPersona>(content);

					var content = await response.Content.ReadAsStringAsync();
					var corp = HttpContext.Session.GetInt32("IdDependencia").Value;


					List<LicenciaPersonaDatos> respuesta = JsonConvert.DeserializeObject<List<LicenciaPersonaDatos>>(content);

					List<PersonaModel> resultado = new();

					foreach (LicenciaPersonaDatos p in respuesta)
					{
						PersonaModel pm = new();
						pm.ConvertirModeloDeLicencias(p);
						CatEntidadesModel entidad = catEntidadesService.ObtenerEntidadesByNombre(p.ESTADO_NACIMIENTO, corp);
						if (entidad != null && entidad.idEntidad > 0)
							pm.PersonaDireccion.idEntidad = entidad.idEntidad;

						pm.PersonaDireccion.calle = p.CALLE;
						pm.PersonaDireccion.colonia = p.COLONIA;
						pm.PersonaDireccion.numero = p.NUM_EXT;


						resultado.Add(pm);
					}

					for (var i = 0; i < resultado.Count(); i++)
					{
						var q = _personasService.CreatePersona(resultado[i]);
						resultado[i].idPersona = q;
					}

					return Json(new { success = true, data = resultado, type = "Licencias" });
				}
			}
			catch (Exception ex)
			{
				// En caso de errores, devolver una respuesta JSON con licencia no encontrada
				return Json(new { encontrada = false, data = personasList, message = "Ocurrió un error al obtener los datos. " + ex.Message + "; " + ex.InnerException });
			}

			// Si no se cumple la condición anterior, devolver una respuesta JSON indicando que no se encontraron resultados
			return Json(new { encontrada = false, data = personasList, message = "No se encontraron resultados." });
		}

		public IActionResult GetBuscar([DataSourceRequest] DataSourceRequest request, BusquedaPersonaModel model)
		{
			perModel = model;
			return PartialView("_ListadoPersonas", new List<PersonaModel>());
		}

		[HttpPost]
		public async Task<IActionResult> BuscarPorParametroPaginado([DataSourceRequest] DataSourceRequest request, BusquedaPersonaModel model)
		{
			var builder = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
			IConfiguration configuration = builder.Build();


			string UrlHost = configuration.GetSection("AppSettings").GetSection("UrlHost").Value.ToString();
			// Realizar la búsqueda de personas
			if (model.PersonaModel != null)
				perModel = model;
			else
				model = perModel;

			var findAll = false;
			var personas = new BusquedaPersonaModel();
			Pagination pagination = new Pagination();
			pagination.PageIndex = request.Page - 1;
			if (request.PageSize == 0)
				request.PageSize = 10;

			if (model.PersonaModel != null)
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
					//pagination.PageSize = 1000000;
					pagination.PageSize = (request.PageSize > 0) ? request.PageSize : 10;

				}
			}
			else
			{
				pagination.PageSize = (request.PageSize > 0) ? request.PageSize : 10;
			}

			//model = perModel;
			var personasList = _personasService.BusquedaPersonaPagination(model, pagination);

			// Verificar si se encontraron resultados en la búsqueda de personas
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





			string parametros = "";
			parametros += string.IsNullOrEmpty(model.PersonaModel.numeroLicenciaBusqueda) ? "" : "licencia=" + model.PersonaModel.numeroLicenciaBusqueda;
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
					LicenciaRespuestaPersona respuesta = JsonConvert.DeserializeObject<LicenciaRespuestaPersona>(content);

					return Json(new { encontrada = false, Data = respuesta.datos, tipo = respuesta.datos == null ? "sin datos" : "success", message = respuesta.mensaje });
					//return Json(respuesta);
				}
			}
			catch (Exception ex)
			{
				// En caso de errores, devolver una respuesta JSON con licencia no encontrada
				return Json(new { encontrada = false, Data = personasList, message = "Ocurrió un error al obtener los datos. " + ex.Message + "; " + ex.InnerException });
			}

			//Si no se cumple la condición anterior, devolver una respuesta JSON indicando que no se encontraron resultados
			return Json(new { encontrada = false, Data = personasList, message = "No se encontraron resultados." });
		}



		[HttpGet]
		public IActionResult ajax_ModalCrearPersona()
		{
			var catTipoPersona = _catDictionary.GetCatalog("CatTipoPersona", "0");
			var catTipoLicencia = _catDictionary.GetCatalog("CatTipoLicencia", "0");
			// var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
			var catGeneros = _catDictionary.GetCatalog("CatGeneros", "0");
			// var catMunicipios = (_catMunicipiosService.GetMunicipiosPorEntidad(entidadDDlValue), "IdMunicipio", "Municipio");

			// ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
			ViewBag.CatGeneros = new SelectList(catGeneros.CatalogList, "Id", "Text");
			//ViewBag.CatEntidades = new SelectList(catEntidades.CatalogList, "Id", "Text");
			ViewBag.CatTipoPersona = new SelectList(catTipoPersona.CatalogList, "Id", "Text");
			ViewBag.CatTipoLicencia = new SelectList(catTipoLicencia.CatalogList, "Id", "Text");
			return PartialView("_CrearPersona", new PersonaModel());
		}

		public JsonResult Tiempo_Vigencia_Drop()
		{
			var result = new SelectList(_personasService.ObtenerVigencias(), "idVigencia", "vigencia");
			return Json(result);
		}
		public JsonResult Entidades_Drop()
		{
			var corp = 1;

			var result = new SelectList(_catEntidadesService.ObtenerEntidades(corp), "idEntidad", "nombreEntidad");
			return Json(result);
		}
		public JsonResult Municipios_Drop(int entidadDDlValue)
		{
			var corp = 1;

			var result = new SelectList(_catMunicipiosService.GetMunicipiosPorEntidad(entidadDDlValue, corp), "IdMunicipio", "Municipio");
			return Json(result);
		}

		[HttpPost]
		public IActionResult ajax_CrearPersona(PersonaModel model)
		{
			//var model = json.ToObject<Gruas2Model>();
			//var errors = ModelState.Values.Select(s => s.Errors);
			//if (ModelState.IsValid)
			//{
			int id = _personasService.CreatePersona(model);
			var ip = HttpContext.Connection.RemoteIpAddress.ToString();
			var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
			//BITACORA
			//_bitacoraServices.insertBitacora(id, ip, "Personas_PersonaMoral", "Insertar", "Insert", user);
			//model.PersonaDireccion.idPersona = id;
			//int idDireccion = _personasService.CreatePersonaDireccion(model.PersonaDireccion);
			object data = new { };
			_bitacoraServices.BitacoraGenerales(CodigosGeneral.C5016, data);

			var modelList = _personasService.GetPersonaById(id);
			return Json(modelList);
			//}
			//return RedirectToAction("Index");
		}


		[HttpGet]
		public IActionResult ajax_ModalEditarPersona(int id)
		{
			var model = _personasService.GetPersonaById(id);
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
			return PartialView("~/Views/Personas/_EditarPersona.cshtml", model);
		}
		public IActionResult ajax_ModalEditarPersonaFisica(int id)
		{
			var model = _personasService.GetPersonaById(id);
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
			return PartialView("~/Views/Personas/_EditarPersonaFisica.cshtml", model);
		}
		[HttpPost]
		public IActionResult ajax_EditarPersona(PersonaModel model)
		{
			//var model = json.ToObject<Gruas2Model>();

			if (model.PersonaDireccion.idPersona == null || model.PersonaDireccion.idPersona <= 0)
			{
				model.PersonaDireccion.idPersona = model.idPersona;
				int idDireccion = _personasService.CreatePersonaDireccion(model.PersonaDireccion);
			}
			else
			{
				int idDireccion = _personasService.UpdatePersonaDireccion(model.PersonaDireccion);
			}
			int id = _personasService.UpdatePersona(model);
			var modelList = _personasService.GetPersonaById((int)model.idPersona);
			var ip = HttpContext.Connection.RemoteIpAddress.ToString();
			var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
			//BITACORA
			//_bitacoraServices.insertBitacora((int)model.idPersona, ip, "Personas_PersonaMoral", "Actualizar", "Update", user);
			//var listPadronGruas = _concesionariosService.GetAllConcesionarios();
			object data = new { };
			_bitacoraServices.BitacoraGenerales(CodigosGeneral.C5018, data);

			return Json(new { data = modelList });
		}




		//  [HttpPost]

		//public ActionResult GuardaDesdeServicio(PersonaModel personaDatos)
		public async Task<IActionResult> GuardaDesdeServicio(string nombre,
																string apellidoPaterno,
																string apellidoMaterno,
																string CURP,
																string RFC,
																string numeroLicencia,
																string tipoLicencia,
																string idGenero,
																DateTime fechaNacimiento,
																DateTime fechaVigencia,
																int idMunicipio,
																string municipio,
																string codigoPostal,
																string entidad,
																string telefono,
																string correo,
																string calle,
																string numero,
																string colonia)
		{
			try
			{
				LicenciaPersonaDatos personaDatos = new LicenciaPersonaDatos();
				personaDatos.NOMBRE = nombre;
				personaDatos.PRIMER_APELLIDO = apellidoPaterno;
				personaDatos.SEGUNDO_APELLIDO = apellidoMaterno;
				personaDatos.CURP = CURP;
				personaDatos.RFC = RFC;
				personaDatos.NUM_LICENCIA = numeroLicencia;
				personaDatos.ID_TIPO_LICENCIA = Convert.ToInt32(tipoLicencia);
				personaDatos.ID_GENERO = Convert.ToInt32(idGenero);
				personaDatos.FECHA_NACIMIENTO = fechaNacimiento;
				personaDatos.FECHA_TERMINO_VIGENCIA = fechaVigencia;
				personaDatos.ID_MUNICIPIO = idMunicipio;
				personaDatos.MUNICIPIO = municipio;
				personaDatos.CP = codigoPostal;
				personaDatos.ESTADO_NACIMIENTO = entidad;
				personaDatos.TELEFONO1 = telefono;
				personaDatos.EMAIL = correo;
				personaDatos.CALLE = calle;
				personaDatos.NUM_EXT = numero;
				personaDatos.COLONIA = colonia;


				int idPersona = _personasService.InsertarDesdeServicio(personaDatos);
				//var datosTabla = _personasService.BuscarPersonaSoloLicencia(personaDatos.NUM_LICENCIA);
				var datosTabla = _personasService.GetPersonaById(idPersona);

				//BITACORA
				var ip = HttpContext.Connection.RemoteIpAddress.ToString();
				var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
				//_bitacoraServices.insertBitacora(idPersona, ip, "Personas_DesdeServicio", "Insertar", "insert", user);
				object data = new { };
				_bitacoraServices.BitacoraGenerales(CodigosGeneral.C5012, data);

				return Json(new { data = datosTabla });
				//return Json(new { data = "hola" });

			}
			catch (Exception ex)
			{
				// Maneja el error de manera adecuada
				return Json(new { error = "Error al guardar en la base de datos: " + ex.Message });
			}
		}

		public async Task<IActionResult> TestSesion()
		{
			return Json("session activa");
		}



		public async Task<IActionResult> PersonasEnLicencias([DataSourceRequest] DataSourceRequest request, BusquedaPersonaModel model)
		{
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			//SqlConnection connection = new SqlConnection(_connectionBD.GetConnection3());
			//connection.Open();
			//SqlCommand command;

			//command = new SqlCommand(
			//           @"SELECT TOP 1 ID_PERSONA FROM PERSONAS", connection);
			//command.CommandType = System.Data.CommandType.Text;
			//var result = 0;
			//var reader = command.ExecuteReader();
			//while (reader.Read())
			//{
			//    result = (int)reader["ID_PERSONA"];
			//}


			//            var builder = new ConfigurationBuilder()
			//.SetBasePath(Directory.GetCurrentDirectory())
			//.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
			//            IConfiguration configuration = builder.Build();


			//            string UrlHost = configuration.GetSection("AppSettings").GetSection("UrlHost").Value.ToString();


			//            string parametros = "";
			//            parametros += string.IsNullOrEmpty(model.PersonaModel.numeroLicenciaBusqueda) ? "" : "licencia=" + model.PersonaModel.numeroLicenciaBusqueda + "&";
			//            parametros += string.IsNullOrEmpty(model.PersonaModel.CURPBusqueda) ? "" : "curp=" + model.PersonaModel.CURPBusqueda + "&";
			//            parametros += string.IsNullOrEmpty(model.PersonaModel.RFCBusqueda) ? "" : "rfc=" + model.PersonaModel.RFCBusqueda + "&";
			//            parametros += string.IsNullOrEmpty(model.PersonaModel.nombreBusqueda) ? "" : "nombre=" + model.PersonaModel.nombreBusqueda + "&";
			//            parametros += string.IsNullOrEmpty(model.PersonaModel.apellidoPaternoBusqueda) ? "" : "primer_apellido=" + model.PersonaModel.apellidoPaternoBusqueda + "&";
			//            parametros += string.IsNullOrEmpty(model.PersonaModel.apellidoMaternoBusqueda) ? "" : "segundo_apellido=" + model.PersonaModel.apellidoMaternoBusqueda;
			//            string ultimo = parametros.Substring(parametros.Length - 1);
			//            if (ultimo.Equals("&"))
			//                parametros = parametros.Substring(0, parametros.Length - 1);

			try
			{
				//string urlServ = Request.GetDisplayUrl();
				//Uri uri = new Uri(urlServ);
				//string requested = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

				//var url = UrlHost + $"/api/Licencias/datos_generales?" + parametros;

				//var httpClient = _httpClientFactory.CreateClient();
				//var response = await httpClient.GetAsync(url);

				var licencias = _licenciasService.ObtenerDatosPersona(model.PersonaModel.numeroLicenciaBusqueda,
																		model.PersonaModel.CURPBusqueda,
																		model.PersonaModel.RFCBusqueda,
																		model.PersonaModel.nombreBusqueda,
																		model.PersonaModel.apellidoPaternoBusqueda,
																		model.PersonaModel.apellidoMaternoBusqueda);


				if (licencias.Count > 0)
				{
                  
                   
					var idEntidadLic = 0;
					var idMunicipioLic = 0;
                    //var content = await response.Content.ReadAsStringAsync();
                    //    LicenciaRespuestaPersona respuesta = JsonConvert.DeserializeObject<LicenciaRespuestaPersona>(content);
                    // var personasEncontradas = content.
                    //  return Json(new { encontrada = false, Data = respuesta.datos, tipo = respuesta.datos == null ? "sin datos" : "success", message = respuesta.mensaje });


					//List<LicenciaPersonaDatos> respuesta = JsonConvert.DeserializeObject<List<LicenciaPersonaDatos>>(content);

					List<PersonaModel> pEncontradas = new List<PersonaModel>();
					foreach (LicenciaPersonaDatos pivote in licencias)
					{
						PersonaModel pm = new PersonaModel();
						pm.PersonaDireccion = new PersonaDireccionModel();
						//pm.idPersona = (int)pivote.ID_PERSONA;
						pm.nombre = pivote.NOMBRE;
						pm.apellidoPaterno = pivote.PRIMER_APELLIDO;
						pm.apellidoMaterno = pivote.SEGUNDO_APELLIDO;
						pm.CURP = pivote.CURP;
						pm.RFC = pivote.RFC;
						pm.numeroLicencia = pivote.NUM_LICENCIA;
						pm.tipoLicencia = pivote.TIPOLICENCIA;
						pm.idGenero = pivote.ID_GENERO == null ? 0 : (int)pivote.ID_GENERO;
						pm.fechaNacimiento = pivote.FECHA_NACIMIENTO;
						pm.vigenciaLicencia = pivote.FECHA_TERMINO_VIGENCIA;
						pm.idGenero = pivote.ID_GENERO == null ? 1 : Convert.ToInt16(pivote.ID_GENERO);
						CatEntidadesModel entidad = _catEntidadesService.ObtenerEntidadesByNombre(pivote.ESTADO_NACIMIENTO, corp);
						if (entidad != null && entidad.idEntidad > 0)
						{
							idEntidadLic = entidad.idEntidad;
						}
                        CatMunicipiosModel municipio = _catMunicipiosService.ObtenerMunicipiosByNombre(pivote.MUNICIPIO, corp);
                        if (municipio != null && municipio.IdMunicipio > 0)
                        {
                            idMunicipioLic = municipio.IdMunicipio;
                        }
                        pm.PersonaDireccion = new PersonaDireccionModel
						{
							idMunicipio = idMunicipioLic,
							municipio = pivote.MUNICIPIO,
							codigoPostal = pivote.CP,
							entidad = pivote.ESTADO_NACIMIENTO,
							telefono = pivote.TELEFONO1,
							correo = pivote.EMAIL,
							idEntidad = idEntidadLic,
							calle = pivote.CALLE,
							numero = pivote.NUM_EXT,
							colonia = pivote.COLONIA
						};

						pEncontradas.Add(pm);
					}

             

                    return Json(pEncontradas);
				}
			}
			catch (Exception ex)
			{
				List<PersonaModel> pEncontradas = new List<PersonaModel>();
				// En caso de errores, devolver una respuesta JSON con licencia no encontrada
				return Json(new { encontrada = false, Data = "", message = "Ocurrió un error al obtener los datos. " + ex.Message + "; " + ex.InnerException });
			}

			//Si no se cumple la condición anterior, devolver una respuesta JSON indicando que no se encontraron resultados
			return Json(new { encontrada = false, Data = "", message = "No se encontraron resultados." });
		}





		public async Task<IActionResult> PersonasEnLicencias2([DataSourceRequest] DataSourceRequest request, BusquedaPersonaModel model)
		{

            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            try
            {


				var licencias = _licenciasService.ObtenerDatosPersona(model.numeroLicenciaBusqueda,
																		model.CURPBusqueda,
																		model.RFCBusqueda,
																		model.nombreBusqueda,
																		model.apellidoPaternoBusqueda,
																		model.apellidoMaternoBusqueda);
                int idPersona = 0;

                foreach (var licencia in licencias)
                {
                    idPersona = _personasService.InsertarDesdeServicio(licencia);
                }


                if (licencias.Count > 0)
				{
					var idEntidadLic = 0;
					//var content = await response.Content.ReadAsStringAsync();
					//    LicenciaRespuestaPersona respuesta = JsonConvert.DeserializeObject<LicenciaRespuestaPersona>(content);
					// var personasEncontradas = content.
					//  return Json(new { encontrada = false, Data = respuesta.datos, tipo = respuesta.datos == null ? "sin datos" : "success", message = respuesta.mensaje });


					//List<LicenciaPersonaDatos> respuesta = JsonConvert.DeserializeObject<List<LicenciaPersonaDatos>>(content);

					List<PersonaModel> pEncontradas = new List<PersonaModel>();
					foreach (LicenciaPersonaDatos pivote in licencias)
					{
						PersonaModel pm = new PersonaModel();
						pm.PersonaDireccion = new PersonaDireccionModel();
						pm.idPersona = idPersona;
						pm.IdPersona = idPersona;
                        pm.nombre = pivote.NOMBRE;
						pm.apellidoPaterno = pivote.PRIMER_APELLIDO;
						pm.apellidoMaterno = pivote.SEGUNDO_APELLIDO;
						pm.CURP = pivote.CURP;
						pm.RFC = pivote.RFC;
						pm.numeroLicencia = pivote.NUM_LICENCIA;
						pm.tipoLicencia = pivote.TIPOLICENCIA;
						pm.idGenero = pivote.ID_GENERO == null ? 0 : (int)pivote.ID_GENERO;
						pm.fechaNacimiento = pivote.FECHA_NACIMIENTO;
						pm.vigenciaLicencia = pivote.FECHA_TERMINO_VIGENCIA;
						pm.idGenero = pivote.ID_GENERO == null ? 1 : Convert.ToInt16(pivote.ID_GENERO);
						CatEntidadesModel entidad = _catEntidadesService.ObtenerEntidadesByNombre(pivote.ESTADO_NACIMIENTO, corp);
						if (entidad != null && entidad.idEntidad > 0)
						{
							idEntidadLic = entidad.idEntidad;
						}
						pm.PersonaDireccion = new PersonaDireccionModel
						{
							idMunicipio = pivote.ID_MUNICIPIO,
							municipio = pivote.MUNICIPIO,
							codigoPostal = pivote.CP,
							entidad = pivote.ESTADO_NACIMIENTO,
							telefono = pivote.TELEFONO1,
							correo = pivote.EMAIL,
							idEntidad = idEntidadLic,
							calle = pivote.CALLE,
							numero = pivote.NUM_EXT,
							colonia = pivote.COLONIA
						};

						pEncontradas.Add(pm);
					}



					return Json(pEncontradas);
				}
			}
			catch (Exception ex)
			{
				List<PersonaModel> pEncontradas = new List<PersonaModel>();
				// En caso de errores, devolver una respuesta JSON con licencia no encontrada
				return Json(new { encontrada = false, Data = "", message = "Ocurrió un error al obtener los datos. " + ex.Message + "; " + ex.InnerException });
			}

			//Si no se cumple la condición anterior, devolver una respuesta JSON indicando que no se encontraron resultados
			return Json(new { encontrada = false, Data = "", message = "No se encontraron resultados." });
		}
	}
    }

