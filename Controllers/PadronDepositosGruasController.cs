using GuanajuatoAdminUsuarios.Framework;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using static GuanajuatoAdminUsuarios.Models.PadronDepositosGruasModel;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class PadronDepositosGruasController : BaseController
    {


        private readonly IPadronDepositosGruasService _padronDepositosGruasService;
        private readonly IGruasService _gruasService;
        private readonly IMunicipiosService _municipiosService;
        private readonly IConcesionariosService _concesionariosService;
        private readonly ICatDictionary _catDictionary;
        private readonly ICatMunicipiosService _catMunicipiosService;


        public PadronDepositosGruasController(IPadronDepositosGruasService padronDepositosGruasService,
             IGruasService gruasService, IMunicipiosService municipiosService, IConcesionariosService concesionariosService
            , ICatDictionary catDictionary,ICatMunicipiosService catMunicipiosService

            )
        {
            _padronDepositosGruasService = padronDepositosGruasService;
            _gruasService = gruasService;
            _municipiosService = municipiosService;
            _concesionariosService = concesionariosService;
            _catDictionary = catDictionary;
            _catMunicipiosService = catMunicipiosService;
        }


        public IActionResult Index()
        {
           
                PadronDepositosGruasBusquedaModel searchModel = new PadronDepositosGruasBusquedaModel();
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

               // List<PadronDepositosGruasModel> listPadronDepositosGruas = _padronDepositosGruasService.GetAllPadronDepositosGruas(idOficina);
                //searchModel.ListPadronDepositosGruas = listPadronDepositosGruas;
                return View(searchModel);
            
          
        }

        [HttpPost]
        public ActionResult ajax_BuscarPadron(PadronDepositosGruasBusquedaModel model)
        {
       
                int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

                var ListPadronDepositosGruas = _padronDepositosGruasService.GetPadronDepositosGruas(model, idOficina);
                if (ListPadronDepositosGruas.Count == 0)
                {
                    ViewBag.NoResultsMessage = "No se encontraron grúas que cumplan con los criterios de búsqueda.";
                }

                return PartialView("_ListadoPadron", ListPadronDepositosGruas);
            }
  


        public JsonResult Municipios_Read()
        {
            var CatMunicipios = _catDictionary.GetCatalog("CatMunicipios", "0");
            var result = new SelectList(CatMunicipios.CatalogList, "Id", "Text");
            return Json(result);
        }
        public JsonResult Municipios_Guanajuato()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

			var result = new SelectList(_catMunicipiosService.GetMunicipiosGuanajuatoActivos(corp), "IdMunicipio", "Municipio");
            return Json(result);
        }


        public JsonResult Municipios_Guanajuatofilter(int del)
        {
            			var corp = HttpContext.Session.GetInt32("IdDependencia").Value;

            var result = new SelectList(_catMunicipiosService.GetMunicipiosPorDelegacion2(del,corp), "IdMunicipio", "Municipio");
            return Json(result);
        }


        public JsonResult Concesionarios_Read()
        {

            var result = new SelectList(_concesionariosService.GetAllConcesionariosConMunicipio(), "IdConcesionario", "Concesionario");
            return Json(result);
        }

        public JsonResult Deposito_Read()
        {

            var result = new SelectList(_padronDepositosGruasService.GetPensionesNoFilter(), "IdPension", "Pension");
            return Json(result);
        }

        public JsonResult TipoGrua_Read()
        {
            var result = new SelectList(_gruasService.GetTipoGruas(), "IdTipoGrua", "TipoGrua");
            return Json(result);
        }

        public List<PadronDepositosGruasModel> GetDepositos(int idOficina)
        {
            var modelList = _padronDepositosGruasService.GetAllPadronDepositosGruas(idOficina);

            var indices = modelList
                         .Select((s, i) => new { index = i, item = s })
                         .GroupBy(grp => grp.item.IdPension)
                         //.Where(w => !string.IsNullOrEmpty(w.))
                         .SelectMany(sm => sm.Select(s => s.index))
                         .ToList();

            var ListaAgrupada = modelList.Select((s, i) => new { index = i, items = s })
                             .GroupBy(x => indices.FirstOrDefault(r => r > x.index))
                             .Select(s => s.Select(ss => ss.items).ToList())
                             .ToList();

            List<PadronDepositosGruasModel> ListItems = new List<PadronDepositosGruasModel>();
            List<PensionPadronModel> padronPension = new List<PensionPadronModel>();
            foreach (var item in ListaAgrupada)
            {

                if (item.Count() > 1)
                {
                    foreach (var itemInside in item)
                    {
                        PensionPadronModel pension = new PensionPadronModel();

                        pension.IdPension = itemInside.IdPension;
                        pension.Pension = itemInside.Pension;
                        pension.Telefono = itemInside.Telefono;
                        pension.Direccion = itemInside.Direccion;
                        pension.IdMunicipio = itemInside.IdMunicipio;
                        padronPension.Add(pension);
                    }

                    var one = item.FirstOrDefault();
                    one.Pensiones = padronPension;
                    ListItems.Add(one);
                }
                else
                {
                    ListItems.Add(item.First());
                }
            }
            return ListItems;

        }

    }
}
