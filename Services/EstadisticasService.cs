using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services
{
    public class EstadisticasService : IEstadisticasService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        private readonly IPersonasService _personasService;
        private readonly IVehiculosService _vehiculosService;
        private readonly IInfraccionesService _infraccionesService;
        public EstadisticasService(ISqlClientConnectionBD sqlClientConnectionBD,
            IVehiculosService vehiculosService,
            IPersonasService personasService,
            IInfraccionesService infraccionesService)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
            _vehiculosService = vehiculosService;
            _personasService = personasService;
            _infraccionesService = infraccionesService;
        }

        public IEnumerable<IncidenciasInfraccionesModel> IncidenciasInfracciones(IncidenciasBusquedaModel model)
        {
            List<IncidenciasInfraccionesModel> modelList = new List<IncidenciasInfraccionesModel>();
            string strQuery = @"SELECT inf.idInfraccion
                                    ,inf.idOficial
                                    ,inf.idDependencia
                                    ,inf.idDelegacion
                                    ,inf.idVehiculo
                                    ,inf.idAplicacion
                                    ,inf.idGarantia
                                    ,inf.idEstatusInfraccion
                                    ,inf.idMunicipio
                                    ,inf.idTramo
                                    ,inf.idCarretera
                                    ,inf.idPersona
                                    ,ISNULL(inf.idPersonaConductor,0) idPersonaConductor
                                    ,inf.placasVehiculo
                                    ,inf.folioInfraccion
                                    ,inf.fechaInfraccion
                                    ,inf.kmCarretera
                                    ,inf.observaciones
                                    ,inf.lugarCalle
                                    ,inf.lugarNumero
                                    ,inf.lugarColonia
                                    ,inf.lugarEntreCalle
                                    ,inf.infraccionCortesia
                                    ,inf.NumTarjetaCirculacion
                                    ,inf.fechaActualizacion
                                    ,inf.actualizadoPor
                                    ,inf.estatus
                                    ,del.idDelegacion, del.delegacion,dep.idDependencia,dep.nombreDependencia,catGar.idGarantia,catGar.garantia
                                    ,estIn.idEstatusInfraccion, estIn.estatusInfraccion
                                    ,gar.idGarantia,gar.numPlaca,gar.numLicencia,gar.vehiculoDocumento
                                    ,tipoP.idTipoPlaca, tipoP.tipoPlaca
                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                    ,catMun.idMunicipio,catMun.municipio
                                    ,catTra.idTramo,catTra.tramo
                                    ,catCarre.idCarretera,catCarre.carretera
                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico
                                    ,motInf.idMotivoInfraccion,motInf.nombre,motInf.fundamento,motInf.calificacionMinima,motInf.calificacionMaxima
                                    ,catMotInf.idMotivoInfraccion,catMotInf.catMotivo
                                    ,catSubInf.idSubConcepto,catSubInf.subConcepto
                                    ,catConInf.idConcepto,catConInf.concepto
                                    FROM infracciones as inf
                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                    left join catDelegaciones	del on inf.idDelegacion = del.idDelegacion
                                    left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
                                    left join catGarantias catGar on inf.idGarantia = catGar.idGarantia
                                    left join garantiasInfraccion gar on catGar.idGarantia= gar.idCatGarantia
                                    left join catTipoPlaca  tipoP on gar.idTipoPlaca=tipoP.idTipoPlaca
                                    left join catTipoLicencia tipoL on tipoL.idTipoLicencia= gar.idTipoLicencia
                                    left join catOficiales catOfi on inf.idOficial = catOfi.idOficial
                                    left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                    left join catTramos catTra on inf.idTramo = catTra.idTramo
                                    left join catCarreteras catCarre on catTra.IdCarretera = catCarre.idCarretera
                                    left join vehiculos veh on inf.idVehiculo = veh.idVehiculo
                                    left join motivosInfraccion motInf on inf.IdInfraccion = motInf.idInfraccion
                                    left join catMotivosInfraccion catMotInf on motInf.idCatMotivosInfraccion = catMotInf.idMotivoInfraccion
                                    left join catSubConceptoInfraccion catSubInf on catMotInf.IdSubConcepto = catSubInf.idSubConcepto
                                    left join catConceptoInfraccion catConInf on  catSubInf.idConcepto = catConInf.idConcepto
                                    WHERE inf.estatus = 1";

            return modelList;
        }

        public List<EstadisticaInfraccionMotivosModel> GetAllInfraccionesEstadisticas(IncidenciasBusquedaModel modelBusqueda, int idDependencia)
        {
            string condiciones = "";
            //condiciones += modelBusqueda.idTipoMotivo.Equals(null) ? "" : " AND inf.transito = @transito ";

            condiciones += modelBusqueda.idDelegacion.Equals(null) || modelBusqueda.idDelegacion == 0 ? "" : " AND inf.idDelegacion = @idDelegacion ";
            condiciones += modelBusqueda.idOficial.Equals(null) || modelBusqueda.idOficial == 0 ? "" : " AND inf.idOficial =@idOficial ";
            condiciones += modelBusqueda.idCarretera.Equals(null) || modelBusqueda.idCarretera == 0 ? "" : " AND inf.idCarretera = @idCarretera ";
            condiciones += modelBusqueda.idTramo.Equals(null) || modelBusqueda.idTramo == 0 ? "" : " AND inf.idTramo = @idTramo ";
            condiciones += modelBusqueda.idTipoVehiculo.Equals(null) || modelBusqueda.idTipoVehiculo == 0 ? "" : " AND veh.idTipoVehiculo = @idTipoVehiculo ";
            condiciones += modelBusqueda.idTipoServicio.Equals(null) || modelBusqueda.idTipoServicio == 0 ? "" : " AND veh.idCatTipoServicio  = @idCatTipoServicio ";
            condiciones += modelBusqueda.idSubTipoServicio.Equals(null) || modelBusqueda.idSubTipoServicio == 0 ? "" : " AND veh.idSubtipoServicio  = @idSubTipoServicio ";
            condiciones += modelBusqueda.idTipoLicencia.Equals(null) || modelBusqueda.idTipoLicencia == 0 ? "" : " AND gar.idTipoLicencia = @idTipoLicencia ";
            condiciones += modelBusqueda.idMunicipio.Equals(null) || modelBusqueda.idMunicipio == 0 ? "" : " AND inf.idMunicipio =@idMunicipio ";
            condiciones += modelBusqueda.IdTipoCortesia.Equals(null) || modelBusqueda.IdTipoCortesia == 0 ? "" : " AND inf.infraccionCortesia=@IdTipoCortesia ";

            string condicionFecha = "";

            if (modelBusqueda.fechaInicio != DateTime.MinValue && modelBusqueda.fechaFin != DateTime.MinValue)
                condiciones += " AND inf.fechaInfraccion >= @fechaInicio AND inf.fechaInfraccion  <= @fechaFin ";
            else if (modelBusqueda.fechaInicio != DateTime.MinValue && modelBusqueda.fechaFin == DateTime.MinValue)
                condiciones += " AND inf.fechaInfraccion >= @fechaInicio ";
            else if (modelBusqueda.fechaInicio == DateTime.MinValue && modelBusqueda.fechaFin != DateTime.MinValue)
                condiciones += " AND inf.fechaInfraccion <= @fechaFin ";
            else
                condiciones += "";


            List<EstadisticaInfraccionMotivosModel> modelList = new List<EstadisticaInfraccionMotivosModel>();
            string strQuery = @"SELECT ci.nombre Motivo, COUNT(m.idMotivoInfraccion) Contador
                               FROM infracciones inf
                                INNER JOIN motivosInfraccion m
                                ON m.idInfraccion = inf.idInfraccion
                                INNER JOIN catMotivosInfraccion ci
                                on m.idCatMotivosInfraccion = ci.idCatMotivoInfraccion 
                                AND ci.estatus = 1
                                LEFT JOIN catSubConceptoInfraccion csi
                                on ci.IdSubConcepto = csi.idSubConcepto
                                AND csi.estatus = 1
                                LEFT JOIN catConceptoInfraccion cci
                                on csi.idConcepto = cci.idConcepto
                                AND cci.estatus = 1
                               LEFT JOIN catMunicipios mun
                               ON inf.idMunicipio = mun.idMunicipio
                                LEFT JOIN vehiculos veh
                               ON inf.idVehiculo = veh.idVehiculo
                                left join garantiasInfraccion gar on gar.idinfraccion = inf.idinfraccion
                                WHERE m.estatus = 1
                               AND inf.estatus = 1 AND inf.transito = @transito " + condiciones + condicionFecha +
							   "group by ci.nombre;";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                  
                        command.Parameters.Add(new SqlParameter("@transito", SqlDbType.Int)).Value = idDependencia;
                   
                    //if (!modelBusqueda.idTipoMotivo.Equals(null))
                    //    command.Parameters.Add(new SqlParameter("@transito", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoMotivo ?? DBNull.Value;

                    if (!modelBusqueda.idDelegacion.Equals(null) && modelBusqueda.idDelegacion != 0)
                        command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = (object)modelBusqueda.idDelegacion ?? DBNull.Value;

                    if (!modelBusqueda.idOficial.Equals(null) && modelBusqueda.idOficial != 0)
                        command.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = (object)modelBusqueda.idOficial ?? DBNull.Value;

                    if (!modelBusqueda.idCarretera.Equals(null) && modelBusqueda.idCarretera != 0)
                        command.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = (object)modelBusqueda.idCarretera ?? DBNull.Value;

                    if (!modelBusqueda.idTramo.Equals(null) && modelBusqueda.idTramo != 0)
                        command.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = (object)modelBusqueda.idTramo ?? DBNull.Value;

                    if (!modelBusqueda.idTipoVehiculo.Equals(null) && modelBusqueda.idTipoVehiculo != 0)
                        command.Parameters.Add(new SqlParameter("@idTipoVehiculo", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoVehiculo ?? DBNull.Value;

                    if (!modelBusqueda.idTipoServicio.Equals(null) && modelBusqueda.idTipoServicio != 0)
                        command.Parameters.Add(new SqlParameter("@idCatTipoServicio", SqlDbType.NVarChar)).Value = (object)modelBusqueda.idTipoServicio ?? DBNull.Value;

                    if (!modelBusqueda.idSubTipoServicio.Equals(null) && modelBusqueda.idSubTipoServicio != 0)
                        command.Parameters.Add(new SqlParameter("@idSubtipoServicio", SqlDbType.NVarChar)).Value = (object)modelBusqueda.idSubTipoServicio ?? DBNull.Value;

                    if (!modelBusqueda.idTipoLicencia.Equals(null) && modelBusqueda.idTipoLicencia != 0)
                        command.Parameters.Add(new SqlParameter("@idTipoLicencia", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoLicencia ?? DBNull.Value;
                    if (!modelBusqueda.IdTipoCortesia.Equals(null) && modelBusqueda.IdTipoCortesia != 0)
                        command.Parameters.Add(new SqlParameter("@IdTipoCortesia", SqlDbType.Int)).Value = (object)modelBusqueda.IdTipoCortesia ?? DBNull.Value;

                    if (!modelBusqueda.idMunicipio.Equals(null) && modelBusqueda.idMunicipio != 0)
                        command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)modelBusqueda.idMunicipio ?? DBNull.Value;

                    if (modelBusqueda.fechaInicio != DateTime.MinValue)
                        command.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.DateTime)).Value = (object)modelBusqueda.fechaInicio ?? DBNull.Value;

                    if (modelBusqueda.fechaFin != DateTime.MinValue)
                        command.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.DateTime)).Value = (object)modelBusqueda.fechaFin ?? DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;


                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            EstadisticaInfraccionMotivosModel model = new EstadisticaInfraccionMotivosModel();
                            model.Contador = reader["Contador"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["Contador"].ToString());
                            model.Motivo = reader["Motivo"].ToString();
                            modelList.Add(model);
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


    }
}
