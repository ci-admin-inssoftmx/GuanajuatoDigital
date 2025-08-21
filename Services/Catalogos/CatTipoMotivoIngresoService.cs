/*
 * Descripción:
 * Proyecto: Services
 * Fecha de creación: Sunday, February 18th 2024 7:30:17 pm
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
public class CatTipoMotivoIngresoService : ICatTipoMotivoIngresoService

    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatTipoMotivoIngresoService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatTipoMotivoIngresoModel> ObtenerTiposMotivoIngresoActivos()
        {
            List<CatTipoMotivoIngresoModel> ListaTiposMotivosIngreso = new List<CatTipoMotivoIngresoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("select id as id, nombre as nombre from catTipoMotivoIngreso where estatus=1 order by nombre;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatTipoMotivoIngresoModel tipoMotivo = new CatTipoMotivoIngresoModel
                            {
                                id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : -1,
                                nombre = reader["nombre"] != DBNull.Value ? reader["nombre"].ToString() : string.Empty
                            };
                            ListaTiposMotivosIngreso.Add(tipoMotivo);

                        }

                    }

                }
                catch (SqlException ex)
                {
                    Logger.Error("Error al obtener tipos de motivos para ingreso de deposito:" + ex);
                }
                finally
                {
                    connection.Close();
                }
            return ListaTiposMotivosIngreso;
        }
    }
}