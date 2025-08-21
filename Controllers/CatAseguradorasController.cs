using DocumentFormat.OpenXml.Spreadsheet;
using GuanajuatoAdminUsuarios;
using GuanajuatoAdminUsuarios.Controllers;
using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Exceptions;
using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Models.Catalogos.Aseguradoras;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class CatAseguradorasController : BaseController
{
    private readonly ICatAseguradorasService _catAseguradorasService;

    public async Task<IActionResult> Index()
    {
        var model = new AseguradoraIndexViewModel
        {
            Filter = new AseguradoraFilterModel(),
            Lista = await GetAll(),
        };
        return View(model);
    }

    public CatAseguradorasController(ICatAseguradorasService catAseguradoraService)
    {
        _catAseguradorasService = catAseguradoraService;
    }

    [HttpGet("[controller]/ObtenerTodos")]
    public async Task<List<AseguradoraDetailsModel>> GetAll()
    {
        IEnumerable<CatAseguradoras> aseguradoras = (IEnumerable<CatAseguradoras>)await _catAseguradorasService.GetAllAsync();
        var result = aseguradoras.Select(AseguradoraDetailsModel.FromEntity);
        return result.ToList();
    }

    [HttpGet("[controller]/ObtenerTodosList")]
    public async Task<ActionResult<List<CatalogModel>>> GetAllList()
    {
        var aseguradoras = await _catAseguradorasService.GetAllAsync();
        return Ok(aseguradoras);
    }

    [HttpGet("[controller]/ObtenerPorId/{id}")]
    public async Task<ActionResult<AseguradoraModel>> GetById(int id)
    {
        var aseguradora = await _catAseguradorasService.GetByIdAsync(id);
        if (aseguradora == null) return NotFound();
        return Ok(aseguradora);
    }

    [HttpPost("[controller]/Guardar")]
    public async Task<ActionResult> Add(AseguradoraModel aseguradoraDto)
    {
        await _catAseguradorasService.AddAsync(aseguradoraDto);
        return CreatedAtAction(nameof(GetById), new { id = aseguradoraDto.NombreAseguradora }, aseguradoraDto);
    }

    [HttpPut("[controller]/Editar")]
    public async Task<ActionResult> Update(AseguradoraModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return PartialView("_Editar", model);
            }
            
            await _catAseguradorasService.UpdateAsync(model.ToEntity(), (int)Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value));
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
        var existingAseguradora = await _catAseguradorasService.GetByIdAsync((int)id);
        if (existingAseguradora == null) return NotFound();

        await _catAseguradorasService.DeleteAsync((int)id);
        return NoContent();
    }

    [HttpPost]
    public async Task<JsonResult> Search([DataSourceRequest] DataSourceRequest searchParameters, AseguradoraFilterModel filterParameters)
    {
        var aseguradoras = await Search(filterParameters);
        return Json(aseguradoras.ToDataSourceResult(searchParameters));
    }

    private async Task<List<AseguradoraDetailsModel>> Search(AseguradoraFilterModel filterParameters)
    {

        IEnumerable<CatAseguradoras> entities = await _catAseguradorasService.SearchAsync(filterParameters);
        var result = entities.Select(AseguradoraDetailsModel.FromEntity);

        return result.ToList();
    }
    public IActionResult LoadCrearPartial()
    {
        return PartialView("_Crear");
    }

    [HttpGet]
    [Route("[controller]/[action]/{id}")]
    public async Task<IActionResult> LoadEditarPartial([FromRoute] int id)
    {
        var aseguradora = await _catAseguradorasService.GetByIdAsync(id);
        if (aseguradora == null)
        {
            return NotFound();
        }

        var model = new AseguradoraModel
        {
            IdAseguradora = aseguradora.IdAseguradora,
            NombreAseguradora = aseguradora.NombreAseguradora,
            Estatus = aseguradora.Estatus,
        };

        return PartialView("_Editar", model);
    }

    [HttpPost]
    public IActionResult LoadEliminarPartial(AseguradoraDetailsModel model)
    {
        return PartialView("_Eliminar", model);
    }
}
