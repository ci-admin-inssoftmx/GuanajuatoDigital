using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Entity;

namespace GuanajuatoAdminUsuarios.Services
{
    public class DescripcionEvetosService : ICatDescripcionesEventoService

    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public DescripcionEvetosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<DescripcionesEventoModel> ObtenerDescripciones()
        {
            //
            List<DescripcionesEventoModel> ListaDescripciones = new List<DescripcionesEventoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT de.*, e.estatusdesc FROM catDescripcionesEvento AS de LEFT JOIN estatus AS e ON de.estatus = e.estatus;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            DescripcionesEventoModel descripcion = new DescripcionesEventoModel();
                            descripcion.idDescripcion = reader["idDescripcion"] != DBNull.Value ? Convert.ToInt32(reader["idDescripcion"]) : 0;
                            descripcion.descripcionEvento = reader["descripcionEvento"] != DBNull.Value ? reader["descripcionEvento"].ToString() : string.Empty;
                            descripcion.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            descripcion.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            descripcion.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);

                            ListaDescripciones.Add(descripcion);
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
            return ListaDescripciones;


        }
    }
}

