using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class BusquedaDepositoService : IBusquedaDepositoService

    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public BusquedaDepositoService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<BusquedaDepositoModel> ObtenerTodosDepositos( int idPension)
        {
            List<BusquedaDepositoModel> modelList = new List<BusquedaDepositoModel>();
            string strQuery = @"SELECT d.idDeposito,d.idSolicitud,d.idInfraccion,d.idVehiculo,d.fechaIngreso,
                                        d.idPension,d.idGrua,sol.fechaSolicitud,inf.folioInfraccion,d.placa,
                                        p.nombre,p.apellidoPaterno,p.apellidoMaterno,sd.fechaSalida,ga.idGrua,C.concesionario,
                                        pen.pension
                                        From depositos AS d
                                        LEFT JOIN solicitudes AS sol ON sol.idSolicitud = d.idSolicitud
                                        LEFT JOIN infracciones AS inf ON inf.idInfraccion = d.idInfraccion
                                        
                                        LEFT JOIN opeDepositosVehiculos v ON v.idOperacion = 
										(
											SELECT MAX(idOperacion) 
											FROM opeDepositosVehiculos z 
											WHERE z.idVehiculo = d.idVehiculo and z.idDeposito = d.idDeposito
										)
                                        LEFT JOIN opeDepositosPersonas AS p ON p.idPersona = d.idPropietario  p.idDeposito=d.idDeposito
                                        LEFT JOIN serviciosDepositos AS sd ON sd.idDeposito = d.idDeposito
                                        LEFT JOIN gruasAsignadas AS ga ON ga.idDeposito = d.idDeposito
                                        LEFT JOIN gruas AS g ON g.idGrua = ga.idGrua
                                        LEFT JOIN pensiones AS pen ON pen.idPension = d.idPension
										LEFT JOIN concesionarios as C ON C.idConcesionario = d.IdConcesionario
                                        WHERE d.idPension =-@idPension AND 
										d.estatusSolicitud >= 3";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idPension", SqlDbType.Int)).Value = idPension;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            BusquedaDepositoModel deposito = new BusquedaDepositoModel();
                            deposito.idDeposito = reader["idDeposito"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idDeposito"].ToString());
                            deposito.fechaEvento = reader["fechaSolicitud"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaSolicitud"].ToString());
                            deposito.folioInfraccion = reader["folioInfraccion"].ToString();
                            deposito.placa = reader["placa"].ToString();
                            deposito.nombre = reader["nombre"].ToString();
                            deposito.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            deposito.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            deposito.fechaIngreso = reader["fechaIngreso"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaIngreso"].ToString());
                            deposito.fechaSalida = reader["fechaSalida"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaSalida"].ToString());
                            deposito.pension = reader["pension"].ToString();
                            deposito.grua = reader["concesionario"].ToString();

                            modelList.Add(deposito);
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
            }
            return modelList;

        }
        public List<BusquedaDepositoModel> ObtenerDepositos(BusquedaDepositoModel model, int idPension)
        {
            List<BusquedaDepositoModel> modelList = new List<BusquedaDepositoModel>();
            string condiciones = "";                      
            condiciones += model.folioInfraccion.IsNullOrEmpty() ? "" : " AND inf.folioInfraccion LIKE '%' + @folioInfraccion + '%' ";
            condiciones += model.placa.IsNullOrEmpty() ? "" : " AND d.placa = @placa";
            condiciones += model.serie.IsNullOrEmpty() ? "" : " AND d.serie = @serie";



            if (model.fechaIngreso != DateTime.MinValue)
            {
                condiciones += " and d.fechaIngreso between @fechaIngresoIn   and  @fechaIngresoFn ";

            };
            condiciones += model.propietario.IsNullOrEmpty() ? "" : " AND (p.nombre + isnull(' '+p.apellidoPaterno,'')+ isnull(' '+p.apellidoMaterno,'')  like '%'+ @nombre + '%'  )";

            string strQuery = @"SELECT d.idDeposito,d.idSolicitud,d.idInfraccion,d.idVehiculo,d.fechaIngreso,
                                        d.idPension,d.idGrua,sol.fechaSolicitud,inf.folioInfraccion,d.placa,
                                        p.nombre,p.apellidoPaterno,p.apellidoMaterno,sd.fechaSalida,C.concesionario,
                                        pen.pension
                                        From depositos AS d
                                        LEFT JOIN solicitudes AS sol ON sol.idSolicitud = d.idSolicitud
                                        LEFT JOIN infracciones AS inf ON inf.idInfraccion = d.idInfraccion
                                        
                                        LEFT JOIN opeDepositosVehiculos v ON v.idOperacion = 
										(
											SELECT MAX(idOperacion) 
											FROM opeDepositosVehiculos z 
											WHERE z.idVehiculo = d.idVehiculo and z.idDeposito = d.idDeposito
										)

                                        LEFT JOIN opeDepositosPersonas p ON p.idOperacion = 
                                        (
                                        SELECT MAX(idOperacion) 
                                        FROM opeDepositosPersonas z 
                                        WHERE z.idPersona = v.idPersona and z.idDeposito = d.idDeposito
                                        )


                                        LEFT JOIN serviciosDepositos AS sd ON sd.idDeposito = d.idDeposito
                                        LEFT JOIN pensiones AS pen ON pen.idPension = d.idPension
										LEFT JOIN concesionarios as C ON C.idConcesionario = d.IdConcesionario
                                        WHERE d.idPension = @idPension AND d.estatusSolicitud >= 3 and d.estatus=1 " + condiciones;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    if (model.fechaIngreso != DateTime.MinValue)
                    {
                        var ffin = model.fechaIngreso.AddDays(1);
                        command.Parameters.Add(new SqlParameter("@fechaIngresoIn", SqlDbType.DateTime)).Value = model.fechaIngreso;
                        command.Parameters.Add(new SqlParameter("@fechaIngresoFn", SqlDbType.DateTime)).Value = ffin;

                    }
                    command.Parameters.Add(new SqlParameter("@folioInfraccion", SqlDbType.VarChar)).Value = (object)model.folioInfraccion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@placa", SqlDbType.VarChar)).Value = (object)model.placa ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@serie", SqlDbType.VarChar)).Value = (object)model.serie ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@nombre", SqlDbType.VarChar)).Value = (object)model.propietario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idPension", SqlDbType.VarChar)).Value = (object)idPension ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            try
                            {
                                BusquedaDepositoModel deposito = new BusquedaDepositoModel();
                                deposito.idDeposito = reader["idDeposito"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idDeposito"].ToString());
                                deposito.fechaEvento = reader["fechaSolicitud"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaSolicitud"].ToString());
                                deposito.folioInfraccion = reader["folioInfraccion"].ToString();
                                deposito.placa = reader["placa"].ToString();
                                deposito.nombre = reader["nombre"].ToString();
                                deposito.apellidoPaterno = reader["apellidoPaterno"].ToString();
                                deposito.apellidoMaterno = reader["apellidoMaterno"].ToString();
                                deposito.fechaIngreso = reader["fechaIngreso"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaIngreso"].ToString());
                                deposito.fechaSalida = reader["fechaSalida"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaSalida"].ToString());
                                deposito.pension = reader["pension"].ToString();
                                deposito.grua = reader["concesionario"].ToString();
                                modelList.Add(deposito);

                            }
                            catch (Exception e)
                            {
                                var t = e;
                            }





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
            }
            return modelList;

        }
        public BusquedaDepositoModel ObtenerDetalles(int idDeposito)
        {
            BusquedaDepositoModel gruas = new BusquedaDepositoModel();
        string strQuery = @"SELECT d.idDeposito,d.idSolicitud,d.idInfraccion,d.idVehiculo,d.fechaIngreso,
                                    d.idPension,d.idGrua,sol.fechaSolicitud,inf.folioInfraccion,v.placas,
                                    p.nombre,p.apellidoPaterno,p.apellidoMaterno,sd.fechaSalida,ga.idGrua,g.noEconomico,
                                    pen.pension

                                    From depositos AS d
                                    LEFT JOIN solicitudes AS sol ON sol.idSolicitud = d.idSolicitud
                                    LEFT JOIN infracciones AS inf ON inf.idInfraccion = d.idInfraccion
                                    
                                    LEFT JOIN opeDepositosVehiculos v ON v.idOperacion = 
										(
											SELECT MAX(idOperacion) 
											FROM opeDepositosVehiculos z 
											WHERE z.idVehiculo = d.idVehiculo and z.idDeposito = d.idDeposito
										)

                                    LEFT JOIN personas AS p ON p.idPersona = v.idPersona
                                    LEFT JOIN serviciosDepositos AS sd ON sd.idDeposito = d.idDeposito
                                    LEFT JOIN gruasAsignadas AS ga ON ga.idGrua = d.idGrua
                                    LEFT JOIN gruas AS g ON g.idGrua = ga.idGrua
                                    LEFT JOIN pensiones AS pen ON pen.idPension = d.idPension
                                    WHERE inf.folioInfraccion = @folioInfraccion OR p.nombre LIKE '%' + @propietario + '%'
                                   OR p.apellidoPaterno LIKE '%' + @propietario + '%'
                                   OR p.apellidoMaterno LIKE '%' + @propietario + '%'
                                   OR placa LIKE '%' + @placa + '%'";
                   using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                    {
                     try
                        {
                        connection.Open();
                        SqlCommand command = new SqlCommand(strQuery, connection);
                        command.CommandType = CommandType.Text;
                            
                        
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                            gruas.idDeposito = reader["idDeposito"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idDeposito"].ToString());
                               
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
                     }
                 return gruas;

           }

    }
}

