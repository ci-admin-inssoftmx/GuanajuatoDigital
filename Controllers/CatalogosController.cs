using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class CatalogosController : Controller
    {

        ICustomCatalogService catalog;
        ICatMunicipiosService _catMunicipiosService;
        IOficiales _oficialesService;

        public CatalogosController(ICustomCatalogService _catalog, ICatMunicipiosService catMunicipiosService, IOficiales oficialesService)
        {
            catalog = _catalog;
            _catMunicipiosService = catMunicipiosService;
            _oficialesService = oficialesService;
        }
        public IActionResult GetDelegaciones(int? idDependencia)
        {
            var catalogos = new List<CatalogModel>();
            var corp = idDependencia.HasValue ? Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value) : 0;

            if (idDependencia.HasValue)
            {
                 corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);
            }
            else if (corp < 2)
            {
                catalogos = catalog.GetDelegaciones();

            }
            else
            {
                catalogos = catalog.GetDelegaciones(corp);
            }


            var result = new SelectList(catalogos, "value", "text");
            return Json(result);
        }

        public IActionResult GetDelegacionesCorp()
        {
            var catalogos = new List<CatalogModel>();
            var corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);            
                catalogos = catalog.GetDelegaciones(corp);
       
            return Json(catalogos);
        }


        public IActionResult GetDelegacionesDiv(int? idDependencia)
        {
            var catalogos = new List<CatalogModel>();
            var corp = idDependencia.HasValue ? Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value) : 0;

            if (idDependencia.HasValue)
            {
                corp = idDependencia.Value;
            }
            else
            {
                corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);
            }

            catalogos = catalog.GetDelegaciones(corp);



            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }
        public IActionResult GetFactorAccidente(int? idDependencia)
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
            var catalogos = catalog.GetFactorAccidente(corp);
            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }

        public IActionResult GetAllCarreteras()
        {
            var catalogos = catalog.GetAllCarreteras();
            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }

        public IActionResult GetCarreterasFiltroDelegacion(int idDelegacion)
        {
            var catalogos = catalog.GetCarreterasByDelegacion(idDelegacion);
            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }

        public IActionResult Drop_CargaActivos()
        {
            var catalogos = catalog.GetTiposCargaActivos();
            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }

        public IActionResult Drop_ClasificacionesActivas()
        {
            var catalogos = catalog.GetClasificacionesAccidentesActivas();
            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }
        public IActionResult Drop_ClasificacionesActivasCorp()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var corporation = corp < 3 ? 1 : corp;
            var catalogos = catalog.GetClasificacionesAccidentesActivas(corporation);
            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }

        public IActionResult Drop_CausaAccidentesActivasCorp()
        {

            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var corporation = corp < 3 ? 1 : corp;
            var catalogos = catalog.GetCausasAccidentesActivas(corporation);
            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }
        public IActionResult Drop_CausaAccidentesActivas()
        {
            int corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            if (corp < 4) corp = 1;
            var catalogos = catalog.GetCausasAccidentesActivas(corp);
            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }

        public IActionResult Drop_CausaAccidentesActivasCorporacion()
        {
            int corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina)?.Value);
            var catalogos = catalog.GetCausasAccidentesActivasCorporacion(corp);
            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }

        public IActionResult Drop_HospitalesActivos()
        {
            var corp = Convert.ToInt32(HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value);
            var catalogos = catalog.GetHospitalesActivos(corp);
            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }

        public IActionResult Drop_Hospitales()
        {
            var catalogos = catalog.GetHospitales();
            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }

        public IActionResult Drop_AutoridadesDisposicionActivas()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var corporation = corp < 2 ? 1 : corp;
            var catalogos = catalog.GetAutoridadesDisposicionActivas(corporation);
            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }

        public IActionResult Drop_AutoridadesDisposicion()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var corporation = corp < 2 ? 1 : corp;
            var catalogos = catalog.GetAutoridadesDisposicion(corporation);
            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }

        public IActionResult Drop_AutoridadesEntregaActivas()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var corporation = corp < 2 ? 1 : corp;
            var catalogos = catalog.GetAutoridadesEntregaActivas(corporation);
            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }
        public IActionResult Drop_AutoridadesEntrega()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var corporation = corp < 2 ? 1 : corp;
            var catalogos = catalog.GetAutoridadesEntrega(corporation);
            var result = new SelectList(catalogos, "value", "text");


            return Json(result);
        }
        public IActionResult Drop_EntidadesActivas()
        {
            var corp = 1;

            var catalogos = catalog.GetEntidadesActivas(corp);
            var result = new SelectList(catalogos, "value", "text");
            return Json(result);
        }

        public IActionResult Drop_EntidadesActivasPorCorporacion()
        {
            var corp = 1;// HttpContext.Session.GetInt32("IdDependencia").Value;
            var catalogos = catalog.GetEntidadesActivasPorCorporacion(corp);
            var result = new SelectList(catalogos, "value", "text");
            return Json(result);
        }
        public IActionResult Drop_Entidades()
        {
            var catalogos = catalog.GetEntidades();
            var result = new SelectList(catalogos, "value", "text");
            return Json(result);
        }

        public JsonResult Drop_MunicipiosActivosPorEntidad(int entidadDDlValue,int IdMun)
        {

            if (entidadDDlValue > 0)
            {
                var tt = new List<CatalogModel>();   

                
                if(IdMun > 0)
                   tt= catalog.GetMunicipiosActivosPorEntidad(entidadDDlValue, IdMun);
                else
                    tt = catalog.GetMunicipiosActivosPorEntidad(entidadDDlValue);

                tt.Add(new CatalogModel() { value = "49", text = "No aplica" });
                tt.Add(new CatalogModel() { value = "47", text = "No especificado" });
                var result = new SelectList(tt, "value", "text");
                return Json(result);
            }
            else
            {
                var tt = new List<CatalogModel>();
                if (IdMun > 0)
                    tt = catalog.GetMunicipiosActivosPorEntidad(entidadDDlValue, IdMun);

                var result = new SelectList(tt, "value", "text");
                return Json(result);
            }
        }


        public IActionResult Drop_MunicipiosTodos(int entidadDDlValue)
        {
            if (entidadDDlValue == 0)
            {
                entidadDDlValue = 11;
            }

            var tt = catalog.GetMunicipiosTodosPorEntidad(entidadDDlValue);

            tt.Add(new CatalogModel() { value = "49", text = "No aplica" });
            tt.Add(new CatalogModel() { value = "47", text = "No especificado" });

            var result = new SelectList(tt, "value", "text");

            return Json(result);
        }

        public IActionResult Drop_CarreterasActivas()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var catalogos = catalog.GetCarreterasActivasPorOficina(idOficina);
            var result = new SelectList(catalogos, "value", "text");

            return Json(result);
        }

        public IActionResult Drop_TodasCarreteras()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var catalogos = catalog.GetTodasCarreterasPorOficina(idOficina);
            var result = new SelectList(catalogos, "value", "text");

            return Json(result);
        }
        public IActionResult Drop_TramosActivos(int carreteraDDValue)
        {
            var catalogos = catalog.GetTramosActivosPorCarretera(carreteraDDValue);
            var result = new SelectList(catalogos, "value", "text");

            return Json(result);
        }

        public IActionResult Drop_TramosTodos(int carreteraDDValue)
        {
            var catalogos = catalog.GetTodoTramosPorCarretera(carreteraDDValue);
            var result = new SelectList(catalogos, "value", "text");

            return Json(result);
        }

        public IActionResult Drop_OficialesActivos()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var result = Oficiales_DropSource(() => _oficialesService.GetOficialesActivosFiltrados(idOficina, idDependencia));
            return Json(result);
        }

        public IActionResult Drop_Oficiales()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var result = Oficiales_DropSource(() => _oficialesService.GetOficialesFiltrados(idOficina, idDependencia));
            return Json(result);
        }

        private SelectList Oficiales_DropSource(Func<List<CatOficialesModel>> fecthFunc)
        {
            List<CatOficialesModel> source = fecthFunc();
            var oficiales = source.Select(o => new CatalogModel
            {
                value = o.IdOficial.ToString(),
                text = $"{o.Nombre} {o.ApellidoPaterno} {o.ApellidoMaterno}"
            });
            var result = new SelectList(oficiales, "value", "text");
            return result;
        }

        public JsonResult Oficiales_DropCorpTodos()
        {
            //int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
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

        public IActionResult Drop_AgenciasActivas()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var corporation = corp < 2 ? 1 : corp;
            var catalogos = catalog.ObtenerAgenciasActivas(corporation);
            var result = new SelectList(catalogos, "value", "text");
            return Json(result);
        }
        public IActionResult Drop_Agencias()
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var corporation = corp < 2 ? 1 : corp;
            var catalogos = catalog.ObtenerAgencias(corporation);
            var result = new SelectList(catalogos, "value", "text");
            return Json(result);
        }


        public IActionResult Municipios_Por_Delegacion_Drop()
        {

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var tt = _catMunicipiosService.GetMunicipiosPorDelegacion2(idOficina);

            tt.Add(new CatMunicipiosModel() { IdMunicipio = 1, Municipio = "No aplica" });
            tt.Add(new CatMunicipiosModel() { IdMunicipio = 2, Municipio = "No especificado" });
            var result = new SelectList(tt, "IdMunicipio", "Municipio");
            return Json(result);

            //var catalogos = catalog.ObtenerAgencias();
            //var result = new SelectList(catalogos, "value", "text");
            //return Json(result);
        }

        public IActionResult Municipios_Drop()
        {

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var tt = _catMunicipiosService.GetMunicipiosPorDelegacion2(corp);

            tt.Add(new CatMunicipiosModel() { IdMunicipio = 1, Municipio = "No aplica" });
            tt.Add(new CatMunicipiosModel() { IdMunicipio = 2, Municipio = "No especificado" });
            var result = new SelectList(tt, "IdMunicipio", "Municipio");
            return Json(result);

            //var catalogos = catalog.ObtenerAgencias();
            //var result = new SelectList(catalogos, "value", "text");
            //return Json(result);
        }

        public IActionResult GetAllPuestosActivos()
        {
            var catalogos = catalog.GetAllPuestosActivos();
            var result = new SelectList(catalogos, "value", "text");

            return Json(result);
        }

        public JsonResult DropCatalogos()
        {

            var result = new SelectList(catalog.ObtieneListaCatalogos(), "idCatalogo", "catalogo");
            return Json(result);
        }

        public JsonResult CorporacionesDrop()
        {

            var result = new SelectList(catalog.ObtieneTodasCorporaciones(), "IdDependencia", "NombreDependencia");
            return Json(result);
        }

    }
}
