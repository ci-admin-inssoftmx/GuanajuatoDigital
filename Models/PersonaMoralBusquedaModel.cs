using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class PersonaMoralBusquedaModel
    {
        public string RFCBusqueda { get; set; }
        public string RazonSocial { get; set; }
        public int IdTipoPersona { get; set; }
        public List<PersonaModel> PersonasMorales { get; set; }
    }
}
