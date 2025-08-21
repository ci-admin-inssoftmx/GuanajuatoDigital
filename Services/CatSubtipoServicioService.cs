using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatSubtipoServicioService : ICatSubtipoServicio
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatSubtipoServicioService (ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<CatSubtipoServicioModel> GetSubtipoPorTipo(int tipoServicioDDlValue)
        {
            //
            List<CatSubtipoServicioModel> ListaSubtipos = new List<CatSubtipoServicioModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT csubt.*, cts.*, e.estatus FROM catSubtipoServicio AS csubt " +
                        "LEFT JOIN catTipoServicio AS cts ON csubt.idTipoServicio = cts.idCatTipoServicio " +
                        "LEFT JOIN estatus AS e ON csubt.estatus = e.estatus WHERE csubt.idTipoServicio = @idTipoServicio;\r\n", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idTipoServicio", SqlDbType.Int)).Value = (object)tipoServicioDDlValue ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatSubtipoServicioModel subtipo = new CatSubtipoServicioModel();
                            subtipo.idSubTipoServicio = Convert.ToInt32(reader["idSubTipoServicio"].ToString());
                            subtipo.idTipoServicio = Convert.ToInt32(reader["idTipoServicio"].ToString());
                            subtipo.subTipoServicio = reader["servicio"].ToString().ToUpper();
                            subtipo.estatusDesc = reader["estatus"].ToString();
                           // subtipo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            subtipo.estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            // subtipo.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
                            ListaSubtipos.Add(subtipo);

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
            return ListaSubtipos;


        }

        public List<CatSubtipoServicioModel> ObtenerSubtiposActivos()
        {
            //
            List<CatSubtipoServicioModel> ListaSubtipos = new List<CatSubtipoServicioModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT csubt.*,cts.*, e.estatusDesc FROM catSubtipoServicio AS csubt
                                                        LEFT JOIN catTipoServicio AS cts ON csubt.idTipoServicio = cts.idCatTipoServicio
                                                        INNER JOIN estatus AS e ON csubt.estatus = e.estatus
                                                         ORDER BY idCatTipoServicio ASC;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatSubtipoServicioModel subtipo = new CatSubtipoServicioModel();
                            subtipo.idSubTipoServicio = Convert.ToInt32(reader["idSubTipoServicio"].ToString());
                            subtipo.idTipoServicio = Convert.ToInt32(reader["idTipoServicio"].ToString());
                            subtipo.subTipoServicio = reader["servicio"].ToString();
                            subtipo.tipoServicio = reader["tipoServicio"].ToString();
                            subtipo.estatusDesc = reader["estatusDesc"].ToString();
                            // subtipo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            subtipo.estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            // subtipo.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
                            ListaSubtipos.Add(subtipo);

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
            return ListaSubtipos;


        }

        public CatSubtipoServicioModel ObtenerSubtipoByID(int idSubtipoServicio)
        {
            CatSubtipoServicioModel subtipo = new CatSubtipoServicioModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT  csubt.*,cts.*, e.estatusdesc FROM catSubtipoServicio AS csubt
                                                        LEFT JOIN catTipoServicio AS cts ON csubt.idTipoServicio = cts.idCatTipoServicio
                                                                LEFT JOIN estatus AS e ON csubt.estatus = e.estatus
                                                                WHERE csubt.idSubtipoServicio=@idSubtipoServicio", connection);
                    command.Parameters.Add(new SqlParameter("@idSubtipoServicio", SqlDbType.Int)).Value = idSubtipoServicio;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            subtipo.idSubTipoServicio = Convert.ToInt32(reader["idSubTipoServicio"].ToString());
                            subtipo.idTipoServicio = Convert.ToInt32(reader["idTipoServicio"].ToString());
                            subtipo.subTipoServicio = reader["servicio"].ToString();
                            subtipo.tipoServicio = reader["tipoServicio"].ToString();
                            subtipo.estatusDesc = reader["estatusdesc"].ToString();
                            // subtipo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            subtipo.estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            // subtipo.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
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

            return subtipo;
        }
        public int CrearSubtipo(CatSubtipoServicioModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catSubtipoServicio(servicio,idTiposervicio,estatus) values(@servicio,@idTiposervicio,@estatus)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@servicio", SqlDbType.VarChar)).Value = model.subTipoServicio;
                    sqlCommand.Parameters.Add(new SqlParameter("@idTiposervicio", SqlDbType.Int)).Value = model.idTipoServicio;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                   // sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                   // sqlCommand.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;

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
        public int EditarSubtipo(CatSubtipoServicioModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catSubtipoServicio set servicio=@servicio,idTipoServicio = @idTipoServicio, estatus = @estatus where idSubtipoServicio=@idSubtipoServicio",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idSubtipoServicio", SqlDbType.Int)).Value = model.idSubTipoServicio;
                    sqlCommand.Parameters.Add(new SqlParameter("@idTipoServicio", SqlDbType.Int)).Value = model.idTipoServicio;
                    sqlCommand.Parameters.Add(new SqlParameter("@servicio", SqlDbType.NVarChar)).Value = model.subTipoServicio;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.VarChar)).Value = model.estatus;
                    //sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    //sqlCommand.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
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

