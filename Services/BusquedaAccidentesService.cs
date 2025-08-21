using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using System.Collections.Generic;
using System.Data;
using System;
using GuanajuatoAdminUsuarios.Models;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Runtime.ConstrainedExecution;
using System.Data.Common;
using GuanajuatoAdminUsuarios.Util;
using GuanajuatoAdminUsuarios.Models.Utils;
using DocumentFormat.OpenXml.Office.Word;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services
{
    public class BusquedaAccidentesService : IBusquedaAccidentesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public BusquedaAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<BusquedaAccidentesModel> GetAllAccidentes()
        {
            //
            List<BusquedaAccidentesModel> ListaAccidentes = new List<BusquedaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idTramo, a.idCarretera, a.idElabora, a.idSupervisa, a.idAutoriza, a.kilometro, a.idOficinaDelegacion, 
                    mun.municipio, 
                    car.carretera, 
                    tra.tramo, 
                    er.estatusReporte, 
                    a.idEstatusReporte, 
                    MAX(vea.placa) AS placa, MAX(vea.serie) AS serie, 
                    MAX(cond.idPersona) AS idConductor, 
                    MAX(vea.idPersona) AS idPropietario, 
                    ela.idOficial AS elabora, 
                    sup.idOficial AS supervisa, 
                    aut.idOficial AS autoriza, 
                    MAX(prop.nombre) AS nombre, MAX(prop.apellidoPaterno) AS apellidoPaterno, MAX(prop.apellidoMaterno) AS apellidoMaterno, 
                    MAX(cond.nombre) AS nombreConductor, MAX(cond.apellidoPaterno) AS apellidoPaternoConductor, MAX(cond.apellidoMaterno) AS apellidoMaternoConductor 
                    FROM accidentes AS a
                    LEFT JOIN vehiculosAccidente AS vea ON a.idAccidente = vea.idAccidente 

                    LEFT JOIN opeAccidentesVehiculos v ON v.idOperacion = 
										        (
											        SELECT MAX(idOperacion) 
											        FROM opeAccidentesVehiculos z 
											        WHERE z.idVehiculo = vea.idVehiculo and z.idAccidente = a.idAccidente
										        )
                    

                   LEFT JOIN conductoresVehiculosAccidente AS cva ON a.idAccidente = cva.idAccidente 
					LEFT JOIN opeConductoresVehiculosAccidentePersonas AS cond ON cva.idPersona = cond.idPersona AND cond.idAccidente = cva.idAccidente AND cond.tipoOperacion = 2 AND cva.idVehiculo = cond.idVehiculo 
                    LEFT JOIN personas AS prop ON vea.idPersona = prop.idPersona 
                    LEFT JOIN catMunicipios AS mun ON a.idMunicipio = mun.idMunicipio
                    LEFT JOIN catCarreteras AS car ON a.idCarretera = car.idCarretera 
                    LEFT JOIN catTramos AS tra ON a.idTramo = tra.idTramo 
                    LEFT JOIN catEstatusReporteAccidente AS er ON a.idEstatusReporte = er.idEstatusReporte 
                    LEFT JOIN catOficiales AS ela ON a.idElabora = ela.idOficial 
                    LEFT JOIN catOficiales AS sup ON a.idSupervisa = sup.idOficial 
                    LEFT JOIN catOficiales AS aut ON a.idAutoriza = aut.idOficial 
                    WHERE a.estatus=1 
                    GROUP BY a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idTramo, a.idCarretera, a.idElabora, a.idSupervisa,a. idAutoriza,a.kilometro,a.idOficinaDelegacion, 
                    mun.municipio, car.carretera, tra.tramo, er.estatusReporte,a.idEstatusReporte, ela.idOficial, sup.idOficial,aut.idOficial
", connection);


                    command.CommandType = CommandType.Text;
                    //command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                   // command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())


                        {
                            BusquedaAccidentesModel accidente = new BusquedaAccidentesModel();
                            accidente.IdAccidente = Convert.IsDBNull(reader["idAccidente"]) ? 0 : Convert.ToInt32(reader["idAccidente"]);
                            accidente.idMunicipio = Convert.IsDBNull(reader["idMunicipio"]) ? 0 : Convert.ToInt32(reader["idMunicipio"]);
                            accidente.idCarretera = Convert.IsDBNull(reader["idCarretera"]) ? 0 : Convert.ToInt32(reader["idCarretera"]);
                            accidente.IdDelegacionBusqueda = Convert.IsDBNull(reader["idOficinaDelegacion"]) ? 0 : Convert.ToInt32(reader["idOficinaDelegacion"]);
                            accidente.idTramo = Convert.IsDBNull(reader["idTramo"]) ? 0 : Convert.ToInt32(reader["idTramo"]);
                            accidente.kilometro = reader["kilometro"].ToString();
                            accidente.idEstatusReporte = Convert.IsDBNull(reader["idEstatusReporte"]) ? 0 : Convert.ToInt32(reader["idEstatusReporte"]);
                            accidente.estatusReporte = reader["estatusReporte"].ToString();
                            accidente.municipio = reader["municipio"].ToString();
                            accidente.carretera = reader["carretera"].ToString();
                            accidente.placa = reader["placa"].ToString();
                            accidente.serie = reader["serie"].ToString();
                            string nombrePropietario = reader["nombre"].ToString();
                            string apellidoPaternoPropietario = reader["apellidoPaterno"].ToString();
                            string apellidoMaternoPropietario = reader["apellidoMaterno"].ToString();
                            accidente.propietario = $"{nombrePropietario} {apellidoPaternoPropietario} {apellidoMaternoPropietario}";
                            string nombreConductor = reader["nombreConductor"].ToString();
                            string apellidoPaternoConductor = reader["apellidoPaternoConductor"].ToString();
                            string apellidoMaternoConductor = reader["apellidoMaternoConductor"].ToString();
                            accidente.conductor = $"{nombreConductor} {apellidoPaternoConductor} {apellidoMaternoPropietario}";
                            accidente.tramo = reader["tramo"].ToString();
                            accidente.idElabora = Convert.IsDBNull(reader["idElabora"]) ? 0 : Convert.ToInt32(reader["idElabora"]);
                            accidente.idSupervisa = Convert.IsDBNull(reader["idSupervisa"]) ? 0 : Convert.ToInt32(reader["idSupervisa"]);
                            accidente.idAutoriza = Convert.IsDBNull(reader["idAutoriza"]) ? 0 : Convert.ToInt32(reader["idAutoriza"]);
                            accidente.idConductor = Convert.IsDBNull(reader["idConductor"]) ? 0 : Convert.ToInt32(reader["idConductor"]);
                            accidente.idPropietario = Convert.IsDBNull(reader["idPropietario"]) ? 0 : Convert.ToInt32(reader["idPropietario"]);
                            accidente.numeroReporte = reader["numeroReporte"].ToString();
                            accidente.fecha = reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue;
                            accidente.hora = reader["hora"] != DBNull.Value ? TimeSpan.Parse(reader["hora"].ToString()) : TimeSpan.MinValue;
                            ListaAccidentes.Add(accidente);

                        }

                    }

                }
                catch (SqlException ex)
                {
                    Logger.Error("Error al obtener accidentes:"+ex);
                }
                finally
                {
                    connection.Close();
                }
            return ListaAccidentes;


        }
        public List<BusquedaAccidentesModel> BusquedaAccidentes(BusquedaAccidentesModel model, int idOficina)
        {
            //
            List<BusquedaAccidentesModel> ListaAccidentes = new List<BusquedaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idTramo, a.idCarretera, a.idElabora, a.idSupervisa, a.idAutoriza, a.kilometro, a.idOficinaDelegacion, " +
                        "MAX(a.estatus) AS estatus, "+                       
                        "mun.municipio, " +
                        "car.carretera, " +                       
                        "tra.tramo, " +
                        "er.estatusReporte, " +
                        "er.idEstatusReporte, " +
                        "MAX(vea.placa) AS placa, MAX(vea.serie) AS serie, " +
                        "MAX(cond.idPersona) AS idConductor, " +
					    "MAX(cond.nombre) AS nombreCond, " +
						"MAX(cond.apellidoPaterno) AS apellidoPaternoCond, " +
						"MAX(cond.apellidoMaterno) AS apellidoMaternoCond, " +
						"MAX(vea.idPersona) AS idPropietario, " +
                        "ela.idOficial AS elabora, " +
                        "sup.idOficial AS supervisa, " +
                        "aut.idOficial AS autoriza, " +
						"MAX(prop.nombre) AS nombrePropietario, " +
						"MAX(prop.apellidoPaterno) AS apellidoPaternoProp, " +
						"MAX(prop.apellidoMaterno) AS apellidoMaternoProp " +
						"FROM accidentes AS a " +
                        "LEFT JOIN vehiculosAccidente AS vea ON a.idAccidente = vea.idAccidente " +

                        @"LEFT JOIN opeAccidentesVehiculos v ON v.idOperacion = 
										        (
											        SELECT MAX(idOperacion) 
											        FROM opeAccidentesVehiculos z 
											        WHERE z.idVehiculo = vea.idVehiculo and z.idAccidente = a.idAccidente
										        )" +

                        "LEFT JOIN conductoresVehiculosAccidente AS cva ON a.idAccidente = cva.idAccidente " +
						"LEFT JOIN opeConductoresVehiculosAccidentePersonas AS cond ON cva.idPersona = cond.idPersona AND cond.idAccidente = cva.idAccidente AND cond.tipoOperacion = 2 AND cva.idVehiculo = cond.idVehiculo " +
                        "LEFT JOIN personas AS prop ON vea.idPersona = prop.idPersona " +
                        "LEFT JOIN catMunicipios AS mun ON a.idMunicipio = mun.idMunicipio " +
                        "LEFT JOIN catCarreteras AS car ON a.idCarretera = car.idCarretera " +
                        "LEFT JOIN catTramos AS tra ON a.idTramo = tra.idTramo " +
                        "LEFT JOIN catEstatusReporteAccidente AS er ON a.idEstatusReporte = er.idEstatusReporte " +
                        "LEFT JOIN catOficiales AS ela ON a.idElabora = ela.idOficial " +
                        "LEFT JOIN catOficiales AS sup ON a.idSupervisa = sup.idOficial " +
                        "LEFT JOIN catOficiales AS aut ON a.idAutoriza = aut.idOficial " +
                        "WHERE a.estatus=1 " +
                        "GROUP BY a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idTramo, a.idCarretera, a.idElabora, a.idSupervisa,a. idAutoriza,a.kilometro,a.idOficinaDelegacion, " +
                        "mun.municipio, car.carretera, tra.tramo, er.estatusReporte,er.idEstatusReporte, ela.idOficial, sup.idOficial, aut.idOficial; ", connection);


                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                   // command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())


                        {
                            BusquedaAccidentesModel accidente = new BusquedaAccidentesModel();
                            accidente.IdAccidente = Convert.IsDBNull(reader["idAccidente"]) ? 0 : Convert.ToInt32(reader["idAccidente"]);
                            accidente.idMunicipio = Convert.IsDBNull(reader["idMunicipio"]) ? 0 : Convert.ToInt32(reader["idMunicipio"]);
                            accidente.idCarretera = Convert.IsDBNull(reader["idCarretera"]) ? 0 : Convert.ToInt32(reader["idCarretera"]);
                            accidente.idTramo = Convert.IsDBNull(reader["idTramo"]) ? 0 : Convert.ToInt32(reader["idTramo"]);
                            accidente.kilometro = reader["kilometro"].ToString();
                            accidente.idEstatusReporte = Convert.IsDBNull(reader["idEstatusReporte"]) ? 0 : Convert.ToInt32(reader["idEstatusReporte"]);
                            accidente.estatusReporte = reader["estatusReporte"].ToString();
                            accidente.municipio = reader["municipio"].ToString();
                            accidente.carretera = reader["carretera"].ToString();
							accidente.nombrePropietario = reader["nombrePropietario"].ToString();
							accidente.apellidoPaternoPropietario = reader["apellidoPaternoProp"].ToString();
							accidente.apellidoMaternoPropietario = reader["apellidoMaternoProp"].ToString();
							accidente.propietario = $"{reader["nombrePropietario"]} {(string.IsNullOrEmpty(reader["apellidoPaternoProp"].ToString()) ? "" : reader["apellidoPaternoProp"].ToString() + " ")}{(string.IsNullOrEmpty(reader["apellidoMaternoProp"].ToString()) ? "" : reader["apellidoMaternoProp"].ToString())}".Trim();
							accidente.nombreConductor = reader["nombreCond"].ToString();
							accidente.apellidoPaternoConductor = reader["apellidoPaternoCond"].ToString();
							accidente.apellidoMaternoConductor = reader["apellidoMaternoCond"].ToString();
							accidente.conductor = $"{reader["nombreCond"]} {(string.IsNullOrEmpty(reader["apellidoPaternoCond"].ToString()) ? "" : reader["apellidoPaternoCond"].ToString() + " ")}{(string.IsNullOrEmpty(reader["apellidoMaternoCond"].ToString()) ? "" : reader["apellidoMaternoCond"].ToString())}".Trim();

							accidente.placa = reader["placa"].ToString();
							accidente.serie = reader["serie"].ToString();

							accidente.tramo = reader["tramo"].ToString();
                            accidente.idElabora = Convert.IsDBNull(reader["idElabora"]) ? 0 : Convert.ToInt32(reader["idElabora"]);
                            accidente.idSupervisa = Convert.IsDBNull(reader["idSupervisa"]) ? 0 : Convert.ToInt32(reader["idSupervisa"]);
                            accidente.idAutoriza = Convert.IsDBNull(reader["idAutoriza"]) ? 0 : Convert.ToInt32(reader["idAutoriza"]);
                            accidente.idConductor = Convert.IsDBNull(reader["idConductor"]) ? 0 : Convert.ToInt32(reader["idConductor"]);
                            accidente.idPropietario = Convert.IsDBNull(reader["idPropietario"]) ? 0 : Convert.ToInt32(reader["idPropietario"]);
                            accidente.numeroReporte = reader["numeroReporte"].ToString();
                            accidente.fecha = reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue;
                            accidente.hora = reader["hora"] != DBNull.Value ? TimeSpan.Parse(reader["hora"].ToString()) : TimeSpan.MinValue;
							accidente.estatus = Convert.IsDBNull(reader["estatus"]) ? 0 : Convert.ToInt32(reader["estatus"]);

							ListaAccidentes.Add(accidente);

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
            return ListaAccidentes;


        }
     
        public List<BusquedaAccidentesPDFModel> BusquedaAccidentes(BusquedaAccidentesPDFModel model, int idOficina)
        {
            //
            List<BusquedaAccidentesPDFModel> ListaAccidentes = new List<BusquedaAccidentesPDFModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idTramo, a.idCarretera, a.idElabora, a.idSupervisa, a.idAutoriza, a.kilometro, a.idOficinaDelegacion,a.estatus " +
                        "mun.municipio, " +
                        "car.carretera, " +
                        "tra.tramo, " +
                        "er.estatusReporte,er.idEstatusReporte, " +
                        "MAX(vea.placa) AS placa, MAX(vea.serie) AS serie, " +
                        "MAX(cond.idPersona) AS idConductor, " +
                        "MAX(vea.idPersona) AS idPropietario, " +
                        "ela.idOficial AS elabora, " +
                        "sup.idOficial AS supervisa, " +
                        "aut.idOficial AS autoriza " +
                        "FROM accidentes AS a " +
                        "LEFT JOIN vehiculosAccidente AS vea ON a.idAccidente = vea.idAccidente " +

                        @"LEFT JOIN opeAccidentesVehiculos v ON v.idOperacion = 
										        (
											        SELECT MAX(idOperacion) 
											        FROM opeAccidentesVehiculos z 
											        WHERE z.idVehiculo = vea.idVehiculo and z.idAccidente = a.idAccidente
										        )" +

                        "LEFT JOIN conductoresVehiculosAccidente AS cva ON a.idAccidente = cva.idAccidente " +
						"LEFT JOIN opeConductoresVehiculosAccidentePersonas AS cond ON cva.idPersona = cond.idPersona AND cond.idAccidente = cva.idAccidente AND cond.tipoOperacion = 2 AND cva.idVehiculo = cond.idVehiculo " +
                        "LEFT JOIN personas AS prop ON vea.idPersona = prop.idPersona " +
                        "LEFT JOIN catMunicipios AS mun ON a.idMunicipio = mun.idMunicipio " +
                        "LEFT JOIN catCarreteras AS car ON a.idCarretera = car.idCarretera " +
                        "LEFT JOIN catTramos AS tra ON a.idTramo = tra.idTramo " +
                        "LEFT JOIN catEstatusReporteAccidente AS er ON a.idEstatusReporte = er.idEstatusReporte " +
                        "LEFT JOIN catOficiales AS ela ON a.idElabora = ela.idOficial " +
                        "LEFT JOIN catOficiales AS sup ON a.idSupervisa = sup.idOficial " +
                        "LEFT JOIN catOficiales AS aut ON a.idAutoriza = aut.idOficial " +
                        "WHERE (vea.placa = @placasBusqueda " +
                        "OR UPPER(a.numeroReporte) = @oficioBusqueda " +
                        "OR a.idAutoriza = @idOficialBusqueda " +
                        "OR a.idSupervisa = @idOficialBusqueda " +
                        "OR a.idAutoriza = @idOficialBusqueda " +
                        "OR a.idCarretera = @idCarreteraBusqueda " +
                        "OR a.idTramo = @idTramoBusqueda " +
                        "OR prop.nombre = @propietarioBusqueda " +
                        "OR UPPER(prop.apellidoPaterno) = @propietarioBusqueda " +
                        "OR UPPER(prop.apellidoMaterno) = @propietarioBusqueda " +
                        "OR cond.nombre = @conductorBusqueda " +
                        "OR UPPER(cond.apellidoPaterno) = @conductorBusqueda " +
                        "OR UPPER(cond.apellidoMaterno) = @conductorBusqueda " +
                        "OR vea.serie = @serieBusqueda)" +
                        "AND a.idOficinaDelegacion = @idOficina AND  a.estatus != 0 " +
                       "AND ((@fechaInicio IS NULL AND @fechaFin IS NULL) OR (a.fecha BETWEEN @fechaInicio AND @fechaFin)) " +
                    "GROUP BY a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idTramo, a.idCarretera, a.idElabora, a.idSupervisa,a. idAutoriza,a.kilometro,a.idOficinaDelegacion, " +
                        "mun.municipio, car.carretera, tra.tramo, er.estatusReporte,er.idEstatusReporte, ela.idOficial, sup.idOficial, aut.idOficial; ", connection);


                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.DateTime)).Value = (model.FechaInicio == DateTime.MinValue) ? DBNull.Value : (object)model.FechaInicio;

                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.DateTime)).Value = (model.FechaFin == DateTime.MinValue) ? DBNull.Value : (object)model.FechaFin;

                    command.Parameters.Add(new SqlParameter("@oficioBusqueda", SqlDbType.NVarChar)).Value = (object)model.folio != null ? model.folioBusqueda.ToUpper() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idOficialBusqueda", SqlDbType.Int)).Value = (object)model.IdOficial ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idCarreteraBusqueda", SqlDbType.Int)).Value = (object)model.idCarretera ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTramoBusqueda", SqlDbType.Int)).Value = (object)model.idTramo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@propietarioBusqueda", SqlDbType.NVarChar)).Value = (object)model.propietario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@conductorBusqueda", SqlDbType.NVarChar)).Value = (object)model.conductor ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@placasBusqueda", SqlDbType.NVarChar)).Value = (object)model.placa ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@serieBusqueda", SqlDbType.NVarChar)).Value = (object)model.serie ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            BusquedaAccidentesPDFModel accidente = new BusquedaAccidentesPDFModel();
                            accidente.IdAccidente = Convert.IsDBNull(reader["idAccidente"]) ? 0 : Convert.ToInt32(reader["idAccidente"]);
                            accidente.idMunicipio = Convert.IsDBNull(reader["idMunicipio"]) ? 0 : Convert.ToInt32(reader["idMunicipio"]);
                            accidente.idCarretera = Convert.IsDBNull(reader["idCarretera"]) ? 0 : Convert.ToInt32(reader["idCarretera"]);
                            accidente.idTramo = Convert.IsDBNull(reader["idTramo"]) ? 0 : Convert.ToInt32(reader["idTramo"]);
                            accidente.kilometro = reader["kilometro"].ToString();
                            accidente.idEstatusReporte = Convert.IsDBNull(reader["idEstatusReporte"]) ? 0 : Convert.ToInt32(reader["idEstatusReporte"]);
                            accidente.estatusReporte = reader["estatusReporte"].ToString();
                            accidente.municipio = reader["municipio"].ToString();
                            accidente.idElabora = Convert.IsDBNull(reader["idElabora"]) ? 0 : Convert.ToInt32(reader["idElabora"]);
                            accidente.idSupervisa = Convert.IsDBNull(reader["idSupervisa"]) ? 0 : Convert.ToInt32(reader["idSupervisa"]);
                            accidente.idAutoriza = Convert.IsDBNull(reader["idAutoriza"]) ? 0 : Convert.ToInt32(reader["idAutoriza"]);
                            accidente.idConductor = Convert.IsDBNull(reader["idConductor"]) ? 0 : Convert.ToInt32(reader["idConductor"]);
                            accidente.idPropietario = Convert.IsDBNull(reader["idPropietario"]) ? 0 : Convert.ToInt32(reader["idPropietario"]);
                            accidente.numeroReporte = reader["numeroReporte"].ToString();
                            accidente.fecha = reader["fecha"] != DBNull.Value ? reader["fecha"].ToString().Split(" ")[0] : string.Empty;
                            accidente.hora = reader["hora"] != DBNull.Value ? TimeSpan.Parse(reader["hora"].ToString()) : TimeSpan.MinValue;
							accidente.estatus = Convert.IsDBNull(reader["estatus"]) ? 0 : Convert.ToInt32(reader["estatus"]);

							ListaAccidentes.Add(accidente);

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
            return ListaAccidentes;


        }
        public BusquedaAccidentesPDFModel ObtenerAccidentePorId(int idAccidente)
        {
            BusquedaAccidentesPDFModel accidente = new BusquedaAccidentesPDFModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idCarretera, a.idTramo, a.kilometro, " +
                                                         "m.municipio, c.carretera, t.tramo, e.estatusDesc,va.Idpersona AS propietario,prop.nombre AS nombreProp, prop.apellidoPaterno AS apellidoPaternoProp, prop.apellidoMaterno AS apellidoMaternoProp , " +
                                                         "cond.nombre AS nombreCond, cond.apellidoPaterno AS apellidoPaternoCond, cond.apellidoMaterno AS apellidoMaternoCond " +
                                                         "FROM accidentes AS a JOIN catMunicipios AS m ON a.idMunicipio = m.idMunicipio " +
                                                         "JOIN catCarreteras AS c ON a.idCarretera = c.idCarretera " +
                                                         "JOIN catTramos AS t ON a.idTramo = t.idTramo " +
                                                         "JOIN estatus AS e ON a.estatus = e.estatus " +
                                                         "LEFT JOIN vehiculosAccidente AS va ON a.idAccidente = va.idAccidente " +
                                                         "LEFT JOIN personas AS prop ON prop.idPersona = va.idPersona " +
                                                         "LEFT JOIN conductoresVehiculosAccidente as cva ON a.idAccidente = cva.idAccidente " +
														 "LEFT JOIN opeConductoresVehiculosAccidentePersonas AS cond ON cond.idPersona = cva.idPersona AND cond.idAccidente = cva.idAccidente AND cond.tipoOperacion = 2 AND cva.idVehiculo = cond.idVehiculo " +
                                                         "WHERE a.idAccidente = @idAccidente AND a.estatus = 1;", connection);

                    command.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.Int)).Value = idAccidente;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            accidente.IdAccidente = reader["idAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idAccidente"]) : 0;
                            accidente.numeroReporte = reader["numeroReporte"] != DBNull.Value ? reader["numeroReporte"].ToString() : string.Empty;
                            accidente.fecha = reader["fecha"] != DBNull.Value ? reader["fecha"].ToString().Split(" ")[0] : string.Empty;
                            accidente.hora = reader["hora"] != DBNull.Value ? reader.GetTimeSpan(reader.GetOrdinal("hora")) : TimeSpan.MinValue;
                            accidente.idMunicipio = reader["idMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["idMunicipio"]) : 0;
                            accidente.idCarretera = reader["idCarretera"] != DBNull.Value ? Convert.ToInt32(reader["idCarretera"]) : 0;

                            accidente.municipio = reader["municipio"] != DBNull.Value ? reader["municipio"].ToString() : string.Empty;
                            accidente.nombreConductor = reader["nombreCond"] != DBNull.Value ? reader["nombreCond"].ToString() : string.Empty;
                            accidente.apellidoPaternoConductor = reader["apellidoPaternoCond"] != DBNull.Value ? reader["apellidoPaternoCond"].ToString() : string.Empty;
                            accidente.apellidoMaternoConductor = reader["apellidoMaternoCond"] != DBNull.Value ? reader["apellidoMaternoCond"].ToString() : string.Empty;
                            accidente.nombreConductorCompleto = $"{reader["nombreCond"]} {reader["apellidoPaternoCond"]} {reader["apellidoMaternoCond"]}";
                            accidente.nombrePropietario = reader["nombreProp"] != DBNull.Value ? reader["nombreProp"].ToString() : string.Empty;
                            accidente.apellidoPaternoPropietario = reader["apellidoPaternoProp"] != DBNull.Value ? reader["apellidoPaternoProp"].ToString() : string.Empty;
                            accidente.apellidoMaternoPropietario = reader["apellidoMaternoProp"] != DBNull.Value ? reader["apellidoMaternoProp"].ToString() : string.Empty;
                            accidente.nombrePropietarioCompleto = $"{reader["nombreProp"]} {reader["apellidoPaternoProp"]} {reader["apellidoMaternoProp"]}";

                            accidente.municipio = reader["municipio"] != DBNull.Value ? reader["municipio"].ToString() : string.Empty;



                            accidente.tramo = reader["tramo"] != DBNull.Value ? reader["tramo"].ToString() : string.Empty;
                            accidente.carretera = reader["carretera"] != DBNull.Value ? reader["carretera"].ToString() : string.Empty;
                            accidente.kilometro = reader["kilometro"] != DBNull.Value ? reader["kilometro"].ToString() : string.Empty;
                            accidente.idTramo = reader["idTramo"] != DBNull.Value ? Convert.ToInt32(reader["idTramo"]) : 0;


                        }
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    connection.Close();
                }

            return accidente;
        }

        public List<BusquedaAccidentesModel> ObtenerAccidentes(BusquedaAccidentesModel model)
        {
            //
            List<BusquedaAccidentesModel> ListaAccidentes = new List<BusquedaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(" SELECT acc.idAccidente, acc.idMunicipio, acc.idCarretera, acc.idTramo, acc.idEstatusReporte, acc.estatus,acc.numeroReporte,acc.fecha,acc.hora, " +
                                                        "mun.Municipio, car.Carretera, tra.Tramo, er.estatusReporte, " +
                                                        "MAX(prop.nombre) AS nombrePropietario, MAX(prop.apellidoPaterno) AS apellidoPaternoPropietario, MAX(prop.apellidoMaterno) AS apellidoMaternoPropietario, " +
                                                        "MAX(cond.nombre) AS nombreConductor, MAX(cond.apellidoPaterno) AS apellidoPaternoConductor, MAX(cond.apellidoMaterno) AS apellidoMaternoConductor " +
                                                        "FROM accidentes AS acc " +
                                                        "LEFT JOIN catMunicipios AS mun ON acc.idMunicipio = mun.idMunicipio " +
                                                        "LEFT JOIN catCarreteras AS car ON acc.idCarretera = car.idCarretera " +
                                                        "LEFT JOIN catTramos AS tra ON acc.idTramo = tra.idTramo " +
                                                        "LEFT JOIN vehiculosAccidente AS vea ON acc.idAccidente = vea.idAccidente " +
                                                        "LEFT JOIN personas AS prop ON prop.idPersona = vea.idPersona " +
                                                        "LEFT JOIN conductoresVehiculosAccidente AS cva ON acc.idAccidente = cva.idAccidente " +
														"LEFT JOIN opeConductoresVehiculosAccidentePersonas AS cond ON cond.idPersona = cva.idPersona AND cond.idAccidente = cva.idAccidente AND cond.tipoOperacion = 2 AND cva.idVehiculo = cond.idVehiculo " +
                                                        "LEFT JOIN catEstatusReporteAccidente AS er ON acc.idEstatusReporte = er.idEstatusReporte " +
                                                        "WHERE acc.estatus = 1 " +
                                                        "GROUP BY acc.idAccidente, acc.idMunicipio, acc.idCarretera, acc.idTramo, acc.idEstatusReporte, acc.estatus,acc.numeroReporte,acc.fecha,acc.hora, " +
                                                        "mun.Municipio, car.Carretera, tra.Tramo, er.estatusReporte;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            BusquedaAccidentesModel accidente = new BusquedaAccidentesModel();
                            accidente.IdAccidente = reader["IdAccidente"] != DBNull.Value ? Convert.ToInt32(reader["IdAccidente"]) : 0;
                            accidente.numeroReporte = reader["numeroReporte"] != DBNull.Value ? reader["numeroReporte"].ToString() : string.Empty;
                            accidente.fecha = reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue;
                            accidente.hora = reader["hora"] != DBNull.Value ? reader.GetTimeSpan(reader.GetOrdinal("hora")) : TimeSpan.Zero;
                            accidente.idMunicipio = reader["idMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["idMunicipio"]) : 0;
                            accidente.idCarretera = reader["idCarretera"] != DBNull.Value ? Convert.ToInt32(reader["idCarretera"]) : 0;
                            accidente.idTramo = reader["IdTramo"] != DBNull.Value ? Convert.ToInt32(reader["IdTramo"]) : 0;
                            accidente.idEstatusReporte = reader["idEstatusReporte"] != DBNull.Value ? Convert.ToInt32(reader["idEstatusReporte"]) : 0;
                            accidente.municipio = reader["municipio"] != DBNull.Value ? reader["municipio"].ToString() : string.Empty;
                            accidente.tramo = reader["tramo"] != DBNull.Value ? reader["tramo"].ToString() : string.Empty;
                            accidente.carretera = reader["carretera"] != DBNull.Value ? reader["carretera"].ToString() : string.Empty;


                            ListaAccidentes.Add(accidente);

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
            return ListaAccidentes;


        }

        public IEnumerable<BusquedaAccidentesModel> GetAllAccidentesPagination(Pagination pagination, BusquedaAccidentesModel model, int IdOficina)
        {
            List<BusquedaAccidentesModel> modelList = new List<BusquedaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    int numeroSecuencial = 1;
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("usp_ObtieneAccidentesBusqueda", connection))
                    {
                        if (model.IdTramoBusqueda == 0)
                            model.IdTramoBusqueda = null;
                        if (model.IdCarreteraBusqueda == 0)
                            model.IdCarreteraBusqueda = null;
                        if (model.idAutoriza == 0)
                            model.idAutoriza = null;
                        if (model.idMunicipio == 0)
                            model.idMunicipio = null;
                        if (model.idSupervisa == 0)
                            model.idSupervisa = null;
                        if (model.IdOficialBusqueda == 0)
                            model.IdOficialBusqueda = null;
                        if (string.IsNullOrEmpty(model.folioBusqueda) )
                            model.folioBusqueda = null;
                        if (string.IsNullOrEmpty(model.placasBusqueda))
                            model.placasBusqueda = null;
                        if (string.IsNullOrEmpty(model.propietarioBusqueda))
                            model.propietarioBusqueda = null;
                        if (string.IsNullOrEmpty(model.serieBusqueda))
                            model.serieBusqueda = null;
                        if (string.IsNullOrEmpty(model.conductorBusqueda))
                            model.conductorBusqueda = null;
						if (string.IsNullOrEmpty(model.FolioEmergenciaBusqueda))
							model.FolioEmergenciaBusqueda = null;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.AddWithValue("@PageIndex", pagination.PageIndex);
                        cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                        if (model.idMunicipio != null)
						    cmd.Parameters.AddWithValue("@IdMunicipio", (object)model.idMunicipio ?? DBNull.Value);
                        if (model.idSupervisa != null)
                            cmd.Parameters.AddWithValue("@IdSupervisa", (object)model.idSupervisa ?? DBNull.Value);
                        if (model.IdCarreteraBusqueda != null)
                            cmd.Parameters.AddWithValue("@IdCarretera", (object)model.IdCarreteraBusqueda ?? DBNull.Value);
                        if (model.IdTramoBusqueda != null)
                            cmd.Parameters.AddWithValue("@IdTramo", (object)model.IdTramoBusqueda ?? DBNull.Value);
                        if (model.IdOficialBusqueda != null)
                            cmd.Parameters.AddWithValue("@IdElabora", (object)model.IdOficialBusqueda ?? DBNull.Value);
                        if (model.idAutoriza != null)
                            cmd.Parameters.AddWithValue("@IdAutoriza", (object)model.idAutoriza ?? DBNull.Value);
                        if (model.IdEstatusAccidente != 0)
                            cmd.Parameters.AddWithValue("@IdEstatusAccidente", (object)model.IdEstatusAccidente ?? DBNull.Value);
                        if (model.IdDelegacionBusqueda != null)
                            cmd.Parameters.AddWithValue("@IdDelegacionBusqueda", (object)model.IdDelegacionBusqueda ?? DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@IdDelegacionBusqueda", IdOficina);

                        if (model.folioBusqueda != null)
                            cmd.Parameters.AddWithValue("@FolioBusqueda", (object)model.folioBusqueda ?? DBNull.Value);
                        if (model.placasBusqueda != null)
                            cmd.Parameters.AddWithValue("@PlacasBusqueda", (object)model.placasBusqueda.ToUpper() ?? DBNull.Value );
                        if (model.propietarioBusqueda != null)
                            cmd.Parameters.AddWithValue("@PropietarioBusqueda", (object)model.propietarioBusqueda ?? DBNull.Value);
                        if (model.serieBusqueda != null)
                            cmd.Parameters.AddWithValue("@SerieBusqueda", (object)model.serieBusqueda ?? DBNull.Value);
                        if (model.conductorBusqueda != null)
                            cmd.Parameters.AddWithValue("@ConductorBusqueda", (object)model.conductorBusqueda ?? DBNull.Value);
						if(model.FolioEmergenciaBusqueda != null)
							cmd.Parameters.AddWithValue("@FolioEmergenciaBusqueda", (object)model.FolioEmergenciaBusqueda ?? DBNull.Value);
						if (model.FechaInicio != null)
						{
                            cmd.Parameters.AddWithValue("@FechaInicio", model.FechaInicio);
                        }
                        else
						{
							cmd.Parameters.AddWithValue("@FechaInicio", DBNull.Value);

						}

						if (model.FechaFin != null)
						{
                            cmd.Parameters.AddWithValue("@FechaFin", model.FechaFin);
                        }
                        else
						{
							cmd.Parameters.AddWithValue("@FechaFin", DBNull.Value);

						}


						/*if (pagination.Filter.Trim() != "")
                            cmd.Parameters.AddWithValue("@Filter", pagination.Filter);*/

						using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())

                            {
                                BusquedaAccidentesModel accidente = new BusquedaAccidentesModel();
                                accidente.IdAccidente = Convert.IsDBNull(reader["idAccidente"]) ? 0 : Convert.ToInt32(reader["idAccidente"]);
                                accidente.idMunicipio = Convert.IsDBNull(reader["idMunicipio"]) ? 0 : Convert.ToInt32(reader["idMunicipio"]);
                                accidente.idCarretera = Convert.IsDBNull(reader["idCarretera"]) ? 0 : Convert.ToInt32(reader["idCarretera"]);
                                accidente.IdDelegacionBusqueda = Convert.IsDBNull(reader["idOficinaDelegacion"]) ? 0 : Convert.ToInt32(reader["idOficinaDelegacion"]);
                                accidente.idDelegacion = Convert.IsDBNull(reader["idOficinaDelegacion"]) ? 0 : Convert.ToInt32(reader["idOficinaDelegacion"]);
								accidente.idTramo = Convert.IsDBNull(reader["idTramo"]) ? 0 : Convert.ToInt32(reader["idTramo"]);
                                accidente.kilometro = reader["kilometro"].ToString();
                                accidente.idEstatusReporte = Convert.IsDBNull(reader["idEstatusReporte"]) ? 0 : Convert.ToInt32(reader["idEstatusReporte"]);
                                accidente.estatusReporte = reader["estatusReporte"].ToString();
                                accidente.municipio = reader["municipio"].ToString();
                                accidente.carretera = reader["carretera"].ToString();
                                accidente.placa = reader["placa"].ToString();
                                accidente.serie = reader["serie"].ToString();
                                string nombrePropietario = reader["nombre"].ToString();
                                string apellidoPaternoPropietario = reader["apellidoPaterno"].ToString();
                                string apellidoMaternoPropietario = reader["apellidoMaterno"].ToString();
                                accidente.propietario = $"{nombrePropietario} {apellidoPaternoPropietario} {apellidoMaternoPropietario}";
                                string nombreConductor = reader["nombreConductor"].ToString();
                                string apellidoPaternoConductor = reader["apellidoPaternoConductor"].ToString();
                                string apellidoMaternoConductor = reader["apellidoMaternoConductor"].ToString();
                                accidente.conductor = $"{nombreConductor} {apellidoPaternoConductor} {apellidoMaternoPropietario}";
                                accidente.tramo = reader["tramo"].ToString();
                                accidente.idElabora = Convert.IsDBNull(reader["idElabora"]) ? 0 : Convert.ToInt32(reader["idElabora"]);
                                accidente.idSupervisa = Convert.IsDBNull(reader["idSupervisa"]) ? 0 : Convert.ToInt32(reader["idSupervisa"]);
                                accidente.idAutoriza = Convert.IsDBNull(reader["idAutoriza"]) ? 0 : Convert.ToInt32(reader["idAutoriza"]);
                                accidente.idConductor = Convert.IsDBNull(reader["idConductor"]) ? 0 : Convert.ToInt32(reader["idConductor"]);
                                accidente.idPropietario = Convert.IsDBNull(reader["idPropietario"]) ? 0 : Convert.ToInt32(reader["idPropietario"]);
                                accidente.folioEmergencia = Convert.IsDBNull(reader["folioEmergencia"]) ? default(int?) : Convert.ToInt32(reader["folioEmergencia"]);
                                accidente.emergenciasId = Convert.IsDBNull(reader["emergenciasId"]) ? default(int?) : Convert.ToInt32(reader["emergenciasId"]);
                                accidente.numeroReporte = reader["numeroReporte"].ToString();
                                accidente.fecha = reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue;
                                accidente.hora = reader["hora"] != DBNull.Value ? TimeSpan.Parse(reader["hora"].ToString()) : TimeSpan.MinValue;
                                accidente.NumeroSecuencial = Convert.IsDBNull(reader["rowIndex"]) ? 0 : Convert.ToInt32(reader["rowIndex"]); 

                               accidente.total = Convert.ToInt32(reader["Total"]);

                                modelList.Add(accidente);


                            }
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
                return modelList;
            }


        }




        List<OnlyId> GetConductoresIdAccidentes(string Conductor)
        {
            List<OnlyId> result = new List<OnlyId>();
            var query = $@"	SELECT per.idAccidente	                        
	                        FROM conductoresVehiculosAccidente AS cva 
	                        LEFT JOIN opeConductoresVehiculosAccidentePersonas	per	WITH (NOLOCK) ON per.fechaActualizacion = 
			                        (
			                        SELECT MAX(fechaActualizacion) 
			                        FROM opeConductoresVehiculosAccidentePersonas zz 
			                        WHERE cva.idPersona = zz.idPersona AND cva.idAccidente = zz.idAccidente AND ZZ.tipoOperacion = 2 
			                        ) AND cva.idAccidente = PER.idAccidente AND CVA.idPersona = PER.idPersona AND per.tipoOperacion = 2 AND cva.idVehiculo = per.idVehiculo
	                        WHERE  UPPER(CONCAT(per.nombre,' ', per.apellidoPaterno,' ',per.apellidoMaterno)) LIKE '%' + UPPER(@conductor) + '%'
	                        GROUP BY per.idAccidente";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@conductor", Conductor);


                        using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                                var aux = new OnlyId();

                                aux.Id = int.Parse(reader["idAccidente"].ToString());
                                result.Add(aux);
                            }
                        }

                    }


                }
                catch (Exception e)
                {

                }
                finally { connection.Close(); }
            }

            return result;
        }
        string getWhereAccidentesPropietarioVehiculo(BusquedaAccidentesModel model)
        {
            var where = "";
            if (!string.IsNullOrEmpty(model.propietarioBusqueda))
            {
                where = $"{where} and UPPER(CONCAT(per.nombre,' ', per.apellidoPaterno,' ',per.apellidoMaterno)) LIKE '%' + UPPER(@PropietarioBusqueda) + '%'";
            }
            if (!string.IsNullOrEmpty(model.placasBusqueda))
            {
                where = $"{where} and v.placas = @PlacasBusqueda";
            }
            if (!string.IsNullOrEmpty(model.serieBusqueda))
            {
                where = $"{where} and v.serie = @SerieBusqueda";
            }
            return where;
        }
        void getParamsAccidentesPropietarioVehiculo(BusquedaAccidentesModel model, SqlCommand cmd)
        {
            if (!string.IsNullOrEmpty(model.propietarioBusqueda))
            {
                cmd.Parameters.AddWithValue("@PropietarioBusqueda", (object)model.propietarioBusqueda );
            }
            if (!string.IsNullOrEmpty(model.placasBusqueda))
            {
                cmd.Parameters.AddWithValue("@PlacasBusqueda", (object)model.placasBusqueda.ToUpper());
            }
            if (!string.IsNullOrEmpty(model.serieBusqueda))
            {
                cmd.Parameters.AddWithValue("@SerieBusqueda", (object)model.serieBusqueda );
            }
        }
        List<OnlyId> GetPropietarioIdAccidentes(BusquedaAccidentesModel model)
        {
            List<OnlyId> result = new List<OnlyId>();

            var query = $@"SELECT vea.idAccidente,per.nombre,per.apellidoPaterno,per.apellidoMaterno
	                        FROM vehiculosAccidente vea 
	                        LEFT JOIN opeAccidentesVehiculos	v	WITH (NOLOCK) ON v.idOperacion = 
			                        (
			                        SELECT MAX(idOperacion) 
			                        FROM opeAccidentesVehiculos z 
			                        WHERE vea.idVehiculo = z.idVehiculo AND vea.idAccidente = z.idAccidente AND (@PlacasBusqueda IS NULL  OR z.placas = @PlacasBusqueda)
			                        ) AND vea.idAccidente = v.idAccidente AND vea.idVehiculo = v.idVehiculo
	                        LEFT JOIN opeConductoresVehiculosAccidentePersonas	per	WITH (NOLOCK) ON per.fechaActualizacion = 
			                        (
			                        SELECT MAX(fechaActualizacion) 
			                        FROM opeConductoresVehiculosAccidentePersonas zz 
			                        WHERE vea.idPersona = zz.idPersona AND vea.idAccidente = zz.idAccidente AND zz.tipoOperacion = 1 AND vea.idVehiculo = zz.idVehiculo
			                        ) AND vea.idPersona = per.idPersona AND vea.idVehiculo = per.idVehiculo AND per.tipoOperacion = 1 AND VEA.idAccidente = per.idAccidente

	                        WHERE	vea.estatus=1
                                    {getWhereAccidentesPropietarioVehiculo(model)}
	                        GROUP BY vea.idAccidente,per.nombre,per.apellidoPaterno,per.apellidoMaterno";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        getParamsAccidentesPropietarioVehiculo(model, cmd);


                        using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                                var aux = new OnlyId();

                                aux.Id = int.Parse(reader["idAccidente"].ToString());
                                result.Add(aux);
                            }
                        }

                    }


                }
                catch (Exception e)
                {

                }
                finally { connection.Close(); }
            }

            return result;
        }

        string getWhereAccidentes(BusquedaAccidentesModel model, List<OnlyId> ConductorPropietario)
        {
            var result = "";

            if (model.IdDelegacionBusqueda.HasValue && model.IdDelegacionBusqueda.Value>0)
            {
                result = $"{result} and a.idOficinaDelegacion = @IdDelegacionBusqueda";
            }
            if (model.IdEstatusAccidente> 0)
            {
                result = $"{result} and a.idEstatusReporte = @IdEstatusAccidente";
            }
            if (model.idMunicipio.HasValue && model.idMunicipio.Value > 0)
            {
                result = $"{result} and a.idMunicipio = @IdMunicipio";
            }
            if (model.idSupervisa.HasValue && model.idSupervisa.Value > 0)
            {
                result = $"{result} and a.idSupervisa = @IdSupervisa";
            }
            if (model.IdCarreteraBusqueda.HasValue && model.IdCarreteraBusqueda.Value > 0)
            {
                result = $"{result} and a.idCarretera = @IdCarretera";
            }
            if (model.IdTramoBusqueda.HasValue && model.IdTramoBusqueda.Value > 0)
            {
                result = $"{result} and a.idTramo = @IdTramo";
            }
            if (model.IdOficialBusqueda.HasValue && model.IdOficialBusqueda.Value > 0)
            {
                result = $"{result} and a.idElabora = @IdElabora";
            }
            if (model.idAutoriza.HasValue && model.idAutoriza.Value > 0)
            {
                result = $"{result} and a.idAutoriza = @IdAutoriza";
            }
            if (!string.IsNullOrEmpty(model.folioBusqueda))
            {
                result = $"{result} and a.numeroreporte = @FolioBusquedaa";
            }
            if (!string.IsNullOrEmpty(model.FolioEmergenciaBusqueda))
            {
                result = $"{result} and ae.folioEmergencia LIKE '%' + @FolioEmergenciaBusqueda + '%'";
            }
            if(model.FechaInicio.HasValue && model.FechaFin.HasValue)
            {
                result = $"{result} and CONVERT(VARCHAR,a.fecha,23) BETWEEN CONVERT(VARCHAR,@FechaInicio,23) AND CONVERT(VARCHAR,@FechaFin,23)";
            }
            else if(!model.FechaInicio.HasValue && model.FechaFin.HasValue)
            {
                result = $"{result} and CONVERT(VARCHAR,a.fecha,23) < CONVERT(VARCHAR,@FechaFin,23)";

            }
            else if (model.FechaInicio.HasValue && !model.FechaFin.HasValue)
            {
                result = $"{result} and CONVERT(VARCHAR,a.fecha,23) < CONVERT(VARCHAR,@FechaInicio,23)";
            }

            if (ConductorPropietario.Count > 0)
            {
                result = $"{result} and a.idAccidente in ( select Id from @dataID )";
            }


            return result;
        }

        void getParamsAccidentes(BusquedaAccidentesModel model, List<OnlyId> ConductorPropietario, SqlCommand cmd)
        {
            if (model.IdDelegacionBusqueda.HasValue && model.IdDelegacionBusqueda.Value > 0)
            {
                cmd.Parameters.AddWithValue("@IdDelegacionBusqueda", model.IdDelegacionBusqueda);
            }
            if (model.IdEstatusAccidente > 0)
            {
                cmd.Parameters.AddWithValue("@IdEstatusAccidente", model.IdEstatusAccidente);
            }
            if (model.idMunicipio.HasValue && model.idMunicipio.Value > 0)
            {
                cmd.Parameters.AddWithValue("@IdMunicipio", model.idMunicipio);
            }
            if (model.idSupervisa.HasValue && model.idSupervisa.Value > 0)
            {
                cmd.Parameters.AddWithValue("@IdSupervisa", model.idSupervisa);
            }
            if (model.IdCarreteraBusqueda.HasValue && model.IdCarreteraBusqueda.Value > 0)
            {
                cmd.Parameters.AddWithValue("@IdCarretera", model.IdCarreteraBusqueda);
            }
            if (model.IdTramoBusqueda.HasValue && model.IdTramoBusqueda.Value > 0)
            {
                cmd.Parameters.AddWithValue("@IdTramo", model.IdTramoBusqueda);
            }
            if (model.IdOficialBusqueda.HasValue && model.IdOficialBusqueda.Value > 0)
            {
                cmd.Parameters.AddWithValue("@IdElabora", model.IdOficialBusqueda);
            }
            if (model.idAutoriza.HasValue && model.idAutoriza.Value > 0)
            {
                cmd.Parameters.AddWithValue("@IdAutoriza", model.idAutoriza);
            }
            if (!string.IsNullOrEmpty(model.folioBusqueda))
            {
                cmd.Parameters.AddWithValue("@FolioBusquedaa", model.folioBusqueda);
            }
            if (!string.IsNullOrEmpty(model.FolioEmergenciaBusqueda))
            {
                cmd.Parameters.AddWithValue("@FolioEmergenciaBusqueda", model.FolioEmergenciaBusqueda);
            }
            if (model.FechaInicio.HasValue && model.FechaFin.HasValue)
            {
                cmd.Parameters.AddWithValue("@FechaInicio", model.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", model.FechaFin);
            }
            else if (!model.FechaInicio.HasValue && model.FechaFin.HasValue)
            {
                cmd.Parameters.AddWithValue("@FechaFin", model.FechaFin);
            }
            else if (model.FechaInicio.HasValue && !model.FechaFin.HasValue)
            {
                cmd.Parameters.AddWithValue("@FechaInicio", model.FechaInicio);
            }

            if (ConductorPropietario.Count > 0)
            {
                var myparameter = new SqlParameter("@dataID", SqlDbType.Structured);
                myparameter.TypeName = "TVP_People";
                myparameter.Value = ConductorPropietario;
                cmd.Parameters.Add(myparameter);
            }
        }


        int GetCountAccidentes(BusquedaAccidentesModel model, List<OnlyId> ConductorPropietario)
        {
            var result = 0;
            var query = $@"SELECT  count(*) count
	                        FROM  accidentes AS a
	                        LEFT JOIN   catMunicipios AS mun	ON a.idMunicipio = mun.idMunicipio
	                        LEFT JOIN   catCarreteras AS car	ON a.idCarretera = car.idCarretera
	                        LEFT JOIN   catTramos AS tra		ON a.idTramo = tra.idTramo
	                        LEFT JOIN   catEstatusReporteAccidente AS er ON a.idEstatusReporte = er.idEstatusReporte 
	                        LEFT JOIN   catOficiales AS ela		ON a.idElabora = ela.idOficial
	                        LEFT JOIN   catOficiales AS sup		ON a.idSupervisa = sup.idOficial
	                        LEFT JOIN   catOficiales AS aut		ON a.idAutoriza = aut.idOficial
	                        LEFT JOIN accidentesEmergencias AS ae ON a.idAccidente = ae.idAccidente
	                        where a.estatus=1
                            {getWhereAccidentes(model, ConductorPropietario)}
                            ";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        getParamsAccidentes(model, ConductorPropietario, cmd);
                        using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                                result = int.Parse(reader["count"].ToString());
                            }
                        }
                    }
                 }
                catch (Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            }

                    return result;
        }


        void CreateQueryAndFetallAccidents(BusquedaAccidentesModel model, Pagination pagination, int count, List<OnlyId> ConductorPropietario,List<BusquedaAccidentesModel> modelList)
        {           
            var query = $@"select * from(SELECT  ROW_NUMBER() OVER ( ORDER BY a.idaccidente ASC ) AS rowIndex,
                            a.idOficinaDelegacion,
		                    a.idAccidente,
                            a.idEstatusReporte,
		                    a.numeroReporte,
		                    a.fecha,
		                    a.hora,
		                    mun.municipio,
		                    car.carretera,
		                    tra.tramo,
		                    er.estatusReporte,
		                    ae.folioEmergencia as folioEmergencia,
		                    {count} Total
	                    FROM  accidentes AS a
	                    LEFT JOIN   catMunicipios AS mun	ON a.idMunicipio = mun.idMunicipio
	                    LEFT JOIN   catCarreteras AS car	ON a.idCarretera = car.idCarretera
	                    LEFT JOIN   catTramos AS tra		ON a.idTramo = tra.idTramo
	                    LEFT JOIN   catEstatusReporteAccidente AS er ON a.idEstatusReporte = er.idEstatusReporte 
	                    LEFT JOIN   catOficiales AS ela		ON a.idElabora = ela.idOficial
	                    LEFT JOIN   catOficiales AS sup		ON a.idSupervisa = sup.idOficial
	                    LEFT JOIN   catOficiales AS aut		ON a.idAutoriza = aut.idOficial
	                    LEFT JOIN accidentesEmergencias AS ae ON a.idAccidente = ae.idAccidente
	                    where a.estatus=1
                        {getWhereAccidentes(model, ConductorPropietario)}
	                    )accidentesaux ORDER BY rowIndex DESC	
	                    OFFSET @PageIndex * @PageSize  ROWS FETCH NEXT @PageSize ROWS ONLY ";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@PageIndex", pagination.PageIndex);
                        cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);                        
                        getParamsAccidentes(model, ConductorPropietario, cmd);


                        using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                                BusquedaAccidentesModel accidente = new BusquedaAccidentesModel();
                                accidente.NumeroSecuencial = Convert.IsDBNull(reader["rowIndex"]) ? 0 : Convert.ToInt32(reader["rowIndex"]);
                                accidente.IdAccidente = Convert.IsDBNull(reader["idAccidente"]) ? 0 : Convert.ToInt32(reader["idAccidente"]);
                                accidente.idDelegacion = Convert.IsDBNull(reader["idOficinaDelegacion"]) ? 0 : Convert.ToInt32(reader["idOficinaDelegacion"]);
                                accidente.numeroReporte = reader["numeroReporte"].ToString();
                                accidente.idEstatusReporte = Convert.IsDBNull(reader["idEstatusReporte"]) ? 0 : Convert.ToInt32(reader["idEstatusReporte"]);
                                accidente.fecha = reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue;
                                accidente.hora = reader["hora"] != DBNull.Value ? TimeSpan.Parse(reader["hora"].ToString()) : TimeSpan.MinValue;
                                accidente.municipio = reader["municipio"].ToString();
                                accidente.carretera = reader["carretera"].ToString();
                                accidente.tramo = reader["tramo"].ToString();
                                accidente.estatusReporte = reader["estatusReporte"].ToString();
                                accidente.folioEmergencia = Convert.IsDBNull(reader["folioEmergencia"]) ? default(int?) : Convert.ToInt32(reader["folioEmergencia"]);
                                accidente.total = Convert.ToInt32(reader["Total"]);                                                                       
                                modelList.Add(accidente);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public IEnumerable<BusquedaAccidentesModel> GetAllAccidentesPaginationWhitcode(Pagination pagination, BusquedaAccidentesModel model, int IdOficina)
        {
            if(!model.IdDelegacionBusqueda.HasValue || model.IdDelegacionBusqueda.Value < 1)
            {
                model.IdDelegacionBusqueda = IdOficina;
            }

            List<BusquedaAccidentesModel> modelList = new List<BusquedaAccidentesModel>();
            List<OnlyId> ConductorPropietario = new List<OnlyId>();
            if (!string.IsNullOrEmpty(model.conductorBusqueda))
            {
                ConductorPropietario.AddRange(GetConductoresIdAccidentes(model.conductorBusqueda));
            }
            if (!string.IsNullOrEmpty(model.propietarioBusqueda) || !string.IsNullOrEmpty(model.placasBusqueda)
                || !string.IsNullOrEmpty(model.serieBusqueda))
            {
                ConductorPropietario.AddRange(GetPropietarioIdAccidentes(model));
            }
            if (ConductorPropietario.Count > 0)
            {
                ConductorPropietario.GroupBy(s => s.Id).Select(s => s.First()).ToList();
            }
            var count = GetCountAccidentes(model, ConductorPropietario);
            CreateQueryAndFetallAccidents(model, pagination, count, ConductorPropietario, modelList);

                return modelList;
            


        }

    }
}
