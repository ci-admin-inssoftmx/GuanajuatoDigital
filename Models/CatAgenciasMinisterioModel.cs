using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatAgenciasMinisterioModel
    {
        public int IdAgenciaMinisterio { get; set; }

        public string NombreAgencia { get; set; }

        public int IdDelegacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public bool EstatusValue { get; set; }

        public string estatusDesc { get; set; }

        public string DelegacionDesc { get; set; }
        public int? Corp { get; set; }


    }
}
