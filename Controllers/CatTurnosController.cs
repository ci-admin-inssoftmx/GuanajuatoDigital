using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Exceptions;
using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Interfaces.Catalogos;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Models.Catalogos.Turnos;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class CatTurnosController : BaseController
    {
        private readonly UserSession _userSession;
        private readonly ICatTurnosService _turnoService;
        private readonly ICatDelegacionesOficinasTransporteService _delegacionesOficinasService;

        public CatTurnosController(UserSession userSession, ICatTurnosService turnosService, ICatDelegacionesOficinasTransporteService delegacionesOficinasService)
        {
            _userSession = userSession ?? throw new System.ArgumentNullException(nameof(userSession));
            _turnoService = turnosService ?? throw new System.ArgumentNullException(nameof(turnosService));
            _delegacionesOficinasService = delegacionesOficinasService ?? throw new System.ArgumentNullException(nameof(delegacionesOficinasService));
        }

        public async Task<IActionResult> Index()
        {
            var model = new TurnosIndexViewModel
            {
                Filter = new TurnoFilterModel(),
                Lista = await GetAll(),
            };
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> Search([DataSourceRequest] DataSourceRequest searchParameters, TurnoFilterModel filterParameters)
        {
            var turnos = await Search(filterParameters);
            return Json(turnos.ToDataSourceResult(searchParameters));
        }

        [HttpPost]
        public async Task<IActionResult> Create(TurnoCreateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return PartialView("_Crear", model);
                }

                await _turnoService.CreateAsync(model.ToEntity());
                return PartialView("_Crear", model);
            }
            catch (TurnoException ex)
            {
                return BadRequest(new { errors = new[] { ex.Message } });
            }            
        }

        [HttpPut]
        public async Task<IActionResult> Edit(TurnoModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = 400;
                    return PartialView("_Editar", model);
                }

                await _turnoService.UpdateAsync(model.ToEntity());
                return PartialView("_Editar", model);
            }
            catch (TurnoException ex)
            {
                return BadRequest(new { errors = new[] { ex.Message } });
            }            
        }

        [HttpDelete]
        [Route("[controller]/[action]/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _turnoService.DeleteAsync(id);
            return NoContent();
        }

        public JsonResult GetDelegacionesOficinasDropDownList()
        {
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
            var turno = await _turnoService.GetByIdAsync(id);
            if (turno == null)
            {
                return NotFound();
            }

            var model = new TurnoModel
            {
                IdTurno = turno.IdTurno,
                Nombre = turno.NombreTurno,
                HoraInicio = turno.InicioTurno,
                HoraFin = turno.FinTurno,
                IdDelegacion = turno.IdDelegacion,
            };

            return PartialView("_Editar", model);
        }

        [HttpPost]
        public IActionResult LoadEliminarPartial(TurnoDetailsModel model)
        {
            return PartialView("_Eliminar", model);
        }


        #endregion Load partial views


        private async Task<List<TurnoDetailsModel>> GetAll()
        {
            int coporacion = _userSession.GetCorporacionId();
            IEnumerable<CatTurno> entities = await _turnoService.GetAllByDependenciaAsync(coporacion);
            var result = entities.Select(TurnoDetailsModel.FromEntity);

            return result.ToList();
        }

        public async Task<IActionResult> GetDropDown(int IdOficina)
        {
            int coporacion = _userSession.GetCorporacionId();
            IEnumerable<CatTurno> entities = await _turnoService.GetAllByDependenciaAsync(coporacion);
            var result = entities.Select(TurnoDetailsModel.FromEntity);

            var result2 = new SelectList(result, "IdTurno", "Nombre");

            return Json(result2);
        }


        private async Task<List<TurnoDetailsModel>> Search(TurnoFilterModel filterParameters)
        {
            int coporacion = _userSession.GetCorporacionId();

            IEnumerable<CatTurno> entities = await _turnoService.SearchAsync(coporacion, filterParameters);
            var result = entities.Select(TurnoDetailsModel.FromEntity);

            return result.ToList();
        }


    }
}
