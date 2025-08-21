using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models.Catalogos.Turnos
{
    public class TurnosIndexViewModel
    {
        public TurnoFilterModel Filter { get; set; }
        public List<TurnoDetailsModel> Lista { get; set; } = new List<TurnoDetailsModel>();
    }
}
