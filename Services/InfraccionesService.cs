using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Office2010.Excel;
using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Framework.Catalogs;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Interfaces.Catalogos;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Models.Generales;
using GuanajuatoAdminUsuarios.RESTModels;
using GuanajuatoAdminUsuarios.Util;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services
{
	public class InfraccionesService : IInfraccionesService
	{
		private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
		private readonly DBContextInssoft _dbContext;
		private readonly IVehiculosService _vehiculosService;
		private readonly IPersonasService _personasService;
		private readonly IBitacoraService _bitacoraService;
		private readonly ICatTurnosService _turnosService;
		private readonly IHttpContextAccessor _http;


		public InfraccionesService(
			ISqlClientConnectionBD sqlClientConnectionBD,
			DBContextInssoft dbContext,
			IHttpContextAccessor http,
			IVehiculosService vehiculosService,
			IPersonasService personasService,
			IBitacoraService bitacoraService,
			ICatTurnosService turnosService)
		{
			_sqlClientConnectionBD = sqlClientConnectionBD;
			_dbContext = dbContext;
			_vehiculosService = vehiculosService;
			_personasService = personasService;
			_bitacoraService = bitacoraService;
			_http = http;
		}

        public int guardarInfraccionCortesia(int idInfraccion, DateTime fechaVencimiento, string Observaciones)
		{
			var result = 1;
            var strQueryInsertSegundo = @"INSERT INTO modificacionInfraccionesCortesia (idInfraccion, fechaVencimiento, observaciones, estatus, fechaActualizacion) 
                                  VALUES (@idInfraccion, @nuevaFechaVencimiento, @observaciones, 1, getdate())";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQueryInsertSegundo, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = idInfraccion;
					command.Parameters.Add(new SqlParameter("@nuevaFechaVencimiento", SqlDbType.DateTime)).Value = fechaVencimiento;
					command.Parameters.Add(new SqlParameter("@observaciones", SqlDbType.NVarChar)).Value = Observaciones ?? (object)DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
						}
					}
                }
				catch(Exception e)
				{

				}
			}
			
          return result;
		}
        public List<CatalogModel> GetCortesias(int IdInfraccion)
		{
			var result = new List<CatalogModel>();
			string strQuery = @"select  FORMAT(fechaVencimiento,'dd/MM/yyyy') fechaVencimiento , isnull(observaciones,'' ) observaciones
								from modificacionInfraccionesCortesia where idInfraccion=@IdInf";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

			{
				try
				{
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@IdInf", IdInfraccion);
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							var aux = new CatalogModel();

							aux.text = (string)reader["fechaVencimiento"];
							aux.value = (string)reader["observaciones"];

							result.Add(aux);

						}
					}


                }
                catch (Exception e)
				{

				}
			}

                    return result;
		}


        public List<InfraccionesModel> GetAllInfracciones(int idOficina, int idDependenciaPerfil)
		{
			List<InfraccionesModel> modelList = new List<InfraccionesModel>();
			string strQuery = @"SELECT DISTINCT inf.idInfraccion
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
                                    ,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,MAX(catGar.idGarantia) AS idGarantia,catGar.garantia
                                    , estIn.estatusInfraccion
                                    ,gar.numLicencia,gar.vehiculoDocumento
                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                    ,catMun.idMunicipio,catMun.municipio
                                    ,catTra.idTramo,catTra.tramo
                                    ,catCarre.idCarretera,catCarre.carretera
                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico 
                                    FROM infracciones as inf
                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                    left join catDelegacionesOficinasTransporte	del on inf.idDelegacion = del.idOficinaTransporte
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
                                    WHERE  inf.idDelegacion = @idOficina AND inf.transito = @idDependenciaPerfil AND inf.estatus= 1 GROUP BY inf.idInfraccion,inf.idOficial,inf.idDependencia
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
									,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,catGar.garantia
                                    ,estIn.estatusInfraccion
                                    ,gar.numPlaca,gar.numLicencia,gar.vehiculoDocumento
                                    ,tipoP.idTipoPlaca, tipoP.tipoPlaca
                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                    ,catMun.idMunicipio,catMun.municipio
                                    ,catTra.idTramo,catTra.tramo
                                    ,catCarre.idCarretera,catCarre.carretera
                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico ";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependenciaPerfil", SqlDbType.Int)).Value = (object)idDependenciaPerfil ?? DBNull.Value;

					//command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesModel model = new InfraccionesModel();
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							model.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
							model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							model.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							model.estatusInfraccion = reader["estatusInfraccion"].ToString();
							model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							model.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"].ToString());
							model.placasVehiculo = reader["placasVehiculo"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
							model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();

							// Se obtiene la persona relacionada a la infracción.
							model.Persona = _personasService.GetPersonaByIdInfraccion((int)model.idPersona, model.idInfraccion);
							model.PersonaInfraccion = model.idPersonaConductor == 0 ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idInfraccion);
							model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							model.strIsPropietarioConductor = model.Vehiculo == null ? "NO" : model.Vehiculo.idPersona == model.idPersona ? "SI" : "NO";
							model.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

							model.NombreConductor = model.PersonaInfraccion.nombreCompleto;
							model.NombrePropietario = model.Vehiculo == null ? "" : model.Vehiculo.Persona == null ? "" : model.Vehiculo.Persona.nombreCompleto;
							model.NombreGarantia = model.Garantia.garantia;
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

		public List<InfraccionesModel> GetAllInfracciones(InfraccionesBusquedaModel model, int idOficina, int idDependenciaPerfil)
		{
			List<InfraccionesModel> InfraccionesList = new List<InfraccionesModel>();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();

					string sqlCondiciones = "";
					sqlCondiciones += (object)model.IdGarantia == null ? "" : " inf.idGarantia=@IdGarantia AND \n";
					sqlCondiciones += (object)model.IdTipoCortesia == null ? "" : " inf.infraccionCortesia=@IdTipoCortesia AND \n";
					sqlCondiciones += (object)model.IdDelegacion == null ? "" : " del.idOficinaTransporte=@IdDelegacion AND \n";
					sqlCondiciones += (object)model.IdEstatus == null ? "" : " estIn.idEstatusInfraccion=@IdEstatus AND \n";
					sqlCondiciones += (object)model.IdDependencia == null ? "" : " dep.idDependencia=@IdDependencia AND \n";
					sqlCondiciones += (object)model.NumeroLicencia == null ? "" : " pInf.numeroLicencia =@numeroLicencia AND \n";
					sqlCondiciones += (object)model.NumeroEconomico == null ? "" : " veh.numeroEconomico =@numeroEconomico AND \n";
					sqlCondiciones += (object)model.folioInfraccion == null ? "" : " UPPER(inf.folioInfraccion) like  '%'+ @FolioInfraccion + '%' AND \n";
					sqlCondiciones += (object)model.placas == null ? "" : " UPPER(veh.placas)=@Placas AND \n";
					sqlCondiciones += (object)model.Propietario == null ? "" : "UPPER(per.nombre + ' ' + per.apellidoPaterno + ' ' + per.apellidoMaterno) COLLATE Latin1_general_CI_AI LIKE '%' + @Propietario + '%' AND \n";
					sqlCondiciones += (object)model.Conductor == null ? "" : "UPPER(pInf.nombre + ' ' + pInf.apellidoPaterno + ' ' + pInf.apellidoMaterno) COLLATE Latin1_general_CI_AI LIKE '%' + @Conductor + '%' AND \n";

					sqlCondiciones += (object)model.FechaInicio == null && (object)model.FechaFin == null ? "" : " inf.fechaInfraccion between @FechaInicio and  @FechaFin AND \n";



					string SqlTransact =
							string.Format(@"SELECT MAX( inf.idInfraccion)AS idInfraccion
                                    ,MAX(inf.idOficial) AS idOficial
                                    ,MAX(inf.idDependencia) AS idDependencia
                                    ,MAX(inf.idDelegacion) AS idDelegacion
                                    ,MAX(inf.idVehiculo)  AS idVehiculo
                                    ,MAX(inf.idAplicacion) AS idAplicacion
                                    ,MAX(inf.idGarantia) AS idGarantia
                                    ,MAX (inf.idEstatusInfraccion) AS idEstatusInfraccion
                                    ,MAX(inf.idMunicipio) AS idMunicipio
                                    ,MAX (inf.idTramo) AS idTramo
                                    ,MAX(inf.idCarretera) AS idCarretera
                                    ,MAX (inf.idPersona) AS idPersona
                                    ,MAX(inf.idPersonaConductor) AS idPersonaConductor
                                    ,MAX(veh.placas) AS placasVehiculo
                                    ,MAX(inf.folioInfraccion) AS folioInfraccion
                                    ,MAX(inf.fechaInfraccion) AS fechaInfraccion
                                    ,MAX(inf.kmCarretera) as kmCarretera
                                    ,MAX (inf.observaciones) AS observaciones
                                    ,MAX (inf.lugarCalle) AS lugarCalle
                                    ,MAX(inf.lugarNumero) AS lugarNumero
                                    ,MAX(inf.lugarColonia)AS lugarColonia
                                    ,MAX(inf.lugarEntreCalle) AS lugarEntreCalle
                                    ,inf.infraccionCortesia
                                    ,MAX(inf.NumTarjetaCirculacion) AS NumTarjetaCirculacion
                                    ,MAX(inf.fechaActualizacion) AS fechaActualizacion
                                    ,MAX(inf.actualizadoPor) AS actualizadoPor
                                    ,MAX(inf.estatus) AS estatus
                                    ,MAX(del.idOficinaTransporte) AS idOficinaTransporte 
									,MAX(del.nombreOficina) AS nombreOficina
									,MAX(dep.idDependencia) AS idDependencia
									,MAX(dep.nombreDependencia) as max_nombreDependencia,
				                    MAX(catGar.garantia) as garantia,
				                    MAX(estIn.estatusInfraccion) as estatusInfraccion,
				                    MAX(gar.numPlaca) as numPlaca,
				                    MAX(gar.numLicencia) as numLicencia,
				                    MAX(gar.vehiculoDocumento) as vehiculoDocumento,
				                    MAX(tipoP.idTipoPlaca) as idTipoPlaca,
				                    MAX(tipoP.tipoPlaca) as tipoPlaca,
				                    MAX(tipoL.idTipoLicencia) as idTipoLicencia,
				                    MAX(tipoL.tipoLicencia) as tipoLicencia,
				                    MAX(catOfi.nombre) as nombre,
				                    MAX(catOfi.apellidoPaterno) as apellidoPaterno,
				                    MAX(catOfi.apellidoMaterno) as apellidoMaterno,
				                    MAX(catOfi.rango) as rango,
				                    MAX(catMun.municipio) as municipio,
				                    MAX(catTra.tramo) as tramo,
				                    MAX(catCarre.carretera) as carretera,
				                    MAX(veh.idMarcaVehiculo) as idMarcaVehiculo,
				                    MAX(veh.idMarcaVehiculo) as idMarcaVehiculo,
				                    MAX(veh.serie) as serie,
				                    MAX(veh.tarjeta) as tarjeta,
				                    MAX(veh.vigenciaTarjeta) as vigenciaTarjeta,
				                    MAX(veh.idTipoVehiculo) as idTipoVehiculo,
				                    MAX(veh.modelo) as modelo,
				                    MAX(veh.idColor) as idColor,
				                    MAX(veh.idEntidad) as idEntidad,
				                    MAX(veh.idCatTipoServicio) as idCatTipoServicio,
				                    MAX(veh.propietario) as propietario,
				                    MAX(veh.numeroEconomico) as numeroEconomico,
				                    MAX(per.nombre) as nombre,
				                    MAX(per.apellidoPaterno) as apellidoPaterno,
				                    MAX(per.apellidoMaterno) as apellidoMaterno,
									MAX(ca.aplicacion) as aplicacion
                                    FROM infracciones as inf
                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                    left join catDelegacionesOficinasTransporte	del on inf.idDelegacion = del.idOficinaTransporte
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
                                    left join personas per on veh.propietario = per.idPersona 
                                    left join opeInfraccionesPersonas pInf on inf.idInfraccion = pInf.idInfraccion AND pInf.idPersona = inf.idPersonaConductor
									left join catAplicacionInfraccion ca on ca.idAplicacion = inf.idAplicacion
                                    where {0} inf.estatus=1 and inf.transito = @idDependenciaPerfil " +
									"GROUP BY inf.idInfraccion, inf.infraccionCortesia", sqlCondiciones);

					SqlCommand command = new SqlCommand(SqlTransact, connection);
					command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependenciaPerfil", SqlDbType.Int)).Value = (object)idDependenciaPerfil ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdGarantia", SqlDbType.Int)).Value = (object)model.IdGarantia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdTipoCortesia", SqlDbType.Int)).Value = (object)model.IdTipoCortesia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdDelegacion", SqlDbType.Int)).Value = (object)model.IdDelegacion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdEstatus", SqlDbType.Int)).Value = (object)model.IdEstatus ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdDependencia", SqlDbType.Int)).Value = (object)model.IdDependencia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@numeroLicencia", SqlDbType.NVarChar)).Value = (object)model.NumeroLicencia != null ? model.NumeroLicencia.ToUpper() : DBNull.Value;
					command.Parameters.Add(new SqlParameter("@numeroEconomico", SqlDbType.NVarChar)).Value = (object)model.NumeroEconomico != null ? model.NumeroEconomico.ToUpper() : DBNull.Value;
					command.Parameters.Add(new SqlParameter("@FolioInfraccion", SqlDbType.NVarChar)).Value = (object)model.folioInfraccion != null ? model.folioInfraccion.ToUpper() : DBNull.Value;
					command.Parameters.Add(new SqlParameter("@Placas", SqlDbType.NVarChar)).Value = (object)model.placas != null ? model.placas.ToUpper() : DBNull.Value;
					command.Parameters.Add(new SqlParameter("@Propietario", SqlDbType.NVarChar)).Value = (object)model.Propietario != null ? model.Propietario.ToUpper() : DBNull.Value;
					command.Parameters.Add(new SqlParameter("@Conductor", SqlDbType.NVarChar)).Value = (object)model.Conductor != null ? model.Conductor.ToUpper() : DBNull.Value;
					command.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.DateTime)).Value = model.FechaInicio == DateTime.MinValue ? new DateTime(1800, 01, 01) : (object)model.FechaInicio;
					command.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.DateTime)).Value = model.FechaFin == DateTime.MinValue ? DateTime.Now : (object)model.FechaFin;
					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesModel infraccionModel = new InfraccionesModel();
							infraccionModel.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							infraccionModel.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							infraccionModel.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							infraccionModel.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
							infraccionModel.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							infraccionModel.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							infraccionModel.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							infraccionModel.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							infraccionModel.estatusInfraccion = reader["estatusInfraccion"].ToString();
							infraccionModel.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							infraccionModel.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							infraccionModel.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							infraccionModel.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							infraccionModel.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"].ToString());
							infraccionModel.placasVehiculo = reader["placasVehiculo"].ToString();
							infraccionModel.folioInfraccion = reader["folioInfraccion"].ToString();
							infraccionModel.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							infraccionModel.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							infraccionModel.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							infraccionModel.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							infraccionModel.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							infraccionModel.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							infraccionModel.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							infraccionModel.infraccionCortesiaValue = reader["infraccionCortesia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["infraccionCortesia"].ToString());
							infraccionModel.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							infraccionModel.aplicacion = reader["aplicacion"].ToString();
							//infraccionModel.Persona = _personasService.GetPersonaById((int)infraccionModel.idPersona);
							infraccionModel.PersonaInfraccion = GetPersonaInfraccionById((int)infraccionModel.idInfraccion);
							infraccionModel.Vehiculo = _vehiculosService.GetVehiculoById((int)infraccionModel.idVehiculo);

							//infraccionModel.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(infraccionModel.idInfraccion);

							infraccionModel.Garantia = infraccionModel.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)infraccionModel.idInfraccion);
							infraccionModel.Garantia = infraccionModel.Garantia ?? new GarantiaInfraccionModel();
							infraccionModel.Garantia.garantia = infraccionModel.Garantia.garantia ?? "";
							infraccionModel.strIsPropietarioConductor = infraccionModel.Vehiculo == null ? "NO" : infraccionModel.Vehiculo.idPersona == infraccionModel.idPersona ? "SI" : "NO";
							infraccionModel.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

							if (infraccionModel.PersonaInfraccion != null)
							{
								infraccionModel.NombreConductor = infraccionModel.PersonaInfraccion.nombreCompleto;
							}
							else
							{
								infraccionModel.NombreConductor = null; // O cualquier otro valor predeterminado que desees
							}
							infraccionModel.NombrePropietario = infraccionModel.Vehiculo == null ? "" : infraccionModel.Vehiculo.Persona == null ? "" : infraccionModel.Vehiculo.Persona.nombreCompleto;
							// infraccionModel.NombreGarantia = infraccionModel.garantia;
							infraccionModel.NombreGarantia = reader["garantia"] == System.DBNull.Value ? string.Empty : reader["garantia"].ToString();

							InfraccionesList.Add(infraccionModel);
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
			return InfraccionesList;
		}




		public List<InfraccionesModel> GetAllInfraccionesBusquedaEspecial(InfraccionesBusquedaEspecialModel model, int idOficina, int idDependenciaPerfil)
		{
			List<InfraccionesModel> InfraccionesList = new List<InfraccionesModel>();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{

					DateTime? fechasIni = string.IsNullOrEmpty(model.FechaInicio) ? null : DateTime.ParseExact(model.FechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
					DateTime? fechasFin = string.IsNullOrEmpty(model.FechaFin) ? null : DateTime.ParseExact(model.FechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture);

					connection.Open();

					string sqlCondiciones = "";

					sqlCondiciones += (object)model.folio == null ? "" : " UPPER(inf.folioInfraccion)=@FolioInfraccion AND \n";
					sqlCondiciones += (object)model.placas == null ? "" : " UPPER(veh.placas)=@Placas AND \n";
					sqlCondiciones += (object)model.oficinas == null ? "" : " del.idOficinaTransporte=@IdDelegacion AND \n";

					sqlCondiciones += (object)model.propietario == null ? "" : "UPPER(per.nombre + ' ' + per.apellidoPaterno + ' ' + per.apellidoMaterno) COLLATE Latin1_general_CI_AI LIKE '%' + @Propietario + '%' AND \n";
					sqlCondiciones += (object)model.conductor == null ? "" : "UPPER(pInf.nombre + ' ' + pInf.apellidoPaterno + ' ' + pInf.apellidoMaterno) COLLATE Latin1_general_CI_AI LIKE '%' + @Conductor + '%' AND \n";
					sqlCondiciones += (object)model.estatus == null ? "" : " inf.idEstatusInfraccion=@IdEstatus AND \n";
					sqlCondiciones += string.IsNullOrEmpty(model.noLicencia) ? "" : " pInf.numeroLicencia =@numeroLicencia AND \n";

					sqlCondiciones += string.IsNullOrEmpty(model.noEconomico) ? "" : "veh.numeroEconomico = @numeroEconomico AND \n";


					sqlCondiciones += (object)fechasIni == null && (object)fechasFin == null ? "" : " inf.fechaInfraccion between @FechaInicio and  @FechaFin AND \n";



					string SqlTransact =
							string.Format(@"SELECT MAX( inf.idInfraccion)AS idInfraccion
                                    ,MAX(inf.idOficial) AS idOficial
                                    ,MAX(inf.idDependencia) AS idDependencia
                                    ,MAX(inf.idDelegacion) AS idDelegacion
                                    ,MAX(inf.idVehiculo)  AS idVehiculo
                                    ,MAX(inf.idAplicacion) AS idAplicacion
                                    ,MAX(inf.idGarantia) AS idGarantia
                                    ,MAX (inf.idEstatusInfraccion) AS idEstatusInfraccion
                                    ,MAX(inf.idMunicipio) AS idMunicipio
                                    ,MAX (inf.idTramo) AS idTramo
                                    ,MAX(inf.idCarretera) AS idCarretera
                                    ,MAX (inf.idPersona) AS idPersona
                                    ,MAX(inf.idPersonaConductor) AS idPersonaConductor
                                    ,MAX(veh.placas) AS placasVehiculo
                                    ,MAX(inf.folioInfraccion) AS folioInfraccion
                                    ,MAX(inf.fechaInfraccion) AS fechaInfraccion
                                    ,MAX(inf.kmCarretera) as kmCarretera
                                    ,MAX (inf.observaciones) AS observaciones
                                    ,MAX (inf.lugarCalle) AS lugarCalle
                                    ,MAX(inf.lugarNumero) AS lugarNumero
                                    ,MAX(inf.lugarColonia)AS lugarColonia
                                    ,MAX(inf.lugarEntreCalle) AS lugarEntreCalle
                                    ,inf.infraccionCortesia
                                    ,MAX(inf.NumTarjetaCirculacion) AS NumTarjetaCirculacion
                                    ,MAX(inf.fechaActualizacion) AS fechaActualizacion
                                    ,MAX(inf.actualizadoPor) AS actualizadoPor
                                    ,MAX(inf.estatus) AS estatus
                                    ,MAX(del.idOficinaTransporte) AS idOficinaTransporte 
									,MAX(del.nombreOficina) AS nombreOficina
									,MAX(dep.idDependencia) AS idDependencia
									,MAX(dep.nombreDependencia) as max_nombreDependencia,
				                    MAX(catGar.garantia) as garantia,
				                    MAX(estIn.estatusInfraccion) as estatusInfraccion,
				                    MAX(gar.numPlaca) as numPlaca,
				                    MAX(gar.numLicencia) as numLicencia,
				                    MAX(gar.vehiculoDocumento) as vehiculoDocumento,
				                    MAX(tipoP.idTipoPlaca) as idTipoPlaca,
				                    MAX(tipoP.tipoPlaca) as tipoPlaca,
				                    MAX(tipoL.idTipoLicencia) as idTipoLicencia,
				                    MAX(tipoL.tipoLicencia) as tipoLicencia,
				                    MAX(catOfi.nombre) as nombre,
				                    MAX(catOfi.apellidoPaterno) as apellidoPaterno,
				                    MAX(catOfi.apellidoMaterno) as apellidoMaterno,
				                    MAX(catOfi.rango) as rango,
				                    MAX(catMun.municipio) as municipio,
				                    MAX(catTra.tramo) as tramo,
				                    MAX(catCarre.carretera) as carretera,
				                    MAX(veh.idMarcaVehiculo) as idMarcaVehiculo,
				                    MAX(veh.idMarcaVehiculo) as idMarcaVehiculo,
				                    MAX(veh.serie) as serie,
				                    MAX(veh.tarjeta) as tarjeta,
				                    MAX(veh.vigenciaTarjeta) as vigenciaTarjeta,
				                    MAX(veh.idTipoVehiculo) as idTipoVehiculo,
				                    MAX(veh.modelo) as modelo,
				                    MAX(veh.idColor) as idColor,
				                    MAX(veh.idEntidad) as idEntidad,
				                    MAX(veh.idCatTipoServicio) as idCatTipoServicio,
				                    MAX(veh.propietario) as propietario,
				                    MAX(veh.numeroEconomico) as numeroEconomico,
				                    MAX(per.nombre) as nombre,
				                    MAX(per.apellidoPaterno) as apellidoPaterno,
				                    MAX(per.apellidoMaterno) as apellidoMaterno
                                    FROM infracciones as inf
                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                    left join catDelegacionesOficinasTransporte	del on inf.idDelegacion = del.idOficinaTransporte
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
                                    left join personas per on veh.propietario = per.idPersona 
                                    left join opeInfraccionesPersonas pInf on inf.idInfraccion = pInf.idInfraccion AND inf.idPersonaConductor = pInf.idPersona
                                    where {0} inf.estatus=1 and inf.transito = @idDependenciaPerfil and inf.idPersonaConductor is not null
									GROUP BY inf.idInfraccion, inf.infraccionCortesia", sqlCondiciones);

					SqlCommand command = new SqlCommand(SqlTransact, connection);


					if (!string.IsNullOrEmpty(model.folio))
						command.Parameters.Add(new SqlParameter("@FolioInfraccion", SqlDbType.NVarChar)).Value = (object)model.folio != null ? model.folio.ToUpper() : DBNull.Value;

					if (!string.IsNullOrEmpty(model.placas))
						command.Parameters.Add(new SqlParameter("@Placas", SqlDbType.NVarChar)).Value = (object)model.placas != null ? model.placas.ToUpper() : DBNull.Value;
					if (!string.IsNullOrEmpty(model.oficinas))
						command.Parameters.Add(new SqlParameter("@IdDelegacion", SqlDbType.Int)).Value = (object)model.oficinas ?? DBNull.Value;

					command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

					command.Parameters.Add(new SqlParameter("@idDependenciaPerfil", SqlDbType.Int)).Value = (object)idDependenciaPerfil ?? DBNull.Value;

					if (!string.IsNullOrEmpty(model.estatus))
						command.Parameters.Add(new SqlParameter("@IdEstatus", SqlDbType.Int)).Value = (object)model.estatus ?? DBNull.Value;
					if (!string.IsNullOrEmpty(model.propietario))
						command.Parameters.Add(new SqlParameter("@Propietario", SqlDbType.NVarChar)).Value = (object)model.propietario != null ? model.propietario.ToUpper() : DBNull.Value;
					if (!string.IsNullOrEmpty(model.conductor))
						command.Parameters.Add(new SqlParameter("@Conductor", SqlDbType.NVarChar)).Value = (object)model.conductor != null ? model.conductor.ToUpper() : DBNull.Value;

					if (!string.IsNullOrEmpty(model.noLicencia))
						command.Parameters.Add(new SqlParameter("@numeroLicencia", SqlDbType.NVarChar)).Value = (object)model.noLicencia != null ? model.noLicencia.ToUpper() : DBNull.Value;

					if (!string.IsNullOrEmpty(model.noEconomico))
						command.Parameters.Add(new SqlParameter("@numeroEconomico", SqlDbType.NVarChar)).Value = (object)model.noEconomico != null ? model.noEconomico.ToUpper() : DBNull.Value;

					if (!string.IsNullOrEmpty(model.FechaInicio))
						command.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.DateTime)).Value = fechasIni == DateTime.MinValue ? new DateTime(1800, 01, 01) : (object)fechasIni;
					if (!string.IsNullOrEmpty(model.FechaFin))
						command.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.DateTime)).Value = fechasFin == DateTime.MinValue ? DateTime.Now : (object)fechasFin;

					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesModel infraccionModel = new InfraccionesModel();
							infraccionModel.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							infraccionModel.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							infraccionModel.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							infraccionModel.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
							infraccionModel.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							infraccionModel.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							infraccionModel.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							infraccionModel.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							infraccionModel.estatusInfraccion = reader["estatusInfraccion"].ToString();
							infraccionModel.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							infraccionModel.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							infraccionModel.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							infraccionModel.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							infraccionModel.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"].ToString());
							infraccionModel.placasVehiculo = reader["placasVehiculo"].ToString();
							infraccionModel.folioInfraccion = reader["folioInfraccion"].ToString();
							infraccionModel.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							infraccionModel.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							infraccionModel.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							infraccionModel.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							infraccionModel.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							infraccionModel.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							infraccionModel.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							infraccionModel.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
							infraccionModel.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							infraccionModel.Persona = _personasService.GetPersonaByIdInfraccion((int)infraccionModel.idPersona, infraccionModel.idInfraccion);
							infraccionModel.PersonaInfraccion = GetPersonaInfraccionById((int)infraccionModel.idInfraccion);
							infraccionModel.Vehiculo = _vehiculosService.GetVehiculoById((int)infraccionModel.idVehiculo);
							//infraccionModel.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(infraccionModel.idInfraccion);

							infraccionModel.Garantia = infraccionModel.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)infraccionModel.idInfraccion);
							infraccionModel.strIsPropietarioConductor = infraccionModel.Vehiculo == null ? "NO" : infraccionModel.Vehiculo.idPersona == infraccionModel.idPersona ? "SI" : "NO";
							infraccionModel.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

							if (infraccionModel.PersonaInfraccion != null)
							{
								infraccionModel.NombreConductor = infraccionModel.PersonaInfraccion.nombreCompleto;
							}
							else
							{
								infraccionModel.NombreConductor = null; // O cualquier otro valor predeterminado que desees
							}
							infraccionModel.NombrePropietario = infraccionModel.Vehiculo == null ? "" : infraccionModel.Vehiculo.Persona == null ? "" : infraccionModel.Vehiculo.Persona.nombreCompleto;
							// infraccionModel.NombreGarantia = infraccionModel.garantia;
							infraccionModel.NombreGarantia = reader["garantia"] == System.DBNull.Value ? string.Empty : reader["garantia"].ToString();
							infraccionModel.Total = Convert.ToInt32(reader["Total"]);

							InfraccionesList.Add(infraccionModel);

						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
					throw ex;
				}
				finally
				{
					connection.Close();
				}
			return InfraccionesList;
		}


		public List<InfraccionesModel> GetAllInfraccionesByFolioInfraccion(string FolioInfraccion)
		{
			List<InfraccionesModel> modelList = new List<InfraccionesModel>();
			string strQuery = @"SELECT top 1 inf.idInfraccion
									,inf.monto AS totalInfraccion
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
                                    ,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,MAX(catGar.idGarantia) AS idGarantia,catGar.garantia
                                    , estIn.estatusInfraccion
                                    ,gar.numLicencia,gar.vehiculoDocumento
                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                    ,catMun.idMunicipio,catMun.municipio
                                    ,catTra.idTramo,catTra.tramo
                                    ,catCarre.idCarretera,catCarre.carretera
                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico 
                                     FROM infracciones as inf
                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                    left join catDelegacionesOficinasTransporte	del on inf.idDelegacion = del.idOficinaTransporte
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
                                      WHERE  inf.folioInfraccion = @FolioInfraccion AND inf.estatus= 1 GROUP BY inf.idInfraccion,inf.idOficial,inf.idDependencia
									,inf.monto
									,inf.idDelegacion
                                    ,inf.idVehiculo
                                    ,inf.idAplicacion
                                    ,inf.idGarantia
                                    ,inf.idEstatusInfraccion
                                    ,inf.idMunicipio
                                    ,inf.idTramo
                                    ,inf.idCarretera
                                    ,inf.idPersona
                                    ,ISNULL(inf.idPersonaConductor,0)
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
									,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,catGar.garantia
                                    ,estIn.estatusInfraccion
                                    ,gar.numPlaca,gar.numLicencia,gar.vehiculoDocumento
                                    ,tipoP.idTipoPlaca, tipoP.tipoPlaca
                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                    ,catMun.idMunicipio,catMun.municipio
                                    ,catTra.idTramo,catTra.tramo
                                    ,catCarre.idCarretera,catCarre.carretera
                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico ";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@FolioInfraccion", SqlDbType.VarChar)).Value = (object)FolioInfraccion ?? DBNull.Value;

					//command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesModel model = new InfraccionesModel();
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							model.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
							model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							model.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							model.estatusInfraccion = reader["estatusInfraccion"].ToString();
							model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							model.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"].ToString());
							model.placasVehiculo = reader["placasVehiculo"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToInt32(reader["infraccionCortesia"].ToString()) > 0;
							model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							model.Persona = _personasService.GetPersonaByIdInfraccion((int)model.idPersona, model.idInfraccion);
							model.PersonaInfraccion = model.idPersonaConductor == null ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idInfraccion);
							model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							model.strIsPropietarioConductor = model.Vehiculo == null ? "NO" : model.Vehiculo.idPersona == model.idPersona ? "SI" : "NO";
							model.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();
							model.totalInfraccion = reader["totalInfraccion"] == System.DBNull.Value ? default(decimal) : Convert.ToDecimal(reader["totalInfraccion"].ToString());

							model.NombreConductor = model.PersonaInfraccion.nombreCompleto == null ? "" : model.PersonaInfraccion.nombreCompleto;
							model.NombrePropietario = model.Vehiculo == null ? "" : model.Vehiculo.Persona == null ? "" : model.Vehiculo.Persona.nombreCompleto;
							if (model.Garantia != null)
								model.NombreGarantia = model.Garantia.garantia == null ? "" : model.Garantia.garantia;
							else
								model.NombreGarantia = "";

							modelList.Add(model);
						}
					}
				}
				catch (Exception ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
					throw ex;
				}
				finally
				{
					connection.Close();
				}
			}

			return modelList;
		}

		public List<InfraccionesModel> GetAllInfraccionesByReciboPago(string ReciboPago)
		{
			List<InfraccionesModel> modelList = new List<InfraccionesModel>();
			string strQuery = @"SELECT top 1 inf.idInfraccion
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
                                    ,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,MAX(catGar.idGarantia) AS idGarantia,catGar.garantia
                                    , estIn.estatusInfraccion
                                    ,gar.numLicencia,gar.vehiculoDocumento
                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                    ,catMun.idMunicipio,catMun.municipio
                                    ,catTra.idTramo,catTra.tramo
                                    ,catCarre.idCarretera,catCarre.carretera
                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico 
                                    FROM infracciones as inf
                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                    left join catDelegacionesOficinasTransporte	del on inf.idDelegacion = del.idOficinaTransporte
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
                                    WHERE  inf.reciboPago = @ReciboPago AND inf.estatus= 1 GROUP BY inf.idInfraccion,inf.idOficial,inf.idDependencia
									,inf.idDelegacion
                                    ,inf.idVehiculo
                                    ,inf.idAplicacion
                                    ,inf.idGarantia
                                    ,inf.idEstatusInfraccion
                                    ,inf.idMunicipio
                                    ,inf.idTramo
                                    ,inf.idCarretera
                                    ,inf.idPersona
                                    ,inf.idPersonaConductor
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
									,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,catGar.garantia
                                    ,estIn.estatusInfraccion
                                    ,gar.numPlaca,gar.numLicencia,gar.vehiculoDocumento
                                    ,tipoP.idTipoPlaca, tipoP.tipoPlaca
                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                    ,catMun.idMunicipio,catMun.municipio
                                    ,catTra.idTramo,catTra.tramo
                                    ,catCarre.idCarretera,catCarre.carretera
                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico ";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@ReciboPago", SqlDbType.VarChar)).Value = (object)ReciboPago ?? DBNull.Value;

					//command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesModel model = new InfraccionesModel();
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							model.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
							model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							model.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							model.estatusInfraccion = reader["estatusInfraccion"].ToString();
							model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							model.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"].ToString());
							model.placasVehiculo = reader["placasVehiculo"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToInt32(reader["infraccionCortesia"].ToString()) > 0;
							model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							model.Persona = _personasService.GetPersonaByIdInfraccion((int)model.idPersona, model.idInfraccion);
							model.PersonaInfraccion = model.idPersonaConductor == null ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idInfraccion);
							model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							model.strIsPropietarioConductor = model.Vehiculo == null ? "NO" : model.Vehiculo.idPersona == model.idPersona ? "SI" : "NO";
							model.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

							model.NombreConductor = model.PersonaInfraccion.nombreCompleto;
							model.NombrePropietario = model.Vehiculo == null ? "" : model.Vehiculo.Persona == null ? "" : model.Vehiculo.Persona.nombreCompleto;
							if (model.Garantia != null)
								model.NombreGarantia = model.Garantia.garantia == null ? "" : model.Garantia.garantia;
							else
								model.NombreGarantia = "";

							modelList.Add(model);
						}
					}
				}
				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					//ex
					throw ex;
				}
				finally
				{
					connection.Close();
				}
			}

			return modelList;
		}



		public InfraccionesModel GetInfraccionById(int IdInfraccion, int idDependencia)
		{
			InfraccionesModel model = new InfraccionesModel();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					const string SqlTransact =
											@"
												SELECT top 1 inf.idInfraccion
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
                                                    ,veh.placas as placasVehiculo
                                                    ,inf.folioInfraccion
                                                    ,inf.fechaInfraccion
                                                    ,inf.kmCarretera
													,inf.fechaVencimiento
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
                                                    ,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,catGar.idGarantia,catGar.garantia
                                                    ,estIn.idEstatusInfraccion, estIn.estatusInfraccion
                                                    ,gar.idGarantia,veh.placas as numPlaca,gar.numLicencia,gar.vehiculoDocumento
                                                    ,tipoP.idTipoPlaca, tipoP.tipoPlaca
                                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                                    ,catMun.idMunicipio,catMun.municipio
                                                    ,catTra.idTramo,catTra.tramo
                                                    ,perInf.RFC,perInf.fechaNacimiento
                                                    ,catCarre.idCarretera,catCarre.carretera
                                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico
                                                    ,motInf.idMotivoInfraccion,motInf.idMotivoInfraccion
                                                    ,ci.nombre
                                                    ,ci.idCatMotivoInfraccion,ci.nombre
                                                    ,catSubInf.idSubConcepto,catSubInf.subConcepto
                                                    ,catConInf.idConcepto,catConInf.concepto													
													,inf.documento
                                                    FROM infracciones as inf
                                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                                    left join catDelegacionesOficinasTransporte	del on inf.idDelegacion = del.idOficinaTransporte
                                                    left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
                                                    left join catGarantias catGar on inf.idGarantia = catGar.idGarantia
													left join garantiasInfraccion gar on inf.idInfraccion= gar.idInfraccion
                                                    left join catTipoPlaca  tipoP on gar.idTipoPlaca=tipoP.idTipoPlaca
                                                    left join catTipoLicencia tipoL on tipoL.idTipoLicencia= gar.idTipoLicencia
                                                    left join catOficiales catOfi on inf.idOficial = catOfi.idOficial
                                                    left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                                    left join motivosInfraccion motInf on inf.IdInfraccion = motInf.idInfraccion
												   INNER JOIN catMotivosInfraccion ci on motInf.idCatMotivosInfraccion = ci.idCatMotivoInfraccion 
                                                    left join catTramos catTra on inf.idTramo = catTra.idTramo
                                                    left join catCarreteras catCarre on catTra.IdCarretera = catCarre.idCarretera
                                                    left join opeInfraccionesVehiculos
													
													veh on inf.idVehiculo = veh.idVehiculo and veh.idinfraccion = inf.idinfraccion
                                                    left join personas per on inf.idPersona = per.idPersona
                                                    left join opeInfraccionesPersonas perInf on inf.idPersona = perInf.idPersona AND inf.idInfraccion= perInf.idInfraccion

                                                    left join catSubConceptoInfraccion catSubInf on ci.IdSubConcepto = catSubInf.idSubConcepto
                                                    left join catConceptoInfraccion catConInf on  catSubInf.idConcepto = catConInf.idConcepto
                                                    WHERE inf.estatus = 1 and inf.idInfraccion=@IdInfraccion and inf.transito = @idDependencia
													order by veh.fechaActualizacion desc ";
					SqlCommand command = new SqlCommand(SqlTransact, connection);
					command.Parameters.Add(new SqlParameter("@IdInfraccion", SqlDbType.Int)).Value = (object)IdInfraccion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							model.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDelegacion"].ToString());
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
							model.fechaNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.fechaVencimiento = reader["fechaVencimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaVencimiento"].ToString());
							model.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							model.municipio = reader["municipio"].ToString();
							//model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
							model.infraccionCortesiaValue = reader["infraccionCortesia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["infraccionCortesia"].ToString());
							model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							model.Persona = _personasService.GetPersonaByIdInfraccion((int)model.idPersona, model.idInfraccion);
							model.PersonaInfraccion = GetPersonaInfraccionById((int)model.idInfraccion);
							model.PersonaInfraccion2 = _personasService.GetPersonaByIdInfraccion((int)model.idPersonaConductor, model.idInfraccion);
							model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							model.strIsPropietarioConductor = model.Vehiculo.idPersona == model.idPersona ? "SI" : "NO";
							model.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();
							model.documento = reader["documento"] == System.DBNull.Value ? string.Empty : reader["documento"].ToString();
							model.umas = GetUmas(model.fechaInfraccion);


							if (model.MotivosInfraccion.Any(w => w.calificacion != null))
							{
								model.totalInfraccion = (model.MotivosInfraccion.Sum(s => (int)s.calificacion) * model.umas);
							}
							model.NombreConductor = model.PersonaInfraccion.nombreCompleto;
							model.NombrePropietario = model.Vehiculo.Persona.nombreCompleto;
							model.NombreConductor = model.PersonaInfraccion.nombreCompleto;
							model.NombrePropietario = model.Vehiculo.Persona.nombreCompleto;

							if (model.Vehiculo.Persona.fechaNacimiento.HasValue)
							{
								model.fechaNacimiento = model.Vehiculo.Persona.fechaNacimiento.Value;
							}
							else
							{
								model.fechaNacimiento = DateTime.MinValue;
							}

							if (model.Garantia != null)
								model.NombreGarantia = String.IsNullOrEmpty(model.Garantia.garantia) ? "" : model.Garantia.garantia;
							else
								model.NombreGarantia = null;

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
			return model;
		}
		public decimal getUMAValue(DateTime fechaInfraccion)
		{
			decimal value = 0;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					int anio = fechaInfraccion.Year;
					connection.Open();
					const string SqlTransact =
											@"select top 1 format(salario,'#.##') salario from catSalariosMinimos catSal
									where catSal.estatus =1 AND catSal.fecha <=@anio  order by fecha desc, idSalario asc";

					SqlCommand command = new SqlCommand(SqlTransact, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@anio", SqlDbType.DateTime)).Value = fechaInfraccion;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							value = Convert.ToDecimal(reader["salario"].ToString());
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
			return value;
		}

		public InfraccionesReportModel GetInfraccionReportById(int IdInfraccion, int idDependencia)
		{
			InfraccionesReportModel model = new InfraccionesReportModel();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open(); ;

					SqlCommand command = new SqlCommand("usp_ObtieneReportePorIdInfraccion", connection);
					command.Parameters.Add(new SqlParameter("@IdInfraccion", SqlDbType.Int)).Value = (object)IdInfraccion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;
					command.CommandType = CommandType.StoredProcedure;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? default(string) : reader["observaciones"].ToString();
							model.horaInfraccion = reader["horaInfraccion"] == System.DBNull.Value ? default(string) : reader["horaInfraccion"].ToString();
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.folioInfraccion = reader["folioInfraccion"] == System.DBNull.Value ? string.Empty : reader["folioInfraccion"].ToString()?.ToUpper();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.fechaVencimiento = reader["fechaVencimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaVencimiento"].ToString());
							model.estatusInfraccion = reader["estatusInfraccion"] == System.DBNull.Value ? string.Empty : reader["estatusInfraccion"].ToString();
							model.nombreOficial = reader["nombreOficial"] == System.DBNull.Value ? string.Empty : reader["nombreOficial"].ToString()?.ToUpper();
							model.municipio = reader["municipio"].ToString()?.ToUpper();
							model.carretera = reader["carretera"].ToString()?.ToUpper();
							model.tramo = reader["tramo"].ToString()?.ToUpper();
							model.kmCarretera = reader["kmCarretera"].ToString()?.ToUpper();

							model.colonia = reader["lugarColonia"].ToString()?.ToUpper();
							model.calle = reader["lugarCalle"].ToString()?.ToUpper();
							model.numero = reader["lugarNumero"].ToString()?.ToUpper();
							model.entreCalle = reader["lugarEntreCalle"].ToString()?.ToUpper();

							model.nombreConductor = reader["nombreConductor"] == System.DBNull.Value ? string.Empty : reader["nombreConductor"].ToString()?.ToUpper();
							model.domicilioConductor = reader["domicilioConductor"] == System.DBNull.Value ? string.Empty : reader["domicilioConductor"].ToString()?.ToUpper();
							model.fechaNacimientoConductor = reader["fechaNacimientoConductor"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimientoConductor"].ToString());
							model.edadConductor = reader["edadConductor"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["edadConductor"].ToString());
							model.generoConductor = reader["generoConductor"] == System.DBNull.Value ? string.Empty : reader["generoConductor"].ToString()?.ToUpper();
							model.telefonoConductor = reader["telefonoConductor"] == System.DBNull.Value ? string.Empty : reader["telefonoConductor"].ToString()?.ToUpper();
							model.numLicenciaConductor = reader["numLicenciaConductor"] == System.DBNull.Value ? string.Empty : reader["numLicenciaConductor"].ToString()?.ToUpper();
							model.tipoLicenciaConductor = reader["tipoLicenciaConductor"] == System.DBNull.Value ? string.Empty : reader["tipoLicenciaConductor"].ToString()?.ToUpper();
							model.vencimientoLicConductor = reader["vencimientoLicConductor"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vencimientoLicConductor"].ToString());
							model.placas = reader["placas"] == System.DBNull.Value ? string.Empty : reader["placas"].ToString()?.ToUpper();
							model.tipoVehiculo = reader["tipoVehiculo"] == System.DBNull.Value ? string.Empty : reader["tipoVehiculo"].ToString()?.ToUpper();
							model.marcaVehiculo = reader["marcaVehiculo"] == System.DBNull.Value ? string.Empty : reader["marcaVehiculo"].ToString()?.ToUpper();
							model.nombreSubmarca = reader["nombreSubmarca"] == System.DBNull.Value ? string.Empty : reader["nombreSubmarca"].ToString()?.ToUpper();
							model.modelo = reader["modelo"] == System.DBNull.Value ? string.Empty : reader["modelo"].ToString()?.ToUpper();
							model.color = reader["color"] == System.DBNull.Value ? string.Empty : reader["color"].ToString()?.ToUpper();
							model.nombrePropietario = reader["nombrePropietario"] == System.DBNull.Value ? string.Empty : reader["nombrePropietario"].ToString()?.ToUpper();
							model.domicilioPropietario = reader["domicilioPropietario"] == System.DBNull.Value ? string.Empty : reader["domicilioPropietario"].ToString()?.ToUpper();
							model.serie = reader["serie"] == System.DBNull.Value ? string.Empty : reader["serie"].ToString()?.ToUpper();
							model.NumTarjetaCirculacion = reader["tarjeta"] == System.DBNull.Value ? string.Empty : reader["tarjeta"].ToString()?.ToUpper();
							model.nombreEntidad = reader["nombreEntidad"] == System.DBNull.Value ? string.Empty : reader["nombreEntidad"].ToString()?.ToUpper();
							model.tipoServicio = reader["tipoServicio"] == System.DBNull.Value ? string.Empty : reader["tipoServicio"].ToString()?.ToUpper();
							model.numeroEconomico = reader["numeroEconomico"] == System.DBNull.Value ? string.Empty : reader["numeroEconomico"].ToString()?.ToUpper();
							model.tieneCortesia = reader["tieneCortesia"] == System.DBNull.Value ? default(bool) : Convert.ToBoolean(Convert.ToByte(reader["tieneCortesia"].ToString()));
							model.cortesia = reader["nombreCortesia"] == System.DBNull.Value ? string.Empty : reader["nombreCortesia"].ToString()?.ToUpper();

							model.montoCalificacion = reader["montoCalificacion"] == System.DBNull.Value ? default(decimal) : Convert.ToDecimal(reader["montoCalificacion"].ToString());
							model.montoPagado = reader["montoPagado"] == System.DBNull.Value ? default(decimal) : Convert.ToDecimal(reader["montoPagado"].ToString());
							model.reciboPago = reader["reciboPago"] == System.DBNull.Value ? string.Empty : reader["reciboPago"].ToString()?.ToUpper();
							model.oficioCondonacion = reader["oficioCondonacion"] == System.DBNull.Value ? string.Empty : reader["oficioCondonacion"].ToString()?.ToUpper();
							model.fechaPago = reader["fechaPago"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaPago"].ToString());
							model.lugarPago = reader["lugarPago"] == System.DBNull.Value ? string.Empty : reader["lugarPago"].ToString()?.ToUpper();
							model.concepto = reader["concepto"] == System.DBNull.Value ? string.Empty : reader["concepto"].ToString()?.ToUpper();
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.Garantia = model.idGarantia == 0 ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							if (model.Garantia != null)
							{
								model.Garantia.tipoLicencia = reader["tipoLicenciaConductor"] == System.DBNull.Value ? string.Empty : reader["tipoLicenciaConductor"].ToString()?.ToUpper();
							}
							model.umas = GetUmas(model.fechaInfraccion);
							model.AplicadaA = reader["aplicadaa"].GetType() == typeof(DBNull) ? "" : reader["aplicadaa"].ToString()?.ToUpper();


							if (model.MotivosInfraccion.Any(w => w.calificacion != null))
							{
								model.totalInfraccion = (model.MotivosInfraccion.Sum(s => (int)s.calificacion) * model.umas);
								model.concepto = model.MotivosInfraccion.FirstOrDefault().Concepto?.ToUpper();
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
			return model;
		}


		public List<SystemCatalogListModel> GetFilterCatalog(FilterCatalogTramoModel Filters)
		{

			var result = new List<SystemCatalogListModel>();

			string strQuery = @"SELECT 
                                idTramo,
								tramo
                                FROM catTramos
                                WHERE estatus = 1 and idCarretera=@carretera or idCarretera in (1,2) ";
			strQuery = string.Format(strQuery);

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.Parameters.Add(new SqlParameter("@carretera", SqlDbType.Int)).Value = (object)Filters.idCarretera ?? DBNull.Value;



					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							var item = new SystemCatalogListModel();
							item.Id = (int)reader["idTramo"];
							item.Text = ((string)reader["tramo"]).ToUpper();
							result.Add(item);
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

			return result;
		}

		public DateTime GetDateInfraccion(int idInfraccion)
		{
			var time = DateTime.Now;
			var str = @"select top 1 fechainfraccion from infracciones where idinfraccion=@idInf";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(str, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInf", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							time = (DateTime)reader["fechainfraccion"];
						}
					}
				}
				catch (Exception e)
				{

				}
				finally
				{

				}
			}
			return time;

		}


		public List<MotivosInfraccionVistaModel> GetMotivosInfraccionByIdInfraccion(int idInfraccion)
		{
			int numeroContinuo = 1;

			List<MotivosInfraccionVistaModel> modelList = new List<MotivosInfraccionVistaModel>();
			/*
				COR  02/07/2024
				Solicitud:Se realiza cambio para visualizar los motivos alineados a la infraccion
				Invariantemente si el catalogo esta activo o no
				Solicitante: Alfonso O.
				Cambio: se comenta del query en la variable strQuery la parte "AND ci.estatus = 1"
			 */

			string strQuery = @"SELECT
                                 m.idMotivoInfraccion
                                ,ci.nombre
                                ,ci.fundamento
                                ,m.calificacionMinima
                                ,m.calificacionMaxima
                                ,m.fechaActualizacion
                                ,m.actualizadoPor
                                ,m.estatus
                                ,m.idCatMotivosInfraccion
                                ,m.idInfraccion
                                ,m.calificacion
                                ,ci.nombre motivo
                                ,ci.IdSubConcepto
                                ,csi.subConcepto
                                ,csi.idConcepto
                                ,cci.concepto
                                FROM motivosInfraccion m
                                INNER JOIN catMotivosInfraccion ci
                                on m.idCatMotivosInfraccion = ci.idCatMotivoInfraccion 
                                --AND ci.estatus = 1
                                LEFT JOIN catSubConceptoInfraccion csi
                                on ci.IdSubConcepto = csi.idSubConcepto
                                AND csi.estatus = 1
                                LEFT JOIN catConceptoInfraccion cci
                                on csi.idConcepto = cci.idConcepto
                                AND cci.estatus = 1
                                WHERE m.estatus = 1
                                AND idInfraccion = @idInfraccion";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							MotivosInfraccionVistaModel model = new MotivosInfraccionVistaModel();
							model.idMotivoInfraccion = reader["idMotivoInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idMotivoInfraccion"].ToString());
							model.Nombre = reader["nombre"].ToString();
							model.Fundamento = reader["fundamento"].ToString();
							model.CalificacionMinima = reader["calificacionMinima"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["calificacionMinima"].ToString());
							model.CalificacionMaxima = reader["calificacionMaxima"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["calificacionMaxima"].ToString());
							model.calificacion = reader["calificacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["calificacion"].ToString());
							model.idCatMotivoInfraccion = reader["idCatMotivosInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatMotivosInfraccion"].ToString());
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.IdSubConcepto = reader["IdSubConcepto"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["IdSubConcepto"].ToString());
							model.IdConcepto = reader["idConcepto"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idConcepto"].ToString());
							model.Motivo = reader["motivo"].ToString();
							model.SubConcepto = reader["subConcepto"].ToString();
							model.Concepto = reader["concepto"].ToString();
							//model.concepto = reader["concepto"].ToString();
							model.NumeroContinuo = numeroContinuo;
							modelList.Add(model);
							numeroContinuo++;

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

		public GarantiaInfraccionModel GetGarantiaById(int idInfraccion)
		{
			List<GarantiaInfraccionModel> modelList = new List<GarantiaInfraccionModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand("usp_ObtieneInfraccionPorId", connection);
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							GarantiaInfraccionModel model = new GarantiaInfraccionModel();
							model.idGarantia = Convert.ToInt32(reader["idGarantia"].ToString());
							model.idCatGarantia = Convert.ToInt32(reader["idCatGarantia"].ToString());
							model.idTipoPlaca = Convert.ToInt32(reader["idTipoPlaca"].ToString());
							model.idTipoLicencia = Convert.ToInt32(reader["idTipoLicencia"].ToString());
							model.numPlaca = reader["numPlaca"].ToString();
							model.numLicencia = reader["numLicencia"].ToString();
							model.vehiculoDocumento = reader["vehiculoDocumento"].ToString();
							model.garantia = reader["garantia"].ToString();
							model.tipoPlaca = reader["tipoPlaca"].ToString();
							model.tipoLicencia = reader["tipoLicencia"].ToString();
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
			return modelList.FirstOrDefault();
		}

		public PersonaInfraccionModel GetPersonaInfraccionById(int idInfraccion)
		{
			List<PersonaInfraccionModel> modelList = new List<PersonaInfraccionModel>();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand("usp_ObtienePersonaInfraccionPorId", connection);
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							PersonaInfraccionModel model = new PersonaInfraccionModel();
							model.idPersonaConductor = reader["idPersonaConductor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersonaConductor"].ToString());
							model.idPersona = reader["idPersonaDireccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersonaDireccion"].ToString());
							model.idCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
							model.idTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"].ToString());
							model.numeroLicencia = reader["numeroLicencia"].ToString();
							model.CURP = reader["CURP"].ToString();
							model.RFC = reader["RFC"].ToString();
							model.nombre = reader["nombre"].ToString();
							model.apellidoPaterno = reader["apellidoPaterno"].ToString();
							model.apellidoMaterno = reader["apellidoMaterno"].ToString();
							model.tipoLicencia = reader["tipoLicencia"].ToString();
							model.tipoPersona = reader["tipoPersona"].ToString();
							model.genero = reader["genero"].ToString();
							model.fechaNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
							model.vigenciaLicencia = reader["vigenciaLicencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());
							model.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
							model.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
							model.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
							model.PersonaDireccion = _personasService.GetPersonaDireccionByIdPersona((int)model.idPersona, idInfraccion);

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


			var t = modelList.FirstOrDefault();

			if (t != null)
			{
				if (t.idPersonaConductor != null && t.idPersonaConductor != 0)
				{
					PersonaModel modelConductor = _personasService.GetPersonaByIdInfraccion(t.idPersonaConductor, idInfraccion);
					modelConductor.PersonaDireccion = new PersonaDireccionModel();

					if (modelConductor != null)
					{
						t.tipoPersona = string.IsNullOrEmpty(modelConductor?.tipoPersona) ? "" : modelConductor?.tipoPersona;
						t.genero = string.IsNullOrEmpty(modelConductor?.genero) ? "" : modelConductor?.genero;
						t.tipoLicencia = string.IsNullOrEmpty(modelConductor?.tipoLicencia) ? "" : modelConductor?.tipoLicencia;
						t.numeroLicencia = string.IsNullOrEmpty(modelConductor?.numeroLicencia) ? "" : modelConductor?.numeroLicencia;
						if (modelConductor?.fechaNacimiento != null)
						{
							t.fechaNacimiento = (DateTime)modelConductor?.fechaNacimiento;
						}

						if (modelConductor?.vigenciaLicencia != null)
						{
							t.vigenciaLicencia = (DateTime)modelConductor?.vigenciaLicencia;
						}
						if (t.idPersonaConductor != 0)
						{
							t.PersonaDireccion = _personasService.GetPersonaDireccionByIdPersona((int)t.idPersonaConductor, idInfraccion);
						}
					}
				}


			}

			var test = new PersonaInfraccionModel();

			return t ?? test;
		}

		/// <summary>
		/// Captura de persona con registro especifico para infraccion, se crea al final del proceso de creacion parte 1
		/// </summary>
		/// <param name="idPersona"></param>
		/// <returns></returns>
		public int CrearPersonaInfraccion(int idInfraccion, int idPersona)
		{
			int result = 0;
			//string strQuery = @"INSERT INTO personasInfracciones(idInfraccion,numeroLicencia, CURP, RFC, nombre,apellidoPaterno, apellidoMaterno, idCatTipoPersona, fechaActualizacion, actualizadoPor, estatus)
			//                             //SELECT @idInfraccion, numeroLicencia, CURP, RFC, nombre,apellidoPaterno, apellidoMaterno, idCatTipoPersona, @fechaActualizacion, @actualizadoPor, @estatus
			//                             //FROM personas
			//                             //WHERE idPersona = @idPersona;";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand("usp_InsertaInfraccionesPersonas", connection);
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.Add(new SqlParameter("idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion;
					command.Parameters.Add(new SqlParameter("@idPersonaConductor", SqlDbType.Int)).Value = (object)idPersona;
					//command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					//command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
					//command.Parameters.Add(new SqlParameter("estatus", SqlDbType.NVarChar)).Value = (object)1;
					result = Convert.ToInt32(command.ExecuteScalar());
				}
				catch (SqlException ex)
				{
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}

		public int CrearGarantiaInfraccion(GarantiaInfraccionModel model, int idInfraccion)
		{
			int result = 0;
			string strQuery = @"INSERT INTO garantiasInfraccion 
								(
									idInfraccion,           
									idCatGarantia,           
									idTipoPlaca,             
									idTipoLicencia,
									numPlaca,
									numLicencia,            
									vehiculoDocumento,       
									fechaActualizacion,     
									actualizadoPor,         
									estatus                 
								)
								VALUES 
								(
									@idInfraccion,
									@idCatGarantia,
									@idTipoPlaca,
									@idTipoLicencia,
									@numPlaca,
									@numLicencia,
									@vehiculoDocumento,
									getdate(),
									@actualizadoPor,
									@estatus
									 );SELECT SCOPE_IDENTITY()";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idCatGarantia", SqlDbType.Int)).Value = (object)model.idCatGarantia ?? DBNull.Value;
					int? idTipoPlaca = (model.idTipoPlaca != null) ? model.idTipoPlaca : 0;
					command.Parameters.Add(new SqlParameter("idTipoPlaca", SqlDbType.Int)).Value = idTipoPlaca;
					int? idTipoLicencia = (model.idTipoLicencia != null) ? model.idTipoLicencia : 0;
					command.Parameters.Add(new SqlParameter("idTipoLicencia", SqlDbType.Int)).Value = idTipoLicencia;
					command.Parameters.Add(new SqlParameter("numPlaca", SqlDbType.NVarChar)).Value = (object)model.numPlaca ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("numLicencia", SqlDbType.NVarChar)).Value = (object)model.numLicencia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("vehiculoDocumento", SqlDbType.NVarChar)).Value = (object)model.vehiculoDocumento ?? DBNull.Value;
					//command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("estatus", SqlDbType.Int)).Value = (object)1;

					result = Convert.ToInt32(command.ExecuteScalar());
				}
				catch (SqlException ex)
				{
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}

		public int ModificarGarantiaInfraccion(InfraccionesModel model, int idInfraccion)
		{
			int result = 0;
			string strQuery = @"UPDATE garantiasInfraccion SET  idCatGarantia = @idCatGarantia
                                                               ,idTipoPlaca = @idTipoPlaca
                                                               ,idTipoLicencia = @idTipoLicencia
                                                               ,numPlaca = @numPlaca
                                                               ,numLicencia = @numLicencia
                                                               ,vehiculoDocumento = @vehiculoDocumento
                                                               ,fechaActualizacion = getdate()
                                                               ,actualizadoPor = @actualizadoPor
                                                               WHERE idInfraccion = @idInfraccion";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("idCatGarantia", SqlDbType.Int)).Value = (object)model.Garantia.idCatGarantia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idTipoPlaca", SqlDbType.Int)).Value = (object)model.Garantia.idTipoPlaca ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("idTipoLicencia", SqlDbType.Int)).Value = (object)model.Garantia.idTipoLicencia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("numPlaca", SqlDbType.NVarChar)).Value = (object)model.Garantia.numPlaca ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("numLicencia", SqlDbType.NVarChar)).Value = (object)model.Garantia.numLicencia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("vehiculoDocumento", SqlDbType.NVarChar)).Value = (object)model.Garantia.vehiculoDocumento ?? DBNull.Value;
					//command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("idGarantia", SqlDbType.Int)).Value = model.idGarantia;
					command.Parameters.Add(new SqlParameter("idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion;

					result = command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}

		public int CrearMotivoInfraccion(MotivoInfraccionModel model)
		{
			int result = 0;
			string strQuery = @"INSERT INTO motivosInfraccion

                                      (calificacionMinima
                                      ,calificacionMaxima
                                      ,calificacion
                                      ,fechaActualizacion
                                      ,actualizadoPor
                                      ,estatus
                                      ,idCatMotivosInfraccion
                                      ,idInfraccion
                                      ,IdConcepto
                                      ,IdSubConcepto)
                               VALUES (@calificacionMinima
                                      ,@calificacionMaxima
                                      ,@calificacion
                                      ,getdate()
                                      ,@actualizadoPor
                                      ,@estatus
                                      ,@idCatMotivosInfraccion
                                      ,@idInfraccion
                                      ,@idConcepto
                                      ,@idSubConcepto);SELECT SCOPE_IDENTITY()";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("calificacionMinima", SqlDbType.Int)).Value = (object)model.calificacionMinima ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("calificacionMaxima", SqlDbType.Int)).Value = (object)model.calificacionMaxima ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("calificacion", SqlDbType.Int)).Value = (object)model.calificacion ?? DBNull.Value;
					//command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("estatus", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("idCatMotivosInfraccion", SqlDbType.Int)).Value = (object)model.idCatMotivoInfraccion;
					command.Parameters.Add(new SqlParameter("idInfraccion", SqlDbType.Int)).Value = (object)model.idInfraccion;
					command.Parameters.Add(new SqlParameter("idConcepto", SqlDbType.Int)).Value = (object)model.idConcepto;
					command.Parameters.Add(new SqlParameter("idSubConcepto", SqlDbType.Int)).Value = (object)model.IdSubConcepto;
					result = Convert.ToInt32(command.ExecuteScalar());
				}
				catch (SqlException ex)
				{
				}
				finally
				{
					connection.Close();
				}
			}

			//_bitacoraService.BitacoraGeneral("motivosInfraccion", Newtonsoft.Json.JsonConvert.SerializeObject(model, Formatting.Indented), OperacionBitacora.CREACION);

			return result;
		}

		public int EliminarMotivoInfraccion(int idMotivoInfraccion)
		{
			int result = 0;
			string strQuery = @"UPDATE motivosInfraccion
                                SET fechaActualizacion = getdate(),
                                    actualizadoPor = @actualizadoPor,
                                    estatus = @estatus
                                WHERE idMotivoInfraccion = @idMotivoInfraccion";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("idMotivoInfraccion", SqlDbType.Int)).Value = (object)idMotivoInfraccion;
					//command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("estatus", SqlDbType.Int)).Value = (object)0;
					result = command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}


		public bool CancelTramite(string id)
		{
			var result = false;

			string queryString = @"update infracciones set estatus=0 where idInfraccion=@id";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(queryString, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = (object)id ?? DBNull.Value;
					SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
					result = true;
				}
				catch (Exception e)
				{

				}
				finally
				{
					connection.Close();
				}
			}


			return result;
		}




		public bool UpdateFolio(string id, string folio)
		{

			var result = false;

			string queryString = @"update infracciones set folioinfraccion=@folio where idInfraccion=@id";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(queryString, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = (object)id ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@folio", SqlDbType.VarChar)).Value = (object)folio ?? DBNull.Value;
					SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
					result = true;
				}
				catch (Exception e)
				{

				}
				finally
				{
					connection.Close();
				}
			}


			return result;

		}

		public void ActualizConductor(int idInfraccion, int idConductor)
		{

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand("usp_ActualizaConductor", connection);
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)idConductor ?? DBNull.Value;
					var result = Convert.ToInt32(command.ExecuteScalar());
				}
				catch (Exception e)
				{

				}
				finally
				{
					connection.Close();
				}

			}
		}

		public InfraccionesModel GetInfraccion2ById(int idInfraccion, int idDependencia)
		{
			List<InfraccionesModel> modelList = new List<InfraccionesModel>();

			using (
				SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand("usp_ObtieneInfraccionPorId2", connection);
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesModel model = new InfraccionesModel();
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							model.idDelegacion = reader["idDelegacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDelegacion"].ToString());
							model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							model.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"].ToString());
							model.placasVehiculo = reader["placasVehiculo"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.nombreOficial = reader["nombreOficial"].ToString();
							model.apellidoPaternoOficial = reader["apellidoPaternoOficial"].ToString();
							model.ObservacionesSub = reader["ObservacionesSub"].ToString();
							model.ObservacionsesApl = reader["ObservacionsesApl"].ToString();
							model.idCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? 0 : ((int)reader["infraccionCortesia"]);
							model.estatusEnvio = reader["idEstatusEnvio"] is DBNull ? 0 : (int)reader["idEstatusEnvio"];
							model.emergenciasId = reader["emergenciasId"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["emergenciasId"].ToString());
							model.folioEmergencia = reader["folioEmergencia"] == System.DBNull.Value ? string.Empty : reader["folioEmergencia"].ToString();
							model.observacionesLugar = reader["observacionesLugar"] == DBNull.Value ? string.Empty : reader["observacionesLugar"].ToString();
							model.IdTurno = reader["idturno"] == DBNull.Value ? 0 : (int)reader["idturno"];
                            model.carretera = reader["carretera"].ToString();
							model.tramo = reader["tramo"].ToString();
							model.municipio = reader["municipio"].ToString();
							model.telefono = reader["telefono"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							DateTime fechaInfraccion = model.fechaInfraccion;
							string horaInfraccionString = reader["horaInfraccion"] == DBNull.Value ? null : reader["horaInfraccion"].ToString();

							TimeSpan horaInfraccionTimeSpan;

							if (horaInfraccionString != null)
							{
								horaInfraccionTimeSpan = TimeSpan.ParseExact(horaInfraccionString, "hhmm", CultureInfo.InvariantCulture);
							}
							else
							{
								horaInfraccionTimeSpan = TimeSpan.Zero;
							}

							DateTime fechaHoraInfraccion = fechaInfraccion.Date + horaInfraccionTimeSpan;

							model.fechaInfraccion = fechaHoraInfraccion;
							model.horaInfraccion = fechaHoraInfraccion;

							model.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();

							//*********************** ir a historico y ya no al registro actual de vehciulo, con el fin de traer la infor del vehiculo del momento en que se registro la infraccion
							//model.Vehiculo = _vehiculosService.GetVehiculoByIdSubMArcaAll((int)model.idVehiculo);
							if (model.idVehiculo != null)
							{
								model.Vehiculo = _vehiculosService.GetVehiculoHistoricoByIdAndIdinfraccion((int)model.idVehiculo, (int)model.idInfraccion);

							}
							else
							{
								model.Vehiculo = null;
							}

							//****************************************


							if (model.Vehiculo != null)
							{
								model.idPropitario = model.Vehiculo.idPersona;
								model.NombrePropietario = model.Vehiculo.Persona.nombreCompleto;
								//if (reader["NumTarjetaCirculacion"].ToString() !="")
								//  model.Vehiculo.tarjeta =  reader["NumTarjetaCirculacion"].ToString();
								//if (reader["placasVehiculo"].ToString()!="")
								//	model.Vehiculo.placas =  reader["placasVehiculo"].ToString();
								//if (reader["fechaVencimiento"] != System.DBNull.Value)
								//	model.Vehiculo.vigenciaTarjeta = Convert.ToDateTime(reader["fechaVencimiento"]);
							}
							else
							{
								//throw new Exception("Vehiculo es nulo, no se puede obtener datos.");
							};
							model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? false : ((int)reader["infraccionCortesia"]) == 1 ? false : true;
							model.NumTarjetaCirculacion = model.Vehiculo.tarjeta;
							model.Persona = _personasService.GetPersonaByIdInfraccion(model.idPersonaConductor, model.idInfraccion);
							model.PersonaInfraccion2 = _personasService.GetPersonaByIdHistorico((int)model.idPersonaConductor, (int)model.idInfraccion, 1);
							model.PersonaInfraccion = model.idPersonaConductor == null ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idInfraccion);
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.strIsPropietarioConductor = model.idPersona == null ? "-" : model.idPersona == model.idPropitario ? "Propietario" : "Conductor";
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							model.Garantia = model.Garantia ?? new GarantiaInfraccionModel();
							model.Garantia.garantia = model.Garantia.garantia ?? "";
							model.umas = GetUmas(model.fechaInfraccion);
							model.Nombreinventarios = reader["nombreArchivo"] is DBNull ? "" : reader["nombreArchivo"].ToString();
							model.inventarios = reader["archivoInventario"] is DBNull ? "" : reader["archivoInventario"].ToString();
							model.fechaVencimiento = reader["fechaVencimiento"] is DBNull ? DateTime.Now : (DateTime)reader["fechaVencimiento"];

                            if (model.MotivosInfraccion.Any(w => w.calificacion != null))
							{
								model.totalInfraccion = (model.MotivosInfraccion.Sum(s => (int)s.calificacion) * model.umas);
							}

							string filePath = reader["archivoInventario"].ToString();

							if (!string.IsNullOrEmpty(filePath))
							{
								var file = new FormFile(Stream.Null, 0, 0, "myFile", Path.GetFileName(filePath))
								{
									Headers = new HeaderDictionary(),
									ContentType = "application/octet-stream"
								};

								model.myFile = file;
							}
							else
							{
								model.myFile = null;
							}
							//Boleta fisica
							model.nombreBoletaStr = reader["nombreBoleta"] is DBNull ? "" : reader["nombreBoleta"].ToString();
							model.boletaFisicaPath = reader["rutaBoleta"] is DBNull ? "" : reader["rutaBoleta"].ToString();

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


			InfraccionesModel infraccionesModel = modelList.FirstOrDefault();
			//if (infraccionesModel != null)
			//{
			//	infraccionesModel.IdTurno = Convert.ToInt32(GetTurnoAsignedToInfraccionOrDefault(infraccionesModel.idInfraccion)?.IdTurno);
			//}


			return infraccionesModel;
		}

		public (string Latitud, string Longitud) GetLatitudLongitudPorInfraccion(int idInfraccion)
		{
			string latitud = null;
			string longitud = null;

			string strQuery = "SELECT longitud, latitud FROM infracciones WHERE idInfraccion = @idInfraccion";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					using (SqlCommand command = new SqlCommand(strQuery, connection))
					{
						command.CommandType = CommandType.Text;
						command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = idInfraccion;
						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								longitud = reader["longitud"] as string;
								latitud = reader["latitud"] as string;
							}
						}
					}
				}
				catch (SqlException ex)
				{
				}
			}

			// Retornamos los valores en una tupla
			return (latitud, longitud);
		}

		public InfraccionesModel GetInfraccion2ByIdMostrar(int idInfraccion, int idDependencia)
		{
			List<InfraccionesModel> modelList = new List<InfraccionesModel>();

			using (
				SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand("usp_ObtieneInfraccionPorIdMostrar", connection);
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesModel model = new InfraccionesModel();
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							model.idDelegacion = reader["idDelegacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDelegacion"].ToString());
							model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
							model.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"].ToString());
							model.placasVehiculo = reader["placasVehiculo"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.nombreOficial = reader["nombreOficial"].ToString();
							model.apellidoPaternoOficial = reader["apellidoPaternoOficial"].ToString();
							model.ObservacionesSub = reader["ObservacionesSub"].ToString();
							model.ObservacionsesApl = reader["ObservacionsesApl"].ToString();
							model.idCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? 0 : ((int)reader["infraccionCortesia"]);
							model.estatusEnvio = reader["idEstatusEnvio"] is DBNull ? 0 : (int)reader["idEstatusEnvio"];


							model.carretera = reader["carretera"].ToString();
							model.tramo = reader["tramo"].ToString();
							model.municipio = reader["municipio"].ToString();
							model.telefono = reader["telefono"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							DateTime fechaInfraccion = model.fechaInfraccion;
							string horaInfraccionString = reader["horaInfraccion"] == DBNull.Value ? null : reader["horaInfraccion"].ToString();

							TimeSpan horaInfraccionTimeSpan;

							if (horaInfraccionString != null)
							{
								horaInfraccionTimeSpan = TimeSpan.ParseExact(horaInfraccionString, "hhmm", CultureInfo.InvariantCulture);
							}
							else
							{
								horaInfraccionTimeSpan = TimeSpan.Zero;
							}

							DateTime fechaHoraInfraccion = fechaInfraccion.Date + horaInfraccionTimeSpan;

							model.fechaInfraccion = fechaHoraInfraccion;
							model.horaInfraccion = fechaHoraInfraccion;

							model.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();

							//*********************** ir a historico y ya no al registro actual de vehciulo, con el fin de traer la infor del vehiculo del momento en que se registro la infraccion
							//model.Vehiculo = _vehiculosService.GetVehiculoByIdSubMArcaAll((int)model.idVehiculo);
							if (model.idVehiculo != null)
							{
								model.Vehiculo = _vehiculosService.GetVehiculoHistoricoByIdAndIdinfraccion((int)model.idVehiculo, (int)model.idInfraccion);

							}
							else
							{
								model.Vehiculo = null;
							}

							//****************************************


							if (model.Vehiculo != null)
							{
								model.idPropitario = model.Vehiculo.idPersona;
								model.NombrePropietario = model.Vehiculo.Persona.nombre;
								//if (reader["NumTarjetaCirculacion"].ToString() !="")
								//  model.Vehiculo.tarjeta =  reader["NumTarjetaCirculacion"].ToString();
								//if (reader["placasVehiculo"].ToString()!="")
								//	model.Vehiculo.placas =  reader["placasVehiculo"].ToString();
								//if (reader["fechaVencimiento"] != System.DBNull.Value)
								//	model.Vehiculo.vigenciaTarjeta = Convert.ToDateTime(reader["fechaVencimiento"]);
							}
							else
							{
								//throw new Exception("Vehiculo es nulo, no se puede obtener datos.");
							};
							model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? false : ((int)reader["infraccionCortesia"]) == 1 ? false : true;
							model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							model.tarjetaInfraccion = reader["vehiculoDocumento"] == System.DBNull.Value ? string.Empty : reader["vehiculoDocumento"].ToString();

							model.Persona = _personasService.GetPersonaByIdHistorico((int)model.idPersona, model.idInfraccion, 2);
							model.PersonaInfraccion2 = _personasService.GetPersonaByIdHistorico((int)model.idPersonaConductor, (int)model.idInfraccion, 1);
							model.PersonaInfraccion = model.idPersonaConductor == null ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idInfraccion);
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.strIsPropietarioConductor = model.idPersona == null ? "-" : model.idPersona == model.idPropitario ? "Propietario" : "Conductor";
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							model.Garantia = model.Garantia ?? new GarantiaInfraccionModel();
							model.Garantia.garantia = model.Garantia.garantia ?? "";
							model.umas = GetUmas(model.fechaInfraccion);
							model.Nombreinventarios = reader["nombreArchivo"] is DBNull ? "" : reader["nombreArchivo"].ToString();
							model.inventarios = reader["archivoInventario"] is DBNull ? "" : reader["archivoInventario"].ToString();
							model.aplicacion = reader["aplicacion"] is DBNull ? "" : reader["aplicacion"].ToString();

							if (model.MotivosInfraccion.Any(w => w.calificacion != null))
							{
								model.totalInfraccion = (model.MotivosInfraccion.Sum(s => (int)s.calificacion) * model.umas);
							}

							string filePath = reader["archivoInventario"].ToString();

							if (!string.IsNullOrEmpty(filePath))
							{
								var file = new FormFile(Stream.Null, 0, 0, "myFile", Path.GetFileName(filePath))
								{
									Headers = new HeaderDictionary(),
									ContentType = "application/octet-stream"
								};

								model.myFile = file;
							}
							else
							{
								model.myFile = null;
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

			return modelList.FirstOrDefault();
		}


		public int ExitDesposito(int idInfraccion)
		{
			var result = 1;

			string strQuer = @"select count(*) result from depositos where idinfraccion=@IdInfraccion";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{

					connection.Open();
					SqlCommand command = new SqlCommand(strQuer, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@IdInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{

						while (reader.Read())
						{
							result = (int)reader["result"];
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

		public NuevaInfraccionModel GetInfraccionAccidenteById(int idInfraccion, int idDependencia)
		{
			List<NuevaInfraccionModel> modelList = new List<NuevaInfraccionModel>();
			string strQuery = @"SELECT idInfraccion
                                      ,idOficial
                                      ,idDependencia
                                      ,idDelegacion
                                      ,idVehiculo
                                      ,idAplicacion
                                      ,idGarantia
                                      ,idEstatusInfraccion
                                      ,idMunicipio
                                      ,idTramo
                                      ,idCarretera
                                      ,idPersona
                                      ,ISNULL(idPersonaConductor,0) idPersonaConductor
                                      ,placasVehiculo
                                      ,folioInfraccion
                                      ,fechaInfraccion
                                      ,kmCarretera
                                      ,observaciones
                                      ,lugarCalle
                                      ,lugarNumero
                                      ,lugarColonia
                                      ,lugarEntreCalle
                                      ,infraccionCortesia
                                      ,NumTarjetaCirculacion
                                      ,fechaActualizacion
                                      ,actualizadoPor
                                      ,estatus
                               FROM infracciones
                               WHERE estatus = 1
                               AND idInfraccion = @idInfraccion AND transito = @idDependencia"
			;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							NuevaInfraccionModel model = new NuevaInfraccionModel();
							model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
							model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
							model.idDelegacion = reader["idDelegacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDelegacion"].ToString());
							model.IdVehiculo = (int)(reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString()));
							model.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
							model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
							model.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
							model.IdMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.IdTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.IdCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.IdPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							model.IdPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"].ToString());
							model.Placa = reader["placasVehiculo"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.Kilometro = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
							model.Tarjeta = reader["NumTarjetaCirculacion"].ToString();
							model.Persona = _personasService.GetPersonaByIdInfraccion((int)model.IdPersona, model.idInfraccion);
							model.PersonaInfraccion = model.IdPersonaConductor == 0 ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idInfraccion);
							model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.IdVehiculo);
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							model.umas = GetUmas(model.fechaInfraccion);
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



			return modelList.FirstOrDefault();
		}


		public decimal GetUmas(DateTime? fecha = null)
		{

			fecha = fecha ?? DateTime.Now;

			decimal umas = 0M;
			string strQuery = @"SELECT top 1 format(salario,'#.##') salario
                               FROM catSalariosMinimos
                               WHERE estatus = 1 and fecha<= @fecha  order by fecha desc, idSalario asc"
			;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@fecha", SqlDbType.DateTime)).Value = (object)fecha ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							umas = reader["salario"] == System.DBNull.Value ? default(decimal) : Convert.ToDecimal(reader["salario"].ToString());
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
			return umas;
		}

		public List<EstadisticaInfraccionMotivosModel> GetAllEstadisticasInfracciones(int idOficina, int idDependencia)
		{
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
                                WHERE m.estatus = 1
                               AND inf.estatus = 1 AND inf.idDelegacion = @idOficina AND inf.transito = @idDependencia
							   group by ci.nombre"
		   ;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
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

		public List<EstadisticaInfraccionMotivosModel> GetBusquedaEstadisticasInfracciones(IncidenciasBusquedaModel modelBusqueda, int idDependencia)
		{
			string condiciones = "";

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
                                WHERE m.estatus = 1
                               AND inf.estatus = 1 AND inf.transito = @idDependencia " + condiciones + condicionFecha + @"
                               group by ci.nombre"
		   ;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					// command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
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
		public List<EstadisticaInfraccionMotivosModel> GetAllMotivosPorInfraccion(int idOficina, int idDependencia)
		{
			List<EstadisticaInfraccionMotivosModel> modelList = new List<EstadisticaInfraccionMotivosModel>();
			string strQuery = @"SELECT numeroMotivos, COUNT(idInfraccion) AS CantidadInfracciones
                                    FROM (
                                        SELECT mi.idInfraccion, COUNT(mi.idMotivoInfraccion) AS numeroMotivos
                                        FROM infracciones i
                                        LEFT JOIN motivosInfraccion mi ON i.idInfraccion = mi.idInfraccion
                                        WHERE i.idDelegacion = @idOficina AND i.transito = @idDependencia AND i.estatus = 1
                                        GROUP BY mi.idInfraccion
                                    ) AS InfraccionesConMotivos
                                    GROUP BY numeroMotivos
                                    HAVING numeroMotivos > 0;";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							EstadisticaInfraccionMotivosModel model = new EstadisticaInfraccionMotivosModel();
							model.NumeroMotivos = reader["numeroMotivos"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["numeroMotivos"].ToString());
							model.ContadorMotivos = reader["CantidadInfracciones"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["CantidadInfracciones"].ToString());
							model.ResultadoMultiplicacion = model.NumeroMotivos * model.ContadorMotivos;
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

		public List<EstadisticaInfraccionMotivosModel> GetAllMotivosPorInfraccionBusqueda(IncidenciasBusquedaModel modelBusqueda, int idoficina, int idDependencia)
		{
			string condiciones = "";
			//condiciones += modelBusqueda.idTipoMotivo.Equals(null)  ? "" : " AND i.transito = @transito ";

			condiciones += modelBusqueda.idDelegacion.Equals(null) || modelBusqueda.idDelegacion == 0 ? "" : " AND i.idDelegacion = @idDelegacion ";
			condiciones += modelBusqueda.idOficial.Equals(null) || modelBusqueda.idOficial == 0 ? "" : " AND i.idOficial =@idOficial ";
			condiciones += modelBusqueda.idCarretera.Equals(null) || modelBusqueda.idCarretera == 0 ? "" : " AND i.idCarretera = @idCarretera ";
			condiciones += modelBusqueda.idTramo.Equals(null) || modelBusqueda.idTramo == 0 ? "" : " AND i.idTramo = @idTramo ";
			condiciones += modelBusqueda.idTipoVehiculo.Equals(null) || modelBusqueda.idTipoVehiculo == 0 ? "" : " AND veh.idTipoVehiculo = @idTipoVehiculo ";
			condiciones += modelBusqueda.idTipoServicio.Equals(null) || modelBusqueda.idTipoServicio == 0 ? "" : " AND veh.idCatTipoServicio  = @idCatTipoServicio ";
			condiciones += modelBusqueda.idSubTipoServicio.Equals(null) || modelBusqueda.idSubTipoServicio == 0 ? "" : " AND veh.idSubtipoServicio  = @idSubTipoServicio ";
			condiciones += modelBusqueda.idTipoLicencia.Equals(null) || modelBusqueda.idTipoLicencia == 0 ? "" : " AND gar.idTipoLicencia = @idTipoLicencia ";
			condiciones += modelBusqueda.idMunicipio.Equals(null) || modelBusqueda.idMunicipio == 0 ? "" : " AND i.idMunicipio =@idMunicipio ";
			condiciones += modelBusqueda.IdTipoCortesia.Equals(null) || modelBusqueda.IdTipoCortesia == 0 ? "" : " AND i.infraccionCortesia=@IdTipoCortesia ";

			string condicionFecha = "";

			if (modelBusqueda.fechaInicio != DateTime.MinValue && modelBusqueda.fechaFin != DateTime.MinValue)
				condiciones += " AND i.fechaInfraccion >= @fechaInicio AND i.fechaInfraccion  <= @fechaFin ";
			else if (modelBusqueda.fechaInicio != DateTime.MinValue && modelBusqueda.fechaFin == DateTime.MinValue)
				condiciones += " AND i.fechaInfraccion >= @fechaInicio ";
			else if (modelBusqueda.fechaInicio == DateTime.MinValue && modelBusqueda.fechaFin != DateTime.MinValue)
				condiciones += " AND i.fechaInfraccion <= @fechaFin ";
			else
				condiciones += "";
			List<EstadisticaInfraccionMotivosModel> modelList = new List<EstadisticaInfraccionMotivosModel>();
			string strQuery = @"SELECT numeroMotivos, COUNT(idInfraccion) AS CantidadInfracciones
                                    FROM (
                                        SELECT mi.idInfraccion, COUNT(mi.idMotivoInfraccion) AS numeroMotivos
                                        FROM infracciones i
                                        LEFT JOIN motivosInfraccion mi ON i.idInfraccion = mi.idInfraccion
                                        LEFT JOIN vehiculos veh ON i.idVehiculo = veh.idVehiculo
										left join garantiasInfraccion gar on i.idinfraccion = gar.idinfraccion
                                        WHERE i.estatus = 1 AND i.transito = @transito" + condiciones + condicionFecha + @"
                                        GROUP BY mi.idInfraccion
                                    ) AS InfraccionesConMotivos
                                    GROUP BY numeroMotivos
                                    HAVING numeroMotivos > 0;";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					if (modelBusqueda.idTipoMotivo.Equals(null))
						command.Parameters.Add(new SqlParameter("@transito", SqlDbType.Int)).Value = idDependencia;
					if (!modelBusqueda.idTipoMotivo.Equals(null))
						command.Parameters.Add(new SqlParameter("@transito", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoMotivo ?? DBNull.Value;

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


					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							EstadisticaInfraccionMotivosModel model = new EstadisticaInfraccionMotivosModel();
							model.NumeroMotivos = reader["numeroMotivos"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["numeroMotivos"].ToString());
							model.ContadorMotivos = reader["CantidadInfracciones"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["CantidadInfracciones"].ToString());
							model.ResultadoMultiplicacion = model.NumeroMotivos * model.ContadorMotivos;

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
		//TODO: Borrar esta consulta ya no sirve para estadisticas
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
							model.municipio = reader["municipio"].ToString();
							model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
							model.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"].ToString());
							model.placasVehiculo = reader["placasVehiculo"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
							model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
							model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
							model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
							model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
							model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
							model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
							model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
							model.Persona = _personasService.GetPersonaByIdInfraccion((int)model.idPersona, model.idInfraccion);
							model.PersonaInfraccion = model.idPersonaConductor == null ? new PersonaInfraccionModel() : GetPersonaInfraccionById((int)model.idInfraccion);
							model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
							model.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
							model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)model.idInfraccion);
							model.umas = GetUmas(model.fechaInfraccion);
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

		public List<InfoInfraccion> GetAllInfraccionesEstadisticasGrid(IncidenciasBusquedaModelEstadisticas modelBusqueda, int idDependencia, Pagination pagination)
		{
			List<InfoInfraccion> modelList = new List<InfoInfraccion>();
			SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection());

			try
			{


				connection.Open();
				SqlCommand command = new SqlCommand("[usp_GRDEstadisticasInfraccionesGridInfraccionesInfo]", connection);
				command.CommandType = CommandType.StoredProcedure;


				command.Parameters.AddWithValue("@PageIndex", pagination.PageIndex);
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

				command.Parameters.AddWithValue("@transito", idDependencia);
				command.Parameters.AddWithValue("@idDelegacion", modelBusqueda.idDelegacion);
				command.Parameters.AddWithValue("@idOficial", modelBusqueda.idOficial);
				command.Parameters.AddWithValue("@idCarretera", modelBusqueda.idCarretera);
				command.Parameters.AddWithValue("@idTramo", modelBusqueda.idTramo);
				command.Parameters.AddWithValue("@idTipoVehiculo", modelBusqueda.idTipoVehiculo);
				command.Parameters.AddWithValue("@idCatTipoServicio", modelBusqueda.idTipoServicio);
				command.Parameters.AddWithValue("@idSubtipoServicio", modelBusqueda.idSubTipoServicio);
				command.Parameters.AddWithValue("@idTipoLicencia", modelBusqueda.idTipoLicencia);
				command.Parameters.AddWithValue("@idMunicipio", modelBusqueda.idMunicipio);
				command.Parameters.AddWithValue("@IdTipoCortesia", modelBusqueda.IdTipoCortesia);
				command.Parameters.AddWithValue("@fechaInicio", modelBusqueda.fechaInicio);
				command.Parameters.AddWithValue("@fechaFin", modelBusqueda.fechaFin);
				try
				{
					SqlDataReader reader = command.ExecuteReader();

					while (reader.Read())
					{
						InfoInfraccion model = new InfoInfraccion();
						model.Folio = reader["Folio"] == System.DBNull.Value ? default(string) : reader["Folio"].ToString();
						model.FolioTxt = reader["FolioTXT"] == System.DBNull.Value ? default(string) : reader["FolioTXT"].ToString();
						model.TTOTTE = reader["TTOTTE"] == System.DBNull.Value ? default(string) : reader["TTOTTE"].ToString();
						model.Estatus = reader["Estatus"] == System.DBNull.Value ? default(string) : reader["Estatus"].ToString();
						model.TipoCortesia = reader["TipoCortesia"] == System.DBNull.Value ? default(string) : reader["TipoCortesia"].ToString();
						model.Delegacion = reader["Delegacion"] == System.DBNull.Value ? default(string) : reader["Delegacion"].ToString();
						model.Municipio = reader["Municipio"] == System.DBNull.Value ? default(string) : reader["Municipio"].ToString();
						model.FechaInfraccion = reader["FechaInfraccion"] == System.DBNull.Value ? default(string) : reader["FechaInfraccion"].ToString();
						model.HoraInfraccion = reader["HoraInfraccion"] == System.DBNull.Value ? default(string) : reader["HoraInfraccion"].ToString();
						model.FechaVencimiento = reader["FechaVencimiento"] == System.DBNull.Value ? default(string) : reader["FechaVencimiento"].ToString();
						model.Carretera = reader["Carretera"] == System.DBNull.Value ? default(string) : reader["Carretera"].ToString();
						model.Tramo = reader["Tramo"] == System.DBNull.Value ? default(string) : reader["Tramo"].ToString();
						model.Kilometraje = reader["Kilometraje"] == System.DBNull.Value ? default(string) : reader["Kilometraje"].ToString();
						model.NombreConductor = reader["NombreConductor"] == System.DBNull.Value ? default(string) : reader["NombreConductor"].ToString();
						model.CURPConductor = reader["CURPConductor"] == System.DBNull.Value ? default(string) : reader["CURPConductor"].ToString();
						model.FechadeNacimiento = reader["FechadeNacimientoConductor"] == System.DBNull.Value ? default(string) : reader["FechadeNacimientoConductor"].ToString();
						model.DomicilioConductor = reader["DomicilioConductor"] == System.DBNull.Value ? default(string) : reader["DomicilioConductor"].ToString();
						model.LicenciaConductor = reader["LicenciaConductor"] == System.DBNull.Value ? default(string) : reader["LicenciaConductor"].ToString();
						model.TipoPersFisica = reader["TipoPersFisica"] == System.DBNull.Value ? default(string) : reader["TipoPersFisica"].ToString();
						model.TipoPersMoral = reader["TipoPersMoral"] == System.DBNull.Value ? default(string) : reader["TipoPersMoral"].ToString();
						model.Propietario = reader["Propietario"] == System.DBNull.Value ? default(string) : reader["Propietario"].ToString();
						model.OficialInfraccion = reader["OficialInfraccion"] == System.DBNull.Value ? default(string) : reader["OficialInfraccion"].ToString();
						model.CalifSalarios = reader["Calificacion"] == System.DBNull.Value ? default(string) : reader["Calificacion"].ToString();
						model.MontoCalif = reader["MontoCalif"] == System.DBNull.Value ? default(string) : reader["MontoCalif"].ToString();
						model.MontoPago = reader["MontoPago"] == System.DBNull.Value ? default(string) : reader["MontoPago"].ToString();
						model.ReciboPago = reader["ReciboPago"] == System.DBNull.Value ? default(string) : reader["ReciboPago"].ToString();
						model.FechaPago = reader["FechaPago"] == System.DBNull.Value ? default(string) : reader["FechaPago"].ToString();
						model.Placas = reader["Placas"] == System.DBNull.Value ? default(string) : reader["Placas"].ToString();
						model.SerieVeh = reader["SerieVeh"] == System.DBNull.Value ? default(string) : reader["SerieVeh"].ToString();
						model.numeroEconomicoVeh = reader["numeroEconomico"] == System.DBNull.Value ? default(string) : reader["numeroEconomico"].ToString();
						model.TarjetadeCirculacion = reader["TarjetadeCirculacion"] == System.DBNull.Value ? default(string) : reader["TarjetadeCirculacion"].ToString();
						model.Marca = reader["Marca"] == System.DBNull.Value ? default(string) : reader["Marca"].ToString();
						model.Submarca = reader["Submarca"] == System.DBNull.Value ? default(string) : reader["Submarca"].ToString();
						model.Modelo = reader["Modelo"] == System.DBNull.Value ? default(string) : reader["Modelo"].ToString();
						model.Color = reader["Color"] == System.DBNull.Value ? default(string) : reader["Color"].ToString();
						model.EntidaddeReg = reader["EntidaddeReg"] == System.DBNull.Value ? default(string) : reader["EntidaddeReg"].ToString();
						model.TipodeVehículo = reader["TipodeVehículo"] == System.DBNull.Value ? default(string) : reader["TipodeVehículo"].ToString();
						model.TipodeServicio = reader["TipodeServicio"] == System.DBNull.Value ? default(string) : reader["TipodeServicio"].ToString();
						model.SubtipodeServicio = reader["SubtipodeServicio"] == System.DBNull.Value ? default(string) : reader["SubtipodeServicio"].ToString();
						model.TipoGarantia = reader["garantia"] == System.DBNull.Value ? default(string) : reader["garantia"].ToString();
						model.TipoAplicacion = reader["TipoAplicacion"] == System.DBNull.Value ? default(string) : reader["TipoAplicacion"].ToString();
						model.Motivo = reader["Motivos"] == System.DBNull.Value ? default(string) : reader["Motivos"].ToString();
						model.MotivoDesc = reader["MotivoDesc"] == System.DBNull.Value ? default(string) : reader["MotivoDesc"].ToString();
						model.Total = reader["Total"] != DBNull.Value ? Convert.ToInt32(reader["Total"]) : 0;
						model.Numero = reader["rowIndex"] != DBNull.Value ? Convert.ToInt32(reader["rowIndex"]) : 0;

						model.NumeroSecuencial = 1;
						modelList.Add(model);

					}
				}
				catch (Exception ex)
				{
					var q = ex;
				}


				command.Clone();
				command.Dispose();
			}
			catch (SqlException ex)
			{

				var t = ex;

				//Guardar la excepcion en algun log de errores
				//ex
			}
			finally
			{
				connection.Close();



			}

			return modelList;
		}

		public List<InfraccionesModel> GetAllAccidentes2()
		{
			List<InfraccionesModel> modelList = new List<InfraccionesModel>();
			string strQuery = @"SELECT inf.idAccidente
                                      ,inf.idMunicipio
                                      ,mun.municipio
                                      ,inf.idCarretera
                                      ,inf.idTramo
                                      ,inf.kilometro
                                      ,inf.fecha
                                      ,inf.fechaActualizacion
                                      ,inf.actualizadoPor
                                      ,inf.estatus
                               FROM accidentes inf
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
							model.idInfraccion = reader["idAccidente"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idAccidente"].ToString());
							model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
							model.municipio = reader["municipio"].ToString();
							model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
							model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
							model.kmCarretera = reader["kilometro"] == System.DBNull.Value ? string.Empty : reader["kilometro"].ToString();
							model.fechaInfraccion = reader["fecha"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fecha"].ToString());
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

		public bool ValidarFolio(string folioInfraccion, int idDependencia)
		{
			int folio = 0;


			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				connection.Open();

				string query = "SELECT COUNT(*) AS Result FROM infracciones WHERE folioInfraccion = @folioInfraccion and  year(fechaInfraccion) = year(getdate()) AND transito = @idDependencia and estatus = 1";

				using (SqlCommand command = new SqlCommand(query, connection))
				{

					command.Parameters.AddWithValue("@folioInfraccion", folioInfraccion.Trim());
					command.Parameters.AddWithValue("@idDependencia", idDependencia);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{

							folio = reader["Result"] == DBNull.Value ? default(int) : Convert.ToInt32(reader["Result"]);
						}
					}
				}
			}
			return folio > 0;
		}



		public bool UpdateFolioS(string id, string folio)
		{
			var result = true;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "UPDATE infracciones SET folioinfraccion = @folio WHERE idInfraccion = @idAccidente and estatus=1 and (idEstatusEnvio = 0 or idEstatusEnvio is null)";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@idAccidente", id);
					command.Parameters.AddWithValue("@folio", folio);
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

		public CatalogModel GetPathFile(string folio)
		{

			var result = new CatalogModel();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "select archivoInventario as result,nombreArchivo as filename from  infracciones  where idInfraccion = @Infraccion";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@Infraccion", folio);
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{

						while (reader.Read())
						{
							result.value = (string)reader["result"];
							result.text = reader["result"] is DBNull ? "" : (string)reader["filename"];
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
					string query = "select count(*) as result from  infracciones where folioInfraccion = @folio  and estatus = 1 ";

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


		/// <summary>
		/// INSERTA INFRACCION / INFRACCION CONDUCTOR.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="IdDependencia"></param>
		/// <returns></returns>
		public int CrearInfraccion(InfraccionesModel model, int IdDependencia)
		{
			int result = 0;
			DateTime fechaVencimiento = model.fechaVencimiento.Date;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				connection.Open();
				using (SqlTransaction trx = connection.BeginTransaction())
				{
					try
					{
						SqlCommand command = new SqlCommand("usp_InsertaInfraccion", connection, trx);
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.Add(new SqlParameter("fechaVencimiento", SqlDbType.Date)).Value = (object)fechaVencimiento;
						command.Parameters.Add(new SqlParameter("fechaInfraccion", SqlDbType.DateTime)).Value = (object)model.fechaInfraccion;
						DateTime fechaInfraccion = model.horaInfraccion;
						TimeSpan horaInfraccion = fechaInfraccion.TimeOfDay;

						string horaFormateada = horaInfraccion.ToString("hhmm");
						string horaInfraccionString = horaFormateada;
						command.Parameters.Add(new SqlParameter("horaInfraccion", SqlDbType.VarChar)).Value = horaInfraccionString;
						command.Parameters.Add(new SqlParameter("folioInfraccion", SqlDbType.VarChar)).Value = (object)model.folioInfraccion.Trim().ToUpper();
						command.Parameters.Add(new SqlParameter("folioEmergencia", SqlDbType.NVarChar)).Value = (object)(model.folioEmergencia != null ? model.folioEmergencia.ToUpper():null) ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("idOficial", SqlDbType.Int)).Value = (object)model.idOficial;
						command.Parameters.Add(new SqlParameter("idDelegacion", SqlDbType.Int)).Value = (object)model.idDelegacion;
						command.Parameters.Add(new SqlParameter("idMunicipio", SqlDbType.Int)).Value = (object)model.idMunicipio;
						command.Parameters.Add(new SqlParameter("idCarretera", SqlDbType.Int)).Value = (object)model.idCarretera;
						command.Parameters.Add(new SqlParameter("idTramo", SqlDbType.Int)).Value = (object)model.idTramo;
						command.Parameters.Add(new SqlParameter("kmCarretera", SqlDbType.VarChar)).Value = (object)model.kmCarretera;
						command.Parameters.Add(new SqlParameter("lugarCalle", SqlDbType.VarChar)).Value = (object)model.lugarCalle == null ? "" : (object)model.lugarCalle.ToUpper();
						command.Parameters.Add(new SqlParameter("lugarNumero", SqlDbType.VarChar)).Value = (object)model.lugarNumero == null ? "" : (object)model.lugarNumero.ToUpper();
						command.Parameters.Add(new SqlParameter("lugarColonia", SqlDbType.VarChar)).Value = (object)model.lugarColonia == null ? "" : (object)model.lugarColonia.ToUpper();
						command.Parameters.Add(new SqlParameter("lugarEntreCalle", SqlDbType.VarChar)).Value = (object)model.lugarEntreCalle == null ? "" : (object)model.lugarEntreCalle.ToUpper();
						command.Parameters.Add(new SqlParameter("idVehiculo", SqlDbType.Int)).Value = (object)model.idVehiculo;
						command.Parameters.Add(new SqlParameter("idPersonaConductor", SqlDbType.Int)).Value = (object)model.idPersona;
						command.Parameters.Add(new SqlParameter("placasVehiculo", SqlDbType.VarChar)).Value = (object)(String.IsNullOrEmpty(model.placasVehiculo) ? "" : model.placasVehiculo.Trim(new Char[] { ' ', '-' }).ToUpper());
						command.Parameters.Add(new SqlParameter("NumTarjetaCirculacion", SqlDbType.VarChar)).Value =
							!string.IsNullOrEmpty(model.NumTarjetaCirculacion) ? (object)model.NumTarjetaCirculacion.ToUpper() : DBNull.Value;
						//command.Parameters.Add(new SqlParameter("idEstatusInfraccion", SqlDbType.Int)).Value = (object)model.idEstatusInfraccion;
						command.Parameters.Add(new SqlParameter("idDependencia", SqlDbType.Int)).Value = IdDependencia;
						command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
						command.Parameters.Add(new SqlParameter("estatus", SqlDbType.Int)).Value = (object)1;

						command.Parameters.Add(new SqlParameter("observacionesLugar", SqlDbType.VarChar)).Value =
						string.IsNullOrEmpty(model.observacionesLugar) ? (object)"" : (object)model.observacionesLugar.Trim();

						result = Convert.ToInt32(command.ExecuteScalar());

						model.idInfraccion = result;

						// si hay un turno asociado, registramos la infraccion en el turno
						if (model.IdTurno.HasValue)
						{
							SqlCommand turnoCommand = new("INSERT INTO turnoInfracciones (idTurno, idInfraccion) VALUES (@IdTurno, @IdInfraccion)", connection, trx);
							turnoCommand.Parameters.Add(new SqlParameter("IdTurno", SqlDbType.BigInt)).Value = model.IdTurno.Value;
							turnoCommand.Parameters.Add(new SqlParameter("IdInfraccion", SqlDbType.Int)).Value = result;
							turnoCommand.ExecuteNonQuery();
						}

						trx.Commit();

						ModificarCoordenadasInfraccion(model);
					}
					catch (Exception)
					{
						trx.Rollback();
					}
					finally
					{
						connection.Close();
					}
				}
			}

			//Crear historico vehiculo infraccion
			try
			{
				var bitacoraVehiculo = _vehiculosService.CrearHistoricoVehiculo(result, (int)model.idVehiculo, 1);
			}
			catch { }

			return result;
		}


		public decimal MontoMotivos(int idInfraccion)
		{
			decimal conteo = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				string query = @"
								select sum(calificacion)*(select top 1 salario from catSalariosMinimos where fecha<max(b.fechaInfraccion) and estatus=1 order by fecha desc ) monto
								from motivosInfraccion a
								join infracciones b on a.idInfraccion = b.idInfraccion
								where a.idInfraccion = @IdInfraccion and a.estatus=1 and b.estatus=1
								";
				SqlCommand command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@IdInfraccion", idInfraccion);

				try
				{
					connection.Open();

					var reader = command.ExecuteReader();

					while (reader.Read())
					{
						conteo = Convert.ToDecimal(reader["monto"]);

					}

				}
				catch (Exception ex)
				{
					Console.WriteLine("Error al ejecutar la consulta: " + ex.Message);
				}
			}

			return conteo;
		}


		public int ConteoMotivos(int idInfraccion)
		{
			int conteo = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				string query = "SELECT COUNT(*) FROM motivosInfraccion WHERE idInfraccion = @IdInfraccion AND estatus = 1";

				SqlCommand command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@IdInfraccion", idInfraccion);

				try
				{
					connection.Open();
					conteo = (int)command.ExecuteScalar();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error al ejecutar la consulta: " + ex.Message);
				}
			}

			return conteo;
		}
		/// <summary>
		/// HMG 
		/// ACTUALIZACIÓN A INFRACCION Y VEHICULO
		/// </summary>
		/// <param name="model"></param>
		/// <param name="vehiculo"></param>
		/// <returns></returns>
		public int ModificarInfraccion(InfraccionesModel model)
		{
			int result = 0;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				connection.Open();
				using (SqlTransaction trx = connection.BeginTransaction())
				{
					try
					{
						SqlCommand command = new SqlCommand("usp_UpdateInfraccionesVehiculos", connection, trx);
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.Add(new SqlParameter("idInfraccion", SqlDbType.Int)).Value = (object)model.idInfraccion;
						command.Parameters.Add(new SqlParameter("idOficial", SqlDbType.Int)).Value = (object)model.idOficial ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("idDependencia", SqlDbType.Int)).Value = (object)model.idDependencia ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("idDelegacion", SqlDbType.Int)).Value = (object)model.idDelegacion ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("idVehiculo", SqlDbType.Int)).Value = (object)model.idVehiculo ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("idAplicacion", SqlDbType.Int)).Value = (object)model.idAplicacion ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("idGarantia", SqlDbType.Int)).Value = (object)model.Garantia.idCatGarantia ?? DBNull.Value;
						model.idEstatusInfraccion = (int)CatEnumerator.catEstatusInfraccion.Capturada;
						command.Parameters.Add(new SqlParameter("idEstatusInfraccion", SqlDbType.Int)).Value = (object)model.idEstatusInfraccion ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("idMunicipio", SqlDbType.Int)).Value = (object)model.idMunicipio ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("idTramo", SqlDbType.Int)).Value = (object)model.idTramo ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("idCarretera", SqlDbType.Int)).Value = (object)model.idCarretera ?? DBNull.Value;
						//if (model.Vehiculo.Persona!=null)
						//command.Parameters.Add(new SqlParameter("idPersona", SqlDbType.Int)).Value = (object)model.Vehiculo.idPersona ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("idPersona", SqlDbType.Int)).Value = (object)model.Persona.idPersona ?? DBNull.Value;
						//else
						//command.Parameters.Add(new SqlParameter("idPersona", SqlDbType.Int)).Value = DBNull.Value;

						command.Parameters.Add(new SqlParameter("idPersonaConductor", SqlDbType.Int)).Value = (object)model.idPersonaConductor ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("placasVehiculo", SqlDbType.NVarChar)).Value = (object)model.placasVehiculo?.ToUpper() ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("folioInfraccion", SqlDbType.NVarChar)).Value = (object)model.folioInfraccion?.ToUpper() ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("kmCarretera", SqlDbType.NVarChar)).Value = (object)model.kmCarretera ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("observaciones", SqlDbType.NVarChar)).Value = (object)model.observaciones ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("lugarCalle", SqlDbType.NVarChar)).Value = (object)model.lugarCalle?.ToUpper() ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("lugarNumero", SqlDbType.NVarChar)).Value = (object)model.lugarNumero?.ToUpper() ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("lugarColonia", SqlDbType.NVarChar)).Value = (object)model.lugarColonia?.ToUpper() ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("lugarEntreCalle", SqlDbType.NVarChar)).Value = (object)model.lugarEntreCalle?.ToUpper() ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("infraccionCortesia", SqlDbType.Int)).Value = (object)model.cortesiaInt ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("NumTarjetaCirculacion", SqlDbType.NVarChar)).Value = (object)model.Vehiculo?.tarjeta?.ToUpper() ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
						command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
						command.Parameters.Add(new SqlParameter("fechaInfraccion", SqlDbType.DateTime)).Value = (object)model.fechaInfraccion;
						command.Parameters.Add(new SqlParameter("@fechaVencimiento", SqlDbType.Date)).Value = (object)model.fechaVencimiento;
						command.Parameters.AddWithValue("monto", (object)model.monto ?? 0);

						command.Parameters.Add(new SqlParameter("emergenciasId", SqlDbType.Int)).Value = (object)model.emergenciasId ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("folioEmergencia", SqlDbType.NVarChar)).Value = (object)model.folioEmergencia?.ToUpper() ?? DBNull.Value;


						DateTime fechaInfraccion = model.horaInfraccion;
						TimeSpan horaInfraccion = fechaInfraccion.TimeOfDay;
						string horaFormateada = horaInfraccion.ToString("hhmm");
						string horaInfraccionString = horaFormateada;
						command.Parameters.Add(new SqlParameter("horaInfraccion", SqlDbType.NVarChar)).Value = horaInfraccionString;

						command.Parameters.Add(new SqlParameter("vigenciaTarjeta", SqlDbType.DateTime2)).Value = (object)model.Vehiculo?.vigenciaTarjeta ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("motor", SqlDbType.NVarChar)).Value = (object)model.Vehiculo?.motor?.ToUpper() ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@numeroEconomico", SqlDbType.NVarChar)).Value = (object)model.Vehiculo?.numeroEconomico?.ToUpper() ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@otros", SqlDbType.NVarChar)).Value = (object)model.Vehiculo?.otros?.ToUpper() ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@poliza", SqlDbType.NVarChar)).Value = (object)model.Vehiculo?.poliza?.ToUpper() ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@capacidad", SqlDbType.Int)).Value = (object)model.Vehiculo?.capacidad ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)model.Vehiculo?.idEntidad ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@idColor", SqlDbType.Int)).Value = (object)model.Vehiculo?.idColor ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@observacionesLugar", SqlDbType.NVarChar)).Value = (object)model.observacionesLugar?.ToUpper() ?? DBNull.Value;

						command.ExecuteNonQuery();

						if (model.IdTurno.HasValue)
						{
							UpsertTurnoAsignedToInfraccion(connection, trx, model.IdTurno.Value, model.idInfraccion);
						}
						else
						{
							RemoveTurnoAssignmentToInfraccion(connection, trx, model.idInfraccion);
						}

						//Crear historico vehiculo infraccion
						try
						{
							var bitacoraVehiculo = _vehiculosService.CrearHistoricoVehiculo(model.idInfraccion, (int)model.idVehiculo, 1);
							//CrearHistoricoPropietario((int)model.idPersona, model.idInfraccion);
						}
						catch { }

						trx.Commit();

						ModificarCoordenadasInfraccion(model);
						return model.idInfraccion;
					}
					catch (Exception ex)
					{
						trx.Rollback();
						if (ex is not SqlException sqlEx)
							throw;

						Logger.Debug("usp_UpdateInfraccionesVehiculos: " + ex.Message);
						return result;
					}
					finally
					{
						connection.Close();
					}
				}
			}
		}

		private void CrearHistoricoPropietario(int idPersona, int idInfraccion)
		{
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand("usp_InsertaInfraccionesPersonas", connection);
					command.CommandType = CommandType.StoredProcedure;

					// Agregar parámetros
					command.Parameters.Add(new SqlParameter("@TipoPersonaOrigen", SqlDbType.Int)).Value = 2;
					command.Parameters.Add(new SqlParameter("@idPersonaConductor", SqlDbType.Int)).Value = idPersona;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = idInfraccion;

					// Ejecutar el procedimiento almacenado
					command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{

				}
				finally
				{
					connection.Close();
				}
			}
		}

		public decimal GetMotivosCal(int idMotivosInfraccion)
		{
			decimal result = 0;
			string strQuery = @"SELECT top 1 format(calificacion,'#.##') calificacion
                               FROM motivosInfraccion
                               WHERE estatus = 1  and idMotivoInfraccion=@IdMotivo";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@IdMotivo", idMotivosInfraccion);

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							result = reader["calificacion"] == System.DBNull.Value ? default(decimal) : Convert.ToDecimal(reader["calificacion"].ToString());
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

			return result;
		}


		public int ModificarCoordenadasInfraccion(InfraccionesModel model)
		{
			int result = 0;
			string strQuery = @"UPDATE infracciones
                           SET
                               longitud = @longitud,
                               latitud = @latitud
                           WHERE idInfraccion = @idInfraccion";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					using (SqlCommand command = new SqlCommand(strQuery, connection))
					{
						command.CommandType = CommandType.Text;

						command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)model.idInfraccion;
						command.Parameters.Add(new SqlParameter("@longitud", SqlDbType.VarChar)).Value = (object)model.Longitud;
						command.Parameters.Add(new SqlParameter("@latitud", SqlDbType.VarChar)).Value = (object)model.Latitud;

						result = command.ExecuteNonQuery();
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
			return result;
		}


		public int ModificarInfraccionPorCortesia(InfraccionesModel model)
		{
			int result = 0;
			string strQuery = @"UPDATE infracciones
                                       SET                                          
                                           infraccionCortesia = @infraccionCortesia
                                          ,fechaActualizacion = getdate()
										  ,ObservacionsesApl = @ObservacionesApl
                                          WHERE idInfraccion = @idInfraccion";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)model.idInfraccion;
					command.Parameters.Add(new SqlParameter("@infraccionCortesia", SqlDbType.Int)).Value = model.cortesiaInt;
					//command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("@ObservacionesApl", SqlDbType.VarChar)).Value = (object)model.ObsevacionesApl;

					//command.Parameters.Add(new SqlParameter("@observacionesCortesia", SqlDbType.NVarChar)).Value = (object)model.observacionesCortesia ?? DBNull.Value;

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
		public int InsertarImagenEnInfraccion(string rutaInventario, int idInfraccion, string nombreArchivo = "GenericFile.txt")
		{
			int result = 0;
			string strQuery = @"UPDATE infracciones
                       SET archivoInventario =@inventario , nombreArchivo=@nameFile

                       WHERE idInfraccion = @idInfraccion";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = idInfraccion;
					command.Parameters.Add(new SqlParameter("@inventario", SqlDbType.VarChar)).Value = rutaInventario;
					command.Parameters.Add(new SqlParameter("@nameFile", SqlDbType.VarChar)).Value = nombreArchivo;

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
		public int GuardarReponse(CrearMultasTransitoChild MT_CrearMultasTransito_res, int idInfraccion)
		{
			int result = 0;
			string strQuery = @"UPDATE infracciones
                                SET partner = @partner,
                                        cuenta = @cuenta,
                                        objeto = @objeto,
                                        documento = @documento,
                                        idEstatusInfraccion =@idEstatusInfraccion,
                                        fechaActualizacion = getdate(),
                                        actualizadoPor = @actualizadoPor                             
                                WHERE idInfraccion = @idInfraccion";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.NVarChar)).Value = idInfraccion;
					command.Parameters.Add(new SqlParameter("@partner", SqlDbType.NVarChar)).Value = MT_CrearMultasTransito_res.BUSINESSPARTNER == null ? "" : MT_CrearMultasTransito_res.BUSINESSPARTNER;
					command.Parameters.Add(new SqlParameter("@cuenta", SqlDbType.NVarChar)).Value = MT_CrearMultasTransito_res.CUENTAnmbb == null ? "" : MT_CrearMultasTransito_res.CUENTAnmbb;
					command.Parameters.Add(new SqlParameter("@objeto", SqlDbType.NVarChar)).Value = MT_CrearMultasTransito_res.OBJETO == null ? "" : MT_CrearMultasTransito_res.OBJETO;
					command.Parameters.Add(new SqlParameter("@documento", SqlDbType.NVarChar)).Value = MT_CrearMultasTransito_res.DOCUMENTNUMBER == null ? "" : MT_CrearMultasTransito_res.DOCUMENTNUMBER;
					//command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("@idEstatusInfraccion", SqlDbType.Int)).Value = (object)7;
					result = command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}



		public int GuardarReponse2(CrearMultasTransitoChild MT_CrearMultasTransito_res, int idInfraccion)
		{

			var documento = MT_CrearMultasTransito_res.ZMESSAGE.Split("el");
			var document = documento[1].Substring(9, 12);
			int result = 0;
			string strQuery = @"UPDATE infracciones
                                SET 
                                        documento = @documento,
                                        idEstatusInfraccion =@idEstatusInfraccion,
                                        fechaActualizacion = getdate(),
                                        actualizadoPor = @actualizadoPor                             
                                WHERE idInfraccion = @idInfraccion";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.NVarChar)).Value = idInfraccion;
					command.Parameters.Add(new SqlParameter("@documento", SqlDbType.NVarChar)).Value = document;
					//command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("@idEstatusInfraccion", SqlDbType.Int)).Value = (object)7;
					result = command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
				}
				finally
				{
					connection.Close();
				}
			}
			return result;
		}



		public List<InfraccionesResumen> GetInfraccionesLicencia(string numLicencia, string CURP)
		{
			List<InfraccionesResumen> modelList = new List<InfraccionesResumen>();
			string strQuery = @"SELECT inf.idInfraccion
	                                ,piin.nombre+' '+ piin.apellidoPaterno +' '+ piin.apellidoMaterno conductor
	                                ,piin.numeroLicencia
	                                ,p.CURP 
	                                ,inf.folioInfraccion
	                                ,inf.fechaInfraccion 
	                                ,estIn.estatusInfraccion 
	                                ,catOfi.nombre + ' ' + catOfi.apellidoPaterno + ' ' + catOfi.apellidoMaterno nombreOficial
	                                ,catMun.municipio  
	                                ,col.color
	                                ,cmv.marcaVehiculo
	                                ,csv.nombreSubmarca
	                                ,veh.placas
	                                ,veh.modelo
	                                ,veh.serie
	                                ,veh.tarjeta
	                                ,veh.vigenciaTarjeta   
                                FROM infracciones as inf  
                                left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion   
                                left join catOficiales catOfi on inf.idOficial = catOfi.idOficial  
                                left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                left join catEntidades catEnt on  catMun.idEntidad = catEnt.idEntidad   
                                left join vehiculos veh on inf.idVehiculo = veh.idVehiculo   
                                left join catMarcasVehiculos cmv on veh.idMarcaVehiculo = cmv.idMarcaVehiculo 
                                left join catSubmarcasVehiculos csv on veh.idSubmarca  = csv.idSubmarca 
                                LEFT join catColores col on veh.idColor = col.idColor 
                                LEFT join opeInfraccionesPersonas piin ON inf.idPersonaConductor  = piin.idPersona AND inf.idInfraccion = piin.idInfraccion
                                LEFT JOIN personas p on piin.idPersona = p.idPersona 
                                WHERE inf.estatus = 1 and (piin.numeroLicencia =@numero_licencia OR p.CURP =@CURP)";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@numero_licencia", SqlDbType.VarChar)).Value = (object)numLicencia ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@CURP", SqlDbType.VarChar)).Value = (object)CURP ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							InfraccionesResumen model = new InfraccionesResumen();
							model.IdInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
							model.conductor = reader["conductor"].ToString();
							model.numeroLicencia = reader["numeroLicencia"].ToString();
							model.CURP = reader["CURP"].ToString();
							model.folioInfraccion = reader["folioInfraccion"].ToString();
							model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
							model.estatusInfraccion = reader["estatusInfraccion"].ToString();
							model.nombreOficial = reader["nombreOficial"].ToString();
							model.municipio = reader["municipio"].ToString();
							model.color = reader["color"].ToString();
							model.marcaVehiculo = reader["marcaVehiculo"].ToString();
							model.nombreSubmarca = reader["nombreSubmarca"].ToString();
							model.placas = reader["placas"].ToString();
							model.modelo = reader["modelo"].ToString();
							model.serie = reader["serie"].ToString();
							model.tarjeta = reader["tarjeta"].ToString();
							model.vigenciaTarjeta = reader["vigenciaTarjeta"].ToString();

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

		public int ModificarEstatusInfraccion(int idInfraccion, int idEstatusInfraccion)
		{
			int result = 0;
			string strQuery = @"UPDATE infracciones
                                       SET idEstatusInfraccion = @idEstatusInfraccion
                                       WHERE idInfraccion = @idInfraccion";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("idInfraccion", SqlDbType.Int)).Value = idInfraccion;
					command.Parameters.Add(new SqlParameter("idEstatusInfraccion", SqlDbType.Int)).Value = idEstatusInfraccion;
					command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;

					result = command.ExecuteNonQuery();
					if (result > 0) // Si la actualización tuvo éxito
					{
						return idInfraccion; // Retornar el idInfraccion
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
			}
			return result;
		}


		public List<InfraccionesModel> GetAllInfraccionesPagination(InfraccionesBusquedaModel model, int idOficina, int idDependenciaPerfil, Pagination pagination)
		{
			List<InfraccionesModel> InfraccionesList = new List<InfraccionesModel>();

			var count = GetCountRowsSearchInfracciones(model, idOficina, idDependenciaPerfil);


            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					using (SqlCommand cmd = new SqlCommand("usp_ObtieneTodasLasInfraccionesNEW", connection))
					//using (SqlCommand cmd = new SqlCommand("usp_ObtieneTodasLasInfraccionesNEWWhitView", connection))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@PageIndex", pagination.PageIndex);
						cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
						cmd.Parameters.AddWithValue("@idOficina", (idOficina == 0) ? DBNull.Value : idOficina);
						cmd.Parameters.AddWithValue("@idDependenciaPerfil", (object)idDependenciaPerfil ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdGarantia", (object)model.IdGarantia ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdTipoCortesia", (object)model.IdTipoCortesia ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdDelegacion", (object)model.IdDelegacion ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdEstatus", (object)model.IdEstatus ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdDependencia", (object)model.IdDependencia ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@numeroLicencia", (object)model.NumeroLicencia != null ? model.NumeroLicencia.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@numeroEconomico", (object)model.NumeroEconomico != null ? model.NumeroEconomico.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@FolioInfraccion", (object)model.folioInfraccion != null ? model.folioInfraccion.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Placas", (object)model.placas != null ? model.placas.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Serie", (object)model.serie != null ? model.serie.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Propietario", (object)model.Propietario != null ? model.Propietario.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Conductor", (object)model.Conductor != null ? model.Conductor.ToUpper() : DBNull.Value);
						cmd.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.Date)).Value = model.FechaInicio.Year <= 1900 ? DBNull.Value : model.FechaInicio;
						cmd.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.Date)).Value = model.FechaFin.Year <= 1900 ? DBNull.Value : model.FechaFin;
						cmd.CommandTimeout = 0;
						//NUEVOS PARAMETROS DE BUSQUEDA FASE 2 SPRINT 4
						cmd.Parameters.AddWithValue("@IdEntidad", (object)model.IdEntidadRegistro ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdMunicipio", (object)model.IdMunicipio ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdCarretera", (object)model.IdCarretera ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdTramo", (object)model.IdTramo ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdTipoVehiculo", (object)model.IdTipoVehiculo ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdTipoSevicio", (object)model.IdTipoServicio ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdSubtipoServicio", (object)model.IdSubtipoServicio ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdMarca", (object)model.IdMarca ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdSubmarca", (object)model.IdSubmarca ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdOficial", (object)model.IdOficial ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdTipoAplicacion", (object)model.IdAplicacion ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdTipoMotivo", (object)model.IdTipoMotivo ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@Modelo", (object)model.modelo != null ? model.modelo.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Kilometro", (object)model.kilometro != null ? model.kilometro.ToUpper() : DBNull.Value);

						using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
						{
							while (reader.Read())
							{
								InfraccionesModel infraccionModel = new InfraccionesModel();
								infraccionModel.transito = reader["transito"].GetType() == typeof(DBNull) ? null : (int)reader["transito"];
								infraccionModel.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
								infraccionModel.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
								infraccionModel.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
								infraccionModel.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
								infraccionModel.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
								infraccionModel.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
								infraccionModel.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
								infraccionModel.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
								infraccionModel.estatusInfraccion = reader["estatusInfraccion"].ToString();
								infraccionModel.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
								infraccionModel.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
								infraccionModel.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
								infraccionModel.idPersona = Convert.ToInt32(reader["idPersona"].ToString());
								infraccionModel.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"].ToString());
								infraccionModel.placasVehiculo = reader["placasVehiculo"] is DBNull ? "" : reader["placasVehiculo"].ToString();
								infraccionModel.folioInfraccion = reader["folioInfraccion"] is DBNull ? "" : reader["folioInfraccion"].ToString();
								infraccionModel.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
								infraccionModel.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
								infraccionModel.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
								infraccionModel.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
								infraccionModel.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
								infraccionModel.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
								infraccionModel.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
								infraccionModel.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"]);
								infraccionModel.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
								infraccionModel.aplicacion = reader["aplicacion"].ToString();
								infraccionModel.infraccionCortesiaString = reader["nombreCortesia"].ToString();
								infraccionModel.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"]);
								infraccionModel.emergenciasId = reader["emergenciasId"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["emergenciasId"]);
								infraccionModel.folioEmergencia = reader["folioEmergencia"] == System.DBNull.Value ? string.Empty : reader["folioEmergencia"].ToString();


								//infraccionModel.PersonaInfraccion = GetPersonaInfraccionById((int)infraccionModel.idInfraccion);
								if (infraccionModel.idPersonaConductor != null)
								{
									infraccionModel.PersonaInfraccion2 = _personasService.GetPersonaByIdHistorico((int)infraccionModel.idPersonaConductor, (int)infraccionModel.idInfraccion, 1);
								}
								else
								{
								}
								//if (infraccionModel.idPersona != null)
								//{
								//	infraccionModel.PersonaConductor2 = _personasService.GetPersonaById((int)infraccionModel.idPersona);
								//}
								//else
								//{
								//	infraccionModel.PersonaConductor2 = new PersonaModel();
								//}

								if (infraccionModel.idVehiculo != null)
								{
									infraccionModel.Vehiculo = _vehiculosService.GetVehiculoByIdHistorico((int)infraccionModel.idVehiculo, (int)infraccionModel.idInfraccion, 1);
								}
								else
								{
									infraccionModel.Vehiculo = new VehiculoModel();
								}


								//infraccionModel.Vehiculo = _vehiculosService.GetVehiculoHistoricoByIdAndIdinfraccion((int)infraccionModel.idVehiculo, (int)infraccionModel.idInfraccion);

								//infraccionModel.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(infraccionModel.idInfraccion);

								infraccionModel.Garantia = infraccionModel.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)infraccionModel.idInfraccion);
								infraccionModel.Garantia = infraccionModel.Garantia ?? new GarantiaInfraccionModel();
								infraccionModel.Garantia.garantia = infraccionModel.Garantia.garantia ?? "";
								infraccionModel.strIsPropietarioConductor = infraccionModel.Vehiculo == null ? "NO" : infraccionModel.Vehiculo.idPersona == infraccionModel.idPersona ? "SI" : "NO";
								infraccionModel.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

								//if (infraccionModel.PersonaInfraccion2 != null)
								//{
								//	infraccionModel.NombreConductor = infraccionModel.PersonaInfraccion2.nombreCompleto;
								//}
								//else
								//{
								//	infraccionModel.NombreConductor = null; // O cualquier otro valor predeterminado que desees
								//}
								if (infraccionModel.Vehiculo != null)
								{
									//infraccionModel.NombrePropietario = infraccionModel.Persona.nombreCompleto;

								}
								//model.idPropitario = model.Vehiculo.idPersona;

								if (infraccionModel.idPersona > 0)
								{
									infraccionModel.Persona = _personasService.GetPersonaByIdHistorico((int)infraccionModel.idPersona, (int)infraccionModel.idInfraccion, 2);
									infraccionModel.NombrePropietario = infraccionModel.Persona.nombreCompleto;
								}
								else
								{
									infraccionModel.NombrePropietario = infraccionModel.Vehiculo.propietario;
								}
								infraccionModel.NombreConductor = (infraccionModel.PersonaInfraccion2 == null) ? "" : infraccionModel.PersonaInfraccion2.nombreCompleto; // infraccionModel.PersonaConductor2 == null ? "" : infraccionModel.PersonaConductor2.nombreCompleto;  //infraccionModel.Vehiculo == null ? "" : infraccionModel.Vehiculo.Persona == null ? "" : infraccionModel.Vehiculo.Persona.nombreCompleto;
																																										 // infraccionModel.NombreGarantia = infraccionModel.garantia;
								infraccionModel.NombreGarantia = reader["garantia"] == System.DBNull.Value ? string.Empty : reader["garantia"].ToString();
								infraccionModel.Total = Convert.ToInt32(reader["Total"]);
								InfraccionesList.Add(infraccionModel);
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
				return InfraccionesList;
			}
		}
		public List<InfraccionesModel> GetAllInfraccionesPaginationInv(InfraccionesBusquedaModel model, int idOficina, int idDependenciaPerfil, Pagination pagination)
		{
			List<InfraccionesModel> InfraccionesList = new List<InfraccionesModel>();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					using (SqlCommand cmd = new SqlCommand("usp_ObtieneTodasLasInfraccionesInvisibles", connection))
					//using (SqlCommand cmd = new SqlCommand("usp_ObtieneTodasLasInfraccionesNEWWhitView", connection))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@PageIndex", pagination.PageIndex);
						cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
						cmd.Parameters.AddWithValue("@idOficina", (idOficina == 0) ? DBNull.Value : idOficina);
						cmd.Parameters.AddWithValue("@idDependenciaPerfil", (object)idDependenciaPerfil ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdGarantia", (object)model.IdGarantia ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdTipoCortesia", (object)model.IdTipoCortesia ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdDelegacion", (object)model.IdDelegacion ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdEstatus", (object)model.IdEstatus ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdDependencia", (object)model.IdDependencia ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@numeroLicencia", (object)model.NumeroLicencia != null ? model.NumeroLicencia.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@numeroEconomico", (object)model.NumeroEconomico != null ? model.NumeroEconomico.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@FolioInfraccion", (object)model.folioInfraccion != null ? model.folioInfraccion.ToUpper() : DBNull.Value);
						//cmd.Parameters.AddWithValue("@FolioEmergencia", (object)model.folioEmergencia != null ? model.folioEmergencia.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Placas", (object)model.placas != null ? model.placas.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Serie", (object)model.serie != null ? model.serie.ToUpper() : DBNull.Value);

						cmd.Parameters.AddWithValue("@Propietario", (object)model.Propietario != null ? model.Propietario.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Conductor", (object)model.Conductor != null ? model.Conductor.ToUpper() : DBNull.Value);

						cmd.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.Date)).Value = model.FechaInicio.Year <= 1900 ? DBNull.Value : model.FechaInicio;
						cmd.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.Date)).Value = model.FechaFin.Year <= 1900 ? DBNull.Value : model.FechaFin;
						cmd.CommandTimeout = 0;
						//NUEVOS PARAMETROS DE BUSQUEDA FASE 2 SPRINT 4
						cmd.Parameters.AddWithValue("@IdEntidad", (object)model.IdEntidadRegistro ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdMunicipio", (object)model.IdMunicipio ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdCarretera", (object)model.IdCarretera ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdTramo", (object)model.IdTramo ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdTipoVehiculo", (object)model.IdTipoVehiculo ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdTipoSevicio", (object)model.IdTipoServicio ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdSubtipoServicio", (object)model.IdSubtipoServicio ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdMarca", (object)model.IdMarca ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdSubmarca", (object)model.IdSubmarca ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdOficial", (object)model.IdOficial ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdTipoAplicacion", (object)model.IdAplicacion ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdTipoMotivo", (object)model.IdTipoMotivo ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@Modelo", (object)model.modelo != null ? model.modelo.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Kilometro", (object)model.kilometro != null ? model.kilometro.ToUpper() : DBNull.Value);

						using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
						{
							while (reader.Read())
							{
								InfraccionesModel infraccionModel = new InfraccionesModel();
								infraccionModel.transito = reader["transito"].GetType() == typeof(DBNull) ? null : (int)reader["transito"];
								infraccionModel.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
								infraccionModel.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
								infraccionModel.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
								infraccionModel.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
								infraccionModel.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
								infraccionModel.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
								infraccionModel.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
								infraccionModel.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
								infraccionModel.estatusInfraccion = reader["estatusInfraccion"].ToString();
								infraccionModel.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
								infraccionModel.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
								infraccionModel.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
								infraccionModel.idPersona = Convert.ToInt32(reader["idPersona"].ToString());
								infraccionModel.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"].ToString());
								infraccionModel.placasVehiculo = reader["placasVehiculo"] is DBNull ? "" : reader["placasVehiculo"].ToString();
								infraccionModel.folioInfraccion = reader["folioInfraccion"] is DBNull ? "" : reader["folioInfraccion"].ToString();
								infraccionModel.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
								infraccionModel.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
								infraccionModel.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
								infraccionModel.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
								infraccionModel.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
								infraccionModel.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
								infraccionModel.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
								infraccionModel.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"]);
								infraccionModel.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
								infraccionModel.aplicacion = reader["aplicacion"].ToString();
								infraccionModel.infraccionCortesiaString = reader["nombreCortesia"].ToString();
								infraccionModel.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"]);
								infraccionModel.emergenciasId = reader["emergenciasId"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["emergenciasId"]);
								infraccionModel.folioEmergencia = reader["folioEmergencia"] == System.DBNull.Value ? string.Empty : reader["folioEmergencia"].ToString();


								//infraccionModel.PersonaInfraccion = GetPersonaInfraccionById((int)infraccionModel.idInfraccion);
								if (infraccionModel.idPersonaConductor != null)
								{
									infraccionModel.PersonaInfraccion2 = _personasService.GetPersonaByIdHistorico((int)infraccionModel.idPersonaConductor, (int)infraccionModel.idInfraccion, 1);
								}
								else
								{
								}
								//if (infraccionModel.idPersona != null)
								//{
								//	infraccionModel.PersonaConductor2 = _personasService.GetPersonaById((int)infraccionModel.idPersona);
								//}
								//else
								//{
								//	infraccionModel.PersonaConductor2 = new PersonaModel();
								//}

								if (infraccionModel.idVehiculo != null)
								{
									infraccionModel.Vehiculo = _vehiculosService.GetVehiculoByIdHistorico((int)infraccionModel.idVehiculo, (int)infraccionModel.idInfraccion, 1);
								}
								else
								{
									infraccionModel.Vehiculo = new VehiculoModel();
								}


								//infraccionModel.Vehiculo = _vehiculosService.GetVehiculoHistoricoByIdAndIdinfraccion((int)infraccionModel.idVehiculo, (int)infraccionModel.idInfraccion);

								//infraccionModel.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(infraccionModel.idInfraccion);

								infraccionModel.Garantia = infraccionModel.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)infraccionModel.idInfraccion);
								infraccionModel.Garantia = infraccionModel.Garantia ?? new GarantiaInfraccionModel();
								infraccionModel.Garantia.garantia = infraccionModel.Garantia.garantia ?? "";
								infraccionModel.strIsPropietarioConductor = infraccionModel.Vehiculo == null ? "NO" : infraccionModel.Vehiculo.idPersona == infraccionModel.idPersona ? "SI" : "NO";
								infraccionModel.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

								//if (infraccionModel.PersonaInfraccion2 != null)
								//{
								//	infraccionModel.NombreConductor = infraccionModel.PersonaInfraccion2.nombreCompleto;
								//}
								//else
								//{
								//	infraccionModel.NombreConductor = null; // O cualquier otro valor predeterminado que desees
								//}
								if (infraccionModel.Vehiculo != null)
								{
									//infraccionModel.NombrePropietario = infraccionModel.Persona.nombreCompleto;

								}
								//model.idPropitario = model.Vehiculo.idPersona;

								if (infraccionModel.idPersona > 0)
								{
									infraccionModel.Persona = _personasService.GetPersonaByIdHistorico((int)infraccionModel.idPersona, (int)infraccionModel.idInfraccion, 2);
									infraccionModel.NombrePropietario = infraccionModel.Persona.nombreCompleto;
								}
								else
								{
									infraccionModel.NombrePropietario = infraccionModel.Vehiculo.propietario;
								}
								infraccionModel.NombreConductor = (infraccionModel.PersonaInfraccion2 == null) ? "" : infraccionModel.PersonaInfraccion2.nombreCompleto; // infraccionModel.PersonaConductor2 == null ? "" : infraccionModel.PersonaConductor2.nombreCompleto;  //infraccionModel.Vehiculo == null ? "" : infraccionModel.Vehiculo.Persona == null ? "" : infraccionModel.Vehiculo.Persona.nombreCompleto;
																																										 // infraccionModel.NombreGarantia = infraccionModel.garantia;
								infraccionModel.NombreGarantia = reader["garantia"] == System.DBNull.Value ? string.Empty : reader["garantia"].ToString();
								infraccionModel.Total = Convert.ToInt32(reader["Total"]);
								InfraccionesList.Add(infraccionModel);
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
				return InfraccionesList;
			}
		}




		void CreateConsultaGetSearchInfracciones(int count,List<InfraccionesModel> InfraccionesList, InfraccionesBusquedaModel model, int idOficina, int idDependenciaPerfil, Pagination pagination)
		{
			var query = $@"SELECT * FROM	(	SELECT	ROW_NUMBER() OVER ( ORDER BY inf.idInfraccion ASC ) AS rowIndex
						,inf.idInfraccion AS idInfraccion
						,inf.idOficial AS idOficial
						,inf.idDependencia AS idDependencia
						,inf.idDelegacion AS idDelegacion
						,inf.idVehiculo  AS idVehiculo
						,inf.idAplicacion AS idAplicacion
						,inf.idGarantia AS idGarantia
						,inf.idEstatusInfraccion AS idEstatusInfraccion
						,inf.idMunicipio AS idMunicipio
						,inf.idTramo AS idTramo
						,inf.idCarretera AS idCarretera
					    ,ISNULL(inf.idPersona,0) AS idPersonaDEINFRA
						,ISNULL(inf.idPersona,0) AS idPersona
						,ISNULL(inf.idPersonaInfraccion,0) AS idPersonaConductor
						--,inf.placasVehiculo AS placasVehiculo
						,veh.placas AS placasVehiculo 
						,inf.folioInfraccion AS folioInfraccion
						,inf.fechaInfraccion AS fechaInfraccion
						,inf.kmCarretera AS kmCarretera
						,inf.observaciones AS observaciones
						,inf.lugarCalle AS lugarCalle
						,inf.lugarNumero AS lugarNumero
						,inf.lugarColonia AS lugarColonia
						,inf.lugarEntreCalle AS lugarEntreCalle
						,inf.infraccionCortesia
						,inf.transito
	                    ,ctc.nombreCortesia AS nombreCortesia
						,inf.NumTarjetaCirculacion AS NumTarjetaCirculacion
						,inf.fechaActualizacion AS fechaActualizacion
						,inf.actualizadoPor AS actualizadoPor
						,inf.estatus AS estatus
						,del.idOficinaTransporte AS idOficinaTransporte 
						,del.nombreOficina AS nombreOficina
						,dep.nombreDependencia AS max_nombreDependencia
						,catGar.garantia AS garantia
						,estIn.estatusInfraccion AS estatusInfraccion
						,gar.numPlaca AS numPlaca
						,per.numeroLicencia AS numLicencia
						,gar.vehiculoDocumento AS vehiculoDocumento
						,tipoP.idTipoPlaca AS idTipoPlaca
						,tipoP.tipoPlaca AS tipoPlaca
						,tipoL.idTipoLicencia AS idTipoLicencia
						,tipoL.tipoLicencia AS tipoLicencia
						,catOfi.rango AS rango
						,catMun.municipio AS municipio
						,catTra.tramo AS tramo
						,catCarre.carretera AS carretera
						,veh.idMarcaVehiculo AS idMarcaVehiculo
						,veh.serie AS serie
						,veh.tarjeta AS tarjeta
						,veh.vigenciaTarjeta AS vigenciaTarjeta
						,veh.idTipoVehiculo AS idTipoVehiculo
						,veh.modelo AS modelo
						,veh.idColor AS idColor
						,veh.idEntidad AS idEntidad
						,veh.idCatTipoServicio AS idCatTipoServicio
						,veh.propietario AS propietario
						,veh.numeroEconomico AS numeroEconomico
						,per.nombre AS nombre
						,per.apellidoPaterno AS apellidoPaterno
						,per.apellidoMaterno AS apellidoMaterno
						,ca.aplicacion AS aplicacion
						,per.numeroLicencia
						,infEm.emergenciasId
						,infEm.folioEmergencia
						,{count} Total
	FROM infracciones							inf
	LEFT JOIN catDependencias					dep			WITH (NOLOCK) ON inf.idDependencia= dep.idDependencia
	LEFT JOIN catDelegacionesOficinasTransporte	del			WITH (NOLOCK) ON inf.idDelegacion = del.idOficinaTransporte
	LEFT JOIN catEstatusInfraccion				estIn		WITH (NOLOCK) ON inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
	LEFT JOIN catGarantias						catGar		WITH (NOLOCK) ON inf.idGarantia = catGar.idGarantia
	LEFT JOIN garantiasInfraccion				gar			WITH (NOLOCK) ON catGar.idGarantia= gar.idCatGarantia and gar.idInfraccion = inf.idinfraccion
	LEFT JOIN catTipoPlaca						tipoP		WITH (NOLOCK) ON gar.idTipoPlaca=tipoP.idTipoPlaca
	LEFT JOIN catTipoLicencia					tipoL		WITH (NOLOCK) ON tipoL.idTipoLicencia= gar.idTipoLicencia
	LEFT JOIN catOficiales						catOfi		WITH (NOLOCK) ON inf.idOficial = catOfi.idOficial
	LEFT JOIN catMunicipios						catMun		WITH (NOLOCK) ON inf.idMunicipio =catMun.idMunicipio
	LEFT JOIN catTramos							catTra		WITH (NOLOCK) ON inf.idTramo = catTra.idTramo
	LEFT JOIN infraccionesEmergencias			infEm	WITH (NOLOCK) ON inf.idInfraccion = infEm.idInfraccion																								 
	LEFT JOIN catCarreteras						catCarre	WITH (NOLOCK) ON catTra.IdCarretera = catCarre.idCarretera
	LEFT JOIN opeInfraccionesVehiculos			veh		WITH (NOLOCK) ON   veh.idOperacion = (select max(auxveh.idOperacion) from opeInfraccionesVehiculos auxveh 
																																		  where inf.idVehiculo = auxveh.idVehiculo and inf.idInfraccion =auxveh.idInfraccion )  
	LEFT JOIN opeInfraccionesPersonas			per			WITH (NOLOCK) ON per.idOperacion = 
												(
												SELECT MAX(idOperacion) 
												FROM opeInfraccionesPersonas z 
												WHERE z.idPersona = inf.idPersona and z.TipoPersonaOrigen=2 and z.idInfraccion =inf.idInfraccion 
												)

    LEFT JOIN opeInfraccionesPersonas			pinf			WITH (NOLOCK) ON pinf.idOperacion = 
												(
												SELECT MAX(idOperacion) 
												FROM opeInfraccionesPersonas z 
												WHERE z.idPersona = inf.idPersonaInfraccion and z.idInfraccion =inf.idInfraccion and z.TipoPersonaOrigen=1
												)

	--LEFT JOIN opeInfraccionesPersonas  pInf ON pinf.TipoPersonaOrigen = 2 and inf.idInfraccion = pInf.idInfraccion AND pInf.idPersona = inf.idPersona 
	LEFT JOIN catAplicacionInfraccion			ca			WITH (NOLOCK) ON ca.idAplicacion = inf.idAplicacion
    LEFT JOIN catTipoCortesia                   ctc         WITH (NOLOCK) ON ctc.id = inf.infraccionCortesia

	WHERE	inf.estatus=1 
			{createQueryAddSearchInfracciones(model, idOficina, idDependenciaPerfil)}
			
	) Persons ORDER BY rowIndex DESC	
	OFFSET @PageIndex * @PageSize  ROWS FETCH NEXT @PageSize ROWS ONLY;";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					using (SqlCommand cmd = new SqlCommand(query, connection))
					//using (SqlCommand cmd = new SqlCommand("usp_ObtieneTodasLasInfraccionesNEWWhitView", connection))
					{
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.AddWithValue("@PageIndex", pagination.PageIndex);
						cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                        AddParametersCountSearchInfracciones(model, idOficina, idDependenciaPerfil, cmd);

                        using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
						{
							while (reader.Read())
							{
								InfraccionesModel infraccionModel = new InfraccionesModel();
								infraccionModel.transito = reader["transito"].GetType() == typeof(DBNull) ? null : (int)reader["transito"];
								infraccionModel.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
								infraccionModel.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
								infraccionModel.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
								infraccionModel.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
								infraccionModel.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
								infraccionModel.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
								infraccionModel.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
								infraccionModel.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
								infraccionModel.estatusInfraccion = reader["estatusInfraccion"].ToString();
								infraccionModel.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
								infraccionModel.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
								infraccionModel.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
								infraccionModel.idPersona = Convert.ToInt32(reader["idPersona"].ToString());
								infraccionModel.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"].ToString());
								infraccionModel.placasVehiculo = reader["placasVehiculo"] is DBNull ? "" : reader["placasVehiculo"].ToString();
								infraccionModel.folioInfraccion = reader["folioInfraccion"] is DBNull ? "" : reader["folioInfraccion"].ToString();
								infraccionModel.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
								infraccionModel.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
								infraccionModel.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
								infraccionModel.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
								infraccionModel.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
								infraccionModel.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
								infraccionModel.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
								infraccionModel.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"]);
								infraccionModel.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
								infraccionModel.aplicacion = reader["aplicacion"].ToString();
								infraccionModel.infraccionCortesiaString = reader["nombreCortesia"].ToString();
								infraccionModel.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"]);
								infraccionModel.emergenciasId = reader["emergenciasId"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["emergenciasId"]);
								infraccionModel.folioEmergencia = reader["folioEmergencia"] == System.DBNull.Value ? string.Empty : reader["folioEmergencia"].ToString();


								//infraccionModel.PersonaInfraccion = GetPersonaInfraccionById((int)infraccionModel.idInfraccion);
								if (infraccionModel.idPersonaConductor != null)
								{
									infraccionModel.PersonaInfraccion2 = _personasService.GetPersonaByIdHistorico((int)infraccionModel.idPersonaConductor, (int)infraccionModel.idInfraccion, 1);
								}
								else
								{
								}
								//if (infraccionModel.idPersona != null)
								//{
								//	infraccionModel.PersonaConductor2 = _personasService.GetPersonaById((int)infraccionModel.idPersona);
								//}
								//else
								//{
								//	infraccionModel.PersonaConductor2 = new PersonaModel();
								//}

								if (infraccionModel.idVehiculo != null)
								{
									infraccionModel.Vehiculo = _vehiculosService.GetVehiculoByIdHistorico((int)infraccionModel.idVehiculo, (int)infraccionModel.idInfraccion, 1);
								}
								else
								{
									infraccionModel.Vehiculo = new VehiculoModel();
								}


								//infraccionModel.Vehiculo = _vehiculosService.GetVehiculoHistoricoByIdAndIdinfraccion((int)infraccionModel.idVehiculo, (int)infraccionModel.idInfraccion);

								//infraccionModel.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(infraccionModel.idInfraccion);

								infraccionModel.Garantia = infraccionModel.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)infraccionModel.idInfraccion);
								infraccionModel.Garantia = infraccionModel.Garantia ?? new GarantiaInfraccionModel();
								infraccionModel.Garantia.garantia = infraccionModel.Garantia.garantia ?? "";
								infraccionModel.strIsPropietarioConductor = infraccionModel.Vehiculo == null ? "NO" : infraccionModel.Vehiculo.idPersona == infraccionModel.idPersona ? "SI" : "NO";
								infraccionModel.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

								//if (infraccionModel.PersonaInfraccion2 != null)
								//{
								//	infraccionModel.NombreConductor = infraccionModel.PersonaInfraccion2.nombreCompleto;
								//}
								//else
								//{
								//	infraccionModel.NombreConductor = null; // O cualquier otro valor predeterminado que desees
								//}
								if (infraccionModel.Vehiculo != null)
								{
									//infraccionModel.NombrePropietario = infraccionModel.Persona.nombreCompleto;

								}
								//model.idPropitario = model.Vehiculo.idPersona;

								if (infraccionModel.idPersona > 0)
								{
									infraccionModel.Persona = _personasService.GetPersonaByIdHistorico((int)infraccionModel.idPersona, (int)infraccionModel.idInfraccion, 2);
									infraccionModel.NombrePropietario = infraccionModel.Persona.nombreCompleto;
								}
								else
								{
									infraccionModel.NombrePropietario = infraccionModel.Vehiculo.propietario;
								}
								infraccionModel.NombreConductor = (infraccionModel.PersonaInfraccion2 == null) ? "" : infraccionModel.PersonaInfraccion2.nombreCompleto; // infraccionModel.PersonaConductor2 == null ? "" : infraccionModel.PersonaConductor2.nombreCompleto;  //infraccionModel.Vehiculo == null ? "" : infraccionModel.Vehiculo.Persona == null ? "" : infraccionModel.Vehiculo.Persona.nombreCompleto;
																																										 // infraccionModel.NombreGarantia = infraccionModel.garantia;
								infraccionModel.NombreGarantia = reader["garantia"] == System.DBNull.Value ? string.Empty : reader["garantia"].ToString();
								infraccionModel.Total = Convert.ToInt32(reader["Total"]);
								InfraccionesList.Add(infraccionModel);
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
		}

        public List<InfraccionesModel> GetAllInfraccionesPaginationWhitcode(InfraccionesBusquedaModel model, int idOficina, int idDependenciaPerfil, Pagination pagination)
        {
            List<InfraccionesModel> InfraccionesList = new List<InfraccionesModel>();

            var count = GetCountRowsSearchInfracciones(model, idOficina, idDependenciaPerfil);

			CreateConsultaGetSearchInfracciones(count, InfraccionesList, model, idOficina, idDependenciaPerfil, pagination);

                return InfraccionesList;
            
        }


        string createQueryAddSearchInfracciones(InfraccionesBusquedaModel model, int idOficina, int idDependenciaPerfil)
        {
            var where = "";

		    if (model.FechaInicio.Year > 1900 && model.FechaFin.Year > 1900)
			{
				where = @$"{where} AND CONVERT(VARCHAR,inf.fechaInfraccion,112) 
																	BETWEEN 
																	CONVERT(VARCHAR,@FechaInicio,112) 
																	AND  
																	CONVERT(VARCHAR,@FechaFin,112) ";
			}else if(model.FechaInicio.Year <= 1900 && model.FechaFin.Year > 1900)
			{
                where = @$"{where} AND CONVERT(VARCHAR,inf.fechaInfraccion,112) < CONVERT(VARCHAR,@FechaFin,112) ";

            }
			else
			{
                where = @$"{where} AND CONVERT(VARCHAR,inf.fechaInfraccion,112) > CONVERT(VARCHAR,@FechaInicio,112) ";
            }

			if (model.IdGarantia.HasValue && model.IdGarantia>0)
			{
				where = $"{where} AND inf.idGarantia = @IdGarantia  ";
			}

			if (model.IdTipoCortesia.HasValue && model.IdTipoCortesia > 0)
			{
                where = $"{where} AND inf.infraccionCortesia = @IdTipoCortesia";
            }
			if (model.IdDelegacion.HasValue && model.IdDelegacion > 0)
			{
                where = $"{where} AND del.idOficinaTransporte = @IdDelegacion";
            }
			if (model.IdEstatus.HasValue && model.IdEstatus > 0)
			{
                where = $"{where} AND estIn.idEstatusInfraccion\t= @IdEstatus";
            }
			if ( idDependenciaPerfil > -1)
			{
                where = $"{where} AND inf.transito\t\t\t\t= @idDependenciaPerfil";
            }

            if (!string.IsNullOrEmpty(model.NumeroLicencia))
            {
                where = $"{where} AND (per.numeroLicencia\t\t= @numeroLicencia OR pInf.numeroLicencia = @numeroLicencia)";
            }
            if (!string.IsNullOrEmpty(model.NumeroEconomico))
            {
                where = $"{where} AND veh.numeroEconomico\t\t= @numeroEconomico";
            }

            if (!string.IsNullOrEmpty(model.folioInfraccion))
            {
                where = $"{where} AND (UPPER(inf.folioInfraccion) LIKE  '%' + @FolioInfraccion + '%')";
            }
            if (!string.IsNullOrEmpty(model.folioEmergencia))
            {
                where = $"{where} AND (infEm.folioEmergencia LIKE  '%' + @FolioEmergencia + '%')";
            }
            if (!string.IsNullOrEmpty(model.placas))
            {
                where = $"{where} AND UPPER(CONVERT(VARCHAR(500),veh.placas))\t= @Placas";
            }
            if (!string.IsNullOrEmpty(model.serie))
            {
                where = $"{where} AND UPPER(CONVERT(VARCHAR(500),veh.serie))\t= @Serie";
            }
            if (!string.IsNullOrEmpty(model.Propietario))
            {
                where = $"{where} AND UPPER(veh.propietario) COLLATE Latin1_general_CI_AI LIKE '%' + @Propietario + '%'";
            }
            if (!string.IsNullOrEmpty(model.Conductor))
            {
                where = $"{where} AND UPPER(CONCAT(pInf.nombre , ' ' , pInf.apellidoPaterno, ' ' , pInf.apellidoMaterno)) COLLATE Latin1_general_CI_AI LIKE '%' + @Conductor + '%'";
            }           

            return where;
		}

		void AddParametersCountSearchInfracciones(InfraccionesBusquedaModel model, int idOficina, int idDependenciaPerfil, SqlCommand cmd)
		{

            if (model.FechaInicio.Year > 1900 && model.FechaFin.Year > 1900)
            {
                cmd.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.Date)).Value =  model.FechaInicio;
                cmd.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.Date)).Value = model.FechaFin;


            }
            else if (model.FechaInicio.Year <= 1900 && model.FechaFin.Year > 1900)
            {
                cmd.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.Date)).Value = model.FechaFin;
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.Date)).Value = model.FechaInicio;
            }

            if (model.IdGarantia.HasValue && model.IdGarantia > 0)
            {
                cmd.Parameters.AddWithValue("@IdGarantia", (object)model.IdGarantia );
            }

            if (model.IdTipoCortesia.HasValue && model.IdTipoCortesia > 0)
            {
                cmd.Parameters.AddWithValue("@IdTipoCortesia", (object)model.IdTipoCortesia );
            }
            if (model.IdDelegacion.HasValue && model.IdDelegacion > 0)
            {
                cmd.Parameters.AddWithValue("@IdDelegacion", (object)model.IdDelegacion );
            }
            if (model.IdEstatus.HasValue && model.IdEstatus > 0)
            {
                cmd.Parameters.AddWithValue("@IdEstatus", (object)model.IdEstatus );
            }
            if (idDependenciaPerfil > -1)
            {
                cmd.Parameters.AddWithValue("@idDependenciaPerfil", (object)idDependenciaPerfil );
            }

            if (!string.IsNullOrEmpty(model.NumeroLicencia))
            {
                cmd.Parameters.AddWithValue("@numeroLicencia",  model.NumeroLicencia.ToUpper() );
            }
            if (!string.IsNullOrEmpty(model.NumeroEconomico))
            {
                cmd.Parameters.AddWithValue("@numeroEconomico",  model.NumeroEconomico.ToUpper() );
            }

            if (!string.IsNullOrEmpty(model.folioInfraccion))
            {
                cmd.Parameters.AddWithValue("@FolioInfraccion",  model.folioInfraccion.ToUpper() );
            }
            if (!string.IsNullOrEmpty(model.folioEmergencia))
            {
                cmd.Parameters.AddWithValue("@FolioEmergencia",  model.folioEmergencia );
            }
            if (!string.IsNullOrEmpty(model.placas))
            {
                cmd.Parameters.AddWithValue("@Placas",  model.placas.ToUpper() );
            }
            if (!string.IsNullOrEmpty(model.serie))
            {
                cmd.Parameters.AddWithValue("@Serie",  model.serie.ToUpper() );
            }
            if (!string.IsNullOrEmpty(model.Propietario))
            {
                cmd.Parameters.AddWithValue("@Propietario",  model.Propietario.ToUpper() );

            }
            if (!string.IsNullOrEmpty(model.Conductor))
            {
                cmd.Parameters.AddWithValue("@Conductor", model.Conductor.ToUpper() );

            }

        }

		int GetCountRowsSearchInfracciones(InfraccionesBusquedaModel model, int idOficina, int idDependenciaPerfil)
		{
			var result = 0;
			var query = $@"SELECT COUNT(*) count
												FROM infracciones							inf
												LEFT JOIN catDependencias					dep		WITH (NOLOCK) ON inf.idDependencia= dep.idDependencia
												LEFT JOIN catDelegacionesOficinasTransporte	del		WITH (NOLOCK) ON inf.idDelegacion = del.idOficinaTransporte
												LEFT JOIN catEstatusInfraccion				estIn	WITH (NOLOCK) ON inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
												LEFT JOIN catGarantias						catGar	WITH (NOLOCK) ON inf.idGarantia = catGar.idGarantia
												LEFT JOIN garantiasInfraccion				gar		WITH (NOLOCK) ON catGar.idGarantia= gar.idCatGarantia and gar.idInfraccion = inf.idinfraccion
												LEFT JOIN catTipoPlaca						tipoP	WITH (NOLOCK) ON gar.idTipoPlaca=tipoP.idTipoPlaca
												LEFT JOIN catTipoLicencia					tipoL	WITH (NOLOCK) ON tipoL.idTipoLicencia= gar.idTipoLicencia
												LEFT JOIN catOficiales						catOfi	WITH (NOLOCK) ON inf.idOficial = catOfi.idOficial
												LEFT JOIN catMunicipios						catMun	WITH (NOLOCK) ON inf.idMunicipio =catMun.idMunicipio
												LEFT JOIN catTramos							catTra	WITH (NOLOCK) ON inf.idTramo = catTra.idTramo
												LEFT JOIN infraccionesEmergencias			infEm	WITH (NOLOCK) ON inf.idInfraccion = infEm.idInfraccion																																								  
												LEFT JOIN catCarreteras						catCarre WITH (NOLOCK) ON catTra.IdCarretera = catCarre.idCarretera
												LEFT JOIN opeInfraccionesVehiculos			veh		WITH (NOLOCK) ON   veh.idOperacion = (select max(auxveh.idOperacion) from opeInfraccionesVehiculos auxveh 
																																		  where inf.idVehiculo = auxveh.idVehiculo and inf.idInfraccion =auxveh.idInfraccion )  
												LEFT JOIN opeInfraccionesPersonas			per			WITH (NOLOCK) ON per.idOperacion = 
												--testvehiculoinfraccion
												(
												SELECT MAX(idOperacion) 
												FROM opeInfraccionesPersonas z 
												WHERE z.idPersona = inf.idPersonaInfraccion and z.idInfraccion =inf.idInfraccion
												)

												LEFT JOIN opeInfraccionesPersonas			pinf			WITH (NOLOCK) ON pinf.idOperacion = 
												(
												SELECT MAX(idOperacion) 
												FROM opeInfraccionesPersonas z 
												WHERE z.idPersona = inf.idPersona and z.idInfraccion =inf.idInfraccion 
												)
												--LEFT JOIN opeInfraccionesPersonas			pInf	WITH (NOLOCK) ON inf.idInfraccion = pInf.idInfraccion AND pInf.idPersona = inf.idPersonaConductor
												LEFT JOIN catAplicacionInfraccion			ca		WITH (NOLOCK) ON ca.idAplicacion = inf.idAplicacion
												WHERE inf.estatus=1 {createQueryAddSearchInfracciones(model,idOficina,idDependenciaPerfil)}";
		
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					using (SqlCommand cmd = new SqlCommand(query, connection))
					{
						cmd.CommandType = CommandType.Text;

						AddParametersCountSearchInfracciones(model, idOficina, idDependenciaPerfil, cmd);

						using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
						{
							while (reader.Read())
							{
								result = int.Parse(reader["count"].ToString());
							}
						}
					}
				}
				catch (Exception e)
				{

				}
				finally
				{
                    connection.Close();
                }
			}


                    return result;
		}




		public List<InfraccionesModel> GetAllInfraccionesBusquedaEspecialPagination(InfraccionesBusquedaEspecialModel model, int idOficina, int idDependenciaPerfil, Pagination pagination)
		{
			List<InfraccionesModel> InfraccionesList = new List<InfraccionesModel>();
			DateTime? fechasIni = string.IsNullOrEmpty(model.FechaInicio) ? null : DateTime.ParseExact(model.FechaInicio, "d/M/yyyy", CultureInfo.InvariantCulture);
			DateTime? fechasFin = string.IsNullOrEmpty(model.FechaFin) ? null : DateTime.ParseExact(model.FechaFin, "d/M/yyyy", CultureInfo.InvariantCulture);

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					using (SqlCommand cmd = new SqlCommand("[usp_ObtieneTodasLasInfraccionesEspeciales]", connection))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@PageIndex", pagination.PageIndex);
						cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);

						cmd.Parameters.AddWithValue("@idOficina", (idOficina == 0) ? DBNull.Value : idOficina);
						cmd.Parameters.AddWithValue("@idDependenciaPerfil", (object)idDependenciaPerfil ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdDelegacion", (object)model.oficinas ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@IdEstatus", (object)model.estatus ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@numeroLicencia", (object)model.noLicencia != null ? model.noLicencia.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@numeroEconomico", (object)model.noEconomico != null ? model.noEconomico.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@FolioInfraccion", (object)model.folio != null ? model.folio.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Placas", (object)model.placas != null ? model.placas.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Serie", (object)model.serie != null ? model.serie.ToUpper() : DBNull.Value);

						cmd.Parameters.AddWithValue("@Propietario", (object)model.propietario != null ? model.propietario.ToUpper() : DBNull.Value);
						cmd.Parameters.AddWithValue("@Conductor", (object)model.conductor != null ? model.conductor.ToUpper() : DBNull.Value);
						//cmd.Parameters.AddWithValue("@FechaInicio", (fechasIni==null) ? DBNull.Value : fechasIni);
						//cmd.Parameters.AddWithValue("@FechaFin", (fechasFin==null) ? DBNull.Value : fechasFin);
						cmd.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.Date)).Value = model.FechaInicio;
						cmd.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.Date)).Value = model.FechaFin;
						using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
						{
							while (reader.Read())
							{
								InfraccionesModel infraccionModel = new InfraccionesModel();
								infraccionModel.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
								infraccionModel.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
								infraccionModel.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
								infraccionModel.idDelegacion = reader["idOficinaTransporte"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficinaTransporte"].ToString());
								infraccionModel.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
								infraccionModel.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
								infraccionModel.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
								infraccionModel.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
								infraccionModel.estatusInfraccion = reader["estatusInfraccion"].ToString();
								infraccionModel.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
								infraccionModel.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
								infraccionModel.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
								infraccionModel.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
								infraccionModel.idPersonaConductor = Convert.ToInt32(reader["idPersonaConductor"].ToString());
								infraccionModel.placasVehiculo = reader["placasVehiculo"].ToString();
								infraccionModel.folioInfraccion = reader["folioInfraccion"].ToString();
								infraccionModel.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
								infraccionModel.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
								infraccionModel.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
								infraccionModel.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
								infraccionModel.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
								infraccionModel.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
								infraccionModel.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
								//infraccionModel.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
								infraccionModel.infraccionCortesiaString = reader["nombreCortesia"] == System.DBNull.Value
								 ? default(string)
								 : reader["nombreCortesia"].ToString();
								infraccionModel.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
								if (infraccionModel.idPersona != null)
									infraccionModel.Persona = _personasService.GetPersonaByIdInfraccion((int)infraccionModel.idPersona, infraccionModel.idInfraccion);
								infraccionModel.PersonaInfraccion2 = _personasService.GetPersonaByIdInfraccion((int)infraccionModel.idPersonaConductor, infraccionModel.idInfraccion);
								infraccionModel.PersonaInfraccion = GetPersonaInfraccionById((int)infraccionModel.idInfraccion);
								infraccionModel.Vehiculo = _vehiculosService.GetVehiculoById((int)infraccionModel.idVehiculo);
								//infraccionModel.MotivosInfraccion = GetMotivosInfraccionByIdInfraccion(infraccionModel.idInfraccion);

								infraccionModel.Garantia = infraccionModel.idGarantia == null ? new GarantiaInfraccionModel() : GetGarantiaById((int)infraccionModel.idInfraccion);
								infraccionModel.strIsPropietarioConductor = infraccionModel.Vehiculo == null ? "NO" : infraccionModel.Vehiculo.idPersona == infraccionModel.idPersona ? "SI" : "NO";
								infraccionModel.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();

								if (infraccionModel.PersonaInfraccion != null)
								{
									infraccionModel.NombreConductor = infraccionModel.PersonaInfraccion.nombreCompleto;
								}
								else
								{
									infraccionModel.NombreConductor = null; // O cualquier otro valor predeterminado que desees
								}
								infraccionModel.NombrePropietario = infraccionModel.Vehiculo == null ? "" : infraccionModel.Vehiculo.Persona == null ? "" : infraccionModel.Vehiculo.Persona.nombreCompleto;
								// infraccionModel.NombreGarantia = infraccionModel.garantia;
								infraccionModel.NombreGarantia = reader["garantia"] == System.DBNull.Value ? string.Empty : reader["garantia"].ToString();
								infraccionModel.Total = Convert.ToInt32(reader["Total"]);

								InfraccionesList.Add(infraccionModel);

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
			return InfraccionesList;
		}
		public int ActualizarEstatusCortesia(int idInfraccion, int cortesiaInt, string observaciones)
		{
			var result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string updateQuery = "UPDATE infracciones SET infraccionCortesia = @cortesiaInt ,ObservacionsesApl=@obs WHERE idInfraccion = @idInfraccion";

					SqlCommand updateCommand = new SqlCommand(updateQuery, connection);

					updateCommand.Parameters.AddWithValue("@idInfraccion", idInfraccion);
					updateCommand.Parameters.AddWithValue("@cortesiaInt", cortesiaInt);
					updateCommand.Parameters.AddWithValue("@obs", observaciones);
					updateCommand.ExecuteNonQuery();

					string selectQuery = "SELECT infraccionCortesia FROM infracciones WHERE idInfraccion = @idInfraccion";
					SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
					selectCommand.Parameters.AddWithValue("@idInfraccion", idInfraccion);

					object infraccionCortesia = selectCommand.ExecuteScalar();
					if (infraccionCortesia != DBNull.Value)
					{
						result = (int)infraccionCortesia; // o cualquier otro tratamiento necesario
					}
				}
				catch (SqlException ex)
				{
					// Manejar la excepción
				}
				finally
				{
					connection.Close();
				}

				return result;

			}


		}

		public List<InfraccionesModel> GetReporteInfracciones(InfraccionesBusquedaModel model, int idOficina, int idDependenciaPerfil)
		{
			List<InfraccionesModel> InfraccionesList = new List<InfraccionesModel>();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					using (SqlCommand cmd = new SqlCommand("[sp_ReporteInfracciones]", connection))
					{


						using (var adapter = new SqlDataAdapter(cmd))
						{
							cmd.CommandType = CommandType.StoredProcedure;

							cmd.Parameters.AddWithValue("@idOficina", (idOficina == 0) ? DBNull.Value : idOficina);
							cmd.Parameters.AddWithValue("@idDependenciaPerfil", (object)idDependenciaPerfil ?? DBNull.Value);
							cmd.Parameters.AddWithValue("@IdGarantia", (object)model.IdGarantia ?? DBNull.Value);
							cmd.Parameters.AddWithValue("@IdTipoCortesia", (object)model.IdTipoCortesia ?? DBNull.Value);
							cmd.Parameters.AddWithValue("@IdDelegacion", (object)model.IdDelegacion ?? DBNull.Value);
							cmd.Parameters.AddWithValue("@IdEstatus", (object)model.IdEstatus ?? DBNull.Value);
							cmd.Parameters.AddWithValue("@IdDependencia", (object)model.IdDependencia ?? DBNull.Value);
							cmd.Parameters.AddWithValue("@numeroLicencia", (object)model.NumeroLicencia != null ? model.NumeroLicencia.ToUpper() : DBNull.Value);
							cmd.Parameters.AddWithValue("@numeroEconomico", (object)model.NumeroEconomico != null ? model.NumeroEconomico.ToUpper() : DBNull.Value);
							cmd.Parameters.AddWithValue("@FolioInfraccion", (object)model.folioInfraccion != null ? model.folioInfraccion.ToUpper() : DBNull.Value);
							cmd.Parameters.AddWithValue("@Placas", (object)model.placas != null ? model.placas.ToUpper() : DBNull.Value);
							cmd.Parameters.AddWithValue("@Propietario", (object)model.Propietario != null ? model.Propietario.ToUpper() : DBNull.Value);
							cmd.Parameters.AddWithValue("@Conductor", (object)model.Conductor != null ? model.Conductor.ToUpper() : DBNull.Value);

							//cmd.Parameters.AddWithValue("@FechaInicio", model.FechaInicio.Year <= 1900 ? DBNull.Value : model.FechaInicio);
							//cmd.Parameters.AddWithValue("@FechaFin", model.FechaFin.Year <= 1900 ? DBNull.Value : model.FechaFin);

							cmd.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.Date)).Value = model.FechaInicio.Year <= 1900 ? DBNull.Value : model.FechaInicio;
							cmd.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.Date)).Value = model.FechaFin.Year <= 1900 ? DBNull.Value : model.FechaFin;

							using SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

							while (reader.Read())
							{

								InfraccionesModel infraccionModel = new InfraccionesModel();
								infraccionModel.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
								infraccionModel.folioInfraccion = reader["folioInfraccion"].ToString();
								infraccionModel.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
								infraccionModel.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
								infraccionModel.estatusInfraccion = reader["estatusInfraccion"].ToString();
								infraccionModel.aplicacion = reader["aplicacion"].ToString();
								infraccionModel.delegacion = reader["nombreOficina"] == System.DBNull.Value ? string.Empty : reader["nombreOficina"].ToString();
								infraccionModel.placasVehiculo = reader["placasVehiculo"].ToString();

								//Asignacion de garantia
								infraccionModel.Garantia = new();
								infraccionModel.Garantia.garantia = reader["garantia"] == System.DBNull.Value ? string.Empty : reader["garantia"].ToString();

								//Asignacion de conductor
								infraccionModel.PersonaInfraccion = new();
								infraccionModel.PersonaInfraccion.nombre = reader["nombrePI"] == System.DBNull.Value ? string.Empty : reader["nombrePI"].ToString();
								infraccionModel.PersonaInfraccion.apellidoPaterno = reader["apellidoPaternoPI"] == System.DBNull.Value ? string.Empty : reader["apellidoPaternoPI"].ToString();
								infraccionModel.PersonaInfraccion.apellidoMaterno = reader["apellidoMaternoPI"] == System.DBNull.Value ? string.Empty : reader["apellidoMaternoPI"].ToString();

								//Asignacion de vehiculo
								infraccionModel.Vehiculo = new();
								infraccionModel.Vehiculo.placas = reader["placasVehiculo"] == System.DBNull.Value ? string.Empty : reader["placasVehiculo"].ToString();
								infraccionModel.Vehiculo.marca = reader["marcaVehiculo"] == System.DBNull.Value ? string.Empty : reader["marcaVehiculo"].ToString();
								infraccionModel.Vehiculo.submarca = reader["submarcaVehiculo"] == System.DBNull.Value ? string.Empty : reader["submarcaVehiculo"].ToString();
								infraccionModel.Vehiculo.modelo = reader["modelo"] == System.DBNull.Value ? string.Empty : reader["modelo"].ToString();

								//Asignacion de propietario del vehiculo
								infraccionModel.Vehiculo.Persona = new();
								infraccionModel.Vehiculo.Persona.nombre = reader["nombre"] == System.DBNull.Value ? string.Empty : reader["nombre"].ToString();
								infraccionModel.Vehiculo.Persona.apellidoPaterno = reader["apellidoPaterno"] == System.DBNull.Value ? string.Empty : reader["apellidoPaterno"].ToString();
								infraccionModel.Vehiculo.Persona.apellidoMaterno = reader["apellidoMaterno"] == System.DBNull.Value ? string.Empty : reader["apellidoMaterno"].ToString();

								InfraccionesList.Add(infraccionModel);
							}
						}
					}
				}

				catch (SqlException ex)
				{
					//Guardar la excepcion en algun log de errores
					Logger.Error("Error al generar reporte infracciones:" + ex);
				}
				finally
				{
					connection.Close();
				}
				return InfraccionesList;
			}
		}



		public int GetDiaFestivo(int idDelegacion, DateTime fecha)
		{

			int resultado = 0;
			string strQuery = @"SELECT 1 as resultado	                                  
                                FROM  catMunicipios as m, catDiasInhabiles as i
                                WHERE 
									 m.estatus=1
									and i.idMunicipio = @idDelegacion
									and i.estatus = 1
									and i.fecha = CONVERT(DATE, @fecha,103) 									
								UNION 

								select 1 as resultado 
								from catDiasInhabiles i
								WHERE i.fecha = CONVERT(DATE, @fecha,103) 
								and i.todosMunicipiosDesc = 'Si'
								and i.estatus = 1
								";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = (object)idDelegacion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@fecha", SqlDbType.DateTime)).Value = (object)fecha ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{

							resultado = reader["resultado"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["resultado"].ToString());


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

			return resultado;


		}

		public string GetPersonaFolioDetencion(int idPersona)
		{
			var resultado = string.Empty;
			var strQuery = @"SELECT TOP 1 ID.folioDetenido
							FROM infraccionesDetenidos AS ID, infracciones AS I
							WHERE ID.idInfraccion = I.idInfraccion
							AND I.idPersonaInfraccion = @idPersonaInfraccion
							ORDER BY ID.fechaActualizacion DESC";

			using (var connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					var command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idPersonaInfraccion", SqlDbType.Int)).Value = (object)idPersona ?? DBNull.Value;

					using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							resultado = reader["folioDetenido"] == DBNull.Value ? string.Empty : reader["folioDetenido"].ToString();
							break;
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

			return resultado;
		}

		public List<object> GetPersonasInvolucradas(int idInfraccion)
		{
			List<Persona> personasInvolucradas = new List<Persona>();

			using (var connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("SELECT idPersona id, nombre + ' ' + apellidoPaterno + ' ' + apellidoMaterno nombre FROM personas WHERE idPersona in (SELECT idPersonaInfraccion FROM infracciones WHERE idInfraccion = @idInfraccion)", connection))
				{
					command.Parameters.AddWithValue("@idInfraccion", idInfraccion);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							personasInvolucradas.Add(new Persona
							{
								IdPersona = reader.GetInt32(0),
								Nombre = reader.GetString(1)
							});
						}
					}
				}
			}
			return personasInvolucradas.Cast<object>().ToList();
		}
		public class Persona
		{
			public int IdPersona { get; set; }
			public string Nombre { get; set; }
		}
		public int CreateUpdatePersonaFolioDetencion(int idInfraccion, string folioDetencion)
		{
			var resultado = 0;
			var resultadoDetenidosId = 0;
			var strQuery = @"SELECT TOP 1 ID.detenidosId
							FROM infraccionesDetenidos AS ID
							WHERE ID.idInfraccion = @idInfraccion"
			;

			using (var connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					var command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;

					using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							resultadoDetenidosId = reader["detenidosId"] == DBNull.Value ? default(int) : Convert.ToInt32(reader["detenidosId"].ToString());
							break;
						}
					}

					connection.Open();

					if (resultadoDetenidosId > 0)
					{
						strQuery = @"UPDATE infraccionesDetenidos
									SET folioDetenido = @folioDetenido, fechaActualizacion = @fechaActualizacion
									WHERE idInfraccion = @idInfraccion AND detenidosId = @detenidosId";
						command = new SqlCommand(strQuery, connection);
						command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@folioDetenido", SqlDbType.NVarChar)).Value = (object)folioDetencion ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.UtcNow ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@detenidosId", SqlDbType.Int)).Value = (object)resultadoDetenidosId ?? DBNull.Value;
						resultado = command.ExecuteNonQuery();
					}
					else
					{
						strQuery = @"INSERT INTO infraccionesDetenidos (folioDetenido, fechaActualizacion, idInfraccion)
									VALUES(@folioDetenido, @fechaActualizacion, @idInfraccion);
									SELECT SCOPE_IDENTITY()";
						command = new SqlCommand(strQuery, connection);
						command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@folioDetenido", SqlDbType.NVarChar)).Value = (object)folioDetencion ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.UtcNow ?? DBNull.Value;
						resultado = Convert.ToInt32(command.ExecuteScalar());
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

			return resultado;
		}

		public int CreateUpdatePersonaFolioDetencionPersona(int idInfraccion, string folioDetencion, int idPersona)
		{
			var resultado = 0;
			var resultadoDetenidosId = 0;
			var strQuery = "";
			if (idInfraccion > 0)
			{
				strQuery = @"SELECT TOP 1 ID.detenidosId
							FROM infraccionesDetenidos AS ID
							WHERE ID.idInfraccion = @idInfraccion"
				;
			}
			else
			{
				strQuery = @"SELECT TOP 1 ID.detenidosId
							FROM infraccionesDetenidos AS ID
							WHERE ID.idPersona = @idPersona"
				;
			}

			using (var connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					var command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					if (idInfraccion > 0)
					{
						command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
					}
					else
					{
						command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)idPersona ?? DBNull.Value;
					}

					using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							resultadoDetenidosId = reader["detenidosId"] == DBNull.Value ? default(int) : Convert.ToInt32(reader["detenidosId"].ToString());
							break;
						}
					}

					connection.Open();

					if (resultadoDetenidosId > 0)
					{
						strQuery = @"UPDATE infraccionesDetenidos
									SET folioDetenido = @folioDetenido, fechaActualizacion = @fechaActualizacion
									WHERE idPersona = @idPersona AND detenidosId = @detenidosId";
						command = new SqlCommand(strQuery, connection);
						command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)idPersona ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@folioDetenido", SqlDbType.NVarChar)).Value = (object)folioDetencion ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.UtcNow ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@detenidosId", SqlDbType.Int)).Value = (object)resultadoDetenidosId ?? DBNull.Value;
						resultado = command.ExecuteNonQuery();
					}
					else
					{
						strQuery = @"INSERT INTO infraccionesDetenidos (folioDetenido, fechaActualizacion, idPersona, idInfraccion)
									VALUES(@folioDetenido, @fechaActualizacion, @idPersona, @idInfraccion);
									SELECT SCOPE_IDENTITY()";
						command = new SqlCommand(strQuery, connection);
						command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)idInfraccion ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)idPersona ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@folioDetenido", SqlDbType.NVarChar)).Value = (object)folioDetencion ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.UtcNow ?? DBNull.Value;
						resultado = Convert.ToInt32(command.ExecuteScalar());
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

			return resultado;
		}

		public int AnexarBoletaFisica(InfraccionesModel arhivoBoleta, int idInfraccion)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string updateQuery = @"
                    UPDATE infracciones
                    SET rutaBoleta = @rutaBoleta,
                        nombreBoleta = @nombreBoleta,
                        actualizadoPor = @actualizadoPor
						 WHERE idInfraccion = @idInfraccion";

					SqlCommand updateCommand = new SqlCommand(updateQuery, connection);

					updateCommand.Parameters.AddWithValue("@idInfraccion", idInfraccion);
					updateCommand.Parameters.AddWithValue("@rutaBoleta", arhivoBoleta.boletaFisicaPath ?? (object)DBNull.Value);
					updateCommand.Parameters.AddWithValue("@nombreBoleta", arhivoBoleta.nombreBoletaStr ?? (object)DBNull.Value);
					updateCommand.Parameters.AddWithValue("@actualizadoPor", 1);

					result = updateCommand.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					// Manejo de excepciones
					// Puedes agregar aquí el manejo de excepciones como logs si es necesario
					return result;
				}
				finally
				{
					connection.Close();
				}
			}

			return result;
		}

		public CatalogModel GetBoletaFisicaPath(int idInfraccion)
		{

			var result = new CatalogModel();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "select rutaBoleta as result,nombreBoleta as filename from  infracciones  where idInfraccion = @idInfraccion AND estatus = 1";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@idInfraccion", idInfraccion);
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{

						while (reader.Read())
						{
							result.value = (string)reader["result"];
							result.text = reader["result"] is DBNull ? "" : (string)reader["filename"];
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

				return result;
			}
		}

		public int GetAlertasInfraccion(int idInfraccion, int idOficina, int idAplicacion, int idPersonaAplicacion)
		{
			//validate if there is a condition in table
			int cantidadInfracciones = 0;
			int result = 0;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					const string SqlTransact =
						@"
							SELECT top(1) * FROM alertamiento 
							where idDelegacion=@idDelegacion  
							and idAplicacion=@idAplicacion
							and anio = YEAR(GETDATE()) order by idAlertamiento desc;";


					SqlCommand command = new SqlCommand(SqlTransact, connection);
					command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idAplicacion", SqlDbType.Int)).Value = (object)idAplicacion ?? DBNull.Value;

					command.CommandType = CommandType.Text;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							cantidadInfracciones = reader["cantidadInfracciones"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["cantidadInfracciones"].ToString());
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

			if (cantidadInfracciones > 0)
			{
				//check how many infractions
				using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
					try
					{
						connection.Open();
						string SqlTransact = "";

						if (idAplicacion == 1 || idAplicacion == 3)
                        {
							
							SqlTransact =
								@"
								SELECT count(idInfraccion) AS total FROM infracciones 
								where (idPersonaInfraccion=@idPersona or idPersonaConductor = @idPersona)
								and YEAR(fechaInfraccion) = YEAR(GETDATE());";
						}
						else 
						{

							SqlTransact =
								@"
								SELECT count(idInfraccion) AS total FROM infracciones 
								where idPersona=@idPersona
								and YEAR(fechaInfraccion) = YEAR(GETDATE());";
						}


						SqlCommand command = new SqlCommand(SqlTransact, connection);
						command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)idPersonaAplicacion ?? DBNull.Value;

						command.CommandType = CommandType.Text;
						int total = 0;
						using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
						{
							while (reader.Read())
							{
								total = reader["total"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["total"].ToString());
							}
						}
						if (total >= cantidadInfracciones)
						{
							result = cantidadInfracciones;
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
			return result;
		}

		private TurnoInfraccion GetTurnoAsignedToInfraccionOrDefault(int idInfraccion)
		{
			return _dbContext.TurnosInfracciones.FirstOrDefault(x => x.IdInfraccion == idInfraccion);
		}

		public void UpsertTurnoAsignedToInfraccion(SqlConnection connection, SqlTransaction transaction, long idTurno, int idInfraccion)
		{
			string table = "turnoInfracciones";
			string infraccionField = "idInfraccion";
			string turnoField = "idTurno";
			string query = $@"
				IF EXISTS (SELECT 1 FROM {table} WHERE {infraccionField} = @infraccion)
				BEGIN
					UPDATE {table} SET {turnoField} = @turno
					WHERE {infraccionField} = @infraccion;
				END
				ELSE
				BEGIN
					INSERT INTO {table} ({infraccionField}, {turnoField})
					VALUES (@infraccion, @turno);
				END";

			SqlCommand command = new(query, connection, transaction);
			command.Parameters.Add(new SqlParameter("@infraccion", SqlDbType.Int)).Value = idInfraccion;
			command.Parameters.Add(new SqlParameter("@turno", SqlDbType.BigInt)).Value = idTurno;

			command.ExecuteNonQuery();
		}

		private void RemoveTurnoAssignmentToInfraccion(SqlConnection connection, SqlTransaction trx, int idInfraccion)
		{
			using SqlCommand turnoCommand = new("DELETE FROM turnoInfracciones WHERE idInfraccion = @infraccion", connection, trx);
			turnoCommand.Parameters.Add(new SqlParameter("infraccion", SqlDbType.Int)).Value = idInfraccion;
			turnoCommand.ExecuteNonQuery();
		}

	}
}

