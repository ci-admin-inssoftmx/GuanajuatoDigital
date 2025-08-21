using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GuanajuatoAdminUsuarios.Services
{
    public class DependenciasService : IDependencias
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public DependenciasService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        /// <summary>
        /// Consulta de Categorias con SQLClient
        /// </summary>
        /// <returns></returns>
        public List<DependenciasModel> GetDependencias(int corp)
        {
            //
            List<DependenciasModel> dependencias = new List<DependenciasModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"Select catDep.*, e.estatusDesc from catDependenciasEnvian catDep
                                                            LEFT JOIN estatus AS e ON e.estatus = catDep.estatus
                                                            where transito = @corp
                                                            ORDER BY nombreDependencia ASC", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            DependenciasModel dependencia = new DependenciasModel();
                            dependencia.IdDependencia = Convert.ToInt32(reader["idDependenciaEnvia"].ToString());
                            dependencia.NombreDependencia = reader["nombreDependencia"].ToString();
                            dependencia.estatusDesc = reader["estatusDesc"].ToString();

                            dependencias.Add(dependencia);

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
            return dependencias;


        }

        public DependenciasModel GetDependenciaById(int idDependencia)
        {
            DependenciasModel dependencia = new DependenciasModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catDependenciasEnvian where idDependenciaEnvia=@idDependencia", connection);
                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = idDependencia;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            dependencia.IdDependencia = Convert.ToInt32(reader["idDependenciaEnvia"].ToString());
                            dependencia.Estatus = Convert.ToInt32(reader["estatus"].ToString());

                            dependencia.NombreDependencia = reader["nombreDependencia"].ToString();
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

            return dependencia;
        }

        public int SaveDependencia(DependenciasModel dependencia,int corp)
        {
            int result = 0;
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand(@"Insert into catDependenciasEnvian(nombreDependencia,fechaActualizacion,actualizadoPor,estatus,transito) 
                                                    values(@NombreDependencia,@fechaActualizacion,@actualizadoPor,@estatus,@corp)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@NombreDependencia", SqlDbType.VarChar)).Value = dependencia.NombreDependencia;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    sqlCommand.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.AddWithValue("corp", corporation);
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
        public int UpdateDependencia(DependenciasModel dependencia)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catDependenciasEnvian set nombreDependencia=@NombreDependencia,estatus= @estatus where idDependenciaEnvia=@idDependencia",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = dependencia.IdDependencia;
                    sqlCommand.Parameters.Add(new SqlParameter("@NombreDependencia", SqlDbType.VarChar)).Value = dependencia.NombreDependencia;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = dependencia.Estatus;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    sqlCommand.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
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


        public int DeleteDependencia(int idDependencia)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("update dependencias set estatus = 0 where idDependencia=@idDependencia", connection);
                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = idDependencia;
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


