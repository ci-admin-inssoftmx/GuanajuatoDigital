using Azure;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IMotivoInfraccionService
    {

        public CatMotivosInfraccionModel GetMotivoByID(int IdCatMotivoInfraccion, int idDependencia);
        public List<CatMotivosInfraccionModel> GetMotivos(int idDependencia);
        public List<CatMotivosInfraccionModel> GetMotivosDropDown(int idDependencia, int idSubconcepto,int idConcepto); 
        public List<CatMotivosInfraccionModel> GetSubconceptos(int idConceptoValue);
        public List<CatMotivosInfraccionModel> GetCatMotivos(int IdDependencia);
        public int CrearMotivo(CatMotivosInfraccionModel motivo, int IdDependencia);
        public int UpdateMotivo(CatMotivosInfraccionModel motivo, int IdDependencia);
        public int DeleteMotivo(CatMotivosInfraccionModel model);
        List<CatMotivosInfraccionModel> GetMotivosBusqueda(CatMotivosInfraccionModel model, int idDependencia);

    }
}
