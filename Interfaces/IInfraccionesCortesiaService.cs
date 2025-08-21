using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IInfraccionesCortesiaService
    {
        List<InfraccionesCortesiaModel> ObtenerTodasInfraccionesCortesia(int idOficina, int corp);
        public int GuardarNuevaFecha(InfraccionesCortesiaModel model);
        public int ModificarEstatusCortesia(int idInfraccion);


    }
}
