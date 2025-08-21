using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
//using Telerik.SvgIcons;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatClasificacionAccidentesService : ICatClasificacionAccidentes
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatClasificacionAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatClasificacionAccidentesModel> GetClasificacionAccidentes(int corp)
        {
            //
            List<CatClasificacionAccidentesModel> ListaClasificaciones = new List<CatClasificacionAccidentesModel>();
			var corporation = corp < 2 ? 1 : corp;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT c.*, e.estatus     
                                                        FROM catClasificacionAccidentes AS c 
                                                        INNER JOIN estatus AS e ON c.estatus = e.estatus 
                                                        Where transito = @corp
                                                        ORDER BY c.fechaActualizacion DESC;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatClasificacionAccidentesModel clasificacion = new CatClasificacionAccidentesModel();
                            clasificacion.IdClasificacionAccidente = Convert.ToInt32(reader["IdClasificacionAccidente"].ToString());
                            clasificacion.NombreClasificacion = reader["NombreClasificacion"].ToString();
                            clasificacion.estatusDesc = reader["estatus"].ToString();
                            clasificacion.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            clasificacion.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            ListaClasificaciones.Add(clasificacion);

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
            return ListaClasificaciones;


        }



        public List<CatClasificacionAccidentesModel> ObtenerClasificacionesActivas(int corp)
        {
            //
            List<CatClasificacionAccidentesModel> ListaClasificaciones = new List<CatClasificacionAccidentesModel>();
			var corporation = corp < 2 ? 1 : corp;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT c.*, e.estatus,e.estatusDesc 
                                                          FROM catClasificacionAccidentes AS c 
                                                          LEFT JOIN estatus AS e ON c.estatus = e.estatus
                                                         where transito =@corp
                                                          ORDER BY NombreClasificacion ASC;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatClasificacionAccidentesModel clasificacion = new CatClasificacionAccidentesModel();
                            clasificacion.IdClasificacionAccidente = reader.GetFieldValue<int?>("IdClasificacionAccidente") ?? 0;
                            clasificacion.NombreClasificacion = reader.GetFieldValue<string>("NombreClasificacion");
                            clasificacion.estatusDesc = reader.GetFieldValue<string>("estatusDesc") ?? string.Empty;
                            clasificacion.FechaActualizacion = reader.GetFieldValue<DateTime?>("FechaActualizacion") ?? DateTime.MinValue;
                            clasificacion.Estatus = reader.GetFieldValue<int?>("estatus") ?? 0;

                            ListaClasificaciones.Add(clasificacion);

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
            return ListaClasificaciones;


        }


        public CatClasificacionAccidentesModel GetClasificacionAccidenteByID(int IdClasificacionAccidente)
        {
            CatClasificacionAccidentesModel clasificacion = new CatClasificacionAccidentesModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catClasificacionAccidentes where idClasificacionAccidente=@idClasificacionAccidente", connection);
                    command.Parameters.Add(new SqlParameter("@idClasificacionAccidente", SqlDbType.Int)).Value = IdClasificacionAccidente;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            clasificacion.IdClasificacionAccidente = Convert.ToInt32(reader["idClasificacionAccidente"].ToString());
                            clasificacion.Estatus = Convert.ToInt32(reader["estatus"].ToString());

                            clasificacion.NombreClasificacion = reader["nombreClasificacion"].ToString();

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
        public int CrearClasificacionAccidente(CatClasificacionAccidentesModel model,int corp)
        {
			var corporation = corp < 2 ? 1 : corp;

			int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catClasificacionAccidentes(nombreClasificacion,estatus,fechaActualizacion,actualizadoPor,transito) values(@nombreClasificacion,@estatus,@fechaActualizacion,@actualizadoPor,@corp)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@nombreClasificacion", SqlDbType.VarChar)).Value = model.NombreClasificacion;
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
        public int EditarClasificacionAccidente(CatClasificacionAccidentesModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catClasificacionAccidentes set nombreClasificacion=@nombreClasificacion, estatus = @estatus,fechaActualizacion = @fechaActualizacion, actualizadoPor =@actualizadoPor where idClasificacionAccidente=@idClasificacionAccidente",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idClasificacionAccidente", SqlDbType.Int)).Value = model.IdClasificacionAccidente;
                    sqlCommand.Parameters.Add(new SqlParameter("@nombreClasificacion", SqlDbType.NVarChar)).Value = model.NombreClasificacion;
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



