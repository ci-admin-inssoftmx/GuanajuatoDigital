using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class TipoCortesiaModel
    {
        public int idTipoCortesia { get; set; }
        public string tipoCortesia { get; set; }
        public DateTime fechaActualizacion { get; set; }
        public int actualizadoPor { get; set; }
        public int estatus { get; set; }
    }
}
