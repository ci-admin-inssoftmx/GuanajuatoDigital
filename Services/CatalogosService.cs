using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services
{
	public class CatalogosService : ICatalogosService
	{
		private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
		public CatalogosService(ISqlClientConnectionBD sqlClientConnectionBD)
		{
			_sqlClientConnectionBD = sqlClientConnectionBD;
		}
		public List<Dictionary<string, string>> GetGenericCatalogos(string tabla, string[] campos)
		{
			List<Dictionary<string, string>> modelList = new List<Dictionary<string, string>>();
			string strParams = string.Join(",", campos);
			string strQuery = @"SELECT 
                                {0}
                                FROM {1}
                                WHERE estatus = 1";
			strQuery = string.Format(strQuery, strParams, tabla);
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.CommandTimeout = 3000;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							Dictionary<string, string> dictionary = new Dictionary<string, string>();
							foreach (string campo in campos)
							{
								dictionary.Add(campo, Convert.ToString(reader[campo]));
							}
							modelList.Add(dictionary);
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
			return modelList;
		}

		public List<Dictionary<string, string>> GetGenericCatalogosByFilter(string tabla, string[] campos, string campoFiltro, int idFiltro)
		{
			List<Dictionary<string, string>> modelList = new List<Dictionary<string, string>>();
			string strCampos = string.Join(",", campos);
			string strQuery = @"SELECT
                                {0}
                                FROM {1}
                                WHERE estatus = 1
                                AND {2} = @idFiltro";
			strQuery = string.Format(strQuery, strCampos, tabla, campoFiltro);
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idFiltro", SqlDbType.Int)).Value = idFiltro;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							Dictionary<string, string> dictionary = new Dictionary<string, string>();
							foreach (string campo in campos)
							{
								dictionary.Add(campo, Convert.ToString(reader[campo]));
							}
							modelList.Add(dictionary);
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
			return modelList;
		}


		public List<Dictionary<string, string>> GetGenericCatalogosByFilter(string tabla, string[] campos, string[] campoFiltro, int[] idFiltro)
		{
			List<Dictionary<string, string>> modelList = new List<Dictionary<string, string>>();
			string strCampos = string.Join(",", campos);
			string strQuery = @"SELECT
                                {0}
                                FROM {1} 
                                WHERE estatus = 1";
			int contador = 0;
			foreach (string campo in campoFiltro)
			{
				strQuery += " AND " + campo + " = " + idFiltro[contador];
				contador++;
			}
			//AND {2} = @idFiltro";
			strQuery = string.Format(strQuery, strCampos, tabla, campoFiltro);
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					//command.Parameters.Add(new SqlParameter("@idFiltro", SqlDbType.Int)).Value = idFiltro;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							Dictionary<string, string> dictionary = new Dictionary<string, string>();
							foreach (string campo in campos)
							{
								dictionary.Add(campo, Convert.ToString(reader[campo]));
							}
							modelList.Add(dictionary);
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
			return modelList;
		}

		public List<Dictionary<string, string>> GetGenericCatalogosByFilters(string tabla, string[] campos, string campoFiltro, int idFiltro, string campoFiltro2, int idFiltro2)
		{
			List<Dictionary<string, string>> modelList = new List<Dictionary<string, string>>();
			string strCampos = string.Join(",", campos);
			string strQuery = @"SELECT
                                {0}
                                FROM {1}
                                WHERE estatus = 1
                                AND {2} = @idFiltro 
                                AND {3} = @idFiltro2 ";
			strQuery = string.Format(strQuery, strCampos, tabla, campoFiltro, campoFiltro2);
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idFiltro", SqlDbType.Int)).Value = idFiltro;
					command.Parameters.Add(new SqlParameter("@idFiltro2", SqlDbType.Int)).Value = idFiltro2;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							Dictionary<string, string> dictionary = new Dictionary<string, string>();
							foreach (string campo in campos)
							{
								dictionary.Add(campo, Convert.ToString(reader[campo]));
							}
							modelList.Add(dictionary);
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
			return modelList;
		}



        public int UpdateRelaciones(RelacionModel model)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
					//Establecer el hijo
                    string query = "UPDATE "+model.Catalogo+ @" SET parent = @parent
                                WHERE "+model.Campo+" = @campo";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@parent", model.Padre);
                    command.Parameters.AddWithValue("@campo", model.Origen);

                    command.ExecuteNonQuery();

					//Establecer el padre
                    query = "UPDATE " + model.Catalogo + @" SET isparent = @isparent
                                WHERE " + model.Campo + " = @campo";

                    command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@isparent", 1);
                    command.Parameters.AddWithValue("@campo", model.Padre);

                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return result;
                }
                finally
                {
                    connection.Close();
                }

                return result;
            }
        }

        public List<DependenciasModel> ObtieneTodasCorporaciones()
        {
            //
            List<DependenciasModel> dependencias = new List<DependenciasModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"Select catCorp.*, e.estatusDesc from CatCorporaciones catCorp
                                                            LEFT JOIN estatus AS e ON e.estatus = catCorp.estatus
                                                            WHERE catCorp.estatus = 1
                                                            ", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            DependenciasModel dependencia = new DependenciasModel();
                            dependencia.IdDependencia = Convert.ToInt32(reader["IdCorporacion"].ToString());
                            dependencia.NombreDependencia = reader["Corporacion"].ToString();
                            dependencia.estatusDesc = reader["estatusDesc"].ToString();

                            dependencias.Add(dependencia);

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
            return dependencias;


        }   
    }
}
