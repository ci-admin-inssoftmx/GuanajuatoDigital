namespace GuanajuatoAdminUsuarios.Models
{
    public class MotivoInfraccionModel : EntityModel
    {
        public int idMotivoInfraccion { get; set; }
        public int idConcepto { get; set; }
        public int IdSubConcepto { get; set; }
        public int idCatMotivoInfraccion { get; set; }
        public int calificacionMinima { get; set; }
        public int calificacionMaxima { get; set; }
        public int? calificacion { get; set; } 
        public int idInfraccion { get; set; }  


    }
}
