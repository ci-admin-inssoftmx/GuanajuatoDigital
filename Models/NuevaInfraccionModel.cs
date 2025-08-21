using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class NuevaInfraccionModel
    {
        public int IdAccidente { get; set; }
        public int IdVehiculo { get; set; }
        public int? IdPersona { get; set; }
        
        public int? IdPersonaConductor { get; set; }
        public int? IdPersonaInfraccion { get; set; }

        public int? idOficial { get; set; }
        public string folioInfraccion { get; set; }
        public string folioEmergencia { get; set; }
        public int? IdMunicipio { get; set; }
        public int? IdCarretera { get; set; }
        public int? IdTramo { get; set; }
        public string Kilometro { get; set; }
		
		public int? IdDelegacion { get; set; }

		public int? IdPropietario { get; set; }
        public string Placa { get; set; }
        public string Tarjeta { get; set; }
        public int idInfraccion{ get; set; }
        public int? idDependencia { get; set; }
        public int? idDelegacion { get; set; }

        public int? idAplicacion { get; set; }
        public int? idGarantia { get; set; }
        public int? idEstatusInfraccion { get; set; }
        public string MontoCamino { get; set; }
        public string MontoCarga { get; set; }
        public string MontoPropietarios { get; set; }
        public string MontoOtros { get; set; }

		
		public DateTime fechaInfraccion { get; set; } = DateTime.Now;
		public DateTime fechaVencimiento { get; set; }

		public string observaciones { get; set; }
        public string lugarCalle { get; set; }
        public string lugarNumero { get; set; }
        public string lugarColonia { get; set; }
        public string lugarEntreCalle { get; set; }
        public bool? infraccionCortesia { get; set; }
        public bool isPropietarioConductor { get; set; }
        public string strIsPropietarioConductor { get; set; }
        public string estatusInfraccion { get; set; }
        public string observacionesCortesia { get; set; }

        public virtual VehiculoModel Vehiculo { get; set; }
        public PersonaModel Persona { get; set; }
        public virtual PersonaInfraccionModel PersonaInfraccion { get; set; }
        public virtual IEnumerable<MotivosInfraccionVistaModel> MotivosInfraccion { get; set; }
        public virtual GarantiaInfraccionModel Garantia { get; set; }
        public string delegacion { get; set; }
        public string NombreConductor { get; set; }
        public string NombrePropietario { get; set; }
        public string NombreGarantia { get; set; }

        public decimal umas { get; set; }
        public decimal totalInfraccion { get; set; }





    }
}
