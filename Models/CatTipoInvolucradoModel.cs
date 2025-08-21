using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatTipoInvolucradoModel
    {
        public int IdTipoInvolucrado { get; set; }

        public string TipoInvolucrado { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
    }
}
