using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class PadronDepositosGruasBusquedaModel
    {
        public int IdMunicipio { get; set; }

        public int IdConcesionario { get; set; }

        public int IdPension { get; set; }
        public int IdTipoGrua { get; set; }

        public List<PadronDepositosGruasModel> ListPadronDepositosGruas { get; set; }
    }
}
