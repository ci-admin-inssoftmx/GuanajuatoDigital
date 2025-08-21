using GuanajuatoAdminUsuarios.Exceptions;
using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Interfaces.Catalogos;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Models.Catalogos.Puestos;
using GuanajuatoAdminUsuarios.Models.Catalogos.Turnos;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class CatPuestosController : BaseController
    {
        private readonly UserSession _userSession;
        private readonly ICatPuestosService _puestosService;
        private readonly ICatDelegacionesOficinasTransporteService _delegacionesOficinasService;
        public CatPuestosController(UserSession userSession, ICatPuestosService puestosService, ICatDelegacionesOficinasTransporteService delegacionesOficinasService)
        {
            _userSession = userSession ?? throw new System.ArgumentNullException(nameof(userSession));
            _puestosService = puestosService ?? throw new System.ArgumentNullException(nameof(puestosService));
            _delegacionesOficinasService = delegacionesOficinasService ?? throw new System.ArgumentNullException(nameof(delegacionesOficinasService));
        }

        public async Task<IActionResult> Index()
        {
            var model = new PuestosIndexViewModel
            {
                Filter = new PuestoFilterModel(),
                Lista = await GetAll(),
            };
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> Search([DataSourceRequest] DataSourceRequest searchParameters, PuestoFilterModel filterParameters)
        {
            var puestos = await Search(filterParameters);
            var result = puestos.ToDataSourceResult(searchParameters);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PuestoModel model)
        {
            try
            {
                ModelState.Remove(nameof(PuestoModel.Id));
                ModelState.Remove(nameof(PuestoModel.IdDelegacion));
                model.IdDelegacion ??= _userSession.GetOficinaDelegacionId();
                
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return PartialView("_Crear", model);
                }

                PuestoModel created = await _puestosService.CreateAsync(model);
                return Json(created);
            }
            catch (PuestoException ex)
            {
                return BadRequest(new { errors = new[] { ex.Message } });
            }
        }


        [HttpPut]
        public async Task<IActionResult> Edit(PuestoModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return PartialView("_Editar", model);
                }

                PuestoModel updated = await _puestosService.UpdateAsync(model);
                return Json(updated);
            }
            catch (PuestoException ex)
            {
                return BadRequest(new { errors = new[] { ex.Message } });
            }
        }

        [HttpDelete]
        [Route("[controller]/[action]/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _puestosService.DeleteAsync(id);
                return NoContent();
            }
            catch (PuestoException ex)
            {
                return BadRequest(new { error = ex.Message });
            }            
        }

        public JsonResult GetDelegacionesOficinasDropDownList()
        {
            if (!_userSession.IsAdmin())
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new List<object>());
            }

            int corporacion = _userSession.GetCorporacionId();
            List<CatDelegacionesOficinasTransporteModel> delegaciones = _delegacionesOficinasService.GetDelegacionesOficinasByDependencia(corporacion);

            var dropDownList = delegaciones.Select(d => new
            {
                Value = d.IdOficinaTransporte,
                Text = d.NombreOficina,
            }).ToList();

            return Json(dropDownList);
        }


        #region Load partial views

        public IActionResult LoadCrearPartial()
        {
            return PartialView("_Crear");
        }

        [HttpGet]
        [Route("[controller]/[action]/{id}")]
        public async Task<IActionResult> LoadEditarPartial([FromRoute] int id)
        {
            PuestoModel puesto = await _puestosService.GetByIdAsync(id);
            if (puesto == null)
            {
                return NotFound();
            }

            return PartialView("_Editar", puesto);
        }

        [HttpPost]
        public IActionResult LoadEliminarPartial(PuestoModel model)
        {
            return PartialView("_Eliminar", model);
        }


        #endregion Load partial views


        private async Task<List<PuestoModel>> GetAll()
        {
            List<PuestoModel> result = await _puestosService.GetAllAsync();
            return result;
        }

        private async Task<List<PuestoModel>> Search(PuestoFilterModel filterParameters)
        {
            List<PuestoModel> result = await _puestosService.SearchAsync(filterParameters);            
            return result;
        }
    }
}
