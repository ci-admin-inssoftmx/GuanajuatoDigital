using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;

namespace GuanajuatoAdminUsuarios.Services
{
    public class LiberacionVehiculoService : ILiberacionVehiculoService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public LiberacionVehiculoService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<LiberacionVehiculoModel> GetAllTopDepositos(int idOficina)
        {
            List<LiberacionVehiculoModel> depositosList = new List<LiberacionVehiculoModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact =
						@"select   top(100) d.IdDeposito,d.IdSolicitud,d.idDelegacion,d.IdMarca,d.IdSubmarca,d.IdPension,d.IdTramo,
                            d.IdColor,d.Serie,d.Placa,d.FechaIngreso,d.Folio,d.Km,d.Liberado,d.Autoriza,d.FechaActualizacion,
                            d.ActualizadoPor, d.estatus,sol.idCarreteraUbicacion, sol.solicitanteNombre,
                            sol.solicitanteAp,sol.solicitanteAm,pen.pension,del.delegacion,col.color,car.carretera,
                            cTra.tramo, m.marcaVehiculo	,subm.nombreSubmarca,v.idPersona,p.nombre,p.apellidoPaterno,p.apellidoMaterno
                            from depositos d 
                            left join solicitudes sol on d.idSolicitud = sol.idSolicitud
                            left join pensiones pen on d.idPension	= pen.idPension
                            left join catDelegaciones del on d.idDelegacion= del.idDelegacion
                            left join catColores col on d.idColor = col.idColor
                            left join catCarreteras car on car.idCarretera = sol.idCarreteraUbicacion

                            left join catTramos cTra  on d.Idtramo=cTra.idTramo
                            left join catMarcasVehiculos m on d.idMarca=m.idMarcaVehiculo
                            left join catSubmarcasVehiculos  subm on d.idSubmarca=subm.idSubmarca
                            left join vehiculos v ON v.placas = d.Placa
                            left join personas p ON p.idPersona = v.idPersona
                            where d.liberado=0 and d.estatus=1 and d.idDelegacion = @idOficina
                            ORDER BY d.fechaActualizacion DESC";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            LiberacionVehiculoModel deposito = new LiberacionVehiculoModel();
                            deposito.IdDeposito = reader["IdDeposito"] is int idDeposito ? idDeposito : 0;
                            deposito.IdSolicitud = reader["IdSolicitud"] is int idSolicitud ? idSolicitud : 0;
                            deposito.IdDelegacion = reader["IdDelegacion"] is int idDelegacion ? idDelegacion : 0;
                            deposito.IdMarca = reader["IdMarca"] is int idMarca ? idMarca : 0;
                            deposito.IdSubmarca = reader["IdSubmarca"] is int idSubmarca ? idSubmarca : 0;
                            deposito.IdPension = reader["IdPension"] is int idPension ? idPension : 0;
                            deposito.IdTramo = reader["IdTramo"] is int idTramo ? idTramo : 0;
                            deposito.IdColor = reader["IdColor"] is int idColor ? idColor : 0;

                            deposito.Serie = reader["Serie"]?.ToString();
                            deposito.Placa = reader["Placa"]?.ToString();
                            deposito.Km = reader["Km"]?.ToString();
							deposito.carretera = reader["carretera"]?.ToString();

							deposito.Liberado = reader["Liberado"] is int liberado ? liberado : 0;

                            deposito.FechaIngreso = reader["FechaIngreso"] is DateTime fechaIngreso ? fechaIngreso : DateTime.MinValue;
                            deposito.Folio = reader["Folio"]?.ToString();
                            deposito.Autoriza = reader["Autoriza"]?.ToString();

                         

