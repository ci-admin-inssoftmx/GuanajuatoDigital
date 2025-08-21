using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class BaseController : Controller
    {
     List<Menu> Menu = new List<Menu>()
        {
            #region DEPOSITOS
            new Menu()
            {
                Controller = "LiberacionVehiculo",
                Action = "Index",
                Modulo = "Depósitos",
                SubModulo = "LiberacionVehiculo"
            },
            new Menu()
            {
                Controller = "transitotransporte",
                Action = "Index",
                Modulo = "Depósitos",
                SubModulo = "transitotransporte"
            },
            new Menu()
            {
                Controller = "Depositos",
                Action = "Index",
                Modulo = "Depósitos",
                SubModulo = "Depositos"
            },
            new Menu()
            {
                Controller = "AsignacionGruas",
                Action = "Index",
                Modulo = "Depósitos",
                SubModulo = "AsignacionGruas"
            },
            new Menu()
            {
                Controller = "PadronGruas",
                Action = "Index",
                Modulo = "Depósitos",
                SubModulo = "PadronGruas"
            },
            new Menu()
            {
                Controller = "PadronDepositosGruas",
                Action = "Index",
                Modulo = "Depósitos",
                SubModulo = "PadronDepositosGruas"
            },
            new Menu()
            {
                Controller = "ReporteAsignacionServicios",
                Action = "Index",
                Modulo = "Depósitos",
                SubModulo = "ReporteAsignacionServicios"
            },
            new Menu()
            {
                Controller = "Pensiones",
                Action = "Index",
                Modulo = "Depósitos",
                SubModulo = "Pensiones"
            },
            new Menu()
            {
                Controller = "Concesionarios",
                Action = "Index",
                Modulo = "Depósitos",
                SubModulo = "Concesionarios"
            },
            #endregion

            #region GENERALES
            new Menu()
            {
                Controller = "Vehiculos",
                Action = "Index",
                Modulo = "Generales",
                SubModulo = "Vehiculos"
            },
            new Menu()
            {
                Controller = "Vehiculos",
                Action = "Editar",
                Modulo = "Generales",
                SubModulo = "Vehiculos"
            },
            new Menu()
            {
                Controller = "Personas",
                Action = "Editar",
                Modulo = "Generales",
                SubModulo = "Personas"
            },           

            #endregion

            

            #region Infracciones
            new Menu()
            {
                Controller = "Infracciones",
                Action = "Index",
                Modulo = "Infracciones",
                SubModulo = "Infracciones"
            },
            #region InfraccionesInvisibles
            new Menu()
            {
                Controller = "Infracciones",
                Action = "InfraccionesInvisibles",
                Modulo = "InfraccionesInvisibles",
                SubModulo = "InfraccionesInvisibles"
            },
            #endregion
            new Menu()
            {
                Controller = "CancelarInfraccion",
                Action = "Index",
                Modulo = "Infracciones",
                SubModulo = "CancelarInfraccion"
            },
            new Menu()
            {
                Controller = "RegistroReciboPago",
                Action = "Index",
                Modulo = "Infracciones",
                SubModulo = "RegistroReciboPago"
            },
            new Menu()
            {
                Controller = "EnvioInfracciones",
                Action = "Index",
                Modulo = "Infracciones",
                SubModulo = "EnvioInfracciones"
            },
            new Menu()
            {
                Controller = "ComparativoInfracciones",
                Action = "Index",
                Modulo = "Infracciones",
                SubModulo = "ComparativoInfracciones"
            },
            new Menu()
            {
                Controller = "Estadisticas",
                Action = "Index",
                Modulo = "Infracciones",
                SubModulo = "Estadisticas"
            },
			 new Menu()
			{
				Controller = "CortesiasNoAplicadas",
				Action = "Index",
				Modulo = "Infracciones",
				SubModulo = "CortesiasNoAplicadas"
			},
              new Menu()
             {
                Controller = "InfraccionesCortesia",
                Action = "Index",
                Modulo = "Infracciones",
                SubModulo = "InfraccionesCortesia"
            },
            #endregion

            #region ACCIDENTES
            new Menu()
            {
                Controller = "CapturaAccidentes",
                Action = "Index",
                Modulo = "Accidentes",
                SubModulo = "CapturaAccidentes"
            },
            new Menu()
            {
                Controller = "BusquedaAccidentes",
                Action = "Index",
                Modulo = "Accidentes",
                SubModulo = "BusquedaAccidentes"
            },
			new Menu()
			{
				Controller = "BusquedaEspecialAccidentes",
				Action = "Index",
				Modulo = "Accidentes",
				SubModulo = "BusquedaEspecialAccidentes"
			},
			new Menu()
            {
                Controller = "EstadisticasAccidentes",
                Action = "Index",
                Modulo = "Accidentes",
                SubModulo = "EstadisticasAccidentes"
            },
            #endregion

            #region CATALOGOS
            new Menu()
            {
                Controller = "CatAgenciasMinisterio",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatAgenciasMinisterio"
            },
            new Menu()
            {
                Controller = "CatAutoridadesDisposicion",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatAutoridadesDisposicion"
            },
            new Menu()
            {
                Controller = "CatAutoridadesEntrega",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatAutoridadesEntrega"
            },
            new Menu()
            {
                Controller = "CatCarreteras",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatCarreteras"
            },
            new Menu()
            {
                Controller = "CatCausasAccidentes",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatCausasAccidentes"
            },
            new Menu()
            {
                Controller = "CatClasificacionAccidentes",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatClasificacionAccidentes"
            },
            new Menu()
            {
                Controller = "Colores",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "Colores"
            },
            new Menu()
            {
                Controller = "CatDelegacionesOficinasTransporte",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatDelegacionesOficinasTransporte"
            },
            new Menu()
            {
                Controller = "Dependencias",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "Dependencias"
            },
            new Menu()
            {
                Controller = "DiasInhabiles",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "DiasInhabiles"
            },
            new Menu()
            {
                Controller = "CatEntidades",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatEntidades"
            },
            new Menu()
            {
                Controller = "CatFactoresAccidentes",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatFactoresAccidentes"
            },
            new Menu()
            {
                Controller = "CatFactoresOpcionesAccidentes",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatFactoresOpcionesAccidentes"
            },
            new Menu()
            {
                Controller = "CatHospitales",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatHospitales"
            },
            new Menu()
            {
                Controller = "CatInstitucionesTraslado",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatInstitucionesTraslado"
            },
            new Menu()
            {
                Controller = "CatMarcasVehiculos",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatMarcasVehiculos"
            },
            new Menu()
            {
                Controller = "MotivosInfraccion",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "MotivosInfraccion"
            },
             new Menu()
            {
                Controller = "CatMunicipios",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatMunicipios"
            },
            new Menu()
            {
                Controller = "Oficiales",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "Oficiales"
            },
            new Menu()
            {
                Controller = "CatOficinasRenta",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatOficinasRenta"
            },
            new Menu()
            {
                Controller = "CatPuestos",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatPuestos"
            },
            new Menu()
            {
                Controller = "SalariosMinimos",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "SalariosMinimos"
            },
            new Menu()
            {
                Controller = "CatSubmarcasVehiculos",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatSubmarcasVehiculos"
            },
            new Menu()
            {
                Controller = "TiposCarga",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "TiposCarga"
            },
            new Menu()
            {
                Controller = "TiposVehiculos",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "TiposVehiculos"
            },
            new Menu()
            {
                Controller = "CatTipoServicio",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatTipoServicio"
            },
            new Menu()
            {
                Controller = "CatSubtipoServicio",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatSubtipoServicio"
            },
            new Menu()
            {
                Controller = "CatTramos",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatTramos"
            },
            new Menu()
            {
                Controller = "CatTurnos",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatTurnos"
            },
             new Menu()
            {
                Controller = "CatAdminWS",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatAdminWS"
            },
             new Menu()
            {
                Controller = "AdminCatalogos",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "AdminCatalogos"
             },
             new Menu()
            {
                Controller = "Relaciones",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "Relaciones"
             },
            new Menu()
            {
                Controller = "CatAseguradoras",
                Action = "Index",
                Modulo = "Catálogos",
                SubModulo = "CatAseguradoras"
            },
            #endregion           

            #region Blocs
            new Menu()
            {
                Controller = "Blocs",
                Action = "Index",
                Modulo = "Blocs",
                SubModulo = "Blocs"
            },
                new Menu()
            {
                Controller = "Blocs",
                Action = "AltaBlocs",
                Modulo = "Blocs",
                SubModulo = "Blocs"
            },    new Menu()
            {
                Controller = "Blocs",
                Action = "Inventario",
                Modulo = "Blocs",
                SubModulo = "Blocs"
            },
	        #endregion

            #region CatCamposObligatorios
            new Menu()
            {
                Controller = "CatCamposObligatorios",
                Action = "Index",
                Modulo = "CatCamposObligatorios",
                SubModulo = "CatCamposObligatorios"
            },
            new Menu()
            {
                Controller = "CatAlertamiento",
                Action = "Index",
                Modulo = "CatCamposObligatorios",
                SubModulo = "CatAlertamiento"
            },
            new Menu()
            {
                Controller = "AdministradorBlocks",
                Action = "Index",
                Modulo = "CatCamposObligatorios",
                SubModulo = "CatAlertamiento"
            },
	        #endregion

            #region CONFIGURACIONES
            new Menu()
            {
                Controller = "Configuraciones",
                Action = "Index",
                Modulo = "Configuraciones",
                SubModulo = "Configuraciones"
            },
           
            #endregion

            #region FIRMAS
            new Menu()
            {
                Controller = "Firmas",
                Action = "Index",
                Modulo = "Firmas",
                SubModulo = "Firmas"
            },
            #endregion
            #region REPORTES
            new Menu()
            {
                Controller = "Reportes",
                Action = "Index",
                Modulo = "Reportes",
                SubModulo = "Bi"
            },
              new Menu()
            {
                Controller = "Reportes",
                Action = "Accidentes",
                Modulo = "Reportes",
                SubModulo = "Bi"
            },
              new Menu()
            {
                Controller = "Reportes",
                Action = "Otros",
                Modulo = "Otros",
                SubModulo = "Bi"
            },
            #endregion
        };

        public readonly IViewRenderService _viewRenderService;
        public BaseController(IViewRenderService viewRenderService)
        {
            _viewRenderService = viewRenderService;
        }

        public BaseController() { }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            object controller = null;
            object action = null;

            context.RouteData.Values.TryGetValue("controller", out controller);
            context.RouteData.Values.TryGetValue("action", out action);

            string modulo = Menu.Where(x => x.Controller == controller.ToString()).FirstOrDefault()?.Modulo;
            string actionVal=action.ToString();

            if(actionVal != "InfraccionesInvisibles")
            {
                HttpContext.Session.SetString("SelectedModulo", modulo.IsNullOrEmpty() ? "Depósitos" : modulo);
                HttpContext.Session.SetString("SelectedSubModulo", "SubModuloTest");
            } else {
                HttpContext.Session.SetString("SelectedModulo", modulo.IsNullOrEmpty() ? "Depósitos" : actionVal);
            }

            
            if (modulo.IsNullOrEmpty())
            {
                var t = controller;
            }

            HttpContext.Session.SetString("SelectedModulo", modulo.IsNullOrEmpty()? "Depósitos" : modulo);
            HttpContext.Session.SetString("SelectedSubModulo", "SubModuloTest");

            await base.OnActionExecutionAsync(context, next);
                       
        }
    }

    public class Menu
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Modulo { get; set; }
        public string SubModulo { get; set; }
    }
}
