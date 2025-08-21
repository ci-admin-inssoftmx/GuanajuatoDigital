using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class EventoModel
    {
        public int IdEvento { get; set; }
        public string Evento { get; set; }
        public DateTime fechaActualizacion { get; set; }
        public int actualizadoPor { get; set; }
        public int estatus { get; set; }
    }
}
