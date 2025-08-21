using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatSubtipoServicioModel
    {
        public int idTipoServicio { get; set; }
        public int idSubTipoServicio { get; set; }
        public string subTipoServicio { get; set; }
        public string tipoServicio { get; set; }

        public int estatus { get; set; }
        public string estatusDesc { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public bool SwitchEstatusSubtipoServicio { get; set; }

        public int? ActualizadoPor { get; set; }

    }
}
