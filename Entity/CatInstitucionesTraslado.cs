using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatInstitucionesTraslado
    {
        public int IdInstitucionTraslado { get; set; }

        public string InstitucionTraslado { get; set; }

        public DateTime? FechaActualizacion { get; set; }
        public int transito { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
    }
}
