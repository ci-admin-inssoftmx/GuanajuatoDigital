using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Entity;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatTipoMotivoAsignacionService : ICatTipoMotivoAsignacionService

    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatTipoMotivoAsignacionService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatTipoMotivoAsignacionModel> ObtenerMotivos()

        {
            //
            List<CatTipoMotivoAsignacionModel> ListaMotivos = new List<CatTipoMotivoAsignacionModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT ta.*, e.estatusdesc FROM tipoMotivoAsignacion AS ta LEFT JOIN estatus AS e ON ta.estatus = e.estatus;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatTipoMotivoAsignacionModel motivo = new CatTipoMotivoAsignacionModel();
                            motivo.idTipoAsignacion = reader["idTipoAsignacion"] != DBNull.Value ? Convert.ToInt32(reader["idTipoAsignacion"]) : 0;
                            motivo.tipoAsignacion = reader["tipoAsignacion"] != DBNull.Value ? reader["tipoAsignacion"].ToString() : string.Empty;
                            motivo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            motivo.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            motivo.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);

                            ListaMotivos.Add(motivo);
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
            return ListaMotivos;


        }
    }
}
