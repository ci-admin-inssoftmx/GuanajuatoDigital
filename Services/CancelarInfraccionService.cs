using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Entity;
using Microsoft.Extensions.DependencyModel;
using GuanajuatoAdminUsuarios.Framework.Catalogs;

namespace GuanajuatoAdminUsuarios.Services
{

    public class CancelarInfraccionService : ICancelarInfraccionService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CancelarInfraccionService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CancelarInfraccionModel> ObtenerInfraccionPorFolio(string FolioInfraccion, int corp)
        {
            //
            List<CancelarInfraccionModel> ListaInfracciones = new List<CancelarInfraccionModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT i.*, v.serie, e.estatusInfraccion, 
                                                            CONCAT( pV.nombre,' ', pV.apellidoPaterno,' ', pV.apellidoMaterno)AS nombrePropietario,
                                                            CONCAT(pI.nombre,' ',pI.apellidoPaterno,' ', pI.apellidoMaterno)AS nombreConductor,
                                                            del.delegacion
															FROM infracciones AS i 
                                                            LEFT JOIN vehiculos AS v ON i.idVehiculo = v.idVehiculo 
                                                            LEFT JOIN catEstatusInfraccion AS e ON i.idEstatusInfraccion = e.idEstatusInfraccion 
                                                            LEFT JOIN personas AS pI ON pI.IdPersona = i.IdPersona 
                                                            LEFT JOIN personas AS pV ON pV.IdPersona = v.idPersona 
									                        LEFT JOIN catDelegaciones AS del ON del.idDelegacion = i.idDelegacion 
                                                            WHERE i.folioInfraccion LIKE '%' + @FolioInfraccion + '%'
                                                            and i.idEstatusInfraccion not in(5) and i.transito = @corp  and i.estatus=1
                                                            ORDER BY i.idInfraccion DESC;", connection);
                    command.Parameters.Add(new SqlParameter("@FolioInfraccion", SqlDbType.NVarChar)).Value = FolioInfraccion;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = corp;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CancelarInfraccionModel infraccion = new CancelarInfraccionModel();
                            infraccion.IdInfraccion = reader["IdInfraccion"] is DBNull ? 0 : Convert.ToInt32(reader["IdInfraccion"]);
                            infraccion.IdDelegacion = reader["IdDelegacion"] is DBNull ? 0 : Convert.ToInt32(reader["IdDelegacion"]);

                            infraccion.FolioInfraccion = reader["folioInfraccion"] is DBNull ? string.Empty : reader["folioInfraccion"].ToString();
                            infraccion.FechaInfraccion = reader["FechaInfraccion"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(reader["FechaInfraccion"]);
                            infraccion.Conductor = reader["nombreConductor"] is DBNull ? string.Empty : reader["nombreConductor"].ToString();
                            infraccion.Placas = reader["placasVehiculo"] is DBNull ? string.Empty : reader["placasVehiculo"].ToString();
                            infraccion.Serie = reader["Serie"] is DBNull ? string.Empty : reader["Serie"].ToString();
                            infraccion.Propietario = reader["nombrePropietario"] is DBNull ? string.Empty : reader["nombrePropietario"].ToString();
                            infraccion.EstatusProceso = reader["idEstatusInfraccion"] is DBNull ? 0 : Convert.ToInt32(reader["idEstatusInfraccion"]);
                            infraccion.descEstatusProceso = reader["estatusInfraccion"] is DBNull ? string.Empty : reader["estatusInfraccion"].ToString();
                            infraccion.Delegacion = reader["delegacion"] is DBNull ? string.Empty : reader["delegacion"].ToString();



                            ListaInfracciones.Add(infraccion);

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
            return ListaInfracciones;


        }

