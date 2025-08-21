using GuanajuatoAdminUsuarios.Framework;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class CatAlertamiento : BaseController
    {

        ICatAlertamientoServices _servAletamiento;
        private readonly ICatDictionary _catDictionary;

        public CatAlertamiento(ICatAlertamientoServices serAlertamiento,ICatDictionary catDictionary)
        {
            _servAletamiento = serAlertamiento;
            _catDictionary = catDictionary; 
        }
        //AdminViews
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult EditarAlertamientoModal(int idService)
        {
            var serviceModel = _servAletamiento.Getdata(idService);
            return PartialView("_Editar", serviceModel);
        }

        public ActionResult CrearServiceModal()
        {
            
            return PartialView("_Crear");
        }

        //Function Ajaxs

        public IActionResult Ajax_EditarAlertamiento(int cantidad,int idAlertamiento)
        {
            _servAletamiento.EditarAlertamiento(idAlertamiento, cantidad);

            return Json(true);
        }

        public IActionResult GetDataGrid([DataSourceRequest] DataSourceRequest request)
        {
            var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
            var data = _servAletamiento.GetdataGrid(corp);
            return Json(data.ToDataSourceResult(request));

        }

        public IActionResult GetAplicada()
        {
            var CatAplicadoA = _catDictionary.GetCatalog("CatAplicadoA", "0");
            return Json(CatAplicadoA.CatalogList);
        }

        public IActionResult Ajax_CrearService(int cantidad,int idAplicacion,int Delegacion)
        {
            var i =_servAletamiento.CrearAlertamiento(cantidad, idAplicacion, Delegacion);
            if (i == 0)
            {
                return Json(false);
            }
            return Json(true);
        }


        public IActionResult GetCorpCatalog()
        {
            var data = _servAletamiento.GetCorpCatalog();
            return Json(data);
        }
        public IActionResult GetAplicadaCatalog(string idCorp)
        {
            int cp;
            List<CatalogModel> data = new List<CatalogModel>(); 
             if (int.TryParse(idCorp, out cp))
             {
                 data = _servAletamiento.GetAplicadaCatalog(cp);
             }            
            return Json(data);
        }

    }
}
