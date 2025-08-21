using System;

namespace GuanajuatoAdminUsuarios.Entity
{
    public class CatTurno
    {
        public long IdTurno { get; set; }
        public int IdDelegacion { get; set; }
        public int IdMunicipio { get; set; }
        public string NombreTurno { get; set; }
        public TimeSpan InicioTurno { get; set; }
        public TimeSpan FinTurno { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int ActualizadoPor { get; set; }

        public virtual CatDelegacione Delegacion { get; set; }
        public virtual CatMunicipios Municipio { get; set; }
    }
}
