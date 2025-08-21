using GuanajuatoAdminUsuarios.RESTModels;
using static GuanajuatoAdminUsuarios.RESTModels.AnulacionDocumentoRequestModel;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IAnulacionDocumentoService
    {
        public RootAnulacionDocumentoResponse CancelarMultasTransitoFinanzas(RootAnulacionDocumentoRequest requestModel);
    }
}
