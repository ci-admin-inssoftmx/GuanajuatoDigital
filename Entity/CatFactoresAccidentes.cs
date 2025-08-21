using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatFactoresAccidentes
    {
        public int IdFactorAccidente { get; set; }

        public string FactorAccidente { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? transito { get; set; }

        public int? Estatus { get; set; }

        public virtual ICollection<CatFactoresOpcionesAccidentes> Factor { get; } = new List<CatFactoresOpcionesAccidentes>();

    }
}
