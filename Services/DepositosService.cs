using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using Logger = GuanajuatoAdminUsuarios.Util.Logger;
using System.Globalization;
using GuanajuatoAdminUsuarios.Entity;

namespace GuanajuatoAdminUsuarios.Services
{
    public class DepositosService : IDepositosService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        private readonly IVehiculosService _vehiculosService;        
        
        public DepositosService(ISqlClientConnectionBD sqlClientConnectionBD, IVehiculosService vehiculosService)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
            _vehiculosService = vehiculosService;
        }
        
        public string GuardarSolicitud(SolicitudDepositoModel model, int idOficina, string oficina, string abreviaturaMunicipio, int anio, int dependencia,int usuario=1)
        {
            string consecutivo = string.Empty;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("usp_InsertaSolicitud", connection);
                    command.CommandTimeout = 6000;
                    command.CommandType = CommandType.StoredProcedure;
                    DateTime fechaSol = model.fechaSolicitud;
                    TimeSpan horaSol = model.horaSolicitud;
                    DateTime fechaHoraSolicitud = fechaSol.Date + horaSol;
                    command.Parameters.AddWithValue("@fechaSolicitud", fechaHoraSolicitud);
                    command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = (object)model.idInfraccion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idVehiculo", SqlDbType.Int)).Value = (object)model.idVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoVehiculo", SqlDbType.Int)).Value = (object)model.idTipoVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idPropietaroGrua", SqlDbType.Int)).Value = (object)model.idConcecionario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = (object)model.idOficial ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idDescripcionEvento", SqlDbType.Int)).Value = (object)model.idDescripcionEvento ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoUsuario", SqlDbType.Int)).Value = (object)model.idTipoUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@nombreUsuario", SqlDbType.VarChar)).Value = (object)model.nombreUsuario.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoPaternoUsuario", SqlDbType.VarChar)).Value = (object)model.apellidoPaternoUsuario.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoMaternoUsuario", SqlDbType.VarChar)).Value = (object)model.apellidoMaternoUsuario.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@coloniaUsuario", SqlDbType.VarChar)).Value = (object)model.coloniaUsuario.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@calleUsuario", SqlDbType.VarChar)).Value = (object)model.calleUsuario.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@numeroUsuario", SqlDbType.VarChar)).Value = (object)model.numeroUsuario.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)model.idEntidad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)model.idMunicipio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@telefonoUsuario", SqlDbType.VarChar)).Value = (object)model.telefonoUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMotivoAsignacion", SqlDbType.Int)).Value = (object)model.idMotivoAsignacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = usuario;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@idServicioRequiere", SqlDbType.Int)).Value = (object)model.idServicioRequiere ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@numeroUbicacion", SqlDbType.VarChar)).Value = (object)model.numeroUbicacion.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@calleUbicacion", SqlDbType.VarChar)).Value = (object)model.calleUbicacion.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@coloniaUbicacion", SqlDbType.VarChar)).Value = (object)model.coloniaUbicacion.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@kilometroUbicacion", SqlDbType.VarChar)).Value = (object)model.kilometroUbicacion.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@interseccion", SqlDbType.VarChar)).Value = (object)model.interseccion.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = (object)model.IdCarretera ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = (object)model.IdTramo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idEntidadUbicacion", SqlDbType.Int)).Value = (object)model.idEntidadUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipioUbicacion", SqlDbType.Int)).Value = (object)model.idMunicipioUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idPensionUbicacion", SqlDbType.Int)).Value = (object)model.idPensionUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Dependencia", SqlDbType.Int)).Value = dependencia;
                    command.Parameters.Add(new SqlParameter("@abreviaturaMunicipio", SqlDbType.VarChar)).Value = abreviaturaMunicipio ;
                    command.Parameters.Add(new SqlParameter("@anio", SqlDbType.Int)).Value = anio;
                    command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = idOficina;

                    ////Se busca el ultimo consecutivo
                    //command = new SqlCommand("select id,consecutivo from foliosSolicitud where abreviaturaMunicipio =@abreviaturaMunicipio and anio=@anio and idDelegacion=@idDelegacion", connection)
                    //{
                    //    CommandType = CommandType.Text
                    //};
                    //command.Parameters.Add(new SqlParameter("@abreviaturaMunicipio", SqlDbType.VarChar)).Value = abreviaturaMunicipio;
                    //command.Parameters.Add(new SqlParameter("@anio", SqlDbType.VarChar)).Value = anio;
                    //command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.VarChar)).Value = idOficina;

                    //int consecutivo = -1;
                    //int idFolioSolicitud = -1;


                    //using (SqlDataReader reader = command.ExecuteReader())
                    //{
                    //    if (reader.Read()) // Intenta leer un registro del resultado
                    //    {
                    //        idFolioSolicitud = reader["id"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["id"]);
                    //        consecutivo = reader["consecutivo"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["consecutivo"]);
                    //    }
                    //}

                    //if (consecutivo == -1)
                    //    throw new Exception("No se pudo crear el folio ya que no se encontraron registros con los datos de usuario");

                    ////Se incrementa en 1 el consecutivo
                    //consecutivo++;

                    ////Se completa con ceros a la izquierda
                    //string consecutivoConCeros = consecutivo.ToString("D5");
                    //string finalFolio = dependencia == (int)DependenciaEnum.PEC ? "-TTO" : "-TPTE";
                    //string anio2 = "/" + (anio % 100);

                    //string newFolio = $"{abreviaturaMunicipio}{consecutivoConCeros}{anio2}{finalFolio}";

                    ////Se actualiza el consecutivo en la tabla de foliosSolicitud
                    //command = new SqlCommand(@"update foliosSolicitud set consecutivo=@consecutivo where id=@id", connection);
                    //command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = idFolioSolicitud;
                    //command.Parameters.Add(new SqlParameter("@consecutivo", SqlDbType.Int)).Value = consecutivo;
                    //command.CommandType = CommandType.Text;
                    //int rowsAffected = command.ExecuteNonQuery();
                    //if (rowsAffected <= 0)
                    //    throw new Exception("No se pudo crear el folio.");



                    //SqlCommand command2 = new SqlCommand(@"
                    //        update solicitudes set folio=@folio where idSolicitud=@id
                    //                    ", connection);
                    //command2.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = (object)result ?? DBNull.Value;
                    //command2.Parameters.Add(new SqlParameter("@folio", SqlDbType.VarChar)).Value = (object)newFolio ?? DBNull.Value;
                    //command2.CommandType = CommandType.Text;
                    //rowsAffected = command2.ExecuteNonQuery();

                    //if (rowsAffected > 0)
                    //{
                    //    return newFolio;
                    //}
                    //else
                    //{
                    //    throw new Exception("No se pudo crear el folio.");
                    //}

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            consecutivo = reader["FolioConsecutivo"].ToString();
                            model.idDeposito = reader["idDeposito"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDeposito"].ToString());

                    }
                    try
                    {
                        InsertarHistoricoVehiculo((int)model.idVehiculo, (int)model.idDeposito);
                    }


                    catch { }
                }
                
               
                catch (SqlException ex)
                {

                    Console.WriteLine("Error de SQL: " + ex.Message);
                }
                finally
                {
                    // Cerrar la conexión en el bloque finally
                    connection.Close();
                }

                return consecutivo;
            }
        }

        private void InsertarHistoricoVehiculo(int idVehiculo, int idDeposito)
        {
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("usp_InsertaDepositosVehiculos", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetros
                    command.Parameters.Add(new SqlParameter("@TipoEvento", SqlDbType.Int)).Value = 3;
                    command.Parameters.Add(new SqlParameter("@idVehiculo", SqlDbType.Int)).Value = idVehiculo;
                    command.Parameters.Add(new SqlParameter("@idDeposito", SqlDbType.Int)).Value = idDeposito;

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

        private string ObtenerFolioSolicitud(SqlConnection connection, int solicitudId)
        {
            string folioSolicitud = "";
            string query = "SELECT folio FROM solicitudes WHERE idSolicitud = @solicitudId";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.Add(new SqlParameter("@solicitudId", SqlDbType.Int)).Value = solicitudId;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        folioSolicitud = reader["folio"].ToString();
                    }
                }
            }

            return folioSolicitud;
        }

        public int ReturnIdDeposito(int IdSolicitud)
        {
            var result = 0;
            var Query = "Select iddeposito from depositos where idsolicitud=@idSolicitud";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(Query, connection);
                    command.Parameters.AddWithValue("@idSolicitud", IdSolicitud);
                    command.CommandType = CommandType.Text;

                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read()) {
                            result = (int)reader["iddeposito"];
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
        public (string Folio, int? IdVehiculo) ActualizarSolicitud(int? Isol, SolicitudDepositoModel model)

        {
            string folio = "";
            int? idVehiculo = null;

            string strQuery = @"
                                UPDATE solicitudes
                                SET fechaSolicitud = @fechaSolicitud,
                                    idTipoVehiculo = @idTipoVehiculo,
                                    idPropietarioGrua = @idPropietarioGrua,
                                    idServicioRequiere = @idServicioRequiere,
                                    idOficial = @idOficial,
                                    idEvento = @idDescripcionEvento,
                                    idTipoUsuario = @idTipoUsuario,
                                    solicitanteNombre = @nombreUsuario,
                                    solicitanteAp = @apellidoPaternoUsuario,
                                    solicitanteAm = @apellidoMaternoUsuario,
                                    solicitanteColonia = @coloniaUsuario,
                                    solicitanteCalle = @calleUsuario,
                                    solicitanteNumero = @numeroUsuario,
                                    solicitanteTel = @telefonoUsuario,
                                    idEntidad = @idEntidad,
                                    idMunicipio = @idMunicipio,
                                    vehiculoCalle = @calleUbicacion,
                                    vehiculoColonia = @coloniaUbicacion,
                                    vehiculoNumero = @numeroUbicacion,
                                    vehiculoInterseccion = @interseccion,
                                    vehiculoKm = @kilometroUbicacion,
                                    idEntidadUbicacion = @idEntidadUbicacion,
                                    idMunicipioUbicacion = @idMunicipioUbicacion,
                                    idCarreteraUbicacion = @idCarreteraUbicacion,
                                    idTramoUbicacion = @idTramoUbicacion,
                                    idPension = @idPensionUbicacion,
                                    idMotivoAsignacion = @idMotivoAsignacion,
                                    fechaActualizacion = @fechaActualizacion,
                                    actualizadoPor = @actualizadoPor
                                WHERE idSolicitud = @idSolicitud;
                                
                                update depositos set 
                                idPension = @idPensionUbicacion
                                where idSolicitud = @idSolicitud;

                            ";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@idSolicitud", Isol);
                    command.CommandType = CommandType.Text;
                    DateTime fechaHoraSolicitud = new DateTime(
                         model.fechaSolicitud.Year,
                         model.fechaSolicitud.Month,
                         model.fechaSolicitud.Day,
                         model.horaSolicitud.Hours,
                         model.horaSolicitud.Minutes,
                         model.horaSolicitud.Seconds
                     );

                    command.Parameters.Add(new SqlParameter("@fechaSolicitud", SqlDbType.DateTime)).Value = fechaHoraSolicitud;

                    command.Parameters.Add(new SqlParameter("@fechaHoraSolicitud", SqlDbType.DateTime)).Value = (object)fechaHoraSolicitud ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoVehiculo", SqlDbType.Int)).Value = (object)model.idTipoVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idPropietarioGrua", SqlDbType.Int)).Value = (object)model.idConcecionario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idServicioRequiere", SqlDbType.Int)).Value = (object)model.idServicioRequiere ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = (object)model.idOficial ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idDescripcionEvento", SqlDbType.Int)).Value = (object)model.idDescripcionEvento ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoUsuario", SqlDbType.Int)).Value = (object)model.idTipoUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@nombreUsuario", SqlDbType.NVarChar)).Value = (object)model.nombreUsuario.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoPaternoUsuario", SqlDbType.NVarChar)).Value = (object)model.apellidoPaternoUsuario.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoMaternoUsuario", SqlDbType.NVarChar)).Value = (object)model.apellidoMaternoUsuario.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@coloniaUsuario", SqlDbType.NVarChar)).Value = (object)model.coloniaUsuario.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@calleUsuario", SqlDbType.NVarChar)).Value = (object)model.calleUsuario.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@numeroUsuario", SqlDbType.NVarChar)).Value = (object)model.numeroUsuario.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)model.idEntidad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)model.idMunicipio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@telefonoUsuario", SqlDbType.NVarChar)).Value = (object)model.telefonoUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMotivoAsignacion", SqlDbType.Int)).Value = (object)model.idMotivoAsignacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;

                    command.Parameters.Add(new SqlParameter("@idEntidadUbicacion", SqlDbType.Int)).Value = (object)model.idEntidadUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipioUbicacion", SqlDbType.Int)).Value = (object)model.idMunicipioUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idCarreteraUbicacion", SqlDbType.Int)).Value = (object)model.IdCarretera ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTramoUbicacion", SqlDbType.Int)).Value = (object)model.IdTramo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idPensionUbicacion", SqlDbType.Int)).Value = (object)model.idPensionUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@calleUbicacion", SqlDbType.NVarChar)).Value = (object)model.calleUbicacion.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@coloniaUbicacion", SqlDbType.NVarChar)).Value = (object)model.coloniaUbicacion.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@numeroUbicacion", SqlDbType.NVarChar)).Value = (object)model.numeroUbicacion.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@interseccion", SqlDbType.NVarChar)).Value = (object)model.interseccion.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@kilometroUbicacion", SqlDbType.NVarChar)).Value = (object)model.kilometroUbicacion.ToUpper() ?? DBNull.Value;

                    command.ExecuteNonQuery();
                    command = new SqlCommand(@"
                        SELECT s.folio, d.idVehiculo FROM solicitudes s
                        LEFT JOIN depositos d ON d.idSolicitud = s.idSolicitud
                        WHERE s.idSolicitud = @idSolicitud",
                        connection);
                    command.Parameters.AddWithValue("@idSolicitud", Isol);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read()) // Verificar si se obtuvo un resultado
                    {
                        // Obtener el folio del registro actualizado
                        folio = reader["folio"].ToString();
                        idVehiculo = reader["idVehiculo"] == DBNull.Value ? null : Convert.ToInt32(reader["idVehiculo"].ToString());
                    }
                    reader.Close(); // Cerrar el lector

                    SqlCommand command2 = new SqlCommand(@"
                            update depositos SET 
                                        idConcesionario = @idPropietarioGrua, fechaActualizacion = @fechaActualizacion,
                                        estatusSolicitud = CASE 
                                                                when estatusSolicitud>0 then estatusSolicitud
                                                                WHEN ISNULL(@idPropietarioGrua, 0) = 0 THEN 2 ELSE 1 END 
                            ,depenInterseccion =@interseccion                                            
                            WHERE idSolicitud = @idSolicitud;
                                        ", connection);
                    command2.Parameters.AddWithValue("@idSolicitud", Isol);
                    command2.Parameters.AddWithValue("@interseccion", model.interseccion.ToUpper() ?? "");
                    command2.Parameters.Add(new SqlParameter("@idPropietarioGrua", SqlDbType.Int)).Value = (object)model.idConcecionario ?? DBNull.Value;
                    command2.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

                    command2.CommandType = CommandType.Text;
                    command2.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    // Manejar excepciones si es necesario
                    return (folio, idVehiculo); // O regresar idActual
                }
                finally
                {
                    connection.Close();
                }
            }
            return (folio, idVehiculo);
        }

        public SolicitudDepositoModel ObtenerSolicitudPorID(int Isol)
        {
            SolicitudDepositoModel solicitud = new SolicitudDepositoModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT sol.idSolicitud,
                                    sol.folio, 
                                    sol.fechaSolicitud, 
                                    sol.idInfraccion,
                                    sol.idTipoVehiculo, 
                                    sol.idPropietarioGrua, 
                                    sol.idOficial, 
                                    sol.idEntidad, 
                                    sol.idTipoUsuario, 
                                    sol.solicitanteNombre, 
                                    sol.solicitanteAp, 
                                    sol.solicitanteAm, sol.solicitanteNumero, 
                                    sol.solicitanteColonia, sol.solicitanteCalle, 
                                    sol.idEntidad, sol.idMunicipio, 
                                    sol.solicitanteTel,sol.idEvento,
                                    sol.idMotivoAsignacion, 
                                    ctv.tipoVehiculo,sol.idServicioRequiere,
                                    propg.responsable, 
                                    ofi.nombre, ofi.apellidoPaterno, ofi.apellidoMaterno, 
                                   dev.descripcionEvento, 
                                    tu.tipoUsuario, 
                                    ent.nombreEntidad, 
                                    mun.municipio,
									sol.vehiculoCalle,sol.vehiculoColonia,
                                    sol.vehiculoNumero,sol.idCarreteraUbicacion,sol.idTramoUbicacion,
									sol.idEntidadUbicacion,sol.idMunicipioUbicacion,sol.vehiculoInterseccion,sol.vehiculoKm, sol.idPension
                            FROM solicitudes AS sol 
                            LEFT JOIN catTiposVehiculo AS ctv ON sol.idTipoVehiculo = ctv.idTipoVehiculo 
                            LEFT JOIN catResponsablePensiones AS propg ON sol.idPropietarioGrua = propg.idResponsable 
                            LEFT JOIN catOficiales AS ofi ON sol.idOficial = ofi.idOficial 
                            LEFT JOIN catDescripcionesEvento as dev ON sol.idEvento = dev.idDescripcion 
                            LEFT JOIN catTiposUsuario AS tu ON sol.idTipoUsuario = tu.idTipoUsuario 
                            LEFT JOIN catEntidades AS ent ON sol.idEntidad = ent.idEntidad 
                            LEFT JOIN catMunicipios AS mun ON sol.idMunicipio = mun.idMunicipio 
                            WHERE sol.idSolicitud = @Isol", connection);
                    command.Parameters.Add(new SqlParameter("@Isol", SqlDbType.Int)).Value = Isol;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            solicitud.idSolicitud = reader["idSolicitud"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idSolicitud"].ToString());
                            solicitud.folio = reader["folio"].ToString();
                            solicitud.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());    
                            solicitud.fechaSolicitud = Convert.ToDateTime(reader["fechaSolicitud"].ToString());
                            solicitud.horaSolicitud = solicitud.fechaSolicitud.TimeOfDay;
                            solicitud.idTipoVehiculo = reader["idTipoVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTipoVehiculo"].ToString());
                            solicitud.idServicioRequiere = reader["idServicioRequiere"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idServicioRequiere"].ToString());
                            solicitud.tipoVehiculo = reader["tipoVehiculo"].ToString();
                            solicitud.idConcecionario = reader["idPropietarioGrua"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPropietarioGrua"].ToString());
                            solicitud.propietarioGrua = reader["responsable"].ToString();
                            solicitud.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
                            solicitud.oficial = String.Concat(reader["nombre"].ToString(), " ", reader["apellidoPaterno"].ToString(), " ", reader["apellidoMaterno"].ToString());
                            solicitud.idDescripcionEvento = reader["idEvento"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEvento"].ToString());
                            solicitud.descripcionEvento = reader["descripcionEvento"].ToString();
                            solicitud.idTipoUsuario = reader["idTipoUsuario"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTipoUsuario"].ToString());
                            solicitud.tipoUsuario = reader["tipoUsuario"].ToString();
                            solicitud.nombreUsuario = reader["solicitanteNombre"].ToString();
                            solicitud.apellidoPaternoUsuario = reader["solicitanteAp"].ToString();
                            solicitud.apellidoMaternoUsuario = reader["solicitanteAm"].ToString();
                            solicitud.usuarioCompleto = String.Concat(reader["solicitanteNombre"].ToString(), " ", reader["solicitanteAp"].ToString(), " ", reader["solicitanteAm"].ToString());
                            solicitud.numeroUsuario = reader["solicitanteNumero"].ToString();
                            solicitud.coloniaUsuario = reader["solicitanteColonia"].ToString();
                            solicitud.calleUsuario = reader["solicitanteCalle"].ToString();
                            solicitud.idEntidad = reader["idEntidad"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEntidad"].ToString());
                            solicitud.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
                            solicitud.entidad = reader["nombreEntidad"].ToString();
                            solicitud.municipio = reader["municipio"].ToString();
                            solicitud.telefonoUsuario = reader["solicitanteTel"].ToString();
                            solicitud.idMotivoAsignacion = reader["idMotivoAsignacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMotivoAsignacion"].ToString());
                            solicitud.idEntidadUbicacion = reader["idEntidadUbicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEntidadUbicacion"].ToString());

                            solicitud.idPensionUbicacion = reader["idPension"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPension"].ToString());
                            solicitud.idMunicipioUbicacion = reader["idMunicipioUbicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipioUbicacion"].ToString());
                            solicitud.IdCarretera = reader["idCarreteraUbicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarreteraUbicacion"].ToString());
                            solicitud.IdTramo = reader["idTramoUbicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramoUbicacion"].ToString());
                            solicitud.calleUbicacion = reader["vehiculoCalle"].ToString();
                            solicitud.coloniaUbicacion = reader["vehiculoColonia"].ToString();
                            solicitud.numeroUbicacion = reader["vehiculoNumero"].ToString();
                            solicitud.kilometroUbicacion = reader["vehiculoKm"].ToString();
                            solicitud.interseccion = reader["vehiculoInterseccion"].ToString();


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

            return solicitud;
        }

        public int CompletarSolicitud(SolicitudDepositoModel model)

        {
            int resultado = 0;
            string strQuery = @"
                                UPDATE solicitudes
                                SET vehiculoNumero = @numeroUbicacion,
                                    vehiculoCalle = @calleUbicacion,
                                    vehiculoColonia = @coloniaUbicacion,
                                    vehiculoKm = @kilometroUbicacion,
                                    idCarreteraUbicacion = @idCarretera,
                                    idTramoUbicacion = @idTramo,
                                    idEntidadUbicacion = @idEntidadUbicacion,
                                    idMunicipioUbicacion = @idMunicipioUbicacion,
                                    idPension = @idPensionUbicacion,
                                    vehiculoInterseccion = @interseccion,
                                    fechaActualizacion = @fechaActualizacion,
                                    actualizadoPor = @actualizadoPor,
                                    estatus = @estatus
                                WHERE idSolicitud = @idSolicitud;
                            ";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@idSolicitud", model.idSolicitud);
                    command.CommandType = CommandType.Text;

                    command.Parameters.Add(new SqlParameter("@numeroUbicacion", SqlDbType.NVarChar)).Value = (object)model.numeroUbicacion.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@calleUbicacion", SqlDbType.NVarChar)).Value = (object)model.calleUbicacion.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@coloniaUbicacion", SqlDbType.NVarChar)).Value = (object)model.coloniaUbicacion.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@kilometroUbicacion", SqlDbType.NVarChar)).Value = (object)model.kilometroUbicacion.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@interseccion", SqlDbType.NVarChar)).Value = (object)model.interseccion.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = (object)model.IdCarretera ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = (object)model.IdTramo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idEntidadUbicacion", SqlDbType.Int)).Value = (object)model.idEntidadUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipioUbicacion", SqlDbType.Int)).Value = (object)model.idMunicipioUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idPensionUbicacion", SqlDbType.Int)).Value = (object)model.idPensionUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return resultado;
                }
                finally
                {
                    connection.Close();
                }
            }
            return resultado;
        }

        public CatalogModel GetPathFile(string folio)
        {

            var result = new CatalogModel();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "select archivoInventario as result,nombreArchivo as filename , idInfraccion from  depositos  where idDeposito = @Infraccion";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@Infraccion", folio);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {

                        while (reader.Read())
                        {
                            result.value = reader["result"] is DBNull ? "" :(string)reader["result"];
                            result.text = reader["result"] is DBNull ? "" : (string)reader["filename"];
                            result.aux = reader["idInfraccion"] is DBNull ? "" : ((int)reader["idInfraccion"]).ToString();
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

        public SolicitudDepositoModel ImportarInfraccion(string folioBusquedaInfraccion,int idDependencia)
        {
            SolicitudDepositoModel model = new SolicitudDepositoModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    string SqlTransact =
                                           @"SELECT TOP 1 inf.idInfraccion
                                                    ,inf.fechainfraccion
                                                    ,inf.horaInfraccion
                                                    ,inf.idOficial
                                                    ,inf.idDependencia
                                                    ,inf.idDelegacion
                                                    ,inf.archivoInventario
                                                    ,inf.nombreArchivo
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
                                                    ,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,catGar.idGarantia,catGar.garantia
                                                    ,estIn.idEstatusInfraccion, estIn.estatusInfraccion
                                                    ,tipoP.idTipoPlaca, tipoP.tipoPlaca
                                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                                    ,catMun.municipio
                                                    ,catTra.idTramo,catTra.tramo
                                                    ,per.RFC,per.fechaNacimiento
                                                    ,catCarre.idCarretera,catCarre.carretera
                                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico
                                                    ,catEntidad.idEntidad as idEntidadUbicacion
                                                    ,(select top 1 cva.idPension from conductoresVehiculosAccidente cva left join infraccionesAccidente ia on cva.idAccidente=ia.idAccidente where ia.idInfraccion=inf.idInfraccion  order by cva.fechaActualizacion  desc) as idPensionUbicacion
                                                    ,(select top 1 e.idDescripcion  from depositos d left join solicitudes s on d.idSolicitud=s.idSolicitud left join catDescripcionesEvento e on s.idEvento=e.idDescripcion  left join infracciones i on d.idInfraccion=i.idInfraccion where i.idInfraccion=inf.idInfraccion order by d.idDeposito desc) as idDescripcionEvento
                                                    FROM infracciones as inf
                                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                                    left join catDelegacionesOficinasTransporte	del on inf.idDelegacion = del.idOficinaTransporte
                                                    left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
                                                    left join catGarantias catGar on inf.idGarantia = catGar.idGarantia
                                                    left join garantiasInfraccion gar on catGar.idGarantia= gar.idCatGarantia and inf.idInfraccion=gar.idInfraccion
                                                    left join catTipoPlaca  tipoP on gar.idTipoPlaca=tipoP.idTipoPlaca
                                                    left join catTipoLicencia tipoL on tipoL.idTipoLicencia= gar.idTipoLicencia
                                                    left join catOficiales catOfi on inf.idOficial = catOfi.idOficial
                                                    left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                                    left join catEntidades catEntidad on catMun.idEntidad=catEntidad.idEntidad
                                                    left join catTramos catTra on inf.idTramo = catTra.idTramo
                                                    left join catCarreteras catCarre on catTra.IdCarretera = catCarre.idCarretera
                                                    left join vehiculos veh on inf.idVehiculo = veh.idVehiculo
                                                    left join personas per on inf.idPersona = per.idPersona
                                                    WHERE inf.folioInfraccion=@folioInfraccion AND inf.transito = @idDependencia ORDER BY inf.fechaInfraccion desc";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@folioInfraccion", SqlDbType.NVarChar)).Value = folioBusquedaInfraccion;
                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.VarChar)).Value = idDependencia;
					command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
                            model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idVehiculo"].ToString());

                            model.fechaSolicitud = reader["fechaInfraccion"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["fechaInfraccion"]);
                            model.horaSolicitud = reader["horaInfraccion"] == DBNull.Value ? TimeSpan.MinValue : TimeSpan.Parse(reader["horaInfraccion"].ToString());
                            model.horaSolicitudStr = reader["horaInfraccion"] == System.DBNull.Value ? string.Empty : reader["horaInfraccion"].ToString();

                            model.idMunicipioUbicacion = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
                            model.IdTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
                            model.IdCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
                            model.kilometroUbicacion = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
                            model.calleUbicacion = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
                            model.numeroUbicacion = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
                            model.coloniaUbicacion = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
                            model.interseccion = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
                            
                            model.pathArchivo = reader["archivoInventario"] == System.DBNull.Value ? string.Empty : reader["archivoInventario"].ToString();
                            model.NombreArchivo = reader["nombreArchivo"] == System.DBNull.Value ? string.Empty : reader["nombreArchivo"].ToString();
                           
                            model.folioInfraccion = reader["folioInfraccion"] == System.DBNull.Value ? string.Empty : reader["folioInfraccion"].ToString();
                            model.municipio = reader["municipio"].ToString();
                            model.idEntidadUbicacion = reader["idEntidadUbicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEntidadUbicacion"].ToString());
                            model.idPensionUbicacion = reader["idPensionUbicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPensionUbicacion"].ToString());
                            model.idTipoVehiculo = reader["idTipoVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTipoVehiculo"].ToString());
                            model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
                            model.idDescripcionEvento = reader["idDescripcionEvento"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDescripcionEvento"].ToString());
                        }
                    }
                }
                catch (SqlException ex)
                {
                    //Guardar la excepcion en algun log de errores
                    Logger.Error("Error al obtener infracción por folio: " + ex);
                }
                finally
                {
                    connection.Close();
                }
            return model;
        }



        public SolicitudDepositoModel ImportarInfraccion(int folioBusquedaInfraccion, int idDependencia)
        {
            SolicitudDepositoModel model = new SolicitudDepositoModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    string SqlTransact =
                                           @"SELECT TOP 1 inf.idInfraccion
                                                    ,inf.horaInfraccion
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
                                                    ,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,catGar.idGarantia,catGar.garantia
                                                    ,estIn.idEstatusInfraccion, estIn.estatusInfraccion
                                                    ,tipoP.idTipoPlaca, tipoP.tipoPlaca
                                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                                    ,catMun.idMunicipio,catMun.municipio
                                                    ,catTra.idTramo,catTra.tramo
                                                    ,per.RFC,per.fechaNacimiento
                                                    ,catCarre.idCarretera,catCarre.carretera
                                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico
                                                    ,catEntidad.idEntidad as idEntidadUbicacion
                                                    ,(select top 1 cva.idPension from conductoresVehiculosAccidente cva left join infraccionesAccidente ia on cva.idAccidente=ia.idAccidente where ia.idInfraccion=inf.idInfraccion  order by cva.fechaActualizacion  desc) as idPensionUbicacion
                                                    ,(select top 1 e.idDescripcion  from depositos d left join solicitudes s on d.idSolicitud=s.idSolicitud left join catDescripcionesEvento e on s.idEvento=e.idDescripcion  left join infracciones i on d.idInfraccion=i.idInfraccion where i.idInfraccion=inf.idInfraccion order by d.idDeposito desc) as idDescripcionEvento
                                                    FROM infracciones as inf
                                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                                    left join catDelegacionesOficinasTransporte	del on inf.idDelegacion = del.idOficinaTransporte
                                                    left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
                                                    left join catGarantias catGar on inf.idGarantia = catGar.idGarantia
                                                    left join garantiasInfraccion gar on catGar.idGarantia= gar.idCatGarantia and inf.idInfraccion=gar.idInfraccion
                                                    left join catTipoPlaca  tipoP on gar.idTipoPlaca=tipoP.idTipoPlaca
                                                    left join catTipoLicencia tipoL on tipoL.idTipoLicencia= gar.idTipoLicencia
                                                    left join catOficiales catOfi on inf.idOficial = catOfi.idOficial
                                                    left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                                    left join catEntidades catEntidad on catMun.idEntidad=catEntidad.idEntidad
                                                    left join catTramos catTra on inf.idTramo = catTra.idTramo
                                                    left join catCarreteras catCarre on catTra.IdCarretera = catCarre.idCarretera
                                                    left join vehiculos veh on inf.idVehiculo = veh.idVehiculo
                                                    left join personas per on inf.idPersona = per.idPersona
                                                    WHERE inf.idInfraccion=@folioInfraccion AND inf.transito = @idDependencia ORDER BY inf.fechaInfraccion desc";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@folioInfraccion", SqlDbType.Int)).Value = folioBusquedaInfraccion;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = idDependencia;

					command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
                            model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idVehiculo"].ToString());
                            model.fechaSolicitud = reader["fechaInfraccion"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["fechaInfraccion"]);
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

                            model.horaSolicitud = horaInfraccionTimeSpan;
                            model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
                            model.idMunicipioUbicacion = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
                            model.IdTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
                            model.IdCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
                            model.kilometroUbicacion = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
                            model.calleUbicacion = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
                            model.numeroUbicacion = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
                            model.coloniaUbicacion = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
                            model.interseccion = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
                            model.folioInfraccion = reader["folioInfraccion"] == System.DBNull.Value ? string.Empty : reader["folioInfraccion"].ToString();
                            model.municipio = reader["municipio"].ToString();
                            model.idEntidadUbicacion = reader["idEntidadUbicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEntidadUbicacion"].ToString());
                            model.idPensionUbicacion = reader["idPensionUbicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPensionUbicacion"].ToString());
                            model.idTipoVehiculo = reader["idTipoVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTipoVehiculo"].ToString());
                            model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
                            model.idDescripcionEvento = reader["idDescripcionEvento"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDescripcionEvento"].ToString());
                        }
                    }
                }
                catch (SqlException ex)
                {
                    //Guardar la excepcion en algun log de errores
                    Logger.Error("Error al obtener infracción por folio: " + ex);
                }
                finally
                {
                    connection.Close();
                }
            return model;
        }


        public List<SolicitudDepositoModel> ObtenerServicios()

        {
            //
            List<SolicitudDepositoModel> ListaServicios = new List<SolicitudDepositoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT sr.idServicio,sr.nombreServicio, e.estatusdesc 
                                                          FROM catServicioRequiere AS sr 
                                                          LEFT JOIN estatus AS e ON sr.estatus = e.estatus
                                                            WHERE sr.estatus = 1;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            SolicitudDepositoModel servicio = new SolicitudDepositoModel();
                            servicio.idServicioRequiere = reader["idServicio"] != DBNull.Value ? Convert.ToInt32(reader["idServicio"]) : 0;
                            servicio.servicioRequiere = reader["nombreServicio"] != DBNull.Value ? reader["nombreServicio"].ToString() : string.Empty;


                            ListaServicios.Add(servicio);
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
            return ListaServicios;


        }

        public void CambiarDepositoVehiculo(int idDeposito, int idSolicitud, int idVehiculo)
        {
            
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open();
                using (SqlTransaction trx = connection.BeginTransaction())
                {
                    try
                    {

                        Vehiculo vehiculo = GetVehiculoById(idVehiculo, connection, trx);                        
                        UpdateVehiculoDeDeposito(idDeposito, vehiculo, connection, trx);

                        ExecuteActionIgnoreException(() => _vehiculosService.CrearHistoricoVehiculo(idDeposito, idVehiculo, 3));

                        InsertarDepositosPersonas(idDeposito, vehiculo.IdPersona, connection, trx);

                        trx.Commit();

                    }
                    catch (Exception)
                    {
                        trx.Rollback();
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                
            }
        }

        public DepositosModel GetDepositoByFolioSolicitud(string folioSolicitud)
        {
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            d.idDeposito,
                            d.idSolicitud
                        FROM depositos d
                        INNER JOIN solicitudes s ON s.idSolicitud = d.idSolicitud
                        WHERE s.folio = @folioSolicitud";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@folioSolicitud", folioSolicitud);

                    SqlDataReader reader = command.ExecuteReader();
                    if (!reader.Read())
                    {
                        reader.Close();
                        return null;
                    }

                    
                    var idDeposito = Convert.ToInt32(reader["idDeposito"].ToString());
                    var idSolcitud = Convert.ToInt32(reader["idSolicitud"].ToString());
                    
                    reader.Close();

                    return new DepositosModel { IdDeposito = idDeposito, IdSolicitud = idSolcitud };
                   

                }
                catch (Exception ex)
                {
                    // manejar la excepción aquí, si es necesario
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        #region Private methods

        private Vehiculo GetVehiculoById(int idVehiculo, SqlConnection connection, SqlTransaction trx = null)
        {
            string query = @"
                SELECT 
                    idVehiculo,
                    placas,
                    serie,
                    idMarcaVehiculo,
                    idSubmarca,
                    idColor,
                    idTipoVehiculo,
                    idPersona 
                FROM vehiculos 
                WHERE idVehiculo = @idVehiculo";

            using var command = new SqlCommand(query, connection, trx);
            command.Parameters.AddWithValue("@idVehiculo", idVehiculo);
            
            using SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read()) return null;

            string placa = reader["placas"].ToString();
            string serie = reader["serie"].ToString();
            int? idMarca = reader["idMarcaVehiculo"] is DBNull ? null : (int)reader["idMarcaVehiculo"];
            int? idSubmarca = reader["idSubmarca"] is DBNull ? null : (int)reader["idSubmarca"]; 
            int? idColor = reader["idColor"] is DBNull ? null : (int)reader["idColor"];
            int? idTipoVehiculo = reader["idTipoVehiculo"] is DBNull ? null : (int)reader["idTipoVehiculo"];
            int idPersona = reader["idPersona"] is DBNull ? 133 : (int)reader["idPersona"];

            reader.Close();
            return new Vehiculo 
                {
                    IdVehiculo = idVehiculo,
                    Placa = placa,
                    Serie = serie,
                    IdMarca = idMarca,
                    IdSubmarca = idSubmarca,
                    IdColor = idColor,
                    IdTipoVehiculo = idTipoVehiculo,
                    IdPersona = idPersona
                };
        }

        private int UpdateVehiculoDeDeposito(int idDeposito, Vehiculo vehiculo, SqlConnection connection, SqlTransaction trx)
        {
            string query = @"
                UPDATE depositos
                SET idVehiculo = @idVehiculo,
                    placa = @placa,
                    serie = @serie,                    
                    idMarca = @idMarca,
                    idSubmarca = @idSubmarca,
                    idColor = @idColor,
                    idPropietario = @idPropietario,
                    fechaActualizacion = @fechaActualizacion
                WHERE idDeposito = @idDeposito";

            using var command = new SqlCommand(query, connection, trx);
            command.Parameters.AddWithValue("@idDeposito", idDeposito);
            command.Parameters.AddWithValue("@idVehiculo", vehiculo.IdVehiculo);
            command.Parameters.AddWithValue("@placa", vehiculo.Placa?.ToUpper());
            command.Parameters.AddWithValue("@serie", vehiculo.Serie?.ToUpper());
            command.Parameters.AddWithValue("@idMarca", vehiculo.IdMarca);            
            command.Parameters.AddWithValue("@idSubmarca", vehiculo.IdSubmarca);
            command.Parameters.AddWithValue("@idColor", vehiculo.IdColor);
            command.Parameters.AddWithValue("@idPropietario", vehiculo.IdPersona);
            command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

            return command.ExecuteNonQuery();
        }

        private int UpdateVehiculoDeSolcitiud(int idSolicitud, Vehiculo vehiculo, SqlConnection connection, SqlTransaction trx)
        {
            string query = @"
                UPDATE solicitudes
                SET idVehiculo = @idVehiculo,
                    idTipoVehiculo = @idTipoVehiculo,
                    fechaActualizacion = @fechaActualizacion
                WHERE idSolicitud = @idSolicitud";

            using var command = new SqlCommand(query, connection, trx);
            command.Parameters.AddWithValue("@idSolicitud", idSolicitud);
            command.Parameters.AddWithValue("@idVehiculo", vehiculo.IdVehiculo);
            command.Parameters.AddWithValue("@idTipoVehiculo", vehiculo.IdTipoVehiculo);
            command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

            return command.ExecuteNonQuery();
        }

        private void InsertarDepositosPersonas(int idDeposito, int idPersona, SqlConnection connection, SqlTransaction trx = null)
        {
            using var command = new SqlCommand("usp_InsertaDepositosPersonas", connection, trx);
            command.CommandType = CommandType.StoredProcedure;

            // Agregar parámetros
            command.Parameters.Add(new SqlParameter("@TipoPersonaOrigen", SqlDbType.Int)).Value = 2;
            command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = idPersona;
            command.Parameters.Add(new SqlParameter("@idDeposito", SqlDbType.Int)).Value = idDeposito;

            // Ejecutar el procedimiento almacenado
            command.ExecuteNonQuery();
        }

        private void ExecuteActionIgnoreException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                // ignore
            }
        }


        #endregion Private methods
    }
}
