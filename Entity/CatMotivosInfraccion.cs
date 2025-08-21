using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatMotivosInfraccion
    {
        public int idCatMotivoInfraccion { get; set; } 
        public string Nombre { get; set; } 
        public string Fundamento { get; set; } 
        public int CalificacionMinima { get; set; } 
        public int CalificacionMaxima { get; set; }
        public int IdConcepto { get; set; }
        public int IdSubConcepto { get; set; }  
        public DateTime? FechaActualizacion { get; set; } 
        public int? ActualizadoPor { get; set; } 
        public int Estatus { get; set; }

        //public virtual ICollection<CatConceptoInfraccion> Conceptos { get; } = new List<CatConceptoInfraccion>();
        //public virtual ICollection<CatSubConceptoInfraccion> SubConceptos { get; } = new List<CatSubConceptoInfraccion>();


    }
}
