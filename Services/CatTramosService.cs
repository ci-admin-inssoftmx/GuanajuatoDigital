using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatTramosService : ICatTramosService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatTramosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatTramosModel> ObtenerTamosPorCarretera(int carreteraDDValue, int corp)
        {
            //
            List<CatTramosModel> ListaTramos = new List<CatTramosModel>();
            var corporation = corp < 2 ? 1 : corp;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT t.*, c.*, e.estatus FROM catTramos AS t INNER JOIN catCarreteras AS c ON t.idCarretera = c.idCarretera" +
                        " INNER JOIN estatus AS e ON t.estatus = e.estatus WHERE t.estatus=1 and ((t.idCarretera = @IdCarretera and t.transito = @corp ) or t.idCarretera in (1,2)) ORDER BY Tramo ASC;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@IdCarretera", SqlDbType.Int)).Value = (object)carreteraDDValue ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporation ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatTramosModel tramo = new CatTramosModel();
                            tramo.IdTramo = Convert.ToInt32(reader["IdTramo"].ToString());
                            tramo.IdCarretera = Convert.ToInt32(reader["IdCarretera"].ToString());
                            tramo.Tramo = reader["Tramo"].ToString();
                            tramo.estatusDesc = reader["estatus"].ToString();
                            tramo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            tramo.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            tramo.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaTramos.Add(tramo);

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
            return ListaTramos;


        }
        public List<CatTramosModel> ObtenerTamosPorCarreteraActivos(int carreteraDDValue, int corp)
        {
            //
            List<CatTramosModel> ListaTramos = new List<CatTramosModel>();
            var corporation = corp < 2 ? 1 : corp;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT t.*, c.*, e.estatus 
                                                        FROM catTramos AS t 
                                                        INNER JOIN catCarreteras AS c ON t.idCarretera = c.idCarretera
                                                        INNER JOIN estatus AS e ON t.estatus = e.estatus 
                                                        WHERE t.estatus = 1 AND ((t.idCarretera = @IdCarretera and t.transito = @corp) or t.idCarretera in (1,2)) ORDER BY Tramo ASC;", connection);                 
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@IdCarretera", SqlDbType.Int)).Value = (object)carreteraDDValue ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporation ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatTramosModel tramo = new CatTramosModel();
                            tramo.IdTramo = Convert.ToInt32(reader["IdTramo"].ToString());
                            tramo.IdCarretera = Convert.ToInt32(reader["IdCarretera"].ToString());
                            tramo.Tramo = reader["Tramo"].ToString();
                            tramo.estatusDesc = reader["estatus"].ToString();
                            tramo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            tramo.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            tramo.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaTramos.Add(tramo);

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
            return ListaTramos;


        }

        public List<CatTramosModel> ObtenerTramos(int corp)
        {
            //
            List<CatTramosModel> ListaTramos = new List<CatTramosModel>();
            var corporation = corp < 2 ? 1 : corp;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT t.*, e.estatusDesc,car.carretera 
                                                                FROM catTramos AS t 
                                                                LEFT JOIN estatus AS e ON t.estatus = e.estatus 
                                                                INNER JOIN catCarreteras AS car ON t.idCarretera = car.idCarretera
                                                                where t.transito = @corp order by t.idtramo desc;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatTramosModel tramo = new CatTramosModel();
                            tramo.IdTramo = Convert.ToInt32(reader["IdTramo"].ToString());
                            tramo.IdCarretera = Convert.ToInt32(reader["IdCarretera"].ToString());
                            tramo.Tramo = reader["Tramo"].ToString();
                            tramo.Carretera = reader["carretera"].ToString();
                            tramo.estatusDesc = reader["estatusDesc"].ToString();
                            tramo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            tramo.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            tramo.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaTramos.Add(tramo);

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
            return ListaTramos;


        }
        public CatTramosModel ObtenerTramoByID(int IdTramo)
        {
            CatTramosModel tramo = new CatTramosModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT t.*, e.estatusdesc, car.carretera,car.idOficinaTransporte 
                                                          FROM catTramos AS t 
                                                            INNER JOIN estatus AS e ON t.estatus = e.estatus 
                                                            INNER JOIN catCarreteras AS car ON t.idCarretera = car.idCarretera WHERE t.idTramo=@IdTramo", connection);
                    command.Parameters.Add(new SqlParameter("@IdTramo", SqlDbType.Int)).Value = IdTramo;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            tramo.IdDelegacion = Convert.ToInt32(reader["idOficinaTransporte"].ToString());
                            tramo.IdCarretera = Convert.ToInt32(reader["IdCarretera"].ToString());
                            tramo.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            tramo.IdTramo = Convert.ToInt32(reader["idTramo"].ToString());
                            tramo.Carretera = reader["carretera"].ToString();
                            tramo.Tramo = reader["tramo"].ToString();

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

            return tramo;
        }
        public int CrearTramo(CatTramosModel model,int corp)
        {
            int result = 0;
            var corporation = corp < 2 ? 1 : corp;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catTramos(tramo,estatus,fechaActualizacion,actualizadoPor,idCarretera,transito) values(@tramo,@estatus,@fechaActualizacion,@actualizadoPor,@idCarretera,@corp)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@tramo", SqlDbType.VarChar)).Value = model.Tramo;
                    sqlCommand.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = model.IdCarretera;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = corporation;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    sqlCommand.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;

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
        public int EditarTramo(CatTramosModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catTramos set tramo=@Tramo, estatus = @estatus,fechaActualizacion = @fechaActualizacion, actualizadoPor =@actualizadoPor, idCarretera =@idCarretera where idtramo=@idTramo",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = model.IdCarretera;
                    sqlCommand.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = model.IdTramo;
                    sqlCommand.Parameters.Add(new SqlParameter("@Tramo", SqlDbType.NVarChar)).Value = model.Tramo;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.VarChar)).Value = model.Estatus;
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

    }
}



