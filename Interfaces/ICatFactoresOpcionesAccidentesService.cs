using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatFactoresOpcionesAccidentesService
    {      
        List<CatFactoresOpcionesAccidentesModel> ObtenerOpcionesFactorAccidente(int corp); 
        List<CatFactoresOpcionesAccidentesModel> ObtenerOpcionesPorFactor(int factorDDValue,int corp);
        List<CatFactoresOpcionesAccidentesModel> ObtenerOpcionesPorCorporacion(int corp);

        CatFactoresOpcionesAccidentesModel ObtenerOpcionesParaEditar(int IdFactoropcionAccidente, int corp);
        public int CrearOpcionFactor(CatFactoresOpcionesAccidentesModel model, int corp);
        public int EditarOpcionFactor(CatFactoresOpcionesAccidentesModel model)
;

    }
}
