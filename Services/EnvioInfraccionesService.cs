using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using System.Collections.Generic;
using System.Data;
using System;
using GuanajuatoAdminUsuarios.Models;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Text;

namespace GuanajuatoAdminUsuarios.Services
{
    public class EnvioInfraccionesService : IEnvioInfraccionesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public EnvioInfraccionesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<EnvioInfraccionesModel> ObtenerInfracciones(EnvioInfraccionesModel model)
        {
            List<EnvioInfraccionesModel> ListaInfracciones = new List<EnvioInfraccionesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    string findFolio = "";

                    if (!string.IsNullOrEmpty(model.folioInfraccion))                    
                        findFolio = "AND folioInfraccion = @FOLIO";                    

                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT inf.idInfraccion, inf.idOficial" +
                                            ", inf.idDependencia" +
                                            ", inf.idDelegacion" +
                                            ", inf.idVehiculo" +
                                            ", inf.idAplicacion" +
                                            ", inf.idGarantia" +
                                            ", inf.idEstatusInfraccion" +
                                            ", inf.idMunicipio" +
                                            ", inf.idTramo" +
                                            ", inf.idCarretera" +
                                            ", inf.idPersona" +
                                            ", inf.idPersonaConductor" +
                                            ", inf.placasVehiculo" +
                                            ", inf.folioInfraccion" +
                                            ", inf.fechaInfraccion" +
                                            ", inf.kmCarretera" +
                                            ", inf.observaciones" +
                                            ", inf.lugarCalle" +
                                            ", inf.lugarNumero" +
                                            ", inf.lugarColonia" +
                                            ", inf.lugarEntreCalle" +
                                            ", inf.infraccionCortesia" +
                                            ", inf.NumTarjetaCirculacion" +
                                            ", inf.fechaActualizacion" +
                                            ", inf.actualizadoPor" +
                                            ", inf.estatus" +
                                            ", CONVERT(varchar, inf.fechaInfraccion, 103) AS fecha" +
											", CONVERT(varchar, dateadd(day, 10,  inf.fechaInfraccion),  103) AS fechaVencimiento" +
											", prop.nombre AS nombrePropietario" +
                                            ", prop.apellidoPaterno AS apellidoPaternoPropietario" +
                                            ", prop.apellidoMaterno AS apellidoMaternoPropietario" +
                                            ", cond.nombre AS nombreConductor" +
                                            ", cond.apellidoPaterno AS apellidoPaternoConductor" +
                                            ", cond.apellidoMaterno AS apellidoMaternoConductor" +
                                            ", estIn.idEstatusInfraccion, estIn.estatusInfraccion " +

                            "FROM infracciones as inf " +
                            "LEFT JOIN catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion " +
                            "LEFT JOIN personas AS prop ON inf.idPersona = prop.idPersona " +
                            "LEFT JOIN opeInfraccionesPersonas AS cond ON inf.idPersonaConductor = cond.idPersona AND inf.idInfraccion = cond.idInfraccion " +
							"WHERE CONVERT(DATETIME, fechaInfraccion, 120) BETWEEN CONVERT(DATETIME, @fechaInicio, 101) AND CONVERT(DATETIME, @fechaFin, 101) AND DATEDIFF(day, inf.fechaInfraccion, GETDATE()) > 10 " + findFolio, connection); 



                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.DateTime)).Value = (object)model.FechaInicio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.DateTime)).Value = (object)model.FechaFin ?? DBNull.Value;

					if (!string.IsNullOrEmpty(model.folioInfraccion))
					{
						command.Parameters.Add(new SqlParameter("@FOLIO", SqlDbType.VarChar)).Value = (object)model.folioInfraccion ?? DBNull.Value;
					}
                   
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            EnvioInfraccionesModel infraccion = new EnvioInfraccionesModel();
                            infraccion.IdInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
                            object idEstatusInfraccionValue = reader["idEstatusInfraccion"];
                            if (idEstatusInfraccionValue != DBNull.Value)
                            {
                                infraccion.idEstatusInfraccion = Convert.ToInt32(idEstatusInfraccionValue);
                            }
                            infraccion.estatusInfraccion = reader["estatusInfraccion"].ToString();
                            infraccion.placas = reader["placasVehiculo"].ToString();
                            infraccion.folioInfraccion = reader["folioInfraccion"].ToString();
                            infraccion.fecha = reader["fecha"].ToString();
							infraccion.fechaVencimiento = reader["fechaVencimiento"].ToString();
							infraccion.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
                            infraccion.nombreConductor = reader["nombreConductor"].ToString();
                            infraccion.apellidoPaternoConductor = reader["apellidoPaternoConductor"].ToString();
                            infraccion.apellidoMaternoConductor = reader["apellidoMaternoConductor"].ToString();
                            infraccion.nombrePropietario = reader["nombrePropietario"].ToString();
                            infraccion.apellidoPaternoPropietario = reader["apellidoPaternoPropietario"].ToString();
                            infraccion.apellidoMaternoPropietario = reader["apellidoMaternoPropietario"].ToString();
                            infraccion.nombreCompletoConductor = $"{reader["nombreConductor"]} {reader["apellidoPaternoConductor"]} {reader["apellidoMaternoConductor"]}";
                            infraccion.nombreCompletoPropietario = $"{reader["nombrePropietario"]} {reader["apellidoPaternoPropietario"]} {reader["apellidoMaternoPropietario"]}";

                            //model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
                            ListaInfracciones.Add(infraccion);

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
            return ListaInfracciones;


        }
        public int GuardarEnvioInfracciones(ModalEnvioModel model)
        {
            int infraccionEnviada = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE infracciones SET oficioEnvio = @Oficio, fechaEnvio = @FechaEnvio, idOficinaRenta = @IdLugarEnvio, idEstatusEnvio = @idEstatusEnvio WHERE idInfraccion = @idInfraccion";

                    foreach (int id in model.SelectedIds)
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Oficio", model.Oficio);
                        command.Parameters.AddWithValue("@FechaEnvio", model.FechaEnvio);
                        command.Parameters.AddWithValue("@IdLugarEnvio", model.IdLugarEnvio);
                        command.Parameters.AddWithValue("@idEstatusEnvio", 1);
                        command.Parameters.AddWithValue("@idInfraccion", id);

                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    // Manejar excepción si es necesario
                }
                finally
                {
                    connection.Close();
                }

            }

            return infraccionEnviada;
        }


    }

}


