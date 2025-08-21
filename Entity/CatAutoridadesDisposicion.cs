using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatAutoridadesDisposicion
    {
        public int IdAutoridadDisposicion { get; set; }

        public string NombreAutoridadDisposicion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
    }
}
