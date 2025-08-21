/*
 * Descripción:
 * Proyecto: RIAG
 * Fecha de creación: Tuesday, February 20th 2024 5:06:14 pm
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Thu Mar 07 2024
 * Última modificación: Thu Mar 07 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */


using System.Collections.Generic;
using System.Linq;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;
using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Util;
using Microsoft.IdentityModel.Tokens;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http;
using Kendo.Mvc.Infrastructure;
using AdminUsuarios.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using GuanajuatoAdminUsuarios.Services;

namespace GuanajuatoAdminUsuarios.Controllers;

[Authorize]
public class BusquedaVehiculoPropietarioController : BaseController
{
    #region Variables
    private readonly ICatDictionary _catDictionary;

    #endregion
    #region Constructor
    public BusquedaVehiculoPropietarioController(ICatDictionary catDictionary)
    {
        _catDictionary = catDictionary;

    }
    #endregion

    #region MostrarBusqueda
    public ActionResult MostrarBusquedaVehiculo()
    {
        VehiculoPropietarioBusquedaModel model = new()
        {
            Vehiculo = new VehiculoModel
            {
                Persona = new PersonaModel()
            },
            IdEntidadBusqueda = CatEntidadesModel.GUANAJUATO
        };
        return PartialView("_BusquedaVehiculoPropietario", model);
    }
    #endregion

