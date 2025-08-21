using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class FirmasController : BaseController
    {
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;
        private readonly IDelegacionesService _delegacionesService;
        private readonly string _rutaArchivo;

        public FirmasController(ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService, IConfiguration configuration, IDelegacionesService delegacionesService)
        {
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
            _delegacionesService = delegacionesService;
            _rutaArchivo = configuration.GetValue<string>("AppSettings:RutaArchivosFirmas");
        }

        public IActionResult Index()
        {
            return View(new FirmasListModel() { firmasList = new List<FirmasModel>() });
        }

        public IActionResult Alta()
        {
            return View("AgregarFirma");
        }

        public ActionResult Editar(int id)
        {
            var existingData = _catDelegacionesOficinasTransporteService.GetDelegacionOficinaTransporteFirmasById(id);
            var model = new FirmasModel()
            {
                Id = existingData.IdOficinaTransporte,
                DependenciaNombre = existingData.NombreOficina,
                FileUrl = existingData.UrlFirma,
                IdPuesto = existingData.IdPuesto,
                Nombre = existingData.JefeOficina,
                FechaInicio = existingData.FechaInicio == null ? "-" : existingData.FechaInicio.Value.ToString("dd/MM/yyyy"),
                FechaFin = existingData.FechaFin == null ? "-" : existingData.FechaFin.Value.ToString("dd/MM/yyyy"),
                Estatus = existingData.Estatus == null ? "-" : existingData.Estatus.Value == 1 ? "Activo" : existingData.Estatus.Value == 1 ? "Inactivo" : "-",
                EstatusNumero = existingData.Estatus
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult GuardarFirma(IFormFile file, FirmasModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Se presentaron errores al momento del guardado." });
            }
            else
            {
                var idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
                var modelNewDelegacionesOficinasTransporte = new CatDelegacionesOficinasTransporteModel();

                if (idOficina != 0)
                {
                    var delegacion = _delegacionesService.GetDelegaciones().Where(x => x.IdDelegacion == idOficina).FirstOrDefault();

                    if (delegacion is not null)
                    {
                        modelNewDelegacionesOficinasTransporte.NombreOficina = delegacion.Delegacion;
                        modelNewDelegacionesOficinasTransporte.IdDelegacion = delegacion.IdDelegacion;

                        if (delegacion.IdMunicipio is not null)
                        {
                            modelNewDelegacionesOficinasTransporte.IdMunicipio = delegacion.IdMunicipio.Value;
                        }
                        else
                        {
                            modelNewDelegacionesOficinasTransporte.IdMunicipio = 0;
                        }
                    }
                }
                else
                {
                    modelNewDelegacionesOficinasTransporte.NombreOficina = "Sin delegación";
                    modelNewDelegacionesOficinasTransporte.IdMunicipio = 0;
                    modelNewDelegacionesOficinasTransporte.IdDelegacion = 0;
                }

                modelNewDelegacionesOficinasTransporte.JefeOficina = model.Nombre;
                modelNewDelegacionesOficinasTransporte.Estatus = 1;
                modelNewDelegacionesOficinasTransporte.FechaInicio = DateTime.ParseExact(model.FechaInicio, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                modelNewDelegacionesOficinasTransporte.FechaFin = DateTime.ParseExact(model.FechaFin, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                modelNewDelegacionesOficinasTransporte.IdPuesto = model.IdPuesto;

                if (file != null && file.Length > 0)
                {
                    string nombreArchivo = _rutaArchivo + "\\" + file.FileName;
                    modelNewDelegacionesOficinasTransporte.UrlFirma = nombreArchivo;
                    using (var stream = new FileStream(nombreArchivo, FileMode.Create))
                    {
                        file.CopyToAsync(stream);
                    }
                }
                else
                {
                    modelNewDelegacionesOficinasTransporte.UrlFirma = "none";
                }

                var existingId = _catDelegacionesOficinasTransporteService.GetDelegacionesOficinasTransporteFirmasRangoFecha(modelNewDelegacionesOficinasTransporte);
                if (existingId > 0)
                {
                    var updateEndDate =
                        _catDelegacionesOficinasTransporteService.UpdateDelegacionesOficinasTransporteFirmasFechaFin(existingId, modelNewDelegacionesOficinasTransporte.FechaInicio.Value.AddDays(-1));

                    if (updateEndDate <= 0)
                    {
                        return Json(new { success = false });
                    }
                }

                var responseId = _catDelegacionesOficinasTransporteService.InsertDelegacionesOficinasTransporteFirmas(modelNewDelegacionesOficinasTransporte);

                if (responseId > 0)
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult ActualizarFirma(IFormFile file, FirmasModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Se presentaron errores al momento del guardado." });
            }
            else
            {
                var existingData = _catDelegacionesOficinasTransporteService.GetDelegacionOficinaTransporteFirmasById(model.Id);

                existingData.IdPuesto = model.IdPuesto;
                existingData.JefeOficina = model.Nombre;
                existingData.Estatus = Convert.ToBoolean(model.Estatus) ? 1 : 0;
                existingData.FechaInicio = DateTime.ParseExact(model.FechaInicio, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                existingData.FechaFin = DateTime.ParseExact(model.FechaFin, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                if (file != null && file.Length > 0)
                {
                    string nombreArchivo = _rutaArchivo + "\\" + file.FileName;
                    existingData.UrlFirma = nombreArchivo;
                    using (var stream = new FileStream(nombreArchivo, FileMode.Create))
                    {
                        file.CopyToAsync(stream);
                    }
                }

                var existingId = _catDelegacionesOficinasTransporteService.GetDelegacionesOficinasTransporteFirmasRangoFecha(existingData);
                if (existingId > 0)
                {
                    var updateEndDate =
                        _catDelegacionesOficinasTransporteService.UpdateDelegacionesOficinasTransporteFirmasFechaFin(existingId, existingData.FechaInicio.Value.AddDays(-1));

                    if (updateEndDate <= 0)
                    {
                        return Json(new { success = false });
                    }
                }

                var responseId = _catDelegacionesOficinasTransporteService.UpdateDelegacionesOficinasTransporteFirmas(existingData);

                if (responseId > 0)
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false });
            }
        }

        public IActionResult GetAllFirmasPagination([DataSourceRequest] DataSourceRequest request, FirmasModel model)

        {
            var idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var modelList = _catDelegacionesOficinasTransporteService.GetDelegacionesOficinasTransporteFirmasByDelegacionId(idOficina);
            var modelListResponse = new List<FirmasModel>();

            if (modelList.Count > 0)
            {
                modelListResponse = modelList.Select(x =>
                    new FirmasModel()
                    {
                        Id = x.IdOficinaTransporte,
                        DependenciaNombre = x.NombreOficina,
                        Nombre = x.JefeOficina,
                        Estatus = x.Estatus == null ? "-" : x.Estatus.Value == 1 ? "Activo" : x.Estatus.Value == 0 ? "Inactivo" : "-",
                        FechaInicio = x.FechaInicio == null ? "-" : x.FechaInicio.Value.ToString("dd/MM/yyyy"),
                        FechaFin = x.FechaFin == null ? "-" : x.FechaFin.Value.ToString("dd/MM/yyyy"),
                        CargoNombre = x.NombrePuesto
                    }
                ).ToList();
            }

            var pagination = new Pagination();
            pagination.PageIndex = request.Page - 1;
            pagination.PageSize = (request.PageSize != 0) ? request.PageSize : 10;
            var total = 0;
            if (modelListResponse.Count() > 0)
                total = modelListResponse.Count;
            request.PageSize = pagination.PageSize;
            var result = new DataSourceResult()
            {
                Data = modelListResponse.Skip(pagination.PageIndex * pagination.PageSize).Take(pagination.PageSize).ToList(),
                Total = total
            };

            return Json(result);
        }
    }
}
