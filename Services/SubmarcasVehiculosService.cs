using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GuanajuatoAdminUsuarios.Services
{
    public class SubmarcasVehiculosService : ISubmarcasVehiculos
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public SubmarcasVehiculosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        /// <summary>
        /// Consulta de Categorias con SQLClient
        /// </summary>
        /// <returns></returns>
        public List<SubmarcasVehiculo> GetSubmarcas()
        {
            //
            List<SubmarcasVehiculo> submarcas = new List<SubmarcasVehiculo>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from submarcasVehiculos", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            SubmarcasVehiculo submarca = new SubmarcasVehiculo();
                            submarca.IdSubmarca = Convert.ToInt32(reader["IdSubmarca"].ToString());
                            submarca.NombreSubmarca = reader["NombreSubmarca"].ToString();
                            submarcas.Add(submarca);

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
            return submarcas;


        }

        public SubmarcasVehiculo GetSubmarcaById(int IdSubarca)
        {
            SubmarcasVehiculo submarca = new SubmarcasVehiculo();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from submarcasVehiculos where idSubmarca=@idSubmarca", connection);
                    command.Parameters.Add(new SqlParameter("@idSubmarca", SqlDbType.Int)).Value = IdSubarca;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            submarca.IdSubmarca = Convert.ToInt32(reader["IdSubmarca"].ToString());
                            submarca.NombreSubmarca = reader["NombreSubmarca"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    connection.Close();
                }

            return submarca;
        }

        public int SaveSubmarca(SubmarcasVehiculo submarca)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into submarcasVehiculos(NombreSubmarca) values(@NombreSubmarca)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@NombreSubmarca", SqlDbType.VarChar)).Value = submarca.NombreSubmarca;
                    sqlCommand.CommandType = CommandType.Text;
                    result = sqlCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return result;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;

        }
        public int UpdateSubmarca(SubmarcasVehiculo submarca)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update submarcasVehiculos set NombreSubmarca=@NombreSubmarca where idSubmarca=@idSubmarca",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idSubmarca", SqlDbType.Int)).Value = submarca.IdSubmarca;
                    sqlCommand.Parameters.Add(new SqlParameter("@NombreSubmarca", SqlDbType.VarChar)).Value = submarca.NombreSubmarca;
                    sqlCommand.CommandType = CommandType.Text;
                    result = sqlCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    //---Log
                    return result;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }


        public int DeleteSubmarca(int idSubmarca)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Delete from submarcasVehiculos where idSubmarca=@idSubmarca", connection);
                    command.Parameters.Add(new SqlParameter("@idSubmarca", SqlDbType.Int)).Value = idSubmarca;
                    command.CommandType = CommandType.Text;
                    result = command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    return result;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }


    }
}



