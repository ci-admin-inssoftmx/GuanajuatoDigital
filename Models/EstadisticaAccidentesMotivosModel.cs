namespace GuanajuatoAdminUsuarios.Models
{
    public class EstadisticaAccidentesMotivosModel
    {
        public string Motivo { get; set; }
		public string Delegacion { get; set; }
		public int Contador { get; set; } 
        public int idInfraccion { get; set; }
    }
}
