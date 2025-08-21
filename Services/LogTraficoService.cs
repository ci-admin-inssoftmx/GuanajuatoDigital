using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class LogTraficoService : ILogTraficoService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public LogTraficoService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public void CreateLog(LogTraficoModel log)
        {
            string strQuery = @"INSERT INTO LogTrafico(jsonRequest,jsonResponse,fecha,valor,api) 
                                VALUES(@jsonRquest,@jsonResponse,@fecha,@valor,@api)
            ";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@jsonRquest", log.jsonRequest);
                    command.Parameters.AddWithValue("@jsonResponse", log.jsonResponse);
                    command.Parameters.AddWithValue("@fecha", log.fecha);
                    command.Parameters.AddWithValue("@valor", log.valor);//Pagada
                    command.Parameters.AddWithValue("@api", log.api);//Pagada
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
