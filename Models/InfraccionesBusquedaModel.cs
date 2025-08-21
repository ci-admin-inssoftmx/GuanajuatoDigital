using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class InfraccionesBusquedaModel
    {
 
        public string folioInfraccion { get; set; } = null!;

        /// <summary>
        /// tblInfracciones
        /// </summary>
        public string folioEmergencia { get; set; } = null!;

        /// <summary>
        /// tblcatEstatusInfraccion ddl
        /// </summary>
        public int? IdEstatus { get; set; }

        /// <summary>
        /// tblInfracciones
        /// </summary>
        public string placas { get; set; } = null!;
        /// <summary>
        /// tblInfracciones
        /// </summary>
        public string serie { get; set; } = null!;
        /// <summary>
        /// tblcatTipoCortesia ddl Pendiente
        /// </summary>
        public int? IdTipoCortesia { get; set; }

        /// <summary>
        /// tblcatDependencias ddl
        /// </summary>
        public int? IdDependencia { get; set; }

        /// <summary>
        /// tblcatGarantias ddl
        /// </summary>
        public int? IdGarantia { get; set; }

        /// <summary>
        /// tblcatDelegaciones ddl
        /// </summary>
        public int? IdDelegacion { get; set; }

        /// <summary>
        /// tblInfracciones campo fechaInfraccion
        /// </summary>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaInicio { get; set; }

        /// <summary>
        /// tblInfracciones campo fechaInfraccion
        /// </summary>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaFin { get; set; }

        /// <summary>
        /// tblInfracciones
        /// </summary>
        public string Propietario { get; set; } = null!;

        /// <summary>
        /// pediente aun no se que tabla
        /// </summary>
        public string NumeroLicencia { get; set; } = null!;

        /// <summary>
        /// tblInfracciones
        /// </summary>
        public string Conductor { get; set; } = null!;

        /// <summary>
        /// tblVehiculo pediente posible tabla vehiculo
        /// </summary>
        public string NumeroEconomico { get; set; } = null!;
        
         public int? IdMarca { get; set; } = null!;
        public int? IdSubmarca { get; set; } = null!;
        public int? IdCarretera { get; set; } = null!;
        public int? IdMunicipio { get; set; } = null!;
        public int?   IdTramo{ get; set; } = null!;
        public int? IdOficial { get; set; } = null!;
        public int? IdEntidadRegistro { get; set; } = null!;
        public int? IdTipoVehiculo { get; set; } = null!;
        public int? IdTipoServicio { get; set; } = null!;
        public int? IdSubtipoServicio { get; set; } = null!;
        public int? IdAplicacion { get; set; } = null!;
        public string kilometro { get; set; } = null!; 
        public string modelo { get; set; } = null!;
        public int? IdTipoMotivo { get; set; } = null!;

        public List<InfraccionesModel> ListInfracciones { get; set; }

    }
}
