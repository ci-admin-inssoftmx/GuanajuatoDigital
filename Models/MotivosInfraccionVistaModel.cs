using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class MotivosInfraccionVistaModel
    {
        public int prioridad { get; set; }
        public int idMotivoInfraccion { get; set; }
        public string Nombre { get; set; }
        public string Fundamento { get; set; }
        public int CalificacionMinima { get; set; }
        public int CalificacionMaxima { get; set; } 
        public int idCatMotivoInfraccion { get; set; }
        public int idInfraccion { get; set; }
         public int NumeroContinuo { get; set; }

        public int? calificacion { get; set; }
        public string Motivo { get; set; }
        public int IdConcepto { get; set; }
        public string Concepto { get; set; }
        public int IdSubConcepto { get; set; }
        public string SubConcepto { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public int? ActualizadoPor { get; set; }
        public int Estatus { get; set; }

        public class CalificacionEditorModel
        {
            public decimal? calificacion { get; set; }
            public decimal CalificacionMinima { get; set; }
            public decimal CalificacionMaxima { get; set; }
        }


    }
}
