using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatEntidadesModel
    {
        public int idEntidad { get; set; }

        public string nombreEntidad { get; set; }

        public DateTime? fechaActualizacion { get; set; }

        public int? actualizadoPor { get; set; }

        public int? estatus { get; set; }

        public string estatusDesc { get; set; }
        public bool ValorEstatusEntidad { get; set; }

        public static int GUANAJUATO=11;
        public int? Corp { get; set; }
        
    }
}
