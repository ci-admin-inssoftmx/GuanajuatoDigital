/*
 * Descripción: Servicio de Vehiculo relacionado con algun servicio de REPUVE o Finanzas
 * Proyecto: RIAG
 * Fecha de creación: Wednesday, February 21st 2024 9:56:46 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Sat Mar 02 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using static GuanajuatoAdminUsuarios.RESTModels.CotejarDatosResponseModel;

namespace GuanajuatoAdminUsuarios.Services
{
    public partial class VehiculoPlataformaService : IVehiculoPlataformaService
    {
        #region Variables
        private readonly IRepuveService _repuveService;
        private readonly ICatMunicipiosService _municipioService;
        private readonly ICatEntidadesService _entidadService;
        private readonly IColores _colorService;
        private readonly ICatMarcasVehiculosService _marcaVehiculoService;
        private readonly ICatSubmarcasVehiculosService _submarcaVehiculoService;
        private readonly ICatDictionary _catDictionary;
        private readonly IVehiculosService _vehiculoService;
		private readonly IBitacoraService _bitacoraServices;
		private readonly ICotejarDocumentosClientService _cotejarDocumentosClientService;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor http;
        private readonly IAppSettingsService _appSettingsService;

        [GeneratedRegex("[0-9-]")]
        private static partial Regex MyRegex();
        #endregion
        #region Constructor
        public VehiculoPlataformaService(IRepuveService repuveService, ICatEntidadesService entidadService, ICatMunicipiosService municipioService, IColores colorService,
        ICatMarcasVehiculosService marcaVehiculoService, ICatSubmarcasVehiculosService submarcaVehiculoService, ICatDictionary catDictionary, IOptions<AppSettings> appSettings,
        IVehiculosService vehiculoService, IBitacoraService bitacoraServices, ICotejarDocumentosClientService cotejarDocumentosClientService,IHttpContextAccessor _http,IAppSettingsService appSettingsService
)
        {
            _repuveService = repuveService;
            _entidadService = entidadService;
            _municipioService = municipioService;
            _colorService = colorService;
            _marcaVehiculoService = marcaVehiculoService;
            _submarcaVehiculoService = submarcaVehiculoService;
            _catDictionary = catDictionary;
            _appSettings = appSettings.Value;
            _vehiculoService = vehiculoService;
            _bitacoraServices = bitacoraServices;
			_cotejarDocumentosClientService = cotejarDocumentosClientService;
            http = _http;
            _appSettingsService = appSettingsService;

        }
        #endregion

        #region BusquedaVehiculoEnplataformas
        /// <summary>
        /// Busca un vehiculo en RIAG, finanzas o REPUVE
        /// </summary>
        /// <param name="busquedaModel"></param>
        /// <returns></returns>
        public async Task<VehiculoModel> BuscarVehiculoEnPlataformas(VehiculoBusquedaModel busquedaModel, int corp)
        {
            var existError = false;
            var endPointName = "CotejarDatosEndPoint";

			try
			{
                var request = JsonConvert.SerializeObject(busquedaModel);
                // Logger.Debug("Infracciones - ajax_BuscarVehiculo - Request:" + request);

                //Se realiza la consulta para validar si el vehiculo tiene reporte de robo
                RepuveConsgralRequestModel repuveGralModel = new(busquedaModel.PlacasBusqueda, busquedaModel.SerieBusqueda);

                // Logger.Debug("Infracciones - ajax_BuscarVehiculo - ValidarRobo ");
                RepuveRoboModel repuveRoboModel = new();

                try
                {
                    repuveRoboModel = await ValidarRoboRepuve(repuveGralModel);
                }
                catch (Exception e)
                {
                    // Logger.Error("Ocurrió un error al consultar robados en REPUVE:" + e);
                }



                var buscarEnServicios = _appSettingsService.VerificarActivo(endPointName,corp);
                
                //Logger.Debug("Infracciones - ajax_BuscarVehiculo - GetVehiculoToAnexo");

                var vehiculoModel = _vehiculoService.GetVehiculoToAnexo(busquedaModel);

                //Se asigna el resutado de reporte de robo
                vehiculoModel.RepuveRobo = repuveRoboModel;
                vehiculoModel.ReporteRobo = repuveRoboModel.EsRobado;

                vehiculoModel.idSubmarcaUpdated = vehiculoModel.idSubmarca;
                vehiculoModel.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel
                {
                    PersonasMorales = new List<PersonaModel>()
                };
                if (vehiculoModel.idVehiculo > 0)
                {
                    return vehiculoModel;
                }

                if (buscarEnServicios && !string.IsNullOrEmpty(busquedaModel.PlacasBusqueda))
                {
                    //Se busca en el padrón de vehiculos de Finanzas
                    CotejarDatosRequestModel cotejarDatosRequestModel = new CotejarDatosRequestModel
                    {
                        Tp_folio = "4",
                        Folio = busquedaModel.PlacasBusqueda.ToUpper(),
                        tp_consulta = "3"
                    };

					// Logger.Debug("Infracciones - ajax_BuscarVehiculo - CotejarDatos");
					_bitacoraServices.BitacoraWS("CotejarDatos:", CodigosWs.C4019, cotejarDatosRequestModel);

					var result = _cotejarDocumentosClientService.CotejarDatos(cotejarDatosRequestModel, endPointName);
                    _bitacoraServices.BitacoraWS("CotejarDatos:", CodigosWs.C4020, result);

                    existError = result == null;

                    if (result != null &&  result.MT_CotejarDatos_res != null && result.MT_CotejarDatos_res.Es_mensaje != null && result.MT_CotejarDatos_res.Es_mensaje.TpMens.ToString().Equals("I", StringComparison.OrdinalIgnoreCase))
                    {
                        // Logger.Debug("Infracciones - ajax_BuscarVehiculo - GetVEiculoModelFromFinanzas - Response - " + JsonConvert.SerializeObject(result));
                        vehiculoModel = await GetVehiculoModelFromFinanzas(result);


                        if (!vehiculoModel.serie.Contains("SINSERIE"))
                        {
                            

                            var idVehiculo = _vehiculoService.BuscarvehiculoSerie(vehiculoModel.serie);

                            if (idVehiculo > 0)
                            {
                                vehiculoModel.idVehiculo = idVehiculo;
                                vehiculoModel.placas = busquedaModel.PlacasBusqueda;
                            }
                                

                            //Se asigna el objeto de la consulta de robado a repuve
                            vehiculoModel.RepuveRobo = repuveRoboModel;
                            vehiculoModel.ReporteRobo = repuveRoboModel.EsRobado;

                            return vehiculoModel;
                        }

                    }
                }
				_bitacoraServices.BitacoraWS("CotejarDatos :", CodigosWs.C4020, "");
                var repuveEndPoint = "RepuveConsgral";
				var repuveActive = _appSettingsService.VerificarActivo(repuveEndPoint,corp);

                //Se busca en REPUVE
                if (repuveActive)
                {
					//Logger.Debug("Infracciones - ajax_BuscarVehiculo - ConsultaGeneral - REPUVE");

					RepuveConsgralResponseModel repuveConsGralResponse = new();
					_bitacoraServices.BitacoraWS(" REPUVE :", CodigosWs.C4013, "");

					try
					{
                        repuveConsGralResponse = _repuveService.ConsultaGeneral(repuveGralModel,corp).FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        //Logger.Error("Ocurrió un error al consultar vehículo en REPUVE:" + ex);
                    }

                    //Logger.Debug(" - Response - " + JsonConvert.SerializeObject(repuveConsGralResponse));
                    //Se obtiene el objeto con los datos encontrados en REPUVE
                    var vehiculo = GetVehiculoModelFromRepuve(repuveConsGralResponse, busquedaModel, repuveRoboModel);

                    if(existError)
                    vehiculo.ErrorFinanzas = "Ocurrio un error al consultar el padron estatal";


                    if( string.IsNullOrEmpty(busquedaModel.SerieBusqueda))
                    {
                        var idVehiculo = _vehiculoService.BuscarvehiculoSerie(vehiculo.serie);
                        if (idVehiculo > 0)
                        {
                            vehiculo.idVehiculo = idVehiculo;
                            vehiculo.placas = busquedaModel.PlacasBusqueda;
                        }
                    }


                    _bitacoraServices.BitacoraWS("REPUVE ", CodigosWs.C4014, "");

					return vehiculo;

                }
                else
                {
					vehiculoModel.ServicioDesactivado = true; 
				}


                return vehiculoModel;
            }
            catch (Exception ex)
            {
                // Logger.Error("Infracciones - ajax_BuscarVehiculo: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region Catalogos y Generacion de Objeto Vehiculo Finanzas y REPUVE
        /// <summary>
        /// Valida si un vehiculo tiene reporte de robo en REPUVE
        /// </summary>
        /// <param name="repuveGralModel"></param>
        /// <returns></returns>

        public async Task<RepuveRoboModel> ValidarRoboRepuve(RepuveConsgralRequestModel repuveGralModel)
        {
			_bitacoraServices.BitacoraWS("4015-Se inicia la invocación al WS REPUVE Robo:", CodigosWs.C4015, "");
			var repuveConsRoboResponse = (await _repuveService.ConsultaRobo(repuveGralModel))?.FirstOrDefault() ?? new RepuveRoboModel();
			_bitacoraServices.BitacoraWS("4016-Se finaliza la generación del WS REPUVE Robo:", CodigosWs.C4016, "");

			return repuveConsRoboResponse;
        }


        /// <summary>
        /// Regresa objeto vehiculo a partir de un objeto de finanzas
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public async Task<VehiculoModel> GetVehiculoModelFromFinanzas(RootCotejarDatosRes result)
        {
			var corp = http.HttpContext.Session.GetInt32("IdDependencia").Value;

			var vehiculoEncontradoData = result.MT_CotejarDatos_res.tb_vehiculo[0];
            var vehiculoDireccionData = result.MT_CotejarDatos_res.tb_direccion[0];
            var vehiculoInterlocutorData = result.MT_CotejarDatos_res;
            var idMunicipio = !string.IsNullOrEmpty(vehiculoDireccionData.municipio)
                  ? _municipioService.obtenerIdPorNombre(vehiculoDireccionData.municipio,corp)
                  : 0;

            var idEntidad = !string.IsNullOrEmpty(vehiculoDireccionData.entidadreg)
                ? _entidadService.obtenerIdPorEntidad(vehiculoDireccionData.entidadreg,corp)
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
            var generoBool = vehiculoInterlocutorData.es_per_fisica?.sexo == "2";

            var idTipo = !string.IsNullOrEmpty(vehiculoEncontradoData.categoria)
             ? ObtenerIdTipoVehiculo(vehiculoEncontradoData.categoria)
             : 0;
            var idTipoServicio = !string.IsNullOrEmpty(vehiculoEncontradoData.servicio)
                ? ObtenerIdTipoServicio(vehiculoEncontradoData.servicio)
                : 0;
            var vehiculo = new VehiculoModel
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
                PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel
                {
                    PersonasMorales = new List<PersonaModel>()
                },
                //Se establece el origen de datos
                origenDatos = "Padrón Estatal"
            };



            vehiculo.Persona = new PersonaModel
            {
                nombreFisico = vehiculoInterlocutorData.es_per_fisica?.Nombre,
                apellidoPaternoFisico = vehiculoInterlocutorData.es_per_fisica?.Ape_paterno,
                apellidoMaternoFisico = vehiculoInterlocutorData.es_per_fisica?.Ape_materno,
                fechaNacimiento = vehiculoInterlocutorData.es_per_fisica?.Fecha_nacimiento,
                CURPFisico = vehiculoInterlocutorData.es_per_fisica?.Nro_curp,
                generoBool = generoBool,
                nombre = vehiculoInterlocutorData.es_per_moral != null ? vehiculoInterlocutorData.es_per_moral.name_org1 : vehiculoInterlocutorData.es_per_fisica.Nombre,
                RFC = vehiculoInterlocutorData.Nro_rfc,
                apellidoPaterno = vehiculoInterlocutorData.es_per_fisica?.Ape_paterno,
                apellidoMaterno = vehiculoInterlocutorData.es_per_fisica?.Ape_materno,
                CURP = vehiculoInterlocutorData.es_per_fisica?.Nro_curp,
                Origen = "PADRÓN ESTATAL",
                idCatTipoPersona = vehiculoInterlocutorData.es_per_fisica != null ? 1 : 2,

                PersonaDireccion = new PersonaDireccionModel
                {
                    telefono = telefonoValido.ToString(),
                    telefonoFisico = vehiculoInterlocutorData.es_per_fisica != null ? telefonoValido.ToString() : null,
                    colonia = vehiculoDireccionData.colonia,
                    coloniaFisico = vehiculoInterlocutorData.es_per_fisica != null ? vehiculoDireccionData.colonia : null,
                    calle = vehiculoDireccionData.calle,
                    calleFisico = vehiculoInterlocutorData.es_per_fisica != null ? vehiculoDireccionData.calle : null,
                    numero = vehiculoDireccionData.nro_exterior,
                    numeroFisico = vehiculoDireccionData.nro_exterior,
                    idMunicipio = idMunicipio,
                    idMunicipioFisico = vehiculoInterlocutorData.es_per_fisica != null ? idMunicipio : null,
                    idMunicipioMoral = vehiculoInterlocutorData.es_per_moral != null ? idMunicipio : null,
                    idEntidad = idEntidad,
                    idEntidadFisico = vehiculoInterlocutorData.es_per_fisica != null ? idEntidad : null,
                }
            };



            return vehiculo;

        }

        /// <summary>
        /// Regresa un objeto del modelo de vehiculo a partir del resultado de REPUVE
        /// </summary>
        /// <param name="repuveResultado"></param>
        /// <param name="busquedaModel"></param>
        /// <param name="repuveRoboModel"></param>
        /// <returns></returns>
        private VehiculoModel GetVehiculoModelFromRepuve(RepuveConsgralResponseModel repuveResultado, VehiculoBusquedaModel busquedaModel, RepuveRoboModel repuveRoboModel)
        {
            var idEntidad = !string.IsNullOrEmpty(repuveResultado.entidad_expide)
                         ? ObtenerIdEntidadRepuve(repuveResultado.entidad_expide)
                         : 0;
            var idColor = !string.IsNullOrEmpty(repuveResultado.color)
                ? ObtenerIdColor(repuveResultado.color)
                : 0;

            var idMarca = !string.IsNullOrEmpty(repuveResultado.marca_padron)
                ? ObtenerIdMarcaRepuve(repuveResultado.marca_padron)
                : 0;

            var idSubmarca = !string.IsNullOrEmpty(repuveResultado.submarca_padron)
                ? ObtenerIdSubmarcaRepuve(repuveResultado.submarca_padron)
                : 0;
            var submarcaLimpio = idSubmarca;

            var idTipo = !string.IsNullOrEmpty(repuveResultado.tipo_vehiculo_padron)
             ? ObtenerIdTipoVehiculo(repuveResultado.tipo_vehiculo_padron)
             : 0;
            var idTipoServicio = !string.IsNullOrEmpty(repuveResultado.tipo_uso_padron)
            ? ObtenerIdTipoServicioRepuve(repuveResultado.tipo_uso_padron)
            : 0;

            var vehiculoEncontrado = new VehiculoModel
            {
                placas = repuveResultado.placa.IsNullOrEmpty() ? repuveRoboModel.Placa : repuveResultado.placa,
                serie = repuveResultado.niv_padron.IsNullOrEmpty() ? repuveRoboModel.Niv : repuveResultado.niv_padron,
                // numeroEconomico = repuveConsGralResponse.tnia,
                motor = repuveResultado.motor,
                //otros = repuveConsGralResponse.
                idColor = idColor,
                idEntidad = idEntidad,
                idMarcaVehiculo = idMarca,
                idSubmarca = idSubmarca,
                //submarca = submarcaLimpio,
                idTipoVehiculo = idTipo,
                modelo = repuveResultado.modelo,
                idCatTipoServicio = idTipoServicio,
                Persona = new PersonaModel(),
                PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel(),
                RepuveRobo = repuveRoboModel,
                ReporteRobo = repuveRoboModel.EsRobado,
                ErrorConsultaRepuve = !repuveResultado.estatus.IsCaseInsensitiveEqual(RepuveConsgralResponseModel.CONSULTA_CORRECTA)
            };

            //Se establece el origen de datos
            vehiculoEncontrado.origenDatos = string.IsNullOrEmpty(vehiculoEncontrado.placas) ? null : "REPUVE";

            vehiculoEncontrado.placas ??= busquedaModel.PlacasBusqueda;
            vehiculoEncontrado.serie ??= busquedaModel.SerieBusqueda;

            return vehiculoEncontrado;
        }

        private int ObtenerIdMarcaRepuve(string marca)
        {

            string marcaLimpio = marca.Trim();
			var corp = http.HttpContext.Session.GetInt32("IdDependencia").Value;

			var idMarca = _marcaVehiculoService.obtenerIdPorMarca(marcaLimpio,corp);
            return idMarca;
        }
        private static string ObtenerSubmarca(string submarca)
        {
            string[] partes = submarca.Split(new[] { '-' }, 2);

            if (partes.Length > 1)
            {
                string submarcaLimpio = partes[1].Trim();

                return submarcaLimpio;
            }

            return "NA"; // Valor predeterminado en caso de no encontrar el guión
        }

        private int ObtenerIdSubmarcaRepuve(string submarca)
        {
            var corp = http.HttpContext.Session.GetInt32("IdDependencia").Value;

            string submarcaLimpio = submarca.Trim();

            var idMarca = _submarcaVehiculoService.obtenerIdPorSubmarca(submarcaLimpio, corp);
            return idMarca;


        }

        private static string RemoveDiacritics(string text)
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

        private int ObtenerIdEntidadRepuve(string entidad)
        {
            int idEntidad = 0;
            var Entidad = _catDictionary.GetCatalog("CatEntidades", "0");
            idEntidad = Entidad.CatalogList
                .Where(w => RemoveDiacritics(w.Text.ToLower()).Contains(RemoveDiacritics(entidad.ToLower())))
                .Select(s => s.Id)
                .FirstOrDefault();
            return idEntidad;
        }

        private int ObtenerIdColor(string color)
        {
            string colorLimpio = MyRegex().Replace(color, "").Trim();
            var idColor = _colorService.obtenerIdPorColor(colorLimpio);
            return idColor;
        }
        private int ObtenerIdMarca(string marca)
        {
            string[] partes = marca.Split(new[] { '-' }, 2);

            if (partes.Length > 1)
            {
                string marcaLimpio = partes[1].Trim();
				var corp = http.HttpContext.Session.GetInt32("IdDependencia").Value;

				var idMarca = _marcaVehiculoService.obtenerIdPorMarca(marcaLimpio,corp);
                return idMarca;
            }

            return 0; // Valor predeterminado en caso de no encontrar el guión
        }

        private int ObtenerIdSubmarca(string submarca)
        {
            string[] partes = submarca.Split(new[] { '-' }, 2);
            var corp = http.HttpContext.Session.GetInt32("IdDependencia").Value;


            if (partes.Length > 1)
            {
                string submarcaLimpio = partes[1].Trim();

                var idSubmarca = _submarcaVehiculoService.obtenerIdPorSubmarca(submarcaLimpio, corp);
                return idSubmarca;
            }

            return 0; // Valor predeterminado en caso de no encontrar el guión
        }
        private int ObtenerIdTipoVehiculo(string categoria)
        {

            int idTipo = 0;
            var tipoVehiculo = _catDictionary.GetCatalog("CatTiposVehiculo", "0");
            idTipo = tipoVehiculo.CatalogList.Where(w => categoria.ToLower().Contains(w.Text.ToLower())).Select(s => s.Id).FirstOrDefault();
            return idTipo;

        }
        private int ObtenerIdTipoServicioRepuve(string servicio)
        {
            int TipoServicio = 0;

            var Tipo = _catDictionary.GetCatalog("CatTipoServicio", "0");

            TipoServicio = Tipo.CatalogList.Where(w => servicio.ToLower().Contains(w.Text.ToLower())).Select(s => s.Id).FirstOrDefault();

            return TipoServicio;
        }
        private int ObtenerIdTipoServicio(string servicio)
        {
            int servicioNumero = int.Parse(servicio.TrimStart('0'));
            var idTipoVehiculo = _catDictionary.GetCatalog("CatTipoServicio", "0");

            var tipoServicio = idTipoVehiculo.CatalogList.FirstOrDefault(item => item.Id == servicioNumero)?.Id;

            return (int)tipoServicio;
        }

        private static bool ConvertirBool(string carga)
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
            return cargaBool;
        }

        private static long LimpiarValorTelefono(string telefono)
        {
            telefono = telefono.Replace(" ", "");
            if (long.TryParse(telefono, out long telefonoValido))
            {
                return telefonoValido;
            }
            else
            {
                return 0; // O algún otro valor que indique que no es válido
            }
        }
        #endregion
    }
}