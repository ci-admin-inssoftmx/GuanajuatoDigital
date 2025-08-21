using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatCamposObligatoriosService : ICatCamposObligatoriosService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatCamposObligatoriosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatCamposObligatoriosModel> GetCamposObligatorios()
        {
            //
            List<CatCamposObligatoriosModel> listaCatCamposObligatorios = new List<CatCamposObligatoriosModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT c.idCampo, c.nombreCampo, c.fechaActualizacion,
                                                               co.idCampoObligatorio, 
                                                               co.idDelegacion, 
                                                               co.idMunicipio, 
                                                               ISNULL(co.accidentes, 0) AS accidentes, 
                                                               ISNULL(co.infracciones, 0) AS infracciones, 
                                                               ISNULL(co.depositos, 0) AS depositos 
                                                        FROM catCampos c 
                                                        LEFT JOIN catCamposObligatorios co ON c.idCampo = co.idCampo 
                                                        ORDER BY c.idCampo ASC", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatCamposObligatoriosModel catCamposObligatorios = new CatCamposObligatoriosModel
                            {
                                IdCampoObligatorio = reader["idCampoObligatorio"] != DBNull.Value ? Convert.ToInt32(reader["idCampoObligatorio"]) : 0,
                                IdDelegacion = reader["idDelegacion"] != DBNull.Value ? Convert.ToInt32(reader["idDelegacion"]) : 0,
                                IdMunicipio = reader["idMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["idMunicipio"]) : 0,
                                IdCampo = Convert.ToInt32(reader["idCampo"]),
                                Campo = reader["nombreCampo"].ToString(),
                                EstatusAccidente = Convert.ToInt32(reader["accidentes"]),
                                EstatusInfracciones = Convert.ToInt32(reader["infracciones"]),
                                EstatusDepositos = Convert.ToInt32(reader["depositos"]),
                                FechaActualizacion = reader["FechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaActualizacion"]) : (DateTime?)null
                            };

                            listaCatCamposObligatorios.Add(catCamposObligatorios);

                        }

                    }

                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            return listaCatCamposObligatorios;


        }

    }

}