    #region Vehiculo
    [HttpPost]
    public async Task<IActionResult> BuscarVehiculoEnPlataformasAsync([FromServices] IOptions<AppSettings> appSettings, [FromServices] IRepuveService repuveService,
    [FromServices] IVehiculoPlataformaService vehiculoPlataformaService, [FromServices] IVehiculosService vehiculoService, [FromServices] ICotejarDocumentosClientService cotejarDocumentosService, VehiculoPropietarioBusquedaModel model)
    {
        HttpContext.Session.Remove("PersonaModel");

        VehiculoBusquedaModel busquedaModel = new VehiculoBusquedaModel();
        VehiculoModel vehiculo = new VehiculoModel();
		var corp = HttpContext.Session.GetInt32("IdDependencia").Value;


		try
		{
            busquedaModel.IdEntidadBusqueda = model.IdEntidadBusqueda;

        }
        catch (Exception ex) { }

        try
        {
            if (!string.IsNullOrEmpty(model.PlacaBusqueda))
            {
                busquedaModel.PlacasBusqueda = model.PlacaBusqueda.ToUpper();
            }
        }
        catch (Exception ex) { }

        try
        {
            if (!string.IsNullOrEmpty(model.SerieBusqueda))
            {
                busquedaModel.SerieBusqueda = model.SerieBusqueda.ToUpper();
            }

        }
        catch (Exception ex) { }




        List<VehiculoModel> listaVehiculos = vehiculoService.GetVehiculoPropietario(busquedaModel);
        if (listaVehiculos.Count > 1)
        {
            var view1 = this.RenderViewAsync("_ListaVehiculos", listaVehiculos, true);
            return Json(new { listaVehiculos = true, view = view1 });

        }
        if (listaVehiculos.Count == 1)
        {

            return Json(new { listaVehiculos.FirstOrDefault().idVehiculo });
        }

        //Esto es una bandera para busqueda sin serie y placa - asi nos permitira obtener el objeto vehiculo preinicializado
        if (busquedaModel.PlacasBusqueda == null && busquedaModel.SerieBusqueda == null)
        {
            busquedaModel.PlacasBusqueda = "flag1";
            vehiculo = null;
        }
        else
        {
            vehiculo = await vehiculoPlataformaService.BuscarVehiculoEnPlataformas(busquedaModel,corp);

        }



        if (vehiculo == null)
        {
            vehiculo = new VehiculoModel();
            vehiculo.Persona = new PersonaModel();
            vehiculo.Persona.PersonaDireccion = new PersonaDireccionModel();
            vehiculo.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
        }
        else
        {
            HttpContext.Session.SetInt32("IdMarcaVehiculo", vehiculo.idMarcaVehiculo);
        }

        if (String.IsNullOrEmpty(model.PlacaBusqueda) && String.IsNullOrEmpty(model.SerieBusqueda))
            ViewBag.SinBusqueda = "TRUE";
        else
            ViewBag.SinBusqueda = "FALSE";

        //Se remueve la bandera antes descrita
        if (busquedaModel.PlacasBusqueda == "flag1")
        {
            vehiculo.placas = "";
        }
        if (vehiculo.idTipoPersona == 2)
        {
            HttpContext.Session.SetObject("PersonaModel", vehiculo.Persona);
        }
        if (vehiculo.Persona == null)
        {
            vehiculo.Persona = new PersonaModel();
            vehiculo.Persona.PersonaDireccion = new PersonaDireccionModel();
            vehiculo.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
        }
        var view = this.RenderViewAsync("_Vehiculo", vehiculo, true);
        return Json(new { crearVehiculo = true, view });
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


    /// <summary>
    /// Crea o actualiza un registro de un vehiculo en la bd
    /// </summary>
    /// <param name="vehiculoService"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public ActionResult CrearEditarVehiculo([FromServices] IPersonasService personasService, [FromServices] IVehiculosService vehiculoService, VehiculoModel model)
    {
        int IdVehiculo;
        if (model.Persona.idPersona == -1)
        {
            var personamodel = new PersonaModel();
            personamodel.nombre = "Se ignora";
            personamodel.idCatTipoPersona = 0;
            personamodel.PersonaDireccion = new PersonaDireccionModel();

            var IdPersona = personasService.CreatePersona(personamodel);
            model.Persona.idPersona = IdPersona;

        }
        model.propietario = model.Persona?.idPersona.ToString();

        if (model.idVehiculo > 0)
        {
            vehiculoService.UpdateVehiculo(model);
            IdVehiculo = model.idVehiculo;
        }
        else
            IdVehiculo = vehiculoService.CreateVehiculo(model);

        if (IdVehiculo <= 0)
            return Json(new { success = false });

        return Json(new { success = true, data = IdVehiculo });
    }
    #endregion

    #region Propietario, Conductor, Persona
    /// <summary>
    /// Muestra vista para crear persona fisica
    /// </summary>
    /// <returns></returns>
    public ActionResult MostrarPersonaFisica(BusquedaPersonaModel model)
    {

        return ViewComponent("CrearPersona", new { model });
    }

    public ActionResult MostrarPersonaFisica2(BusquedaPersonaModel model)
    {

        return ViewComponent("EditarPersonaFisica", new { model });
    }
    /// <summary>
    /// Muestra vista para crear persona moral
    /// </summary>
    /// <returns></returns>
    public ActionResult MostrarPersonaMoral()
    {
        PersonaModel model;

        var q = HttpContext.Session.GetObject<PersonaModel>("PersonaModel");

        if (q == null)
        {
            model = new PersonaModel
            {
                PersonaDireccion = new PersonaDireccionModel()
            };
        }
        else
        {
            model = q;
        }

        return PartialView("_PersonaMoral", model);
    }


    /// <summary>
    /// Busca todas las personas morales
    /// </summary>
    /// <param name="personasService"></param>
    /// <param name="PersonaMoralBusquedaModel"></param>
    /// <returns></returns>
    public ActionResult BuscarPersonaMoral([FromServices] IPersonasService personasService, PersonaMoralBusquedaModel PersonaMoralBusquedaModel)
    {
        PersonaMoralBusquedaModel.IdTipoPersona = (int)TipoPersona.Moral;
        var personasMoralesModel = personasService.GetAllPersonasMorales(PersonaMoralBusquedaModel);
        return PartialView("_ListPersonasMorales", personasMoralesModel);
    }

    /// <summary>
    /// Busca todas las personas fisicas
    /// </summary>
    /// <param name="personasService"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult BuscarPersonasFiscas([FromServices] IPersonasService personasService)
    {
        var personasFisicas = personasService.GetAllPersonas();
        return PartialView("_PersonasFisicas", personasFisicas);
    }

    [HttpPost]
    public IActionResult BuscarPersonaFisicaWithPaginado([FromServices] IPersonasService personasService, [DataSourceRequest] DataSourceRequest request, BusquedaPersonaModel model)
    {
        //Se eliminan espacios en blanco de los campos de busqueda
        model.PersonaModel ??= new();
        model.CURPBusqueda = model.PersonaModel.CURPBusqueda?.Trim();
        model.RFCBusqueda = model.PersonaModel.RFCBusqueda?.Trim();
        model.NombreBusqueda = model.PersonaModel.nombreBusqueda?.Trim();
        model.ApellidoPaternoBusqueda = model.PersonaModel.apellidoPaternoBusqueda?.Trim();
        model.ApellidoMaternoBusqueda = model.PersonaModel.apellidoMaternoBusqueda?.Trim();
        model.NumeroLicenciaBusqueda = model.PersonaModel.numeroLicenciaBusqueda?.Trim();
        model.IdTipoPersona = Convert.ToInt16(TipoPersona.Fisica);

        //Logger.Info("Buscar persona fisica en RIAG por :" + model);
        Pagination pagination = new()
        {
            PageIndex = request.Page - 1
        };
        if (model.PersonaModel != null)
        {
            if (model.PersonaModel.apellidoMaternoBusqueda == null &&
                model.PersonaModel.apellidoPaternoBusqueda == null &&
                model.PersonaModel.CURPBusqueda == null &&
                model.PersonaModel.RFCBusqueda == null &&
                model.PersonaModel.numeroLicenciaBusqueda == null &&
                model.PersonaModel.nombreBusqueda == null)
            {
                pagination.PageSize = (request.PageSize > 0) ? request.PageSize : 10000;
            }
            else
            {
                pagination.PageSize = 10000;
            }
        }
        else
        {
            pagination.PageSize = (request.PageSize > 0) ? request.PageSize : 10;
        }

        int total = personasService.ObtenerTotalBusquedaPersona(model, pagination);
        model.Total = total;

        model.Pagination = pagination;
        // Verificar si se encontraron resultados en la búsqueda de personas
        if (total > 0)
            return Json(new { encontrada = true, result = model });


        // Si no se encontraron resultados en la búsqueda de personas, realizar la búsqueda por licencia
        return Json(new { encontrada = false, tipo = "sin datos", message = "busca en licencias" });
    }

    public IActionResult MostrarListaPersonasRiagEncontradas(BusquedaPersonaModel model)
    {
        return ViewComponent("ListaPersonasEncontradas", new { model });
    }

    public IActionResult MostrarListaPersonasLicenciasEncontradas(BusquedaPersonaModel model)
    {
        return ViewComponent("ListaPersonasEncontradasOtras", new { listaPersonas = model.ListadoPersonasOtras });
    }

    public IActionResult GuardaPersonaLicenciasEnRiag([FromServices] IPersonasService personasService, [FromServices] IBitacoraService bitacoraServices, PersonaLicenciaModel personaLicencia)
    {

        //Se busca a la persona por licencia o curp
        int idPersona = personasService.ExistePersona(personaLicencia.NumeroLicencia, personaLicencia.Curp);

        //Si no existe la persona se inserta
        if (idPersona <= 0)
            idPersona = personasService.InsertarPersonaDeLicencias(personaLicencia);

        //Se obtienen los datos de la persona por id
        PersonaModel persona = personasService.GetPersonaById(idPersona);


        //BITACORA
        var ip = HttpContext.Connection.RemoteIpAddress.ToString();
        var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
        //bitacoraServices.insertBitacora(idPersona, ip, "Personas_DesdeServicio", "Insertar", "insert", user);
        object data = new { };
        bitacoraServices.BitacoraGenerales(CodigosGeneral.C5012, data);
        BusquedaPersonaModel modelo = new()
        {
            ListadoPersonas = new List<PersonaModel>()
        };
        modelo.ListadoPersonas.Add(persona);
        return Json(new { data = modelo });
    }
    /// <summary>
    /// Crea un nuevo registro en la bd de una persona fisica
    /// </summary>
    /// <param name="personasService"></param>
    /// <param name="Persona"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public ActionResult CrearPersonaFisica([FromServices] IPersonasService personasService, BusquedaPersonaModel modeloBusqueda)
    {
        var persona = modeloBusqueda.PersonaModel;
        int IdPersonaFisica = 0;
        if (persona.idPersona > 0)
        {
            persona.idCatTipoPersona = (int)TipoPersona.Fisica;
            int result = personasService.UpdatePersona(persona);
            if (result > 0)
                IdPersonaFisica = (int)persona.idPersona;
        }
        else
        {
            persona.idCatTipoPersona = (int)TipoPersona.Fisica;
            IdPersonaFisica = personasService.CreatePersona(persona);
        }
        if (IdPersonaFisica == 0)
        {
            throw new Exception("Ocurrio un error al dar de alta la persona");
        }
        var modelList = personasService.ObterPersonaPorIDList(IdPersonaFisica);

        modeloBusqueda.ListadoPersonas = modelList;

        return Json(new { success = true, data = modeloBusqueda });
    }
    /// <summary>
    /// Crea un nuevo registro en la bd de una persona moral
    /// </summary>
    /// <param name="personasService"></param>
    /// <param name="Persona"></param>
    /// <returns></returns>
    public ActionResult CrearPersonaMoral([FromServices] IPersonasService personasService, PersonaModel Persona)
    {
        Persona.idCatTipoPersona = (int)TipoPersona.Moral;
        Persona.PersonaDireccion.telefono = System.String.IsNullOrEmpty(Persona.PersonaDireccion.telefono) ? null : Persona.PersonaDireccion.telefono;
        var IdPersonaMoral = personasService.CreatePersonaMoral(Persona);
        if (IdPersonaMoral == 0)
            return Json(new { success = false, message = "Ocurrió un error al procesar su solicitud." });
        else
        {
            var modelList = personasService.ObterPersonaPorIDList(IdPersonaMoral);
            return PartialView("_ListPersonasMorales", modelList);
        }


        //var personasMoralesModel = _personasService.GetAllPersonasMorales();

    }
    /// <summary>
    /// Busca personas en el sistema de licencias a través de un servicio web publicado
    /// </summary>
    /// <param name="_httpClientFactory"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<IActionResult> BuscarPersonasEnLicencias([FromServices] IHttpClientFactory _httpClientFactory, [FromServices] ICatEntidadesService catEntidadesService, BusquedaPersonaModel model)
    {
        var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        IConfiguration configuration = builder.Build();


        string UrlHost = configuration.GetSection("AppSettings").GetSection("UrlHost").Value.ToString();
        string parametros = "";
        parametros += string.IsNullOrEmpty(model.NumeroLicenciaBusqueda) ? "" : "licencia=" + model.NumeroLicenciaBusqueda + "&";
        parametros += string.IsNullOrEmpty(model.CURPBusqueda) ? "" : "curp=" + model.CURPBusqueda + "&";
        parametros += string.IsNullOrEmpty(model.RFCBusqueda) ? "" : "rfc=" + model.RFCBusqueda + "&";
        parametros += string.IsNullOrEmpty(model.NombreBusqueda) ? "" : "nombre=" + model.NombreBusqueda + "&";
        parametros += string.IsNullOrEmpty(model.ApellidoPaternoBusqueda) ? "" : "primer_apellido=" + model.ApellidoPaternoBusqueda + "&";
        parametros += string.IsNullOrEmpty(model.ApellidoMaternoBusqueda) ? "" : "segundo_apellido=" + model.ApellidoMaternoBusqueda;
        string ultimo = parametros[^1..];
        if (ultimo.Equals("&"))
            parametros = parametros[..^1];

        //string urlServ = Request.GetDisplayUrl();
        //Uri uri = new(urlServ);
        //string requested = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

        var url = UrlHost + $"/api/Licencias/datos_generales?" + parametros;

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;


            List<LicenciaPersonaDatos> respuesta = JsonConvert.DeserializeObject<List<LicenciaPersonaDatos>>(content);

            List<PersonaModel> resultado = new();

            foreach (LicenciaPersonaDatos p in respuesta)
            {
                PersonaModel pm = new();
                pm.ConvertirModeloDeLicencias(p);
                pm.PersonaDireccion.calle = pm.calle.ToString();
                pm.PersonaDireccion.numero = pm.numero.ToString();
                pm.PersonaDireccion.colonia = pm.colonia.ToString();
                CatEntidadesModel entidad = catEntidadesService.ObtenerEntidadesByNombre(p.ESTADO_NACIMIENTO, corp);
                if (entidad != null && entidad.idEntidad > 0)
                    pm.PersonaDireccion.idEntidad = entidad.idEntidad;
                resultado.Add(pm);
            }

            return Json(new { success = true, data = resultado });
        }

        return Json(new { success = true, message = "No se pudo conectar al servicio de licencias" });
    }


