using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatTiposVehiculosService : ICatTiposVehiculosService

    {

        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatTiposVehiculosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<TiposVehiculosModel> GetTiposVehiculos()
        {
            //
            List<TiposVehiculosModel> ListaTiposVehiculo = new List<TiposVehiculosModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT tv.*, e.estatusdesc FROM catTiposVehiculo AS tv LEFT JOIN estatus AS e ON tv.estatus = e.estatus ORDER BY TipoVehiculo ASC;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            TiposVehiculosModel tipoVehiculo = new TiposVehiculosModel();

                            tipoVehiculo.IdTipoVehiculo = Convert.ToInt32(reader["IdTipoVehiculo"] is DBNull ? 0 : reader["IdTipoVehiculo"]);
                            tipoVehiculo.TipoVehiculo = reader["TipoVehiculo"] is DBNull ? string.Empty : reader["TipoVehiculo"].ToString();
                            tipoVehiculo.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            tipoVehiculo.EstatusDesc = reader["estatusDesc"] is DBNull ? string.Empty : reader["estatusDesc"].ToString();
                            tipoVehiculo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            tipoVehiculo.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            tipoVehiculo.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);

                            ListaTiposVehiculo.Add(tipoVehiculo);
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
            return ListaTiposVehiculo;


        }
        public TiposVehiculosModel GetTipoVehiculoByID(int IdTipoVehiculo)
        {
            TiposVehiculosModel tipoVehiculo = new TiposVehiculosModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT tv.*, e.estatusdesc FROM catTiposVehiculo AS tv LEFT JOIN estatus AS e ON tv.estatus = e.estatus WHERE IdTipoVehiculo = @IdTipoVehiculo ", connection);
                    command.Parameters.Add(new SqlParameter("@IdTipoVehiculo", SqlDbType.Int)).Value = IdTipoVehiculo;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            tipoVehiculo.IdTipoVehiculo = Convert.ToInt32(reader["IdTipoVehiculo"] is DBNull ? 0 : reader["IdTipoVehiculo"]);
                            tipoVehiculo.TipoVehiculo = reader["TipoVehiculo"] is DBNull ? string.Empty : reader["TipoVehiculo"].ToString();
                            tipoVehiculo.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            tipoVehiculo.EstatusDesc = reader["estatusDesc"] is DBNull ? string.Empty : reader["estatusDesc"].ToString();
                            tipoVehiculo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            tipoVehiculo.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            tipoVehiculo.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);


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

            return tipoVehiculo;
        }



		public bool ValidarExistenciaTipoVehiculo(string descripcion)
		{
			var result = false;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand sqlCommand = new SqlCommand("select count(*) result from catTiposVehiculo where  TipoVehiculo=@descripcion", connection);
					sqlCommand.Parameters.Add(new SqlParameter("@descripcion", SqlDbType.VarChar)).Value = descripcion;

					sqlCommand.CommandType = CommandType.Text;
					var read = sqlCommand.ExecuteReader();

					while (read.Read())
					{

						result = (0 == (int)read["result"]);


					}



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

		public int CreateTipoVehiculo(TiposVehiculosModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catTiposVehiculo(TipoVehiculo,estatus,fechaActualizacion,actualizadoPor) values(@TipoVehiculo,@estatus,@fechaActualizacion,@actualizadoPor)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@TipoVehiculo", SqlDbType.VarChar)).Value = model.TipoVehiculo;
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
        public int UpdateTipoVehiculo(TiposVehiculosModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catTiposVehiculo set TipoVehiculo=@TipoVehiculo, estatus = @estatus,fechaActualizacion = @fechaActualizacion, actualizadoPor =@actualizadoPor where IdTipoVehiculo=@IdTipoVehiculo",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@IdTipoVehiculo", SqlDbType.Int)).Value = model.IdTipoVehiculo;
                    sqlCommand.Parameters.Add(new SqlParameter("@TipoVehiculo", SqlDbType.NVarChar)).Value = model.TipoVehiculo;
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
        public int obtenerIdPorTipo(string categoria)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("SELECT idTipoVehiculo FROM catTiposVehiculo WHERE tipoVehiculo = @tipoVehiculo", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@tipoVehiculo", SqlDbType.NVarChar)).Value = categoria;
                    sqlCommand.CommandType = CommandType.Text;

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read()) // Intenta leer un registro del resultado
                        {
                            // Obtiene el valor de la columna "idMunicipio"
                            result = Convert.ToInt32(reader["idTipoVehiculo"]);
                        }
                    }
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
    }
}
