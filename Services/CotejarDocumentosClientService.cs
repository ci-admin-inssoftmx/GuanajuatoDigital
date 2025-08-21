using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using SITTEG.APIClientInfrastructure.Client;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.RESTModels;
using static GuanajuatoAdminUsuarios.RESTModels.CotejarDatosResponseModel;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CotejarDocumentosClientService : ICotejarDocumentosClientService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        protected readonly IGenericClient _apiClient;
        private const string uri = "https://alfasiae.guanajuato.gob.mx";

        public CotejarDocumentosClientService(IGenericClient apiClient, ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public void consumeService()
        {
            string url = "/RESTAdapter/CotejarDatos";
            dynamic requestModel = new { x = 1 };
            //var x = _apiClient.PostGenericResponse<, >(url, requestModel);
        }

        public RootCotejarDatosRes CotejarDatos(CotejarDatosRequestModel requestModel, string endPointName)
        {

            //var bodyRequest= "{\"Tp_folio\":\"4\",\"Folio\":\"E01038\",\"tp_consulta\":\"3\"}";
            var json = JsonConvert.SerializeObject(requestModel, Formatting.Indented);
            var bodyRequest = json;
            string result = string.Empty;
            RootCotejarDatosRes responseModel = new RootCotejarDatosRes();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection2()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SP_WebClientConnects", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@EndpointName", SqlDbType.NVarChar)).Value = endPointName;
                    command.Parameters.Add(new SqlParameter("@Body", SqlDbType.NVarChar)).Value = bodyRequest;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.NVarChar)).Value = 1;

                    command.CommandType = CommandType.StoredProcedure;
                    result = Convert.ToString(command.ExecuteScalar());
                    responseModel = JsonConvert.DeserializeObject<RootCotejarDatosRes>(result);
                }
                catch (SqlException ex)
                {
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }

            return responseModel;
        }

    }
}
