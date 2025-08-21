using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatTramosModel
    {
        public int IdTramo { get; set; }

        public string Tramo { get; set; }
        public string Carretera { get; set; }

        public DateTime? FechaActualizacion { get; set; }
        
        public int IdDelegacion { get; set; }

        public int IdCarretera { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
        public bool ValorEstatusTramo { get; set; }
        public int? Corp { get; set; }

    }
}
