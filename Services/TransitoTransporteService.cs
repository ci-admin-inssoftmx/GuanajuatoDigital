using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class TransitoTransporteService : ITransitoTransporteService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public TransitoTransporteService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<TransitoTransporteModel> GetAllTransitoTransporte(int idOficina)
        {
            #region Consulta base
            //var query = @"select top(100) d.IdDeposito,d.IdSolicitud,d.IdDelegacion,d.IdMarca,d.IdSubmarca,d.IdPension,d.IdTramo,
            //            d.IdColor,d.Serie,d.Placa,d.FechaIngreso,d.Folio,d.Km,d.Liberado,d.Autoriza,d.FechaActualizacion,
            //            del.delegacion, d.ActualizadoPor, d.estatus, m.marcaVehiculo,subm.nombreSubmarca,sol.solicitanteNombre,
            //            sol.solicitanteAp,sol.solicitanteAm, col.color,pen.pension,cTra.tramo,
            //            --esto es nuevo
            //            sol.fechaSolicitud,sol.folio, dep.IdDependencia,dep.nombreDependencia,inf.IdInfraccion,inf.FolioInfraccion,
            //            veh.idVehiculo,veh.propietario,veh.numeroEconomico,
            //            g.IdGrua,g.Grua

            //            from depositos d inner join catDelegaciones del on d.idDelegacion= del.idDelegacion
            //            inner join marcasVehiculos m on d.idMarca=m.idMarcaVehiculo
            //            inner join colores col on d.idColor = col.idColor
            //            inner join pensiones pen on d.idPension	= pen.idPension
            //            inner join catTramos cTra  on d.Idtramo=cTra.idTramo
            //            inner join catSubmarcasVehiculos  subm on m.idMarcaVehiculo=subm.idMarcaVehiculo
            //            inner join solicitudes sol on d.idSolicitud = sol.idSolicitud
            //            inner join dependencias dep on sol.IdDependencia = dep.IdDependencia
            //            inner join Infracciones inf on sol.idInfraccion = inf.IdInfraccion
            //            inner join	vehiculos  veh on sol.idVehiculo =veh.idVehiculo 
            //            left join 	GruasDepositos gd on  d.IdDeposito=	 gd.IdDeposito
            //            left join Gruas g on g.IdGrua= gd.IdGrua
            //            where 
            //            --d.liberado=0 and d.estatus=1	and
            //            (d.IdDeposito=@IdDeposito  OR d.IdMarca=@IdMarca 
            //            OR d.Serie LIKE '%' + @Serie + '%' OR d.FechaIngreso =@FechaIngreso 
            //            OR d.Folio LIKE '%' + @Folio + '%')";
            #endregion

            List<TransitoTransporteModel> transitoList = new List<TransitoTransporteModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact =
                                        @"SELECT 
                                            d.iddeposito,
                                            MAX(d.idsolicitud) as idsolicitud,
                                            MAX(d.idDelegacion) as idDelegacion,
                                            MAX(d.idmarca) as idmarca,
                                            MAX(d.idsubmarca) as idsubmarca,
                                            MAX(d.idpension) as idpension,
                                            MAX(d.idtramo) as idtramo,
                                            MAX(d.idcolor) as idcolor,
                                            MAX(d.serie) as serie,
                                            MAX(d.placa) as placa,
                                            MAX(d.fechaingreso) as fechaingreso,
                                            MAX(d.folio) as folio,
                                            MAX(d.km) as km,
                                            MAX(d.liberado) as liberado,
                                            MAX(d.autoriza) as autoriza,
                                            MAX(d.fechaactualizacion) as fechaactualizacion,
                                            MAX(del.delegacion) as delegacion,
                                            MAX(d.actualizadopor) as actualizadopor,
                                            MAX(d.estatus) as estatus,
                                            MAX(m.marcavehiculo) as marcavehiculo,
                                            MAX(subm.nombresubmarca) as nombresubmarca,
                                            MAX(sol.solicitantenombre) as solicitantenombre,
                                            MAX(sol.solicitanteap) as solicitanteap,
                                            MAX(sol.solicitanteam) as solicitanteam,
                                            MAX(col.color) as color,
                                            MAX(pen.pension) as pension,
                                            MAX(ctra.tramo) as tramo,
                                            MAX(sol.fechasolicitud) as fechasolicitud,
                                            MAX(sol.folio) as FolioSolicitud,
                                            MAX(inf.idinfraccion) as idinfraccion,
                                            MAX(inf.folioinfraccion) as folioinfraccion,
                                            MAX(veh.idvehiculo) as idvehiculo,
                                            MAX(veh.propietario) as propietario,
                                            MAX(veh.numeroeconomico) as numeroeconomico,
                                            MAX(veh.modelo) as modelo,
                                            MAX(con.IdConcesionario) as IdConcesionario,
                                            MAX(con.concesionario) as concesionario,
                                            MAX(sd.Fechasalida) as FechaLiberacion,
                                            MAX(d.IdDependenciaGenera) as IdDependenciaGenera,
                                            MAX(d.IdDependenciaTransito) as IdDependenciaTransito,
                                            MAX(d.IdDependenciaNoTransito) as IdDependenciaNoTransito,
                                            MAX(dep.idDependencia) as idDependencia,
                                            MAX(dep.nombreDependencia) as nombreDependencia

                                        from depositos d left join catDelegaciones del on d.idDelegacion= del.idDelegacion
                                        left join catMarcasVehiculos m on d.idMarca=m.idMarcaVehiculo
                                        left join catColores col on d.idcolor = col.idcolor
                                        left join pensiones pen on d.idpension	= pen.idpension
                                        left join catTramos ctra  on d.idtramo=ctra.idtramo
                                        left join catSubmarcasVehiculos  subm on d.idSubmarca=subm.idSubmarca
                                        left join solicitudes sol on d.idsolicitud = sol.idsolicitud
                                        left join infracciones inf on sol.idinfraccion = inf.idinfraccion
                                        left join vehiculos  veh on sol.idvehiculo =veh.idvehiculo 
                                        left join concesionarios con on con.IdConcesionario =d.IdConcesionario
                                        left join catDependencias dep on ((dep.idDependencia=d.IdDependenciaTransito)OR (dep.idDependencia=d.IdDependenciaNoTransito))
                                         inner join serviciosDepositos sd on sd.idDeposito= d.iddeposito
                                        where  sol.estatus !=0 and d.estatus!=0 and d.idDelegacion = @idOficina
                                        GROUP BY d.iddeposito";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))

                    {
                        while (reader.Read())
                        {
                            TransitoTransporteModel transito = new TransitoTransporteModel();
                            transito.IdDeposito = Convert.IsDBNull(reader["IdDeposito"]) ? 0 : Convert.ToInt32(reader["IdDeposito"]);
                            transito.IdSolicitud = Convert.IsDBNull(reader["IdSolicitud"]) ? 0 : Convert.ToInt32(reader["IdSolicitud"]);
                            transito.IdDelegacion = Convert.IsDBNull(reader["IdDelegacion"]) ? 0 : Convert.ToInt32(reader["IdDelegacion"]);
                            transito.IdMarca = Convert.IsDBNull(reader["IdMarca"]) ? 0 : Convert.ToInt32(reader["IdMarca"]);
                            transito.IdSubmarca = Convert.IsDBNull(reader["IdSubmarca"]) ? 0 : Convert.ToInt32(reader["IdSubmarca"]);
                            transito.IdPension = Convert.IsDBNull(reader["IdPension"]) ? 0 : Convert.ToInt32(reader["IdPension"]);
                            transito.IdTramo = Convert.IsDBNull(reader["IdTramo"]) ? 0 : Convert.ToInt32(reader["IdTramo"]);
                            transito.IdColor = Convert.IsDBNull(reader["IdColor"]) ? 0 : Convert.ToInt32(reader["IdColor"]);
                            transito.Serie = reader["Serie"].ToString();
                            transito.Placa = reader["Placa"].ToString();
                            transito.FechaIngreso = Convert.IsDBNull(reader["FechaIngreso"]) ? DateTime.MinValue : Convert.ToDateTime(reader["FechaIngreso"]);
                            transito.FechaLiberacion = Convert.IsDBNull(reader["FechaLiberacion"]) ? DateTime.MinValue : Convert.ToDateTime(reader["FechaLiberacion"]);
                            transito.Folio = reader["Folio"].ToString();
                            transito.Km = reader["Km"].ToString();
                            transito.Liberado = Convert.IsDBNull(reader["Liberado"]) ? 0 : Convert.ToInt32(reader["Liberado"]);
                            transito.Autoriza = reader["Autoriza"].ToString();
                            transito.FechaActualizacion = Convert.IsDBNull(reader["FechaActualizacion"]) ? DateTime.MinValue : Convert.ToDateTime(reader["FechaActualizacion"]);
                            transito.ActualizadoPor = Convert.IsDBNull(reader["ActualizadoPor"]) ? 0 : Convert.ToInt32(reader["ActualizadoPor"]);
                            transito.DepositoEstatus = Convert.IsDBNull(reader["Estatus"]) ? 0 : Convert.ToInt32(reader["Estatus"]);
                            transito.marcaVehiculo = reader["marcaVehiculo"].ToString();
                            transito.nombreSubmarca = reader["nombreSubmarca"].ToString();
                            transito.delegacion = reader["delegacion"].ToString();
                            transito.modelo = reader["modelo"].ToString();
                            transito.solicitanteNombre = reader["solicitanteNombre"].ToString();
                            transito.solicitanteAp = reader["solicitanteAp"].ToString();
                            transito.solicitanteAm = reader["solicitanteAm"].ToString();
                            transito.Color = reader["Color"].ToString();
                            transito.pension = reader["pension"].ToString();
                            transito.tramo = reader["tramo"].ToString();

                            //nuevos
                            transito.FechaSolicitud = Convert.IsDBNull(reader["FechaSolicitud"]) ? DateTime.MinValue : Convert.ToDateTime(reader["FechaSolicitud"]);
                            transito.IdDependencia = Convert.IsDBNull(reader["IdDependencia"]) ? 0 : Convert.ToInt32(reader["IdDependencia"]);
                            transito.NombreDependencia = reader["NombreDependencia"].ToString();
                            transito.IdInfraccion = Convert.IsDBNull(reader["IdInfraccion"]) ? 0 : Convert.ToInt32(reader["IdInfraccion"]);
                            transito.FolioInfraccion = reader["folioInfraccion"].ToString();
                            transito.IdVehiculo = Convert.IsDBNull(reader["IdVehiculo"]) ? 0 : Convert.ToInt32(reader["IdVehiculo"]);
                            transito.propietario = reader["propietario"].ToString();
                            transito.numeroEconomico = reader["numeroEconomico"].ToString();
                            transito.FolioSolicitud = reader["FolioSolicitud"].ToString();
                            transito.IdConcesionario = Convert.IsDBNull(reader["IdConcesionario"]) ? 0 : Convert.ToInt32(reader["IdConcesionario"]);
                            transito.Concesionario = reader["concesionario"].ToString();
                            transito.IdDependenciaGenera = Convert.IsDBNull(reader["IdDependenciaGenera"]) ? 0 : (int)reader["IdDependenciaGenera"];
                            transito.IdDependenciaTransito = Convert.IsDBNull(reader["IdDependenciaTransito"]) ? 0 : (int)reader["IdDependenciaTransito"];
                            transito.IdDependenciaNoTransito = Convert.IsDBNull(reader["IdDependenciaNoTransito"]) ? 0 : (int)reader["IdDependenciaNoTransito"];
                            //sqlreader[indexAge] as int? ?? default(int)
                            transitoList.Add(transito);

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
            return transitoList;
        }

        public List<TransitoTransporteModel> GetTransitoTransportes(TransitoTransporteBusquedaModel model, Pagination pagination, int idOficina)
        {
            List<TransitoTransporteModel> transitoList = new List<TransitoTransporteModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    SqlCommand command = new SqlCommand("dbo.usp_ObtieneListadoTransitoTransporte_WithView", connection);

                    command.Parameters.Add(new SqlParameter("@PageIndex", SqlDbType.Int)).Value = pagination.PageIndex;
                    command.Parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int)).Value = pagination.PageSize;
                    if (pagination.Filter.Trim() != "")
                        command.Parameters.AddWithValue("@Filter", pagination.Filter);


                    if (pagination.Sort != null && pagination.Sort != "")
                    {
                        command.Parameters.AddWithValue("@SortDirection", pagination.Sort);
                        command.Parameters.AddWithValue("@SortMember", pagination.SortCamp);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@SortDirection", "DES");
                    }
                    command.Parameters.Add(new SqlParameter("@Placa", SqlDbType.VarChar)).Value = (model.Placas != null) ? model.Placas.Trim() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Serie", SqlDbType.VarChar)).Value = (model.Serie != null) ? model.Serie.Trim() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@FolioSolicitud", SqlDbType.VarChar)).Value = (model.FolioSolicitud != null) ? model.FolioSolicitud.Trim() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@folioInfraccion", SqlDbType.VarChar)).Value = (model.FolioInfraccion!=null) ? model.FolioInfraccion.Trim() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Propietario", SqlDbType.VarChar)).Value = (model.Propietario!=null) ? model.Propietario.Trim() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@numeroEconomico", SqlDbType.VarChar)).Value = (model.NumeroEconomico!=null) ? model.NumeroEconomico.Trim() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdDelegacion", SqlDbType.Int)).Value = (object)model.IdDelegacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdPension", SqlDbType.Int)).Value = (object)model.IdPension ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdDependenciaNoTransito", SqlDbType.Int)).Value = (object)model.IdDependenciaNoTransito ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdDependenciaTransito", SqlDbType.Int)).Value = (object)model.IdDependenciaTransito ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idEstatus", SqlDbType.Int)).Value = (object)model.IdEstatus ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@FechaIngreso", SqlDbType.Date)).Value = (object)model.FechaIngreso ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@FechaIngresoFin", SqlDbType.Date)).Value = (object)model.FechaIngresoFin ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdTipoEvento", SqlDbType.VarChar)).Value = (object)model.Evento ?? DBNull.Value;

                    command.CommandTimeout = 0;
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TransitoTransporteModel transito = new TransitoTransporteModel();
                            transito.transito = reader["transito"].GetType() == typeof(DBNull) ? 0 : (int)reader["transito"];
                            transito.cons = Convert.ToInt32(reader["rowIndex"].ToString());
                            transito.IdDeposito = Convert.ToInt32(reader["IdDeposito"] is DBNull ? 0 : reader["IdDeposito"]);
                            transito.IdSolicitud = Convert.ToInt32(reader["IdSolicitud"] is DBNull ? 0 : reader["IdSolicitud"]);
                            transito.IdDelegacion = Convert.ToInt32(reader["IdDelegacion"] is DBNull ? 0 : reader["IdDelegacion"]);
                            transito.IdVehiculo = Convert.ToInt32(reader["IdVehiculo"] is DBNull ? 0 : reader["IdVehiculo"]);
                            transito.BanderaTransito = reader["BanderaTransito"] is DBNull ? null : (int?)reader["BanderaTransito"];

                            transito.IdMarca = Convert.ToInt32(reader["IdMarca"] is DBNull ? 0 : reader["IdMarca"]);
                            transito.IdSubmarca = Convert.ToInt32(reader["IdSubmarca"] is DBNull ? 0 : reader["IdSubmarca"]);
                            transito.IdPension = Convert.ToInt32(reader["IdPension"] is DBNull ? 0 : reader["IdPension"]);
                            transito.IdTramo = Convert.ToInt32(reader["IdTramo"] is DBNull ? 0 : reader["IdTramo"]);
                            transito.IdColor = Convert.ToInt32(reader["IdColor"] is DBNull ? 0 : reader["IdColor"]);
                            transito.Serie = reader["Serie"] is DBNull ? "" : reader["Serie"].ToString();
                            transito.Placa = reader["placas"] is DBNull ? "" : reader["placas"].ToString(); 
                            transito.FechaIngreso = Convert.ToDateTime(reader["FechaIngreso"] is DBNull ? DateTime.MinValue : reader["FechaIngreso"]);
                            transito.FechaLiberacion = Convert.ToDateTime(reader["FechaLiberacion"] is DBNull ? DateTime.MinValue : reader["FechaLiberacion"]);
                            transito.Folio = reader["Folio"] is DBNull ? "" : reader["Folio"].ToString();
                            transito.Km = reader["Km"] is DBNull ? "" : reader["Km"].ToString();

                            //transito.Km = reader["Km"] is DBNull ? "" : Convert.ToDecimal(reader["Km"]).ToString("G29");
                            transito.Liberado = Convert.ToInt32(reader["Liberado"] is DBNull ? 0 : reader["Liberado"]);
                            transito.Autoriza = reader["Autoriza"] is DBNull ? "" : reader["Autoriza"].ToString();
                            transito.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            transito.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
                            transito.DepositoEstatus = Convert.ToInt32(reader["Estatus"] is DBNull ? 0 : reader["Estatus"]);
                            transito.marcaVehiculo = reader["marcaVehiculo"] is DBNull ? "" : reader["marcaVehiculo"].ToString();
                            transito.nombreSubmarca = reader["nombreSubmarca"] is DBNull ? "" : reader["nombreSubmarca"].ToString();
                            transito.delegacion = reader["delegacion"] is DBNull ? "" : reader["delegacion"].ToString();
                            transito.modelo = reader["modelo"] is DBNull ? "" : reader["modelo"].ToString();
                            transito.solicitanteNombre = reader["solicitanteNombre"] is DBNull ? "" : reader["solicitanteNombre"].ToString();
                            transito.solicitanteAp = reader["solicitanteAp"] is DBNull ? "" : reader["solicitanteAp"].ToString();
                            transito.solicitanteAm = reader["solicitanteAm"] is DBNull ? "" : reader["solicitanteAm"].ToString();
                            transito.Color = reader["Color"] is DBNull ? "" : reader["Color"].ToString();
                            transito.pension = reader["pension"] is DBNull ? "" : reader["pension"].ToString();
                            transito.tramo = reader["tramo"] is DBNull ? "" : reader["tramo"].ToString();
                            transito.IdGrua = Convert.ToInt32(reader["idGrua"] is DBNull ? 0 : reader["idGrua"]);

                            // Nuevos
                            transito.FechaSolicitud = Convert.ToDateTime(reader["FechaSolicitud"] is DBNull ? DateTime.MinValue : reader["FechaSolicitud"]);
                            transito.IdDependencia = Convert.ToInt32(reader["IdDependencia"] is DBNull ? 0 : reader["IdDependencia"]);
                            transito.NombreDependencia = reader["NombreDependencia"] is DBNull ? "" : reader["NombreDependencia"].ToString();
                            transito.IdInfraccion = Convert.ToInt32(reader["IdInfraccion"] is DBNull ? 0 : reader["IdInfraccion"]);
                            transito.FolioInfraccion = reader["folioInfraccion"] is DBNull ? "" : reader["folioInfraccion"].ToString();
                            transito.propietario = reader["propietario"] is DBNull ? "" : reader["propietario"].ToString();
                            transito.numeroEconomico = reader["numeroEconomico"] is DBNull ? "" : reader["numeroEconomico"].ToString();
                            transito.FolioSolicitud = reader["FolioSolicitud"] is DBNull ? "" : reader["FolioSolicitud"].ToString();
                            transito.IdConcesionario = Convert.ToInt32(reader["IdConcesionario"] is DBNull ? 0 : reader["IdConcesionario"]);
                            transito.estatusSolicitud = string.Concat(reader["nombreEstatus"], " ", reader["delegacion"]);
                            transito.nombreEstatus = reader["nombreEstatus"] is DBNull ? "" : reader["nombreEstatus"].ToString();
                            transito.SolicitudEstatus = Convert.ToInt32(reader["estatusSolicitud"] is DBNull ? 0 : reader["estatusSolicitud"]);
                            transito.Concesionario = reader["concesionario"] is DBNull ? "" : reader["concesionario"].ToString();
                            transito.IdDependenciaGenera = reader["IdDependenciaGenera"] is DBNull ? 0 : (int)reader["IdDependenciaGenera"];
                            transito.IdDependenciaTransito = reader["IdDependenciaTransito"] is DBNull ? 0 : (int)reader["IdDependenciaTransito"];
                            transito.IdDependenciaNoTransito = reader["IdDependenciaNoTransito"] is DBNull ? 0 : (int)reader["IdDependenciaNoTransito"];
                            transito.evento  = reader["Evento"] is DBNull ? "" : reader["Evento"].ToString();
                            transito.total = Convert.ToInt32(reader["Total"].ToString());
                            transitoList.Add(transito);

                        }
                        reader.Close();
                    }

                }
                catch (SqlException ex)
                {
                    //Guardar la excepcion en algun log de errores
                    //ex
                    var q = ex;
                }
                finally
                {
                    connection.Close();
                }
            return transitoList;
        }

        public TransitoTransporteModel GetTransitoTransporteById(int IdDeposito)
        {
         TransitoTransporteModel transito = new TransitoTransporteModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                 

                    const string SqlTransact =
                                       @"SELECT d.idDeposito,d.placa,d.idInfraccion,d.fechaIngreso,d.idVehiculo,d.fechaLiberacion,
                                                sol.idSolicitud,sol.fechaSolicitud,sol.idEvento,sol.idTipoUsuario,
                                                sol.solicitanteNombre,sol.solicitanteAp,sol.solicitanteAm,sol.folio,sol.solicitanteMunicipio,
	                                            sol.vehiculoCalle,sol.vehiculoNumero,sol.vehiculoColonia,sol.idCarreteraUbicacion,
	                                            sol.idTramoUbicacion,sol.idPension,sol.vehiculoInterseccion,sol.vehiculoKm,sol.solicitanteColonia,
	                                            sol.solicitanteCalle,sol.solicitanteNumero,con.concesionario,
	                                            csr.nombreServicio as tipoServicio,
                                                tas.tipoAsignacion,munSol.municipio AS MunicipioSolicitante,entSol.nombreEntidad AS EntidadSolicitante,
                                                te.descripcionEvento,tu.tipoUsuario,tve.tipoVehiculo,c.concesionario,
                                                ga.abanderamiento,ga.arrastre,ga.salvamento,ofi.nombre,ofi.apellidoPaterno,ofi.apellidoMaterno,
                                                ga.fechaArribo,ga.fechaInicio,ga.fechaFinal,ga.minutosManiobra,ga.operadorGrua,
	                                            ent.nombreEntidad,mun.municipio,v.idPersona,v.serie,per.nombre AS nombrePropietario,per.apellidoPaterno AS propietarioAP,
                                                per.apellidoMaterno AS propietarioAM,per.CURP,per.RFC,v.modelo,mv.marcaVehiculo,sbv.nombreSubmarca,
                                                inf.folioInfraccion,sde.fechaSalida,car.carretera AS carreteraUbicacion,tra.tramo AS tramoUbicacion,
	                                            pen.pension,g.noEconomico,g.idTipoGrua,g.placas AS placasGrua,ctg.TipoGrua
                                                FROM depositos AS d
                                                LEFT JOIN solicitudes AS sol ON sol.idSolicitud = d.idSolicitud
                                                LEFT JOIN catDescripcionesEvento AS te ON te.idDescripcion = sol.idEvento
                                                LEFT JOIN catTiposUsuario AS tu ON tu.idTipoUsuario = sol.idTipoUsuario
                                                LEFT JOIN catTiposVehiculo AS tve ON tve.idTipoVehiculo = sol.idTipoVehiculo
                                                LEFT JOIN concesionarios AS c ON c.idConcesionario = sol.idPropietarioGrua
                                                LEFT JOIN gruasAsignadas AS ga ON ga.idDeposito = d.idDeposito
                                                LEFT JOIN gruas AS g ON ga.idGrua = g.idGrua 
	                                            LEFT JOIN catTipoGrua AS ctg ON ctg.IdTipoGrua = g.idTipoGrua 
	                                            LEFT JOIN catOficiales AS ofi ON ofi.idOficial = sol.idOficial
                                                LEFT JOIN tipoMotivoAsignacion AS tas ON tas.idTipoAsignacion = sol.idMotivoAsignacion
                                                LEFT JOIN catEntidades AS ent ON ent.idEntidad = sol.idEntidadUbicacion
                                                LEFT JOIN catEntidades AS entSol ON entSol.idEntidad = sol.idEntidad
                                                LEFT JOIN catMunicipios AS mun ON mun.idMunicipio = sol.idMunicipioUbicacion
                                                LEFT JOIN catMunicipios AS munSol ON munSol.idMunicipio = sol.idMunicipio
	                                            LEFT JOIN catCarreteras AS car ON car.idCarretera = sol.idCarreteraUbicacion
                                                LEFT JOIN catTramos AS tra ON tra.idTramo = sol.idTramoUbicacion

                                                LEFT JOIN opeDepositosVehiculos v ON v.idOperacion = 
										        (
											        SELECT MAX(idOperacion) 
											        FROM opeDepositosVehiculos z 
											        WHERE z.idVehiculo = d.idVehiculo and z.idDeposito = @IdDeposito
										        )

                                                LEFT JOIN catMarcasVehiculos AS mv ON mv.idMarcaVehiculo = v.idMarcaVehiculo
                                                LEFT JOIN catSubmarcasVehiculos AS sbv ON sbv.idSubmarca = v.idSubmarca

                                                LEFT JOIN opeDepositosPersonas per ON per.idOperacion = 
                                                (
                                                SELECT MAX(idOperacion) 
                                                FROM opeDepositosPersonas z 
                                                WHERE z.idPersona = v.idPersona and z.idDeposito = @IdDeposito
                                                )

                                                LEFT JOIN serviciosDepositos AS sde ON sde.idDeposito = d.idDeposito
                                                LEFT JOIN infracciones AS inf ON inf.idInfraccion = d.idInfraccion
                                                LEFT JOIN pensiones AS pen ON pen.idPension = d.idPension
	                                            LEFT JOIN concesionarios AS con ON con.idConcesionario = sol.idPropietarioGrua
	                                            LEFT JOIN catServicioRequiere AS csr ON sol.idServicioRequiere = csr.idServicio             
                                                 where  sol.estatus !=0 and d.estatus!=0 and	d.idDeposito=@idDeposito ";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@idDeposito", SqlDbType.Int)).Value = (object)IdDeposito ?? DBNull.Value;


                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            transito.IdDeposito = reader["idDeposito"] != DBNull.Value && int.TryParse(reader["idDeposito"].ToString(), out int idDeposito) ? idDeposito : 0;
                            transito.IdSolicitud = reader["idSolicitud"] != DBNull.Value && int.TryParse(reader["idSolicitud"].ToString(), out int idSolicitud) ? idSolicitud : 0;
                            transito.salvamento = reader["salvamento"] != DBNull.Value && int.TryParse(reader["salvamento"].ToString(), out int salvamento) ? salvamento : 0;
                            transito.arrastre = reader["arrastre"] != DBNull.Value && int.TryParse(reader["arrastre"].ToString(), out int arrastre) ? arrastre : 0;
                            transito.abanderamiento = reader["abanderamiento"] != DBNull.Value && int.TryParse(reader["abanderamiento"].ToString(), out int abanderamiento) ? abanderamiento : 0;
                            transito.minutosManiobra = reader["minutosManiobra"] != DBNull.Value && int.TryParse(reader["minutosManiobra"].ToString(), out int minutosManiobra) ? minutosManiobra : 0;
                            transito.Serie = reader["serie"].ToString();
                            transito.Placa = reader["placa"].ToString();
                            string fechaIngresoStr = reader["fechaIngreso"].ToString();
                            if (DateTime.TryParse(fechaIngresoStr, out DateTime fechaIngreso) && fechaIngreso != DateTime.MinValue)
                            {
                                transito.FechaIngreso = fechaIngreso;
                            }
                            else
                            {
                        
                            }
                            string fechaLiberacionStr = reader["fechaLiberacion"].ToString();
                            if (DateTime.TryParse(fechaLiberacionStr, out DateTime fechaLiberacion) && fechaLiberacion != DateTime.MinValue)
                            {
                                transito.FechaLiberacion = fechaLiberacion;
                            }
                            else
                            {

                            }
                            transito.Folio = reader["folio"].ToString();
                            transito.evento = reader["descripcionEvento"].ToString();
                            transito.tipoVehiculo = reader["tipoVehiculo"].ToString();

                            if (reader["vehiculoKm"] != null)
                            {

                                transito.Km = reader["vehiculoKm"].ToString();
                            }
                            

                             transito.tipoUsuario = reader["tipoUsuario"].ToString();
                            transito.marcaVehiculo = reader["marcaVehiculo"].ToString();
                            transito.nombreSubmarca = reader["nombreSubmarca"].ToString();
                            transito.motivoAsignacion = reader["tipoAsignacion"].ToString();
                            transito.modelo = reader["modelo"].ToString();

                            transito.solicitanteNombre = reader["solicitanteNombre"].ToString();
                            transito.solicitanteAp = reader["solicitanteAp"].ToString();
                            transito.solicitanteAm = reader["solicitanteAm"].ToString();

							transito.solicitanteColonia = reader["solicitanteColonia"].ToString();
							transito.solicitanteCalle = reader["solicitanteCalle"].ToString();
							transito.solicitanteNumero = reader["solicitanteNumero"].ToString();
                            transito.solicitanteMunicipio = reader["MunicipioSolicitante"].ToString();
                            transito.solicitanteEntidad = reader["EntidadSolicitante"].ToString();

                            transito.pension = reader["pension"].ToString();
                            transito.tramo = reader["tramoUbicacion"].ToString();
                            transito.calle = reader["vehiculoCalle"].ToString();
						
							transito.carretera = reader["carreteraUbicacion"].ToString();
                            transito.numero = reader["vehiculoNumero"].ToString();
                            transito.colonia = reader["vehiculoColonia"].ToString();
                            transito.interseccion = reader["vehiculoInterseccion"].ToString();
                            transito.municipio = reader["municipio"].ToString();

                            //nuevos
                            string fechaSolicitudStr = reader["fechaSolicitud"].ToString();

                            if (DateTime.TryParse(fechaSolicitudStr, out DateTime fechaSolicitud) && fechaSolicitud != DateTime.MinValue)
                            {
                                // La conversión fue exitosa y la fecha no es igual a DateTime.MinValue, puedes utilizar 'fechaSolicitud' aquí
                                transito.FechaSolicitud = fechaSolicitud;
                            }
                            else
                            {
                           
                            }
                            transito.FolioInfraccion = reader["folioInfraccion"].ToString();
                           // transito.IdVehiculo = Convert.ToInt32(reader["idVehiculo"].ToString());
                            transito.propietarioNombre = reader["nombrePropietario"].ToString();
                            transito.propietarioApellidoPaterno = reader["propietarioAP"].ToString();
                            transito.propietarioApellidoMaterno = reader["propietarioAM"].ToString();
    
							transito.CURP = reader["CURP"].ToString() + " / ";

                            if (reader["RFC"]!=null)
                            {
                                transito.CURP = transito.CURP+reader["RFC"].ToString();
                            }

							transito.oficialNombre = reader["nombre"].ToString();
                            transito.oficialApellidoPaterno = reader["apellidoPaterno"].ToString();
                            transito.oficialApellidoMaterno = reader["apellidoMaterno"].ToString();

                            transito.numeroEconomico = reader["noEconomico"].ToString();
                            transito.FolioSolicitud = reader["folio"].ToString();
                            transito.tipoGrua = reader["TipoGrua"].ToString();
                            transito.placasGrua = reader["placasGrua"].ToString();
                            transito.Concesionario = reader["concesionario"].ToString();

                            if (reader["tipoServicio"] != null)
                            {
                                transito.tipoServicio = reader["tipoServicio"].ToString();
                            }
                            transito.operador  = reader["operadorGrua"].ToString();
                            string fechaArriboStr = reader["fechaArribo"].ToString();
                            if (DateTime.TryParse(fechaArriboStr, out DateTime fechaArribo) && fechaArribo != DateTime.MinValue)
                            {
                                transito.FechaArribo = fechaArribo;
                            }
                            else
                            {

                            }
                            string fechaInicioStr = reader["fechaInicio"].ToString();
                            if (DateTime.TryParse(fechaInicioStr, out DateTime fechaInicio) && fechaInicio != DateTime.MinValue)
                            {
                                transito.FechaInicio = fechaInicio;
                            }
                            else
                            {

                            }
                            string fechaFinalStr = reader["fechaFinal"].ToString();
                            if (DateTime.TryParse(fechaFinalStr, out DateTime fechaFinal) && fechaFinal != DateTime.MinValue)
                            {
                                transito.FechaFinal = fechaFinal;
                            }
                            else
                            {

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
            return transito;
        }


        /// <summary>
        /// Servicio Provicional ya que no tenemos acceso a repositorio
        /// </summary>
        /// <returns></returns>
        public List<Pensiones> GetPensiones()
        {
            List<Pensiones> pensiones = new List<Pensiones>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from pensiones", connection);
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 3000;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Pensiones pension = new Pensiones();
                            pension.IdPension = Convert.ToInt32(reader["idPension"].ToString());
                            pension.Pension = reader["pension"].ToString();
                            pensiones.Add(pension);
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
            return pensiones;
        }



        public int DeleteTransitoTransporte(int IdDeposito, int IdSolicitud)
        {
            int result = 0;
            if ((IdDeposito != null || IdDeposito != 0) && (IdSolicitud != null || IdSolicitud != 0))
            {
                using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                {
                    try
                    {
                        connection.Open();
                        const string SqlTransact =
                            @"UPDATE depositos
                        SET estatus= 0, fechaAutorizacion = @fechaAutorizacion
                        WHERE idDeposito =@IdDeposito

                        UPDATE solicitudes
                        SET estatus= 0
                        WHERE idSolicitud =@IdSolicitud";

                        SqlCommand command = new SqlCommand(SqlTransact, connection);
                        command.Parameters.Add(new SqlParameter("@IdDeposito", SqlDbType.Int)).Value = (object)IdDeposito ?? DBNull.Value;
                        command.Parameters.Add(new SqlParameter("@IdSolicitud", SqlDbType.Int)).Value = (object)IdSolicitud ?? DBNull.Value;
                        command.Parameters.Add(new SqlParameter("@fechaAutorizacion", SqlDbType.DateTime)).Value = DateTime.Now;

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
            }
            return result;

        }
    }
}
