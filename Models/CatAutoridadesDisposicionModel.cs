using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatAutoridadesDisposicionModel
    {
        public int IdAutoridadDisposicion { get; set; }

        public string NombreAutoridadDisposicion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
        public bool ValorEstatusAutoridadesDisp { get; set; }
        public int? Corp { get; set; }


    }
}
