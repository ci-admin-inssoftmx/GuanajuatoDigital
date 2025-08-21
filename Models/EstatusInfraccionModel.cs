using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class EstatusInfraccionModel
    {
        public int idEstatusInfraccion { get; set; }
        public string estatusInfraccion { get; set; }
        public DateTime fechaActualizacion { get; set; }
        public int actualizadoPor { get; set; }
        public int estatus { get; set; }

    }
}
