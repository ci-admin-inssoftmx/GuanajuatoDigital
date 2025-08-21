namespace GuanajuatoAdminUsuarios.Models
{
    public class Gruas2Model : EntityModel
    {
        
            
        public int idDeposito { get; set; }
        public int idGrua { get; set; }
        public int idConcesionario { get; set; }
        public int idClasificacion { get; set; }
        public int? idDelegacion { get; set; }
        public int? idConcesionarioBusqueda { get; set; }
        public int idTipoGrua { get; set; }
        public int idSituacion { get; set; }
        public string noEconomico { get; set; }
        public string placas { get; set; }
        public string modelo { get; set; }
        public string capacidad { get; set; }
        public string concesionario { get; set; }
        public string Delegacion { get; set; }

        public string municipio { get; set; }
        
        public string clasificacion { get; set; }
        public string tipoGrua { get; set; }
        public string situacion { get; set; }
        public bool isPension { get; set; }

    }
}
