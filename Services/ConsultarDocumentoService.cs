using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.RESTModels;
using Newtonsoft.Json;
using SITTEG.APIClientInfrastructure.Client;
using System.Data;
using System;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoRequestModel;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;
using static GuanajuatoAdminUsuarios.RESTModels.CotejarDatosResponseModel;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class ConsultarDocumentoService : IConsultarDocumentoService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;

        public ConsultarDocumentoService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public RootConsultarDocumentoResponse ConsultarDocumento(RootConsultarDocumentoRequest requestModel, string endPointName)
        {
            var json = JsonConvert.SerializeObject(requestModel, Formatting.Indented);
            var bodyRequest = json;
            string result = string.Empty;
            RootConsultarDocumentoResponse responseModel = new RootConsultarDocumentoResponse();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection2()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SP_ConsultarDocumentoRest", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@EndpointName", SqlDbType.NVarChar)).Value = endPointName;
                    command.Parameters.Add(new SqlParameter("@Body", SqlDbType.NVarChar)).Value = bodyRequest;
                    command.CommandType = CommandType.StoredProcedure;
                    result = Convert.ToString(command.ExecuteScalar());
                    try
                    {
                        responseModel = JsonConvert.DeserializeObject<RootConsultarDocumentoResponse>(result);

                    }
                    catch(Exception e)
                    {
                        var a = e;
                    }
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
