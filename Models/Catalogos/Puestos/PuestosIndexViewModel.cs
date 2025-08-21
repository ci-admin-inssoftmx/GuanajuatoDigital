using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models.Catalogos.Puestos
{
    public class PuestosIndexViewModel
    {
        public PuestoFilterModel Filter { get; set; }
        public List<PuestoModel> Lista { get; set; } = new List<PuestoModel>();
    }
}
