using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]

    public class AdminCatalogosController : BaseController
    {
        private readonly IAdminCatalogosService _adminCatalogosService;
        private readonly ICatalogosService _catalogosService;
        private readonly IDependencias _catDependencias;
        private readonly ICustomCatalogService _catalog;

        private readonly IDictionary<CatalogoViews, string> _vistaCatalogosMapping;
        public AdminCatalogosController(IAdminCatalogosService adminCatalogosService, ICatalogosService catalogosService,
                                        IDependencias catDependencias, ICustomCatalogService catalog
)
        {
            _adminCatalogosService = adminCatalogosService;
            _catalogosService = catalogosService;
            _catDependencias = catDependencias;
            _catalog = catalog;

            _vistaCatalogosMapping = new Dictionary<CatalogoViews, string>
        {
            { CatalogoViews.Carreteras, "~/Views/CatCarreteras/_ListaCarreteras.cshtml" },
            { CatalogoViews.Tramos, "~/Views/CatTramos/_ListaTramos.cshtml" },
            { CatalogoViews.Dependencias, "~/Views/Dependencias/_ListaDependencias.cshtml" },
            { CatalogoViews.Municipios, "~/Views/CatMunicipios/_ListaMunicipios.cshtml" },
            { CatalogoViews.Entidades, "~/Views/CatEntidades/_ListaEntidades.cshtml" },
            { CatalogoViews.OficialesDeTransito, "~/Views/Oficiales/_ListaOficiales.cshtml" },
            { CatalogoViews.MarcasDeVehiculos, "~/Views/CatMarcasVehiculos/_ListaMarcasVehiculos.cshtml" },
            { CatalogoViews.SubmarcasDeVehiculos, "~/Views/CatSubmarcasVehiculos/_ListaSubmarcas.cshtml" },
            { CatalogoViews.TipoDeVehiculos, "~/Views/TiposVehiculos/_ListaTiposVehiculos.cshtml" },
            { CatalogoViews.UMAS, "~/Views/SalariosMinimos/_ListaSalariosMinimos.cshtml" },
            { CatalogoViews.Colores, "~/Views/Colores/_ListaColores.cshtml" },
            { CatalogoViews.MotivosDeInfraccion, "~/Views/MotivosInfraccion/_ListaMotivosInfraccion.cshtml" },
            { CatalogoViews.DiasInhabiles, "~/Views/DiasInhabiles/_ListaDiasInhabiles.cshtml" },
            { CatalogoViews.AgenciasDelMinisterioPublico, "~/Views/CatAgenciasMinisterio/_ListaAgenciasMinisterio.cshtml" },
            { CatalogoViews.AutoridadesDisposicion, "~/Views/CatAutoridadesDisposicion/_ListaAutoridadesDisposicion.cshtml" },
            { CatalogoViews.AutoridadesEntrega, "~/Views/CatAutoridadesEntrega/_ListaAutoridadesEntrega.cshtml" },
            { CatalogoViews.InstitucionesTraslado, "~/Views/CatInstitucionesTraslado/_ListaInstitucionesTraslado.cshtml" },
            { CatalogoViews.ClasificacionDeAccidentes, "~/Views/CatClasificacionAccidentes/_ListaClasificacionAccidentes.cshtml" },
            { CatalogoViews.CausasDeAccidentes, "~/Views/CatCausasAccidentes/_ListaCausasAccidentes.cshtml" },
            { CatalogoViews.DelegacionesOficinasTransporte, "~/Views/CatDelegacionesOficinasTransporte/_ListDelegacionesOficinas.cshtml" },
            { CatalogoViews.Hospitales, "~/Views/CatHospitales/_ListaHospitales.cshtml" },
            { CatalogoViews.FactoresDeAccidentes, "~/Views/CatFactoresAccidentes/_ListaFactoresAccidentes.cshtml" },

            { CatalogoViews.FactoresOpciones, "~/Views/CatFactoresOpcionesAccidentes/_ListaFactoresOpcionesAccidentes.cshtml" },
            { CatalogoViews.OficinasRenta, "~/Views/CatOficinasRenta/_ListaOficinasRenta.cshtml" },
            { CatalogoViews.TiposCarga, "~/Views/TiposCarga/_ListaTiposCarga.cshtml" },
            { CatalogoViews.TipoServicio, "~/Views/CatTipoServicio/_ListaCatTipoServicio.cshtml" },
            { CatalogoViews.SubtipoServicio, "~/Views/CatSubtipoServicio/_ListaSubtiposServicio.cshtml" },
            { CatalogoViews.Turnos, "~/Views/CatTurnos/_ListaTurnos.cshtml" },
            { CatalogoViews.Aseguradoras, "~/Views/CatAseguradoras/_ListaAseguradoras.cshtml" }

            };
        }
        public IActionResult Index()
        {
            return View();
        }
     
        public JsonResult CatalogosDDL()
        {

            var result = new SelectList(_adminCatalogosService.ObtieneListaCatalogos(), "IdCatalogo", "Catalogo");
            return Json(result);
        }
    

        public JsonResult GetCatalogos([DataSourceRequest] DataSourceRequest request, int idCatalogo, int idDependencia)
        {
            var listaDatos = _adminCatalogosService.BusquedaPorCatalogo(idCatalogo, idDependencia);
            return Json(listaDatos.ToDataSourceResult(request));
        }

        [HttpPost]
        public IActionResult BuscarCatalogos(int? idCatalogo, int? idDependencia)
        {
            ViewBag.Admin = true;
            if (idCatalogo.HasValue)
            {
                if (Enum.IsDefined(typeof(CatalogoViews), idCatalogo.Value))
                {
                    var catalogoView = (CatalogoViews)idCatalogo.Value;
                    if (_vistaCatalogosMapping.TryGetValue(catalogoView, out string vistaParcial))
                    {
                        return PartialView(vistaParcial);
                    }
                }
            }

            return PartialView("~/Views/Catalogos/_VistaDefault.cshtml");
        }

        [HttpPost]
        public IActionResult FiltrosCatalogo(int idCatalogo, int? idDependencia)
        {
            var corp = idDependencia.HasValue ? Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value) : 0;

            if (idDependencia.HasValue)
            {
                corp = idDependencia.Value;
            }
            else
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);
            }
            ViewBag.SelectedCatalogo = idCatalogo;
            var delegaciones = _catalog.GetDelegaciones(corp);
            ViewBag.Delegaciones = delegaciones;
            return PartialView("_Filtros");
        }
      
    }
}
