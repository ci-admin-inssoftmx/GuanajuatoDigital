using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatPuesto
    {
        public int IdPuesto { get; set; }
        public string NombrePuesto { get; set; }
        public string Descripcion { get; set; }
        public int Estatus { get; set; }
        public int? IdDelegacion { get; set; }
        public int Transito { get; set; }        
        public DateTime FechaCreacion { get; set; }
        public int ActualizadoPor { get; set; }

        public CatDelegacione Delegacion { get; set; }
    }
}
