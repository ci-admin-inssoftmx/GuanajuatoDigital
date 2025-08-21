using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatAgenciasMinisterio
    {
        public int IdAgenciaMinisterio { get; set; }

        public string NombreAgencia { get; set; }

        public int IdDelegacion { get; set; }

        public int transito { get; set; }


		public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public virtual ICollection<Delegaciones> Delegaciones { get; } = new List<Delegaciones>();

    }
}
