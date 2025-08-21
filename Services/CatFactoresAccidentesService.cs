using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatFactoresAccidentesService : ICatFactoresAccidentesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatFactoresAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatFactoresAccidentesModel> GetFactoresAccidentes(int corp)
        {
            //
            List<CatFactoresAccidentesModel> ListaFactores = new List<CatFactoresAccidentesModel>();
			var corporation = corp < 2 ? 1 : corp;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT f.*, e.estatusDesc 
                                                          FROM catFactoresAccidentes AS f 
                                                          INNER JOIN estatus AS e ON f.estatus = e.estatus 
                                                          Where transito = @corp  
                                                          ORDER BY FactorAccidente ASC;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatFactoresAccidentesModel factor = new CatFactoresAccidentesModel();
                            factor.IdFactorAccidente = Convert.ToInt32(reader["IdFactorAccidente"].ToString());
                            factor.FactorAccidente = reader["FactorAccidente"].ToString();
                            factor.estatusDesc = reader["estatusDesc"].ToString();
                            factor.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            factor.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            factor.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaFactores.Add(factor);

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
            return ListaFactores;


        }

        public List<CatFactoresAccidentesModel> GetFactoresAccidentesActivos(int corp)
        {
            //
            List<CatFactoresAccidentesModel> ListaFactores = new List<CatFactoresAccidentesModel>();
			var corporation = corp < 2 ? 1 : corp;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT f.*, e.estatusDesc 
                                                        FROM catFactoresAccidentes AS f 
                                                        INNER JOIN estatus AS e ON f.estatus = e.estatus 
                                                        WHERE f.estatus = 1 and transito = @corp
                                                        ORDER BY FactorAccidente ASC;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatFactoresAccidentesModel factor = new CatFactoresAccidentesModel();
                            factor.IdFactorAccidente = Convert.ToInt32(reader["IdFactorAccidente"]?.ToString() ?? "0");
                            factor.FactorAccidente = reader["FactorAccidente"]?.ToString() ?? "";
                            factor.estatusDesc = reader["estatusDesc"]?.ToString() ?? "";
                            factor.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"]?.ToString() ?? DateTime.MinValue.ToString());
                            factor.Estatus = Convert.ToInt32(reader["estatus"]?.ToString() ?? "0");

                            // factor.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaFactores.Add(factor);

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
            return ListaFactores;


        }
        public CatFactoresAccidentesModel GetFactorByID(int IdFactorAccidente)
        {
            CatFactoresAccidentesModel clasificacion = new CatFactoresAccidentesModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catFactoresAccidentes where idFactorAccidente=@IdFactorAccidente", connection);
                    command.Parameters.Add(new SqlParameter("@IdFactorAccidente", SqlDbType.Int)).Value = IdFactorAccidente;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            clasificacion.IdFactorAccidente = Convert.ToInt32(reader["IdFactorAccidente"].ToString());
                            clasificacion.Estatus = Convert.ToInt32(reader["estatus"].ToString());

                            clasificacion.FactorAccidente = reader["FactorAccidente"].ToString();

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

            return clasificacion;
        }
        public int GuardarFactor(CatFactoresAccidentesModel model,int corp)
        {
			var corporation = corp < 2 ? 1 : corp;

			int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catFactoresAccidentes(FactorAccidente,estatus,fechaActualizacion,actualizadoPor,transito) values(@FactorAccidente,@estatus,@fechaActualizacion,@actualizadoPor,@corp)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@FactorAccidente", SqlDbType.VarChar)).Value = model.FactorAccidente;
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
        public int UpdateFactor(CatFactoresAccidentesModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catFactoresAccidentes set FactorAccidente=@FactorAccidente, estatus = @estatus,fechaActualizacion = @fechaActualizacion, actualizadoPor =@actualizadoPor where idFactorAccidente=@IdFactorAccidente",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@IdFactorAccidente", SqlDbType.Int)).Value = model.IdFactorAccidente;

                    sqlCommand.Parameters.Add(new SqlParameter("@FactorAccidente", SqlDbType.NVarChar)).Value = model.FactorAccidente;
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

