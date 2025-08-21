using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class Concesionarios2Model : EntityModel
    {
        public int idConcesionario { get; set; }
        public string nombre { get; set; }
        public int idDelegacion { get; set; }
        public int idMunicipio { get; set; }
        public string alias { get; set; }
        public string razonSocial { get; set; }
        public string delegacion { get; set; }
        public string municipio { get; set; }
    }
}
