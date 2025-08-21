/*
 * Descripción:
 * Proyecto: RIAG
 * Fecha de creación: Wednesday, March 6th 2024 2:59:52 pm
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Thu Mar 07 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */

namespace GuanajuatoAdminUsuarios.Models.Components
{
    public class BusquedaPersonaFisicaConfig
    {
        public bool IsModal { get; set; }
        public string TextoBotonAgregarPersona { get; set; } = "Agregar nueva persona";
        public string NombreVariableToChange { get; set; } = "ddlIdPersona";
        //Nombre del método en javascript que debe ejecutarse una vez que se seleccione una persona de RIAG
         public string NombreMetodoToTrigger { get; set; } = "onPersonaSeleccionadaTrigger";
    }
}