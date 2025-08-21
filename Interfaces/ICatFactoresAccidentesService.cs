using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatFactoresAccidentesService
    {
        List<CatFactoresAccidentesModel> GetFactoresAccidentes(int corp);
        List<CatFactoresAccidentesModel> GetFactoresAccidentesActivos(int corp);
        CatFactoresAccidentesModel GetFactorByID(int IdFactorAccidente);
        public int GuardarFactor(CatFactoresAccidentesModel factor,int corp);
        public int UpdateFactor(CatFactoresAccidentesModel factor);

    }
}
