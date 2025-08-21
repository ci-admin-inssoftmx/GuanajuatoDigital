using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatClasificacionAccidentes
    {
        List<CatClasificacionAccidentesModel> GetClasificacionAccidentes(int corp);

        List<CatClasificacionAccidentesModel> ObtenerClasificacionesActivas(int corp);

        public CatClasificacionAccidentesModel GetClasificacionAccidenteByID(int IdClasificacionAccidente);
        public int CrearClasificacionAccidente(CatClasificacionAccidentesModel model,int corp);
        public int EditarClasificacionAccidente(CatClasificacionAccidentesModel model);

    }
}
