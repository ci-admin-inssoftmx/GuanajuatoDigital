using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class LogTraficoModel
    {
        public int Id { get; set; }
        public string jsonRequest { get; set; }
        public string jsonResponse { get; set; }
        public DateTime fecha { get; set; }
        public string valor { get; set; }
        public string api { get; set; }
    }
}
