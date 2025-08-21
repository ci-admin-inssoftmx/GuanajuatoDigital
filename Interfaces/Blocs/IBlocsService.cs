using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models.Blocs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Interfaces.Blocs
{
    public interface IBlocsService
    {
        BlocsAsignaDto AsignaBloc(string NSerie);        
        BlocModel GetBlocBySerie(string serie);
        string AsignarOficialBloc(long regId, int idOficial, string urlEvidencia, int folioInicial, int folioFinal, int idUsuario, string nombreUsuario, int iddelegacion, string delegacion);
        BlocsBlocbySerieDto BlocBySerie(string Serie);
        string CalculaSerie(int idCatBloc);
        int ExistFolio(int folio,int delegacion , int idOfical , int idflujo);
        void UpdateFolio(int idFolio,int idOperacio,int flujoid);
        bool ValidarFolios(int folioI,int foliofinal,int delegacion,int idflujo);

        Task<string> CancelarlBloc(long RegId, string Comen, int IdUsuario);
        Task<string> CanselarFolio(long id, int IdUsuario);
        string ElimBloc(int idBloc);
        (string B64 , string mimetyte) GetBase64(int idBloc);
        List<BlocsCatalogoModel> getCatBlocs();
        List<BlocsCatEdosModel> getCatEstado();
        List<BlocsOficialDto> getCatOficial();
        List<BlocsAltaModel> getLisBlocsAlta();
        List<BlocListItemModel> getLisBlocsConsulta();
        List<BlocsInventarioModel> getLisBlocsInventario();
        int IdDelegacion(string Delegacion);
        string NuevoBloc(RegistraBloc bloc);
    }
}
