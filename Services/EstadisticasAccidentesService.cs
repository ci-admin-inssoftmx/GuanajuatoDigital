using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using SpreadsheetLight;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using GuanajuatoAdminUsuarios.Entity;
using DocumentFormat.OpenXml.EMMA;
using Org.BouncyCastle.Asn1.Nist;

namespace GuanajuatoAdminUsuarios.Services
{
	public class EstadisticasAccidentesService : IEstadisticasAccidentesService
	{
		private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
		private readonly IVehiculosService _vehiculosService;
		private readonly IPersonasService _personasService;
        private readonly IBusquedaAccidentesService _busquedaAccidentesService;

        public EstadisticasAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD
								  , IVehiculosService vehiculosService
								  , IPersonasService personasService)
		{
			_sqlClientConnectionBD = sqlClientConnectionBD;
			_vehiculosService = vehiculosService;
			_personasService = personasService;
		}


		public IEnumerable<IncidenciasInfraccionesModel> IncidenciasInfracciones(AccidentesBusquedaModel model)
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


		public List<InfraccionesModel> GetAllInfracciones2()
		{
			List<InfraccionesModel> modelList = new List<InfraccionesModel>();
			string strQuery = @"SELECT inf.idInfraccion
                                      ,inf.idOficial
                                      ,inf.idDependencia
                                      ,inf.idDelegacion
                                      ,inf.idVehiculo
                                      ,inf.idAplicacion
                                      ,inf.idGarantia
                                      ,inf.idEstatusInfraccion
                                      ,inf.idMunicipio
                                      ,mun.municipio
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
                               FROM infracciones inf
                               LEFT JOIN catMunicipios mun
                               ON inf.idMunicipio = mun.idMunicipio
                               WHERE inf.estatus = 1"
			;

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
							InfraccionesModel model = new InfraccionesModel();
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							model.idDelegacion = reader["idDelegacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDelegacion"].ToString());
							model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							model.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							model.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"].ToString());
							model.placasVehiculo = reader["placasVehiculo"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.kmCarretera = reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"].ToString();
							model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
							model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							model.Persona = _personasService.GetPersonaByIdInfraccion((int)model.idPersona, model.idInfraccion);
							model.PersonaInfraccion = null;
							model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
							model.MotivosInfraccion = null;
							model.Garantia = null;
							model.umas = 0;
							if (model.MotivosInfraccion.Any(w => w.calificacion != null))
							{
								model.totalInfraccion = (model.MotivosInfraccion.Sum(s => (int)s.calificacion) * model.umas);
							}
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

		public List<BusquedaAccidentesModel> ObtenerAccidentes()
		{
			//
			List<BusquedaAccidentesModel> ListaAccidentes = new List<BusquedaAccidentesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"SELECT 
									acc.idAccidente, 
									acc.idMunicipio AS idMunicipio,
									mun.municipio AS municipio,
									acc.idOficinaDelegacion AS idOficinaDelegacion,
									acc.idElabora AS idElabora,
									acc.idCarretera AS idCarretera, 
									acc.idTramo AS idTramo, 
									acc.idClasificacionAccidente AS idClasificacionAccidente,
									facAcc.idFactor AS idFactorAccidente,
									acc.idFactorOpcionAccidente AS idFactorOpcionAccidente,
									acc.estatus AS estatus,
									del.delegacion AS delegacion,
									acc.numeroReporte AS numeroReporte,
									acc.fecha AS fecha,
									acc.hora AS hora,
									veh.idVehiculo AS idVehiculo,
									veh.idCatTipoServicio AS idCatTipoServicio,
									veh.idSubtipoServicio AS idSubtipoServicio,
									veh.idTipoVehiculo AS idTipoVehiculo,
									accau.idCausaAccidente AS idCausaAccidente
								FROM 
									accidentes AS acc
								LEFT JOIN 
									catMunicipios AS mun ON acc.idMunicipio = mun.idMunicipio 
								LEFT JOIN 
									catCarreteras AS car ON acc.idCarretera = car.idCarretera 
								LEFT JOIN 
									catTramos AS tra ON acc.idTramo = tra.idTramo 
								LEFT JOIN 
									(
										SELECT TOP 1 idAccidente, 
											   idVehiculo 
										FROM conductoresVehiculosAccidente 
										WHERE estatus = 1 AND idVehiculo IS NOT NULL 
										GROUP BY idAccidente, idVehiculo
									) AS cva ON acc.idAccidente = cva.idAccidente 
								LEFT JOIN 
									vehiculos AS veh ON cva.idVehiculo = veh.idVehiculo
								LEFT JOIN 
									catDelegaciones AS del ON acc.idOficinaDelegacion = del.idDelegacion
								LEFT JOIN 
									(
										SELECT TOP 1 idAccidente, 
											   idCausaAccidente 
										FROM accidenteCausas
									) AS accau ON acc.idAccidente = accau.idAccidente
									LEFT JOIN 
									(
										SELECT TOP 1 idAccidente, 
											   idFactor 
										FROM AccidenteFactoresOpciones
									) AS facAcc ON acc.idAccidente = facAcc.idAccidente
								WHERE 
									acc.estatus = 1;", connection);
					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							BusquedaAccidentesModel accidente = new BusquedaAccidentesModel();

							accidente.IdAccidente = reader["IdAccidente"] != DBNull.Value ? Convert.ToInt32(reader["IdAccidente"]) : 0;
							accidente.idMunicipio = reader["idMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["idMunicipio"]) : 0;
							accidente.municipio = reader["municipio"].ToString();
							accidente.Delegacion = reader["delegacion"].ToString();
							accidente.idDelegacion = reader["idOficinaDelegacion"] != DBNull.Value ? Convert.ToInt32(reader["idOficinaDelegacion"]) : 0;
							accidente.IdOficial = reader["idElabora"] != DBNull.Value ? Convert.ToInt32(reader["idElabora"]) : 0;
							accidente.idCarretera = reader["idCarretera"] != DBNull.Value ? Convert.ToInt32(reader["idCarretera"]) : 0;
							accidente.idTramo = reader["IdTramo"] != DBNull.Value ? Convert.ToInt32(reader["IdTramo"]) : 0;
							accidente.idClasificacionAccidente = reader["idClasificacionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idClasificacionAccidente"]) : 0;
							//accidente.idTipoLicencia = reader["idTipoLicencia"] != DBNull.Value ? Convert.ToInt32(reader["idTipoLicencia"]) : 0;
							accidente.idCausaAccidente = reader["idCausaAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idCausaAccidente"]) : 0;
							accidente.idFactorAccidente = reader["idFactorAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorAccidente"]) : 0;
							accidente.idFactorOpcionAccidente = reader["idFactorOpcionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorOpcionAccidente"]) : 0;
							accidente.IdTipoVehiculo = reader["idTipoVehiculo"] != DBNull.Value ? Convert.ToInt32(reader["idTipoVehiculo"]) : 0;
							accidente.IdTipoServicio = reader["idCatTipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idCatTipoServicio"]) : 0;
							accidente.IdSubtipoServicio = reader["idSubtipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idSubtipoServicio"]) : 0;

							accidente.numeroReporte = reader["numeroReporte"] != DBNull.Value ? reader["numeroReporte"].ToString() : string.Empty;
							accidente.fecha = reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue;
							accidente.hora = reader["hora"] != DBNull.Value ? reader.GetTimeSpan(reader.GetOrdinal("hora")) : TimeSpan.Zero;



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

		public List<CatalogModel> GetMunicipiosFilter()
		{
			var result = new List<CatalogModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"select idMunicipio,municipio from catMunicipios where idEntidad=11 ", connection);
					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							var aux = new CatalogModel();

							aux.value = reader["idMunicipio"].ToString();
							aux.text = reader["municipio"].ToString();

							result.Add(aux);

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


			return result;
		}
		public List<CatalogModel> GetCarreterasFilter()
		{
			var result = new List<CatalogModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"select idCarretera,carretera from catCarreteras c where estatus=1 ", connection);
					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							var aux = new CatalogModel();

							aux.value = reader["idCarretera"].ToString();
							aux.text = reader["carretera"].ToString();

							result.Add(aux);

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


			return result;
		}

		public List<CatalogModel> GetDelegacionesFilter()
		{
			var result = new List<CatalogModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"select idDelegacion,delegacion from catDelegaciones c where estatus=1 and transito=1 ", connection);
					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							var aux = new CatalogModel();

							aux.value = reader["idDelegacion"].ToString();
							aux.text = reader["delegacion"].ToString();

							result.Add(aux);

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


			return result;
		}


		public List<CatalogModel> GetTramosFilter()
		{
			var result = new List<CatalogModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"select idCarretera,carretera from catCarreteras c where estatus=1 ", connection);
					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							var aux = new CatalogModel();

							aux.value = reader["idCarretera"].ToString();
							aux.text = reader["carretera"].ToString();

							result.Add(aux);

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


			return result;
		}


		// public List<ListadoAccidentesPorAccidenteModel> AccidentesPorAccidente(BusquedaAccidentesModel recibido)
	/*	public IEnumerable<ListadoAccidentesPorAccidenteModel> AccidentesPorAccidente(BusquedaAccidentesModel recibido)

		{
			List<ListadoAccidentesPorAccidenteModel> modelList = new List<ListadoAccidentesPorAccidenteModel>();
			var condicionesFiltro = new List<string>();

			if (recibido.idMunicipio > 0)
				condicionesFiltro.Add($"ac.idMunicipio = {recibido.idMunicipio}");

			if (recibido.idDelegacion > 0)
				condicionesFiltro.Add($"ac.idOficinaDelegacion = {recibido.idDelegacion}");

			if (recibido.IdOficial > 0)
				condicionesFiltro.Add($"ac.idElabora = {recibido.IdOficial}");

			if (recibido.idCarretera > 0)
				condicionesFiltro.Add($"ac.idCarretera = {recibido.idCarretera}");

			if (recibido.idTramo > 0)
				condicionesFiltro.Add($"ac.idTramo = {recibido.idTramo}");

			if (recibido.idClasificacionAccidente > 0)
				condicionesFiltro.Add($"ac.idClasificacionAccidente = {recibido.idClasificacionAccidente}");

			if (recibido.idFactorAccidente > 0)
				condicionesFiltro.Add($"facAccSub.idFactor = {recibido.idFactorAccidente}");

			if (recibido.IdTipoVehiculo > 0)
				condicionesFiltro.Add($"veh.IdTipoVehiculo = {recibido.IdTipoVehiculo}");

			if (recibido.IdTipoServicio > 0)
				condicionesFiltro.Add($"veh.IdTipoServicio = {recibido.IdTipoServicio}");

			if (recibido.IdSubtipoServicio > 0)
				condicionesFiltro.Add($"veh.IdSubtipoServicio = {recibido.IdSubtipoServicio}");

			if (recibido.idCausaAccidente > 0)
				condicionesFiltro.Add($"accCau.idCausaAccidente = {recibido.idCausaAccidente}");

			if (recibido.idFactorOpcionAccidente > 0)
				condicionesFiltro.Add($"facAccSub.idFactorOpcion = {recibido.idFactorOpcionAccidente}");

			if (recibido.FechaInicio.HasValue)
			{
				condicionesFiltro.Add($"ac.fecha >= '{recibido.FechaInicio.Value.ToString("yyyy-MM-dd")}'");
			}

			if (recibido.FechaFin.HasValue)
			{
				condicionesFiltro.Add($"ac.fecha <= '{recibido.FechaFin.Value.ToString("yyyy-MM-dd")}'");
			}
			if (recibido.hora != TimeSpan.Zero)
				condicionesFiltro.Add($"ac.hora = '{recibido.hora}'");

			string condicionesSql = string.Join(" AND ", condicionesFiltro);

			string strQuery = @"SELECT DISTINCT
                                        ac.idAccidente,
                                        ac.numeroReporte AS Numreporteaccidente,
                                        CONVERT(varchar, ac.fecha, 103) AS Fecha,
                                        ac.hora AS Hora,
                                        ac.idMunicipio AS idMunicipio,
                                        ac.idOficinaDelegacion AS idOficinaDelegacion,
                                        ac.idElabora AS idOficial,
                                        ac.idCarretera AS idCarretera,
                                        ac.idTramo AS idTramo,
                                        ac.idClasificacionAccidente AS idClasificacionAccidente,
                                        del.delegacion AS Delegacion,
                                        mun.municipio,
                                        car.carretera,
                                        tram.tramo,
                                        ac.kilometro,
                                        ac.latitud,
                                        ac.longitud,
                                        veh.vehiculos AS Vehiculo,
                                        veh.idTipoVehiculo AS idTipoVehiculo,
                                        veh.idTipoServicio AS idTipoServicio,
                                        veh.idSubtipoServicio AS idSubtipoServicio,
                                        CONCAT(ofic.nombre, ' ', ofic.apellidoPaterno, ' ', ofic.apellidoMaterno) AS NombredelOficial,
                                        ac.montoCamino AS Dañosalcamino,
                                        ac.montoCarga AS Dañosacarga,
                                        ac.montoPropietarios AS Dañosapropietario,
                                        ac.montoOtros AS Otrosdaños,
                                        inv.Lesionados AS Lesionados,
                                        inv.Muertos AS Muertos,
                                        Factores.FactoresOpciones,
                                        Causas.CausasAccidente,
                                        ac.descripcionCausas AS CausasDescripcion
                                    FROM 
                                        accidentes ac 
                                    LEFT JOIN 
                                        catDelegaciones del ON del.idDelegacion = ac.idOficinaDelegacion
                                    LEFT JOIN 
                                        catMunicipios mun ON mun.idMunicipio = ac.idMunicipio
                                    LEFT JOIN 
                                        catCarreteras car ON car.idCarretera = ac.idCarretera
                                    LEFT JOIN 
                                        catTramos tram ON tram.idTramo = ac.idTramo
                                    LEFT JOIN 
                                        catOficiales ofic ON ofic.idOficial = ac.idElabora
                                    LEFT JOIN 
                                        accidenteCausas accCau ON accCau.idAccidente = ac.idAccidente 
                                    LEFT JOIN 
                                        catCausasAccidentes cauac ON cauac.idCausaAccidente = accCau.idCausaAccidente 
                                    LEFT JOIN 
                                        (
                                            SELECT 
                                                idAccidente,
                                                STUFF((
                                                    SELECT CHAR(13) + CHAR(10) + CONCAT(fac.factorAccidente, '/', facOp.factorOpcionAccidente) + ';'
                                                    FROM AccidenteFactoresOpciones facAccSub
                                                    LEFT JOIN catFactoresAccidentes fac ON fac.idFactorAccidente = facAccSub.idFactor
                                                    LEFT JOIN catFactoresOpcionesAccidentes facOp ON facOp.idFactorOpcionAccidente = facAccSub.idFactorOpcion
                                                    WHERE facAccSub.idAccidente = Accidentes.idAccidente AND facAccSub.estatus = 1
                                                    FOR XML PATH(''), TYPE
                                                ).value('.', 'VARCHAR(MAX)'), 1, 1, '') AS FactoresOpciones
                                            FROM accidentes AS Accidentes
                                            GROUP BY idAccidente
                                        ) AS Factores ON Factores.idAccidente = ac.idAccidente
                                    LEFT JOIN 
                                        (
                                            SELECT 
                                                idAccidente,
                                                STUFF((
                                                    SELECT CHAR(13) + CHAR(10) + CONCAT(cauac.causaAccidente, ':') + ','
                                                    FROM accidenteCausas accCau
                                                    LEFT JOIN catCausasAccidentes cauac ON cauac.idCausaAccidente = accCau.idCausaAccidente
                                                    WHERE accCau.idAccidente = Accidentes.idAccidente
                                                    FOR XML PATH(''), TYPE
                                                ).value('.', 'VARCHAR(MAX)'), 1, 1, '') AS CausasAccidente
                                            FROM accidentes AS Accidentes
                                            GROUP BY idAccidente
                                        ) AS Causas ON Causas.idAccidente = ac.idAccidente
                                    LEFT JOIN 
                                        (
                                            SELECT 
                                                idAccidente,
                                                STRING_AGG(Vehiculos, ';') AS vehiculos,
                                                MAX(idTipoVehiculo) AS idTipoVehiculo, 
                                                MAX(idCatTipoServicio) AS idTipoServicio,
                                                MAX(idSubtipoServicio) AS idSubtipoServicio

                                            FROM 
                                                (
                                                    SELECT 
                                                        ac.idAccidente,
                                                        veh.idTipoVehiculo,  
                                                        veh.idCatTipoServicio,
                                                        veh.idSubtipoServicio,
                                                        CONCAT(
                                                            '  Vehículo ', 
                                                            ROW_NUMBER() OVER (PARTITION BY ac.idAccidente ORDER BY ac.idAccidente),
                                                            ':  ', 
                                                            mv.marcaVehiculo, ' ', 
                                                            sm.nombreSubmarca, ' ', 
                                                            veh.modelo, ', TIPO: ', 
                                                            tv.tipoVehiculo, ', Servicio: ', 
                                                            ts.tipoServicio, ', Placa: ', 
                                                            veh.placas, ', Serie: ', 
                                                            veh.serie, ' , Propietario: ',
                                                            pro.nombre,' ',pro.apellidoPaterno,' ',pro.apellidoMaterno,' , Conductor: ',
                                                            con.nombre,' ',con.apellidoPaterno,' ',con.apellidoMaterno
                                                        ) AS Vehiculos
                                                    FROM 
                                                        accidentes ac
                                                        LEFT JOIN vehiculosAccidente vehacc ON vehacc.idAccidente = ac.idAccidente AND vehacc.estatus = 1 
                                                        LEFT JOIN vehiculos veh ON veh.idVehiculo = vehacc.idVehiculo
                                                        LEFT JOIN catMarcasVehiculos mv ON mv.idMarcaVehiculo = veh.idMarcaVehiculo
                                                        LEFT JOIN catSubmarcasVehiculos sm ON sm.idSubmarca = veh.idSubmarca
                                                        LEFT JOIN catEntidades e ON e.idEntidad = veh.idEntidad
                                                        LEFT JOIN catColores cc ON cc.idColor = veh.idColor
                                                        LEFT JOIN catTiposVehiculo tv ON tv.idTipoVehiculo = veh.idTipoVehiculo
                                                        LEFT JOIN catTipoServicio ts ON ts.idCatTipoServicio = veh.idCatTipoServicio
                                                        left join Personas con on con.idpersona=vehacc.idpersona
                                                        left join Personas pro on pro.idpersona=veh.idpersona
                                                ) AS veh
                                            GROUP BY 
                                                idAccidente
                                        ) veh ON veh.idAccidente = ac.idAccidente
                                    LEFT JOIN 
                                        (
                                            SELECT 
                                                acc.idAccidente,
                                                COUNT(CASE WHEN invacc.idEstadoVictima = 1 THEN invacc.idEstadoVictima END) AS Lesionados,
                                                COUNT(CASE WHEN invacc.idEstadoVictima = 2 THEN invacc.idEstadoVictima END) AS Muertos
                                            FROM 
                                                accidentes acc
                                            INNER JOIN 
                                                involucradosAccidente invacc ON invacc.idAccidente = acc.idAccidente
                                            GROUP BY 
                                                acc.idAccidente
                                        ) inv ON inv.idAccidente = ac.idAccidente
                                    WHERE 
                                        ac.estatus <> 0 ";

			if (!string.IsNullOrWhiteSpace(condicionesSql))
			{
				strQuery += " AND " + condicionesSql;
			}
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					int numeroSecuencial = 1; // Inicializa el número secuencial en 1
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							ListadoAccidentesPorAccidenteModel model = new ListadoAccidentesPorAccidenteModel();
							model.Numreporteaccidente = reader["Numreporteaccidente"] == System.DBNull.Value ? default(string) : reader["Numreporteaccidente"].ToString();
							model.idMunicipio = reader["idMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["idMunicipio"]) : 0;
							model.idDelegacion = reader["idOficinaDelegacion"] != DBNull.Value ? Convert.ToInt32(reader["idOficinaDelegacion"]) : 0;
							model.IdOficial = reader["idOficial"] != DBNull.Value ? Convert.ToInt32(reader["idOficial"]) : 0;
							model.idCarretera = reader["idCarretera"] != DBNull.Value ? Convert.ToInt32(reader["idCarretera"]) : 0;
							model.idTramo = reader["idTramo"] != DBNull.Value ? Convert.ToInt32(reader["idTramo"]) : 0;
							model.idClasificacionAccidente = reader["idClasificacionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idClasificacionAccidente"]) : 0;
							//model.idCausaAccidente = reader["idCausaAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idCausaAccidente"]) : 0;
							//model.idFactorAccidente = reader["idFactorAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorAccidente"]) : 0;
							//model.idFactorOpcionAccidente = reader["idFactorOpcionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorOpcionAccidente"]) : 0;
							model.IdTipoVehiculo = reader["idTipoVehiculo"] != DBNull.Value ? Convert.ToInt32(reader["idTipoVehiculo"]) : 0;
							model.IdTipoServicio = reader["idTipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idTipoServicio"]) : 0;
							model.IdSubtipoServicio = reader["idSubtipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idSubtipoServicio"]) : 0;

							model.Fecha = reader["Fecha"] == System.DBNull.Value ? default(string) : reader["Fecha"].ToString();
							model.Hora = reader["Hora"] == System.DBNull.Value ? default(string) : reader["Hora"].ToString();
							model.Delegacion = reader["Delegacion"] == System.DBNull.Value ? default(string) : reader["Delegacion"].ToString();
							model.carretera = reader["carretera"] == System.DBNull.Value ? default(string) : reader["carretera"].ToString();
							model.municipio = reader["municipio"] == System.DBNull.Value ? default(string) : reader["municipio"].ToString();
							model.tramo = reader["tramo"] == System.DBNull.Value ? default(string) : reader["tramo"].ToString();
							model.kilometro = reader["kilometro"] == System.DBNull.Value ? default(string) : Decimal.Parse((string)reader["kilometro"]).ToString("G29");
							model.latitud = reader["latitud"] == System.DBNull.Value ? default(string) : reader["latitud"].ToString();
							model.longitud = reader["longitud"] == System.DBNull.Value ? default(string) : reader["longitud"].ToString();
							model.Vehiculo = reader["Vehiculo"] == System.DBNull.Value ? default(string) : reader["Vehiculo"].ToString();
							model.NombredelOficial = reader["NombredelOficial"] == System.DBNull.Value ? default(string) : reader["NombredelOficial"].ToString();
							model.Dañosalcamino = reader["Dañosalcamino"] == System.DBNull.Value ? default(string) : reader["Dañosalcamino"].ToString();
							model.Dañosacarga = reader["Dañosacarga"] == System.DBNull.Value ? default(string) : reader["Dañosacarga"].ToString();
							model.Dañosapropietario = reader["Dañosapropietario"] == System.DBNull.Value ? default(string) : reader["Dañosapropietario"].ToString();
							model.Otrosdaños = reader["Otrosdaños"] == System.DBNull.Value ? default(string) : reader["Otrosdaños"].ToString();
							model.Lesionados = reader["Lesionados"] == System.DBNull.Value ? default(string) : reader["Lesionados"].ToString();
							model.Muertos = reader["Muertos"] == System.DBNull.Value ? default(string) : reader["Muertos"].ToString();
							model.FactoresOpciones = reader["FactoresOpciones"] == System.DBNull.Value ? default(string) : reader["FactoresOpciones"].ToString();
							model.Causas = reader["CausasAccidente"] == System.DBNull.Value ? default(string) : reader["CausasAccidente"].ToString();
							model.CausasDescripcion = reader["CausasDescripcion"] == System.DBNull.Value ? default(string) : reader["CausasDescripcion"].ToString();
							if (!string.IsNullOrEmpty(model.FactoresOpciones))
							{
								string cleanedString = model.FactoresOpciones.Replace("\n", "").Replace("\r", "");

								string[] factoresOpcionesArray = cleanedString.Split(new[] { ": ", ":" }, StringSplitOptions.RemoveEmptyEntries);

								for (int i = 0; i < factoresOpcionesArray.Length; i += 3)
								{
									if (i + 2 < factoresOpcionesArray.Length)
									{
										if (int.TryParse(factoresOpcionesArray[i], out int idFactorAccidente) &&
											int.TryParse(factoresOpcionesArray[i + 1], out int idFactorOpcionAccidente))
										{
											model.idFactorAccidente = idFactorAccidente;
											model.idFactorOpcionAccidente = idFactorOpcionAccidente;


										}
									}
								}
							}
							model.NumeroSecuencial = numeroSecuencial;
							modelList.Add(model);
							numeroSecuencial++;
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
		}*/

		public IEnumerable<ListadoAccidentesPorAccidenteModel> AccidentesPorAccidente(BusquedaAccidentesModel recibido,Pagination pagination)
		{
			List<ListadoAccidentesPorAccidenteModel> modelList = new List<ListadoAccidentesPorAccidenteModel>();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					int numeroContinuo = 1;
					connection.Open();
					using (SqlCommand command = new SqlCommand("usp_ObtenerDatosAccidentesporAccidente", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.AddWithValue("@PageNumber", pagination.PageIndex);
						command.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                        if (pagination.Filter.Trim() != "")
                            command.Parameters.AddWithValue("@Filter", pagination.Filter);
                        if (pagination.Sort != null && pagination.Sort != "")
                        {
                            command.Parameters.AddWithValue("@SortDirection", pagination.Sort);
                            command.Parameters.AddWithValue("@SortMember", pagination.SortCamp);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@SortDirection", "ASC");
                        }
                        command.Parameters.AddWithValue("@idMunicipio", recibido.idMunicipio);
						command.Parameters.AddWithValue("@idDelegacion", recibido.idDelegacion);
						command.Parameters.AddWithValue("@IdOficial", recibido.IdOficial);
						command.Parameters.AddWithValue("@idCarretera", recibido.idCarretera);
						command.Parameters.AddWithValue("@idTramo", recibido.idTramo);
						command.Parameters.AddWithValue("@idClasificacionAccidente", recibido.idClasificacionAccidente);
						command.Parameters.AddWithValue("@idCausaAccidente", recibido.idCausaAccidente);
						command.Parameters.AddWithValue("@idFactorAccidente", recibido.idFactorAccidente);
						command.Parameters.AddWithValue("@IdTipoVehiculo", recibido.IdTipoVehiculo);
						command.Parameters.AddWithValue("@IdTipoServicio", recibido.IdTipoServicio);
						command.Parameters.AddWithValue("@IdSubtipoServicio", recibido.IdSubtipoServicio);
						command.Parameters.AddWithValue("@idFactorOpcionAccidente", recibido.idFactorOpcionAccidente);
						command.Parameters.AddWithValue("@FechaInicio", recibido.FechaInicio);
						command.Parameters.AddWithValue("@FechaFin", recibido.FechaFin);
                        if (recibido.hora == TimeSpan.Zero)
                        {
                            command.Parameters.AddWithValue("@hora", DBNull.Value);
                        }
						else
						{
                            command.Parameters.AddWithValue("@hora", recibido.hora);
                        }
                        
                            using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
						{
							while (reader.Read())
							{
								ListadoAccidentesPorAccidenteModel model = new ListadoAccidentesPorAccidenteModel();
                                model.Numero = reader["RowNumber"] != DBNull.Value ? Convert.ToInt32(reader["RowNumber"]) : 0;
                                model.Numreporteaccidente = reader["Numreporteaccidente"] == System.DBNull.Value ? default(string) : reader["Numreporteaccidente"].ToString();
								model.idMunicipio = reader["idMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["idMunicipio"]) : 0;
								model.idDelegacion = reader["idOficinaDelegacion"] != DBNull.Value ? Convert.ToInt32(reader["idOficinaDelegacion"]) : 0;
								model.IdOficial = reader["idOficial"] != DBNull.Value ? Convert.ToInt32(reader["idOficial"]) : 0;
								model.idCarretera = reader["idCarretera"] != DBNull.Value ? Convert.ToInt32(reader["idCarretera"]) : 0;
								model.idTramo = reader["idTramo"] != DBNull.Value ? Convert.ToInt32(reader["idTramo"]) : 0;
								model.idClasificacionAccidente = reader["idClasificacionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idClasificacionAccidente"]) : 0;
								//model.idCausaAccidente = reader["idCausaAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idCausaAccidente"]) : 0;
								//model.idFactorAccidente = reader["idFactorAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorAccidente"]) : 0;
								//model.idFactorOpcionAccidente = reader["idFactorOpcionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorOpcionAccidente"]) : 0;
								model.IdTipoVehiculo = reader["idTipoVehiculo"] != DBNull.Value ? Convert.ToInt32(reader["idTipoVehiculo"]) : 0;
								model.IdTipoServicio = reader["idTipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idTipoServicio"]) : 0;
								model.IdSubtipoServicio = reader["idSubtipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idSubtipoServicio"]) : 0;

								model.Fecha = reader["Fecha"] == System.DBNull.Value ? default(string) : reader["Fecha"].ToString();
								model.Hora = reader["Hora"] == System.DBNull.Value ? default(string) : reader["Hora"].ToString();
								model.Delegacion = reader["Delegacion"] == System.DBNull.Value ? default(string) : reader["Delegacion"].ToString();
								model.carretera = reader["carretera"] == System.DBNull.Value ? default(string) : reader["carretera"].ToString();
								model.municipio = reader["municipio"] == System.DBNull.Value ? default(string) : reader["municipio"].ToString();
								model.tramo = reader["tramo"] == System.DBNull.Value ? default(string) : reader["tramo"].ToString();
								model.kilometro = reader["kilometro"] == System.DBNull.Value ? default(string) : Decimal.Parse((string)reader["kilometro"]).ToString("G29");
								model.latitud = reader["latitud"] == System.DBNull.Value ? default(string) : reader["latitud"].ToString();
								model.longitud = reader["longitud"] == System.DBNull.Value ? default(string) : reader["longitud"].ToString();
								model.Vehiculo = reader["Vehiculo"] == System.DBNull.Value ? default(string) : reader["Vehiculo"].ToString();
								model.NombredelOficial = reader["NombredelOficial"] == System.DBNull.Value ? default(string) : reader["NombredelOficial"].ToString();
								model.Dañosalcamino = reader["Dañosalcamino"] == System.DBNull.Value ? default(string) : reader["Dañosalcamino"].ToString();
								model.Dañosacarga = reader["Dañosacarga"] == System.DBNull.Value ? default(string) : reader["Dañosacarga"].ToString();
								model.Dañosapropietario = reader["Dañosapropietario"] == System.DBNull.Value ? default(string) : reader["Dañosapropietario"].ToString();
								model.Otrosdaños = reader["Otrosdaños"] == System.DBNull.Value ? default(string) : reader["Otrosdaños"].ToString();
								model.Lesionados = reader["Lesionados"] == System.DBNull.Value ? default(string) : reader["Lesionados"].ToString();
								model.Muertos = reader["Muertos"] == System.DBNull.Value ? default(string) : reader["Muertos"].ToString();
								model.FactoresOpciones = reader["FactoresOpciones"] == System.DBNull.Value ? default(string) : reader["FactoresOpciones"].ToString();
								model.Causas = reader["CausasAccidente"] == System.DBNull.Value ? default(string) : reader["CausasAccidente"].ToString();
								model.CausasDescripcion = reader["CausasDescripcion"] == System.DBNull.Value ? default(string) : reader["CausasDescripcion"].ToString();
								model.Total = reader["TotalRegistros"] != DBNull.Value ? Convert.ToInt32(reader["TotalRegistros"]) : 0;

								if (!string.IsNullOrEmpty(model.FactoresOpciones))
								{
									string cleanedString = model.FactoresOpciones.Replace("\n", "").Replace("\r", "");

									string[] factoresOpcionesArray = cleanedString.Split(new[] { ": ", ":" }, StringSplitOptions.RemoveEmptyEntries);

									for (int i = 0; i < factoresOpcionesArray.Length; i += 3)
									{
										if (i + 2 < factoresOpcionesArray.Length)
										{
											if (int.TryParse(factoresOpcionesArray[i], out int idFactorAccidente) &&
												int.TryParse(factoresOpcionesArray[i + 1], out int idFactorOpcionAccidente))
											{
												model.idFactorAccidente = idFactorAccidente;
												model.idFactorOpcionAccidente = idFactorOpcionAccidente;


											}
										}
									}
								}
								model.NumeroSecuencial = numeroContinuo;
								modelList.Add(model);
								numeroContinuo++;
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

		//public List<ListadoAccidentesPorVehiculoModel> AccidentesPorVehiculo(BusquedaAccidentesModel model)
		/* public IEnumerable<ListadoAccidentesPorVehiculoModel> AccidentesPorVehiculo(BusquedaAccidentesModel model)

		{
			 List<ListadoAccidentesPorVehiculoModel> modelList = new List<ListadoAccidentesPorVehiculoModel>();
			 var condicionesFiltro = new List<string>();

			 if (model.idMunicipio > 0)
				 condicionesFiltro.Add($"ac.idMunicipio = {model.idMunicipio}");

			 if (model.idDelegacion > 0)
				 condicionesFiltro.Add($"ac.idOficinaDelegacion = {model.idDelegacion}");

			 if (model.IdOficial > 0)
				 condicionesFiltro.Add($"ac.idElabora = {model.IdOficial}");

			 if (model.idCarretera > 0)
				 condicionesFiltro.Add($"ac.idCarretera = {model.idCarretera}");

			 if (model.idTramo > 0)
				 condicionesFiltro.Add($"ac.idTramo = {model.idTramo}");

			 if (model.idClasificacionAccidente > 0)
				 condicionesFiltro.Add($"ac.idClasificacionAccidente = {model.idClasificacionAccidente}");

			 if (model.idCausaAccidente > 0)
				 condicionesFiltro.Add($"accCau.idCausaAccidente = {model.idCausaAccidente}");

			 if (model.idFactorAccidente > 0)
				 condicionesFiltro.Add($"factAcc.idFactor = {model.idFactorAccidente}");

			 if (model.IdTipoVehiculo > 0)
				 condicionesFiltro.Add($"veh.IdTipoVehiculo = {model.IdTipoVehiculo}");

			 if (model.IdTipoServicio > 0)
				 condicionesFiltro.Add($"veh.IdTipoServicio = {model.IdTipoServicio}");

			 if (model.IdSubtipoServicio > 0)
				 condicionesFiltro.Add($"veh.IdSubtipoServicio = {model.IdSubtipoServicio}");

			 if (model.idFactorOpcionAccidente > 0)
				 condicionesFiltro.Add($"factAcc.idFactorOpcion = {model.idFactorOpcionAccidente}");

			 if (model.FechaInicio.HasValue)
			 {
				 condicionesFiltro.Add($"ac.fecha >= '{model.FechaInicio.Value.ToString("yyyy-MM-dd")}'");
			 }

			 if (model.FechaFin.HasValue)
			 {
				 condicionesFiltro.Add($"ac.fecha <= '{model.FechaFin.Value.ToString("yyyy-MM-dd")}'");
			 }


			 if (model.hora != TimeSpan.Zero)
				 condicionesFiltro.Add($"ac.hora = '{model.hora}'");

			 string condicionesSql = string.Join(" AND ", condicionesFiltro);

			 string strQuery = @"SELECT DISTINCT  
				 ac.numeroReporte		as Numreporteaccidente
				 ,CONVERT(varchar, ac.fecha, 103)				as Fecha
				 ,ac.hora				as Hora
				 ,ac.idMunicipio         as idMunicipio
				 ,ac.idOficinaDelegacion as idOficinaDelegacion
				 ,ac.idElabora           as idOficial
				 ,ac.idCarretera         as idCarretera
				 ,ac.idTramo             as idTramo
				 ,accCau.idCausaAccidente  as idCausaAccidente
				 ,ac.idClasificacionAccidente   as idClasificacionAccidente
				 ,factAcc.idFactor       as idFactorAccidente
				 ,factAcc.idFactorOpcion  as idFactorOpcionAccidente
				 ,veh.placas				as PlacasVeh
				 ,veh.serie				as SerieVeh
				 ,veh.idTipoVehiculo     as idTipoVehiculo 
				 ,veh.idSubtipoServicio  as idSubtipoServicio
				 ,veh.idCatTipoServicio  as idTipoServicio
				 ,CONCAT(prop.nombre, ' ',prop.apellidoPaterno,' ',prop.apellidoMaterno)		as PropietarioVeh
				 ,tv.tipoVehiculo		as TipoVeh
				 ,ts.tipoServicio		as ServicioVeh
				 ,mv.marcaVehiculo		as Marca 
				 ,sm.nombreSubmarca		as Submarca
				 ,veh.modelo				as Modelo
				 ,CONCAT(cond.nombre, ' ',cond.apellidoPaterno,' ',cond.apellidoMaterno) 	as ConductorVeh
				 ,pe.pension				as DepositoVehículo
				 ,del.delegacion			as Delegacion
				 ,mun.municipio			as Municipio
				 ,car.carretera			as Carretera
				 ,tram.tramo				as Tramo
				 ,ac.kilometro			as Kilómetro
				 ,ac.latitud				as Latitud
				 ,ac.longitud			as Longitud
				 ,CONCAT(ofic.nombre, ' ',ofic.apellidoPaterno,' ',ofic.apellidoMaterno) 	as NombredelOficial
				 ,ac.montoCamino			as Dañosalcamino
				 ,ac.montoCarga			as Dañosacarga
				 ,ac.montoPropietarios	as Dañosapropietario
				 ,ac.montoOtros			as Otrosdaños
				 ,inv.Lesionados			as Lesionados
				 ,inv.Muertos			as Muertos
				 ,cauac.causaAccidente	as Causas
				 ,ac.descripcionCausas	as CausasDescripcion
				 FROM accidentes ac
				 LEFT JOIN catDelegaciones del on del.idDelegacion = ac.idOficinaDelegacion
				 LEFT JOIN catMunicipios mun on mun.idMunicipio = ac.idMunicipio
				 LEFT JOIN catCarreteras car on car.idCarretera = ac.idCarretera
				 LEFT JOIN catTramos tram on tram.idTramo = ac.idTramo
				 LEFT JOIN AccidenteFactoresOpciones factAcc on factAcc.idAccidente = ac.idAccidente
				 LEFT JOIN catFactoresAccidentes fac on fac.idFactorAccidente = factAcc.idFactor
				 LEFT JOIN catFactoresOpcionesAccidentes facOp on facOp.idFactorAccidente = factAcc.idFactorOpcion
				 LEFT JOIN catOficiales ofic on ofic.idOficial = ac.idElabora
				 LEFT JOIN accidenteCausas accCau on accCau.idAccidente = ac.idAccidente
				 LEFT JOIN catCausasAccidentes cauac on cauac.idCausaAccidente = accCau.idCausaAccidente
				 left join vehiculosAccidente vehacc on vehacc.idAccidente = ac.idAccidente AND vehacc.estatus = 1
				 left join vehiculos veh on veh.idVehiculo = vehacc.idVehiculo
				 left join catMarcasVehiculos mv ON mv.idMarcaVehiculo = veh.idMarcaVehiculo
				 left join catSubmarcasVehiculos sm ON sm.idSubmarca = veh.idSubmarca
				 left join catEntidades e ON e.idEntidad = veh.idEntidad
				 left join catColores cc ON cc.idColor = veh.idColor
				 left join catTiposVehiculo tv ON tv.idTipoVehiculo = veh.idTipoVehiculo
				 left join catTipoServicio ts ON ts.idCatTipoServicio = veh.idCatTipoServicio
				 left join (SELECT acc.idAccidente ,count(CASE WHEN invacc.idEstadoVictima = 1 THEN invacc.idEstadoVictima END) AS Lesionados,  count(CASE WHEN invacc.idEstadoVictima = 2 THEN invacc.idEstadoVictima END) AS Muertos
				 FROM accidentes acc
				 inner join involucradosAccidente invacc on invacc.idAccidente = acc.idAccidente
				 GROUP by acc.idAccidente) inv on inv.idAccidente = ac.idAccidente
				 LEFT JOIN conductoresVehiculosAccidente cva ON cva.idAccidente = ac.idAccidente 
				 left join pensiones pe on pe.idPension = cva.idPension
				 LEFT JOIN opeConductoresVehiculosAccidentePersonas cond ON cond.idPersona = cva.idPersona  AND cond.idAccidente = cva.idAccidente AND cond.tipoOperacion = 2 AND cva.idVehiculo = cond.idVehiculo 
				 LEFT JOIN personas prop on prop.idPersona = veh.idpersona
				 WHERE ac.estatus <> 0";
			 if (!string.IsNullOrWhiteSpace(condicionesSql))
			 {
				 strQuery += " AND " + condicionesSql;
			 }

			 using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			 {
				 try
				 {
					 connection.Open();
					 SqlCommand command = new SqlCommand(strQuery, connection);
					 command.CommandType = CommandType.Text;
					 int numeroContinuo = 1;

					 using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					 {*/
		public IEnumerable<ListadoAccidentesPorVehiculoModel> AccidentesPorVehiculo(BusquedaAccidentesModel model, Pagination pagination)
		{
			List<ListadoAccidentesPorVehiculoModel> modelList = new List<ListadoAccidentesPorVehiculoModel>();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					int numeroContinuo = 1;
					connection.Open();
					using (SqlCommand command = new SqlCommand("usp_ObtieneAccidentesPorVehiculo", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.AddWithValue("@PageNumber", pagination.PageIndex);
						command.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                        if (pagination.Filter.Trim() != "")
                            command.Parameters.AddWithValue("@Filter", pagination.Filter);

						if (pagination.Sort != null && pagination.Sort != "")
						{
							command.Parameters.AddWithValue("@SortDirection", pagination.Sort);
							command.Parameters.AddWithValue("@SortMember", pagination.SortCamp);
						}
						else
						{
							command.Parameters.AddWithValue("@SortDirection", "ASC");
						}
						command.Parameters.AddWithValue("@idMunicipio", model.idMunicipio);
						command.Parameters.AddWithValue("@idDelegacion", model.idDelegacion);
						command.Parameters.AddWithValue("@IdOficial", model.IdOficial);
						command.Parameters.AddWithValue("@idCarretera", model.idCarretera);
						command.Parameters.AddWithValue("@idTramo", model.idTramo);
						command.Parameters.AddWithValue("@idClasificacionAccidente", model.idClasificacionAccidente);
						command.Parameters.AddWithValue("@idCausaAccidente", model.idCausaAccidente);
						command.Parameters.AddWithValue("@idFactorAccidente", model.idFactorAccidente);
						command.Parameters.AddWithValue("@IdTipoVehiculo", model.IdTipoVehiculo);
						command.Parameters.AddWithValue("@IdTipoServicio", model.IdTipoServicio);
						command.Parameters.AddWithValue("@IdSubtipoServicio", model.IdSubtipoServicio);
						command.Parameters.AddWithValue("@idFactorOpcionAccidente", model.idFactorOpcionAccidente);
						command.Parameters.AddWithValue("@FechaInicio", model.FechaInicio);
						command.Parameters.AddWithValue("@FechaFin", model.FechaFin);
                        if (model.hora == TimeSpan.Zero)
                        {
                            command.Parameters.AddWithValue("@hora", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@hora", model.hora);
                        }
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
						{

							while (reader.Read())
							{
								ListadoAccidentesPorVehiculoModel vehiculo = new ListadoAccidentesPorVehiculoModel();
                                vehiculo.Numero = reader["RowNum"] != DBNull.Value ? Convert.ToInt32(reader["RowNum"]) : 0;

                                vehiculo.idMunicipio = reader["idMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["idMunicipio"]) : 0;
								vehiculo.idDelegacion = reader["idOficinaDelegacion"] != DBNull.Value ? Convert.ToInt32(reader["idOficinaDelegacion"]) : 0;
								vehiculo.IdOficial = reader["idOficial"] != DBNull.Value ? Convert.ToInt32(reader["idOficial"]) : 0;
								vehiculo.idCarretera = reader["idCarretera"] != DBNull.Value ? Convert.ToInt32(reader["idCarretera"]) : 0;
								vehiculo.idTramo = reader["idTramo"] != DBNull.Value ? Convert.ToInt32(reader["idTramo"]) : 0;
								//vehiculo.idClasificacionAccidente = reader["idClasificacionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idClasificacionAccidente"]) : 0;
								//vehiculo.idCausaAccidente = reader["idCausaAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idCausaAccidente"]) : 0;
								//vehiculo.idFactorAccidente = reader["idFactorAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorAccidente"]) : 0;
								//vehiculo.idFactorOpcionAccidente = reader["idFactorOpcionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorOpcionAccidente"]) : 0;
								vehiculo.IdTipoVehiculo = reader["idTipoVehiculo"] != DBNull.Value ? Convert.ToInt32(reader["idTipoVehiculo"]) : 0;
								vehiculo.IdTipoServicio = reader["idTipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idTipoServicio"]) : 0;
								vehiculo.IdSubtipoServicio = reader["idSubtipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idSubtipoServicio"]) : 0;

								vehiculo.Numreporteaccidente = reader["Numreporteaccidente"] == System.DBNull.Value ? default(string) : reader["Numreporteaccidente"].ToString();
								vehiculo.NumVeh = reader["NumeroVehiculo"] == System.DBNull.Value ? default(string) : reader["NumeroVehiculo"].ToString();
								vehiculo.PlacasVeh = reader["PlacasVeh"] == System.DBNull.Value ? default(string) : reader["PlacasVeh"].ToString();
								vehiculo.SerieVeh = reader["SerieVeh"] == System.DBNull.Value ? default(string) : reader["SerieVeh"].ToString();
								vehiculo.PropietarioVeh = reader["PropietarioVeh"] == System.DBNull.Value ? default(string) : reader["PropietarioVeh"].ToString();
								vehiculo.TipoVeh = reader["TipoVeh"] == System.DBNull.Value ? default(string) : reader["TipoVeh"].ToString();
								vehiculo.ServicioVeh = reader["ServicioVeh"] == System.DBNull.Value ? default(string) : reader["ServicioVeh"].ToString();
								vehiculo.Marca = reader["Marca"] == System.DBNull.Value ? default(string) : reader["Marca"].ToString();
								vehiculo.Submarca = reader["Submarca"] == System.DBNull.Value ? default(string) : reader["Submarca"].ToString();
								vehiculo.Modelo = reader["Modelo"] == System.DBNull.Value ? default(string) : reader["Modelo"].ToString();
								vehiculo.ConductorVeh = reader["ConductorVeh"] == System.DBNull.Value ? default(string) : reader["ConductorVeh"].ToString();
								vehiculo.DepositoVehículo = reader["DepositoVehículo"] == System.DBNull.Value ? default(string) : reader["DepositoVehículo"].ToString();
								vehiculo.Delegacion = reader["Delegacion"] == System.DBNull.Value ? default(string) : reader["Delegacion"].ToString();
								vehiculo.Municipio = reader["Municipio"] == System.DBNull.Value ? default(string) : reader["Municipio"].ToString();
								vehiculo.Carretera = reader["Carretera"] == System.DBNull.Value ? default(string) : reader["Carretera"].ToString();
								vehiculo.Tramo = reader["Tramo"] == System.DBNull.Value ? default(string) : reader["Tramo"].ToString();
								var km1 = (string)reader["Kilómetro"];

                                vehiculo.Kilómetro = reader["Kilómetro"] == System.DBNull.Value ? default(string) : Decimal.Parse((string)reader["Kilómetro"]).ToString("G29");
								vehiculo.Latitud = reader["Latitud"] == System.DBNull.Value ? default(string) : reader["Latitud"].ToString();
								vehiculo.Longitud = reader["Longitud"] == System.DBNull.Value ? default(string) : reader["Longitud"].ToString();
								vehiculo.NombredelOficial = reader["NombredelOficial"] == System.DBNull.Value ? default(string) : reader["NombredelOficial"].ToString();
								vehiculo.Dañosalcamino = reader["Dañosalcamino"] == System.DBNull.Value ? default(string) : reader["Dañosalcamino"].ToString();
								vehiculo.DañosaCarga = reader["DañosaCarga"] == System.DBNull.Value ? default(string) : reader["DañosaCarga"].ToString();
								vehiculo.Dañosapropietario = reader["Dañosapropietario"] == System.DBNull.Value ? default(string) : reader["Dañosapropietario"].ToString();
								vehiculo.Otrosdaños = reader["Otrosdaños"] == System.DBNull.Value ? default(string) : reader["Otrosdaños"].ToString();
								vehiculo.Lesionados = reader["Lesionados"] == System.DBNull.Value ? default(string) : reader["Lesionados"].ToString();
								vehiculo.Muertos = reader["Muertos"] == System.DBNull.Value ? default(string) : reader["Muertos"].ToString();
								vehiculo.Causas = reader["Causas"] == System.DBNull.Value ? default(string) : reader["Causas"].ToString();
								vehiculo.fecha = reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue;
								vehiculo.hora = reader["hora"] != DBNull.Value ? TimeSpan.Parse(reader["hora"].ToString()) : TimeSpan.MinValue;
								vehiculo.CausasDescripcion = reader["CausasDescripcion"] == System.DBNull.Value ? default(string) : reader["CausasDescripcion"].ToString();
								vehiculo.Total = Convert.ToInt32(reader["TotalRegistros"].ToString());

								vehiculo.NumeroContinuo = numeroContinuo;
								modelList.Add(vehiculo);
								numeroContinuo++;

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

        public Stream AccidentesPorVehiculoExcel(BusquedaAccidentesModel model)
        {
            List<ListadoAccidentesPorVehiculoModel> modelList = new List<ListadoAccidentesPorVehiculoModel>();

            SLDocument sl = new SLDocument();
            Stream file = new MemoryStream();

            sl.SetCellValue(1 , 1, "NumReporteRccidente");
            sl.SetCellValue(1 , 2, "Fecha");
            sl.SetCellValue(1 , 3, "Hora");
			sl.SetCellValue(1, 4, "Num. Vehículo");
			sl.SetCellValue(1, 5, "Placas");
			sl.SetCellValue(1, 6, "Serie");
			sl.SetCellValue(1, 7, "Propietario");
			sl.SetCellValue(1, 8, "Tipo Servicio");
			sl.SetCellValue(1, 9, "Marca");
			sl.SetCellValue(1, 10, "Subarca");
			sl.SetCellValue(1, 11, "Modelo");
			sl.SetCellValue(1, 12, "Conductor");
			sl.SetCellValue(1, 13, "Depósito");
			sl.SetCellValue(1, 14, "Delegación");
			sl.SetCellValue(1, 15, "Municipio");
			sl.SetCellValue(1, 16, "Carretera");
			sl.SetCellValue(1, 17, "Tramo");
			sl.SetCellValue(1, 18, "Kilómetro");
			sl.SetCellValue(1, 19, "Latitud");
			sl.SetCellValue(1, 20, "Longitud");
			sl.SetCellValue(1, 21, "Oficial");
			sl.SetCellValue(1, 22, "Daños Camino");
			sl.SetCellValue(1, 23, "Daños Carga");
			sl.SetCellValue(1, 24, "Daños Propietario");
			sl.SetCellValue(1, 25, "Otros Daños");
			sl.SetCellValue(1, 26, "Lesionados");
			sl.SetCellValue(1, 27, "Muertos");
			sl.SetCellValue(1, 28, "Causas");
			sl.SetCellValue(1, 29, "Descripción Causas");





			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    int numeroContinuo = 1;
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("usp_ObtieneAccidentesPorVehiculoExcel", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                   
                        command.Parameters.AddWithValue("@idMunicipio", model.idMunicipio);
                        command.Parameters.AddWithValue("@idDelegacion", model.idDelegacion);
                        command.Parameters.AddWithValue("@IdOficial", model.IdOficial);
                        command.Parameters.AddWithValue("@idCarretera", model.idCarretera);
                        command.Parameters.AddWithValue("@idTramo", model.idTramo);
                        command.Parameters.AddWithValue("@idClasificacionAccidente", model.idClasificacionAccidente);
                        command.Parameters.AddWithValue("@idCausaAccidente", model.idCausaAccidente);
                        command.Parameters.AddWithValue("@idFactorAccidente", model.idFactorAccidente);
                        command.Parameters.AddWithValue("@IdTipoVehiculo", model.IdTipoVehiculo);
                        command.Parameters.AddWithValue("@IdTipoServicio", model.IdTipoServicio);
                        command.Parameters.AddWithValue("@IdSubtipoServicio", model.IdSubtipoServicio);
                        command.Parameters.AddWithValue("@idFactorOpcionAccidente", model.idFactorOpcionAccidente);
                        command.Parameters.AddWithValue("@FechaInicio", model.FechaInicio);
                        command.Parameters.AddWithValue("@FechaFin", model.FechaFin);
                        command.Parameters.AddWithValue("@hora", DBNull.Value);
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
						
                            while (reader.Read())
                            {
                                ListadoAccidentesPorVehiculoModel vehiculo = new ListadoAccidentesPorVehiculoModel();
                                ////vehiculo.idMunicipio = reader["idMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["idMunicipio"]) : 0;
                                ////vehiculo.idDelegacion = reader["idOficinaDelegacion"] != DBNull.Value ? Convert.ToInt32(reader["idOficinaDelegacion"]) : 0;
                                ////vehiculo.IdOficial = reader["idOficial"] != DBNull.Value ? Convert.ToInt32(reader["idOficial"]) : 0;
                                ////vehiculo.idCarretera = reader["idCarretera"] != DBNull.Value ? Convert.ToInt32(reader["idCarretera"]) : 0;
                                ////vehiculo.idTramo = reader["idTramo"] != DBNull.Value ? Convert.ToInt32(reader["idTramo"]) : 0;
                                ////vehiculo.IdTipoVehiculo = reader["idTipoVehiculo"] != DBNull.Value ? Convert.ToInt32(reader["idTipoVehiculo"]) : 0;
                                ////vehiculo.IdTipoServicio = reader["idTipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idTipoServicio"]) : 0;
                                ////vehiculo.IdSubtipoServicio = reader["idSubtipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idSubtipoServicio"]) : 0;


                                sl.SetCellValue(1 + numeroContinuo, 1, reader["Numreporteaccidente"] == System.DBNull.Value ? default(string) : reader["Numreporteaccidente"].ToString());
                                DateTime fecha = reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue;

                                sl.SetCellValue(1 + numeroContinuo, 2, fecha);

                                SLStyle dateStyle = sl.CreateStyle();
                                dateStyle.FormatCode = "dd/MM/yyyy"; // Establecer el formato de fecha

                                sl.SetCellStyle(1 + numeroContinuo, 2, dateStyle);
                                sl.AutoFitColumn(2);

                                if (reader["hora"] != DBNull.Value)
								{
									TimeSpan hora = TimeSpan.Parse(reader["hora"].ToString());
									sl.SetCellValue(1 + numeroContinuo, 3, hora.ToString());
								}
								else
								{
									sl.SetCellValue(1 + numeroContinuo, 3, TimeSpan.MinValue.ToString());
								}
								sl.SetCellValue(1 + numeroContinuo, 4, reader["NumeroVehiculo"] == System.DBNull.Value ? default(string) : reader["NumeroVehiculo"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 5, reader["PlacasVeh"] == System.DBNull.Value ? default(string) : reader["PlacasVeh"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 6, reader["SerieVeh"] == System.DBNull.Value ? default(string) : reader["SerieVeh"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 7, reader["PropietarioVeh"] == System.DBNull.Value ? default(string) : reader["PropietarioVeh"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 8, reader["ServicioVeh"] == System.DBNull.Value ? default(string) : reader["ServicioVeh"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 9, reader["Marca"] == System.DBNull.Value ? default(string) : reader["Marca"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 10, reader["Submarca"] == System.DBNull.Value ? default(string) : reader["Submarca"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 11, reader["Modelo"] == System.DBNull.Value ? default(string) : reader["Modelo"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 12, reader["ConductorVeh"] == System.DBNull.Value ? default(string) : reader["ConductorVeh"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 13, reader["DepositoVehículo"] == System.DBNull.Value ? default(string) : reader["DepositoVehículo"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 14, reader["Delegacion"] == System.DBNull.Value ? default(string) : reader["Delegacion"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 15, reader["Municipio"] == System.DBNull.Value ? default(string) : reader["Municipio"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 16, reader["Carretera"] == System.DBNull.Value ? default(string) : reader["Carretera"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 17, reader["Tramo"] == System.DBNull.Value ? default(string) : reader["Tramo"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 18, reader["Kilómetro"] == System.DBNull.Value ? default(string) : Decimal.Parse((string)reader["Kilómetro"]).ToString("G29"));
								sl.SetCellValue(1 + numeroContinuo, 19, reader["Latitud"] == System.DBNull.Value ? default(string) : reader["Latitud"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 20, reader["Longitud"] == System.DBNull.Value ? default(string) : reader["Longitud"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 21, reader["NombredelOficial"] == System.DBNull.Value ? default(string) : reader["NombredelOficial"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 22, reader["Dañosalcamino"] == System.DBNull.Value ? default(string) : reader["Dañosalcamino"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 23, reader["DañosaCarga"] == System.DBNull.Value ? default(string) : reader["DañosaCarga"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 24, reader["Dañosapropietario"] == System.DBNull.Value ? default(string) : reader["Dañosapropietario"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 25, reader["Otrosdaños"] == System.DBNull.Value ? default(string) : reader["Otrosdaños"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 26, reader["Lesionados"] == System.DBNull.Value ? default(string) : reader["Lesionados"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 27, reader["Muertos"] == System.DBNull.Value ? default(string) : reader["Muertos"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 28, reader["Causas"] == System.DBNull.Value ? default(string) : reader["Causas"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 29, reader["CausasDescripcion"] == System.DBNull.Value ? default(string) : reader["CausasDescripcion"].ToString());

								//                        vehiculo.PropietarioVeh = reader["PropietarioVeh"] == System.DBNull.Value ? default(string) : reader["PropietarioVeh"].ToString();
								//                        vehiculo.TipoVeh = reader["TipoVeh"] == System.DBNull.Value ? default(string) : reader["TipoVeh"].ToString();
								//                        vehiculo.ServicioVeh = reader["ServicioVeh"] == System.DBNull.Value ? default(string) : reader["ServicioVeh"].ToString();
								//                        vehiculo.Marca = reader["Marca"] == System.DBNull.Value ? default(string) : reader["Marca"].ToString();
								//                        vehiculo.Submarca = reader["Submarca"] == System.DBNull.Value ? default(string) : reader["Submarca"].ToString();
								//                        vehiculo.Modelo = reader["Modelo"] == System.DBNull.Value ? default(string) : reader["Modelo"].ToString();
								//                        vehiculo.ConductorVeh = reader["ConductorVeh"] == System.DBNull.Value ? default(string) : reader["ConductorVeh"].ToString();
								//                        vehiculo.DepositoVehículo = reader["DepositoVehículo"] == System.DBNull.Value ? default(string) : reader["DepositoVehículo"].ToString();
								//                        vehiculo.Delegacion = reader["Delegacion"] == System.DBNull.Value ? default(string) : reader["Delegacion"].ToString();
								//                        vehiculo.Municipio = reader["Municipio"] == System.DBNull.Value ? default(string) : reader["Municipio"].ToString();
								//                        vehiculo.Carretera = reader["Carretera"] == System.DBNull.Value ? default(string) : reader["Carretera"].ToString();
								//                        vehiculo.Tramo = reader["Tramo"] == System.DBNull.Value ? default(string) : reader["Tramo"].ToString();
								//                        vehiculo.Kilómetro = reader["Kilómetro"] == System.DBNull.Value ? default(string) : Decimal.Parse((string)reader["Kilómetro"]).ToString("G29");
								//                        vehiculo.Latitud = reader["Latitud"] == System.DBNull.Value ? default(string) : reader["Latitud"].ToString();
								//                        vehiculo.Longitud = reader["Longitud"] == System.DBNull.Value ? default(string) : reader["Longitud"].ToString();
								//                        vehiculo.NombredelOficial = reader["NombredelOficial"] == System.DBNull.Value ? default(string) : reader["NombredelOficial"].ToString();
								//                        vehiculo.Dañosalcamino = reader["Dañosalcamino"] == System.DBNull.Value ? default(string) : reader["Dañosalcamino"].ToString();
								//                        vehiculo.DañosaCarga = reader["DañosaCarga"] == System.DBNull.Value ? default(string) : reader["DañosaCarga"].ToString();
								//                        vehiculo.Dañosapropietario = reader["Dañosapropietario"] == System.DBNull.Value ? default(string) : reader["Dañosapropietario"].ToString();
								//                        vehiculo.Otrosdaños = reader["Otrosdaños"] == System.DBNull.Value ? default(string) : reader["Otrosdaños"].ToString();
								//                        vehiculo.Lesionados = reader["Lesionados"] == System.DBNull.Value ? default(string) : reader["Lesionados"].ToString();
								//                        vehiculo.Muertos = reader["Muertos"] == System.DBNull.Value ? default(string) : reader["Muertos"].ToString();
								//                        vehiculo.Causas = reader["Causas"] == System.DBNull.Value ? default(string) : reader["Causas"].ToString();
								//                        vehiculo.fecha = reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue;
								//                        vehiculo.hora = reader["hora"] != DBNull.Value ? TimeSpan.Parse(reader["hora"].ToString()) : TimeSpan.MinValue;
								//                        vehiculo.CausasDescripcion = reader["CausasDescripcion"] == System.DBNull.Value ? default(string) : reader["CausasDescripcion"].ToString();
								//                        vehiculo.Total = Convert.ToInt32(reader["TotalRegistros"].ToString());
								//                        vehiculo.NumeroContinuo = numeroContinuo;


								//modelList.Add(vehiculo);
								numeroContinuo++;

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
            sl.SaveAs(file);
            sl.Dispose();

            return file;
        }
		public Stream AccidentesPorAccidenteExcel(BusquedaAccidentesModel model)
		{
			SLDocument sl = new SLDocument();
			Stream file = new MemoryStream();

			sl.SetCellValue(1, 1, "NumReporteRccidente");
			sl.SetCellValue(1, 2, "Fecha");
			sl.SetCellValue(1, 3, "Hora");
			sl.SetCellValue(1, 4, "Delegación");
			sl.SetCellValue(1, 5, "Municipio");
			sl.SetCellValue(1, 6, "Carretera");
			sl.SetCellValue(1, 7, "Tramo");
			sl.SetCellValue(1, 8, "Kilómetro");
			sl.SetCellValue(1, 9, "Latitud");
			sl.SetCellValue(1, 10, "Longitud");
			sl.SetCellValue(1, 11, "Vehículo");
			sl.SetCellValue(1, 12, "Oficial");
			
			sl.SetCellValue(1, 13, "Daños Caminno");
			sl.SetCellValue(1, 14, "Daños Carga");
			sl.SetCellValue(1, 15, "Daños Propietario");
			sl.SetCellValue(1, 16, "Otros Daños");
			sl.SetCellValue(1, 17, "Lesionados");
			sl.SetCellValue(1, 18, "Muertos");
			sl.SetCellValue(1, 19, "Factores/Opciones");
			sl.SetCellValue(1, 20, "Causas");
			sl.SetCellValue(1, 21, "Causas Descripción");
		
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					int numeroContinuo = 1;
					connection.Open();
					using (SqlCommand command = new SqlCommand("usp_ObtenerDatosAccidentesporAccidenteExcel", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						
						command.Parameters.AddWithValue("@idMunicipio", model.idMunicipio);
						command.Parameters.AddWithValue("@idDelegacion", model.idDelegacion);
						command.Parameters.AddWithValue("@IdOficial", model.IdOficial);
						command.Parameters.AddWithValue("@idCarretera", model.idCarretera);
						command.Parameters.AddWithValue("@idTramo", model.idTramo);
						command.Parameters.AddWithValue("@idClasificacionAccidente", model.idClasificacionAccidente);
						command.Parameters.AddWithValue("@idCausaAccidente", model.idCausaAccidente);
						command.Parameters.AddWithValue("@idFactorAccidente", model.idFactorAccidente);
						command.Parameters.AddWithValue("@IdTipoVehiculo", model.IdTipoVehiculo);
						command.Parameters.AddWithValue("@IdTipoServicio", model.IdTipoServicio);
						command.Parameters.AddWithValue("@IdSubtipoServicio", model.IdSubtipoServicio);
						command.Parameters.AddWithValue("@idFactorOpcionAccidente", model.idFactorOpcionAccidente);
						command.Parameters.AddWithValue("@FechaInicio", model.FechaInicio);
						command.Parameters.AddWithValue("@FechaFin", model.FechaFin);
						command.Parameters.AddWithValue("@hora", DBNull.Value);
						using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
						{
							while (reader.Read())
							{
								ListadoAccidentesPorAccidenteModel modelList = new ListadoAccidentesPorAccidenteModel();

								sl.SetCellValue(1 + numeroContinuo, 1, reader["Numreporteaccidente"] == System.DBNull.Value ? default(string) : reader["Numreporteaccidente"].ToString());
                                DateTime fecha = reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue;

                                sl.SetCellValue(1 + numeroContinuo, 2, fecha);

                                SLStyle dateStyle = sl.CreateStyle();
                                dateStyle.FormatCode = "dd/MM/yyyy"; // Establecer el formato de fecha

                                sl.SetCellStyle(1 + numeroContinuo, 2, dateStyle); sl.SetCellValue(1 + numeroContinuo, 2, reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue);
                                sl.AutoFitColumn(2);

                                if (reader["hora"] != DBNull.Value)
								{
									TimeSpan hora = TimeSpan.Parse(reader["hora"].ToString());
									sl.SetCellValue(1 + numeroContinuo, 3, hora.ToString());
								}
								else
								{
									sl.SetCellValue(1 + numeroContinuo, 3, TimeSpan.MinValue.ToString());
								}
								//sl.SetCellValue(1 + numeroContinuo, 4, reader["NumeroVehiculo"] == System.DBNull.Value ? default(string) : reader["NumeroVehiculo"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 4, reader["Delegacion"] == System.DBNull.Value ? default(string) : reader["Delegacion"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 5, reader["Municipio"] == System.DBNull.Value ? default(string) : reader["Municipio"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 6, reader["Carretera"] == System.DBNull.Value ? default(string) : reader["Carretera"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 7, reader["Tramo"] == System.DBNull.Value ? default(string) : reader["Tramo"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 8, reader["kilometro"] == System.DBNull.Value ? default(string) : Decimal.Parse((string)reader["kilometro"]).ToString("G29"));
								sl.SetCellValue(1 + numeroContinuo, 9, reader["latitud"] == System.DBNull.Value ? default(string) : reader["latitud"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 10, reader["longitud"] == System.DBNull.Value ? default(string) : reader["longitud"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 11, reader["Vehiculo"] == System.DBNull.Value ? default(string) : reader["Vehiculo"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 12, reader["NombredelOficial"] == System.DBNull.Value ? default(string) : reader["NombredelOficial"].ToString());					
								
								
								
								sl.SetCellValue(1 + numeroContinuo, 13, reader["Dañosalcamino"] == System.DBNull.Value ? default(string) : reader["Dañosalcamino"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 14, reader["DañosaCarga"] == System.DBNull.Value ? default(string) : reader["DañosaCarga"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 15, reader["Dañosapropietario"] == System.DBNull.Value ? default(string) : reader["Dañosapropietario"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 16, reader["Otrosdaños"] == System.DBNull.Value ? default(string) : reader["Otrosdaños"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 17, reader["Lesionados"] == System.DBNull.Value ? default(string) : reader["Lesionados"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 18, reader["Muertos"] == System.DBNull.Value ? default(string) : reader["Muertos"].ToString());
								modelList.FactoresOpciones = reader["FactoresOpciones"] == System.DBNull.Value ? default(string) : reader["FactoresOpciones"].ToString();


								if (!string.IsNullOrEmpty(modelList.FactoresOpciones))
								{
									string cleanedString = modelList.FactoresOpciones.Replace("\n", "").Replace("\r", "");

									string[] factoresOpcionesArray = cleanedString.Split(new[] { ": ", ":" }, StringSplitOptions.RemoveEmptyEntries);

									for (int i = 0; i < factoresOpcionesArray.Length; i += 3)
									{
										if (i + 2 < factoresOpcionesArray.Length)
										{
											if (int.TryParse(factoresOpcionesArray[i], out int idFactorAccidente) &&
												int.TryParse(factoresOpcionesArray[i + 1], out int idFactorOpcionAccidente))
											{
												modelList.idFactorAccidente = idFactorAccidente;
												modelList.idFactorOpcionAccidente = idFactorOpcionAccidente;


											}
										}
									}
								}
								sl.SetCellValue(1 + numeroContinuo, 19, reader["FactoresOpciones"] == System.DBNull.Value ? default(string) : reader["FactoresOpciones"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 20, reader["CausasAccidente"] == System.DBNull.Value ? default(string) : reader["CausasAccidente"].ToString());
								sl.SetCellValue(1 + numeroContinuo, 21, reader["CausasDescripcion"] == System.DBNull.Value ? default(string) : reader["CausasDescripcion"].ToString());


								//model.Numreporteaccidente = reader["Numreporteaccidente"] == System.DBNull.Value ? default(string) : reader["Numreporteaccidente"].ToString();
								//model.idMunicipio = reader["idMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["idMunicipio"]) : 0;
								//model.idDelegacion = reader["idOficinaDelegacion"] != DBNull.Value ? Convert.ToInt32(reader["idOficinaDelegacion"]) : 0;
								//model.IdOficial = reader["idOficial"] != DBNull.Value ? Convert.ToInt32(reader["idOficial"]) : 0;
								//model.idCarretera = reader["idCarretera"] != DBNull.Value ? Convert.ToInt32(reader["idCarretera"]) : 0;
								//model.idTramo = reader["idTramo"] != DBNull.Value ? Convert.ToInt32(reader["idTramo"]) : 0;
								//model.idClasificacionAccidente = reader["idClasificacionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idClasificacionAccidente"]) : 0;
								//model.idCausaAccidente = reader["idCausaAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idCausaAccidente"]) : 0;
								//model.idFactorAccidente = reader["idFactorAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorAccidente"]) : 0;
								//model.idFactorOpcionAccidente = reader["idFactorOpcionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorOpcionAccidente"]) : 0;
								//model.IdTipoVehiculo = reader["idTipoVehiculo"] != DBNull.Value ? Convert.ToInt32(reader["idTipoVehiculo"]) : 0;
								//model.IdTipoServicio = reader["idTipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idTipoServicio"]) : 0;
								//model.IdSubtipoServicio = reader["idSubtipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idSubtipoServicio"]) : 0;

								//model.Fecha = reader["Fecha"] == System.DBNull.Value ? default(string) : reader["Fecha"].ToString();
								//model.Hora = reader["Hora"] == System.DBNull.Value ? default(string) : reader["Hora"].ToString();
								/*model.Delegacion = reader["Delegacion"] == System.DBNull.Value ? default(string) : reader["Delegacion"].ToString();
								model.carretera = reader["carretera"] == System.DBNull.Value ? default(string) : reader["carretera"].ToString();
								model.municipio = reader["municipio"] == System.DBNull.Value ? default(string) : reader["municipio"].ToString();
								model.tramo = reader["tramo"] == System.DBNull.Value ? default(string) : reader["tramo"].ToString();
								model.kilometro = reader["kilometro"] == System.DBNull.Value ? default(string) : Decimal.Parse((string)reader["kilometro"]).ToString("G29");
								model.latitud = reader["latitud"] == System.DBNull.Value ? default(string) : reader["latitud"].ToString();
								model.longitud = reader["longitud"] == System.DBNull.Value ? default(string) : reader["longitud"].ToString();
								model.Vehiculo = reader["Vehiculo"] == System.DBNull.Value ? default(string) : reader["Vehiculo"].ToString();
								model.NombredelOficial = reader["NombredelOficial"] == System.DBNull.Value ? default(string) : reader["NombredelOficial"].ToString();
								model.Dañosalcamino = reader["Dañosalcamino"] == System.DBNull.Value ? default(string) : reader["Dañosalcamino"].ToString();
								model.Dañosacarga = reader["Dañosacarga"] == System.DBNull.Value ? default(string) : reader["Dañosacarga"].ToString();
								model.Dañosapropietario = reader["Dañosapropietario"] == System.DBNull.Value ? default(string) : reader["Dañosapropietario"].ToString();
								model.Otrosdaños = reader["Otrosdaños"] == System.DBNull.Value ? default(string) : reader["Otrosdaños"].ToString();
								model.Lesionados = reader["Lesionados"] == System.DBNull.Value ? default(string) : reader["Lesionados"].ToString();
								model.Muertos = reader["Muertos"] == System.DBNull.Value ? default(string) : reader["Muertos"].ToString();
								model.FactoresOpciones = reader["FactoresOpciones"] == System.DBNull.Value ? default(string) : reader["FactoresOpciones"].ToString();
								model.Causas = reader["CausasAccidente"] == System.DBNull.Value ? default(string) : reader["CausasAccidente"].ToString();
								model.CausasDescripcion = reader["CausasDescripcion"] == System.DBNull.Value ? default(string) : reader["CausasDescripcion"].ToString();
								model.Total = reader["TotalRegistros"] != DBNull.Value ? Convert.ToInt32(reader["TotalRegistros"]) : 0;

								if (!string.IsNullOrEmpty(model.FactoresOpciones))
								{
									string cleanedString = model.FactoresOpciones.Replace("\n", "").Replace("\r", "");

									string[] factoresOpcionesArray = cleanedString.Split(new[] { ": ", ":" }, StringSplitOptions.RemoveEmptyEntries);

									for (int i = 0; i < factoresOpcionesArray.Length; i += 3)
									{
										if (i + 2 < factoresOpcionesArray.Length)
										{
											if (int.TryParse(factoresOpcionesArray[i], out int idFactorAccidente) &&
												int.TryParse(factoresOpcionesArray[i + 1], out int idFactorOpcionAccidente))
											{
												model.idFactorAccidente = idFactorAccidente;
												model.idFactorOpcionAccidente = idFactorOpcionAccidente;


											}
										}
									}
								}
								model.NumeroSecuencial = numeroContinuo;
								modelList.Add(model);*/
								numeroContinuo++;
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
			sl.SaveAs(file);
			sl.Dispose();

			return file;
		}


	}
}