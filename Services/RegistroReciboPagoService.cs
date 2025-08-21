using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using System.Collections.Generic;
using System.Data;
using System;
using GuanajuatoAdminUsuarios.Models;
using System.Data.SqlClient;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;

namespace GuanajuatoAdminUsuarios.Services
{
    public class RegistroReciboPagoService : IRegistroReciboPagoService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        private readonly IInfraccionesService _infraccionesService;
        public RegistroReciboPagoService(ISqlClientConnectionBD sqlClientConnectionBD, IInfraccionesService infraccionesService)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
            _infraccionesService = infraccionesService;
        }

        public List<RegistroReciboPagoModel> ObtInfracciones(string FolioInfraccion , string corp)
        {
            //
            List<RegistroReciboPagoModel> ListaInfracciones = new List<RegistroReciboPagoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT i.*, opv.placas,v.serie, pveh.nombre AS nombre1, pveh.apellidoPaterno AS apellidoPaterno1,
                                                            pveh.apellidoMaterno AS apellidoMaterno1, pinf.nombre AS nombre2, 
                                                            pinf.apellidoPaterno AS apellidoPaterno2, pinf.apellidoMaterno AS apellidoMaterno2,
                                                            e.estatusInfraccion, cde.delegacion,v.serie
                                                        FROM infracciones AS i 
														INNER JOIN opeInfraccionesVehiculos opv on opv.idOperacion =(select top 1 idOperacion from opeInfraccionesVehiculos iv where iv.idInfraccion = i.idInfraccion and iv.idVehiculo =i.idVehiculo order by iv.fechaActualizacion desc)
                                                        LEFT JOIN catEstatusInfraccion AS e ON i.idEstatusInfraccion = e.idEstatusInfraccion
                                                        LEFT JOIN vehiculos AS v ON v.idVehiculo = i.idVehiculo
                                                        LEFT JOIN personas AS pveh ON pveh.idPersona = v.idPersona
                                                        LEFT JOIN personas AS pinf ON pinf.idPersona = i.idPersonaConductor
                                                        LEFT JOIN catDelegaciones AS cde ON cde.idDelegacion = i.idDelegacion 
                                                        WHERE folioInfraccion like '%'+@FolioInfraccion+'%' and  e.estatusInfraccion in('Capturada','En proceso','Enviada') and i.transito = @corp and i.estatus=1", connection);

                    command.Parameters.Add(new SqlParameter("@FolioInfraccion", SqlDbType.NVarChar)).Value = FolioInfraccion??"";
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = Convert.ToInt32(corp);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            RegistroReciboPagoModel infraccion = new RegistroReciboPagoModel();
                            infraccion.IdInfraccion = Convert.ToInt32(reader["IdInfraccion"].ToString());
                            infraccion.FolioInfraccion = reader["folioInfraccion"].ToString();
                            infraccion.Placas = reader["placas"].ToString();
                            infraccion.FechaInfraccion = Convert.ToDateTime(reader["FechaInfraccion"].ToString());
                            infraccion.Propietario = $"{reader["nombre1"]} {reader["apellidoPaterno1"]} {reader["apellidoMaterno1"]}";
                            infraccion.Serie = reader["Serie"] is DBNull ? string.Empty : reader["Serie"].ToString();
                            infraccion.Conductor = $"{reader["nombre2"]} {reader["apellidoPaterno2"]} {reader["apellidoMaterno2"]}";
                            infraccion.Delegacion = reader["delegacion"].ToString();
                            infraccion.EstatusInfraccion = reader["estatusInfraccion"].ToString();

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

        public RegistroReciboPagoModel ObtenerDetallePorId(int Id)
        {

            RegistroReciboPagoModel infraccion = new RegistroReciboPagoModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT i.idInfraccion,i.folioInfraccion,monto
                                                            FROM infracciones i
                                                            WHERE i.idInfraccion = @Id
                                                        ;", connection);
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int)).Value = Id;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            infraccion.IdInfraccion = Convert.ToInt32(reader["IdInfraccion"].ToString());
                            infraccion.FolioInfraccion = reader["FolioInfraccion"].ToString();
                            infraccion.Monto = (reader["monto"] != DBNull.Value) ? float.Parse(reader["monto"].ToString()) : 0.0f;

                            if (reader["monto"] != DBNull.Value)
                            {

                                if (float.TryParse(reader["monto"].ToString(), out float montoFloat))
                                {
                                    infraccion.MontoSTR = montoFloat.ToString("#,##0.00");

                                }
                                else
                                {
                                    infraccion.MontoSTR = "0.00"; // O cualquier otro valor predeterminado si la conversión falla
                                }
                            }
                        }

                    }

                    SqlCommand command1 = new SqlCommand(@"declare 
                                                            @date datetime,
                                                            @calificacion decimal,
                                                            @uma float,
                                                            @year int;

                                                        select @date = fechaInfraccion from infracciones where idinfraccion = @Id;

                                                        select @calificacion = sum(calificacion) from motivosInfraccion where idInfraccion = @Id;

                                                        set @year = year(@date);

SELECT top 1 @uma=format(salario,'#.##') 
                               FROM catSalariosMinimos
                               WHERE estatus = 1 and fecha<= @date  order by fecha desc, idSalario asc
                                                        select (@calificacion * @uma)    as monto , @calificacion cal , @uma uma  ;
", connection);
                    command1.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int)).Value = Id;
                    command1.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command1.ExecuteReader())
                    {
                        while (reader.Read())
                        {


                            /*if (reader["monto"] != DBNull.Value)
                            {



                                if (float.TryParse(reader["monto"].ToString(), out float montoFloat))
                                {
                                    infraccion.MontoSTR = montoFloat.ToString("#,##0.00");

                                }
                                else
                                {
                                    infraccion.MontoSTR = "0.00"; // O cualquier otro valor predeterminado si la conversión falla
                                }
                            }*/




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
        public int GuardarRecibo(string ReciboPago, float Monto, DateTime FechaPago, string LugarPago, int IdInfraccion, float MontoCalculado)
        {
            int infraccionModificada = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    int estatusinf = 0;

					MontoCalculado= float.Parse(MontoCalculado.ToString("0.00")); 
					Monto = float.Parse(Monto.ToString("0.00")); //monto pagado

					if (MontoCalculado== Monto)
                    {
						estatusinf = 3;
					} else if (Monto > MontoCalculado)
					{
						estatusinf = 6;
					}
					else if (Monto < MontoCalculado)
					{
						estatusinf = 4;
					}
					connection.Open();
                    string query = "UPDATE infracciones SET estatusProceso=@estatusInf , idEstatusInfraccion=@estatusInf, reciboPago = @reciboPago, montoPagado = @monto, fechaPago = @fechaPago, lugarPago = @lugarPago WHERE idInfraccion = @idInfraccion";


                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@reciboPago", ReciboPago??"");
                    command.Parameters.AddWithValue("@monto", Monto);
                    command.Parameters.AddWithValue("@fechaPago", FechaPago);
                    command.Parameters.AddWithValue("@lugarPago", LugarPago != null ? LugarPago : DBNull.Value);
                    command.Parameters.AddWithValue("@idInfraccion", IdInfraccion);
					command.Parameters.AddWithValue("@estatusInf", estatusinf);
					command.ExecuteNonQuery();
                }



                catch (SqlException ex)
                {
                    // Manejar la excepción
                }
                finally
                {
                    connection.Close();
                }
            }

            return infraccionModificada;
        }
        public bool VerificarActivo(string endPointName)
        {
            bool isActive = false;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT IsActive FROM serviceAppSettings WHERE SettingName = @endPointName", connection);
                    command.Parameters.Add(new SqlParameter("@endPointName", SqlDbType.NVarChar)).Value = endPointName;
                    command.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.Read())
                        {
                            isActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
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
            }

            return isActive;
        }

    }

}

