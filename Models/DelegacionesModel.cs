using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class DelegacionesModel
    {
        public int IdDelegacion { get; set; }

        public string Delegacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
    }
}
