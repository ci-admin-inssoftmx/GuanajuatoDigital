using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatFactoresOpcionesAccidentes
    {
        public int IdFactorOpcionAccidente { get; set; }

        public string FactorOpcionAccidente { get; set; }

        public int IdFactorAccidente { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public virtual ICollection<CatFactoresAccidentes> FactorAccidente { get; } = new List<CatFactoresAccidentes>();


    }
}
