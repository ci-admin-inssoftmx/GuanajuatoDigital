using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatAutoridadesDisposicionService
    {
        List<CatAutoridadesDisposicionModel> ObtenerAutoridadesActivas(int corp);
        CatAutoridadesDisposicionModel GetAutoridadesByID(int IdAutoridadDisposicion,int corp);
        public int GuardarAutoridad(CatAutoridadesDisposicionModel autoridad, int corp);
        public int UpdateAutoridad(CatAutoridadesDisposicionModel autoridad, int corp);

    }
}
