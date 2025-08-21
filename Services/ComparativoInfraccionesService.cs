using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GuanajuatoAdminUsuarios.Services
{
    public class ComparativoInfraccionesService : IComparativoInfraccionesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public ComparativoInfraccionesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }               

        public List<ResultadoGeneral> BusquedaResultadosGenerales(ComparativoInfraccionesModel modelBusqueda)
        {
            List<ResultadoGeneral> modelList = new List<ResultadoGeneral>();
            string condiciones = "";

            condiciones += " AND YEAR(inf.fechaInfraccion) in (@año1,@año2) ";
            condiciones += modelBusqueda.idDelegacion.Equals(null) || modelBusqueda.idDelegacion == 0 ? "" : " AND inf.idDelegacion = @idDelegacion ";
            condiciones += modelBusqueda.idOficial.Equals(null) || modelBusqueda.idOficial == 0 ? "" : " AND inf.idOficial =@idOficial ";
            condiciones += modelBusqueda.idCarretera.Equals(null) || modelBusqueda.idCarretera == 0 ? "" : " AND inf.idCarretera = @idCarretera ";
            condiciones += modelBusqueda.idTramo.Equals(null) || modelBusqueda.idTramo == 0 ? "" : " AND inf.idTramo = @idTramo ";
            condiciones += modelBusqueda.idTipoVehiculo.Equals(null) || modelBusqueda.idTipoVehiculo == 0 ? "" : " AND veh.idTipoVehiculo = @idTipoVehiculo ";
            condiciones += modelBusqueda.idTipoServicio.Equals(null) || modelBusqueda.idTipoServicio == 0 ? "" : " AND veh.idCatTipoServicio  = @idCatTipoServicio ";
            condiciones += modelBusqueda.idTipoLicencia.Equals(null) || modelBusqueda.idTipoLicencia == 0 ? "" : " AND gar.idTipoLicencia = @idTipoLicencia ";
            condiciones += modelBusqueda.idMunicipio.Equals(null) || modelBusqueda.idMunicipio == 0 ? "" : " AND inf.idMunicipio =@idMunicipio ";
            if (modelBusqueda.idTipo == 0)
                condiciones += " AND catMotInf.transito IN(0,1)";
            else if (modelBusqueda.idTipo == 1)
                condiciones += " AND catMotInf.transito=1";
            else if (modelBusqueda.idTipo == 2)
                condiciones += " AND catMotInf.transito=0";

            /* string query = @"SELECT YEAR(inf.fechaInfraccion) AS ANIO, COUNT(*) AS TOTAL
                 FROM infracciones inf
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
                 left join catMotivosInfraccion catMotInf on motInf.idCatMotivosInfraccion = catMotInf.idCatMotivoInfraccion 
                 left join catSubConceptoInfraccion catSubInf on catMotInf.IdSubConcepto = catSubInf.idSubConcepto
                 left join catConceptoInfraccion catConInf on  catSubInf.idConcepto = catConInf.idConcepto
                 WHERE inf.estatus = 1 @WHERES
                 GROUP BY YEAR(inf.fechaInfraccion)"
             ;*/
            string query = @"SELECT YEAR(inf.fechaInfraccion) AS ANIO, COUNT(*) AS TOTAL
                FROM infracciones inf
                left join motivosInfraccion motInf on inf.IdInfraccion = motInf.idInfraccion
                left join catMotivosInfraccion catMotInf on motInf.idCatMotivosInfraccion = catMotInf.idCatMotivoInfraccion 
                WHERE inf.estatus = 1 @WHERES
                GROUP BY YEAR(inf.fechaInfraccion)"
           ;

            string strQuery = query.Replace("@WHERES", condiciones);

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;

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
                        command.Parameters.Add(new SqlParameter("@idCatTipoServicio", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoServicio ?? DBNull.Value;

                    if (!modelBusqueda.idTipoLicencia.Equals(null) && modelBusqueda.idTipoLicencia != 0)
                        command.Parameters.Add(new SqlParameter("@idTipoLicencia", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoLicencia ?? DBNull.Value;

                    if (!modelBusqueda.idMunicipio.Equals(null) && modelBusqueda.idMunicipio != 0)
                        command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)modelBusqueda.idMunicipio ?? DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@año1", SqlDbType.Int)).Value = (object)modelBusqueda.año1 ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@año2", SqlDbType.Int)).Value = (object)modelBusqueda.año2 ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ResultadoGeneral model = new ResultadoGeneral();
                            model.año = reader["ANIO"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["ANIO"].ToString());
                            model.total = reader["TOTAL"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["TOTAL"].ToString());
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

		public List<DetallePorCausa> BusquedaDetallesPorCausas(ComparativoInfraccionesModel modelBusqueda)
		{
			List<DetallePorCausa> modelList = new List<DetallePorCausa>();
			string condiciones = "";

			// Siempre incluir estas condiciones 
			condiciones += " AND inf.estatus = 1";
			condiciones += " AND YEAR(inf.fechaInfraccion) IN (@año1, @año2)";

			if (modelBusqueda.idDelegacion != null && modelBusqueda.idDelegacion != 0)
				condiciones += " AND inf.idDelegacion = @idDelegacion";
			if (modelBusqueda.idOficial != null && modelBusqueda.idOficial != 0)
				condiciones += " AND inf.idOficial = @idOficial";
			if (modelBusqueda.idCarretera != null && modelBusqueda.idCarretera != 0)
				condiciones += " AND inf.idCarretera = @idCarretera";
			if (modelBusqueda.idTramo != null && modelBusqueda.idTramo != 0)
				condiciones += " AND inf.idTramo = @idTramo";
			if (modelBusqueda.idTipoVehiculo != null && modelBusqueda.idTipoVehiculo != 0)
				condiciones += " AND veh.idTipoVehiculo = @idTipoVehiculo";
			if (modelBusqueda.idTipoServicio != null && modelBusqueda.idTipoServicio != 0)
				condiciones += " AND veh.idCatTipoServicio = @idCatTipoServicio";
			if (modelBusqueda.idTipoLicencia != null && modelBusqueda.idTipoLicencia != 0)
				condiciones += " AND gar.idTipoLicencia = @idTipoLicencia";
			if (modelBusqueda.idMunicipio != null && modelBusqueda.idMunicipio != 0)
				condiciones += " AND inf.idMunicipio = @idMunicipio";

			if (modelBusqueda.idTipo == 0)
				condiciones += " AND catMotInf.transito IN (0, 1)";
			else if (modelBusqueda.idTipo == 1)
				condiciones += " AND catMotInf.transito = 1";
			else if (modelBusqueda.idTipo == 2)
				condiciones += " AND catMotInf.transito = 0";

			string query = $@"SELECT catMotInf.nombre AS CAUSA, COUNT(*) AS CANTIDAD, YEAR(inf.fechaInfraccion) AS ANIO 
                      FROM infracciones inf
                      LEFT JOIN motivosInfraccion motInf ON inf.IdInfraccion = motInf.idInfraccion
                      LEFT JOIN catMotivosInfraccion catMotInf ON motInf.idCatMotivosInfraccion = catMotInf.idCatMotivoInfraccion 
                      WHERE 1=1 {condiciones}
                      GROUP BY catMotInf.nombre, YEAR(inf.fechaInfraccion)";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(query, connection);
					command.CommandType = CommandType.Text;

					command.Parameters.Add(new SqlParameter("@año1", SqlDbType.Int)).Value = modelBusqueda.año1;
					command.Parameters.Add(new SqlParameter("@año2", SqlDbType.Int)).Value = modelBusqueda.año2;

					if (modelBusqueda.idDelegacion != null && modelBusqueda.idDelegacion != 0)
						command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = modelBusqueda.idDelegacion;

					if (modelBusqueda.idOficial != null && modelBusqueda.idOficial != 0)
						command.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = modelBusqueda.idOficial;

					if (modelBusqueda.idCarretera != null && modelBusqueda.idCarretera != 0)
						command.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = modelBusqueda.idCarretera;

					if (modelBusqueda.idTramo != null && modelBusqueda.idTramo != 0)
						command.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = modelBusqueda.idTramo;

					if (modelBusqueda.idTipoVehiculo != null && modelBusqueda.idTipoVehiculo != 0)
						command.Parameters.Add(new SqlParameter("@idTipoVehiculo", SqlDbType.Int)).Value = modelBusqueda.idTipoVehiculo;

					if (modelBusqueda.idTipoServicio != null && modelBusqueda.idTipoServicio != 0)
						command.Parameters.Add(new SqlParameter("@idCatTipoServicio", SqlDbType.Int)).Value = modelBusqueda.idTipoServicio;

					if (modelBusqueda.idTipoLicencia != null && modelBusqueda.idTipoLicencia != 0)
						command.Parameters.Add(new SqlParameter("@idTipoLicencia", SqlDbType.Int)).Value = modelBusqueda.idTipoLicencia;

					if (modelBusqueda.idMunicipio != null && modelBusqueda.idMunicipio != 0)
						command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = modelBusqueda.idMunicipio;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							DetallePorCausa model = new DetallePorCausa();
							model.causaTrunc = reader["CAUSA"] == System.DBNull.Value ? default(string) : Truncate(reader["CAUSA"].ToString(), 35);
							model.causa = reader["CAUSA"] == System.DBNull.Value ? default(string) : reader["CAUSA"].ToString();
							model.cantidad = reader["CANTIDAD"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["CANTIDAD"].ToString());
							model.año = reader["ANIO"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["ANIO"].ToString());
							modelList.Add(model);
						}
					}
				}
				catch (SqlException ex)
				{
					// Guardar la excepción en algún log de errores
				}
				finally
				{
					connection.Close();
				}
			}
			return modelList;
		}

		private string Truncate(string value, int maxLength)
		{
			if (string.IsNullOrEmpty(value) || value.Length <= maxLength) return value;
			return value.Substring(0, maxLength) + "..."; 
		}

		public List<DesgloseTotalInfraccion> DesgloseTotalesInfracciones(ComparativoInfraccionesModel modelBusqueda)
        {
            List<DesgloseTotalInfraccion> modelList = new List<DesgloseTotalInfraccion>();
            string condiciones = "";

            condiciones += " AND YEAR(inf.fechaInfraccion) in (@año1,@año2) ";
            condiciones += modelBusqueda.idDelegacion.Equals(null) || modelBusqueda.idDelegacion == 0 ? "" : " AND inf.idDelegacion = @idDelegacion ";
            condiciones += modelBusqueda.idOficial.Equals(null) || modelBusqueda.idOficial == 0 ? "" : " AND inf.idOficial =@idOficial ";
            condiciones += modelBusqueda.idCarretera.Equals(null) || modelBusqueda.idCarretera == 0 ? "" : " AND inf.idCarretera = @idCarretera ";
            condiciones += modelBusqueda.idTramo.Equals(null) || modelBusqueda.idTramo == 0 ? "" : " AND inf.idTramo = @idTramo ";
            condiciones += modelBusqueda.idTipoVehiculo.Equals(null) || modelBusqueda.idTipoVehiculo == 0 ? "" : " AND veh.idTipoVehiculo = @idTipoVehiculo ";
            condiciones += modelBusqueda.idTipoServicio.Equals(null) || modelBusqueda.idTipoServicio == 0 ? "" : " AND veh.idCatTipoServicio  = @idCatTipoServicio ";
            condiciones += modelBusqueda.idTipoLicencia.Equals(null) || modelBusqueda.idTipoLicencia == 0 ? "" : " AND gar.idTipoLicencia = @idTipoLicencia ";
            condiciones += modelBusqueda.idMunicipio.Equals(null) || modelBusqueda.idMunicipio == 0 ? "" : " AND inf.idMunicipio =@idMunicipio ";
            if (modelBusqueda.idTipo ==0 )
                condiciones += " AND catMotInf.transito IN(0,1)";
            else if (modelBusqueda.idTipo == 1)
                condiciones += " AND catMotInf.transito=1";
            else if (modelBusqueda.idTipo == 2)
                condiciones += " AND catMotInf.transito=0";

            string query = @"SELECT C.NUMERO_MOTIVO AS NUMERO_MOTIVO, COUNT(C.idInfraccion) AS TOTAL_INFRACCIONES, CUENTA* Count(c.idInfraccion) AS TOTAL_CONTAB, C.FECHA AS ANIO 
                        FROM (
                        SELECT COUNT(distinct MI.idMotivoInfraccion) CUENTA, CONCAT('CON ',COUNT(distinct MI.idMotivoInfraccion), ' MOTIVO(S)') AS NUMERO_MOTIVO, 
                        inf.idInfraccion,
	                    YEAR(inf.fechaInfraccion) AS FECHA
	                    FROM infracciones inf
	                    LEFT JOIN motivosInfraccion MI ON MI.idInfraccion = inf.idInfraccion
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
	                    left join catMotivosInfraccion catMotInf on motInf.idCatMotivosInfraccion = catMotInf.idCatMotivoInfraccion 
	                    left join catSubConceptoInfraccion catSubInf on catMotInf.IdSubConcepto = catSubInf.idSubConcepto
	                    left join catConceptoInfraccion catConInf on  catSubInf.idConcepto = catConInf.idConcepto
	                    WHERE inf.estatus = 1 @WHERES
	                    GROUP BY inf.idInfraccion, YEAR(inf.fechaInfraccion)
                    ) C
                    GROUP BY C.CUENTA, C.NUMERO_MOTIVO, C.FECHA
                    ORDER BY FECHA, C.CUENTA ASC "
			;
            string strQuery = query.Replace("@WHERES", condiciones);

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;

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
                        command.Parameters.Add(new SqlParameter("@idCatTipoServicio", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoServicio ?? DBNull.Value;

                    if (!modelBusqueda.idTipoLicencia.Equals(null) && modelBusqueda.idTipoLicencia != 0)
                        command.Parameters.Add(new SqlParameter("@idTipoLicencia", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoLicencia ?? DBNull.Value;

                    if (!modelBusqueda.idMunicipio.Equals(null) && modelBusqueda.idMunicipio != 0)
                        command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)modelBusqueda.idMunicipio ?? DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@año1", SqlDbType.Int)).Value = (object)modelBusqueda.año1 ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@año2", SqlDbType.Int)).Value = (object)modelBusqueda.año2 ?? DBNull.Value;
                    //if (modelBusqueda.idTipo == 0)
                    //command.Parameters.Add(new SqlParameter("@idTipo", SqlDbType.Int)).Value = (object)modelBusqueda.idTipo ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            DesgloseTotalInfraccion model = new DesgloseTotalInfraccion();
                            model.numeroMotivo = reader["NUMERO_MOTIVO"] == System.DBNull.Value ? default(string) : reader["NUMERO_MOTIVO"].ToString();
                            model.totalInfracciones = reader["TOTAL_INFRACCIONES"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["TOTAL_INFRACCIONES"].ToString());
                            model.totalContabiliza = reader["TOTAL_CONTAB"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["TOTAL_CONTAB"].ToString());
                            model.año = reader["ANIO"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["ANIO"].ToString());
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
