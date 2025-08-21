using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatAutoridadesEntregaService
    {
        List<CatAutoridadesEntregaModel> ObtenerAutoridadesActivas(int corp);
        CatAutoridadesEntregaModel GetAutoridadesByID(int IdAutoridadEntrega, int corp);
        public int GuardarAutoridad(CatAutoridadesEntregaModel autoridad, int corp);
        public int UpdateAutoridad(CatAutoridadesEntregaModel autoridad, int corp);

    }
}
