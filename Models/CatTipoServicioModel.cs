using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatTipoServicioModel
    {
        public int idCatTipoServicio { get; set; }

        public string tipoServicio { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
        public bool SwitchEstatusTipoServicio { get; set; }
    }
}
