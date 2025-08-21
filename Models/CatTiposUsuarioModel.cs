using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatTiposUsuarioModel
    {
        public int idTipoUsuario { get; set; }

        public string tipoUsuario { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
    }
}
