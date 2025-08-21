using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatDelegacionesOficinasTransporteService : ICatDelegacionesOficinasTransporteService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatDelegacionesOficinasTransporteService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinas()
        {
            //
            List<CatDelegacionesOficinasTransporteModel> ListaDelegacionsOficinas = new List<CatDelegacionesOficinasTransporteModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT del.*, e.estatusDesc,m.municipio, ISNULL(d.Transito,0) Transito 
                                                          FROM catDelegacionesOficinasTransporte AS del 
                                                          INNER JOIN catDelegaciones d ON del.idOficinaTransporte = d.idDelegacion
                                                          INNER JOIN estatus AS e ON del.estatus = e.estatus 
                                                          INNER JOIN catMunicipios AS m ON del.idMunicipio = m.idMunicipio;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatDelegacionesOficinasTransporteModel delegacionOficina = new CatDelegacionesOficinasTransporteModel();
                            delegacionOficina.IdOficinaTransporte = Convert.ToInt32(reader["IdOficinaTransporte"].ToString());
                            delegacionOficina.NombreOficina = reader["NombreOficina"].ToString();
                            delegacionOficina.JefeOficina = reader["JefeOficina"].ToString();
                            delegacionOficina.Municipio = reader["Municipio"].ToString();
                            delegacionOficina.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            delegacionOficina.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            delegacionOficina.estatusDesc = reader["estatusDesc"].ToString();
                            delegacionOficina.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            delegacionOficina.Transito = Convert.ToBoolean(reader["Transito"]) ? 1 : 0;
                            //delegacionOficina.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaDelegacionsOficinas.Add(delegacionOficina);

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
            return ListaDelegacionsOficinas;


        }
        public List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinasActivos()
        {
            //
            List<CatDelegacionesOficinasTransporteModel> ListaDelegacionsOficinas = new List<CatDelegacionesOficinasTransporteModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"  SELECT del.*, e.estatusDesc, ISNULL(d.transito,0) transito 
                                                            FROM catDelegacionesOficinasTransporte AS del 
                                                            INNER JOIN estatus AS e ON del.estatus = e.estatus 
                                                            INNER JOIN catDelegaciones d ON del.idOficinaTransporte = d.idDelegacion 
                                                            WHERE del.estatus = 1  
                                                            ORDER BY nombreOficina ASC;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatDelegacionesOficinasTransporteModel delegacionOficina = new CatDelegacionesOficinasTransporteModel();
                            delegacionOficina.IdDelegacion = Convert.ToInt32(reader["idOficinaTransporte"].ToString());
                            delegacionOficina.Delegacion = reader["nombreOficina"].ToString().ToUpper();
                            delegacionOficina.FechaActualizacion = (reader["FechaActualizacion"] as DateTime?) ?? DateTime.MinValue;
                            delegacionOficina.estatusDesc = reader["estatusDesc"].ToString();
                            delegacionOficina.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            delegacionOficina.Transito = Convert.ToInt32(reader["transito"]) ;
                            //delegacionOficina.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaDelegacionsOficinas.Add(delegacionOficina);

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
            return ListaDelegacionsOficinas;


        }

        public string GetDelegacionOficinaById(int idOficinaTransporte)
        {
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"
                            SELECT TOP 1 nombreOficina 
                            FROM catDelegacionesOficinasTransporte 
                            WHERE idOficinaTransporte = @IdOficina
                            ORDER BY nombreOficina ASC;", connection);
                    command.Parameters.AddWithValue("@IdOficina", idOficinaTransporte);
                    command.CommandType = CommandType.Text;

                    var nombreOficina = command.ExecuteScalar()?.ToString();

                    return nombreOficina;
                }
                catch (SqlException ex)
                {
                    // Manejar la excepción, por ejemplo, escribir en un registro de errores
                    // ex
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
       
        public int EditarDelegacion(CatDelegacionesOficinasTransporteModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catDelegacionesOficinasTransporte set jefeOficina=@jefeOficina WHERE idOficinaTransporte = @idOficinaTransporte",connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficinaTransporte", SqlDbType.Int)).Value = model.IdOficinaTransporte;
                    sqlCommand.Parameters.Add(new SqlParameter("@jefeOficina", SqlDbType.NVarChar)).Value = model.JefeOficina;        
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

        public List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinasByDependencia(int idDependencia)
        {
            //
            List<CatDelegacionesOficinasTransporteModel> ListaDelegacionsOficinas = new List<CatDelegacionesOficinasTransporteModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT del.*, e.estatusDesc,m.municipio, ISNULL(d.Transito,0) Transito 
                                                          FROM catDelegacionesOficinasTransporte AS del 
                                                          INNER JOIN catDelegaciones d ON del.iddelegacion = d.idDelegacion
                                                          INNER JOIN estatus AS e ON del.estatus = e.estatus 
                                                          INNER JOIN catMunicipios AS m ON del.idMunicipio = m.idMunicipio
                                                            WHERE d.transito = @idDependencia;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatDelegacionesOficinasTransporteModel delegacionOficina = new CatDelegacionesOficinasTransporteModel();
                            delegacionOficina.IdOficinaTransporte = Convert.ToInt32(reader["IdOficinaTransporte"].ToString());
                            delegacionOficina.IdDelegacion = Convert.ToInt32(reader["IdOficinaTransporte"].ToString());

                            delegacionOficina.NombreOficina = reader["NombreOficina"].ToString();
                            delegacionOficina.Delegacion = reader["NombreOficina"].ToString();
                            delegacionOficina.JefeOficina = reader["JefeOficina"].ToString();
                            delegacionOficina.Municipio = reader["Municipio"].ToString();
                            delegacionOficina.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            delegacionOficina.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            delegacionOficina.estatusDesc = reader["estatusDesc"].ToString();
                            delegacionOficina.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            delegacionOficina.Transito = Convert.ToBoolean(reader["Transito"]) ? 1 : 0;
                            //delegacionOficina.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaDelegacionsOficinas.Add(delegacionOficina);

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
            return ListaDelegacionsOficinas;


        }

        public List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinasFiltrado(int idDependencia)
        {
            //
            List<CatDelegacionesOficinasTransporteModel> ListaDelegacionsOficinas = new List<CatDelegacionesOficinasTransporteModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT del.*, e.estatusDesc,m.municipio, ISNULL(d.Transito,0) Transito 
                                                          FROM catDelegacionesOficinasTransporte AS del 
                                                          INNER JOIN catDelegaciones d ON del.idOficinaTransporte = d.idDelegacion
                                                          INNER JOIN estatus AS e ON del.estatus = e.estatus 
                                                          INNER JOIN catMunicipios AS m ON del.idMunicipio = m.idMunicipio
                                                            WHERE d.transito = @idDependencia;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatDelegacionesOficinasTransporteModel delegacionOficina = new CatDelegacionesOficinasTransporteModel();
                            delegacionOficina.IdOficinaTransporte = Convert.ToInt32(reader["IdOficinaTransporte"].ToString());
                            delegacionOficina.IdDelegacion = Convert.ToInt32(reader["IdOficinaTransporte"].ToString());

                            delegacionOficina.NombreOficina = reader["NombreOficina"].ToString().ToUpper();
                            delegacionOficina.Delegacion = reader["NombreOficina"].ToString().ToUpper();
                            delegacionOficina.JefeOficina = reader["JefeOficina"].ToString();
                            delegacionOficina.Municipio = reader["Municipio"].ToString();
                            delegacionOficina.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            delegacionOficina.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            delegacionOficina.estatusDesc = reader["estatusDesc"].ToString();
                            delegacionOficina.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            delegacionOficina.Transito = Convert.ToBoolean(reader["Transito"]) ? 1 : 0;
                            //delegacionOficina.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaDelegacionsOficinas.Add(delegacionOficina);

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
            return ListaDelegacionsOficinas;


        }


        public List<CatDelegacionesOficinasTransporteModel> GetDelegacionesFiltrado(int idDependencia)
        {
            //
            List<CatDelegacionesOficinasTransporteModel> ListaDelegacionsOficinas = new List<CatDelegacionesOficinasTransporteModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT idDelegacion,delegacion 
                                                          FROM catDelegaciones AS d                                                           
                                                            WHERE d.transito = @idDependencia;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatDelegacionesOficinasTransporteModel delegacionOficina = new CatDelegacionesOficinasTransporteModel();
                            delegacionOficina.IdOficinaTransporte = Convert.ToInt32(reader["idDelegacion"].ToString());
                            delegacionOficina.NombreOficina = reader["delegacion"].ToString();

                            ListaDelegacionsOficinas.Add(delegacionOficina);

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
            return ListaDelegacionsOficinas;


        }




        public List<CatDelegacionesOficinasTransporteModel> GetDelegacionesDropDown()
        {
            //
            List<CatDelegacionesOficinasTransporteModel> ListaDelegacionsOficinas = new List<CatDelegacionesOficinasTransporteModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT del.*, e.estatusDesc, ISNULL(del.Transito,0) Transito 
                                                          FROM catDelegaciones AS del 
                                                          INNER JOIN estatus AS e ON del.estatus = e.estatus;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatDelegacionesOficinasTransporteModel delegacionOficina = new CatDelegacionesOficinasTransporteModel();
                            delegacionOficina.IdOficinaTransporte = Convert.ToInt32(reader["idDelegacion"].ToString());
                            delegacionOficina.NombreOficina = reader["delegacion"].ToString();                          
                            delegacionOficina.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            delegacionOficina.estatusDesc = reader["estatusDesc"].ToString();
                            delegacionOficina.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            delegacionOficina.Transito = Convert.ToInt32(reader["Transito"]) ;
                            //delegacionOficina.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaDelegacionsOficinas.Add(delegacionOficina);

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
            return ListaDelegacionsOficinas;


        }

        public List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinasTransporteFirmasByDelegacionId(int delegacionId)
        {
            var response = new List<CatDelegacionesOficinasTransporteModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT cdot.*, cp.nombrePuesto
                                                            FROM catDelegacionesOficinasTransporte AS cdot
                                                            INNER JOIN catPuestos AS cp ON cdot.idPuesto = cp.idPuesto
                                                            WHERE cdot.idDelegacion = @idDelegacion;", connection);
                    command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = delegacionId;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var delegacionOficina = new CatDelegacionesOficinasTransporteModel();
                            delegacionOficina.IdOficinaTransporte = Convert.ToInt32(reader["IdOficinaTransporte"].ToString());
                            delegacionOficina.NombreOficina = reader["nombreOficina"].ToString();
                            delegacionOficina.JefeOficina = reader["jefeOficina"].ToString();
                            delegacionOficina.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            delegacionOficina.FechaInicio = Convert.ToDateTime(reader["fechaInicio"].ToString());
                            delegacionOficina.FechaFin = Convert.ToDateTime(reader["fechaFin"].ToString());
                            delegacionOficina.UrlFirma = reader["urlFirma"].ToString();
                            delegacionOficina.NombrePuesto = reader["nombrePuesto"].ToString();
                            response.Add(delegacionOficina);
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

            return response;
        }

        public int InsertDelegacionesOficinasTransporteFirmas(CatDelegacionesOficinasTransporteModel model)
        {
            int result = 0;
            using (var connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    var sqlQuery = @"INSERT INTO catDelegacionesOficinasTransporte (nombreOficina, jefeOficina, idMunicipio, fechaActualizacion, estatus, fechaInicio, fechaFin, urlFirma, idDelegacion, idPuesto)
                                    VALUES(@nombreOficina, @jefeOficina, @idMunicipio, @fechaActualizacion, @estatus, @fechaInicio, @fechaFin, @urlFirma, @idDelegacion, @idPuesto);
                                    SELECT SCOPE_IDENTITY();";
                    connection.Open();
                    var sqlCommand = new
                        SqlCommand(sqlQuery, connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@nombreOficina", SqlDbType.NVarChar)).Value = model.NombreOficina;
                    sqlCommand.Parameters.Add(new SqlParameter("@jefeOficina", SqlDbType.NVarChar)).Value = model.JefeOficina;
                    sqlCommand.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = model.IdMunicipio;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.UtcNow;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = model.Estatus;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.DateTime)).Value = model.FechaInicio;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.DateTime)).Value = model.FechaFin;
                    sqlCommand.Parameters.Add(new SqlParameter("@urlFirma", SqlDbType.NVarChar)).Value = model.UrlFirma;
                    sqlCommand.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = model.IdDelegacion;
                    sqlCommand.Parameters.Add(new SqlParameter("@idPuesto", SqlDbType.Int)).Value = model.IdPuesto;
                    sqlCommand.CommandType = CommandType.Text;
                    result = Convert.ToInt32(sqlCommand.ExecuteScalar());
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
            }

            return result;
        }

        public int GetDelegacionesOficinasTransporteFirmasRangoFecha(CatDelegacionesOficinasTransporteModel model)
        {
            int result = 0;
            using (var connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    var sqlQuery = @"SELECT TOP 1 idOficinaTransporte FROM catDelegacionesOficinasTransporte
                                    WHERE idDelegacion = @idDelegacion
                                    AND idPuesto = @idPuesto
                                    AND fechaFin >= @fechaInicio
                                    AND estatus = 1;";
                    connection.Open();
                    var sqlCommand = new
                        SqlCommand(sqlQuery, connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.DateTime)).Value = model.FechaInicio;
                    sqlCommand.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = model.IdDelegacion;
                    sqlCommand.Parameters.Add(new SqlParameter("@idPuesto", SqlDbType.Int)).Value = model.IdPuesto;
                    sqlCommand.CommandType = CommandType.Text;
                    result = Convert.ToInt32(sqlCommand.ExecuteScalar());
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
            }

            return result;
        }

        public int UpdateDelegacionesOficinasTransporteFirmasFechaFin(int idOficinaTransporte, DateTime fechaFin)
        {
            int result = 0;
            using (var connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    var sqlQuery = @"UPDATE catDelegacionesOficinasTransporte
                                    SET fechaFin = @fechaFin, fechaActualizacion = @fechaActualizacion
                                    WHERE idOficinaTransporte = @idOficinaTransporte;";
                    connection.Open();
                    var sqlCommand = new
                        SqlCommand(sqlQuery, connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.DateTime)).Value = fechaFin;
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficinaTransporte", SqlDbType.Int)).Value = idOficinaTransporte;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.UtcNow;
                    sqlCommand.CommandType = CommandType.Text;
                    result = sqlCommand.ExecuteNonQuery();
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
            }

            return result;
        }

		public CatDelegacionesOficinasTransporteModel GetDelegacionOficinaTransporteFirmasById(int idOficinaTransporte)
		{
			var response = new CatDelegacionesOficinasTransporteModel();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"SELECT cdot.*
                                                        FROM catDelegacionesOficinasTransporte AS cdot
                                                        WHERE cdot.idOficinaTransporte = @idOficinaTransporte;", connection);
					command.Parameters.Add(new SqlParameter("@idOficinaTransporte", SqlDbType.Int)).Value = idOficinaTransporte;
					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							response.IdOficinaTransporte = Convert.ToInt32(reader["IdOficinaTransporte"].ToString());
							response.NombreOficina = reader["nombreOficina"].ToString();
							response.JefeOficina = reader["jefeOficina"].ToString();
							response.Estatus = Convert.ToInt32(reader["estatus"].ToString());
							response.FechaInicio = Convert.ToDateTime(reader["fechaInicio"].ToString());
							response.FechaFin = Convert.ToDateTime(reader["fechaFin"].ToString());
							response.UrlFirma = reader["urlFirma"].ToString();
							response.IdDelegacion = Convert.ToInt32(reader["idDelegacion"].ToString());
							response.IdPuesto = Convert.ToInt32(reader["idPuesto"].ToString());
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

			return response;
		}

        public int UpdateDelegacionesOficinasTransporteFirmas(CatDelegacionesOficinasTransporteModel model)
        {
            int result = 0;
            using (var connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    var sqlQuery = @"UPDATE catDelegacionesOficinasTransporte
                                    SET jefeOficina = @jefeOficina, fechaActualizacion = @fechaActualizacion, estatus = @estatus, fechaInicio = @fechaInicio, fechaFin = @fechaFin, urlFirma = @urlFirma, idPuesto = @idPuesto
                                    WHERE idOficinaTransporte = @idOficinaTransporte;";
                    connection.Open();
                    var sqlCommand = new
                        SqlCommand(sqlQuery, connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@jefeOficina", SqlDbType.NVarChar)).Value = model.JefeOficina;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.UtcNow;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = model.Estatus;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.DateTime)).Value = model.FechaInicio;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.DateTime)).Value = model.FechaFin;
                    sqlCommand.Parameters.Add(new SqlParameter("@urlFirma", SqlDbType.NVarChar)).Value = model.UrlFirma;
                    sqlCommand.Parameters.Add(new SqlParameter("@idPuesto", SqlDbType.Int)).Value = model.IdPuesto;
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficinaTransporte", SqlDbType.Int)).Value = model.IdOficinaTransporte;
                    sqlCommand.CommandType = CommandType.Text;
                    result = sqlCommand.ExecuteNonQuery();
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
            }

            return result;
        }


        public bool ExistCorp(int corpId)
        {
            var result = false;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT count(*) result
                                                          FROM CatCorporaciones
                                                          WHERE IdCorporacion =@IdCorp;", connection);
                    command.Parameters.Add(new SqlParameter("@IdCorp", SqlDbType.Int)).Value = corpId;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            result = ((int)reader["result"])>0;
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

            return result;
        }
        public bool ExistDelegacion(int idDelegacion)
        {
            var result = false;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT count(*) result
                                                          FROM catDelegaciones
                                                          WHERE idDelegacion =@IdDelegacion;", connection);
                    command.Parameters.Add(new SqlParameter("@IdDelegacion", SqlDbType.Int)).Value = idDelegacion;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            result = ((int)reader["result"]) > 0;
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
            return result;
        }


        public bool ExistDelegacionMun(int idDelegacion)
        {
            var result = false;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT count(*) result
                                                          FROM Delegaciones
                                                          WHERE iddelegacion =@IdDelegacion;", connection);
                    command.Parameters.Add(new SqlParameter("@IdDelegacion", SqlDbType.Int)).Value = idDelegacion;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            result = ((int)reader["result"]) > 0;
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
            return result;
        }
        public int NewDelegacion(int delegacion, string descripcion, int corp)
        {
            var result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"insert into catDelegaciones values
                                                          (@Desc,getdate(),1,1,@Corp,@idOfi); 
                                                          select SCOPE_IDENTITY() as  result", connection);

                    command.Parameters.Add(new SqlParameter("@Desc", SqlDbType.VarChar)).Value = descripcion;
                    command.Parameters.Add(new SqlParameter("@Corp", SqlDbType.Int)).Value = corp;
                    command.Parameters.Add(new SqlParameter("@idOfi", SqlDbType.Int)).Value = delegacion;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            result = Convert.ToInt32(reader["result"]);
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
            return result;
        }



        public int NewDelegacionMun(int delegacion, string descripcion, int corp,int muni)
        {
            var result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"insert into Delegaciones values
                                                          (@idOfi,@Desc,getdate(),1,1,@muni,@Corp); 
                                                          ", connection);

                    command.Parameters.Add(new SqlParameter("@Desc", SqlDbType.VarChar)).Value = descripcion;
                    command.Parameters.Add(new SqlParameter("@Corp", SqlDbType.Int)).Value = corp;
                    command.Parameters.Add(new SqlParameter("@idOfi", SqlDbType.Int)).Value = delegacion;
                    command.Parameters.Add(new SqlParameter("@muni", SqlDbType.Int)).Value = muni;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
 
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
            return result;
        }


        public int NewMunicipio( string descripcion, int corp,int idOfi)
        {
            var result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"insert into catMunicipios values
                                                         (@Desc,getdate(),1,1,@idOfi,11,'M'+UPPER(LEFT(@desc,2)),@Corp);
                                                          select SCOPE_IDENTITY() as result", connection);

                    command.Parameters.Add(new SqlParameter("@Desc", SqlDbType.VarChar)).Value = descripcion;
                    command.Parameters.Add(new SqlParameter("@Corp", SqlDbType.Int)).Value = corp;
                    command.Parameters.Add(new SqlParameter("@idOfi", SqlDbType.Int)).Value = idOfi;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            result = Convert.ToInt32(reader["result"]);
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
            return result;
        }



        public int NewCorp(int corp, string descripcion)
        {
            var result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"insert into CatCorporaciones values
                                                          (@Corp,@Desc,1)
                                                          select SCOPE_IDENTITY() as result", connection);

                    command.Parameters.Add(new SqlParameter("@Desc", SqlDbType.VarChar)).Value = descripcion;
                    command.Parameters.Add(new SqlParameter("@Corp", SqlDbType.Int)).Value = corp;

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            result = Convert.ToInt32(reader["result"]);
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

            return result;
        }


    }
}