        public List<CancelarInfraccionModel> ObtenerInfraccionPorFolioFinanzas(string FolioInfraccion, int corp)
        {
            //
            List<CancelarInfraccionModel> ListaInfracciones = new List<CancelarInfraccionModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT i.*, v.serie, e.estatusInfraccion, 
                                                            CONCAT( pV.nombre,' ', pV.apellidoPaterno,' ', pV.apellidoMaterno)AS nombrePropietario,
                                                            CONCAT(pI.nombre,' ',pI.apellidoPaterno,' ', pI.apellidoMaterno)AS nombreConductor,
                                                            del.delegacion
															FROM infracciones AS i 
                                                            LEFT JOIN vehiculos AS v ON i.idVehiculo = v.idVehiculo 
                                                            LEFT JOIN catEstatusInfraccion AS e ON i.idEstatusInfraccion = e.idEstatusInfraccion 
                                                            LEFT JOIN personas AS pI ON pI.IdPersona = i.IdPersona 
                                                            LEFT JOIN personas AS pV ON pV.IdPersona = v.idPersona 
									                        LEFT JOIN catDelegaciones AS del ON del.idDelegacion = i.idDelegacion 
                                                            WHERE i.folioInfraccion LIKE '%' + @FolioInfraccion + '%'
                                                            and i.estatus=1
                                                            ORDER BY i.idInfraccion DESC;", connection);
                    command.Parameters.Add(new SqlParameter("@FolioInfraccion", SqlDbType.NVarChar)).Value = FolioInfraccion;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = corp; //  and i.idEstatusInfraccion not in(5) and i.transito = @corp  and i.estatus=1
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CancelarInfraccionModel infraccion = new CancelarInfraccionModel();
                            infraccion.IdInfraccion = reader["IdInfraccion"] is DBNull ? 0 : Convert.ToInt32(reader["IdInfraccion"]);
                            infraccion.IdDelegacion = reader["IdDelegacion"] is DBNull ? 0 : Convert.ToInt32(reader["IdDelegacion"]);

                            infraccion.FolioInfraccion = reader["folioInfraccion"] is DBNull ? string.Empty : reader["folioInfraccion"].ToString();
                            infraccion.FechaInfraccion = reader["FechaInfraccion"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(reader["FechaInfraccion"]);
                            infraccion.Conductor = reader["nombreConductor"] is DBNull ? string.Empty : reader["nombreConductor"].ToString();
                            infraccion.Placas = reader["placasVehiculo"] is DBNull ? string.Empty : reader["placasVehiculo"].ToString();
                            infraccion.Serie = reader["Serie"] is DBNull ? string.Empty : reader["Serie"].ToString();
                            infraccion.Propietario = reader["nombrePropietario"] is DBNull ? string.Empty : reader["nombrePropietario"].ToString();
                            infraccion.EstatusProceso = reader["idEstatusInfraccion"] is DBNull ? 0 : Convert.ToInt32(reader["idEstatusInfraccion"]);
                            infraccion.descEstatusProceso = reader["estatusInfraccion"] is DBNull ? string.Empty : reader["estatusInfraccion"].ToString();
                            infraccion.Delegacion = reader["delegacion"] is DBNull ? string.Empty : reader["delegacion"].ToString();



                            ListaInfracciones.Add(infraccion);

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
            return ListaInfracciones;


        }



        public CancelarInfraccionModel ObtenerDetalleInfraccion(int Id)
        {

            CancelarInfraccionModel infraccion = new CancelarInfraccionModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT i.IdInfraccion,i.idDelegacion, i.FolioInfraccion, i.FechaInfraccion, 
                    i.placasVehiculo, i.idPersona, i.idEstatusInfraccion, v.Serie, e.estatusInfraccion, 
                    CONCAT(pV.nombre, ' ', pV.apellidoPaterno, ' ', pV.apellidoMaterno) AS nombrePropietario, 
                    CONCAT(pI.nombre, ' ', pI.apellidoPaterno, ' ', pI.apellidoMaterno) AS nombreConductor 
                    FROM infracciones i 
                    JOIN vehiculos v ON i.idVehiculo = v.idVehiculo 
                    JOIN catEstatusInfraccion AS e ON i.idEstatusInfraccion = e.idEstatusInfraccion 
                    LEFT JOIN opeInfraccionesPersonas AS pI ON pI.IdPersona = i.IdPersonaConductor AND i.idInfraccion = pI.idInfraccion 
                    LEFT JOIN personas AS pV ON pV.IdPersona = v.idPersona 
                    WHERE i.IdInfraccion = @Id", connection);
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int)).Value = Id;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            
                            infraccion.IdInfraccion = Convert.ToInt32(reader["IdInfraccion"].ToString());
                            infraccion.IdDelegacion = Convert.ToInt32(reader["idDelegacion"].ToString());
                            infraccion.FolioInfraccion = reader["FolioInfraccion"].ToString();
                            infraccion.FechaInfraccion = Convert.ToDateTime(reader["FechaInfraccion"].ToString());
                            infraccion.Conductor = reader["nombreConductor"].ToString();
                            infraccion.Placas = reader["placasVehiculo"].ToString();
                            infraccion.Serie = reader["Serie"].ToString();
                            infraccion.Propietario = reader["nombrePropietario"].ToString();
                            infraccion.descEstatusProceso = reader["estatusInfraccion"].ToString();

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
            return infraccion;


        }
        public CancelarInfraccionModel CancelarInfraccionBD(int IdInfraccion,string OficioRevocacion)
        {
            string result = "";

            CancelarInfraccionModel infraccion = new CancelarInfraccionModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"Update infracciones set idEstatusInfraccion = @idEstatusInfraccion, oficioRevocacion = @OficioRevocacion,fechaActualizacion = @fechaActualizacion,actualizadoPor = @actualizadoPor, estatus = @estatus where idInfraccion = @IdInfraccion;
                                                          select folioinfraccion from infracciones where idinfraccion=@IdInfraccion;", connection);
                    command.Parameters.Add(new SqlParameter("@IdInfraccion", SqlDbType.Int)).Value = IdInfraccion;
                    command.Parameters.Add(new SqlParameter("@OficioRevocacion", SqlDbType.VarChar)).Value = OficioRevocacion;
                    command.Parameters.Add(new SqlParameter("@idEstatusInfraccion", SqlDbType.Int)).Value = CatEnumerator.catEstatusInfraccion.Solventada;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;

                    command.CommandType = CommandType.Text;
                    var r  = command.ExecuteReader();
                    while (r.Read())
                    {
                        result = (string)r["folioinfraccion"];
                    }
                    infraccion.EstatusProceso = 2;
                    infraccion.FolioInfraccion = result;
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
            return infraccion;


        }

        public string CancelarInfraccionFinanzas(int IdInfraccion)
        {
            string result = "0";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("usp_UpdateCancelaInf", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@IdInfraccion", SqlDbType.Int)).Value = IdInfraccion;
                    command.Parameters.Add(new SqlParameter("@idEstatusInfraccion", SqlDbType.Int)).Value = 2; // capturada

                    result = Convert.ToString(command.ExecuteNonQuery());
                }
                finally
                {
                    connection.Close();
                }

            }

           
            return result;
        }



    }
}



