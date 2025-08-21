using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
//using Telerik.SvgIcons;
using DocumentFormat.OpenXml.Bibliography;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatMunicipiosService : ICatMunicipiosService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatMunicipiosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }



        public List<CatMunicipiosModel> GetMunicipios2(int corp)
        {
            //
            List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"
                        SELECT m.*, e.estatusdesc, del.nombreOficina, ent.nombreEntidad 
                        FROM catMunicipios AS m 
                        INNER JOIN estatus AS e ON m.estatus = e.estatus
                        INNER JOIN catDelegacionesOficinasTransporte AS del ON m.idOficinaTransporte = del.idOficinaTransporte
                        INNER JOIN catEntidades AS ent ON m.idEntidad = ent.idEntidad and m.estatus=1
                        WHERE m.estatus=1 and m.transito = @corp", connection);
                    
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMunicipiosModel municipio = new CatMunicipiosModel();
                            municipio.IdMunicipio = reader["IdMunicipio"] is DBNull ? 0 : Convert.ToInt32(reader["IdMunicipio"]);
                            municipio.IdOficinaTransporte = reader["IdOficinaTransporte"] is DBNull ? 0 : Convert.ToInt32(reader["IdOficinaTransporte"]);
                            municipio.IdEntidad = reader["IdEntidad"] is DBNull ? 0 : Convert.ToInt32(reader["IdEntidad"]);
                            municipio.Municipio = reader["Municipio"] is DBNull ? string.Empty : reader["Municipio"].ToString();
                            municipio.nombreOficina = reader["nombreOficina"] is DBNull ? string.Empty : reader["nombreOficina"].ToString();
                            municipio.nombreEntidad = reader["nombreEntidad"] is DBNull ? string.Empty : reader["nombreEntidad"].ToString();
                            municipio.estatusDesc = reader["estatusDesc"] is DBNull ? string.Empty : reader["estatusDesc"].ToString();
                            municipio.FechaActualizacion = reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(reader["FechaActualizacion"]);
                            municipio.Estatus = reader["estatus"] is DBNull ? 0 : Convert.ToInt32(reader["estatus"]);
                            municipio.ActualizadoPor = reader["ActualizadoPor"] is DBNull ? 0 : Convert.ToInt32(reader["ActualizadoPor"]);

                            ListaMunicipios.Add(municipio);

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
            return ListaMunicipios;


        }


        public List<CatMunicipiosModel> GetMunicipios3(int corp)
        {
            //
            List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"
                        SELECT m.*, e.estatusdesc, del.nombreOficina, ent.nombreEntidad 
                        FROM catMunicipios AS m 
                        INNER JOIN estatus AS e ON m.estatus = e.estatus
                        INNER JOIN catDelegacionesOficinasTransporte AS del ON m.idOficinaTransporte = del.idOficinaTransporte
                        INNER JOIN catEntidades AS ent ON m.idEntidad = ent.idEntidad 
                        WHERE m.transito = @corp", connection);
                    
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMunicipiosModel municipio = new CatMunicipiosModel();
                            municipio.IdMunicipio = reader["IdMunicipio"] is DBNull ? 0 : Convert.ToInt32(reader["IdMunicipio"]);
                            municipio.IdOficinaTransporte = reader["IdOficinaTransporte"] is DBNull ? 0 : Convert.ToInt32(reader["IdOficinaTransporte"]);
                            municipio.IdEntidad = reader["IdEntidad"] is DBNull ? 0 : Convert.ToInt32(reader["IdEntidad"]);
                            municipio.Municipio = reader["Municipio"] is DBNull ? string.Empty : reader["Municipio"].ToString();
                            municipio.nombreOficina = reader["nombreOficina"] is DBNull ? string.Empty : reader["nombreOficina"].ToString();
                            municipio.nombreEntidad = reader["nombreEntidad"] is DBNull ? string.Empty : reader["nombreEntidad"].ToString();
                            municipio.estatusDesc = reader["estatusDesc"] is DBNull ? string.Empty : reader["estatusDesc"].ToString();
                            municipio.FechaActualizacion = reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(reader["FechaActualizacion"]);
                            municipio.Estatus = reader["estatus"] is DBNull ? 0 : Convert.ToInt32(reader["estatus"]);
                            municipio.ActualizadoPor = reader["ActualizadoPor"] is DBNull ? 0 : Convert.ToInt32(reader["ActualizadoPor"]);

                            ListaMunicipios.Add(municipio);

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
            return ListaMunicipios;


        }



        public List<CatMunicipiosModel> GetMunicipios(int corp)
        {
            //
            List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"
                        SELECT m.*, e.estatusdesc, del.nombreOficina, ent.nombreEntidad 
                        FROM catMunicipios AS m
                        INNER JOIN estatus AS e ON m.estatus = e.estatus
                        INNER JOIN catDelegacionesOficinasTransporte AS del ON m.idOficinaTransporte = del.idOficinaTransporte
                        INNER JOIN catEntidades AS ent ON m.idEntidad = ent.idEntidad  and m.estatus=1
                        WHERE m.transito = @corp", connection);

                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMunicipiosModel municipio = new CatMunicipiosModel();
                            municipio.IdMunicipio = reader["IdMunicipio"] is DBNull ? 0 : Convert.ToInt32(reader["IdMunicipio"]);
                            municipio.IdOficinaTransporte = reader["IdOficinaTransporte"] is DBNull ? 0 : Convert.ToInt32(reader["IdOficinaTransporte"]);
                            municipio.IdEntidad = reader["IdEntidad"] is DBNull ? 0 : Convert.ToInt32(reader["IdEntidad"]);
                            municipio.Municipio = reader["Municipio"] is DBNull ? string.Empty : reader["Municipio"].ToString();
                            municipio.nombreOficina = reader["nombreOficina"] is DBNull ? string.Empty : reader["nombreOficina"].ToString();
                            municipio.nombreEntidad = reader["nombreEntidad"] is DBNull ? string.Empty : reader["nombreEntidad"].ToString();
                            municipio.estatusDesc = reader["estatusDesc"] is DBNull ? string.Empty : reader["estatusDesc"].ToString();
                            municipio.FechaActualizacion = reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(reader["FechaActualizacion"]);
                            municipio.Estatus = reader["estatus"] is DBNull ? 0 : Convert.ToInt32(reader["estatus"]);
                            municipio.ActualizadoPor = reader["ActualizadoPor"] is DBNull ? 0 : Convert.ToInt32(reader["ActualizadoPor"]);

                            ListaMunicipios.Add(municipio);

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
            return ListaMunicipios;


        }
        public CatMunicipiosModel GetMunicipioByID(int IdMunicipio)
        {
            CatMunicipiosModel municipio = new CatMunicipiosModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT m.*, e.estatusdesc, del.nombreOficina, ent.nombreEntidad FROM catMunicipios AS m INNER JOIN estatus AS e ON m.estatus = e.estatus INNER JOIN catDelegacionesOficinasTransporte AS del ON m.idOficinaTransporte = del.idOficinaTransporte INNER JOIN catEntidades AS ent ON m.idEntidad = ent.idEntidad WHERE m.idMunicipio=@IdMunicipio", connection);
                    command.Parameters.Add(new SqlParameter("@IdMunicipio", SqlDbType.Int)).Value = IdMunicipio;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            municipio.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            municipio.IdEntidad = Convert.ToInt32(reader["IdEntidad"].ToString());
                            municipio.IdOficinaTransporte = Convert.ToInt32(reader["IdOficinaTransporte"].ToString());
                            municipio.nombreOficina = reader["nombreOficina"].ToString();
                            municipio.nombreEntidad = reader["nombreEntidad"].ToString();
                            municipio.Municipio = reader["Municipio"].ToString();
                            municipio.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            municipio.abreviatura = reader["abreviatura"] is DBNull ? "" : reader["abreviatura"].ToString();

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

            return municipio;
        }
        public int AgregarMunicipio(CatMunicipiosModel model,int corp)
        {
			var corporation = corp < 2 ? 1 : corp;
			int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catMunicipios(municipio,estatus,idOficinaTransporte,idEntidad,fechaActualizacion,actualizadoPor,abreviatura,transito) values(@Municipio,@Estatus,@idOficinaTransporte,@idEntidad,@FechaActualizacion,@ActualizadoPor,@abreviatura,@corp)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@Municipio", SqlDbType.VarChar)).Value = model.Municipio;
                    sqlCommand.Parameters.Add(new SqlParameter("@Estatus", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficinaTransporte", SqlDbType.Int)).Value =model.IdOficinaTransporte;
                    sqlCommand.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = model.IdEntidad;
                    sqlCommand.Parameters.Add(new SqlParameter("@FechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActualizadoPor", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = corporation;
                    sqlCommand.Parameters.Add(new SqlParameter("@abreviatura", SqlDbType.VarChar)).Value = model.abreviatura??"";

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
        public int EditarMunicipio(CatMunicipiosModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catMunicipios set municipio=@Municipio, idOficinaTransporte =@idOficinaTransporte,idEntidad = @idEntidad,estatus = @Estatus,fechaActualizacion = @FechaActualizacion, actualizadoPor =@ActualizadoPor, abreviatura=@abreviatura where idMunicipio=@IdMunicipio",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@IdMunicipio", SqlDbType.Int)).Value = model.IdMunicipio;
                    sqlCommand.Parameters.Add(new SqlParameter("@Municipio", SqlDbType.NVarChar)).Value = model.Municipio;
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficinaTransporte", SqlDbType.Int)).Value = model.IdOficinaTransporte;
                    sqlCommand.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = model.IdEntidad;
                    sqlCommand.Parameters.Add(new SqlParameter("@Estatus", SqlDbType.VarChar)).Value = model.Estatus;
                    sqlCommand.Parameters.Add(new SqlParameter("@FechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActualizadoPor", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@abreviatura", SqlDbType.VarChar)).Value = model.abreviatura??"";
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
        public List<CatMunicipiosModel> GetMunicipiosPorEntidad(int entidadDDlValue,int corp)
        {
            //
            List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT m.*, ent.*, e.estatus 
                                                        FROM catMunicipios AS m 
                                                    LEFT JOIN catEntidades AS ent ON m.idEntidad = ent.idEntidad 
                                                    LEFT JOIN estatus AS e ON m.estatus = e.estatus
                                                    WHERE m.idEntidad = @idEntidad and m.transito=@corp", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)entidadDDlValue ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporation ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMunicipiosModel municipio = new CatMunicipiosModel();
                            municipio.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            municipio.IdEntidad = Convert.ToInt32(reader["IdEntidad"].ToString());
                            municipio.Municipio = reader["Municipio"].ToString().ToUpper();
                            municipio.estatusDesc = reader["estatus"].ToString();
                            municipio.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            municipio.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            municipio.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
                            ListaMunicipios.Add(municipio);

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
            return ListaMunicipios;


        }

        public List<CatMunicipiosModel> GetMunicipiosActivePorEntidad(int entidadDDlValue,int corp)
        {
            //
            List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT m.*, ent.*, e.estatus 
                                                        FROM catMunicipios AS m 
                                                        LEFT JOIN catEntidades AS ent ON m.idEntidad = ent.idEntidad 
                                                        LEFT JOIN estatus AS e ON m.estatus = e.estatus 
                                                        WHERE m.estatus=1 AND m.idEntidad = @idEntidad and m.transito = @corp;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)entidadDDlValue ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporation ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMunicipiosModel municipio = new CatMunicipiosModel();
                            municipio.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            municipio.IdEntidad = Convert.ToInt32(reader["IdEntidad"].ToString());
                            municipio.Municipio = reader["Municipio"].ToString();
                            municipio.estatusDesc = reader["estatus"].ToString();
                            municipio.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            municipio.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            municipio.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
                            ListaMunicipios.Add(municipio);

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
            return ListaMunicipios;


        }
        public int obtenerIdPorNombre(string municipio,int corp)
        { 
            int result = 0;
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("SELECT idMunicipio FROM catMunicipios WHERE municipio = @municipio and transito = @corp", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@municipio", SqlDbType.NVarChar)).Value = municipio;
                    sqlCommand.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = corporation;
                    sqlCommand.CommandType = CommandType.Text;

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read()) // Intenta leer un registro del resultado
                        {
                            // Obtiene el valor de la columna "idMunicipio"
                            result = Convert.ToInt32(reader["idMunicipio"]);
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

        public CatMunicipiosModel ObtenerMunicipiosByNombre(string nombre, int corp)
        {
            //SE compara menor que 4 para que funcione bien para usuarios tipo pension

            var corporation = corp < 4 ? 1 : corp;

            CatMunicipiosModel municipio = new CatMunicipiosModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT *
                                                            FROM catMunicipios
                                                            WHERE transito = @corp
                                                              AND LOWER(municipio) COLLATE Latin1_General_CI_AI LIKE '%' + LOWER(@municipio) COLLATE Latin1_General_CI_AI + '%'"
                                                            , connection);
                    command.Parameters.AddWithValue("@municipio", nombre.ToLower());
                    command.Parameters.AddWithValue("@corp", corporation);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            municipio.IdMunicipio = Convert.ToInt32(reader["idMunicipio"].ToString());
                            municipio.nombreEntidad = reader["municipio"].ToString();
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

            return municipio;
        }

        public List<CatMunicipiosModel> GetMunicipiosPorDelegacion2(int idOficina,int corp)
		{
			//
			List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"SELECT m.*,e.estatus 
                                FROM catMunicipios AS m 
                        LEFT JOIN estatus AS e ON m.estatus = e.estatus 
                                WHERE m.idOficinaTransporte = @idOficina and m.estatus=1 and transito = @corp ;", connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporation ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CatMunicipiosModel municipio = new CatMunicipiosModel();
							municipio.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
							municipio.IdEntidad = Convert.ToInt32(reader["IdEntidad"].ToString());
							municipio.Municipio = reader["Municipio"].ToString();
							municipio.estatusDesc = reader["estatus"].ToString();
							municipio.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
							municipio.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
							municipio.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
							ListaMunicipios.Add(municipio);

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
			return ListaMunicipios;
		}
        public List<CatMunicipiosModel> GetMunicipiosPorDelegacion2(int corp)
        {
            //
            List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT m.*,e.estatus 
                                                    FROM catMunicipios AS m 
                                                        LEFT JOIN estatus AS e ON m.estatus = e.estatus
														join catEntidades en on en.identidad=m.identidad
                                                        WHERE  m.estatus=1 and nombreEntidad like 'GUANAJUATO%' and m.idOficinaTransporte = @corp;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMunicipiosModel municipio = new CatMunicipiosModel();
                            municipio.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            municipio.IdEntidad = Convert.ToInt32(reader["IdEntidad"].ToString());
                            municipio.Municipio = reader["Municipio"].ToString().ToUpper();
                            municipio.estatusDesc = reader["estatus"].ToString();
                            municipio.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            municipio.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            municipio.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
                            ListaMunicipios.Add(municipio);

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
            return ListaMunicipios;
        }



        public List<CatMunicipiosModel> GetMunicipiosPorDelegacion(int idOficina,int corp)
        {
            //
            List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"  SELECT m.*,e.estatus 
                                                            FROM catMunicipios AS m 
                                                            LEFT JOIN estatus AS e ON m.estatus = e.estatus 
                                                            WHERE m.idOficinaTransporte = @idOficina and transito = @corp ;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporation ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMunicipiosModel municipio = new CatMunicipiosModel();
                            municipio.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            municipio.IdEntidad = Convert.ToInt32(reader["IdEntidad"].ToString());
                            municipio.Municipio = reader["Municipio"].ToString();
                            municipio.estatusDesc = reader["estatus"].ToString();
                            municipio.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            municipio.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            municipio.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
                            ListaMunicipios.Add(municipio);

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
            return ListaMunicipios;


        }

        public List<CatMunicipiosModel> GetMunicipiosPorDelegacionActivos(int idOficina,int corp)
        {
            //
            List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"  SELECT m.*,e.estatus 
                                                            FROM catMunicipios AS m 
                                                            LEFT JOIN estatus AS e ON m.estatus = e.estatus 
                                                            WHERE m.estatus = 1 AND m.idOficinaTransporte = @idOficina and m.transito = @corp ;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporation ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMunicipiosModel municipio = new CatMunicipiosModel();
                            municipio.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            municipio.IdEntidad = Convert.ToInt32(reader["IdEntidad"].ToString());
                            municipio.Municipio = reader["Municipio"].ToString();
                            municipio.estatusDesc = reader["estatus"].ToString();
                            municipio.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            municipio.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            municipio.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
                            ListaMunicipios.Add(municipio);

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
            return ListaMunicipios;


        }


        public List<CatMunicipiosModel> GetMunicipiosPorDelegacionTodos(int idOficina,int corp)
        {
            //
            List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT m.*,e.estatus 
                                                    FROM catMunicipios AS m LEFT JOIN estatus AS e ON m.estatus = e.estatus 
                                                    WHERE m.idOficinaTransporte = @idOficina and transito = @corp ;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporation ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMunicipiosModel municipio = new CatMunicipiosModel();
                            municipio.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            municipio.IdEntidad = Convert.ToInt32(reader["IdEntidad"].ToString());
                            municipio.Municipio = reader["Municipio"].ToString();
                            municipio.estatusDesc = reader["estatus"].ToString();
                            municipio.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            municipio.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            municipio.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
                            ListaMunicipios.Add(municipio);

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
            return ListaMunicipios;


        }

        public List<CatMunicipiosModel> GetMunicipiosGuanajuato(int corp)
        {
            //
            List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT cm.idMunicipio,cm.municipio,e.estatusDesc
                                                            FROM catMunicipios cm
                                                            LEFT JOIN estatus e ON e.estatus =cm.estatus
                                                            left join catEntidades en on en.identidad = cm.identidad
															WHERE en.nombreEntidad like 'Guanajuato%'  and cm.transito = @corp", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMunicipiosModel municipio = new CatMunicipiosModel();
                            municipio.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            municipio.Municipio = reader["Municipio"].ToString();
                          
                            ListaMunicipios.Add(municipio);

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
            return ListaMunicipios;


        }

        public List<CatMunicipiosModel> GetMunicipiosGuanajuatoActivos(int corp)
        {
            //
            List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();
            var corporation = corp < 2 ? 1 : corp;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT cm.idMunicipio,cm.municipio,e.estatusDesc
                                                            FROM catMunicipios cm
                                                            LEFT JOIN estatus e ON e.estatus =cm.estatus
                                                            left join catEntidades en on en.identidad = cm.identidad
															WHERE en.nombreEntidad like 'Guanajuato%'  and cm.transito = @corp and cm.estatus = 1", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMunicipiosModel municipio = new CatMunicipiosModel();
                            municipio.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            municipio.Municipio = reader["Municipio"].ToString();

                            ListaMunicipios.Add(municipio);

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
            return ListaMunicipios;


        }

        public List<CatMunicipiosModel> GetMunicipiosCatalogo(int corp)
        {
            //
            List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"
                        SELECT m.*, e.estatusdesc, del.nombreOficina, ent.nombreEntidad 
                        FROM catMunicipios AS m 
                        INNER JOIN estatus AS e ON m.estatus = e.estatus
                        INNER JOIN catDelegacionesOficinasTransporte AS del ON m.idOficinaTransporte = del.idOficinaTransporte
                        INNER JOIN catEntidades AS ent ON m.idEntidad = ent.idEntidad
                        WHERE m.transito = @corp", connection);
                    
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMunicipiosModel municipio = new CatMunicipiosModel();
                            municipio.IdMunicipio = reader["IdMunicipio"] is DBNull ? 0 : Convert.ToInt32(reader["IdMunicipio"]);
                            municipio.IdOficinaTransporte = reader["IdOficinaTransporte"] is DBNull ? 0 : Convert.ToInt32(reader["IdOficinaTransporte"]);
                            municipio.IdEntidad = reader["IdEntidad"] is DBNull ? 0 : Convert.ToInt32(reader["IdEntidad"]);
                            municipio.Municipio = reader["Municipio"] is DBNull ? string.Empty : reader["Municipio"].ToString();
                            municipio.nombreOficina = reader["nombreOficina"] is DBNull ? string.Empty : reader["nombreOficina"].ToString();
                            municipio.nombreEntidad = reader["nombreEntidad"] is DBNull ? string.Empty : reader["nombreEntidad"].ToString();
                            municipio.estatusDesc = reader["estatusDesc"] is DBNull ? string.Empty : reader["estatusDesc"].ToString();
                            municipio.FechaActualizacion = reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(reader["FechaActualizacion"]);
                            municipio.Estatus = reader["estatus"] is DBNull ? 0 : Convert.ToInt32(reader["estatus"]);
                            municipio.ActualizadoPor = reader["ActualizadoPor"] is DBNull ? 0 : Convert.ToInt32(reader["ActualizadoPor"]);
                            municipio.abreviatura = reader["abreviatura"] is DBNull ? "" : reader["abreviatura"].ToString();
                            ListaMunicipios.Add(municipio);

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
            return ListaMunicipios;


        }

        public List<CatMunicipiosModel> GetMunicipiosCatalogoFiltro(string nombre, int idEntidad,int idDelegacion,int corp)
        {
            //
            List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    string sqlCondiciones = "";
                    sqlCondiciones += nombre == null ? "" : " m.municipio LIKE '%' + @nombre + '%' AND \n";
                    sqlCondiciones += idEntidad == 0 ? "" : " m.idEntidad=@idEntidad AND \n";
                    sqlCondiciones += idDelegacion == 0 ? "" : " m.idOficinaTransporte=@idDelegacion AND \n";

                    string SqlTransact =
                            string.Format("SELECT m.*, e.estatusdesc, del.nombreOficina, ent.nombreEntidad FROM catMunicipios AS m INNER JOIN estatus AS e ON m.estatus = e.estatus" +
                        " INNER JOIN catDelegacionesOficinasTransporte AS del ON m.idOficinaTransporte = del.idOficinaTransporte" +
                        " INNER JOIN catEntidades AS ent ON m.idEntidad = ent.idEntidad" +
                        " WHERE {0} idMunicipio!=0 and m.transito = @corp;", sqlCondiciones);



                    connection.Open();

                    SqlCommand command = new SqlCommand(SqlTransact, connection);

                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMunicipiosModel municipio = new CatMunicipiosModel();
                            municipio.IdMunicipio = reader["IdMunicipio"] is DBNull ? 0 : Convert.ToInt32(reader["IdMunicipio"]);
                            municipio.IdOficinaTransporte = reader["IdOficinaTransporte"] is DBNull ? 0 : Convert.ToInt32(reader["IdOficinaTransporte"]);
                            municipio.IdEntidad = reader["IdEntidad"] is DBNull ? 0 : Convert.ToInt32(reader["IdEntidad"]);
                            municipio.Municipio = reader["Municipio"] is DBNull ? string.Empty : reader["Municipio"].ToString();
                            municipio.nombreOficina = reader["nombreOficina"] is DBNull ? string.Empty : reader["nombreOficina"].ToString();
                            municipio.nombreEntidad = reader["nombreEntidad"] is DBNull ? string.Empty : reader["nombreEntidad"].ToString();
                            municipio.estatusDesc = reader["estatusDesc"] is DBNull ? string.Empty : reader["estatusDesc"].ToString();
                            municipio.FechaActualizacion = reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(reader["FechaActualizacion"]);
                            municipio.Estatus = reader["estatus"] is DBNull ? 0 : Convert.ToInt32(reader["estatus"]);
                            municipio.ActualizadoPor = reader["ActualizadoPor"] is DBNull ? 0 : Convert.ToInt32(reader["ActualizadoPor"]);
                            municipio.abreviatura = reader["abreviatura"] is DBNull ? "" : reader["abreviatura"].ToString();
                            ListaMunicipios.Add(municipio);

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
            return ListaMunicipios;


        }
		public List<CatMunicipiosModel> GetMunicipiosActivePorCorporacion(int corp)
		{
			//
			List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"SELECT m.*, e.estatus 
                                                        FROM catMunicipios AS m 
                                                        LEFT JOIN estatus AS e ON m.estatus = e.estatus 
                                                        WHERE m.estatus=1 and m.transito = @corp;", connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporation ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CatMunicipiosModel municipio = new CatMunicipiosModel();
							municipio.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
							municipio.IdEntidad = Convert.ToInt32(reader["IdEntidad"].ToString());
							municipio.Municipio = reader["Municipio"].ToString();
							municipio.estatusDesc = reader["estatus"].ToString();
							municipio.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
							municipio.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
							municipio.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
							ListaMunicipios.Add(municipio);

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
			return ListaMunicipios;


		}
	}
}



