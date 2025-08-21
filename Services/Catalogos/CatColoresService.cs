using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace GuanajuatoAdminUsuarios.Services
{
    public class CatColoresService : IColores
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatColoresService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public int obtenerIdPorColor(string colorLimpio)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("SELECT idColor FROM catColores WHERE color = @color", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@color", SqlDbType.NVarChar)).Value = colorLimpio;
                    sqlCommand.CommandType = CommandType.Text;

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read()) // Intenta leer un registro del resultado
                        {
                            // Obtiene el valor de la columna "idMunicipio"
                            result = Convert.ToInt32(reader["idColor"]);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Manejo de errores y log
                    return result;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }
        public List<ColoresModel> ObtenerColoresActivosPorCorp(int corp)
        {
            //
            List<ColoresModel> listaColores = new List<ColoresModel>();
            var corporation = corp < 2 ? 1 : corp;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT catColores.*, estatus.estatusdesc 
                                                          FROM catColores 
                                                          JOIN estatus ON catColores.estatus = estatus.estatus 
                                                          WHERE catColores.estatus = 1 and transito = @corp", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ColoresModel colores = new ColoresModel();
                            colores.IdColor = Convert.ToInt32(reader["IdColor"].ToString());
                            colores.color = reader["color"].ToString();
                            //marcasVehiculo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            //marcasVehiculo.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            colores.Estatus = Convert.ToInt32(reader["Estatus"].ToString());
                            colores.estatusDesc = reader["estatusDesc"].ToString();
                            listaColores.Add(colores);
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
            return listaColores;


        }

    }
}
