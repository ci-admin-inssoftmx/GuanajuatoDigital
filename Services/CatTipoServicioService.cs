using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatTipoServicioService : ICatTipoServicio
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatTipoServicioService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatTipoServicioModel> ObtenerTiposActivos()
        {
            //
            List<CatTipoServicioModel> ListaTipos = new List<CatTipoServicioModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT ts.*, e.estatusDesc FROM catTipoServicio AS ts INNER JOIN estatus AS e ON ts.estatus = e.estatus
                                                         ORDER BY idCatTipoServicio ASC;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatTipoServicioModel tipo = new CatTipoServicioModel();
                            tipo.idCatTipoServicio = Convert.ToInt32(reader["idCatTipoServicio"].ToString());
                            tipo.tipoServicio = reader["tipoServicio"].ToString();
                            tipo.estatusDesc = reader["estatusDesc"].ToString();
                            tipo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            tipo.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            ListaTipos.Add(tipo);

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
            return ListaTipos;


        }

        public CatTipoServicioModel ObtenerTipoByID(int idTipoServicio)
        {
            CatTipoServicioModel tipo = new CatTipoServicioModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT ts.*, e.estatusdesc FROM catTipoServicio AS ts
                                                                LEFT JOIN estatus AS e ON ts.estatus = e.estatus
                                                                WHERE ts.idCatTipoServicio=@idTipoServicio", connection);
                    command.Parameters.Add(new SqlParameter("@idTipoServicio", SqlDbType.Int)).Value = idTipoServicio;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            tipo.idCatTipoServicio = Convert.ToInt32(reader["idCatTipoServicio"].ToString());
                            tipo.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            tipo.tipoServicio = reader["tipoServicio"].ToString();
                            tipo.estatusDesc = reader["estatus"].ToString();
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

            return tipo;
        }
        public int CrearTipo(CatTipoServicioModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catTipoServicio(tiposervicio,estatus,fechaActualizacion,actualizadoPor) values(@tipoServicio,@estatus,@fechaActualizacion,@actualizadoPor)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@tipoServicio", SqlDbType.VarChar)).Value = model.tipoServicio;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
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
        public int EditarTipo(CatTipoServicioModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catTipoServicio set tipoServicio=@tipoServicio, estatus = @estatus,fechaActualizacion = @fechaActualizacion, actualizadoPor =@actualizadoPor where idCatTipoServicio=@idCatTipoServicio",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idCatTipoServicio", SqlDbType.Int)).Value = model.idCatTipoServicio;
                    sqlCommand.Parameters.Add(new SqlParameter("@tipoServicio", SqlDbType.NVarChar)).Value = model.tipoServicio;
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


