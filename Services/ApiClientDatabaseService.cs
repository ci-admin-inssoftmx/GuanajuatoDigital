using GuanajuatoAdminUsuarios.Interfaces;
using Newtonsoft.Json;
using static GuanajuatoAdminUsuarios.RESTModels.CotejarDatosResponseModel;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Framework.Catalogs;
using static GuanajuatoAdminUsuarios.Framework.Catalogs.CatWebServicesEnumerator;

namespace GuanajuatoAdminUsuarios.Services
{
	public class ApiClientDatabaseService : IApiClientDatabaseService
	{
		private readonly ICatDictionary _catDictionary;
		private readonly ISqlClientConnectionBD _sqlClientConnectionBD;

		public ApiClientDatabaseService(ISqlClientConnectionBD sqlClientConnectionBD, ICatDictionary catDictionary)
		{
			_catDictionary = catDictionary;
			_sqlClientConnectionBD = sqlClientConnectionBD;
		}
		public TResponse HttpGet<TResponse, TRequest, TEnum>(TRequest requestModel, int wsService)
		{
			var json = JsonConvert.SerializeObject(requestModel, Formatting.Indented);
			var bodyRequest = json;
			string result = string.Empty;
			string endPointName = _catDictionary.GetCatalogSystem<TEnum>(wsService);
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection2()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand("SP_WebClientGetNoAuthentication", connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@EndpointName", SqlDbType.NVarChar)).Value = endPointName;
					command.Parameters.Add(new SqlParameter("@Body", SqlDbType.NVarChar)).Value = bodyRequest;
					command.CommandType = CommandType.StoredProcedure;
					result = Convert.ToString(command.ExecuteScalar());
					var responseModel = JsonConvert.DeserializeObject<TResponse>(result);
					return responseModel;
				}
				catch (SqlException ex)
				{
					return default;
				}
				finally
				{
					connection.Close();
				}
			}
		}
		public TResponse HttpPost<TResponse, TRequest, TEnum>(TRequest requestModel, int wsService,int corp)
		{
			var corporation = corp < 3 ? 1 : corp;
			var json = JsonConvert.SerializeObject(requestModel, Formatting.Indented);
			var bodyRequest = json;
			string result = string.Empty;
			string endPointName = _catDictionary.GetCatalogSystem<TEnum>(wsService);
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection2()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand("SP_WebClientPostNoAuthentication", connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@EndpointName", SqlDbType.NVarChar)).Value = endPointName;
					command.Parameters.Add(new SqlParameter("@Body", SqlDbType.NVarChar)).Value = bodyRequest;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = corporation;

                    command.CommandType = CommandType.StoredProcedure;
					result = Convert.ToString(command.ExecuteScalar());
					result = result.Replace("([{", "[{").Replace("}])", "}]");
					result = result.Substring(0, result.LastIndexOf("]") + 1);
					var responseModel = JsonConvert.DeserializeObject<TResponse>(result);
					return responseModel;
				}
				catch (SqlException ex)
				{
					return default;
				}
				finally
				{
					connection.Close();
				}
			}
		}
	}
}
