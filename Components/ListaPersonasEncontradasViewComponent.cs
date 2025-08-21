/*
 * Descripción:
 * Proyecto: RIAG
 * Fecha de creación: Tuesday, February 27th 2024 12:36:56 pm
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Thu Mar 07 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Components
{
    public class ListaPersonasEncontradasViewComponent : ViewComponent
    {
        private readonly IPersonasService _personasService;
        public ListaPersonasEncontradasViewComponent(IPersonasService personasService){
            _personasService = personasService;
        }
        /// <summary>
        /// Lista de personas encontradas en RIAG
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(BusquedaPersonaModel model)
        {
            List<PersonaModel> listaPersonas = model.ListadoPersonas;
            if (model.ListadoPersonas==null || model.ListadoPersonas.Count==0)
             listaPersonas = _personasService.BuscarPersonasWithPagination(model, model.Pagination);

             //Se asignan las opciones una vez seleccionada una persona
            ViewBag.VariableToChange = model.Config.NombreVariableToChange;
            ViewBag.NombreMetodoToTrigger = model.Config.NombreMetodoToTrigger;

            return await Task.FromResult((IViewComponentResult)View("ListaPersonasEncontradas", listaPersonas ));
        }
    }
}