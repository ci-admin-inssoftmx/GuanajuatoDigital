using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatCascoModel
    {
        public int IdCasco { get; set; }

        public string Casco { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
    }
}
