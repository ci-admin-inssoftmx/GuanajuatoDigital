using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Configuration;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatCausasAccidentesService
    {
        List<CatCausasAccidentesModel> ObtenerCausasActivas(int corp);
        public CatCausasAccidentesModel ObtenerCausaByID(int IdCausaAccidente);
        public int CrearCausa(CatCausasAccidentesModel model,int corp);
        public int EditarCausa(CatCausasAccidentesModel model);

    }
}
