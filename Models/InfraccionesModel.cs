using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class InfraccionesModel : EntityModel
    {

      
        public string documento { get; set; }
        public decimal monto { get; set; }
        public int estatusEnvio { get;set; }
        public int estatusCortesia { get; set; } = 0;
        public string ObservacionesSub { get; set; } = "";
        public string ObservacionsesApl { get; set; } = "";
        public int? transito { get; set; }
        public string ObsevacionesApl { get; set; }
        public int idInfraccion { get; set; }
        public int? idOficial { get; set; }
        public int? idDependencia { get; set; }
        public int? idDelegacion { get; set; }
        public int? idVehiculo { get; set; }
        public int? idAplicacion { get; set; }
        public int? idGarantia { get; set; }
        public int? idEstatusInfraccion { get; set; }
        public int? idMunicipio { get; set; }
        public string municipio { get; set; }
        public int? idTramo { get; set; }
        public int? idCarretera { get; set; }
        public int? idPropitario { get; set; }
        
        public int? idPersona { get; set; }
        public int? idCortesia { get; set; }
        
        public string placasVehiculo { get; set; }
        public string folioInfraccion { get; set; }
        public int? emergenciasId { get; set; }
        public string folioEmergencia { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime ?fechaNacimiento { get; set; }

		[Display(Name = "Fecha")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime fechaInfraccion { get; set; } 
		public DateTime horaInfraccion { get; set; } 
        public TimeOnly hora { get; set; }

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime fechaVencimiento { get; set; } = DateTime.Now;

		public string aplicacion { get; set; }

		public void CalcularFechas()
        {
            // Agregar 10 d�as a la fecha de imposici�n para obtener la fecha de vencimiento
            fechaVencimiento = fechaInfraccion.AddDays(10);
        }
        public string kmCarretera { get; set; }
        public string observaciones { get; set; }
        public string observacionesLugar { get; set; }
        public string? lugarCalle { get; set; }
        public string? lugarNumero { get; set; }
        public string? lugarColonia { get; set; }
        public string? lugarEntreCalle { get; set; }
		public string? Direccion { get; set; }
		public string? Latitud { get; set; }
		public string? Longitud { get; set; }
		public bool? infraccionCortesia { get; set; }
        public int? infraccionCortesiaValue { get; set; }

        public string infraccionCortesiaString { get; set; }

        public int cortesiaInt { get; set; }
        public string NumTarjetaCirculacion { get; set; }
        public string placaInfraccion { get; set; }
        public string licenciaInfraccion { get; set; }
        public string tipoLicenciaInfraccion { get; set; }

        public bool isPropietarioConductor { get; set; }
        public string strIsPropietarioConductor { get; set; }
        public string estatusInfraccion { get; set; }
        public string observacionesCortesia { get; set; }
        public string nombreOficial { get; set; }
        public string apellidoPaternoOficial { get; set; }
        public string apellidoMaternoOficial { get; set; }
        public string nombreCompletoOficial
        {
            get
            {
                return $"{nombreOficial} {apellidoPaternoOficial} {apellidoMaternoOficial}";
            }
        }
        public string tramo { get; set; }
        public string telefono { get; set; }      
        public string carretera { get; set; }
        public virtual VehiculoModel Vehiculo { get; set; }
        public PersonaModel Persona { get; set; }
        
        public virtual PersonaInfraccionModel PersonaInfraccion { get; set; }
         public virtual PersonaModel PersonaInfraccion2 { get; set; }


        public virtual PersonaInfraccionModel PersonaConductor { get; set; }
        public virtual PersonaModel PersonaConductor2 { get; set; }


        public virtual IEnumerable<MotivosInfraccionVistaModel> MotivosInfraccion { get; set; }
        public virtual GarantiaInfraccionModel Garantia { get; set; }

        #region Columnas Adicionales Reportes
        public string delegacion { get; set; }
        public string NombreConductor { get; set; }
        public string NombrePropietario { get; set; }
        public string NombreGarantia { get; set; }

        public decimal umas { get; set; }
        public decimal totalInfraccion { get; set; }
        public int idPersonaConductor { get; set; }
        public int Total { get; set; }
        public IFormFile myFile { get; set; }
        public string inventarios { get; set; }
        public string Nombreinventarios { get; set; }
        public string tarjetaInfraccion { get; set; }
		//PARA BOLETA FISICA
		public IFormFile boletaInfraccion { get; set; }

		public string boletaFisicaPath { get; set; }
		public string nombreBoletaStr { get; set; }

        public int? IdTurno { get; set; }

		#endregion
	}
    public class InfraccionesReportModel : EntityModel
    {

        public string horaInfraccion { get; set; }
        public string AplicadaA { get; set; }
        public string observaciones { get; set; }
        public decimal Uma { get; set; }
        public int idInfraccion { get; set; }
        public string folioInfraccion { get; set; }
        public DateTime fechaInfraccion { get; set; }
        public DateTime fechaVencimiento { get; set; }
        public string estatusInfraccion { get; set; }
        public string nombreOficial { get; set; }  
        public string municipio { get; set; }
        public string carretera { get; set; }
        public string colonia { get; set; }
        public string calle { get; set; }
        
        public string numero { get; set; }
        public string entreCalle { get; set; }

        public string tramo { get; set; }
        public string kmCarretera { get; set; }
        public string nombreConductor { get; set; }
        public string domicilioConductor { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? fechaNacimientoConductor { get; set; }
        public int? edadConductor { get; set; }
        public string generoConductor { get; set; }
        public string telefonoConductor { get; set; }
        public string numLicenciaConductor { get; set; }
        public string tipoLicenciaConductor { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? vencimientoLicConductor { get; set; }
        public string placas {  get; set; }
        public string tipoVehiculo { get; set; }
        public string marcaVehiculo { get; set; }
        public string nombreSubmarca { get; set; }
        public string modelo { get; set; }
        public string color { get; set; }
        public string nombrePropietario { get; set; }
        public string domicilioPropietario { get; set; }
        public string serie { get; set; }
        public string NumTarjetaCirculacion { get; set; }
        public string nombreEntidad { get; set; }
        public string tipoServicio { get; set; }
        public string numeroEconomico {  get; set; }
        public string cortesia { get; set; }

        public bool tieneCortesia { get; set; }
        public decimal montoCalificacion { get; set; }
        public decimal montoPagado { get; set; }
        public string reciboPago { get; set; }
        public string oficioCondonacion { get;set; }
        public DateTime? fechaPago { get; set; }
        public string lugarPago { get; set; }
        public string concepto { get; set; }
        public decimal umas { get; set; }
        public decimal totalInfraccion { get; set; }
        public int idGarantia { get; set; }
        public virtual List<MotivosInfraccionVistaModel> MotivosInfraccion { get; set; }
        public virtual GarantiaInfraccionModel Garantia { get; set; }

	

		
	}
}
