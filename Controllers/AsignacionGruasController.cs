using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.Util;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GuanajuatoAdminUsuarios.Controllers
{

    [Authorize]
    public class AsignacionGruasController : BaseController
    {
        private readonly IAsignacionGruasService _asignacionGruasService;
        private readonly IGruasService _gruasService;
        private readonly IVehiculosService _vehiculosService;
        private readonly IVehiculoPlataformaService _vehiculoPlataformaService;
        private readonly IBitacoraService _bitacoraServices;
        private readonly IDepositosService _depositosService;
        private readonly IInfraccionesService _infraccionesService;
        private readonly string _rutaArchivo;
        private bool editar = true;

        public AsignacionGruasController(IAsignacionGruasService asignacionGruasService, IGruasService gruasService, IBitacoraService bitacoraServices, IConfiguration configuration,
            IVehiculosService vehiculosService, IVehiculoPlataformaService vehiculoPlataformaService, IDepositosService depositosService, IInfraccionesService infraccionesService)

        {
            _asignacionGruasService = asignacionGruasService;
            _gruasService = gruasService;
            _bitacoraServices = bitacoraServices;
            _rutaArchivo = configuration.GetValue<string>("AppSettings:RutaArchivoInventarioDeposito");
            _vehiculosService = vehiculosService;
            _vehiculoPlataformaService = vehiculoPlataformaService;
            _depositosService = depositosService;
            _infraccionesService = infraccionesService;
        }
        public IActionResult Index(string folio)
        {

            GruasTodas_Drop();
            var q = User.FindFirst(CustomClaims.Nombre).Value;
            ViewBag.FolioSolicitud = folio ?? "";
            return View();
        }

        public IActionResult ajax_BuscarSolicitudes(AsignacionGruaModel model)
        {
            var q = model.dateBusqueda;
            if (!string.IsNullOrEmpty(q))
            {
                model.fecha = DateTime.ParseExact(model.dateBusqueda, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            int idOficina = Convert.ToInt32(User.FindFirst(CustomClaims.OficinaDelegacion).Value);
            int idDependencia = HttpContext.Session.GetInt32("IdDependencia") ?? 0;
            var resultadoSolicitudes = _asignacionGruasService.BuscarSolicitudes(model, idOficina, idDependencia);

            return Json(resultadoSolicitudes);
        }

        public IActionResult DatosGruas(int iSo, string folio, int iPg, int idDeposito,bool? Edit)
        {
            string folioOId = iSo > 0 ? iSo.ToString() : folio;
            HttpContext.Session.SetString("iSo", folioOId);
            HttpContext.Session.SetInt32("iPg", iPg);

            HttpContext.Session.SetInt32("EditDeposito", Edit.HasValue? 1:0);


           int idOficina = HttpContext.Session.GetInt32("IdOficina") ?? 0;
            int idDependencia = HttpContext.Session.GetInt32("IdDependencia") ?? 0;

            var solicitud = _asignacionGruasService.BuscarSolicitudPord(iSo, folio, idOficina, idDependencia);
            HttpContext.Session.SetInt32("idDeposito", solicitud.IdDeposito == 0 ? idDeposito : solicitud.IdDeposito);

            var datosInfraccion = _asignacionGruasService.DatosInfraccionAsociada(solicitud.FolioSolicitud, idDependencia);

            if (idDeposito > 0)
            {
                _bitacoraServices.BitacoraDepositos(idDeposito, "Gruas", CodigosDepositos.C3028, solicitud);
            }

            // Verifica si idInfraccion es nulo
            if (datosInfraccion.idInfraccion == 0)
            {
                datosInfraccion = _asignacionGruasService.DatosVehiculoAsociado(solicitud.FolioSolicitud, idDependencia);

            }
            ViewBag.folio = folio;
            datosInfraccion.inventarios = solicitud.inventarios;
            datosInfraccion.Nombreinventarios = solicitud.Nombreinventarios;
            return View("capturaGruas", datosInfraccion);
        }

        public IActionResult GruasAsignadasTabla([DataSourceRequest] DataSourceRequest request)
        {
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;
            var DatosTabla = _asignacionGruasService.BusquedaGruaTabla(iDep);
            return Json(DatosTabla.ToDataSourceResult(request));
        }

        public IActionResult ModalInfracciones()
        {
            return PartialView("_ModalInf");
        }

        public IActionResult ModalVehiculos()
        {
            return PartialView("_ModalVeh");
        }
        public IActionResult ModalAgregarGrua()
        {
            this.editar = false;
            return PartialView("_ModalAgregarGrua");
        }

        public async Task<ActionResult> BuscarVehiculo(VehiculoBusquedaModel model)
        {
            try
            {
                var SeleccionVehiculo = _vehiculosService.BuscarPorParametro(model.PlacasBusqueda ?? "", model.SerieBusqueda ?? "", model.FolioBusqueda);

                if (System.String.IsNullOrEmpty(model.PlacasBusqueda) && System.String.IsNullOrEmpty(model.SerieBusqueda))
                    ViewBag.SinBusquedaAG = "TRUE";
                else
                    ViewBag.SinBusquedaAG = "FALSE";

                if (SeleccionVehiculo > 0)
                {
                    var text = "";
                    var value = "";

                    if (!string.IsNullOrEmpty(model.SerieBusqueda))
                    {
                        text = "serie";
                        value = model.SerieBusqueda;

                    }
                    else if (!string.IsNullOrEmpty(model.PlacasBusqueda))
                    {
                        text = "placas";
                        value = model.PlacasBusqueda.ToUpper();
                    }



                    return Json(new { noResults = false, data = value, field = text });
                }
                else
                {
                    var jsonPartialVehiculosByWebServices = await ajax_BuscarVehiculo(model);

                    if (jsonPartialVehiculosByWebServices != null)
                    {
                        return Json(new { noResults = true, data = jsonPartialVehiculosByWebServices });
                    }
                    else
                    {
                        return Json(new { noResults = true, data = "" });
                    }
                }
            }
            catch (Exception)
            {
                return Json(new { noResults = true, error = "Se produjo un error al procesar la solicitud", data = "" });
            }
        }


        public IActionResult DescargarItem(string IdInfraccion)
        {

            var path2 = _depositosService.GetPathFile(IdInfraccion);


            var path = new CatalogModel();
            if (!string.IsNullOrEmpty(path2.aux))
            {
                path = _infraccionesService.GetPathFile(path2.aux);
            }

            _ = new byte[] { };
            byte[] data;
            if (System.IO.File.Exists(path2.value) && !string.IsNullOrEmpty(path2.text))
            {
                data = System.IO.File.ReadAllBytes(path2.value);
                MemoryStream stream = new MemoryStream(data);
                var contentType = "application/octet-stream";
                var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                if (provider.TryGetContentType(path2.value, out var mimeType))
                {
                    contentType = mimeType;
                }
                string b64 = Convert.ToBase64String(stream.ToArray());
                return Json(new { file = b64, app = contentType, name = path2.text });

            }
            else if (!string.IsNullOrEmpty(path.value) && System.IO.File.Exists(path.value) && !string.IsNullOrEmpty(path.text))
            {
                data = System.IO.File.ReadAllBytes(path.value);
                MemoryStream stream = new MemoryStream(data);

                var contentType = "application/octet-stream";
                var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                if (provider.TryGetContentType(path.value, out var mimeType))
                {
                    contentType = mimeType;
                }



                string b64 = Convert.ToBase64String(stream.ToArray());
                return Json(new { file = b64, app = contentType, name = path.text });
            }
            else
            {
                return Json(new { file = "" });
            }





        }


        [HttpPost]
        public async Task<string> ajax_BuscarVehiculo(VehiculoBusquedaModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.PlacasBusqueda))
                {
                    model.PlacasBusqueda = model.PlacasBusqueda.ToUpper();
                }
                if (!string.IsNullOrEmpty(model.SerieBusqueda))
                {
                    model.SerieBusqueda = model.SerieBusqueda.ToUpper();
                }

				var corp = HttpContext.Session.GetInt32("IdDependencia").Value;
				var models = await _vehiculoPlataformaService.BuscarVehiculoEnPlataformas(model,corp);
                HttpContext.Session.SetInt32("IdMarcaVehiculo", models.idMarcaVehiculo);


                var test = await this.RenderViewAsync2("", models);


                return test;
            }
            catch (Exception ex)
            {
                Logger.Error("Infracciones - ajax_BuscarVehiculo: " + ex.Message);
                return null;
            }
        }


        [HttpPost]
        public ActionResult ActualizarDatosVehiculo([FromBody] AsignacionGruaModel selectedRowData)

        {

            try
            {
                int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;

                _asignacionGruasService.ActualizarDatos(selectedRowData, iDep);

                selectedRowData.folioInfraccion = string.IsNullOrEmpty(selectedRowData.folioInfraccion) ? "-" : selectedRowData.folioInfraccion;
                selectedRowData.Placa = string.IsNullOrEmpty(selectedRowData.Placa) ? "-" : selectedRowData.Placa;
                selectedRowData.Serie = string.IsNullOrEmpty(selectedRowData.Serie) ? "-" : selectedRowData.Serie;
                selectedRowData.Tarjeta = string.IsNullOrEmpty(selectedRowData.Tarjeta) ? "-" : selectedRowData.Tarjeta;
                selectedRowData.Marca = string.IsNullOrEmpty(selectedRowData.Marca) ? "-" : selectedRowData.Marca;
                selectedRowData.Submarca = string.IsNullOrEmpty(selectedRowData.Submarca) ? "-" : selectedRowData.Submarca;
                selectedRowData.Modelo = string.IsNullOrEmpty(selectedRowData.Modelo) ? "-" : selectedRowData.Modelo;
                selectedRowData.Propietario = string.IsNullOrEmpty(selectedRowData.Propietario) ? "-" : selectedRowData.Propietario;
                selectedRowData.CURP = string.IsNullOrEmpty(selectedRowData.CURP) ? "-" : selectedRowData.CURP;
                selectedRowData.RFC = string.IsNullOrEmpty(selectedRowData.RFC) ? "-" : selectedRowData.RFC;


                //BITACORA
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                //_bitacoraServices.insertBitacora(iDep, ip, "AsignacionGruas_DatosVehiculo", "Actualizar", "Update", user);
                object data = new { idVehiculo = selectedRowData.IdVehiculo };
                _bitacoraServices.BitacoraGenerales(CodigosGeneral.C5011, selectedRowData);


                return Ok(selectedRowData);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error al actualizar los datos");
            }
        }
        [HttpPost]

        public ActionResult BuscarInfracciones(string folioInfraccion)
        {
            var SeleccionVehiculo = _asignacionGruasService.ObtenerInfracciones(folioInfraccion);
            return Json(SeleccionVehiculo);
        }
        public JsonResult Gruas_Drop()
        {
            int iPg = HttpContext.Session.GetInt32("iPg") ?? 0;

            var result = new SelectList(_gruasService.GetGruasByIdConcesionario(iPg), "idGrua", "noEconomico");
            return Json(result);
        }


        public JsonResult GruasTodas_Drop()
        {
            int iPg = HttpContext.Session.GetInt32("iPg") ?? 0;

            var result = new SelectList(_gruasService.GetGruas(), "IdGrua", "noEconomico");
            return Json(result);
        }
        [HttpPost]

        public IActionResult ActualizarDatos(IFormCollection formData, int abanderamiento, int arrastre, int salvamento)
        {
            var DatosGruas = _asignacionGruasService.EditarDatosGrua(formData, abanderamiento, arrastre, salvamento);
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;
            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(iDep, ip, "AsignacionGruas_DatosGrua", "Actualizar", "Update", user);
            object data = new { catalogo = "Gruas Asignadas", idAsignacion = int.Parse(formData["idAsignacion"].ToString()), idDeposito = iDep };
            object data2 = new { catalogo = "Gruas Asignadas", idDeposito = iDep, obj = formData };

            // _bitacoraServices.BitacoraGenerales(CodigosGeneral.C5007, data2);

            _bitacoraServices.BitacoraDepositos(iDep, "Asignacion Gruas", CodigosDepositos.C3030, data);

            var DatosTabla = _asignacionGruasService.BusquedaGruaTabla(iDep);
            return Json(DatosTabla);
        }

        [HttpPost]

        public IActionResult ActualizarDatos2(IFormCollection formData, int abanderamiento, int arrastre, int salvamento, string horaInicioInsertEdit, string horaArriboInsertEdit, string horaTerminoInsertEdit)
        {
            int iSo = HttpContext.Session.GetInt32("iSo") ?? 0;
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;
            int DatosGruas = _asignacionGruasService.UpdateDatosGrua(formData, abanderamiento, arrastre, salvamento, iDep, iSo, horaInicioInsertEdit, horaArriboInsertEdit, horaTerminoInsertEdit);

            var DatosTabla = _asignacionGruasService.BusquedaGruaTabla(iDep);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(iDep, ip, "AsignacionGruas_DatosGrua", "Insertar", "insert", user);
            object data = new { catalogo = "Gruas Asignadas", idAsignacion = int.Parse(formData["idAsignacion"].ToString()), idDeposito = iDep };
            object data2 = new { catalogo = "Gruas Asignadas", idDeposito = iDep, obj = formData };

            _bitacoraServices.BitacoraGenerales(CodigosGeneral.C5007, data2);
            _bitacoraServices.BitacoraDepositos(iDep, "Asignacion Gruas", CodigosDepositos.C3031, formData);


            return Json(DatosTabla);
        }


        public IActionResult InsertarDatos(IFormCollection formData, int abanderamiento, int arrastre, int salvamento, string horaInicioInsert, string horaArriboInsert, string horaTerminoInsert)
        {
            int iSo = HttpContext.Session.GetInt32("iSo") ?? 0;
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;

            _asignacionGruasService.InsertDatosGrua(formData, abanderamiento, arrastre, salvamento, iDep, iSo, horaInicioInsert, horaArriboInsert, horaTerminoInsert);

            var DatosTabla = _asignacionGruasService.BusquedaGruaTabla(iDep);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(iDep, ip, "AsignacionGruas_DatosGrua", "Insertar", "insert", user);
            object data = new { catalogo = "Gruas Asignadas", idDeposito = iDep };
            object data2 = new { catalogo = "Gruas Asignadas", idDeposito = iDep, obj= formData };

            _bitacoraServices.BitacoraGenerales(CodigosGeneral.C5009, data2);
            _bitacoraServices.BitacoraDepositos(iDep, "Asignacion Gruas", CodigosDepositos.C3030, formData);

            return Json(DatosTabla);
        }

        public IActionResult AgregarObservaciones(AsignacionGruaModel formData)
        {
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;
            var DatosTabla = _asignacionGruasService.AgregarObs(formData, iDep);

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(iDep, ip, "AsignacionGruas_Observaciones", "Insertar", "insert", user);
            object data = new { catalogo = "Depositos agregar observaciones", idDeposito = iDep, observaciones = formData.observaciones };
            _bitacoraServices.BitacoraGenerales(CodigosGeneral.C5007, data);

            if(HttpContext.Session.GetInt32("EditDeposito")>0)
                _bitacoraServices.BitacoraDepositos(iDep, "Asignar Grua", CodigosDepositos.C3031, formData);
            else
                _bitacoraServices.BitacoraDepositos(iDep, "Asignar Grua", CodigosDepositos.C3030, formData);

            return Json(DatosTabla);
        }
        [HttpPost]
        public async Task<IActionResult> AgregarInventario(AsignacionGruaModel model)
        {
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;


            try
            {
                if (model.MyFile != null && model.MyFile.Length > 0)
                {

                    //Se crea el nombre del archivo del inventario
                    string nombreArchivo = _rutaArchivo + "/" + iDep + "_" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").Replace("/", "").Replace(":", "").Replace(" ", "") + System.IO.Path.GetExtension(model.MyFile.FileName);
                    //Se escribe el archivo en disco
                    using (Stream fileStream = new FileStream(nombreArchivo, FileMode.Create))
                    {
                        await model.MyFile.CopyToAsync(fileStream);
                    }
                    var nombre = model.MyFile.FileName;


                    int resultado = _asignacionGruasService.InsertarInventario(nombreArchivo, iDep, model.numeroInventario, nombre);
                    if (resultado == 0)
                        return Json(new { success = false, message = "Ocurrió un error al actualizar depósito" });

                    //BITACORA
                    var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                    var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                    //_bitacoraServices.insertBitacora(iDep, ip, "AsignacionGruas_Inventario", "Insertar", "insert", user);
                    object data = new { catalogo = "Depositos Insertar Inventario", idDeposito = iDep, nombreArchivo = nombreArchivo, numInventario = model.numeroInventario };
                    _bitacoraServices.BitacoraGenerales(CodigosGeneral.C5007, data);
                    return Json(new { success = true, message = "Imagen e información guardadas exitosamente" });
                }
                else if (!string.IsNullOrEmpty(model.NombreArchivo))
                {
                    int resultado = _asignacionGruasService.InsertarInventario("", iDep, model.numeroInventario, model.NombreArchivo);
                    return Json(new { success = true, message = "Imagen e información guardadas exitosamente" });
                }
                else
                {
                    return Json(new { success = false, message = "No hay cambios que actuaizar en el inventario" });

                }
            }
            catch (Exception ex)
            {
                Logger.Error("Ocurrió un error al cargar el archivo a depósito: " + ex);
                return Json(new { success = false, message = "Ocurrió un error al guardar el archivo" });
            }
        }

        public IActionResult ModalEditarGrua(int idAsignacion)
        {
            this.editar = true;
            var eliminarGrua = _asignacionGruasService.ObtenerAsignacionPorId(idAsignacion);

            return PartialView("_ModalEditarGrua", eliminarGrua);
        }

        public IActionResult ModalEliminarGrua(int idAsignacion)
        {
            var eliminarGrua = _asignacionGruasService.ObtenerAsignacionPorId(idAsignacion);

            return PartialView("_ModalEliminarGrua");
        }

        public IActionResult EliminarGrua(int idAsignacion)
        {
            var eliminarGrua2 = _asignacionGruasService.ObtenerAsignacionPorId(idAsignacion);
            var eliminarGrua = _asignacionGruasService.EliminarGrua(idAsignacion);
            int iDep = HttpContext.Session.GetInt32("idDeposito") ?? 0;

            //BITACORA
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
            //_bitacoraServices.insertBitacora(idAsignacion, ip, "AsignacionGruas_Grua", "Eliminar", "delete", user);
            object data = new { catalogo = "Gruas Asignadas eliminar", estatus = 0, idAsignacion = idAsignacion };
            _bitacoraServices.BitacoraGenerales(CodigosGeneral.C5007, data);
            _bitacoraServices.BitacoraDepositos(iDep, "Asignacion Gruas", CodigosDepositos.C3032, eliminarGrua2); 

            var DatosTabla = _asignacionGruasService.BusquedaGruaTabla(iDep);

            return Json(DatosTabla);
        }

    }
}
