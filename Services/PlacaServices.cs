using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Numerics;
//using Telerik.SvgIcons;
using CommandType = System.Data.CommandType;

namespace GuanajuatoAdminUsuarios.Services
{
    public class PlacaServices : IPlacaServices
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public PlacaServices(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        /// <summary>
        /// Se obtendra placas de la tabla depositos
        /// </summary>
        /// <param name="DelegacionId"></param>
        /// <returns></returns>
        public List<PlacaModel> GetPlacasByDelegacionId(int idPension, bool? noEspension)
        {
            List<PlacaModel> placas = new List<PlacaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    if (noEspension==true)
                    {
                        command = new SqlCommand("SELECT "+
                                            "d.IdDeposito, "+
                                            "MAX(d.IdSolicitud) AS IdSolicitud, " +
                                            "MAX(d.idDelegacion) AS idDelegacion, " +
                                            "MAX(d.IdMarca) AS IdMarca, " +
                                            "MAX(d.IdSubmarca) AS IdSubmarca, " +
                                            "MAX(d.IdPension) AS IdPension, " +
                                            "MAX(d.IdTramo) AS IdTramo, " +
                                            "MAX(d.IdColor) AS IdColor, " +
                                            "MAX(d.Serie) AS Serie, " +
                                            "d.Placa, " +
                                            "MAX(d.FechaIngreso) AS FechaIngreso, " +
                                            "MAX(sol.idCarreteraUbicacion) AS idCarreteraUbicacion, " +
                                            "MAX(d.Folio) AS Folio, " +
                                            "MAX(d.Km) AS Km, " +
                                            "MAX(d.Liberado) AS Liberado, " +
                                            "MAX(d.Autoriza) AS Autoriza, " +
                                            "MAX(car.carretera) AS carretera, " +
                                            "MAX(d.FechaActualizacion) AS FechaActualizacion, " +
                                            "MAX(d.ActualizadoPor) AS ActualizadoPor, " +
                                            "MAX(d.estatus) AS estatus, " +
                                            "sol.solicitanteNombre, " +
                                            "sol.solicitanteAp, " +
                                            "sol.solicitanteAm, " +
                                            "sol.folio, " +
                                            "pen.pension, " +
                                            "del.delegacion, " +
                                            "col.color, " +
                                            "cTra.tramo, " +
                                            "m.marcaVehiculo, " +
                                            "subm.nombreSubmarca, " +
                                            "v.idPersona, " +
                                           " p.nombre, " +
                                          "  p.apellidoPaterno, " +
                                         "   p.apellidoMaterno " +
                                        "FROM " +
                                         "   depositos d " +
                                        "LEFT JOIN " +
                                         "   solicitudes sol ON d.idSolicitud = sol.idSolicitud " +
                                        "LEFT JOIN " +
                                        "    pensiones pen ON d.idPension = pen.idPension " +
                                       " LEFT JOIN " +
                                         "   catDelegaciones del ON d.idDelegacion = del.idDelegacion " +
                                        "LEFT JOIN " +
                                         "   catColores col ON d.idColor = col.idColor " +
                                        "LEFT JOIN " +
                                        "    catTramos cTra ON d.Idtramo = cTra.idTramo " +
                                       " LEFT JOIN " +
                                        "    catMarcasVehiculos m ON d.idMarca = m.idMarcaVehiculo " +
                                       " LEFT JOIN " +
                                       "     catSubmarcasVehiculos subm ON d.idSubmarca = subm.idSubmarca " +
                                       " LEFT JOIN " +
                                       "     vehiculos v ON v.placas = d.Placa " +
                                      "  LEFT JOIN " +
                                     "       personas p ON p.idPersona = v.idPersona " +
                                        "LEFT JOIN " +
                                         "   catCarreteras car ON car.idCarretera = sol.idCarreteraUbicacion " +
                                        "WHERE " +
                                            "d.liberado = 1 AND d.estatusSolicitud = 5 AND d.idDelegacion = @idPension GROUP BY " +
                                            "d.IdDeposito, " +
                                            "sol.solicitanteNombre, " +
                                            "sol.solicitanteAp, " +
                                            "sol.solicitanteAm, " +
                                            "sol.folio, " +
                                            "pen.pension, " +
                                            "del.delegacion, " +
                                            "col.color, " +
                                            "cTra.tramo, " +
                                            "m.marcaVehiculo, " +
                                            "subm.nombreSubmarca, " +
                                            "v.idPersona, " +
                                            "p.nombre, " +
                                            "p.apellidoPaterno, " +
                                            "p.apellidoMaterno, " +
                                            "d.Placa  ", connection);
                    }
                    else
                    {
                        command = new SqlCommand("Select * from depositos where idPension=@idPension", connection);
                    }

                    command.Parameters.Add(new SqlParameter("@idPension", SqlDbType.Int)).Value = idPension;
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PlacaModel placa = new PlacaModel();
                            placa.IdDepositos = Convert.ToInt32(reader["IdDeposito"].ToString());
                            placa.Placa = reader["Placa"].ToString();
                            placas.Add(placa);
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
            return placas;
        }

        public List<PlacaModel> GetPlacasIngresos(int idPension)
        {
            List<PlacaModel> placas = new List<PlacaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    
                        command = new SqlCommand(@"SELECT d.idDeposito, d.placa FROM depositos d
                                                WHERE d.fechaIngreso is null AND d.idPension = @idPension AND d.liberado = 0 AND d.estatusSolicitud = 3 AND d.estatus = 1 ", connection);

                    command.Parameters.Add(new SqlParameter("@idPension", SqlDbType.Int)).Value = idPension;
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PlacaModel placa = new PlacaModel();
                            placa.IdDepositos = Convert.ToInt32(reader["IdDeposito"].ToString());
                            placa.Placa = reader["Placa"].ToString();
                            placas.Add(placa);
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
            return placas;
        }

        public List<PlacaModel> GetPlacasLiberacion(int idOficina)
        {
            List<PlacaModel> placas = new List<PlacaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    {
                        command = new SqlCommand(@"SELECT d.placa,d.idDeposito
                            FROM depositos d 
                            WHERE d.estatusSolicitud = 4 AND d.idDelegacion = @idDelegacion  AND d.placa IS NOT NULL; ", connection);
                    }
                    command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = idOficina;
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PlacaModel placa = new PlacaModel();
                            placa.IdDepositos = Convert.ToInt32(reader["IdDeposito"].ToString());
                            placa.Placa = reader["Placa"].ToString();
                            placas.Add(placa);
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
            return placas;
        }
        public List<PlacaModel> GetPlacasSalidas(int idPension)
        {
            List<PlacaModel> placas = new List<PlacaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();

                    command = new SqlCommand(@"SELECT d.idDeposito, d.placa FROM depositos d
                                                WHERE d.idPension = @idPension AND d.liberado = 1 AND d.estatusSolicitud = 5 ", connection);



                    command.Parameters.Add(new SqlParameter("@idPension", SqlDbType.Int)).Value = idPension;
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PlacaModel placa = new PlacaModel();
                            placa.IdDepositos = Convert.ToInt32(reader["IdDeposito"].ToString());
                            placa.Placa = reader["Placa"].ToString();
                            placas.Add(placa);
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
            return placas;
        }

    }
}

