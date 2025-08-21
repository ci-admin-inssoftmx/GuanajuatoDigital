using DocumentFormat.OpenXml.Spreadsheet;
using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Interfaces.Blocs;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Services.Blocs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class CatCamposObligatoriosController : BaseController
    {
        private readonly ICatCamposObligatoriosService _catCamposObligatoriosService;

        public CatCamposObligatoriosController(ICatCamposObligatoriosService catCamposObligatoriosService)
        {
            _catCamposObligatoriosService = catCamposObligatoriosService;
        }

        DBContextInssoft dbContext = new DBContextInssoft();
        public IActionResult Index()
        {
            //int idCampoObligatorio = (int)HttpContext.Session.GetInt32("IdCampoObligatorio");
            HttpContext.Session.SetString("SelectedModulo", "CatCamposObligatorios");
            var ListCatCamposObligatoriosTModel = _catCamposObligatoriosService.GetCamposObligatorios();
            return View("Index", ListCatCamposObligatoriosTModel);
        }




        #region Modal Action
        public ActionResult IndexModal()
        {
            int idCampoObligatorio = (int)HttpContext.Session.GetInt32("IdCampoObligatorio");
            var ListCatCamposObligatoriosTModel = _catCamposObligatoriosService.GetCamposObligatorios();
            return View("Index", ListCatCamposObligatoriosTModel);
        }

        [HttpPost]
        public ActionResult AgregarCamposObligatoriosMod(CatCamposObligatoriosModel model)
        {
            var errors = ModelState.Values.Select(s => s.Errors);
            Console.WriteLine(model);
            ModelState.Remove("IdCampoObligatorio");
            if (ModelState.IsValid)
            {

                CrearOActualizarCamposObligatorios(model);
                var ListCatCamposObligatoriosTModel = GetCamposObligatorios();
                return Json(ListCatCamposObligatoriosTModel);
            }

            return PartialView("_Crear");
        }

        public JsonResult GetCamposObligatoriosJSON([DataSourceRequest] DataSourceRequest request)
        {
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            var ListCatCamposObligatoriosTModel = _catCamposObligatoriosService.GetCamposObligatorios();
            return Json(ListCatCamposObligatoriosTModel.ToDataSourceResult(request));
        }

        #endregion

        #region Acciones a base de datos

        public void CrearOActualizarCamposObligatorios(CatCamposObligatoriosModel model)
        {
            try
            {
                // Buscar el registro existente
                var existingRecord = dbContext.CatCamposObligatorios
                    .SingleOrDefault(c => c.IdCampo == model.IdCampo);

                if (existingRecord != null)
                {
                    // Actualizar el registro existente
                    existingRecord.Accidentes = model.EstatusAccidente;
                    existingRecord.Infracciones = model.EstatusInfracciones;
                    existingRecord.Depositos = model.EstatusDepositos;
                    existingRecord.FechaActualizacion = DateTime.Now;

                    // Cambiar el estado del registro a Modificado
                    dbContext.Entry(existingRecord).State = EntityState.Modified;
                    dbContext.CatCamposObligatorios.Update(existingRecord);
                }
                else
                {
                    // Crear un nuevo registro
                    var newRecord = new CatCamposObligatorios
                    {
                        IdCampoObligatorio = model.IdCampoObligatorio,
                        IdDelegacion = (int)HttpContext.Session.GetInt32("IdOficina"),
                        IdMunicipio = (int)HttpContext.Session.GetInt32("IdDependencia"),
                        IdCampo = model.IdCampo,
                        Accidentes = model.EstatusAccidente,
                        Infracciones = model.EstatusInfracciones,
                        Depositos = model.EstatusDepositos,
                        FechaActualizacion = DateTime.Now,
                        ActualizadoPor = (int?)Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value)
                };

                    dbContext.CatCamposObligatorios.Add(newRecord);
                }

                // Guardar los cambios en la base de datos
                dbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Manejo de errores específicos para la actualización de base de datos
                // Aquí puedes registrar el error o lanzar una excepción personalizada
                throw new ApplicationException("Error al actualizar o crear el registro en la base de datos.", ex);
            }
        }



        public List<CatCamposObligatoriosModel> GetCamposObligatorios()
        {
            var listCatCamposObligatoriosModel = dbContext.CatCamposObligatorios
                                                .Join(dbContext.CatMunicipios,
                                                    cco => cco.IdMunicipio,
                                                    cm => cm.IdMunicipio,
                                                    (cco, cm) => new { cco, cm })
                                                .Join(dbContext.CatDelegaciones,
                                                    combined => combined.cco.IdDelegacion,
                                                    cd => cd.IdDelegacion,
                                                    (combined, cd) => new { combined.cco, combined.cm, cd })
                                                .Join(dbContext.CatCampos,
                                                    combined => combined.cco.IdCampo,
                                                    cf => cf.IdCampo,
                                                    (combined, cf) => new CatCamposObligatoriosModel
                                                    {
                                                        IdCampoObligatorio = combined.cco.IdCampoObligatorio,
                                                        IdDelegacion = combined.cco.IdDelegacion,
                                                        Delegacion = combined.cd.Delegacion,
                                                        IdMunicipio = combined.cco.IdMunicipio,
                                                        Municipio = combined.cm.Municipio,
                                                        IdCampo = combined.cco.IdCampo,
                                                        Campo = cf.NombreCampo, // Se mapea a "Campo" en lugar de "NombreCampo"
                                                        EstatusAccidente = combined.cco.Accidentes, // Asumiendo que "Accidentes" corresponde a "EstatusAccidente"
                                                        EstatusInfracciones = combined.cco.Infracciones, // Asumiendo que "Infracciones" corresponde a "EstatusInfracciones"
                                                        EstatusDepositos = combined.cco.Depositos, // Asumiendo que "Depositos" corresponde a "EstatusDepositos"
                                                        FechaActualizacion = null, // Valor predeterminado si no está disponible en la consulta
                                                        ActualizadoPor = null // Valor predeterminado si no está disponible en la consulta
                                                    })
                                                .OrderByDescending(result => result.IdCampoObligatorio)
                                                .ToList();
            return listCatCamposObligatoriosModel;
        }
        #endregion



    }
}