                            deposito.marcaVehiculo = reader["marcaVehiculo"]?.ToString();
                            deposito.nombreSubmarca = reader["nombreSubmarca"]?.ToString();
                            deposito.delegacion = reader["delegacion"]?.ToString();
                            deposito.solicitanteNombre = reader["solicitanteNombre"]?.ToString();
                            deposito.solicitanteAp = reader["solicitanteAp"]?.ToString();
                            deposito.solicitanteAm = reader["solicitanteAm"]?.ToString();
                            deposito.Color = reader["Color"]?.ToString();
                            deposito.pension = reader["pension"]?.ToString();
                            deposito.tramo = reader["tramo"]?.ToString();
                            deposito.nombrePropietario = reader["nombre"]?.ToString()??"Se Ignora";
                            deposito.apPaternoPropietario = reader["apellidoPaterno"]?.ToString();
                            deposito.apMaternoPropietario = reader["apellidoMaterno"]?.ToString();
							deposito.carretera = reader["carretera"]?.ToString();

							depositosList.Add(deposito);
                           
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
            return depositosList;
        }

        public List<LiberacionVehiculoModel> GetDepositos(LiberacionVehiculoBusquedaModel model,int idOficina)
        {
            List<LiberacionVehiculoModel> depositosList = new List<LiberacionVehiculoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                   // string condicionFecha = model.FechaIngreso == DateTime.MinValue ? @"d.FechaIngreso >= @FechaIngreso " : @"d.FechaIngreso = @FechaIngreso ";

                    connection.Open();
                    string condiciones = "";
                    condiciones += model.Placas.IsNullOrEmpty() ? "" : " AND d.Placa LIKE '%' + @Placa + '%' ";
                    condiciones += model.IdMarcaVehiculo.Equals(null) || model.IdMarcaVehiculo == 0 ? "" : " AND d.IdMarca = @IdMarca ";
                    condiciones += model.Serie.IsNullOrEmpty() ? "" : " AND d.Serie = @Serie";
                    condiciones += model.Folio.IsNullOrEmpty() ? "" : " AND d.Folio LIKE '%' + @Folio + '%' ";
                    if (model.FechaIngreso.HasValue && model.FechaIngreso != DateTime.MinValue)
                    {
                        condiciones +=(" AND CAST(d.FechaIngreso AS DATE) = @FechaIngreso ");
                    }
                    if (string.IsNullOrEmpty(condiciones.Trim()))
                    {
                        condiciones = "";
                    }

                    string SqlTransact = @"WITH opeVehiculos AS (
    SELECT
        z.idVehiculo,
        z.idDeposito,
        MAX(z.idOperacion) AS idOperacion
    FROM
        opeDepositosVehiculos z
    GROUP BY
        z.idVehiculo,
        z.idDeposito
),
ultimaOperacion AS (
    SELECT
        v.idVehiculo,
        v.idDeposito,
        v.idColor,
        v.idPersona
    FROM
        opeDepositosVehiculos v
    JOIN opeVehiculos o ON
        v.idVehiculo = o.idVehiculo
        AND v.idDeposito = o.idDeposito
        AND v.idOperacion = o.idOperacion
)
SELECT
    d.IdDeposito,
    MAX(d.IdSolicitud) AS IdSolicitud,
    MAX(d.idDelegacion) AS idDelegacion,
    MAX(d.IdMarca) AS IdMarca,
    MAX(d.IdSubmarca) AS IdSubmarca,
    MAX(d.IdPension) AS IdPension,
    MAX(d.IdTramo) AS IdTramo,
    MAX(d.IdColor) AS IdColor,
    MAX(d.Serie) AS Serie,
    d.Placa,
    MAX(d.FechaIngreso) AS FechaIngreso,
    MAX(sol.idCarreteraUbicacion) AS idCarreteraUbicacion,
    MAX(d.Folio) AS Folio,
    MAX(d.Km) AS Km,
    MAX(d.Liberado) AS Liberado,
    MAX(d.Autoriza) AS Autoriza,
    MAX(car.carretera) AS carretera,
    MAX(d.FechaActualizacion) AS FechaActualizacion,
    MAX(d.ActualizadoPor) AS ActualizadoPor,
    MAX(d.estatus) AS estatus,
    sol.solicitanteNombre,
    sol.solicitanteAp,
    sol.solicitanteAm,
    sol.folio,
    pen.pension,
    del.delegacion,
    ISNULL(col.color, MAX(col2.color)) AS color,
    cTra.tramo,
    m.marcaVehiculo,
    subm.nombreSubmarca,
    v.idPersona,
    MAX(per.nombre) AS nombre,
    MAX(per.apellidoPaterno) AS apellidoPaterno,
    MAX(per.apellidoMaterno) AS apellidoMaterno
FROM
    depositos d
LEFT JOIN solicitudes sol ON d.idSolicitud = sol.idSolicitud
LEFT JOIN pensiones pen ON d.idPension = pen.idPension
LEFT JOIN catDelegaciones del ON d.idDelegacion = del.idDelegacion
LEFT JOIN catColores col2 ON d.idColor = col2.idColor
LEFT JOIN catTramos cTra ON d.Idtramo = cTra.idTramo
LEFT JOIN catMarcasVehiculos m ON d.idMarca = m.idMarcaVehiculo
LEFT JOIN catSubmarcasVehiculos subm ON d.idSubmarca = subm.idSubmarca
LEFT JOIN ultimaOperacion v ON v.idVehiculo = d.idVehiculo AND v.idDeposito = d.idDeposito
LEFT JOIN catColores col ON v.idColor = col.idColor
LEFT JOIN personas p ON p.idPersona = d.idPropietario
LEFT JOIN catCarreteras car ON car.idCarretera = sol.idCarreteraUbicacion
LEFT JOIN opeDepositosPersonas per ON per.idOperacion =(SELECT MAX(idOperacion) FROM opeDepositosPersonas z WHERE z.idPersona = d .idPropietario AND z.idDeposito = d .idDeposito)
WHERE
    d.liberado = 0 AND d.estatusSolicitud = 4 AND d.idDelegacion = @idOficina " + condiciones + @"
GROUP BY
    d.IdDeposito,
    d.Placa,
    sol.solicitanteNombre,
    sol.solicitanteAp,
    sol.solicitanteAm,
    sol.folio,
    pen.pension,
    del.delegacion,
	col.color,
    cTra.tramo,
    m.marcaVehiculo,
    subm.nombreSubmarca,
    v.idPersona; ";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.CommandTimeout = 90;

