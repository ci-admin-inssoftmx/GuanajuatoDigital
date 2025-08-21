using System;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class RegistroReciboPagoModel
    {
        public int? IdInfraccion { get; set; }
        public string FolioInfraccion { get; set; }
        public string Placas { get; set; }

        public int IdOficial { get; set; }
        public int IdDependencian { get; set; }
        public int IdDelegacion { get; set; }
        public string Oficial { get; set; }
        public string Municipio { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaInfraccion { get; set; }
        public string FechaInfraccioFormateada => FechaInfraccion.ToString("dd/MM/yyyy");

        public string Carretera { get; set; }
        public string Tramo { get; set; }
        public string KmCarretera { get; set; }
        public int IdVehiculo { get; set; }
        public int IdConductor { get; set; }

        public string Conductor { get; set; }
        public string Propietario { get; set; }
        public string Serie { get; set; }
        public string Delegacion { get; set; }


        public int IdAplicacion { get; set; }
        public int InfraccionCortesia { get; set; }
        public int IdGarantia { get; set; }
        public int EstatusProceso { get; set; }
        public string EstatusInfraccion { get; set; }

        public DateTime? FechaActualizacion { get; set; }
        public int ActualizadoPor { get; set; }
        public int Estatus { get; set; }

        public string ReciboPago { get; set; }
        public float? Monto { get; set; }
		public string MontoRecibido { get; set; }
        
        public DateTime? FechaPago { get; set; }
        public string LugarPago { get; set; }
        public string MontoSTR { get; set; }

        public int Calificacion { get; set; }

     



    }
}
