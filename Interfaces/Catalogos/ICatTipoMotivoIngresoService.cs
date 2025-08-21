/*
 * Descripción:
 * Proyecto: Interfaces
 * Fecha de creación: Sunday, February 18th 2024 7:28:28 pm
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Sun Feb 18 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */

using System.Collections.Generic;
using GuanajuatoAdminUsuarios.Models;
namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatTipoMotivoIngresoService
    {
        List<CatTipoMotivoIngresoModel> ObtenerTiposMotivoIngresoActivos();

    }
}