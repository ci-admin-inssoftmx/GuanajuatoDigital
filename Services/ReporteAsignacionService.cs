using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class ReporteAsignacionService : IReporteAsignacionService
    {

        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public ReporteAsignacionService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<ReporteAsignacionModel> GetAllReporteAsignaciones(int idOficina)
        {
            List<ReporteAsignacionModel> ReporteAsignacionesList = new List<ReporteAsignacionModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact =
                                   @"SELECT dep.iddeposito,
                                   MAX(dep.idsolicitud) AS idsolicitud,
                                   MAX(dep.idDelegacion) AS idDelegacion,
                                   MAX(dep.idmarca) AS idmarca,
                                   MAX(dep.idsubmarca) AS idsubmarca,
                                   MAX(dep.idpension) AS idpension,
                                   MAX(dep.idtramo) AS idtramo,
                                   MAX(dep.idcolor) AS idcolor,
                                   MAX(dep.serie) AS serie,
                                   MAX(dep.placa) AS placa,
                                   MAX(dep.fechaingreso) AS fechaingreso,
                                   MAX(dep.folio) AS folio,
                                   MAX(dep.numeroInventario) AS numeroInventario,
                                   MAX(dep.km) AS km,
                                   MAX(dep.liberado) AS liberado,
                                   MAX(dep.autoriza) AS autoriza,
                                   MAX(dep.fechaactualizacion) AS fechaactualizacion,
                                   MAX(del.delegacion) AS delegacion,
                                   MAX(dep.actualizadopor) AS actualizadopor,
                                   MAX(dep.estatus) AS estatus,
                                   MAX(dep.FechaLiberacion) AS FechaLiberacion,
                                   MAX(dep.IdDependenciaGenera) AS IdDependenciaGenera,
                                   MAX(dep.IdDependenciaTransito) AS IdDependenciaTransito,
                                   MAX(dep.IdDependenciaNoTransito) AS IdDependenciaNoTransito,
                                   MAX(sol.solicitantenombre) AS solicitantenombre,
                                   MAX(sol.solicitanteap) AS solicitanteap,
                                   MAX(sol.solicitanteam) AS solicitanteam,
                                   MAX(pen.pension) AS pension,
                                   MAX(pen.estatus) AS pensionEstatus,
                                   MAX(e.estatusDesc) AS descripcionEstatus,
                                   MAX(sol.vehiculoCarretera) AS vehiculoCarretera,
                                   MAX(sol.vehiculoTramo) AS vehiculoTramo,
                                   MAX(sol.vehiculoKm) AS vehiculoKm,
                                   MAX(sol.idMotivoAsignacion) AS idMotivoAsignacion,
                                   MAX(ta.tipoAsignacion) AS tipoAsignacion,
                                   MAX(sol.fechasolicitud) AS fechasolicitud,
                                   MAX(sol.folio) AS FolioSolicitud,
                                   MAX(sol.evento) AS evento,
                                   MAX(sol.solicitanteColonia) AS solicitanteColonia,
                                   MAX(sol.solicitanteCalle) AS solicitanteCalle,
                                   MAX(sol.solicitanteNumero) AS solicitanteNumero,
                                   MAX(sol.tipoVehiculo) AS tipoVehiculo,
                                   MAX(sol.oficial) AS oficial,
                                   MAX(sol.folio) AS folio,
                                   MAX(sol.propietarioGrua) AS propietarioGrua,
                                   MAX(g.IdGrua) AS IdGrua,
                                   MAX(g.noEconomico) AS noEconomico,
                                   MAX(con.IdConcesionario) AS IdConcesionario,
                                   MAX(con.concesionario) AS concesionario
                                    FROM depositos dep
                                    LEFT JOIN catDelegaciones del ON dep.idDelegacion = del.idDelegacion
                                    LEFT JOIN pensiones pen ON dep.idpension = pen.idpension
                                    LEFT JOIN solicitudes sol ON dep.idsolicitud = sol.idsolicitud
                                    LEFT JOIN tipoMotivoAsignacion ta ON ta.idTipoAsignacion = sol.idMotivoAsignacion
                                    LEFT JOIN concesionarios con ON con.IdConcesionario = dep.IdConcesionario
                                    LEFT JOIN gruas g ON g.idConcesionario = con.idConcesionario
                                    LEFT JOIN estatus e ON e.estatus = pen.estatus
                                    WHERE dep.idDelegacion = @idOficina
                                    GROUP BY dep.idDeposito;
                                    ";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ReporteAsignacionModel ReporteAsignacion = new ReporteAsignacionModel();
                            ReporteAsignacion.idSolicitud = reader["idSolicitud"] != DBNull.Value ? Convert.ToInt32(reader["idSolicitud"].ToString()) : 0;
                            ReporteAsignacion.pensionEstatus = reader["pensionEstatus"] != DBNull.Value ? Convert.ToInt32(reader["idSolicitud"].ToString()) : 0;
                            ReporteAsignacion.vehiculoCarretera = reader["vehiculoCarretera"] != DBNull.Value ? reader["vehiculoCarretera"].ToString() : string.Empty;
                            ReporteAsignacion.vehiculoTramo = reader["vehiculoTramo"] != DBNull.Value ? reader["vehiculoTramo"].ToString() : string.Empty;
                            ReporteAsignacion.vehiculoKm = reader["vehiculoKm"] != DBNull.Value ? reader["vehiculoKm"].ToString() : string.Empty;
                            ReporteAsignacion.fechaSolicitud = reader["fechaSolicitud"] != DBNull.Value ? Convert.ToDateTime(reader["fechaSolicitud"].ToString()) : DateTime.MinValue;
                            ReporteAsignacion.evento = reader["evento"] != DBNull.Value ? reader["evento"].ToString() : string.Empty;
                            ReporteAsignacion.solicitanteNombre = reader["solicitanteNombre"] != DBNull.Value ? reader["solicitanteNombre"].ToString() : string.Empty;
                            ReporteAsignacion.solicitanteAp = reader["solicitanteAp"] != DBNull.Value ? reader["solicitanteAp"].ToString() : string.Empty;
                            ReporteAsignacion.solicitanteAm = reader["solicitanteAm"] != DBNull.Value ? reader["solicitanteAm"].ToString() : string.Empty;
                            ReporteAsignacion.solicitanteColonia = reader["solicitanteColonia"] != DBNull.Value ? reader["solicitanteColonia"].ToString() : string.Empty;
                            ReporteAsignacion.solicitanteCalle = reader["solicitanteCalle"] != DBNull.Value ? reader["solicitanteCalle"].ToString() : string.Empty;
                            ReporteAsignacion.solicitanteNumero = reader["solicitanteNumero"] != DBNull.Value ? reader["solicitanteNumero"].ToString() : string.Empty;
                            ReporteAsignacion.tipoVehiculo = reader["tipoVehiculo"] != DBNull.Value ? reader["tipoVehiculo"].ToString() : string.Empty;
                            ReporteAsignacion.idMotivo = reader["idMotivoAsignacion"] != DBNull.Value ? Convert.ToInt32(reader["idMotivoAsignacion"].ToString()) : 0;
                            ReporteAsignacion.motivoAsignacion = reader["tipoAsignacion"] != DBNull.Value ? reader["tipoAsignacion"].ToString() : string.Empty;

                            ReporteAsignacion.propietarioGrua = reader["propietarioGrua"] != DBNull.Value ? reader["propietarioGrua"].ToString() : string.Empty;
                           // ReporteAsignacion.oficial = reader["oficial"] != DBNull.Value ? reader["oficial"].ToString() : string.Empty;
                            ReporteAsignacion.folio = reader["folio"] != DBNull.Value ? reader["folio"].ToString() : string.Empty;
                            ReporteAsignacion.vehiculoPension = reader["pension"] != DBNull.Value ? reader["pension"].ToString() : string.Empty;
                            ReporteAsignacion.fechaLiberacion = reader["fechaLiberacion"] != DBNull.Value ? Convert.ToDateTime(reader["fechaLiberacion"].ToString()) : DateTime.MinValue;
                            ReporteAsignacion.IdGrua = reader["IdGrua"] != DBNull.Value ? Convert.ToInt32(reader["IdGrua"].ToString()) : 0;
                            ReporteAsignacion.noEconomico = reader["noEconomico"] != DBNull.Value ? reader["noEconomico"].ToString() : string.Empty;
                            ReporteAsignacion.Delegacion = reader["Delegacion"] != DBNull.Value ? reader["Delegacion"].ToString() : string.Empty;
                            ReporteAsignacion.Alias = reader["concesionario"] != DBNull.Value ? reader["concesionario"].ToString() : string.Empty;
                            ReporteAsignacion.numeroIventario = reader["numeroInventario"] != DBNull.Value ? reader["numeroInventario"].ToString() : string.Empty;

                            ReporteAsignacionesList.Add(ReporteAsignacion);

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
            return ReporteAsignacionesList;
        }

        public List<ReporteAsignacionModel> GetAllReporteAsignaciones(ReporteAsignacionBusquedaModel model, int idOficina)
        {
            List<ReporteAsignacionModel> ReporteAsignacionesList = new List<ReporteAsignacionModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {

                    connection.Open();
                    string condiciones = "";
                    condiciones += model.idDelegacion.Equals(null) || model.IdGrua == 0 ? "" : " AND d.idDelegacion = @IdDelegacion ";

                    condiciones += model.IdGrua.Equals(null) || model.IdGrua == 0 ? "" : " AND g.idConcesionario = @IdGrua ";
                    condiciones += model.IdPension.Equals(null) || model.IdPension == 0 ? "" : " AND d.idPension = @IdPension ";
                    condiciones += model.IdEvento.Equals(null) || model.IdEvento == 0 ? "" : " AND sol.idEvento = @IdEvento ";


                    if (model.FechaInicio != DateTime.MinValue && model.FechaFin != DateTime.MinValue)
                    {
                        condiciones += @" AND (CONVERT(VARCHAR,sol.fechasolicitud,112)
                                           BETWEEN CONVERT(VARCHAR,ISNULL(@FechaIngreso,sol.fechasolicitud),112)
                                            AND CONVERT(VARCHAR,ISNULL(@FechaIngresoFin,sol.fechasolicitud),112)) ";
                    }

                    //if (!model.Evento.IsNullOrEmpty())
                    //{
                    //    if (model.Evento.ToUpper().Contains("SELECCIONE EVENTO"))
                    //    {
                    //        condiciones += model.Evento.IsNullOrEmpty() ? "" : "UPPER(t.evento) = UPPER(@Evento)";
                    //    }
                    //}
                    
                    if (string.IsNullOrEmpty(condiciones.Trim()))
                    {
                        condiciones = "";
                    }
                    string SqlTransact =
                                     @"SELECT sol.idSolicitud,sol.vehiculoCarretera, sol.vehiculoTramo,sol.fechaSolicitud,
                                    ISNULL(sol.vehiculoKm,'') vehiculoKm, sol.idEvento,sol.idMotivoAsignacion,sol.solicitanteNombre,sol.idPension,
                                    sol.solicitanteAp,sol.solicitanteAm,sol.solicitanteEntidad,sol.solicitanteMunicipio,sol.solicitanteColonia,
                                    sol.solicitanteCalle,sol.solicitanteNumero,sol.idTipoVehiculo,sol.folio,ga.idGrua,ga.idConcesionario,
                                    d.fechaLiberacion,d.numeroInventario,c.concesionario,c.alias,cOfi.nombre,cOfi.apellidoPaterno,cOfi.apellidoMaterno,p.pension,
                                    carr.carretera,tra.tramo,mot.tipoAsignacion,ev.descripcionEvento,mun.municipio,tve.tipoVehiculo,cdel.nombreOficina, ga.idConcesionario

                                    FROM solicitudes sol
                                    LEFT JOIN depositos d on d.idSolicitud = sol.idSolicitud
                                    LEFT JOIN gruasAsignadas ga ON ga.idDeposito = d.idDeposito
                                    LEFT JOIN gruas g ON g.idGrua = ga.idGrua
                                    LEFT JOIN concesionarios c ON c.idConcesionario = ga.idConcesionario
                                    LEFT JOIN catOficiales cOfi ON cOfi.idOficial = sol.idOficial
                                    LEFT JOIN pensiones p ON p.idPension = sol.idPension
                                    LEFT JOIN catDelegacionesOficinasTransporte cdel ON cdel.idOficinaTransporte = d.idDelegacion
                                    LEFT JOIN catCarreteras carr ON carr.idCarretera = sol.idCarreteraUbicacion
                                    LEFT JOIN catTramos tra ON tra.idTramo = sol.idTramoUbicacion
                                    LEFT JOIN tipoMotivoAsignacion mot ON mot.idTipoAsignacion = sol.idMotivoAsignacion
                                    LEFT JOIN  catDescripcionesEvento ev ON ev.idDescripcion = sol.idEvento
                                    LEFT JOIN catMunicipios mun ON mun.idMunicipio = sol.solicitanteMunicipio
                                    LEFT JOIN catTiposVehiculo tve ON tve.idTipoVehiculo = sol.idTipoVehiculo
                                    WHERE sol.estatus = 1 " + condiciones;                          


                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@IdDelegacion", SqlDbType.Int)).Value = (object)model.idDelegacion ?? DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@IdGrua", SqlDbType.Int)).Value = (object)model.IdGrua ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdPension", SqlDbType.Int)).Value = (object)model.IdPension ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdEvento", SqlDbType.Int)).Value = (object)model.IdEvento != null ? model.IdEvento : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@FechaIngreso", SqlDbType.DateTime)).Value = (model.FechaInicio == DateTime.MinValue) ? DBNull.Value : (object)model.FechaInicio;
                    command.Parameters.Add(new SqlParameter("@FechaIngresoFin", SqlDbType.DateTime)).Value = (model.FechaFin == DateTime.MinValue) ? DBNull.Value : (object)model.FechaFin;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ReporteAsignacionModel ReporteAsignacion = new ReporteAsignacionModel();
                            ReporteAsignacion.idSolicitud = reader["idSolicitud"] != DBNull.Value ? Convert.ToInt32(reader["idSolicitud"].ToString()) : 0;
                            ReporteAsignacion.vehiculoCarretera = reader["vehiculoCarretera"] != DBNull.Value ? reader["vehiculoCarretera"].ToString() : string.Empty;

                            ReporteAsignacion.carretera = reader["carretera"] != DBNull.Value ? reader["carretera"].ToString() : string.Empty;
                            ReporteAsignacion.tramo = reader["tramo"] != DBNull.Value ? reader["tramo"].ToString() : string.Empty;

                            ReporteAsignacion.vehiculoTramo = reader["vehiculoTramo"] != DBNull.Value ? reader["vehiculoTramo"].ToString() : string.Empty;
                            ReporteAsignacion.vehiculoKm = reader["vehiculoKm"] != DBNull.Value ? reader["vehiculoKm"].ToString() : string.Empty;
                            ReporteAsignacion.fechaSolicitud = (DateTime)(reader["fechaSolicitud"] != DBNull.Value
                          ? Convert.ToDateTime(reader["fechaSolicitud"])
                          : (DateTime?)DateTime.MinValue);

                            ReporteAsignacion.evento = reader["descripcionEvento"] != DBNull.Value ? reader["descripcionEvento"].ToString() : string.Empty;
                            ReporteAsignacion.motivoAsignacion = reader["tipoAsignacion"] != DBNull.Value ? reader["tipoAsignacion"].ToString() : string.Empty;

                            ReporteAsignacion.solicitanteNombre = reader["solicitanteNombre"] != DBNull.Value ? reader["solicitanteNombre"].ToString() : string.Empty;
                            ReporteAsignacion.solicitanteAp = reader["solicitanteAp"] != DBNull.Value ? reader["solicitanteAp"].ToString() : string.Empty;
                            ReporteAsignacion.solicitanteAm = reader["solicitanteAm"] != DBNull.Value ? reader["solicitanteAm"].ToString() : string.Empty;
                            ReporteAsignacion.solicitanteColonia = reader["solicitanteColonia"] != DBNull.Value ? reader["solicitanteColonia"].ToString() : string.Empty;
                            ReporteAsignacion.solicitanteCalle = reader["solicitanteCalle"] != DBNull.Value ? reader["solicitanteCalle"].ToString() : string.Empty;
                            ReporteAsignacion.solicitanteNumero = reader["solicitanteNumero"] != DBNull.Value ? reader["solicitanteNumero"].ToString() : string.Empty;
                            ReporteAsignacion.tipoVehiculo = reader["tipoVehiculo"] != DBNull.Value ? reader["tipoVehiculo"].ToString() : string.Empty;
                            ReporteAsignacion.Alias = reader["alias"] != DBNull.Value ? reader["alias"].ToString() : string.Empty;
                            //ReporteAsignacion.oficial = reader["oficial"] != DBNull.Value ? reader["oficial"].ToString() : string.Empty;
                            ReporteAsignacion.folio = reader["folio"] != DBNull.Value ? reader["folio"].ToString() : string.Empty;
                            ReporteAsignacion.vehiculoPension = reader["pension"] != DBNull.Value ? reader["pension"].ToString() : string.Empty;
                            ReporteAsignacion.fechaLiberacion = reader["fechaLiberacion"] != DBNull.Value ? Convert.ToDateTime(reader["fechaLiberacion"].ToString()) : (DateTime?)null;
                            ReporteAsignacion.IdGrua = reader["IdGrua"] != DBNull.Value ? Convert.ToInt32(reader["IdGrua"].ToString()) : 0;
                           // ReporteAsignacion.grua = reader["noEconomico"] != DBNull.Value ? reader["noEconomico"].ToString() : string.Empty;
                            ReporteAsignacion.Delegacion = reader["nombreOficina"] != DBNull.Value ? reader["nombreOficina"].ToString() : string.Empty;
                            ReporteAsignacion.propietarioGrua = reader["concesionario"] != DBNull.Value ? reader["concesionario"].ToString() : string.Empty;
                            ReporteAsignacion.numeroIventario = reader["numeroInventario"] != DBNull.Value ? reader["numeroInventario"].ToString() : string.Empty;
                            ReporteAsignacion.nombreOficial = reader["nombre"] != DBNull.Value ? reader["nombre"].ToString() : string.Empty;
                            ReporteAsignacion.apellidoPaternoOficial = reader["apellidoPaterno"] != DBNull.Value ? reader["apellidoPaterno"].ToString() : string.Empty;
                            ReporteAsignacion.apellidoMaternoOficial = reader["apellidoMaterno"] != DBNull.Value ? reader["apellidoMaterno"].ToString() : string.Empty;

                            ReporteAsignacionesList.Add(ReporteAsignacion);
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
            return ReporteAsignacionesList;

        }


    }
}
