using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatConceptoInfraccion
    {
        public int idConcepto { get; set; }

        public string concepto { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
    }
}
