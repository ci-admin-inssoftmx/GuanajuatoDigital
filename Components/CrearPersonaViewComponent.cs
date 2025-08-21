/*
 * Descripción:
 * Proyecto: RIAG
 * Fecha de creación: Thursday, February 29th 2024 2:22:40 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Thu Mar 07 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */


using System.Threading.Tasks;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Components
{
    public class CrearPersonaViewComponent : ViewComponent
    {
        private readonly IPersonasService _personasService;
        public CrearPersonaViewComponent(IPersonasService personasService)
        {
            _personasService = personasService;
        }
        public async Task<IViewComponentResult> InvokeAsync(BusquedaPersonaModel model)
        {
            model.PersonaModel ??= new PersonaModel();

            if ( model.PersonaModel.idPersona==null ||  model.PersonaModel.idPersona == 0)
            {
                 model.PersonaModel.PersonaDireccion ??= new PersonaDireccionModel();
            }
            else
            {
                 model.PersonaModel = _personasService.GetPersonaById((int) model.PersonaModel.idPersona);
                 model.PersonaModel.generoBool =  model.PersonaModel.idGenero == 1;
            }

            return await Task.FromResult((IViewComponentResult)View("CrearPersonaFisica", model));
        }
    }
}