                    command.Parameters.Add(new SqlParameter("@Placa", SqlDbType.NVarChar)).Value = (object)model.Placas ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdMarca", SqlDbType.Int)).Value = (object)model.IdMarcaVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Serie", SqlDbType.NVarChar)).Value = (object)model.Serie ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@FechaIngreso", SqlDbType.Date)).Value = (object)model.FechaIngreso ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Folio", SqlDbType.NVarChar)).Value = (object)model.Folio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            LiberacionVehiculoModel deposito = new LiberacionVehiculoModel();
                            deposito.IdDeposito = reader["IdDeposito"] is DBNull ? 0 : Convert.ToInt32(reader["IdDeposito"]);
                            deposito.IdSolicitud = reader["IdSolicitud"] is DBNull ? 0 : Convert.ToInt32(reader["IdSolicitud"]);
                            deposito.IdDelegacion = reader["IdDelegacion"] is DBNull ? 0 : Convert.ToInt32(reader["IdDelegacion"]);
                            deposito.IdMarca = reader["IdMarca"] is DBNull ? 0 : Convert.ToInt32(reader["IdMarca"]);
                            deposito.IdSubmarca = reader["IdSubmarca"] is DBNull ? 0 : Convert.ToInt32(reader["IdSubmarca"]);
                            deposito.IdPension = reader["IdPension"] is DBNull ? 0 : Convert.ToInt32(reader["IdPension"]);
                            deposito.IdTramo = reader["IdTramo"] is DBNull ? 0 : Convert.ToInt32(reader["IdTramo"]);
                            deposito.IdColor = reader["IdColor"] is DBNull ? 0 : Convert.ToInt32(reader["IdColor"]);
                            deposito.Serie = reader["Serie"] is DBNull ? string.Empty : reader["Serie"].ToString();
                            deposito.Placa = reader["Placa"] is DBNull ? string.Empty : reader["Placa"].ToString();
                            deposito.FechaIngreso = reader["FechaIngreso"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(reader["FechaIngreso"]);
                            deposito.Folio = reader["Folio"] is DBNull ? string.Empty : reader["Folio"].ToString();
                            deposito.Km = reader["Km"] is DBNull || string.IsNullOrEmpty(reader["Km"].ToString()) ? string.Empty : Decimal.Parse((string)reader["Km"]).ToString("G29");
                            deposito.Liberado = reader["Liberado"] is DBNull ? 0 : Convert.ToInt32(reader["Liberado"]);
                            deposito.Autoriza = reader["Autoriza"] is DBNull ? string.Empty : reader["Autoriza"].ToString();
                            deposito.FechaActualizacion = reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(reader["FechaActualizacion"]);
                            deposito.ActualizadoPor = reader["ActualizadoPor"] is DBNull ? 0 : Convert.ToInt32(reader["ActualizadoPor"]);
                            deposito.Estatus = reader["Estatus"] is DBNull ? 0 : Convert.ToInt32(reader["Estatus"]);
                            deposito.marcaVehiculo = reader["marcaVehiculo"] is DBNull ? string.Empty : reader["marcaVehiculo"].ToString();
                            deposito.nombreSubmarca = reader["nombreSubmarca"] is DBNull ? string.Empty : reader["nombreSubmarca"].ToString();
                            deposito.delegacion = reader["delegacion"] is DBNull ? string.Empty : reader["delegacion"].ToString();
                            deposito.solicitanteNombre = reader["solicitanteNombre"] is DBNull ? string.Empty : reader["solicitanteNombre"].ToString();
                            deposito.solicitanteAp = reader["solicitanteAp"] is DBNull ? string.Empty : reader["solicitanteAp"].ToString();
                            deposito.solicitanteAm = reader["solicitanteAm"] is DBNull ? string.Empty : reader["solicitanteAm"].ToString();
                            deposito.Color = reader["Color"] is DBNull ? string.Empty : reader["Color"].ToString();
                            deposito.pension = reader["pension"] is DBNull ? string.Empty : reader["pension"].ToString();
                            deposito.carretera = reader["carretera"] is DBNull ? string.Empty : reader["carretera"].ToString();
                            deposito.tramo = reader["tramo"] is DBNull ? string.Empty : reader["tramo"].ToString();
                            deposito.nombrePropietario = reader["nombre"]?.ToString()??"Se Ignora";
                            deposito.apPaternoPropietario = reader["apellidoPaterno"]?.ToString()??"";
                            deposito.apMaternoPropietario = reader["apellidoMaterno"]?.ToString()??"";
                            depositosList.Add(deposito);
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
            return depositosList;
        }

        public LiberacionVehiculoModel GetDepositoByID(int Id, int idOficina)
        {
            LiberacionVehiculoModel deposito = new LiberacionVehiculoModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact =
                        @"select 
                        d.IdDeposito,
                        d.IdSolicitud,
                        d.idDelegacion,
                        d.IdMarca,
                        d.IdSubmarca,
                        d.IdPension,
                        d.IdTramo,
		                d.IdColor,
                        d.Serie,
                        v.placas,
                        d.FechaIngreso,d.Folio,d.Km,d.Liberado,d.Autoriza,d.FechaActualizacion,
		                del.delegacion, d.ActualizadoPor, d.estatus, m.marcaVehiculo,subm.nombreSubmarca, sol.solicitanteNombre,
						sol.solicitanteAp,sol.solicitanteAm,sol.idCarreteraUbicacion,car.carretera,col.color,pen.pension, cTra.tramo,
                                            v.idPersona,
                                            p.nombre,
                                            p.apellidoPaterno,
                                            p.apellidoMaterno
		                from depositos d left join catDelegaciones del on d.idDelegacion= del.idDelegacion
			
                        LEFT JOIN opeDepositosVehiculos v ON v.idOperacion = 
											(
											   SELECT MAX(idOperacion) 
											   FROM opeDepositosVehiculos z 
											   WHERE z.placas = d.Placa and z.idDeposito = @IdDeposito
											)

		                left join catMarcasVehiculos m on d.idMarca=m.idMarcaVehiculo
		                left join catSubmarcasVehiculos  subm on d.idSubmarca=subm.idSubmarca
						left join solicitudes sol on d.idSolicitud = sol.idSolicitud
						left join catColores col on v.idColor = col.idColor
	                    left join pensiones pen on d.idPension	= pen.idPension
                        left join catTramos cTra  on d.Idtramo=cTra.idTramo
                        left join catCarreteras car on car.idCarretera = sol.idCarreteraUbicacion
                        LEFT JOIN opeDepositosPersonas p ON p.idPersona = d.idPropietario and p.idDeposito=d.idDeposito
		                where d.liberado=0 and d.estatus=1 and d.IdDeposito=@IdDeposito and d.idDelegacion = @idOficina";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@IdDeposito", SqlDbType.Int)).Value = (object)Id ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            deposito.IdDeposito = reader["IdDeposito"] as int? ?? 0;
                            deposito.IdSolicitud = reader["IdSolicitud"] as int? ?? 0;
                            deposito.IdDelegacion = reader["IdDelegacion"] as int? ?? 0;
                            deposito.IdMarca = reader["IdMarca"] as int? ?? 0;
                            deposito.IdSubmarca = reader["IdSubmarca"] as int? ?? 0;
                            deposito.IdPension = reader["IdPension"] as int? ?? 0;
                            deposito.IdTramo = reader["IdTramo"] is DBNull ?0: (int)reader["IdTramo"];
                            deposito.IdColor = reader["IdColor"] is DBNull?0 : (int)reader["IdColor"];
                            deposito.Serie = reader["Serie"]?.ToString();
                            deposito.Placa = reader["Placas"]?.ToString();
                            deposito.FechaIngreso = reader["FechaIngreso"] as DateTime? ?? DateTime.MinValue;
                            deposito.Folio = reader["Folio"]?.ToString();
                            deposito.Km = reader["Km"] is DBNull || string.IsNullOrEmpty(reader["Km"].ToString()) ? string.Empty : Decimal.Parse((string)reader["Km"]).ToString("G29");
                            deposito.Liberado = reader["Liberado"] as int? ?? 0;
                            deposito.Autoriza = reader["Autoriza"]?.ToString();
                            deposito.FechaActualizacion = reader["FechaActualizacion"] as DateTime? ?? DateTime.MinValue;
                            deposito.ActualizadoPor = reader["ActualizadoPor"] as int? ?? 0;
                            deposito.Estatus = reader["Estatus"] as int? ?? 0;
                            deposito.marcaVehiculo = reader["marcaVehiculo"]?.ToString();
                            deposito.nombreSubmarca = reader["nombreSubmarca"]?.ToString();
                            deposito.delegacion = reader["delegacion"]?.ToString();
                            deposito.solicitanteNombre = reader["nombre"]?.ToString(); //reader["solicitanteNombre"]?.ToString();
                            deposito.solicitanteAp = reader["apellidoPaterno"]?.ToString();
                            deposito.solicitanteAm = reader["apellidoMaterno"]?.ToString();
                            deposito.Color = reader["Color"]?.ToString();
                            deposito.pension = reader["pension"]?.ToString();
                            deposito.tramo = reader["tramo"]?.ToString();
                            deposito.carretera = reader["carretera"]?.ToString();
                            deposito.nombrePropietario = reader["nombre"]?.ToString() ?? "Se Ignora - -";
                            deposito.apPaternoPropietario = reader["apellidoPaterno"]?.ToString();
                            deposito.apMaternoPropietario = reader["apellidoMaterno"]?.ToString();

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
            return deposito;
        }

        public LiberacionVehiculoModel GetDepositoRutaFormatoManual(int Id)
        {
            LiberacionVehiculoModel deposito = new LiberacionVehiculoModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    const string SqlTransact =
                        @"SELECT d.rutaFormatoManual
                  FROM depositos d
                  WHERE d.IdDeposito = @IdDeposito";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@IdDeposito", SqlDbType.Int)).Value = (object)Id ?? DBNull.Value;

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            deposito.FormatoManualPath = reader["rutaFormatoManual"]?.ToString();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Manejar excepciones de SQL si es necesario
                }
                finally
                {
                    connection.Close();
                }
            }
            return deposito;
        }



        public int UpdateDeposito(LiberacionVehiculoModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    const string SqlTransact =
                        @"Update depositos set 
                          archivoAcreditacionPropiedad=@acreditacionPropiedadStr,
                          archivoPersonalidad=@acreditacionPersonalidadStr,
                          archivoReciboPago=@ReciboPagoStr,
                          rutaAcreditacionPropiedad=@rutaAcreditacionPropiedad,
                          rutaAcreditacionPersonalidad=@rutaAcreditacionPersonalidad,
                          rutaFormatoManual=@rutaFormatoManual,
                          rutaReciboPago=@rutaReciboPago,
                          Observaciones=@Observaciones,
                          observacionesPropiedad=@ObservacionesPropiedad,
                          observacionesInfraccion=@ObservacionesInfraccion,
                          observacionesPersonalidad = @ObservacionesPersonalidad,
                          Autoriza=@Autoriza,Liberado=@liberado,FechaActualizacion=@FechaActualizacion,
                          FechaLiberacion=@FechaLiberacion, 
                          estatusSolicitud = case when estatusSolicitud > 5 then estatusSolicitud else 5 end
                          where IdDeposito=@IdDeposito";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@IdDeposito", SqlDbType.Int)).Value = (object)model.IdDeposito ?? DBNull.Value;
                    //command.Parameters.Add(new SqlParameter("@AcreditacionPropiedad", SqlDbType.Image)).Value = (object)model.AcreditacionPropiedad ?? DBNull.Value;
                    //command.Parameters.Add(new SqlParameter("@AcreditacionPersonalidad", SqlDbType.Image)).Value = (object)model.AcreditacionPersonalidad ?? DBNull.Value;
                    //command.Parameters.Add(new SqlParameter("@ReciboPago", SqlDbType.Image)).Value = (object)model.ReciboPago ?? DBNull.Value;

                    //RUTAS
                    command.Parameters.Add(new SqlParameter("@rutaAcreditacionPropiedad", SqlDbType.Text)).Value = (object)model.ImageAcreditacionPropiedadPath ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@rutaAcreditacionPersonalidad", SqlDbType.Text)).Value = (object)model.ImageAcreditacionPersonalidadPath ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@rutaReciboPago", SqlDbType.Text)).Value = (object)model.ImageReciboPagoPath ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@rutaFormatoManual", SqlDbType.Text)).Value = (object)model.FormatoManualPath ?? DBNull.Value;
                    //NOMBRES
                    command.Parameters.Add(new SqlParameter("@acreditacionPropiedadStr", SqlDbType.Text)).Value = (object)model.AcreditacionPropiedadStr ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@acreditacionPersonalidadStr", SqlDbType.Text)).Value = (object)model.AcreditacionPersonalidadStr ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@ReciboPagoStr", SqlDbType.Text)).Value = (object)model.ReciboPagoStr ?? DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@Observaciones", SqlDbType.Text)).Value = (object)model.Observaciones.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@ObservacionesPropiedad", SqlDbType.Text)).Value = (object)model.ObservacionesPropiedad.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@ObservacionesInfraccion", SqlDbType.Text)).Value = (object)model.ObservacionesInfraccion.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@ObservacionesPersonalidad", SqlDbType.Text)).Value = (object)model.ObservacionesPersonalidad.ToUpper() ?? DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@Autoriza", SqlDbType.NVarChar)).Value = (object)model.Autoriza.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Liberado", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@FechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    command.Parameters.Add(new SqlParameter("@FechaLiberacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    command.CommandType = CommandType.Text;
                    result = command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return result;
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
