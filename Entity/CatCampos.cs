using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatCampos
    {
        public int IdCampo { get; set; }

        public string NombreCampo { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }
    }
}
