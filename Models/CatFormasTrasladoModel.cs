using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatFormasTrasladoModel
    {
        public int idFormaTraslado { get; set; }

        public string formaTraslado { get; set; }

        public DateTime? fechaActualizacion { get; set; }

        public int? actualizadoPor { get; set; }

        public int? estatus { get; set; }

        public string estatusDesc { get; set; }
    }
}
