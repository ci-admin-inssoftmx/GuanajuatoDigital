using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class DelegacionesService : IDelegacionesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public DelegacionesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<Delegaciones> GetDelegaciones()
        {
            List<Delegaciones> delegaciones = new List<Delegaciones>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select idDelegacion, delegacion, fechaActualizacion, actualizadoPor, estatus, ISNULL(transito,0) transito from catDelegaciones where estatus=1 ORDER BY delegacion ASC", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Delegaciones delegacion = new Delegaciones();
                            delegacion.IdDelegacion = Convert.ToInt32(reader["idDelegacion"].ToString());
                            delegacion.Delegacion = reader["delegacion"].ToString();
                            delegacion.Transito = Convert.ToBoolean(reader["transito"]);
                            delegaciones.Add(delegacion);
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
            return delegaciones;
        }


        public string getAbreviaturaMunicipio(int idDelegacion)
        {
            string resultado = "";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("select m.abreviatura as abreviatura  from delegaciones d left join catMunicipios m on d.idMunicipio=m.idMunicipio  where idDelegacion= @idDelegacion", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = idDelegacion;
                    sqlCommand.CommandType = CommandType.Text;

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read()) // Intenta leer un registro del resultado
                        {
                            // Obtiene el valor de la columna abreviatura
                            resultado =reader["abreviatura"] == System.DBNull.Value ? string.Empty : reader["abreviatura"].ToString();
                        }
                    }
                }
                catch (SqlException ex)
                {
                   Logger.Error("Error al obtener al abrevitura del municipio:"+ex);
                }
                finally
                {
                    connection.Close();
                }
            }
            return resultado;
        }
   
    
    
        public CatalogModel GetExtensions()
        {
            var result = new CatalogModel();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand sqlCommand = new SqlCommand("select top 1 maxZise size , extensions from FileExtensions\r\n", connection);
					sqlCommand.CommandType = CommandType.Text;
					using (SqlDataReader reader = sqlCommand.ExecuteReader())
					{
						if (reader.Read()) // Intenta leer un registro del resultado
						{
                            result.value = Convert.ToString(reader["size"]);
                            result.text = (string)reader["extensions"];
                            
						}
					}
				}
				catch (SqlException ex)
				{
					Logger.Error("Error al obtener las extenciones:" + ex);
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
