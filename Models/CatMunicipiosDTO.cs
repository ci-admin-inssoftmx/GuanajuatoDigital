using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatMunicipiosDTO
    {
        public string Municipio2 { get; set; }
        public int IdEntidad2 { get; set; }
        public int IdOficinaTransporte2 { get; set; }
        public List<CatMunicipiosModel> MunicipiosModel { get; set; }

    }
}
