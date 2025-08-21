using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Entity;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatTipoUsuarioService : ICatTipoUsuarioService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatTipoUsuarioService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatTiposUsuarioModel> ObtenerTiposUsuario()

        {
            //
            List<CatTiposUsuarioModel> ListaTiposUsuario = new List<CatTiposUsuarioModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT tiu.*, e.estatusdesc FROM catTiposUsuario AS tiu LEFT JOIN estatus AS e ON tiu.estatus = e.estatus;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatTiposUsuarioModel motivo = new CatTiposUsuarioModel();
                            motivo.idTipoUsuario = reader["idTipoUsuario"] != DBNull.Value ? Convert.ToInt32(reader["idTipoUsuario"]) : 0;
                            motivo.tipoUsuario = reader["tipoUsuario"] != DBNull.Value ? reader["tipoUsuario"].ToString() : string.Empty;
                            motivo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            motivo.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            motivo.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);

                            ListaTiposUsuario.Add(motivo);
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
            return ListaTiposUsuario;


        }
    }
}
