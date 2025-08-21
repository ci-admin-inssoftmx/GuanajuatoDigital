using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Utils;
using GuanajuatoAdminUsuarios.Models.Files;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class LiberacionVehiculoController : BaseController
    {
        #region DPIServices

        private readonly IPlacaServices _placaServices;
        private readonly IMarcasVehiculos _marcaServices;
        private readonly ILiberacionVehiculoService _liberacionVehiculoService;
        private readonly IRepuveService _repuveService;
        private readonly IBitacoraService _bitacoraServices;
        private readonly string _rutaArchivo;
        private readonly IPdfGenerator _pdfService;
        private static readonly Dictionary<string, List<byte[]>> FileChunks = new Dictionary<string, List<byte[]>>();

        public LiberacionVehiculoController(IPlacaServices placaServices,
            IMarcasVehiculos marcaServices, ILiberacionVehiculoService liberacionVehiculoService, IRepuveService repuveService, IConfiguration configuration, IBitacoraService bitacoraService, IPdfGenerator pdfService)
        {
            _placaServices = placaServices;
            _marcaServices = marcaServices;
            _liberacionVehiculoService = liberacionVehiculoService;
            _repuveService = repuveService;
            _bitacoraServices = bitacoraService;
            _rutaArchivo = configuration.GetValue<string>("AppSettings:RutaArchivosLiberacionVehiculo");
            _pdfService = pdfService;
        }


        #endregion

        public IActionResult Index()
        {
            int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

            LiberacionVehiculoBusquedaModel searchModel = new LiberacionVehiculoBusquedaModel();
            // List<LiberacionVehiculoModel> ListDepositos = _liberacionVehiculoService.GetAllTopDepositos(idOficina);
            //searchModel.ListDepositosLiberacion = ListDepositos;
            return View(searchModel);
        }



        public JsonResult Placas_Read()
        {
            int idOficina = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value); //HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var result = new SelectList(_placaServices.GetPlacasLiberacion(idOficina), "IdDepositos", "Placa");
            return Json(result);
        }

        public JsonResult Marcas_Read()
        {
            var result = new SelectList(_marcaServices.GetMarcas(), "IdMarcaVehiculo", "MarcaVehiculo");
            return Json(result);
        }

        [HttpPost]
        public ActionResult ajax_BuscarVehiculo(LiberacionVehiculoBusquedaModel model)
        {
            /* int IdModulo = 230;
             string listaPermisosJson = HttpContext.Session.GetString("Autorizaciones");
             List<int> listaPermisos = JsonConvert.DeserializeObject<List<int>>(listaPermisosJson);
             if (listaPermisos != null && listaPermisos.Contains(IdModulo))
             {*/
            int idOficina = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value);
            var ListVehiculosModel = _liberacionVehiculoService.GetDepositos(model, idOficina);

            if (ListVehiculosModel.Count == 0)
            {
                ViewBag.NoResultsMessage = "No se encontraron resultados que cumplan con los criterios de búsqueda.";
            }

            return PartialView("_ListadoVehiculos", ListVehiculosModel);
            /* }
             else
             {
                 TempData["ErrorMessage"] = "Este usuario no tiene acceso a esta sección.";
                 return PartialView("ErrorPartial");
             }*/
        }



        [HttpGet]
        public async Task<ActionResult> ajax_UpdateLiberacion(int Id)
        {

            int idOficina = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value); //HttpContext.Session.GetInt32("IdOficina") ?? 0;
            var model = _liberacionVehiculoService.GetDepositoByID(Id, idOficina);
            /*  RepuveConsgralRequestModel repuveGralModel = new RepuveConsgralRequestModel()
              {
                  placa = model.Placa,
                  niv = model.Serie
              };*/

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(Id, ip, "LiberacionVehiculo_Liberacion", "Actualizar", "update", user);
            object data = new { catalogo = "Depositos liberacion" };
            _bitacoraServices.BitacoraGenerales(CodigosGeneral.C5006, data);
            /*  try
              {
                  var repuveConsRoboResponse = await _repuveService.ConsultaRobo(repuveGralModel);

                  if (repuveConsRoboResponse != null && repuveConsRoboResponse.Count > 0)
                  {
                      ViewBag.ReporteRobo = repuveConsRoboResponse.FirstOrDefault().EsRobado;
                  }
              }
              catch (Exception ex)
              {

                  ViewBag.ReporteRobo = false; 
              }*/
            //model.FechaIngreso.ToString("dd/MM/yyyy");
            return PartialView("_UpdateLiberacion", model);
        }


        //public ActionResult UpdateLiberacion(LiberacionVehiculoModel model, IFormFile ImageAcreditacionPropiedad, IFormFile ImageAcreditacionPersonalidad, IFormFile ImageReciboPago)
        /*  [HttpPost]
           public ActionResult UpdateLiberacion(IFormFile ImageAcreditacionPropiedad, IFormFile ImageAcreditacionPersonalidad, IFormFile ImageReciboPago, string data)
           {
               int result = 0;
               try
               {

                   var model = JsonConvert.DeserializeObject<LiberacionVehiculoModel>(data);
                   if (ImageAcreditacionPropiedad != null)
                   {
                       using (var ms1 = new MemoryStream())
                       {
                           ImageAcreditacionPropiedad.CopyTo(ms1);
                           model.AcreditacionPropiedad = ms1.ToArray();
                       }
                   }
                   if (ImageAcreditacionPersonalidad != null)
                   {
                       using (var ms2 = new MemoryStream())
                       {
                           ImageAcreditacionPersonalidad.CopyTo(ms2);
                           model.AcreditacionPersonalidad = ms2.ToArray();
                       }
                   }
                   if (ImageReciboPago != null)
                   {
                       using (var ms3 = new MemoryStream())
                       {
                           ImageReciboPago.CopyTo(ms3);
                           model.ReciboPago = ms3.ToArray();
                       }
                   }

                   ////Prueba de que allmacena bien la imagen  
                   ////var imgByte = model.AcreditacionPropiedad;
                   ////return new FileContentResult(imgByte, "image/jpeg");
                   result = _liberacionVehiculoService.UpdateDeposito(model);

               }
               catch (Exception ex)
               {

               }
               if (result > 0)
               {
                   int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;

                   List<LiberacionVehiculoModel> ListProuctModel = _liberacionVehiculoService.GetAllTopDepositos(idOficina);
                   return Json(ListProuctModel);
               }
               else
               {
                   //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                   return Json(null);
               }
           }*/
        [HttpPost]
        public async Task<IActionResult> UploadChunk(IFormFile fileChunk, string fieldName)
        {
            if (fileChunk == null || fileChunk.Length == 0)
            {
                return BadRequest("Invalid file chunk.");
            }

            try
            {
                if (!FileChunks.ContainsKey(fieldName))
                {
                    FileChunks[fieldName] = new List<byte[]>();
                }

                using (var memoryStream = new MemoryStream())
                {
                    await fileChunk.CopyToAsync(memoryStream);
                    FileChunks[fieldName].Add(memoryStream.ToArray());
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Error al procesar el chunk: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLiberacion(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<LiberacionVehiculoModel>(data);

                foreach (var field in FileChunks)
                {
                    var fileData = field.Value.SelectMany(chunk => chunk).ToArray();
                    var filePath = Path.Combine(_rutaArchivo, $"{model.IdDeposito}_{field.Key}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(field.Key)}");

                    await System.IO.File.WriteAllBytesAsync(filePath, fileData);

                    if (field.Key == "ImageAcreditacionPropiedad")
                    {
                        model.ImageAcreditacionPropiedadPath = filePath;
                        model.AcreditacionPropiedadStr = "ImageAcreditacionPropiedad" + model.IdDeposito.ToString();
                    }
                    else if (field.Key == "ImageAcreditacionPersonalidad")
                    {
                        model.ImageAcreditacionPersonalidadPath = filePath;
                        model.AcreditacionPersonalidadStr = "ImageAcreditacionPersonalidad" + model.IdDeposito.ToString();
                    }
                    else if (field.Key == "ImageReciboPago")
                    {
                        model.ImageReciboPagoPath = filePath;
                        model.ReciboPagoStr = "ImageReciboPago" + model.IdDeposito.ToString();
                    }
                }
                var pdfBytes = await GenerarPdfLiberacion(model, _rutaArchivo);

                string pdfBase64 = Convert.ToBase64String(pdfBytes);
                // Llamar al método del servicio para actualizar la liberación
                int result = _liberacionVehiculoService.UpdateDeposito(model);
                if (result == 0)
                {
                    return Json(new { success = false, message = "Ocurrió un error al actualizar la liberación del vehículo" });
                }
                object data2 = new { idDeposito = model.IdDeposito };
                _bitacoraServices.BitacoraDepositos(model.IdDeposito, "Liberacion Vehiculo", CodigosDepositos.C3004, data2);
                return Json(new
                {
                    success = true,
                    message = "Liberación del vehículo actualizada exitosamente",
                    pdfBase64 = pdfBase64,
                    fileName = $"LiberacionVehiculo_{model.IdDeposito}.pdf"
                });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Ocurrió un error al procesar la solicitud", details = e.Message });
            }
            finally
            {
                FileChunks.Clear();
            }
        }

        private async Task SaveFileAsync(int idDeposito, IFormFile file, string prefix)
        {
            //string nombreArchivo = _rutaArchivo + "/" + idDeposito"/" + prefix + "_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + System.IO.Path.GetExtension(file.FileName);

            using (Stream fileStream = new FileStream(prefix, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }

        private async Task<byte[]> GenerarPdfLiberacion(LiberacionVehiculoModel model, string rutaArchivo)
        {
            int idOficina = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value);
            var modelData = _liberacionVehiculoService.GetDepositoByID(model.IdDeposito, idOficina);

            var modelPdf = new PDFLiberacionVehiculoModel
            {
                Propietario = modelData.nombrePropietario,
                Autoriza = model.Autoriza,
                Observaciones = modelData.Observaciones ?? "Sin observaciones",
                Placas = modelData.Placa,
                Marca = modelData.marcaVehiculo,
                SubMarca = modelData.nombreSubmarca,
                Serie = modelData.Serie,
                Color = modelData.Color,
                FechaIngreso = modelData.FechaIngreso,
                AcreditacionPersonalidadUrl = model.ImageAcreditacionPersonalidadPath ?? string.Empty,
                AcreditacionPropiedadUrl = model.ImageAcreditacionPropiedadPath ?? string.Empty,
                ReciboInfraccionUrl = model.ImageReciboPagoPath ?? string.Empty,
                Pension = modelData.pension,
                Folio = model.Folio,
                Id = modelData.IdDeposito,
                LogoUrl = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "logo-gto.png"),
                Paginador1Url = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "paginador1.png"),
                Paginador2Url = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "paginador2.png"),
                Paginador3Url = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "paginador3.png")
            };

            string jsonView = await this.RenderViewAsync("_LiberacionVehiculoModel", modelPdf, true);

            byte[] pdfBytes = _pdfService.CreatePDFByHTML(jsonView, "", iTextSharp.text.PageSize.LETTER);

            model.FormatoManualPath = Path.Combine(rutaArchivo, $"Formato_Manual_{model.IdDeposito}_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            await System.IO.File.WriteAllBytesAsync(model.FormatoManualPath, pdfBytes);

            return pdfBytes;
        }


    }
}