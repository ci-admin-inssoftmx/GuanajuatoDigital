using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatClasificacionAccidentesModel
    {
        public int IdClasificacionAccidente { get; set; }

        public string NombreClasificacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }
        public string estatusDesc { get; set; }

        public bool ValorEstatusClasificacionAccidentes { get; set; }
        public int? Corp { get; set; }


    }
}
