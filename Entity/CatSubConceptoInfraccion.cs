using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatSubConceptoInfraccion
    {
        public int idSubConcepto { get; set; }

        public string subConcepto { get; set; }

        public int idConcepto { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
        //public virtual ICollection<CatConceptoInfraccion> Conceptos { get; } = new List<CatConceptoInfraccion>();
    }
}
