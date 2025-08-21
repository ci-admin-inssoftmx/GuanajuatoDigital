using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CortesiasNoAplicadasModel
    {
        public int? IdInfraccion { get; set; }
        public string folioInfraccion { get; set; }
        public string ObservacionesSub { get; set; }
        public string FechaInfraccion { get; set; }
        public string Conductor { get; set; }
        public string Placas { get; set; }
        public string Serie { get; set; }
        public string Propietario { get; set; }
        public string Estatus { get; set; }
        public string Oficial { get; set; }
        public string Camino { get; set; }
        public string Municipio { get; set; }
        public string Carretera { get; set; }
        public string Tramo { get; set; }
        public string KmCarretera { get; set; }
        public string Vehiculo { get; set; }
        public int IdVehiculo { get; set; }
        public int InfraccionCortesia { get; set; }
        public string EstatusProceso { get; set; }
        public string Delegacion { get; set; }
        public string TipoAplicacion { get; set; }
        public string TipoCortesia { get; set; }
        public string CalificacionTotal { get; set; }
        public string TipoGarantia { get; set; }
        public string TipoPlaca { get; set; }
        public string TipoLicencia { get; set; }
        public string Licencia { get; set; }
        public string Tarjeta { get; set; }
        public string ArchivoInventario { get; set; }

        public DateTime? FechaVencimiento { get; set; }
        public string MontoCalificacion { get; set; }
        public string MontoPagado { get; set; }
        public string Recibo { get; set; }
        public DateTime? FechaPago { get; set; }
        public string LugarPago { get; set; }
        public string OficioConDonacion { get; set; }
        public string Observaciones { get; set; }
        public string Capturista { get; set; }
        public string Baja { get; set; }
        public decimal umas { get; set; }
        public decimal totalInfraccion { get; set; }
        public int Total { get; set; }
        public DateTime fecha { get; set; }

        public virtual IEnumerable<MotivosInfraccionVistaModel> MotivosInfraccion { get; set; }

    }
}
