using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.RESTModels;
using System.Threading.Tasks;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;
using GuanajuatoAdminUsuarios.Util;
using Kendo.Mvc;
using GuanajuatoAdminUsuarios.Services.Blocs;
using GuanajuatoAdminUsuarios.Interfaces.Blocs;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class DepositosController : BaseController
    {
        private readonly IDepositosService _catDepositosService;
        private readonly ICatTiposVehiculosService _catTiposVehiculoService;
        private readonly ICatResponsablesPensiones _catResponsablesPensiones;
        private readonly IOficiales _oficialesService;
        private readonly ICatEntidadesService _catEntidadesService;
        private readonly ICatMunicipiosService _catMunicipiosService;
        private readonly ICatDescripcionesEventoService _descripcionesEventoService;
        private readonly ICatTipoUsuarioService _catTipoUsuarioService;
        private readonly ICatTipoMotivoAsignacionService _catTipoMotivoAsignacionService;
        private readonly ICatCarreterasService _catCarreterasService;
        private readonly ICatTramosService _catTramosService;
        private readonly IPensionesService _pensionesService;
        private readonly IConcesionariosService _concesionariosService;
        private readonly IBitacoraService _bitacoraServices;
        private readonly ICatDictionary _catDictionary;
        private readonly IPersonasService _personasService;
        private readonly IVehiculosService _vehiculosService;
        private readonly IVehiculoPlataformaService _vehiculoPlataformaService;
		private readonly IBlockPermisoDepositos _canBlock;
		private readonly IBlocsService _Block;

		private string resultValue = string.Empty;
		bool canPermiso;

		public DepositosController(IDepositosService catDepositosService, ICatTiposVehiculosService catTiposVehiculoService, ICatResponsablesPensiones catResponsablesPensiones, IOficiales oficialesService, ICatEntidadesService catEntidadesService, ICatMunicipiosService catMunicipiosService,
            ICatDescripcionesEventoService descripcionesEventoService, ICatTipoMotivoAsignacionService catTipoMotivoAsignacionService, ICatTipoUsuarioService catTipoUsuarioService, ICatCarreterasService catCarreterasService, ICatTramosService catTramosService, IPensionesService pensionesService,
            IConcesionariosService concesionariosService, IBitacoraService bitacoraService,
            ICatDictionary catDictionary,
            IPersonasService personasService,
            IVehiculosService vehiculosService,
            IVehiculoPlataformaService vehiculoPlataformaService,IBlockPermisoDepositos block, IBlocsService serviceblock)
        {
            _catDepositosService = catDepositosService;
            _catTiposVehiculoService = catTiposVehiculoService;
            _catResponsablesPensiones = catResponsablesPensiones;
            _oficialesService = oficialesService;
            _catEntidadesService = catEntidadesService;
            _catMunicipiosService = catMunicipiosService;
            _descripcionesEventoService = descripcionesEventoService;
            _catTipoMotivoAsignacionService = catTipoMotivoAsignacionService;
            _catTipoUsuarioService = catTipoUsuarioService;
            _catCarreterasService = catCarreterasService;
            _catTramosService = catTramosService;
            _pensionesService = pensionesService;
            _concesionariosService = concesionariosService;
            _bitacoraServices = bitacoraService;
            _catDictionary = catDictionary;
            _personasService = personasService;
            _vehiculosService = vehiculosService;
            _vehiculoPlataformaService = vehiculoPlataformaService;
			_canBlock = block;
			_Block = serviceblock;
			canPermiso = _canBlock.getdate();
		}

        public IActionResult Depositos(int? Isol)
        {

            if (Isol.HasValue)
            {

                HttpContext.Session.SetInt32("isEditSolicitud", 1);
                var solicitud = _catDepositosService.ObtenerSolicitudPorID(Isol.Value);
                var isedit = HttpContext.Session.GetInt32("isEditSolicitud");
                ViewBag.canBlock = canPermiso;
                ViewBag.EsEdicion = isedit.HasValue ? isedit.ToString() : "0";
                return View(solicitud);
            }
            else
            {
                ViewBag.canBlock = canPermiso;
                SolicitudDepositoModel modelo = new SolicitudDepositoModel
                {
                    idEntidad = 11,
                    entidad = "Guanajuato",
                };
                return View(modelo);
            }
        }


        public IActionResult DepositosInfraccion()
        {
            int infraccionID = HttpContext.Session.GetInt32("LastInfCapturada").HasValue ? HttpContext.Session.GetInt32("LastInfCapturada").Value : 0;

            HttpContext.Session.Remove("LastInfCapturada");

            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            var complemntarRegistro = _catDepositosService.ImportarInfraccion(infraccionID, idDependencia);
            complemntarRegistro.idEntidad = 11;
            ViewBag.canBlock = canPermiso;
            if (complemntarRegistro.idInfraccion != null && complemntarRegistro.idInfraccion != 0)
                return View("Depositos", complemntarRegistro);
            else
                return View("Depositos");

        }

        public IActionResult Ubicacion(int Isol)
        {
            var solicitud = _catDepositosService.ObtenerSolicitudPorID(Isol);

            return View(solicitud);

        }
        
        public IActionResult Editar(int Isol)
        {
            var solicitud = _catDepositosService.ObtenerSolicitudPorID(Isol);
            ViewBag.canBlock = canPermiso;
            return View("Depositos", solicitud);

        }

        public IActionResult CargarVehiculo(string folio)
        {
            ViewBag.FolioSolicitud = folio;
            return View("CargarVehiculo");
        }


        public ActionResult GetAllVehiculosPagination([DataSourceRequest] DataSourceRequest request, VehiculoBusquedaModel filtroBusqueda)
        {

            filterValue(request.Filters);

            Pagination pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            pagination.PageSize = (request.PageSize != 0) ? request.PageSize : 10;
            pagination.Filter = resultValue;
            
            IList<SortDescriptor> t = request.Sorts;
            if (t.Count > 0)
            {
                var q = t[0];
                pagination.Sort = q.SortDirection.ToString().Substring(0, 3);
                pagination.SortCamp = q.Member;
            }

            VehiculoBusquedaModel safeFiltroBusqueda = GetSafeFiltroVehiculoBusquedaFiltro(filtroBusqueda);
            var vehiculosList =  safeFiltroBusqueda != null 
                ? _vehiculosService.GetAllVehiculosPaginationByFilter(pagination, safeFiltroBusqueda)
                : _vehiculosService.GetAllVehiculosPagination(pagination);

            var total = 0;
            if (vehiculosList.Any())
                total = vehiculosList.ToList().FirstOrDefault().total;

            request.PageSize = pagination.PageSize;
            var result = new DataSourceResult()
            {
                Data = vehiculosList,
                Total = total
            };

            return Json(result);
        }

        private VehiculoBusquedaModel GetSafeFiltroVehiculoBusquedaFiltro(VehiculoBusquedaModel filtroBusqueda)
        {
            if (filtroBusqueda == null) return filtroBusqueda;

            bool isNotSafe = 
                string.IsNullOrWhiteSpace(filtroBusqueda.PlacasBusqueda) &&
                string.IsNullOrWhiteSpace(filtroBusqueda.SerieBusqueda) &&
                (filtroBusqueda.IdEntidadBusqueda is null || filtroBusqueda.IdEntidadBusqueda == 0);

            return isNotSafe ? null : filtroBusqueda;
        }

        private void filterValue(IEnumerable<IFilterDescriptor> filters)
        {
            if (!filters.Any())            
                return;
            
            foreach (var filter in filters)
            {
                if (filter is FilterDescriptor descriptor)
                {
                    resultValue = descriptor.Value.ToString();
                    break;
                }

                if (filter is CompositeFilterDescriptor compositeFilter && resultValue == "")
                {
                    filterValue(compositeFilter.FilterDescriptors);
                }                    
            }
        }


        [HttpPut]
        public IActionResult CambiarVehiculoSolicitudDeposito(string folioSolicitud, int idVehiculo)
        {
            DepositosModel deposito = _catDepositosService.GetDepositoByFolioSolicitud(folioSolicitud);
            if (deposito is null)
            {
                return NotFound($"no existe depósito generado a partir de solicitud {folioSolicitud}");
            }

            _catDepositosService.CambiarDepositoVehiculo(deposito.IdDeposito, deposito.IdSolicitud, idVehiculo);

            return Ok();
        }

        public ActionResult ModalAgregarVehiculo()
        {
            return PartialView("_ModalVehiculo");
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
            var result = await this.RenderViewAsync2("Depositos", models);
            return result;
        }


        // busca por coincidencia exacta de placas y/o serie
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
                    return await BuscarVehiculoPorPlataforma(model);                    
                }
            }
            catch (Exception ex)
            {
                return Json(new { noResults = true, error = "Se produjo un error al procesar la solicitud", data = "" });
            }
        }

        public async Task<ActionResult> BuscarVehiculoPorPlataforma(VehiculoBusquedaModel model)
        {
            try
            {
                var jsonPartialVehiculosByWebServices = await ajax_BuscarVehiculo(model);

                if (jsonPartialVehiculosByWebServices != null)                
                    return Json(new { noResults = true, data = jsonPartialVehiculosByWebServices });
                
                return Json(new { noResults = true, data = "" });                
            }
            catch (Exception ex)
            {
                return Json(new { noResults = true, error = "Se produjo un error al procesar la solicitud", data = "" });
            }
            
        }

        [HttpPost]
        public async Task<string> ajax_BuscarVehiculo(VehiculoBusquedaModel model)
        {
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

				var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

				var models = await _vehiculoPlataformaService.BuscarVehiculoEnPlataformas(model, corp);
                HttpContext.Session.SetInt32("IdMarcaVehiculo", models.idMarcaVehiculo);


                var test = await this.RenderViewAsync2("Depositos", models);


                return test;
            }
            catch (Exception ex)
            {
                Logger.Error("Depósitos - ajax_BuscarVehiculo: " + ex.Message);
                return null;
            }
        }




        [HttpPost]
        public ActionResult ajax_BuscarPersonasFiscas()
        {
            var personasFisicas = _personasService.GetAllPersonas();
            return PartialView("_PersonasFisicas", personasFisicas);
        }

        [HttpPost]
        public ActionResult ajax_BuscarPersonaMoral(PersonaMoralBusquedaModel PersonaMoralBusquedaModel)
        {
            PersonaMoralBusquedaModel.IdTipoPersona = (int)TipoPersona.Moral;
            var personasMoralesModel = _personasService.GetAllPersonasMorales(PersonaMoralBusquedaModel);
            return PartialView("_ListPersonasMorales", personasMoralesModel);
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



            var IdVehiculo = _vehiculosService.CreateVehiculo(model);

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


        /////////////////////////////////////////////////


        public JsonResult TiposVehiculos_Drop()
        {
            var result = new SelectList(_catTiposVehiculoService.GetTiposVehiculos(), "IdTipoVehiculo", "TipoVehiculo");
            return Json(result);
        }

        public JsonResult TiposVehiculosActivos_Drop()
        {
            var result = new SelectList(_catTiposVehiculoService.GetTiposVehiculos().Where(x => x.Estatus == 1), "IdTipoVehiculo", "TipoVehiculo");
            return Json(result);
        }

        public JsonResult Concecionarios_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var result = new SelectList(_concesionariosService.GetConcesionarios(idOficina), "IdConcesionario", "Concesionario");
            return Json(result);
        }
        public JsonResult ConcecionariosActivos_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var result = new SelectList(_concesionariosService.GetConcesionariosActivos(idOficina), "IdConcesionario", "Concesionario");
            return Json(result);
        }
        public JsonResult Oficiales_Drop()
        {
            var oficiales = _oficialesService.GetOficialesActivos()
                .Select(o => new
                {
                    IdOficial = o.IdOficial,
                    NombreCompleto = $"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}"
                });
            oficiales = oficiales.Skip(1);
            var result = new SelectList(oficiales, "IdOficial", "NombreCompleto");

            return Json(result);
        }
        public JsonResult Entidades_Drop()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catEntidadesService.ObtenerEntidades(corp), "idEntidad", "nombreEntidad");
            return Json(result);
        }
        public JsonResult Municipios_Drop(int entidadDDlValue)
        {

            if (entidadDDlValue == 0)
            {
                entidadDDlValue = 11;
            }
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var tt = _catMunicipiosService.GetMunicipiosPorEntidad(entidadDDlValue, corp);

            tt.Add(new CatMunicipiosModel() { IdMunicipio = 49, Municipio = "No aplica" });
            tt.Add(new CatMunicipiosModel() { IdMunicipio = 47, Municipio = "No especificado" });

            var result = new SelectList(tt, "IdMunicipio", "Municipio");

            return Json(result);
        }

        public JsonResult Descripcion_Drop()
        {
            var result = new SelectList(_descripcionesEventoService.ObtenerDescripciones(), "idDescripcion", "descripcionEvento");
            return Json(result);
        }
        public JsonResult TiposUsuario_Drop()
        {
            var result = new SelectList(_catTipoUsuarioService.ObtenerTiposUsuario(), "idTipoUsuario", "tipoUsuario");
            return Json(result);
        }
        public JsonResult Servicios_Drop()
        {
            var result = new SelectList(_catDepositosService.ObtenerServicios(), "idServicioRequiere", "servicioRequiere");
            return Json(result);
        }
        public JsonResult Motivos_Drop()
        {
            var result = new SelectList(_catTipoMotivoAsignacionService.ObtenerMotivos(), "idTipoAsignacion", "tipoAsignacion");
            return Json(result);
        }
        public JsonResult Carreteras_Drop()
        {
            var result = new SelectList(_catCarreterasService.ObtenerCarreteras(), "IdCarretera", "Carretera");
            return Json(result);
        }

        public JsonResult Tramos_Drop(int carreteraDDValue)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var tt = _catTramosService.ObtenerTamosPorCarretera(carreteraDDValue, corp);
            tt.Add(new CatTramosModel() { IdTramo = 1, Tramo = "No aplica" });
            tt.Add(new CatTramosModel() { IdTramo = 2, Tramo = "No especificado" });


            var result = new SelectList(tt, "IdTramo", "Tramo");



            return Json(result);
        }
        public JsonResult Pensiones_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_pensionesService.GetAllPensiones(idOficina), "IdPension", "Pension");
            return Json(result);
        }

        public JsonResult Pensiones_Drop2()
        {
            // int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_pensionesService.GetAllPensiones2(), "IdPension", "Pension");
            return Json(result);
        }


        public ActionResult ajax_EnviarSolicitudDeposito(int? Isol, [FromBody] SolicitudDepositoModel model)
        {

          

            var iddeleg = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value);
            var idblockfolio = 0;
            if (canPermiso)
            {
                idblockfolio = _Block.ExistFolio(model.folioBDeposito, iddeleg, model.idOficial.Value, 3);

                if (idblockfolio == 0)
                {
                    return Json(new { success = false, message = "Folio de bloc no existe", errorFolio = 1 });
                }

            }



            if (!string.IsNullOrEmpty(model.horaSolicitudStr))
            {
                TimeSpan hora;
                if (TimeSpan.TryParse(model.horaSolicitudStr, out hora))
                {
                    // Asignar la hora convertida al modelo
                    model.horaSolicitud = hora;
                }
                else
                {
                    ModelState.AddModelError("HoraComoString", "La hora proporcionada no es válida.");
                }
            }
            if (model.idSolicitud.HasValue && model.idSolicitud.Value > 0)
            {
                // mEs una actualizaci�n, as� que actualiza los datos en la base de datos
                // utilizando el ID 'Isol' para identificar la solicitud existente
                var registroActualizado = _catDepositosService.ActualizarSolicitud((int)model.idSolicitud, model);

                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                //_bitacoraServices.insertBitacora(registroActualizado, ip, "Depositos_EnviarSolicitudDeposito", "Actualizar", "update", user);
                model.idDeposito = _catDepositosService.ReturnIdDeposito(model.idSolicitud.Value);  


                _bitacoraServices.BitacoraDepositos(model.idDeposito.Value, "Solicitud de Deposito/solicitud", CodigosDepositos.C3029, model);

                return Json(new 
                { 
                    success = true, 
                    data = new { folio = registroActualizado.Folio, idVehiculo = registroActualizado.IdVehiculo }, 
                    update = true 
                });

            }
            else
            {
                var oficina = User.FindFirst(CustomClaims.Oficina).Value;
                int idOficina = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value); //HttpContext.Session.GetInt32("IdOficina") ?? 0;
                string abreviaturaMunicipio = User.FindFirst(CustomClaims.AbreviaturaMunicipio).Value;
                int dependencia = Convert.ToInt32(HttpContext.Session.GetInt32("IdDependencia"));
                int idUsuarios = Convert.ToInt32(User.FindFirst(CustomClaims.IdUsuario).Value);

                if (abreviaturaMunicipio.IsNullOrEmpty())
                {
                    return Json(new { success = false, message = "La delegación del usuario no tiene asociado un municipio, no se puede generar el folio de la solicitud de depósito" });

                }

                var resultadoBusqueda = _catDepositosService.GuardarSolicitud(model, idOficina, oficina, abreviaturaMunicipio, DateTime.Now.Year, dependencia, idUsuarios);

                //BITACORA
                //var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                //var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                //_bitacoraServices.insertBitacora(resultadoBusqueda, ip, "Depositos_EnviarSolicitudDeposito", "Insertar", "insert", user);

                ////////////////////////////////////////////////////////////
                if (canPermiso)
                    _Block.UpdateFolio(idblockfolio, (int)model.idDeposito, 3);
                _bitacoraServices.BitacoraDepositos((int)model.idDeposito, "Solicitud de Deposito", CodigosDepositos.C3001, model);

                return Json(new { success = true, data = new { folio = resultadoBusqueda } });
            }
        }




        public ActionResult ajax_EnviarComplementoSolicitud(SolicitudDepositoModel model)
        {
            var complemntarRegistro = _catDepositosService.CompletarSolicitud(model);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(complemntarRegistro, ip, "Depositos_CompletarSolicitud", "Insertar", "insert", user);
            _bitacoraServices.BitacoraDepositos(model.idDeposito.Value, "Solicitud de Deposito", CodigosDepositos.C3029, model);
            return Ok();
        }
        public ActionResult ajax_ImportarInfoInfraccion(string folioBusquedaInfraccion)
        {
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");
            var complemntarRegistro = _catDepositosService.ImportarInfraccion(folioBusquedaInfraccion, idDependencia);
            return Json(complemntarRegistro);
        }


        public JsonResult PensionesTodas_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var result = new SelectList(_pensionesService.GetAllPensiones2(), "IdPension", "Pension");
            return Json(result);
        }

    }

}
