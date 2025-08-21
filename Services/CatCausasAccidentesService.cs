using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatCausasAccidentesService : ICatCausasAccidentesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatCausasAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatCausasAccidentesModel> ObtenerCausasActivas(int corp)
        {
            //
            List<CatCausasAccidentesModel> ListaCausas = new List<CatCausasAccidentesModel>();
            var corporation = corp < 2 ? 1 : corp;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT c.*, e.estatusDesc 
                                                          FROM catCausasAccidentes AS c 
                                                          INNER JOIN estatus AS e ON c.estatus = e.estatus
                                                            where transito = @corp
                                                         ORDER BY CausaAccidente ASC;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatCausasAccidentesModel causa = new CatCausasAccidentesModel();
                            causa.IdCausaAccidente = Convert.ToInt32(reader["IdCausaAccidente"].ToString());
                            causa.CausaAccidente = reader["CausaAccidente"].ToString();
                            causa.estatusDesc = reader["estatusDesc"].ToString();
                            causa.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            causa.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            //carretera.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaCausas.Add(causa);

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
            return ListaCausas;


        }

        public CatCausasAccidentesModel ObtenerCausaByID(int IdCausaAccidente)
        {
            CatCausasAccidentesModel causa = new CatCausasAccidentesModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT c.*, e.estatusdesc FROM catCausasAccidentes AS c 
                                                                LEFT JOIN estatus AS e ON c.estatus = e.estatus
                                                                WHERE c.IdCausaAccidente=@IdCausaAccidente", connection);
                    command.Parameters.Add(new SqlParameter("@IdCausaAccidente", SqlDbType.Int)).Value = IdCausaAccidente;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            causa.IdCausaAccidente = Convert.ToInt32(reader["IdCausaAccidente"].ToString());
                            causa.Estatus = Convert.ToInt32(reader["estatus"].ToString());

                            causa.CausaAccidente = reader["CausaAccidente"].ToString();
                            causa.estatusDesc = reader["estatus"].ToString();
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

            return causa;
        }
        public int CrearCausa(CatCausasAccidentesModel model,int corp)
        {
            int result = 0;
            var corporation = corp < 2 ? 1 : corp;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catCausasAccidentes(CausaAccidente,estatus,fechaActualizacion,actualizadoPor,transito) values(@CausaAccidente,@estatus,@fechaActualizacion,@actualizadoPor,@corp)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@CausaAccidente", SqlDbType.VarChar)).Value = model.CausaAccidente;
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
        public int EditarCausa(CatCausasAccidentesModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catCausasAccidentes set CausaAccidente=@CausaAccidente, estatus = @estatus,fechaActualizacion = @fechaActualizacion, actualizadoPor =@actualizadoPor where IdCausaAccidente=@IdCausaAccidente",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@IdCausaAccidente", SqlDbType.Int)).Value = model.IdCausaAccidente;
                    sqlCommand.Parameters.Add(new SqlParameter("@CausaAccidente", SqlDbType.NVarChar)).Value = model.CausaAccidente;
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

