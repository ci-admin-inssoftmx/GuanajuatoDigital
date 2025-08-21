using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class GarantiasModel
    {
        public int idGarantia { get; set; }
        public string garantia { get; set; }
        public DateTime fechaActualizacion { get; set; }
        public int actualizadoPor { get; set; }
        public int estatus { get; set; }

    }
}
