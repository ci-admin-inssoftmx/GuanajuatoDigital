/*
 * Descripción:
 * Proyecto: Services
 * Fecha de creación: Sunday, February 18th 2024 11:08:42 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Sun Feb 18 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Util;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatDependenciaEnviaService : ICatDependenciaEnviaService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatDependenciaEnviaService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatDependenciaEnviaModel> ObtenerDependenciasEnviaActivas()
        {
            List<CatDependenciaEnviaModel> ListaEntidades = new List<CatDependenciaEnviaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("select idDependenciaEnvia as id, nombreDependencia as nombre from catDependenciasEnvian where estatus=1 order by nombreDependencia;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatDependenciaEnviaModel entidad = new CatDependenciaEnviaModel
                            {
                                id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : -1,
                                nombre = reader["nombre"] != DBNull.Value ? reader["nombre"].ToString() : string.Empty
                            };
                            ListaEntidades.Add(entidad);

                        }

                    }

                }
                catch (SqlException ex)
                {
                    Logger.Error("Error al obtener Entidades Envian:" + ex);
                }
                finally
                {
                    connection.Close();
                }
            return ListaEntidades;
        }
    }
}
