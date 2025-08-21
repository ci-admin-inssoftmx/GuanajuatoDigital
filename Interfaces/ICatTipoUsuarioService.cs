using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatTipoUsuarioService
    {
        List<CatTiposUsuarioModel> ObtenerTiposUsuario();

    }
}
