using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatOficinasRentaModel
    {
        public int IdOficinaRenta { get; set; }

        public string NombreOficina { get; set; }

        public int IdDelegacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
        public string estatusDesc { get; set; }

        public string DelegacionDesc { get; set; }

         public bool ValorEstatusOficinasRenta { get; set; }
        public int Transito { get; set; }
        public int? Corp { get; set; }

    }
}
