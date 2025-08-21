using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IPlacaServices
    {
        List<PlacaModel> GetPlacasByDelegacionId(int idPension, bool? noEsPension);
        public List<PlacaModel> GetPlacasIngresos(int idPension);
        public List<PlacaModel> GetPlacasSalidas(int idPension);

        List<PlacaModel> GetPlacasLiberacion(int idOficina); 
    }
}
