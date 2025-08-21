using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatSalariosMinimos
    {
        public int IdSalario { get; set; }

        public string Area { get; set; }

        public float Salario { get; set; }
        public DateTime? Fecha { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
    }
}
