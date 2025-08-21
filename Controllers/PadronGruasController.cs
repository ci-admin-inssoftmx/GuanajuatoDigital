using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class PadronGruasController : BaseController
    {
        private readonly ICatDictionary _catDictionary;
        private readonly IGruasService _gruasService;
        private readonly IConcesionariosService _concesionariosService;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;
        private readonly IEstadisticasAccidentesService _estadisticasAccidentesService;

        public PadronGruasController(ICatDictionary catDictionary,
                                     IGruasService gruasService,
                                     IConcesionariosService concesionariosService,
                                     IEstadisticasAccidentesService estadisticasAccidentesService,

                                     ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService
            
            )
        {
            _catDictionary = catDictionary;
            _gruasService = gruasService;
            _concesionariosService = concesionariosService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
            _estadisticasAccidentesService = estadisticasAccidentesService;


        }
        public IActionResult Index()
        {

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            //IEnumerable<Gruas2Model> listGruas = _gruasService.GetAllGruas(idOficina);
            var listGruas = new List<Gruas2Model>();
            var catTipoGruas = _catDictionary.GetCatalog("CatTiposGrua", "0");
            var catDelegaciones = _estadisticasAccidentesService.GetDelegacionesFilter();
            



            // var catConcesionario = _concesionariosService.GetAllConcesionariosConMunicipio(idOficina);

            ViewBag.CatTipoGruas = new SelectList(catTipoGruas.CatalogList, "Id", "Text");
            ViewBag.CatDelegaciones = new SelectList(catDelegaciones, "value", "text");
            // ViewBag.CatConcesionario = new SelectList(catConcesionario, "Id", "Text");

            return View(listGruas);

        }
        public JsonResult Concesionarios_Drop()
        {
            var result = new SelectList(_concesionariosService.GetAllConcesionariosConMunicipio(), "IdConcesionario", "Concesionario");
            return Json(result);
        }
        public JsonResult Delegaciones_Drop()
        {
            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasActivos(), "IdDelegacion", "Delegacion");
            return Json(result);
        }
        public JsonResult Concecionarios_Drop()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var result = new SelectList(_concesionariosService.GetConcesionarios(idOficina), "IdConcesionario", "Concesionario");
            return Json(result);
        }
        [HttpGet]
        public ActionResult ajax_BuscarGruas(string placas, string noEconomico, int? idTipoGrua, int? idDelegacion, int? idConcesionario)
        {

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var listPadronGruas = _gruasService.GetGruasToGrid(placas, noEconomico, idTipoGrua, idOficina, idDelegacion, idConcesionario);

            return PartialView("_ListadoGruas", listPadronGruas);
        }


        /// <summary>
        /// Accion que redirige a la vista
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ajax_create()
        {

            var catDelegaciones = _estadisticasAccidentesService.GetDelegacionesFilter();
            var catClasificacionGruas = _catDictionary.GetCatalog("CatClasificacionGruas", "0");
            var catTipoGruas = _catDictionary.GetCatalog("CatTiposGrua", "0");
            var catSituacionGruas = _catDictionary.GetCatalog("CatSituacionGruas", "0");
            var catConcesionario = _concesionariosService.GetAllConcesionariosConMunicipio();
            ViewBag.CatConcesionario = new SelectList(catConcesionario, "IdConcesionario", "Concesionario");
            ViewBag.CatDelegaciones = new SelectList(catDelegaciones, "value", "text");
            ViewBag.CatClasificacionGruas = new SelectList(catClasificacionGruas.CatalogList, "Id", "Text");
            ViewBag.CatTipoGruas = new SelectList(catTipoGruas.CatalogList, "Id", "Text");
            ViewBag.CatSituacionGruas = new SelectList(catSituacionGruas.CatalogList, "Id", "Text");
            return PartialView("_CrearGrua", new Gruas2Model());
        }


        [HttpPost]
        public IActionResult ajax_createGrua(Gruas2Model model)
        {
            //var model = json.ToObject<Gruas2Model>();
            //var errors = ModelState.Values.Select(s => s.Errors);
            //if (ModelState.IsValid)
            //{
                int index = _gruasService.CrearGrua(model);
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var listPadronGruas = _gruasService.GetAllGruas(idOficina);
                return PartialView("_ListadoGruas", listPadronGruas);
            //}
            //return RedirectToAction("_ListadoGruas");
        }
        /// <summary>
        /// Accion que redirige a la vista
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ajax_edit(int idGrua)
        {

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var model = _gruasService.GetGruaById(idGrua);
            var catConcesionarios = _concesionariosService.GetAllConcesionarios();
            var catDelegacione = _catDictionary.GetCatalog("CatDelegaciones", "0");
            var catClasificacionGruas = _catDictionary.GetCatalog("CatClasificacionGruas", "0");
            var catTipoGruas = _catDictionary.GetCatalog("CatTiposGrua", "0");
            var catSituacionGruas = _catDictionary.GetCatalog("CatSituacionGruas", "0");
            ViewData["CatDelegaciones"] = new SelectList(catDelegacione.CatalogList, "Id", "Text");
            ViewData["CatClasificacionGruas"] = new SelectList(catClasificacionGruas.CatalogList, "Id", "Text");
            ViewData["CatTipoGruas"] = new SelectList(catTipoGruas.CatalogList, "Id", "Text");
            ViewData["CatSituacionGruas"] = new SelectList(catSituacionGruas.CatalogList, "Id", "Text");
            ViewData["CatConcesionarios"] = new SelectList(catConcesionarios, "IdConcesionario", "Concesionario");
            return PartialView("_EditarGrua", model);
        }
    
            public IActionResult ajax_activar(int idGrua)
        {

            var model = _gruasService.GetGruaById(idGrua);           
            return PartialView("_DesactivarGrua", model);
        }
        [HttpPost]

        public IActionResult ajax_desactivarGrua(Gruas2Model model)
        {    
                int index = _gruasService.EliminarGrua(model);
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var listPadronGruas = _gruasService.GetAllGruas(idOficina);
                return PartialView("_ListadoGruas", listPadronGruas);
                       
        }
        [HttpPost]
        public IActionResult ajax_editGrua(Gruas2Model model)
        {
            //var model = json.ToObject<Gruas2Model>();
            var errors = ModelState.Values.Select(s => s.Errors);
            if (ModelState.IsValid)
            {
                int index = _gruasService.EditarGrua(model);
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var listPadronGruas = _gruasService.GetAllGruas(idOficina);
                return PartialView("_ListadoGruas", listPadronGruas);
            }
            return RedirectToAction("Index");
        }

    }
}
