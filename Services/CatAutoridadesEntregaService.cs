using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatAutoridadesEntregaService : ICatAutoridadesEntregaService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatAutoridadesEntregaService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatAutoridadesEntregaModel> ObtenerAutoridadesActivas(int corp)
        {
            //
            var corporation = corp < 2 ? 1 : corp;
            List<CatAutoridadesEntregaModel> ListaAutoridades = new List<CatAutoridadesEntregaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT catAutoridadesEntrega.*, estatus.estatusdesc 
                                                          FROM catAutoridadesEntrega 
                                                           JOIN estatus ON catAutoridadesEntrega.estatus = estatus.estatus
                                                        where transito = @corp
                                                        ORDER BY AutoridadEntrega ASC", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatAutoridadesEntregaModel autoridad = new CatAutoridadesEntregaModel();
                            autoridad.IdAutoridadEntrega = Convert.ToInt32(reader["IdAutoridadEntrega"].ToString());
                            autoridad.AutoridadEntrega = reader["AutoridadEntrega"].ToString();
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
        public CatAutoridadesEntregaModel GetAutoridadesByID(int IdAutoridadEntrega,int corp)
        {
            CatAutoridadesEntregaModel autoridad = new CatAutoridadesEntregaModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"Select * from catAutoridadesEntrega where idAutoridadEntrega=@idAutoridadEntrega", connection);
                    command.Parameters.Add(new SqlParameter("@idAutoridadEntrega", SqlDbType.Int)).Value = IdAutoridadEntrega;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            autoridad.IdAutoridadEntrega = Convert.ToInt32(reader["idAutoridadEntrega"].ToString());
                            autoridad.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            autoridad.AutoridadEntrega = reader["autoridadEntrega"].ToString();
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

        public int GuardarAutoridad(CatAutoridadesEntregaModel autoridad,int corp)
        {
			var corporation = corp < 2 ? 1 : corp;
			int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand(@"Insert into catAutoridadesEntrega(autoridadEntrega,estatus,fechaActualizacion,transito) values(@autoridadEntrega,@Estatus,@FechaActualizacion,@corp)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@autoridadEntrega", SqlDbType.VarChar)).Value = autoridad.AutoridadEntrega;
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
        public int UpdateAutoridad(CatAutoridadesEntregaModel autoridad,int corp)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catAutoridadesEntrega set autoridadEntrega=@autoridadEntrega, estatus = @Estatus where idAutoridadEntrega=@idAutoridadEntrega",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idAutoridadEntrega", SqlDbType.Int)).Value = autoridad.IdAutoridadEntrega;
                    sqlCommand.Parameters.Add(new SqlParameter("@autoridadEntrega", SqlDbType.VarChar)).Value = autoridad.AutoridadEntrega;
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
