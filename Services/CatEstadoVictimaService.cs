using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatEstadoVictimaService : ICatEstadoVictimaService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatEstadoVictimaService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatEstadoVictimaModel> ObtenerEstados()
        {
            //
            List<CatEstadoVictimaModel> ListaEstados = new List<CatEstadoVictimaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT catEstadoVictima.*, estatus.estatusdesc FROM catEstadoVictima JOIN estatus ON catEstadoVictima.estatus = estatus.estatus", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatEstadoVictimaModel estado = new CatEstadoVictimaModel();
                            estado.IdEstadoVictima = Convert.ToInt32(reader["IdEstadoVictima"].ToString());
                            estado.EstadoVictima = reader["EstadoVictima"].ToString();
                            ListaEstados.Add(estado);

                        }

                    }

                }
                catch (SqlException ex)
                {
                  
                }
                finally
                {
                    connection.Close();
                }
            return ListaEstados;


        }
    }
}

