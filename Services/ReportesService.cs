using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class ReportesService : IReportes
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public ReportesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
   
            public List<ReportesModel>BusquedaInfraccionesAccidentesMunicipio(int corp)
            {
                //
                List<ReportesModel> ListaTiposLicencia = new List<ReportesModel>();

                using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                    try

                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(@"SELECT 
                                                                del.idOficinaTransporte AS idOficina,
                                                                del.nombreOficina,
                                                                ISNULL(acc.total_accidentes, 0) AS total_accidentes,
                                                                ISNULL(inf.total_infracciones, 0) AS total_infracciones
                                                            FROM 
                                                                catDelegacionesOficinasTransporte del
                                                            LEFT JOIN 
                                                                (SELECT 
                                                                     a.idOficinaDelegacion, 
                                                                     COUNT(*) AS total_accidentes
                                                                 FROM 
                                                                     accidentes a
                                                                 WHERE 
                                                                      a.estatus = 1
                                                                 GROUP BY 
                                                                     a.idOficinaDelegacion) acc ON del.idOficinaTransporte = acc.idOficinaDelegacion
                                                            LEFT JOIN 
                                                                (SELECT 
                                                                     i.idDelegacion, 
                                                                     COUNT(*) AS total_infracciones
                                                                 FROM 
                                                                     infracciones i
                                                                 WHERE 
                                                                      i.estatus = 1 AND i.transito = @corp
                                                                 GROUP BY 
                                                                     i.idDelegacion) inf ON del.idOficinaTransporte = inf.idDelegacion
                                                            WHERE 
                                                                acc.total_accidentes IS NOT NULL OR inf.total_infracciones IS NOT NULL
                                                            ORDER BY 
                                                                total_accidentes DESC, total_infracciones DESC;
                                                            ", connection);
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("corp", corp);
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                            ReportesModel infra = new ReportesModel();

                                infra.nombreOficina = reader["nombreOficina"] != DBNull.Value ? reader["nombreOficina"].ToString() : "";
                                infra.cantidadAccidentes = reader["total_accidentes"] != DBNull.Value ? Convert.ToInt32(reader["total_accidentes"]) : 0;
                            infra.cantidadInfracciones = reader["total_infracciones"] != DBNull.Value ? Convert.ToInt32(reader["total_infracciones"]) : 0;

                            ListaTiposLicencia.Add(infra);

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
                return ListaTiposLicencia;


            }
        public List<ReportesModel> BusquedaInfTodasCorporaciones()
        {
            //
            List<ReportesModel> ListaInfracciones = new List<ReportesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT transito,c.Corporacion, COUNT(*) AS total_infracciones
                                                            FROM infracciones i
                                                            JOIN CatCorporaciones c ON c.IdCorporacion = i.transito
                                                            GROUP BY transito,Corporacion
                                                            ORDER BY total_infracciones DESC;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ReportesModel infra = new ReportesModel();

                            infra.Corporacion = reader["Corporacion"] != DBNull.Value ? reader["Corporacion"].ToString() : "";
                            infra.cantidad = reader["total_infracciones"] != DBNull.Value ? Convert.ToInt32(reader["total_infracciones"]) : 0;

                            ListaInfracciones.Add(infra);

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
        public List<ReportesModel> BusquedaAccidentesTodasCorporaciones()
        {
            //
            List<ReportesModel> ListaAccidentes = new List<ReportesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT m.transito,c.Corporacion,COUNT(*) AS total_accidentes
                                                            FROM accidentes  a
                                                            LEFT JOIN catMunicipios m on M.idMunicipio = a.idMunicipio
                                                            JOIN CatCorporaciones c ON c.IdCorporacion = m.transito
                                                            GROUP BY m.transito,c.Corporacion
                                                            ORDER BY total_accidentes DESC;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ReportesModel accidente = new ReportesModel();

                            accidente.Corporacion = reader["Corporacion"] != DBNull.Value ? reader["Corporacion"].ToString() : "";
                            accidente.cantidad = reader["total_accidentes"] != DBNull.Value ? Convert.ToInt32(reader["total_accidentes"]) : 0;

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

        public List<ReportesModel> BusquedaInfPorTipoLicencia(int corp)
        {
            //
            List<ReportesModel> ListaTiposLicencia = new List<ReportesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT 
                                                                tl.tipoLicencia,
                                                                COUNT(i.idInfraccion) AS total_infracciones,
	                                                            SUM(COUNT(i.idInfraccion)) OVER () AS suma_total_infracciones
   
                                                            FROM 
                                                                infracciones i
                                                            LEFT JOIN 
                                                                personas p ON p.idPersona = i.idPersonaInfraccion
                                                            LEFT JOIN 
                                                                catTipoLicencia tl ON tl.idTipoLicencia = p.idTipoLicencia
                                                            WHERE 
                                                                tl.idTipoLicencia IS NOT NULL AND i.estatus = 1 AND i.transito = @corp
                                                            GROUP BY 
                                                                tl.tipoLicencia
                                                            ORDER BY 
                                                                total_infracciones DESC;
                                                            ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corp);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ReportesModel infra = new ReportesModel();

                            infra.tipoLicencia = reader["tipoLicencia"] != DBNull.Value ? reader["tipoLicencia"].ToString() : "";
                            infra.cantidad = reader["total_infracciones"] != DBNull.Value ? Convert.ToInt32(reader["total_infracciones"]) : 0;
                            infra.ContadorTotal = reader["suma_total_infracciones"] != DBNull.Value ? Convert.ToInt32(reader["suma_total_infracciones"]) : 0;

                            ListaTiposLicencia.Add(infra);

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
            return ListaTiposLicencia;


        }
    

    public List<ReportesModel> BusquedaMunicipiosMasAccidentes(int corp)
    {
        //
        List<ReportesModel> ListaAccidentes = new List<ReportesModel>();

        using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            try

            {
                connection.Open();
                SqlCommand command = new SqlCommand(@"SELECT 
                                                                tl.tipoLicencia,
                                                                COUNT(i.idInfraccion) AS total_infracciones,
	                                                            SUM(COUNT(i.idInfraccion)) OVER () AS suma_total_infracciones
   
                                                            FROM 
                                                                infracciones i
                                                            LEFT JOIN 
                                                                personas p ON p.idPersona = i.idPersonaInfraccion
                                                            LEFT JOIN 
                                                                catTipoLicencia tl ON tl.idTipoLicencia = p.idTipoLicencia
                                                            WHERE 
                                                                tl.idTipoLicencia IS NOT NULL AND i.estatus = 1 AND i.transito = @corp
                                                            GROUP BY 
                                                                tl.tipoLicencia
                                                            ORDER BY 
                                                                total_infracciones DESC;
                                                            ", connection);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("corp", corp);
                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        ReportesModel accidente = new ReportesModel();

                            accidente.municipo = reader["tipoLicencia"] != DBNull.Value ? reader["tipoLicencia"].ToString() : "";
                            accidente.cantidad = reader["total_infracciones"] != DBNull.Value ? Convert.ToInt32(reader["total_infracciones"]) : 0;

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


    public List<ReportesModel> BusquedaInfraccionesPorLicencia(string txtLicencia)
    {
        //
        List<ReportesModel> ListaInfracciones = new List<ReportesModel>();

        using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            try

            {
                connection.Open();
                SqlCommand command = new SqlCommand(@"SELECT i.idInfraccion,
                                                            i.folioInfraccion,
                                                            est.estatusInfraccion,
                                                            (SELECT COUNT(*) 
                                                             FROM infracciones i2 
                                                             LEFT JOIN personas p2 ON p2.idPersona = i2.idPersonaInfraccion
                                                             WHERE p2.numeroLicencia = @licencia AND i2.estatus = 1) AS totalRegistros
                                                        FROM 
                                                            infracciones i
                                                        LEFT JOIN 
                                                            personas p ON p.idPersona = i.idPersonaInfraccion
                                                        LEFT JOIN 
                                                            catEstatusInfraccion est ON est.idEstatusInfraccion = i.idEstatusInfraccion
                                                        WHERE 
                                                            p.numeroLicencia = @licencia AND i.estatus = 1;
                                                        ", connection);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@licencia", txtLicencia);
                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        ReportesModel infraccion = new ReportesModel();

                            infraccion.folioInfraccion = reader["folioInfraccion"] != DBNull.Value ? reader["folioInfraccion"].ToString() : "";
                            infraccion.estatusInfraccion = reader["estatusInfraccion"] != DBNull.Value ? reader["estatusInfraccion"].ToString() : "";
                            infraccion.Id = reader["idInfraccion"] != DBNull.Value ? Convert.ToInt32(reader["idInfraccion"]) : 0;

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

        public List<ReportesModel> BusquedaMunicipiosMasInfracciones(int corp)
        {
            //
            List<ReportesModel> ListaInfracciones = new List<ReportesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT 
                                                            i.idMunicipio,
	                                                        m.municipio,
                                                            COUNT(*) AS numeroInfracciones
                                                        FROM 
                                                            infracciones i
                                                        LEFT JOIN catMunicipios m ON m.idMunicipio = i.idMunicipio
                                                        WHERE i.estatus = 1 AND i.transito = @corp
                                                        GROUP BY 
                                                            i.idMunicipio,m.municipio
                                                        ORDER BY 
                                                            numeroInfracciones DESC;
                                                        ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@corp", corp);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ReportesModel infraccion = new ReportesModel();

                            infraccion.municipo = reader["municipio"] != DBNull.Value ? reader["municipio"].ToString() : "";
                            infraccion.cantidadInfracciones = reader["numeroInfracciones"] != DBNull.Value ? Convert.ToInt32(reader["numeroInfracciones"]) : 0;
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

        public List<ReportesModel> BusquedaMunicipiosMasAccidentesB()
        {
            //
            List<ReportesModel> ListaInfracciones = new List<ReportesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT 
                                                            a.idMunicipio,
	                                                        m.municipio,
                                                            COUNT(*) AS numeroAccidentes
                                                        FROM 
                                                            accidentes a
                                                        LEFT JOIN catMunicipios m ON m.idMunicipio = a.idMunicipio
                                                        WHERE a.estatus = 1
                                                        GROUP BY 
                                                            a.idMunicipio,m.municipio
                                                        ORDER BY 
                                                            numeroAccidentes DESC;
                                                        ", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ReportesModel infraccion = new ReportesModel();

                            infraccion.municipo = reader["municipio"] != DBNull.Value ? reader["municipio"].ToString() : "";
                            infraccion.cantidadAccidentes = reader["numeroAccidentes"] != DBNull.Value ? Convert.ToInt32(reader["numeroAccidentes"]) : 0;
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

        public List<ReportesModel> BusquedaMunicipiosColoniasMasInfracciones(int corp)
        {
            //
            List<ReportesModel> ListaInfracciones = new List<ReportesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT TOP 20
                                                                i.idMunicipio,
	                                                            m.municipio,
                                                                i.lugarColonia,
                                                                COUNT(*) AS numeroInfracciones
                                                            FROM 
                                                                infracciones i 
                                                            LEFT JOIN catMunicipios m ON m.idMunicipio = i.idmunicipio
                                                            WHERE i.estatus = 1  AND i.transito = @corp
                                                            GROUP BY 
                                                                i.idMunicipio,
                                                                i.lugarColonia,
	                                                            m.municipio
                                                            ORDER BY 
                                                                numeroInfracciones DESC;
                                                            ;
                                                        ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@corp", corp);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ReportesModel infraccion = new ReportesModel();

                            infraccion.municipo = reader["municipio"] != DBNull.Value ? reader["municipio"].ToString() : "";
                            infraccion.colonia = reader["lugarColonia"] != DBNull.Value ? reader["lugarColonia"].ToString() : "";
                            infraccion.cantidadInfracciones = reader["numeroInfracciones"] != DBNull.Value ? Convert.ToInt32(reader["numeroInfracciones"]) : 0;
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

        public List<ReportesModel> BusquedaMunicipiosColoniasMasAccidentes()
        {
            //
            List<ReportesModel> ListaAccidentes = new List<ReportesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT TOP 20
                                                                a.idMunicipio,
	                                                            m.municipio,
                                                                a.lugarColonia,
                                                                COUNT(*) AS numeroAccidentes
                                                            FROM 
                                                                accidentes a
                                                            LEFT JOIN catMunicipios m ON m.idMunicipio = a.idmunicipio
                                                            WHERE a.estatus = 1 
                                                            GROUP BY 
                                                                a.idMunicipio,
                                                                a.lugarColonia,
	                                                            m.municipio
                                                            ORDER BY 
                                                                numeroAccidentes DESC;
                                                        ", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ReportesModel accidente = new ReportesModel();

                            accidente.municipo = reader["municipio"] != DBNull.Value ? reader["municipio"].ToString() : "";
                            accidente.colonia = reader["lugarColonia"] != DBNull.Value ? reader["lugarColonia"].ToString() : "";
                            accidente.cantidadAccidentes = reader["numeroAccidentes"] != DBNull.Value ? Convert.ToInt32(reader["numeroAccidentes"]) : 0;
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

        public List<ReportesModel> BusquedaDanosAccidentes()
        {
            //
            List<ReportesModel> ListaAccidentes = new List<ReportesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT 
	                                                        numeroReporte,
                                                            montoCarga,
                                                            montoCamino,
                                                            montoPropietarios,
                                                            montoOtros,
                                                            montoVehiculo,
                                                           (ISNULL(TRY_CAST(REPLACE(REPLACE(montoCarga, '$', ''), ',', '') AS DECIMAL(10, 2)), 0) +
                                                             ISNULL(TRY_CAST(REPLACE(REPLACE(montoCamino, '$', ''), ',', '') AS DECIMAL(10, 2)), 0) +
                                                             ISNULL(TRY_CAST(REPLACE(REPLACE(montoPropietarios, '$', ''), ',', '') AS DECIMAL(10, 2)), 0) +
                                                             ISNULL(TRY_CAST(REPLACE(REPLACE(montoOtros, '$', ''), ',', '') AS DECIMAL(10, 2)), 0) +
                                                             ISNULL(TRY_CAST(REPLACE(REPLACE(montoVehiculo, '$', ''), ',', '') AS DECIMAL(10, 2)), 0)) 
                                                             AS total
                                                        FROM 
                                                            accidentes
                                                        WHERE estatus = 1
                                                        ", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ReportesModel accidente = new ReportesModel();

                            accidente.numeroReporte = reader["numeroReporte"] != DBNull.Value ? reader["numeroReporte"].ToString() : "";
                            accidente.montoCarga = reader["montoCarga"] != DBNull.Value ? reader["montoCarga"].ToString() : "";
                            accidente.montoCamino = reader["montoCamino"] != DBNull.Value ? reader["montoCamino"].ToString() : "";
                            accidente.montoPropietarios = reader["montoPropietarios"] != DBNull.Value ? reader["montoPropietarios"].ToString() : "";
                            accidente.montoOtros = reader["montoOtros"] != DBNull.Value ? reader["montoOtros"].ToString() : "";
                            accidente.ContadorTotal = reader["total"] != DBNull.Value ? Convert.ToInt32(reader["total"]) : 0;
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

        public List<ReportesModel> BusquedaInfraccionesProDiayHora()
        {
            //
            List<ReportesModel> ListaInfracciones = new List<ReportesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SET LANGUAGE Spanish;
                                                                WITH InfraccionesPorHora AS (
                                                                    SELECT 
                                                                        DATENAME(WEEKDAY, fechaInfraccion) AS DiaDeLaSemana,
                                                                        CAST(SUBSTRING(horaInfraccion, 1, 2) + ':' + SUBSTRING(horaInfraccion, 3, 2) AS TIME) AS Hora,
                                                                        COUNT(*) AS ConteoInfracciones
                                                                    FROM 
                                                                        infracciones
                                                                    WHERE 
                                                                        horaInfraccion IS NOT NULL AND estatus = 1
                                                                    GROUP BY 
                                                                        DATENAME(WEEKDAY, fechaInfraccion),
                                                                        CAST(SUBSTRING(horaInfraccion, 1, 2) + ':' + SUBSTRING(horaInfraccion, 3, 2) AS TIME)
                                                                ),
                                                                InfraccionesMaximas AS (
                                                                    SELECT 
                                                                        DiaDeLaSemana,
                                                                        Hora,
                                                                        ConteoInfracciones,
                                                                        ROW_NUMBER() OVER (PARTITION BY DiaDeLaSemana ORDER BY ConteoInfracciones DESC) AS rn
                                                                    FROM 
                                                                        InfraccionesPorHora
                                                                )

                                                                SELECT 
                                                                    DiaDeLaSemana,
                                                                    Hora,
                                                                    ConteoInfracciones
                                                                FROM 
                                                                    InfraccionesMaximas
                                                                WHERE 
                                                                    rn = 1  
                                                                ORDER BY 
                                                                    ConteoInfracciones DESC;
                                                        ", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ReportesModel infraccion = new ReportesModel();

                            infraccion.diaSemana = reader["DiaDeLaSemana"] != DBNull.Value ? reader["DiaDeLaSemana"].ToString() : "";
                            infraccion.hora = reader["Hora"] != DBNull.Value ?
                                              ((TimeSpan)reader["Hora"]).ToString(@"hh\:mm") : "";
                            infraccion.cantidadInfracciones = reader["ConteoInfracciones"] != DBNull.Value ? Convert.ToInt32(reader["ConteoInfracciones"]) : 0;
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
    }
}

