/*
 * Descripción:
 * Proyecto: Sistema de Infracciones y Accidentes
 * Fecha de creación: Tuesday, February 20th 2024 10:41:56 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Fri Feb 23 2024
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
    public class VehiculoPropietarioDatosViewComponent : ViewComponent
    {
        private readonly IVehiculosService _vehiculosService;
        public VehiculoPropietarioDatosViewComponent(IVehiculosService vehiculosService)
        {
            _vehiculosService = vehiculosService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int idVehiculo)  
       {
        var vehiculo = _vehiculosService.GetVehiculoById(idVehiculo);
            //var modelo = new VehiculoPropietarioBusquedaModel();
            vehiculo.cargaTexto = (vehiculo.carga == true) ? "Si" : "No";
            //return PartialView("_DetalleVehiculo", model);
            return await Task.FromResult((IViewComponentResult) View("_DetalleVehiculo", vehiculo));

            //return await Task.FromResult((IViewComponentResult)View("VehiculoPropietarioDatos", vehiculo));

        }
    }
}