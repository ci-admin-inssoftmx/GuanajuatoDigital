using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatEntidadesService : ICatEntidadesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatEntidadesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatEntidadesModel> ObtenerEntidades(int corp)
        {
            //
            List<CatEntidadesModel> ListaEntidades = new List<CatEntidadesModel>();
			var corporation = corp < 2 ? 1 : corp;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT ent.*, e.estatusDesc 
                                                            FROM catEntidades AS ent 
                                                            LEFT JOIN estatus AS e ON ent.estatus = e.estatus 
                                                            where transito = @corp
                                                            ORDER BY nombreEntidad ASC;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatEntidadesModel entidad = new CatEntidadesModel();
                            entidad.idEntidad = reader["idEntidad"] != DBNull.Value ? Convert.ToInt32(reader["idEntidad"]) : 0;
                            entidad.nombreEntidad = reader["nombreEntidad"] != DBNull.Value ? reader["nombreEntidad"].ToString().ToUpper() : string.Empty;
                            entidad.estatusDesc = reader["estatusDesc"] != DBNull.Value ? reader["estatusDesc"].ToString() : string.Empty;

                            entidad.fechaActualizacion = reader["fechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(reader["fechaActualizacion"]) : (DateTime?)null;
                            entidad.estatus = reader["estatus"] != DBNull.Value ? Convert.ToInt32(reader["estatus"]) : 0;
                            entidad.actualizadoPor = reader["actualizadoPor"] != DBNull.Value ? Convert.ToInt32(reader["actualizadoPor"]) : 0;

                            ListaEntidades.Add(entidad);

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
            return ListaEntidades;


        }

		public List<CatEntidadesModel> ObtenerEntidadesActivas(int corp)
		{
			//
			List<CatEntidadesModel> ListaEntidades = new List<CatEntidadesModel>();
			var corporation = corp < 2 ? 1 : corp;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"SELECT ent.*, e.estatusDesc 
                                                            FROM catEntidades AS ent 
                                                            LEFT JOIN estatus AS e ON ent.estatus = e.estatus 
                                                            where transito = @corp and ent.estatus=1
                                                            ORDER BY nombreEntidad ASC;", connection);
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("corp", corporation);

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CatEntidadesModel entidad = new CatEntidadesModel();
							entidad.idEntidad = reader["idEntidad"] != DBNull.Value ? Convert.ToInt32(reader["idEntidad"]) : 0;
							entidad.nombreEntidad = reader["nombreEntidad"] != DBNull.Value ? reader["nombreEntidad"].ToString() : string.Empty;
							entidad.estatusDesc = reader["estatusDesc"] != DBNull.Value ? reader["estatusDesc"].ToString() : string.Empty;

							entidad.fechaActualizacion = reader["fechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(reader["fechaActualizacion"]) : (DateTime?)null;
							entidad.estatus = reader["estatus"] != DBNull.Value ? Convert.ToInt32(reader["estatus"]) : 0;
							entidad.actualizadoPor = reader["actualizadoPor"] != DBNull.Value ? Convert.ToInt32(reader["actualizadoPor"]) : 0;

							ListaEntidades.Add(entidad);

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
			return ListaEntidades;


		}

		public CatEntidadesModel ObtenerEntidadesByID(int idEntidad)
        {
            CatEntidadesModel entidad = new CatEntidadesModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catEntidades where idEntidad=@idEntidad", connection);
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = idEntidad;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            entidad.idEntidad = Convert.ToInt32(reader["idEntidad"].ToString());
                            entidad.nombreEntidad = reader["nombreEntidad"].ToString();
                            entidad.estatus = Convert.ToInt32(reader["estatus"].ToString());

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

            return entidad;
        }

        public CatEntidadesModel ObtenerEntidadesByNombre(string nombre,int corp)
        {
            //SE compara menor que 4 para que funcione bien para usuarios tipo pension
			var corporation = corp < 4 ? 1 : corp;

			CatEntidadesModel entidad = new CatEntidadesModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"Select * from catEntidades where transito = @corp and   lower(nombreEntidad) LIKE '%' + @entidad + '%'", connection);
                    command.Parameters.AddWithValue("@entidad", nombre.ToLower()); 
                    command.Parameters.AddWithValue("@corp", corporation); 
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            entidad.idEntidad = Convert.ToInt32(reader["idEntidad"].ToString());
                            entidad.nombreEntidad = reader["nombreEntidad"].ToString();
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

            return entidad;
        }

        public int CrearEntidad(CatEntidadesModel model,int corp)
        {
			var corporation = corp < 2 ? 1 : corp;

			int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catEntidades(nombreEntidad,estatus,fechaActualizacion,actualizadoPor,transito) values(@nombreEntidad,@estatus,@fechaActualizacion,@actualizadoPor,@corp)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@nombreEntidad", SqlDbType.VarChar)).Value = model.nombreEntidad;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    sqlCommand.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
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
        public int EditarEntidad(CatEntidadesModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catEntidades set nombreEntidad=@nombreEntidad, estatus = @estatus,fechaActualizacion = @fechaActualizacion, actualizadoPor =@actualizadoPor where idEntidad=@idEntidad",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = model.idEntidad;
                    sqlCommand.Parameters.Add(new SqlParameter("@nombreEntidad", SqlDbType.NVarChar)).Value = model.nombreEntidad;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.VarChar)).Value = model.estatus;
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
        public int obtenerIdPorEntidad(string entidad, int corp)
        {
			var corporation = corp < 2 ? 1 : corp;

			int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("SELECT idEntidad FROM catEntidades WHERE transito = @corp and nombreEntidad = @entidad", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@entidad", SqlDbType.NVarChar)).Value = entidad;
                    sqlCommand.Parameters.AddWithValue("corp", corporation);


                    sqlCommand.CommandType = CommandType.Text;

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read()) // Intenta leer un registro del resultado
                        {
                            // Obtiene el valor de la columna "idMunicipio"
                            result = Convert.ToInt32(reader["idEntidad"]);
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
    }
}

