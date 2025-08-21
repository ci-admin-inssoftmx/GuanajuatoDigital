/*
 * Descripción:
 * Proyecto: Models
 * Fecha de creación: Sunday, February 18th 2024 10:44:54 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Fri Feb 23 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */


using System;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class SolicitudDepositoOtraDependenciaModel : VehiculoPropietarioBusquedaModel
    {

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FechaSolicitud { get; set; }
        public TimeSpan HoraSolicitud { get; set; }

        public int IdDependenciaEnvia { get; set; }
        public int IdTipoMotivoIngreso { get; set; }
         public int IdMunicipoEnvia { get; set; }

        public int IdMunicipioUbicacion { get; set; }
        public int IdCarretera { get; set; }
        public int IdTramo { get; set; }
        public string KilometroUbicacion { get; set; }

        public string ColoniaUbicacion { get; set; }
        public string CalleUbicacion { get; set; }
        public string NumeroUbicacion { get; set; }
        public string InterseccionUbicacion { get; set; }

    }
}