using GuanajuatoAdminUsuarios.RESTModels;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoRequestModel;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;
using static GuanajuatoAdminUsuarios.RESTModels.CotejarDatosResponseModel;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IConsultarDocumentoService
    {
        RootConsultarDocumentoResponse ConsultarDocumento(RootConsultarDocumentoRequest requestModel, string endPointName);
    }
}