    #endregion
    #region Catalogos
    public JsonResult GetEntidades_Drop()
    {
        var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
        var result = new SelectList(catEntidades.CatalogList.Where(s => s.Corp == 1).ToList(), "Id", "Text");
        return Json(result);
    }
    public JsonResult SubTipoServicios_Drop()
    {
        var catEntidades = _catDictionary.GetCatalog("CatSubtipoServicio", "nombreEntidad");
        var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
        return Json(result);
    }

    public JsonResult GetSubtipoPorTipo_Drop([FromServices] ICatSubtipoServicio subtipoServicio, int idTipoServicio)
    {
        var result = new SelectList(subtipoServicio.GetSubtipoPorTipo(idTipoServicio), "idSubTipoServicio", "subTipoServicio");
        return Json(result);
    }
    public JsonResult Entidades_Drop()
    {
        var catEntidades = _catDictionary.GetCatalog("CatEntidades", "0");
        var result = new SelectList(catEntidades.CatalogList.Where(s => s.Corp == 1).ToList(), "Id", "Text");
        return Json(result);
    }

    public JsonResult TipoServicios_Drop()
    {
        var catEntidades = _catDictionary.GetCatalog("CatTipoServicio", "0");
        var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
        return Json(result);
    }
    public JsonResult Municipios_Drop([FromServices] ICatMunicipiosService _catMunicipiosService, int entidadDDlValue)
    {
        var corp = 1;// HttpContext.Session.GetInt32("IdDependencia").Value;

        var result = new SelectList(_catMunicipiosService.GetMunicipiosActivePorEntidad(entidadDDlValue, corp), "IdMunicipio", "Municipio");
        return Json(result);
    }
    public JsonResult Colores_Drop()
    {
        var catEntidades = _catDictionary.GetCatalog("CatColores", "0");
        var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
        return Json(result);
    }

    public JsonResult Marcas_Drop()
    {
        var catEntidades = _catDictionary.GetCatalog("CatMarcasVehiculos", "0");
        var orderedList = catEntidades.CatalogList.OrderBy(item => item.Text);
        var result = new SelectList(orderedList, "Id", "Text"); return Json(result);
    }

    public JsonResult SubMarcas_Drop()
    {
        var catEntidades = _catDictionary.GetCatalog("CatSubmarcasVehiculos", "0");
        var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
        //var selected = result.Where(x => x.Value == Convert.ToString(idSubmarca)).First();
        //selected.Selected = true;
        return Json(result);
    }

    public JsonResult TiposVehiculo_Drop()
    {
        var catEntidades = _catDictionary.GetCatalog("CatTiposVehiculo", "0");
        var result = new SelectList(catEntidades.CatalogList, "Id", "Text");
        return Json(result);
    }
    public JsonResult TipoLicencias_Drop([FromServices] ICatTipoLicenciasService catTipoLicenciasService)
    {
        var result = new SelectList(catTipoLicenciasService.ObtenerTiposLicencia(), "idTipoLicencia", "tipoLicencia");
        return Json(result);
    }

    #endregion

}
