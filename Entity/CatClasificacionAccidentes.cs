using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatClasificacionAccidentes
    {
        public int IdClasificacionAccidente { get; set; }

        public string NombreClasificacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
    }
}
