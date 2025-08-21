using DocumentFormat.OpenXml.Bibliography;
using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Exceptions;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GuanajuatoAdminUsuarios.Services
{
    public class OficialesService : IOficiales
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public OficialesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        /// <summary>
        /// Consulta de Categorias con SQLClient
        /// </summary>
        /// <returns></returns>
        public List<CatOficialesModel> GetOficiales()
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"Select *, cd.nombreOficina,e.estatusDesc from catOficiales co
                                                            LEFT JOIN catDelegacionesOficinasTransporte cd ON cd.idOficinaTransporte = co.idOficina
                                                            LEFT JOIN estatus e ON e.estatus = co.estatus", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatOficialesModel oficial = new CatOficialesModel();
                            oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
                            oficial.Rango = reader["Rango"].ToString();
                            oficial.Nombre = reader["Nombre"].ToString();
                            oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
                            oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();
                            oficial.nombreOficina = reader["nombreOficina"].ToString();
                            oficial.estatusDesc = reader["estatusDesc"].ToString();
                            object idOficinaValue = reader["idOficina"];
                            oficial.IdOficina = Convert.IsDBNull(idOficinaValue) ? 0 : Convert.ToInt32(idOficinaValue);




                            oficiales.Add(oficial);

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
            return oficiales;


        }

        public List<CatOficialesModel> GetOficialesByCorporacion(int corporacion)
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"
Select *, cd.nombreOficina,e.estatusDesc from catOficiales co
                                                            LEFT JOIN catDelegacionesOficinasTransporte cd ON cd.idOficinaTransporte = co.idOficina
                                                            LEFT JOIN estatus e ON e.estatus = co.estatus
															where co.transito=@corp AND co.estatus = 1", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporacion ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatOficialesModel oficial = new CatOficialesModel();
                            oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
                            oficial.Rango = reader["Rango"].ToString();
                            oficial.Nombre = reader["Nombre"].ToString();
                            oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
                            oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();
                            oficial.nombreOficina = reader["nombreOficina"].ToString();
                            oficial.estatusDesc = reader["estatusDesc"].ToString();
                            object idOficinaValue = reader["idOficina"];
                            oficial.IdOficina = Convert.IsDBNull(idOficinaValue) ? 0 : Convert.ToInt32(idOficinaValue);




                            oficiales.Add(oficial);

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
            return oficiales;


        }


        public List<CatOficialesModel> GetCatalogoOficialesDependencia(int idDependencia)
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"Select *, cd.nombreOficina,e.estatusDesc from catOficiales co
                                                            LEFT JOIN catDelegacionesOficinasTransporte cd ON cd.idOficinaTransporte = co.idOficina
                                                            LEFT JOIN estatus e ON e.estatus = co.estatus
                                                            WHERE co.transito = @idDependencia", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatOficialesModel oficial = new CatOficialesModel();
                            oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
                            oficial.Rango = reader["Rango"].ToString();
                            oficial.Nombre = reader["Nombre"].ToString();
                            oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
                            oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();
                            oficial.nombreOficina = reader["nombreOficina"].ToString();
                            oficial.estatusDesc = reader["estatusDesc"].ToString();

                            object idOficinaValue = reader["idOficina"];
                            oficial.IdOficina = Convert.IsDBNull(idOficinaValue) ? 0 : Convert.ToInt32(idOficinaValue);




                            oficiales.Add(oficial);

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
            return oficiales;


        }
        public List<CatOficialesModel> GetOficialesActivos()
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT ofi.*, e.estatusDesc FROM catOficiales AS ofi INNER JOIN estatus AS e ON ofi.estatus = e.estatus WHERE ofi.estatus = 1 ORDER BY Nombre ASC;", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatOficialesModel oficial = new CatOficialesModel();
                            oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
                            oficial.Rango = reader["Rango"].ToString();
                            oficial.Nombre = reader["Nombre"].ToString();
                            oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
                            oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();
                            oficial.estatusDesc = reader["estatusDesc"].ToString();
                            //oficial.FechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            oficial.Estatus = Convert.ToInt32(reader["Estatus"].ToString());
                            oficial.transito = (Convert.ToBoolean(reader["transito"])) ? 1 : 0;

                            oficiales.Add(oficial);

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
            return oficiales;


        }

		public List<CatOficialesModel> GetOficialesTodos()
		{
			//
			List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"
                        SELECT ofi.*, e.estatusDesc, p.nombrePuesto
                        FROM catOficiales AS ofi 
                        INNER JOIN estatus AS e ON ofi.estatus = e.estatus 
                        LEFT JOIN catPuestos p ON ofi.idPuesto = p.idPuesto AND p.estatus = 1 
                        ORDER BY Nombre ASC;", connection);
					command.CommandType = CommandType.Text;
					//sqlData Reader sirve para la obtencion de datos 
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CatOficialesModel oficial = new CatOficialesModel();
							oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
							oficial.Rango = reader["Rango"].ToString();
							oficial.Nombre = reader["Nombre"].ToString();
							oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
							oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();
							oficial.estatusDesc = reader["estatusDesc"].ToString();
							//oficial.FechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
							oficial.Estatus = Convert.ToInt32(reader["Estatus"].ToString());
							oficial.transito = Convert.ToInt32(reader["transito"].GetType() == typeof(DBNull)?0: reader["transito"]);
                            oficial.UrlFirma = reader["urlFirma"] != DBNull.Value ? reader["urlFirma"].ToString() : null;
                            oficial.Puesto = reader["nombrePuesto"] != DBNull.Value ? reader["nombrePuesto"].ToString() : null;

							oficiales.Add(oficial);

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
			return oficiales;


		}
		public List<CatOficialesModel> GetOficialesActivosTodos(bool todos, int tipo)
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("usp_ObtieneOfialesActivos", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Todos", SqlDbType.Bit)).Value = todos;
                    command.Parameters.Add(new SqlParameter("@Tipo", SqlDbType.Int)).Value = tipo;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatOficialesModel oficial = new CatOficialesModel();
                            oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
                            oficial.Rango = reader["Rango"].ToString();
                            oficial.Nombre = reader["Nombre"].ToString();
                            oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
                            oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();
                            oficial.estatusDesc = reader["estatusDesc"].ToString();
                            //oficial.FechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            oficial.Estatus = Convert.ToInt32(reader["Estatus"].ToString());
                            oficial.transito = (Convert.ToBoolean(reader["transito"])) ? 1 : 0;

                            oficiales.Add(oficial);

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
            return oficiales;


        }

        public CatOficialesModel GetOficialById(int IdOficial)
        {
            CatOficialesModel oficial = new CatOficialesModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catOficiales where idOficial=@IdOficial", connection);
                    command.Parameters.Add(new SqlParameter("@IdOficial", SqlDbType.Int)).Value = IdOficial;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
                            oficial.Nombre = reader["Nombre"].ToString();
                            oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
                            oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();
                            oficial.IdOficina = Convert.ToInt32(reader["idOficina"]);
                            oficial.Estatus = Convert.ToInt32(reader["estatus"].ToString());
							oficial.UrlFirma = reader["urlFirma"] != DBNull.Value ? reader["urlFirma"].ToString() : null;
							oficial.FechaInicio = reader["fechaInicio"] != DBNull.Value ? Convert.ToDateTime(reader["fechaInicio"]) : null;
							oficial.FechaFin = reader["fechaFin"] != DBNull.Value ? Convert.ToDateTime(reader["fechaFin"]) : null;
                            if (!reader.IsDBNull(reader.GetOrdinal("idPuesto")))
                            {
                                oficial.IdPuesto = Convert.ToInt32(reader["idPuesto"]);
                            }
                            else
                            {
                                oficial.IdPuesto = 0; 
                            }
                            oficial.IdTurno = Convert.ToInt32(reader["idturno"].ToString());

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

            return oficial;
        }

        public int SaveOficial(CatOficialesModel oficial, int idDependencia)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    var sqlQuery = @"Insert into catOficiales (Nombre, estatus,ApellidoPaterno,ApellidoMaterno,idOficina,transito{0}) values
                                                        (@Nombre,@estatus,@ApellidoPaterno,@ApellidoMaterno,@idDependencia,@tran{1})";
                    var query = "";
                    if (oficial.IdTurno > 0)
                    {
                        query = string.Format(sqlQuery, ",idturno", ",@IdTurno");
                    }
                    else
                    {
                        query = string.Format(sqlQuery, "", "");
                    }


                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand(query, connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.VarChar)).Value = oficial.Nombre;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@ApellidoPaterno", SqlDbType.VarChar)).Value = oficial.ApellidoPaterno==null ? "": oficial.ApellidoPaterno;
                    sqlCommand.Parameters.Add(new SqlParameter("@ApellidoMaterno", SqlDbType.VarChar)).Value = oficial.ApellidoMaterno==null ? "": oficial.ApellidoMaterno;
                    sqlCommand.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = oficial.IdOficina == null ? 0 : oficial.IdOficina;
                    //sqlCommand.Parameters.Add(new SqlParameter("@tran", SqlDbType.Int)).Value = idDependencia == null ? 0 : idDependencia;
                    sqlCommand.Parameters.Add(new SqlParameter("@tran", SqlDbType.Int)).Value = idDependencia;
                    sqlCommand.Parameters.Add(new SqlParameter("@urlFirma", SqlDbType.NVarChar)).Value = oficial.UrlFirma ?? "";
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.Date)).Value = oficial.FechaInicio;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.Date)).Value = oficial.FechaFin;
                    var paramIdPuesto = new SqlParameter("@idPuesto", SqlDbType.Int) { Value = oficial.IdPuesto != 0 && oficial.IdPuesto != null ? (object)oficial.IdPuesto : DBNull.Value };
                    sqlCommand.Parameters.Add(paramIdPuesto);
                    if (oficial.IdTurno > 0)
                        sqlCommand.Parameters.Add(new SqlParameter("@IdTurno", SqlDbType.Int)).Value = oficial.IdTurno;

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
        public int UpdateOficial(CatOficialesModel oficial)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catOficiales set Nombre = @Nombre, ApellidoPaterno=@ApellidoPaterno, ApellidoMaterno=@ApellidoMaterno, estatus = @estatus, idOficina=@idDependencia, urlFirma=@urlFirma, fechaInicio= @fechaInicio, fechaFin=@fechaFin, idPuesto=@idPuesto where idOficial=@idOficial",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = oficial.IdOficial;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = oficial.Estatus;
                    sqlCommand.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.VarChar)).Value = oficial.Nombre;
                    sqlCommand.Parameters.Add(new SqlParameter("@ApellidoPaterno", SqlDbType.VarChar)).Value = oficial.ApellidoPaterno == null ? "" : oficial.ApellidoPaterno;
                    sqlCommand.Parameters.Add(new SqlParameter("@ApellidoMaterno", SqlDbType.VarChar)).Value = oficial.ApellidoMaterno == null ? "" : oficial.ApellidoMaterno;
                    sqlCommand.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = oficial.IdOficina == null ? 0 : oficial.IdOficina;
                    sqlCommand.Parameters.Add(new SqlParameter("@urlFirma", SqlDbType.NVarChar)).Value = oficial.UrlFirma ?? "";
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.Date)).Value = oficial.FechaInicio;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.Date)).Value = oficial.FechaFin;
                    var paramIdPuesto = new SqlParameter("@idPuesto", SqlDbType.Int) { Value = oficial.IdPuesto != 0 && oficial.IdPuesto != null ? (object)oficial.IdPuesto : DBNull.Value };
                    sqlCommand.Parameters.Add(paramIdPuesto);
                    if(oficial.IdTurno>0)
                    sqlCommand.Parameters.AddWithValue("@idturno", oficial.IdTurno);

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


        public int DeleteOficial(int IdOficial)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Delete from oficiales where idOficial=@idOficial", connection);
                    command.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = IdOficial;
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
        public List<CatOficialesModel> GetOficialesActivosFiltrados(int idOficina,int idDependencia)
        {
            var oficiales = GetOficialesFiltrados(idOficina, idDependencia, onlyActivos: true);
            return oficiales;
        }

		public List<CatOficialesModel> GetOficialesFiltrados(int idOficina, int idDependencia)
		{
			var oficiales = GetOficialesFiltrados(idOficina, idDependencia, onlyActivos: false);
			return oficiales;
		}


		private List<CatOficialesModel> GetOficialesFiltrados(int idOficina, int idDependencia, bool onlyActivos)
		{
			List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand($@"
                        SELECT ofi.*, e.estatusDesc 
                        FROM catOficiales AS ofi 
                        INNER JOIN estatus AS e ON ofi.estatus = e.estatus
                        WHERE {(onlyActivos ? "ofi.estatus = 1 AND" : "")} ofi.idOficina = @idOficina AND ofi.transito = @idDependencia
                        ORDER BY Nombre ASC;", 
                        connection);

					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CatOficialesModel oficial = new CatOficialesModel();
							oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
							oficial.Rango = reader["Rango"].ToString();
							oficial.Nombre = reader["Nombre"].ToString();
							oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
							oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();
							oficial.estatusDesc = reader["estatusDesc"].ToString();
							oficial.Estatus = Convert.ToInt32(reader["Estatus"].ToString());

                            oficiales.Add(oficial);
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
			return oficiales;


		}
		public List<CatOficialesModel> GetOficialesPorDependencia(int idDependencia)
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT ofi.*, e.estatusDesc, ISNULL(X.idOficinaTransporte,0) idOficinaTransporte, ISNULL(x.nombreOficina,'') nombreOficina
                                                            FROM catOficiales AS ofi 
                                                            INNER JOIN estatus AS e ON ofi.estatus = e.estatus
                                                            LEFT JOIN catDelegacionesOficinasTransporte X ON ofi.idOficina = x.idOficinaTransporte
                                                            WHERE ofi.estatus = 1 AND ofi.transito = @idDependencia
                                                            ORDER BY Nombre ASC;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatOficialesModel oficial = new CatOficialesModel();
                            oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
                            oficial.Rango = reader["Rango"].ToString();
                            oficial.Nombre = reader["Nombre"].ToString();
                            oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
                            oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();
                            oficial.estatusDesc = reader["estatusDesc"].ToString();
                            //oficial.FechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            oficial.Estatus = Convert.ToInt32(reader["Estatus"].ToString());
                            oficial.nombreOficina = reader["nombreOficina"].ToString(); 
                            oficial.IdOficina = Convert.ToInt32(reader["idOficinaTransporte"].ToString()); 

                            oficiales.Add(oficial);

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
            return oficiales;
        }

		public int GetTurnoOficial(int Oficial)
		{
			//
			int oficiales = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"select isnull(idturno,0) idturno from catOficiales where idOficial=@IdOficial", connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@IdOficial", SqlDbType.Int)).Value = (object)Oficial ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
                            oficiales = (int)reader["idturno"];

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
			return oficiales;
		}



		public List<CatOficialesModel> GetOficialesPorDependencia2(int idDependencia)
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT ofi.*, e.estatusDesc, ISNULL(X.idOficinaTransporte,0) idOficinaTransporte, ISNULL(x.nombreOficina,'') nombreOficina
                                                            FROM catOficiales AS ofi 
                                                            INNER JOIN estatus AS e ON ofi.estatus = e.estatus
                                                            LEFT JOIN catDelegacionesOficinasTransporte X ON ofi.idOficina = x.idOficinaTransporte
                                                            WHERE  ofi.transito = @idDependencia
                                                            ORDER BY idOficial desc;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatOficialesModel oficial = new CatOficialesModel();
                            oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
                            oficial.Rango = reader["Rango"].ToString();
                            oficial.Nombre = reader["Nombre"].ToString();
                            oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
                            oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();
                            oficial.estatusDesc = reader["estatusDesc"].ToString();
                            //oficial.FechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            oficial.Estatus = Convert.ToInt32(reader["Estatus"].ToString());
                            oficial.nombreOficina = reader["nombreOficina"].ToString();
                            oficial.IdOficina = Convert.ToInt32(reader["idOficinaTransporte"].ToString());

                            oficiales.Add(oficial);

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
            return oficiales;
        }


        public List<CatOficialesModel> GetOficialesPorDependencia()
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT ofi.*, e.estatusDesc 
                                                            FROM catOficiales AS ofi 
                                                            INNER JOIN estatus AS e ON ofi.estatus = e.estatus
                                                            WHERE ofi.estatus = 1 
                                                            ORDER BY Nombre ASC;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatOficialesModel oficial = new CatOficialesModel();
                            oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
                            oficial.Rango = reader["Rango"].ToString();
                            oficial.Nombre = reader["Nombre"].ToString();
                            oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
                            oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();
                            oficial.estatusDesc = reader["estatusDesc"].ToString();
                            //oficial.FechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            oficial.Estatus = Convert.ToInt32(reader["Estatus"].ToString());


                            oficiales.Add(oficial);

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
            return oficiales;


        }

		public void ValidateFirmaDeOficialOrThrow(int idOficial, DateTime fechaCapturaAccidente)
		{
			CatOficialesModel oficial = this.GetOficialById(idOficial);
            if (oficial == null || oficial.IdOficial == default)
            {
				throw new NotFoundException("No se encontró el oficial con el id proporcionado");
			}

            var nombreOficial = $"{oficial.Nombre} {oficial.ApellidoPaterno} {oficial.ApellidoMaterno}";
            if (string.IsNullOrEmpty(oficial.UrlFirma))
            {
				throw new FirmaException($"El oficial {nombreOficial} no tiene una firma registrada");
			}

            var fechaCapturaAccidenteDate = fechaCapturaAccidente.Date;
            if (oficial.FechaInicio.HasValue && fechaCapturaAccidenteDate < oficial.FechaInicio.Value.Date ||
                oficial.FechaFin.HasValue && fechaCapturaAccidenteDate > oficial.FechaFin.Value.Date)
            {
				throw new FirmaException($"La firma del oficial {nombreOficial} no es válida para la fecha {fechaCapturaAccidente.Date}");
			}

            return;
		}
	}
}

