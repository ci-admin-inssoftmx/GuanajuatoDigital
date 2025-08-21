using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Framework;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class RelacionesController : BaseController
    {
        DBContextInssoft dbContext = new DBContextInssoft();
        private readonly ICatAutoridadesDisposicionService _catAutoridadesDisposicionservice;
        private readonly ICatalogosService _catalogosService;
        private readonly ICatDictionary _catDictionary;
        private readonly IRelacionService _relacion;

        public RelacionesController(ICatAutoridadesDisposicionService catAutoridadesDisposicionservice, ICatalogosService catalogosService, ICatDictionary catDictionary,IRelacionService relacionService)
        {
            _catAutoridadesDisposicionservice = catAutoridadesDisposicionservice;
            _catalogosService = catalogosService;
            _catDictionary = catDictionary;
            _relacion=relacionService;
        }

        public IActionResult Index()
        {

            //var ListColoresModel = GetColores();
            return View();
        }

        public IActionResult GetDataParent([DataSourceRequest] DataSourceRequest request,int id,int IdC)
        {
            var r = _relacion.GetDataParent(id,IdC);

            return Json( r.ToDataSourceResult(request));
        }

        public IActionResult GetData([DataSourceRequest] DataSourceRequest request, int id)
        {
            var r = _relacion.GetData(id);

            return Json(r.ToDataSourceResult(request));
        }

        public IActionResult GetCatalogParents(int Id)
        {
            var catalog = new List<CatalogModel>();
            
            if(Id == 0)
            {
                return Json(catalog);
            }

            catalog.Add(new CatalogModel { value = "-1", text = "Todos" });
            catalog.AddRange(_relacion.GetCatalog(Id));

            return Json(catalog);
        }

        public IActionResult CreateParent(int id , string desc, string desc2)
        {
            var can = _relacion.InsertParent(id,desc, desc2);

            return Json(new { data = can });
        }


        public IActionResult AddParent(int id , int idP , int idO)
        {
            var r = _relacion.updateParent(id,idP,idO);
            return Json(r);
        }

        public IActionResult AddParentnull(int id, int idP, int idO)
        {
            var r = _relacion.updateParentNull(id, idP, idO);
            return Json(r);
        }


    }

}
