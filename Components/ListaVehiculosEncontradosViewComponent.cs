/*
 * Descripción:
 * Proyecto: RIAG
 * Fecha de creación: Friday, February 23rd 2024 9:19:44 pm
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Fri Feb 23 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */
using System.Collections.Generic;
using System.Threading.Tasks;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuanajuatoAdminUsuarios.Components
{
    public class ListaVehiculosEncontradosViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<VehiculoModel> listaVehiculos)
        {
            //var modelo = new VehiculoPropietarioBusquedaModel();
            return await Task.FromResult((IViewComponentResult)View("ListaVehiculos", listaVehiculos));
        }
    }
}