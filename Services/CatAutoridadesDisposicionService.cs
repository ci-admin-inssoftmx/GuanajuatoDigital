using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatAutoridadesDisposicionService : ICatAutoridadesDisposicionService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatAutoridadesDisposicionService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatAutoridadesDisposicionModel> ObtenerAutoridadesActivas(int corp)
        {
            //
            List<CatAutoridadesDisposicionModel> ListaAutoridades = new List<CatAutoridadesDisposicionModel>();
            var corporation = corp < 2 ? 1 : corp;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT catAutoridadesDisposicion.*, estatus.estatusdesc 
                                                          FROM catAutoridadesDisposicion JOIN estatus ON catAutoridadesDisposicion.estatus = estatus.estatus
														  where transito=@corp
                                                        ORDER BY nombreAutoridadDisposicion ASC", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatAutoridadesDisposicionModel autoridad = new CatAutoridadesDisposicionModel();
                            autoridad.IdAutoridadDisposicion = Convert.ToInt32(reader["IdAutoridadDisposicion"].ToString());
                            autoridad.NombreAutoridadDisposicion = reader["NombreAutoridadDisposicion"].ToString();
                            //marcasVehiculo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            //marcasVehiculo.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            autoridad.Estatus = Convert.ToInt32(reader["Estatus"].ToString());
                            autoridad.estatusDesc = reader["estatusDesc"].ToString();
                            ListaAutoridades.Add(autoridad);
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
            return ListaAutoridades;


        }

        public CatAutoridadesDisposicionModel GetAutoridadesByID(int IdAutoridad, int corp)
        {
            CatAutoridadesDisposicionModel autoridad = new CatAutoridadesDisposicionModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"Select * from catAutoridadesDisposicion where idAutoridadDisposicion=@IdAutoridad", connection);
                    command.Parameters.Add(new SqlParameter("@IdAutoridad", SqlDbType.Int)).Value = IdAutoridad;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            autoridad.IdAutoridadDisposicion = Convert.ToInt32(reader["idAutoridadDisposicion"].ToString());
                            autoridad.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            autoridad.NombreAutoridadDisposicion = reader["nombreAutoridadDisposicion"].ToString();
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

            return autoridad;
        }

        public int GuardarAutoridad(CatAutoridadesDisposicionModel autoridad, int corp)
        {
            int result = 0;
			var corporation = corp < 2 ? 1 : corp;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand(@"Insert into catAutoridadesDisposicion(nombreAutoridadDisposicion,estatus,fechaActualizacion,transito) 
                                                                        values(@nombreAutoridadDisposicion,@Estatus,@FechaActualizacion,@corp)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@nombreAutoridadDisposicion", SqlDbType.VarChar)).Value = autoridad.NombreAutoridadDisposicion;
                    sqlCommand.Parameters.Add(new SqlParameter("@Estatus", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@FechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
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
        public int UpdateAutoridad(CatAutoridadesDisposicionModel autoridad, int corp)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catAutoridadesDisposicion set nombreAutoridadDisposicion=@nombreAutoridadDisposicion, estatus = @Estatus where idAutoridadDisposicion=@idAutoridadDisposicion",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idAutoridadDisposicion", SqlDbType.Int)).Value = autoridad.IdAutoridadDisposicion;
                    sqlCommand.Parameters.Add(new SqlParameter("@nombreAutoridadDisposicion", SqlDbType.VarChar)).Value = autoridad.NombreAutoridadDisposicion;
                    sqlCommand.Parameters.Add(new SqlParameter("@Estatus", SqlDbType.Int)).Value = autoridad.Estatus;

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
