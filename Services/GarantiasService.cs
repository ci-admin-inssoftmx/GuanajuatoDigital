using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class GarantiasService : IGarantiasService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public GarantiasService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<GarantiasModel> GetGarantias()
        {
            List<GarantiasModel> ListGarantias = new List<GarantiasModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catGarantias where estatus=1", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            GarantiasModel garantia = new GarantiasModel();
                            garantia.idGarantia = Convert.ToInt32(reader["idGarantia"].ToString());
                            garantia.garantia = reader["garantia"].ToString().ToUpper();
                            ListGarantias.Add(garantia);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    //Guardar la excepcion en algun log de errores
                    //ex
                }
                finally
                {
                    connection.Close();
                }
            return ListGarantias;
        }
    }
}
