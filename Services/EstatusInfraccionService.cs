using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class EstatusInfraccionService: IEstatusInfraccionService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public EstatusInfraccionService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }


        public List<EstatusInfraccionModel> GetEstatusInfracciones()
        {
            List<EstatusInfraccionModel> ListEstatus = new List<EstatusInfraccionModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM catEstatusInfraccion WHERE estatus = 1 AND idEstatusInfraccion <> @idEstatus", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idEstatus", 8);

                    // sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            EstatusInfraccionModel estatus = new EstatusInfraccionModel();
                            estatus.idEstatusInfraccion = Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
                            estatus.estatusInfraccion = reader["estatusInfraccion"].ToString().ToUpper();
                            ListEstatus.Add(estatus);
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
            return ListEstatus;
        }

        public List<EstatusInfraccionModel> GetEstatusInvisibleInfracciones()
        {
            List<EstatusInfraccionModel> ListEstatus = new();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catEstatusInfraccion where estatus = 1 and estatusInfraccion = 'Invisible'", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            EstatusInfraccionModel estatus = new()
                            {
                                idEstatusInfraccion = Convert.ToInt32(reader["idEstatusInfraccion"].ToString()),
                                estatusInfraccion = reader["estatusInfraccion"].ToString()
                            };
                            ListEstatus.Add(estatus);
                        }
                    }
                }
                catch (SqlException ex)
                {
                }
                finally
                {
                    connection.Close();
                }
            return ListEstatus;
        }

        public int GetEstatusId(string estatusName)
        {
            int estatusId = 0; // Variable para almacenar el id encontrado

            try
            {
                using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT idEstatusInfraccion FROM catEstatusInfraccion WHERE estatusInfraccion = @estatusName", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@estatusName", estatusName);

                        object result = command.ExecuteScalar(); 

                        if (result != null)
                        {
                            estatusId = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Manejar la excepción o guardar en log
            }

            return estatusId;
        }
    }
}
