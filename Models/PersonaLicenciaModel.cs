/*
 * Descripción:
 * Proyecto: RIAG
 * Fecha de creación: Wednesday, February 28th 2024 11:07:23 pm
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Sat Mar 02 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GuanajuatoAdminUsuarios.Models
{
    public class PersonaLicenciaModel
    {
        public decimal IdPersona { get; set; }

        public string NombreCompleto { get; set; }

        public string Nombre { get; set; }

        public string PrimerApellido { get; set; }

        public string SegundoApellido { get; set; }

        public string FechaNacimiento { get; set; }

        public string Curp { get; set; }

        public string Rfc { get; set; }

        public int IdGenero { get; set; }

        public string Genero { get; set; }

        public string NumeroLicencia { get; set; }

        public decimal IdLicencia { get; set; }

        public string EstadoNacimiento { get; set; }

        public string Nacionalidad { get; set; }

        public int IdTipoLicencia { get; set; }

        public string TipoLicencia { get; set; }

        public string FechaInicioVigencia { get; set; }

        public string FechaTerminoVigencia { get; set; }

        public int? IdMunicipio { get; set; }

        public string Municipio { get; set; }

        public string Cp { get; set; }

        public string Colonia { get; set; }

        public string Calle { get; set; }

        public string NumExt { get; set; }

        public string NumInt { get; set; }
        public string Telefono { get; set; }

        public string Email { get; set; }


        public void ConvertirModelo(LicenciaPersonaDatos p)
        {
            Nombre = p.NOMBRE;
            NombreCompleto = p.NOMBRE_COMPLETO;
            PrimerApellido = p.PRIMER_APELLIDO;
            SegundoApellido = p.SEGUNDO_APELLIDO;
            FechaNacimiento = (p.FECHA_NACIMIENTO.Value.Year < 1800) ? null : p.FECHA_NACIMIENTO.Value.ToString("yyyy-MM-dd");
            FechaTerminoVigencia = (p.FECHA_TERMINO_VIGENCIA.Value.Year < 1800) ? null : p.FECHA_TERMINO_VIGENCIA.Value.ToString("yyyy-MM-dd");
            Curp = p.CURP;
            Rfc = p.RFC;
            NumeroLicencia = p.NUM_LICENCIA;
            TipoLicencia = p.TIPOLICENCIA;
            Calle = p.CALLE;
            NumExt = p.NUM_EXT;
            Colonia = p.COLONIA;
            IdMunicipio = p.ID_MUNICIPIO;
            Municipio = p.MUNICIPIO;
            Cp = p.CP;
            EstadoNacimiento = p.ESTADO_NACIMIENTO;
            Telefono = p.TELEFONO1;
            Email = p.EMAIL;
            IdGenero = p.ID_GENERO == null ? 1 : Convert.ToInt16(p.ID_GENERO);

        }
    }
}