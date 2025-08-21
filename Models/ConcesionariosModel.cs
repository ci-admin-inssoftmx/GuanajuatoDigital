using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class ConcesionariosModel
    {
        public int IdConcesionario { get; set; }
        public string Concesionario { get; set; }
        public DateTime fechaActualizacion { get; set; }
        public int actualizadoPor { get; set; }
        public int estatus { get; set; }
    }
}
