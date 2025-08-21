using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ITipoCortesiaService
    {
        public List<TipoCortesiaModel> GetTipoCortesias();
    }
}
