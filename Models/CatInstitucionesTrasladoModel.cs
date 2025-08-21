using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatInstitucionesTrasladoModel
    {
        public int IdInstitucionTraslado { get; set; }

        public string InstitucionTraslado { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
        public string estatusDesc { get; set; }
        public bool ValorEstatusInstTraslado { get; set; }
        public int? Corp { get; set; }


    }
}
