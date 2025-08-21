using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatTipoMotivoAsignacionModel
    {
        public int idTipoAsignacion { get; set; }

        public string tipoAsignacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
    }
}
