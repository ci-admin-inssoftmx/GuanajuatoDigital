using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatTipoLicenciasModel
    {
        public int idTipoLicencia { get; set; }

        public string tipoLicencia { get; set; }

        public DateTime? fechaActualizacion { get; set; }

        public int? actualizadoPor { get; set; }

        public int? estatus { get; set; }

        public string estatusDesc { get; set; }
    }
}
