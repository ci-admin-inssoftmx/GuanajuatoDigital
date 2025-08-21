using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatOficinasRenta
    {
        public int IdOficinaRenta { get; set; }

        public string NombreOficina { get; set; }

        public int IdDelegacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public virtual ICollection<Delegaciones> Delegaciones { get; } = new List<Delegaciones>();

    }
}
