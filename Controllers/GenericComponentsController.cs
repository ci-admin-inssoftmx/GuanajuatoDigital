using GuanajuatoAdminUsuarios.Framework;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class GenericComponentsController : BaseController
    {
        private readonly ICatDictionary _catDictionary;
        public GenericComponentsController(IViewRenderService viewRenderService, ICatDictionary catDictionary) : base(viewRenderService)
        {
            _catDictionary = catDictionary;
        }

        [HttpPost]
        public ActionResult GetComboByCatalog(string htmlName, string isRequired, string catalog, string parameter, string functionName, string multiple)
        {
            var listNameCatalogs = catalog.Split(',').ToList();
            var listParameters = parameter.Split(',').ToList();
            var catalogsParamsList = listNameCatalogs.Zip(listParameters, (s, i) => new string[] { s, i }).ToList();

            var modelList = catalogsParamsList
                .Where(w => !string.IsNullOrEmpty(w[0]) && w[0].Contains("Cat"))
                .Distinct()
                .Select(s => _catDictionary.GetCatalog(s[0], s[1]))
                .ToList();

            ViewBag.multiple = multiple == "true";
            ViewBag.ModelList = modelList.FirstOrDefault().CatalogList;
            ViewBag.isRequired = isRequired == "true";
            ViewBag.htmlName = htmlName;
            ViewBag.functionName = functionName;
            return PartialView("_GenericDropDownCatalogs");
        }


    }
}
