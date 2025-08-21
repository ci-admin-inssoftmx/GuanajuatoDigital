/*
 * Descripción:
 * Proyecto: RIAG
 * Fecha de creación: Saturday, March 2nd 2024 1:37:34 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Sat Mar 02 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */

using System.Collections.ObjectModel;
using GuanajuatoAdminUsuarios.Models;
namespace GuanajuatoAdminUsuarios.Common
{
    class Constants
    {
      public  static readonly ReadOnlyCollection<TipoLicencia> tipoLicencias = new(new[]
    {
    new TipoLicencia(){Id=1 , Descripcion= "TIPO A  CHOFER AUTOMOVILISTA" },
    new TipoLicencia(){Id=2 , Descripcion= "TIPO B CHOFER SERVICIO PÚBLICO" },
    new TipoLicencia(){Id=3 , Descripcion= "TIPO C  CHOFER SERVICIO DE CARGA" },
    new TipoLicencia(){Id=4 , Descripcion= "TIPO D -MOTOCICLISTA" },
    new TipoLicencia(){Id=6 , Descripcion= "PERMISO A -AUTOMOVIL-" },
    new TipoLicencia(){Id=7 , Descripcion= "PERMISO D -MOTOCICLETA-" },
    new TipoLicencia(){Id=8 , Descripcion= "NO APLICA" },
    new TipoLicencia(){Id=1 , Descripcion= "TIPO A" },
    new TipoLicencia(){Id=2 , Descripcion= "TIPO B" },
    new TipoLicencia(){Id=3 , Descripcion= "TIPO C" },
    new TipoLicencia(){Id=4 , Descripcion= "TIPO D" },
    new TipoLicencia(){Id=6 , Descripcion= "PERMISO A" },
    new TipoLicencia(){Id=7 , Descripcion= "PERMISO D" },
    new TipoLicencia(){Id=1 , Descripcion= "A-AUTOMOVILISTA" },
    new TipoLicencia(){Id=2 , Descripcion= "B-CHOFER DE SERVICIO PÚBLICO" },
    new TipoLicencia(){Id=3 , Descripcion= "C-CHOFER DE SERVICIO DE CARGA" },
    new TipoLicencia(){Id=4 , Descripcion= "D-MOTOCICLISTA" },
    new TipoLicencia(){Id=6 , Descripcion= "PA-PERMISO AUTOMOVILISTA" },
    new TipoLicencia(){Id=7 , Descripcion= "PD-PERMISO MOTOCICLISTA" },

});
    }
}