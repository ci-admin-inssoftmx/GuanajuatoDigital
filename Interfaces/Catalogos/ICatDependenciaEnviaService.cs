/*
 * Descripción:
 * Proyecto: Interfaces
 * Fecha de creación: Sunday, February 18th 2024 11:10:20 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Sun Feb 18 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */

using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatDependenciaEnviaService
    {
        List<CatDependenciaEnviaModel> ObtenerDependenciasEnviaActivas();
    }
}