using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using System.Collections.Generic;
using System.Data;
using System;
using GuanajuatoAdminUsuarios.Models;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GuanajuatoAdminUsuarios.Services
{

    public class BusquedaEspecialAccidentesService : IBusquedaEspecialAccidentesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public BusquedaEspecialAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<BusquedaEspecialAccidentesModel> BusquedaAccidentes(BusquedaEspecialAccidentesModel model, int idOficina)
        {
            //
            List<BusquedaEspecialAccidentesModel> ListaAccidentes = new List<BusquedaEspecialAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT 
    a.idAccidente, 
    a.numeroReporte, 
    a.fecha, 
    a.hora, 
    a.idMunicipio, 
    a.idTramo, 
    a.idCarretera, 
    a.idElabora, 
    a.idSupervisa, 
    a.idAutoriza, 
    a.kilometro, 
    a.idOficinaDelegacion, 
    a.idEstatusReporte, 
    mun.municipio, 
    car.carretera, 
    tra.tramo, 
    er.estatusReporte AS estatusReporteDesc, 
    MAX(vea.placa) AS placa, 
    MAX(vea.serie) AS serie, 
    MAX(cond.idPersona) AS idConductor, 
    MAX(vea.idPersona) AS idPropietario, 
    ela.idOficial AS elabora, 
    sup.idOficial AS supervisa, 
    aut.idOficial AS autoriza 
FROM 
    accidentes AS a 
    LEFT JOIN vehiculosAccidente AS vea ON a.idAccidente = vea.idAccidente 
    LEFT JOIN vehiculos AS v ON vea.idVehiculo = v.idVehiculo 
    LEFT JOIN conductoresVehiculosAccidente AS cva ON a.idAccidente = cva.idAccidente 
    LEFT JOIN opeConductoresVehiculosAccidentePersonas AS cond ON cva.idPersona = cond.idPersona AND cond.idAccidente = cva.idAccidente  AND cond.tipoOperacion = 2 AND cva.idVehiculo = cond.idVehiculo 
    LEFT JOIN personas AS prop ON vea.idPersona = prop.idPersona 
    LEFT JOIN catMunicipios AS mun ON a.idMunicipio = mun.idMunicipio 
    LEFT JOIN catCarreteras AS car ON a.idCarretera = car.idCarretera 
    LEFT JOIN catTramos AS tra ON a.idTramo = tra.idTramo 
    LEFT JOIN catEstatusReporteAccidente AS er ON a.idEstatusReporte = er.idEstatusReporte 
    LEFT JOIN catOficiales AS ela ON a.idElabora = ela.idOficial 
    LEFT JOIN catOficiales AS sup ON a.idSupervisa = sup.idOficial 
    LEFT JOIN catOficiales AS aut ON a.idAutoriza = aut.idOficial 
WHERE 
    a.idOficinaDelegacion = @idOficina
    AND a.estatus != 0
    AND (
        (@fechaInicio IS NULL OR @fechaFin IS NULL OR (a.fecha BETWEEN @fechaInicio AND @fechaFin))
        OR vea.placa = @placasBusqueda
        OR UPPER(a.numeroReporte) = @oficioBusqueda
        OR a.idAutoriza = @idOficialBusqueda
        OR a.idSupervisa = @idOficialBusqueda
        OR a.idCarretera = @idCarreteraBusqueda
        OR a.idTramo = @idTramoBusqueda
        OR prop.nombre = @propietarioBusqueda
        OR UPPER(prop.apellidoPaterno) = @propietarioBusqueda
        OR UPPER(prop.apellidoMaterno) = @propietarioBusqueda
        OR cond.nombre = @conductorBusqueda
        OR UPPER(cond.apellidoPaterno) = @conductorBusqueda
        OR UPPER(cond.apellidoMaterno) = @conductorBusqueda
        OR vea.serie = @serieBusqueda
        OR a.idEstatusReporte = @idEstatusReporte
    )
GROUP BY 
    a.idAccidente, 
    a.numeroReporte, 
    a.fecha, 
    a.hora, 
    a.idMunicipio, 
    a.idTramo, 
    a.idCarretera, 
    a.idElabora, 
    a.idEstatusReporte, 
    a.idSupervisa, 
    a.idAutoriza, 
    a.kilometro, 
    a.idOficinaDelegacion, 
    mun.municipio, 
    car.carretera, 
    tra.tramo, 
    er.estatusReporte, 
    ela.idOficial, 
    sup.idOficial, 
    aut.idOficial;
", connection);


                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.DateTime)).Value = (model.FechaInicio == DateTime.MinValue) ? DBNull.Value : (object)model.FechaInicio;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    // command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.DateTime)).Value = (model.FechaFin == DateTime.MinValue) ? DBNull.Value : (object)model.FechaFin;
                    command.Parameters.Add(new SqlParameter("@idEstatusReporte", SqlDbType.Int)).Value = (object)model.idEstatusReporte ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@oficioBusqueda", SqlDbType.NVarChar)).Value = (object)model.folioBusqueda != null ? model.folioBusqueda.ToUpper() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idDelegacionBusqueda", SqlDbType.Int)).Value = (object)model.IdDelegacionBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idOficialBusqueda", SqlDbType.Int)).Value = (object)model.IdOficialBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idCarreteraBusqueda", SqlDbType.Int)).Value = (object)model.IdCarreteraBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTramoBusqueda", SqlDbType.Int)).Value = (object)model.IdTramoBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@propietarioBusqueda", SqlDbType.NVarChar)).Value = (object)model.propietarioBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@conductorBusqueda", SqlDbType.NVarChar)).Value = (object)model.conductorBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@placasBusqueda", SqlDbType.NVarChar)).Value = (object)model.placasBusqueda.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@serieBusqueda", SqlDbType.NVarChar)).Value = (object)model.serieBusqueda ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())


                        {
                            BusquedaEspecialAccidentesModel accidente = new BusquedaEspecialAccidentesModel();
                            accidente.IdAccidente = Convert.IsDBNull(reader["idAccidente"]) ? 0 : Convert.ToInt32(reader["idAccidente"]);
                            accidente.idMunicipio = Convert.IsDBNull(reader["idMunicipio"]) ? 0 : Convert.ToInt32(reader["idMunicipio"]);
                            accidente.idCarretera = Convert.IsDBNull(reader["idCarretera"]) ? 0 : Convert.ToInt32(reader["idCarretera"]);
                            accidente.idTramo = Convert.IsDBNull(reader["idTramo"]) ? 0 : Convert.ToInt32(reader["idTramo"]);
                            accidente.kilometro = reader["kilometro"].ToString();
                            accidente.idEstatusReporte = Convert.IsDBNull(reader["idEstatusReporte"]) ? 0 : Convert.ToInt32(reader["idEstatusReporte"]);
                            accidente.estatusReporte = reader["estatusReporteDesc"].ToString();
                            accidente.municipio = reader["municipio"].ToString();
                            accidente.carretera = reader["carretera"].ToString();
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
                    SqlCommand command = new SqlCommand("SELECT a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idTramo, a.idCarretera, a.idElabora, a.idSupervisa, a.idAutoriza, a.kilometro, a.idOficinaDelegacion, " +
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
                        "LEFT JOIN vehiculos AS v ON vea.idVehiculo = v.idVehiculo " +
                        "LEFT JOIN conductoresVehiculosAccidente AS cva ON a.idAccidente = cva.idAccidente " +
						"LEFT JOIN opeConductoresVehiculosAccidentePersonas AS cond ON cva.idPersona = cond.idPersona  AND cond.idAccidente = cva.idAccidente AND cond.tipoOperacion = 2 AND cva.idVehiculo = cond.idVehiculo " +
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
                        "AND a.idOficinaDelegacion = @idOficina " +
                       "AND ((@fechaInicio IS NULL AND @fechaFin IS NULL) OR (a.fecha BETWEEN @fechaInicio AND @fechaFin)) " +
                    "GROUP BY a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idTramo, a.idCarretera, a.idElabora, a.idSupervisa,a. idAutoriza,a.kilometro,a.idOficinaDelegacion, " +
                        "mun.municipio, car.carretera, tra.tramo, er.estatusReporte,er.idEstatusReporte, ela.idOficial, sup.idOficial, aut.idOficial; ", connection);


                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.DateTime)).Value = (model.FechaInicio == DateTime.MinValue) ? DBNull.Value : (object)model.FechaInicio;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    // command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.DateTime)).Value = (model.FechaFin == DateTime.MinValue) ? DBNull.Value : (object)model.FechaFin;
                    command.Parameters.Add(new SqlParameter("@oficioBusqueda", SqlDbType.NVarChar)).Value = (object)model.folio != null ? model.folioBusqueda.ToUpper() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idDelegacionBusqueda", SqlDbType.Int)).Value = (object)model.idDelegacion ?? DBNull.Value;
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
														 "LEFT JOIN opeConductoresVehiculosAccidentePersonas AS cond ON cond.idPersona = cva.idPersona  AND cond.idAccidente = cva.idAccidente AND cond.tipoOperacion = 2 AND cva.idVehiculo = cond.idVehiculo " +
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

        public List<BusquedaEspecialAccidentesModel> ObtenerAccidentes(BusquedaEspecialAccidentesModel model)
        {
            //
            List<BusquedaEspecialAccidentesModel> ListaAccidentes = new List<BusquedaEspecialAccidentesModel>();

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
                            BusquedaEspecialAccidentesModel accidente = new BusquedaEspecialAccidentesModel();
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
        public int EliminarSeleccionado(int idAccidente)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE accidentes SET estatus = 0,actualizadoPor = @ActualizadoPor, FechaActualizacion = @FechaActualizacion WHERE idAccidente = @idAccidente";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.Add(new SqlParameter("@ActualizadoPor", 1));
                    command.Parameters.Add(new SqlParameter("@FechaActualizacion", DateTime.Now));
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return result;
                }
                finally
                {
                    connection.Close();
                }

                return result;
            }
        }
        public List<BusquedaEspecialAccidentesModel> ObtenerTodosAccidentes()
        {
            //
            List<BusquedaEspecialAccidentesModel> ListaAccidentes = new List<BusquedaEspecialAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idTramo, a.idCarretera, a.idElabora, a.idSupervisa, a.idAutoriza, a.kilometro, a.idOficinaDelegacion, " +
                        "mun.municipio, " +
                        "car.carretera, " +
                        "tra.tramo, " +
                        "er.estatusReporte, " +
                        "a.idEstatusReporte, " +
                        "MAX(vea.placa) AS placa, MAX(vea.serie) AS serie, " +
                        "MAX(cond.idPersona) AS idConductor, " +
                        "MAX(vea.idPersona) AS idPropietario, " +
                        "ela.idOficial AS elabora, " +
                        "sup.idOficial AS supervisa, " +
                        "aut.idOficial AS autoriza, " +
                        "MAX(prop.nombre) AS nombre, MAX(prop.apellidoPaterno) AS apellidoPaterno, MAX(prop.apellidoMaterno) AS apellidoMaterno, " +
                        "MAX(cond.nombre) AS nombreConductor, MAX(cond.apellidoPaterno) AS apellidoPaternoConductor, MAX(cond.apellidoMaterno) AS apellidoMaternoConductor " +
                        "FROM accidentes AS a " +
                        "LEFT JOIN vehiculosAccidente AS vea ON a.idAccidente = vea.idAccidente " +
                        "LEFT JOIN vehiculos AS v ON vea.idVehiculo = v.idVehiculo " +
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
                        "mun.municipio, car.carretera, tra.tramo, er.estatusReporte,a.idEstatusReporte, ela.idOficial, sup.idOficial,aut.idOficial; ", connection);


                    command.CommandType = CommandType.Text;
                    // command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    // command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())


                        {
                            BusquedaEspecialAccidentesModel accidente = new BusquedaEspecialAccidentesModel();
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

                }
                finally
                {
                    connection.Close();
                }
            return ListaAccidentes;


        }



        public bool UpdateFolio(string id, string folio)
        {
            var result = true;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE accidentes SET numeroreporte = @folio, fechaActualizacion = @fechaActualizacion WHERE idAccidente = @idAccidente";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idAccidente", id);
                    command.Parameters.AddWithValue("@folio", folio);
                    command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return result;
                }
                finally
                {
                    connection.Close();
                }

                return result;
            }


        }




        public bool validarFolio(string folio)
        {
            var result = false;
            var count = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "select count(*) as result from  accidentes where numeroreporte = @folio ";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@folio", folio);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {

                        while (reader.Read())
                        {
                            count = (int)reader["result"];
                        }

                    }
                }
                catch (SqlException ex)
                {
                    return result;
                }
                finally
                {
                    connection.Close();
                }

                result = count == 0;

                return result;
            }


        }
        public IEnumerable<BusquedaEspecialAccidentesModel> GetAllAccidentesPagination(Pagination pagination, BusquedaEspecialAccidentesModel model)
        {
            List<BusquedaEspecialAccidentesModel> modelList = new List<BusquedaEspecialAccidentesModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    int numeroSecuencial = 1;
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("usp_ObtieneAccidentesBusqueda", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PageIndex", pagination.PageIndex);
                        cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                        if (model.idMunicipio != null)
                            cmd.Parameters.AddWithValue("@IdMunicipio", (object)model.idMunicipio ?? DBNull.Value);
                        if (model.IdOficialBusqueda != null)
                            cmd.Parameters.AddWithValue("@IdSupervisa", (object)model.IdOficialBusqueda ?? DBNull.Value);
                        if (model.IdCarreteraBusqueda != null)
                            cmd.Parameters.AddWithValue("@IdCarretera", (object)model.IdCarreteraBusqueda ?? DBNull.Value);
                        if (model.IdTramoBusqueda != null)
                            cmd.Parameters.AddWithValue("@IdTramo", (object)model.IdTramoBusqueda ?? DBNull.Value);
                        if (model.IdOficialBusqueda != null)
                            cmd.Parameters.AddWithValue("@IdElabora", (object)model.IdOficialBusqueda ?? DBNull.Value);
                        if (model.IdOficialBusqueda != null)
                            cmd.Parameters.AddWithValue("@IdAutoriza", (object)model.IdOficialBusqueda ?? DBNull.Value);
                        if (model.IdEstatusAccidente != 0)
                            cmd.Parameters.AddWithValue("@IdEstatusAccidente", (object)model.IdEstatusAccidente ?? DBNull.Value);
                        if (model.IdDelegacionBusqueda != null)
                            cmd.Parameters.AddWithValue("@IdDelegacionBusqueda", (object)model.IdDelegacionBusqueda ?? DBNull.Value);
                        if (model.folioBusqueda != null)
                            cmd.Parameters.AddWithValue("@FolioBusqueda", (object)model.folioBusqueda ?? DBNull.Value);
                        if (model.placasBusqueda != null)
                            cmd.Parameters.AddWithValue("@PlacasBusqueda", (object)model.placasBusqueda.ToUpper() ?? DBNull.Value);
                        if (model.propietarioBusqueda != null)
                            cmd.Parameters.AddWithValue("@PropietarioBusqueda", (object)model.propietarioBusqueda ?? DBNull.Value);
                        if (model.serieBusqueda != null)
                            cmd.Parameters.AddWithValue("@SerieBusqueda", (object)model.serieBusqueda ?? DBNull.Value);
                        if (model.conductorBusqueda != null)
                            cmd.Parameters.AddWithValue("@ConductorBusqueda", (object)model.conductorBusqueda ?? DBNull.Value);
                        if (model.FolioEmergenciaBusqueda != null)
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
                                BusquedaEspecialAccidentesModel accidente = new BusquedaEspecialAccidentesModel();
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
                                accidente.folioEmergencia = Convert.IsDBNull(reader["folioEmergencia"]) ? default(int?) : Convert.ToInt32(reader["folioEmergencia"]);
                                accidente.emergenciasId = Convert.IsDBNull(reader["emergenciasId"]) ? default(int?) : Convert.ToInt32(reader["emergenciasId"]);
                                accidente.total = Convert.ToInt32(reader["Total"]);
                                accidente.Numero = Convert.IsDBNull(reader["rowIndex"]) ? 0 : Convert.ToInt32(reader["rowIndex"]);

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
    }
}
 

