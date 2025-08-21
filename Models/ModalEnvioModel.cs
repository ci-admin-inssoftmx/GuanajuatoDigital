using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class ModalEnvioModel
    {
        public List<int> SelectedIds { get; set; }
        public string Oficio { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public int IdLugarEnvio { get; set; }
    }
}
