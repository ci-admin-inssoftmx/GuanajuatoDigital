using GuanajuatoAdminUsuarios.Data.Entities2;
using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Interfaces.Blocs;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Models.Blocs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;


namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class BlocsController : BaseController
    {
        private readonly IBlocsService _blocsService;
        private readonly IConfiguration _configuration;
        private readonly string _rutaArchivo;

        public BlocsController(IBlocsService blocsService, IConfiguration configuration)
        {
            this._blocsService = blocsService;
            this._configuration = configuration;
            _rutaArchivo = configuration.GetValue<string>("AppSettings:RutaArchivosBlocs");
        }



        #region views

        // Alta Blocs
        public IActionResult AltaBlocs()
        {
            return View();
        }

        //Consulta Block
        public IActionResult Index()
        {

            var ListBlocsmodel = _blocsService.getLisBlocsConsulta();
            return View(ListBlocsmodel);
        }

        public ActionResult CancelParcial(long blocId, string numeroSerie)
        {
            ViewBag.NumeroSerie = numeroSerie;
            ViewBag.BlocId = blocId;

            return View("_CancelarBloc");
        }
        #endregion

        #region partialViews

        //Alta Blocs
        public ActionResult ModalEliminarBloc(string blocId, string blocSerie)
        {
            ViewBag.BlocId = blocId;
            ViewBag.BlocSerie = blocSerie;
            return PartialView("_ModalEliminarBloc");
        }

        //ConsultaBlocks
        public ActionResult AsignarOficialParcial(string numeroSerie)
        {
            var blockModel = _blocsService.GetBlocBySerie(numeroSerie);
            return View("_AsignarOficial", BlocAsignacionOficialModel.CreateFrom(blockModel));

        }


        //InventarioBlocks
        public IActionResult Inventario()
        {
            return View();
        }


        #endregion

        #region Ajax
        //AltaBlocks
        [HttpPost]
        public IActionResult GuardaBloc(AltaBlocksWviewModel data)
        {
            string Usuario = User.FindFirst(CustomClaims.Nombre).Value;
            string usrId = User.FindFirst(CustomClaims.IdUsuario).Value;
            string corp = User.FindFirst(CustomClaims.TipoOficina).Value;
            var mensaje = "";

            var nocan = _blocsService.ValidarFolios(data.folioInicio.toInt(), data.folioFin.toInt(),corp.toInt(),data.idflujo.toInt());

            if (nocan)
            {
                mensaje = "Err";
                return Json(new
                {
                    Serie = "",
                    TipoBloc = "",
                    menaje = mensaje
                });
            }

            string Serie = _blocsService.CalculaSerie(data.idflujo.toInt());
            RegistraBloc registraBloc = new RegistraBloc()
            {
                Delegacion = 1,
                Municipio = 1,
                transito = corp.toInt(),
                TipoBlocId = data.idflujo.toInt(),
                Serie = Serie,
                ResponsableCarga = Usuario,
                FechaCarga = DateTime.Now,
                AsignadorBloc = Usuario,
                idUsrAlta = usrId.toInt(),
                FolioInicial = data.folioInicio.toInt(),
                FolioFinal = data.folioFin.toInt(),
                TotalBoletas = data.folioFin.toInt() - data.folioInicio.toInt() + 1,
                FechaActualizacion = DateTime.Now,
                OficialAsignado = "",
                idEstatusBlock = 1
            };
            string Resul = _blocsService.NuevoBloc(registraBloc);

            BlocsSerieDto blocsSerieDto = new BlocsSerieDto()
            {
                Serie = Serie,
                TipoBloc = data.idflujo,
                menaje = mensaje
            };
            return Json(blocsSerieDto);
        }
        public JsonResult getLisGridAlta([DataSourceRequest] DataSourceRequest request)
        {
            var lisBlocsModel = _blocsService.getLisBlocsAlta();

            return Json(lisBlocsModel.ToDataSourceResult(request));
        }
        [HttpPost]
        public IActionResult ElimBloc(int NumBloc)
        {
            string Resul = _blocsService.ElimBloc(NumBloc);
            if (Resul != "Ok")
            {
                return BadRequest(Resul);
            }
            return PartialView("_BlocsListaAlta");
        }

        //ConsultaBlocks
        public JsonResult getLisGridConsulta([DataSourceRequest] DataSourceRequest request)
        {
            var lisBlocsModel = _blocsService.getLisBlocsConsulta();

            return Json(lisBlocsModel.ToDataSourceResult(request));
        }
        [HttpPost]
        public async Task<IActionResult> AsignarOficial([FromForm] BlocAsignarOficialRequest request)
        {


            string nombreUsuario = User.FindFirst(CustomClaims.Nombre).Value;
            int idUsuario = Convert.ToInt32(User.FindFirst(CustomClaims.IdUsuario).Value);

            string nombreArchivo = "";
            if (request.File != null && request.File.Length > 0)
            {
                // se crea el nombre del archivo de la garantia
                string nombreArchivoTemplate = "{registraId}_{timestamp}{extension}";
                nombreArchivo = Path.Combine(_rutaArchivo, nombreArchivoTemplate
                    .Replace("{registraId}", request.RegistraId.ToString().Trim())
                    .Replace("{timestamp}", DateTime.Now.ToString("ddMMyyyyHHmmss"))
                    .Replace("{extension}", Path.GetExtension(request.File.FileName)));
                try
                {
                    // se escribe el archivo en disco
                    using Stream fileStream = new FileStream(nombreArchivo, FileMode.Create);
                    await request.File.CopyToAsync(fileStream);
                }
                catch (Exception e)
                {
                    return Problem(detail: e.Message, title: "Error al cargar el archivo", statusCode: StatusCodes.Status500InternalServerError);
                }
            }

            string result = _blocsService.AsignarOficialBloc(
                regId: request.RegistraId,
                idOficial: request.CatOficial,
                urlEvidencia: nombreArchivo,
                folioInicial: request.FolioInicial,
                folioFinal: request.FolioFinal,
                idUsuario: idUsuario,
                nombreUsuario: nombreUsuario,
                iddelegacion:request.iddelegacion,
                delegacion: request.delegacion);

            if (result != "Ok")
            {
                return BadRequest(result);
            }

            BlocsResulDto blocsResul = new BlocsResulDto { Resul = result };
            return Json(blocsResul);
        }
        [HttpPost]
        public async Task<IActionResult> CancelBlocModal(int registraId, string comment)
        {
            int idUser = Convert.ToInt32(User.FindFirst(CustomClaims.IdUsuario).Value);
            string Resul = await _blocsService.CancelarlBloc(registraId, comment, idUser);

            if (Resul != "Ok")
            {
                return BadRequest(Resul);
            }

            BlocsResulDto blocsResul = new BlocsResulDto()
            {
                Resul = Resul
            };
            return Json(blocsResul);
        }
        //InventarioBlocks
        [HttpPost]
        public async Task<IActionResult> CancelarFolioDetalle(int id)
        {
            int idUser = Convert.ToInt32(User.FindFirst(CustomClaims.IdUsuario).Value);
            string Resul = await _blocsService.CanselarFolio(id, idUser);

            if (Resul != "Ok")
            {
                return BadRequest(Resul);
            }

            BlocsResulDto blocsResul = new BlocsResulDto()
            {
                Resul = Resul
            };
            return Json(blocsResul);
        }


        public async Task<IActionResult> GetImg(int idBlock)
        {
            string b64, mimetype;

            (b64, mimetype) = _blocsService.GetBase64(idBlock);

            return Json(new { b64=b64 , mime=mimetype , id=idBlock });
        }


        #endregion








        public List<BlocListItemModel> getLisBlocsConsulta()
        {
            return _blocsService.getLisBlocsConsulta();
        }
             
        public List<BlocsAltaModel> getLisBlocsAlta()
        {
            return _blocsService.getLisBlocsAlta();
        }

        [HttpGet]
        public IActionResult Delegacion()
        {
            BlocDelegacionModel datDeleg = new BlocDelegacionModel()
            {
                nombreDelegacion = User.FindFirst(CustomClaims.Oficina).Value,
                Usuario = User.FindFirst(CustomClaims.Nombre).Value
            };
            return Json(datDeleg);
        }




        public JsonResult GetCatalogoBlocs()
        {
            var catBlocs = _blocsService.getCatBlocs();
            return Json(catBlocs);
        }

        public JsonResult GetCatalogoEstado()
        {
            var catBlocs = _blocsService.getCatEstado();
            return Json(catBlocs);
        }







        public JsonResult getCatOficial()
        {
            var catOficial = _blocsService.getCatOficial();
            return Json(catOficial);
        }       

      

        
        public IActionResult EditarBloc(string NSerie)
        {
            var blocs = _blocsService.BlocBySerie(NSerie);
            return View(blocs);
        }



        public JsonResult getLisGridInventario([DataSourceRequest] DataSourceRequest request)
        {
            List<BlocsInventarioModel> lisBlocsModel = new List<BlocsInventarioModel>();
            try
            {
                lisBlocsModel = _blocsService.getLisBlocsInventario();
                return Json(lisBlocsModel.ToDataSourceResult(request));
            }
            catch (Exception ex)
            {
                return Json(lisBlocsModel);
            }
            
        }

        public List<BlocsInventarioModel> getLisBlocsInventario()
        {
            return _blocsService.getLisBlocsInventario();
        }

      






    }
}

