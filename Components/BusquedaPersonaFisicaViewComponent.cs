/*
 * Descripción:
 * Proyecto: RIAG
 * Fecha de creación: Tuesday, February 27th 2024 9:57:35 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Wed Mar 06 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Models.Components;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Components
{
    public class BusquedaPersonaFisicaViewComponent : ViewComponent
    {
        /// <summary>
        /// Componente de busqueda de persona modal
        /// </summary>
        /// <param name="isModal"></param>
        /// <param name="persona"></param>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(BusquedaPersonaFisicaConfig config, PersonaModel persona)
        {
            List<PersonaModel> otrasPersonas = new();
            persona ??= new PersonaModel();
            if (persona != null && persona.idCatTipoPersona == 1)
                otrasPersonas.Add(persona);
            persona.PersonaDireccion ??= new PersonaDireccionModel();
            var modelo = new BusquedaPersonaModel
            {
                ListadoPersonasOtras = otrasPersonas,
                Config = config ?? new BusquedaPersonaFisicaConfig()
            };
            return await Task.FromResult((IViewComponentResult)View("BusquedaPersonaFisica", modelo));
        }
    }
}