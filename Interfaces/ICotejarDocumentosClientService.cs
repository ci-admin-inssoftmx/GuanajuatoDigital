using GuanajuatoAdminUsuarios.RESTModels;
using static GuanajuatoAdminUsuarios.RESTModels.CotejarDatosResponseModel;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICotejarDocumentosClientService
    {
        RootCotejarDatosRes CotejarDatos(CotejarDatosRequestModel requestModel, string endPointName);
    }
}
