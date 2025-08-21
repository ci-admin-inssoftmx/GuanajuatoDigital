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
    public class PensionesController : BaseController
    {
        #region DPIServices

        private readonly IPensionesService _pensionesService;
        private readonly ICatDictionary _catDictionary;
        private readonly IBitacoraService _bitacoraServices;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;
        public PensionesController(
            ICatDictionary catDictionary,
            IPensionesService pensionesService,
            IBitacoraService bitacoraServices,
            ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService)
        {
            _pensionesService = pensionesService;
            _catDictionary = catDictionary;
            _bitacoraServices = bitacoraServices;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
        }


        #endregion

        public IActionResult Index()
        {

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            // List<PensionModel> pensionesList = _pensionesService.GetAllPensiones(idOficina);
            //  var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            //  ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
            return View();

        }



        public IActionResult EditarPensionEstatus(bool PensionSwitch, int idestatus)
        {
            var can = _pensionesService.EstatusPension(PensionSwitch, idestatus);


            return ajax_BuscarPensiones("", null);
        }



        [HttpGet]
        public ActionResult ajax_BuscarPensiones(string pension, int? idDelegacion)
        {

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var ListPensionesModel = _pensionesService.GetPensionesToGrid(pension, idDelegacion);
            if (ListPensionesModel.Count == 0 || (string.IsNullOrEmpty(pension) && idDelegacion == null))
            {
                ListPensionesModel = new List<PensionModel>();
                ViewBag.NoResultsMessage = "No se encontraron registros que cumplan con los criterios de búsqueda.";
            }
            return PartialView("_ListadoPensiones", ListPensionesModel);

        }


        [HttpPost]
        public ActionResult ajax_ModalCrearPension()
        {

            //var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            int idDependencia = 1;

            ViewBag.CatDelegaciones = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasByDependencia(idDependencia), "IdOficinaTransporte", "NombreOficina");

            var catResponsablesPensiones = _catDictionary.GetCatalog("CatResponsablesPensiones", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");

            //ViewBag.CatDelegaciones = catDelegaciones ; //new SelectList(catDelegaciones, "IdOficinaTransporte", "NombreOficina");
            ViewBag.CatResponsablesPensiones = new SelectList(catResponsablesPensiones.CatalogList, "Id", "Text");
            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");
            return PartialView("_CrearPension", new PensionModel());
        }

        public JsonResult Delegaciones_DropExt()
        {
            int idDependencia = 1;

            var result = new SelectList(_catDelegacionesOficinasTransporteService.GetDelegacionesOficinasByDependencia(idDependencia), "IdOficinaTransporte", "NombreOficina");
            return Json(result);
        }



        [HttpPost]
        public ActionResult ajax_CrearPension(PensionModel model)
        {
            //var errors = ModelState.Values.Select(s => s.Errors);
            //ModelState.Remove("CategoryName");

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            int idPension = _pensionesService.CrearPension(model);
            List<Gruas2Model> gruasPensionesList = _pensionesService.GetGruasDisponiblesByIdPension(idPension, idOficina);

            model.IdPension = idPension;
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);

            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var corporation = corp < 2 ? 1 : corp;
            var ListPensionesModel = new List<PensionModel>();
            //if (corporation == 1)
              //  ListPensionesModel = _pensionesService.GetAllPensiones2();

            //BITACORA
            //_bitacoraServices.insertBitacora(idPension, ip, "Pension", "Insertar", "Insert", user);
            object data = new { catalogo = "Pensiones" };
            _bitacoraServices.BitacoraGenerales(CodigosGeneral.C5004, data);
            var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            var catResponsablesPensiones = _catDictionary.GetCatalog("CatResponsablesPensiones", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");

            ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
            ViewBag.CatResponsablesPensiones = new SelectList(catResponsablesPensiones.CatalogList, "Id", "Text");
            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");

            ViewBag.ListadoGruasPensiones = gruasPensionesList;
            return PartialView("_ListadoPensiones", ListPensionesModel);

        }




        public JsonResult Consesionarios(int idDEl)
        {

            var data = _pensionesService.GetConcesionarios(idDEl);
            var result = new SelectList(data, "value", "text");
            return Json(result);
        }


        public IActionResult ajax_InserAsociado(int idPEncion, int idAsociado)
        {

            var result = 0;

            var can = _pensionesService.ExistData(idPEncion, idAsociado);

            if (can)
            {
                var data = _pensionesService.InsertAsociado(idPEncion, idAsociado);
                result = 1;
            }



            return Json(result);
        }




        [HttpGet]
        public ActionResult ajax_ModalEditarPension(int idPension)
        {
            HttpContext.Session.SetInt32("IdPenciondata", idPension);

            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;


            var model = _pensionesService.GetPensionById(idPension, idOficina).FirstOrDefault();

            var gruasPensionesList = _pensionesService.GetGruasDisponiblesByIdPension(model.IdPension, idOficina);

            var catDelegaciones = _catDictionary.GetCatalog("CatDelegaciones", "0");
            var catResponsablesPensiones = _catDictionary.GetCatalog("CatResponsablesPensiones", "0");
            var catMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");

            ViewBag.CatDelegaciones = new SelectList(catDelegaciones.CatalogList, "Id", "Text");
            ViewBag.CatResponsablesPensiones = new SelectList(catResponsablesPensiones.CatalogList, "Id", "Text");
            ViewBag.CatMunicipios = new SelectList(catMunicipios.CatalogList, "Id", "Text");

            ViewBag.ListadoGruasPensiones = gruasPensionesList;
            return PartialView("_EditarPension", model);
        }



        public IActionResult ajax_GetListAsociadosPenciones([DataSourceRequest] DataSourceRequest request)
        {
            int idPension = HttpContext.Session.GetInt32("IdPenciondata").Value;
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            var gruasPensionesList = _pensionesService.GetAsociados(idPension, idOficina);


            return Json(gruasPensionesList.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult ajax_EditarPension(PensionModel model)
        {
            var ListPensionesModel = new List<PensionModel>();
            if (ModelState.IsValid)
            {
                int idPension = _pensionesService.EditarGrua(model);
                int eliminaGruas = _pensionesService.EliminarPensionGruas(model.IdPension);
                if (!string.IsNullOrEmpty(model.strIdGruas))
                {
                    var strListIdGruas = model.strIdGruas.Split(',').Select(s => Convert.ToInt32(s)).ToList();
                    int altaGruas = _pensionesService.CrearPensionGruas(model.IdPension, strListIdGruas);
                }
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                //BITACORA
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                //_bitacoraServices.insertBitacora(idPension, ip, "Pension", "Actualizar", "Update", user);
                object data = new { catalogo = "Pensiones" };
                _bitacoraServices.BitacoraGenerales(CodigosGeneral.C5007, data);

                var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
                var corporation = corp < 2 ? 1 : corp;

                if (corporation == 1)
                {
                    ListPensionesModel = _pensionesService.GetAllPensiones2();

                }

                return PartialView("_ListadoPensiones", ListPensionesModel);
            }
            //SetDDLCategories();
            //return View("Create");
            return PartialView("_ListadoPensiones", ListPensionesModel);
        }

        [HttpPost]
        public ActionResult ajax_CargaGeolocalizacion(PensionModel model)
        {
            try
            {
                return PartialView("_CargaGeolocalizacion", model);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult ajax_VerGeolocalizacion(PensionModel model)
        {
            try
            {
                return PartialView("_VerGeolocalizacion", model);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
