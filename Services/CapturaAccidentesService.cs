using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Exceptions;
using GuanajuatoAdminUsuarios.ExternalServices.Interfaces;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Interfaces.Files;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Models.Files;
using GuanajuatoAdminUsuarios.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Services
{

	public class CapturaAccidentesService : ICapturaAccidentesService
	{
		private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
		private readonly IVehiculosService _vehiculosService;
		private readonly ICadService _cadService;
        private readonly IAccidenteFileManager _accidenteFileManager;
        private readonly DBContextInssoft dbContext;
		
		public CapturaAccidentesService(
			ISqlClientConnectionBD sqlClientConnectionBD,
			IVehiculosService vehiculosService,
			IAccidenteFileManager accidenteFileManager,
			ICadService cadService)
		{
			_sqlClientConnectionBD = sqlClientConnectionBD;
			_vehiculosService = vehiculosService;
			_cadService = cadService;
			_accidenteFileManager = accidenteFileManager;
			dbContext = new DBContextInssoft();
		}

		public int AgregarAccidenteFolioDetencion(FechaHoraIngresoModel model, int idAccidente)
		{
			int result = 0;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();

					string queryExistenciaInvolucrados = "SELECT idInvolucradosAccidente FROM involucradosAccidente WHERE idAccidente = @idAccidente and idPersona = @idPersona";
					SqlCommand commandExistenciaInvolucrados = new SqlCommand(queryExistenciaInvolucrados, connection);
					commandExistenciaInvolucrados.Parameters.AddWithValue("@idAccidente", idAccidente);
					commandExistenciaInvolucrados.Parameters.AddWithValue("@idPersona", model.IdPersona);
					int idInvolucradosAccidente = (int)commandExistenciaInvolucrados.ExecuteScalar();

					if (idInvolucradosAccidente > 0)
					{
						string queryExistenciaDetenidos = "SELECT detenidosId FROM accidentesDetenidos WHERE idInvolucradosAccidente = @idInvolucradosAccidente";
						SqlCommand commandExistenciaDetenidos = new SqlCommand(queryExistenciaDetenidos, connection);
						commandExistenciaDetenidos.Parameters.AddWithValue("@idInvolucradosAccidente", idInvolucradosAccidente);
						var idDetenidosString = commandExistenciaDetenidos.ExecuteScalar();
						long idDetenidos = 0;

						if (idDetenidosString is not null)
						{
							idDetenidos = (long)idDetenidosString;
						}

						if (idDetenidos > 0)
						{
							SqlCommand sqlCommand = new
								SqlCommand(@"UPDATE accidentesDetenidos SET idPersona = @idPersona, folioDetenido = @folioDetenido, 
                                    fechaActualizacion = fechaActualizacion WHERE  detenidosId = @detenidosId;",
								connection);
							sqlCommand.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = model.IdPersona;
							sqlCommand.Parameters.Add(new SqlParameter("@folioDetenido", SqlDbType.NVarChar)).Value = model.FolioDetencionStr?.ToUpper();
							sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
							sqlCommand.Parameters.Add(new SqlParameter("@detenidosId", SqlDbType.BigInt)).Value = idDetenidos;

							sqlCommand.CommandType = CommandType.Text;
							result = sqlCommand.ExecuteNonQuery();
						}
						else
						{
							SqlCommand sqlCommand = new
								SqlCommand(@"INSERT INTO accidentesDetenidos (idInvolucradosAccidente, idPersona, folioDetenido, fechaActualizacion) 
                                        VALUES(@idInvolucradosAccidente, @idPersona, @folioDetenido, @fechaActualizacion);",
								connection);
							sqlCommand.Parameters.Add(new SqlParameter("@idInvolucradosAccidente", SqlDbType.Int)).Value = idInvolucradosAccidente;
							sqlCommand.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = model.IdPersona;
							sqlCommand.Parameters.Add(new SqlParameter("@folioDetenido", SqlDbType.NVarChar)).Value = model.FolioDetencionStr?.ToUpper();
							sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;

							sqlCommand.CommandType = CommandType.Text;
							result = sqlCommand.ExecuteNonQuery();
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
			}
			return result;
		}


		public int GuardarComplementoVehiculo(CapturaAccidentesModel model, int IdVehiculo, int idAccidente, string numerosEconomicos, int idConcesionario, string idGruas)
		{
			int result = 0;
			string strQuery = @"
                                IF EXISTS (SELECT 1 FROM conductoresVehiculosAccidente WHERE idVehiculo = @IdVehiculo AND idAccidente = @IdAccidente)
                                    UPDATE conductoresVehiculosAccidente
                                    SET idTipoCarga = @IdTipoCarga,
                                        poliza = @Poliza,
                                        idDelegacion = @IdDelegacion,
                                        idPension = @IdPension,
                                        idFormaTraslado = @IdFormaTraslado,
                                        fechaActualizacion = getdate(),
                                        actualizadoPor = @actualizadoPor,
                                        estatus = @estatus,
                                        idGruas = @gruas,
                                        numerosEconomicos = @numerosEconomicos,
                                        idConcesionario = @idConcesionario
                                    WHERE idVehiculo = @IdVehiculo AND idAccidente = @IdAccidente;";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@IdTipoCarga", SqlDbType.Int)).Value = (object)model.IdTipoCarga ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@Poliza", SqlDbType.NVarChar)).Value = (object)model.Poliza?.ToUpper() ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdDelegacion", SqlDbType.Int)).Value = (object)model.IdDelegacion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdPension", SqlDbType.Int)).Value = (object)model.IdPension ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idConcesionario", SqlDbType.Int)).Value = (object)idConcesionario ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@gruas", SqlDbType.NVarChar)).Value = (object)idGruas;
					command.Parameters.Add(new SqlParameter("@numerosEconomicos", SqlDbType.NVarChar)).Value = (object)numerosEconomicos?.ToUpper();
					command.Parameters.Add(new SqlParameter("@IdFormaTraslado", SqlDbType.Int)).Value = (object)model.IdFormaTraslado ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdVehiculo", SqlDbType.Int)).Value = (object)IdVehiculo ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idGruas", SqlDbType.NVarChar)).Value = (object)idGruas;
					command.Parameters.Add(new SqlParameter("@IdAccidente", SqlDbType.Int)).Value = (object)idAccidente ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
					command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;

					result = command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					// Manejar la excepción
				}
				finally
				{
					connection.Close();
				}
			}

			return result;
		}


		public List<CapturaAccidentesModel> ObtenerAccidentes(int idOficina)
		{
			//
			List<CapturaAccidentesModel> ListaAccidentes = new List<CapturaAccidentesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"SELECT acc.IdAccidente,acc.NumeroReporte,acc.Fecha,acc.Hora,acc.IdMunicipio,acc.IdCarretera,
                        acc.IdTramo,acc.idEstatusReporte,
                        mun.Municipio, car.Carretera, tra.Tramo, er.estatusReporte,er.idEstatusReporte
                        FROM accidentes AS acc  
                        LEFT JOIN catMunicipios AS mun ON acc.idMunicipio = mun.idMunicipio 
                        LEFT JOIN catCarreteras AS car ON acc.idCarretera = car.idCarretera  
                        LEFT JOIN catTramos AS tra ON acc.idTramo = tra.idTramo  
                        LEFT JOIN catEstatusReporteAccidente AS er ON acc.idEstatusReporte = er.idEstatusReporte
                        WHERE acc.estatus = 1 AND acc.idOficinaDelegacion = @idOficina and acc.idEstatusReporte != 3;", connection);
					command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = idOficina;
					//command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = idDependencia;
					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CapturaAccidentesModel accidente = new CapturaAccidentesModel();
							accidente.IdAccidente = Convert.ToInt32(reader["IdAccidente"].ToString());
							accidente.NumeroReporte = reader["NumeroReporte"].ToString();
							accidente.Fecha = Convert.ToDateTime(reader["Fecha"].ToString());
							accidente.Hora = reader.GetTimeSpan(reader.GetOrdinal("Hora"));
							accidente.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
							accidente.IdCarretera = Convert.ToInt32(reader["IdCarretera"].ToString());
							accidente.IdTramo = Convert.ToInt32(reader["IdTramo"].ToString());
							accidente.idEstatusReporte = Convert.ToInt32(reader["idEstatusReporte"].ToString());
							accidente.EstatusReporte = reader["estatusReporte"].ToString();
							accidente.Municipio = reader["Municipio"].ToString();
							accidente.Tramo = reader["Tramo"].ToString();
							accidente.Carretera = reader["Carretera"].ToString();

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


		public CapturaAccidentesModel ObtenerAccidentePorId(int idAccidente, int idOficina)
		{
			CapturaAccidentesModel accidente = new CapturaAccidentesModel();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"
                        SELECT DISTINCT 
							a.idAccidente,
							a.numeroReporte,
							a.fecha,
							a.hora,
							a.idMunicipio, 
							a.idCarretera, 
							a.idTramo, 
							a.kilometro, 
							a.idClasificacionAccidente, 
							a.idFactorAccidente,
							a.IdFactorOpcionAccidente,
							a.idOficinaDelegacion,
                            a.latitud, a.longitud,
							a.descripcionCausas,
							m.municipio,
							c.carretera,
							t.tramo,
							e.estatusDesc, 
							ac.idCausaAccidente,
							d.nombreOficina,
							d.jefeOficina,
							ae.emergenciasId,
							ae.folioEmergencia,
							a.observacionesTramo,
							ta.idTurno,
							a.urlCroquis,
							a.lugarCalle,
							a.lugarColonia,
							a.lugarNumero
							,a.urlAccidenteArchivosLugar
                        FROM accidentes AS a                         
                        left JOIN catMunicipios AS m ON a.idMunicipio = m.idMunicipio
                        left JOIN catDelegacionesOficinasTransporte as d on d.idOficinaTransporte= a.idOficinaDelegacion
                        left JOIN catCarreteras AS c ON a.idCarretera = c.idCarretera 
                        left JOIN catTramos AS t ON a.idTramo = t.idTramo 
                        left JOIN estatus AS e ON a.estatus = e.estatus 
                        LEFT JOIN accidenteCausas AS ac ON ac.idAccidente = a.idAccidente 
                        LEFT JOIN accidentesEmergencias AS ae on ae.idAccidente = a.idAccidente
						LEFT JOIN turnoAccidentes AS ta ON ta.idAccidente = a.idAccidente
                        WHERE a.idAccidente = @idAccidente AND a.estatus = 1
                    ", connection);
					command.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.Int)).Value = idAccidente;
					// command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = idOficina;
					// command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = idDependencia;

					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							accidente.IdAccidente = Convert.ToInt32(reader["IdAccidente"].ToString());
							accidente.NumeroReporte = reader["NumeroReporte"].ToString();
							accidente.Fecha = Convert.ToDateTime(reader["Fecha"].ToString());
							accidente.Hora = reader.GetTimeSpan(reader.GetOrdinal("Hora"));
							accidente.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
							accidente.IdCarretera = Convert.ToInt32(reader["IdCarretera"].ToString());
							accidente.IdClasificacionAccidente = reader["IdClasificacionAccidente"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["IdClasificacionAccidente"].ToString());
							accidente.IdFactorAccidente = reader["IdFactorAccidente"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["IdFactorAccidente"].ToString());
							accidente.IdFactorOpcionAccidente = reader["IdFactorOpcionAccidente"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["IdFactorOpcionAccidente"].ToString());
							accidente.IdCausaAccidente = reader["idCausaAccidente"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCausaAccidente"].ToString());
							accidente.Municipio = reader["Municipio"].ToString();
							accidente.Tramo = reader["Tramo"].ToString();
							accidente.DescripcionCausa = reader["descripcionCausas"].ToString();
							accidente.Carretera = reader["Carretera"].ToString();
							accidente.Latitud = reader["latitud"].ToString();
							accidente.Longitud = reader["longitud"].ToString();
                            accidente.lugarCalle = reader["lugarCalle"].ToString();
                            accidente.lugarNumero = reader["lugarNumero"].ToString();
                            accidente.lugarColonia = reader["lugarColonia"].ToString();

                            accidente.FolioEmergencia = reader["folioEmergencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["folioEmergencia"].ToString());
							accidente.IdEmergencia = reader["emergenciasId"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["emergenciasId"].ToString());
							accidente.observacionesTramo = reader["observacionesTramo"].ToString();

							var valueDec = 0;
							var km = reader["kilometro"] == System.DBNull.Value ? default(string) : Decimal.Parse((string)reader["kilometro"]).ToString("G29");
							//if (reader["Kilometro"].ToString().Split('.').Length>1)
							//    valueDec = Convert.ToInt32(reader["Kilometro"].ToString().Split('.')[1]);

							//if (valueDec > 0)
							//    km = reader["Kilometro"].ToString().Split('.')[0] + "." + valueDec.ToString();
							//else
							//    km = reader["Kilometro"].ToString().Split('.')[0];

							accidente.Kilometro = km;
							accidente.IdTramo = Convert.ToInt32(reader["IdTramo"].ToString());
							accidente.DelegacionOficina = reader["nombreOficina"].ToString();
							accidente.jefeOficina = reader["jefeOficina"].ToString();
							accidente.IdTurno = reader["idTurno"] == System.DBNull.Value ? default(long?) : Convert.ToInt64(reader["idTurno"].ToString());
							accidente.Fecha = accidente.Fecha.Value.Add(accidente.Hora.Value);
							var tt = accidente.Hora.Value.ToString(@"hh\:mm", new CultureInfo("en-US"));
							var urlCroquis = reader["urlCroquis"].ToString();
							IFormFile archivoCroquis = null;
							if (!string.IsNullOrEmpty(urlCroquis))
							{
								var filePath = Path.GetFullPath(urlCroquis);

								if (File.Exists(filePath))
								{
									try
									{
										using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
										{
											if (fileStream.Length > 0)
											{
												archivoCroquis = new FormFile(fileStream, 0, fileStream.Length, urlCroquis, Path.GetFileName(filePath))
												{
													Headers = new HeaderDictionary(), // Inicializar Headers
													ContentType = "application/octet-stream" // Establecer un ContentType predeterminado
												};
											}
											else
											{
												Console.WriteLine("El archivo está vacío.");
											}
										}
									}
									catch (IOException ex)
									{
										Console.WriteLine($"Error al abrir el archivo: {ex.Message}");
									}
								}
								else
								{
									Console.WriteLine("El archivo no existe en la ruta especificada.");
								}
							}
							if (archivoCroquis != null)
							{
								var fileName = archivoCroquis.FileName; // Verifica si esta propiedad está funcionando
								var length = archivoCroquis.Length; // Verifica si esta propiedad está funcionando
								Console.WriteLine($"FileName: {fileName}, Length: {length}");
								var contentDisposition = archivoCroquis.ContentDisposition;
							}
							else
							{
								Console.WriteLine("ArchivoCroquis es null");
							}
							accidente.archivoCroquis = archivoCroquis;
							accidente.archivoCroquisPath = urlCroquis;
							accidente.archivoLugarPath = reader["urlAccidenteArchivosLugar"].ToString();

						}
					}
				}
				catch (Exception ex)
				{
					var tt = ex;
				}
				finally
				{
					connection.Close();
				}

			return accidente;
		}
		public async Task<bool> GuardarAccidenteEmergencia(int idMunicipio, int idAccidente, int idOficina, int? folioEmergencia)
		{
			AccidentesEmergencia accidenteEmergencia = new()
			{
				Oficina = idOficina,
				Delegacion = idOficina,
				Municipio = idMunicipio,
				IdAccidente = idAccidente,
				FolioEmergencia = folioEmergencia.ToString().ToUpper()
			};
			dbContext.AccidentesEmergencias.Add(accidenteEmergencia);
			try
			{
				await dbContext.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				Logger.Error($"Error al guardar AccidenteEmergencia: {ex.Message}");
				return false;
			}
		}

		public int GuardarParte1(CapturaAccidentesModel model, int idOficina, string abreviaturaMunicipio, int anio, string oficina = "NRA")

		{
			int result = 0;
			int lastInsertedId = 0;			
			
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
                connection.Open();
				using (SqlTransaction trx = connection.BeginTransaction())
				{
                    try
                    {
                        string strQuery = @"INSERT INTO accidentes( 
                                         [Hora]
                                        ,[idOficinaDelegacion]
                                        ,[idMunicipio]
                                        ,[idTramo]
                                        ,[observacionesTramo]
                                        ,[Fecha]
                                        ,[idCarretera]
                                        ,[kilometro]
                                        ,[idEstatusReporte]
                                        ,[fechaActualizacion]
                                        ,[actualizadoPor]
                                        ,[estatus]
                                        ,[latitud]
                                        ,[longitud]
										,[lugarCalle]
										,[lugarNumero]
										,[lugarColonia]
                                        )
                                VALUES (
                                         @Hora
                                        ,@idOficina
                                        ,@idMunicipio
                                        ,@idTramo
                                        ,@observacionesTramo
                                        ,@Fecha
                                        ,@idCarretera
                                        ,@kilometro
                                        ,@idEstatusReporte
                                        ,@fechaActualizacion
                                        ,@actualizadoPor
                                        ,@estatus
                                        ,@latitud
                                        ,@longitud
                                        ,@calle
                                        ,@numero
                                        ,@colonia
                                         );
                                    SELECT SCOPE_IDENTITY();"; // Obtener el último ID insertado

                        SqlCommand command = new SqlCommand(strQuery, connection, trx);
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Hora", SqlDbType.Time)).Value = (object)model.Hora ?? DBNull.Value;
                        command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                        // command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;
                        command.Parameters.Add(new SqlParameter("@kilometro", SqlDbType.NVarChar)).Value = (object)model.Kilometro ?? DBNull.Value;
                        command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)model.IdMunicipio ?? DBNull.Value;
                        command.Parameters.Add(new SqlParameter("@idEstatusReporte", SqlDbType.Int)).Value = 1;
                        command.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = (object)model.IdTramo ?? DBNull.Value;
                        command.Parameters.Add(new SqlParameter("@observacionesTramo", SqlDbType.NVarChar)).Value = (object)model.observacionesTramo?.ToUpper() ?? DBNull.Value;

                        command.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = (object)model.IdCarretera ?? DBNull.Value;
                        command.Parameters.Add(new SqlParameter("@Fecha", SqlDbType.DateTime)).Value = (object)model.Fecha ?? DBNull.Value;
                        command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                        command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                        command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@latitud", SqlDbType.NVarChar)).Value = (object)model.Latitud ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@longitud", SqlDbType.NVarChar)).Value = (object)model.Longitud ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@calle", SqlDbType.NVarChar)).Value = (object)model.lugarCalle ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@numero", SqlDbType.NVarChar)).Value = (object)model.lugarNumero ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@colonia", SqlDbType.NVarChar)).Value = (object)model.lugarColonia ?? DBNull.Value;
                        result = Convert.ToInt32(command.ExecuteScalar());
                        lastInsertedId = result;
                        //Se busca el ultimo consecutivo
                        command = new SqlCommand("select id,consecutivo from foliosAccidentes where abreviaturaMunicipio =@abreviaturaMunicipio and anio=@anio and idDelegacion=@idDelegacion", connection, trx)
                        {
                            CommandType = CommandType.Text
                        };
                        command.Parameters.Add(new SqlParameter("@abreviaturaMunicipio", SqlDbType.VarChar)).Value = abreviaturaMunicipio?.ToUpper();
                        command.Parameters.Add(new SqlParameter("@anio", SqlDbType.VarChar)).Value = anio;
                        command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.VarChar)).Value = idOficina;

                        int consecutivo = -1;
                        int idFolioSolicitud = -1;


                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Intenta leer un registro del resultado
                            {
                                idFolioSolicitud = reader["id"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["id"]);
                                consecutivo = reader["consecutivo"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["consecutivo"]);
                            }
                        }

                        if (consecutivo == -1)
                            throw new Exception("No se pudo crear el folio ya que no se encontraron registros con los datos de usuario");

                        //Se incrementa en 1 el consecutivo
                        consecutivo++;

                        command = new SqlCommand("select abreviatura from catMunicipios where idMunicipio =@idMunicipio", connection, trx)
                        {
                            CommandType = CommandType.Text
                        };
                        command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.VarChar)).Value = model.IdMunicipio;
                        string abreviaturaMun = "";

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Intenta leer un registro del resultado
                            {
                                abreviaturaMun = reader["abreviatura"].ToString();

                            }
                        }
                        //Se completa con ceros a la izquierda
                        string consecutivoConCeros = consecutivo.ToString("D5");
                        string anio2 = "/" + (anio % 100);

                        string newFolio = $"{abreviaturaMun}{consecutivoConCeros}{anio2}";

                        //Se actualiza el consecutivo en la tabla de foliosSolicitud
                        command = new SqlCommand(@"update foliosAccidentes set consecutivo=@consecutivo where id=@id", connection, trx);
                        command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = idFolioSolicitud;
                        command.Parameters.Add(new SqlParameter("@consecutivo", SqlDbType.Int)).Value = consecutivo;
                        command.CommandType = CommandType.Text;
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected <= 0)
                            throw new Exception("No se pudo crear el folio.");



                        SqlCommand command2 = new SqlCommand(@"update accidentes set numeroReporte=@folio where idAccidente=@id", connection, trx);
                        command2.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = (object)lastInsertedId ?? DBNull.Value;
                        command2.Parameters.Add(new SqlParameter("@folio", SqlDbType.VarChar)).Value = (object)newFolio?.ToUpper() ?? DBNull.Value;
                        command2.CommandType = CommandType.Text;
                        rowsAffected = command2.ExecuteNonQuery();

                        if (rowsAffected <= 0)                        
                            throw new Exception("No se pudo crear el folio.");

                        // si hay un turno asociado, registramos la infraccion en el turno
                        if (model.IdTurno.HasValue)
                        {
                            SqlCommand turnoCommand = new("INSERT INTO turnoAccidentes (idTurno, idAccidente) VALUES (@IdTurno, @IdAccidente)", connection, trx);
                            turnoCommand.Parameters.Add(new SqlParameter("IdTurno", SqlDbType.BigInt)).Value = model.IdTurno.Value;
                            turnoCommand.Parameters.Add(new SqlParameter("IdAccidente", SqlDbType.Int)).Value = lastInsertedId;
                            turnoCommand.ExecuteNonQuery();
                        }


                        //Guarda infraccionesAccidentes						
                        trx.Commit();
                        return lastInsertedId;
                    }
                    catch (Exception ex)
                    {
                        trx.Rollback();
                        if (ex is not SqlException sqlEx)                        
                            throw;                        

                        Console.WriteLine("Error de SQL: " + sqlEx.Message);
                        return lastInsertedId;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }                
			}
		}
		public List<CapturaAccidentesModel> BuscarPorParametro(string Placa, string Serie, string Folio, int corp)
		{
			List<CapturaAccidentesModel> Vehiculo = new List<CapturaAccidentesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command;

					if (!string.IsNullOrEmpty(Serie))
					{
						command = new SqlCommand(
						"SELECT TOP 200 v.*, mv.marcaVehiculo, sm.nombreSubmarca, e.nombreEntidad, cc.color, tv.tipoVehiculo, ts.tipoServicio, " +
						"p.CURP, p.RFC, p.nombre, p.apellidoPaterno, p.apellidoMaterno " +
						"FROM vehiculos v " +
						"left JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
						"left JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca " +
						"left JOIN catEntidades e ON v.idEntidad = e.idEntidad " +
						"left JOIN catColores cc ON v.idColor = cc.idColor " +
						"left JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
						"left JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
						"LEFT JOIN personas p ON v.idPersona = p.idPersona  " +
						"WHERE v.estatus = 1 AND v.serie = @Serie  ORDER BY v.idVehiculo DESC", connection);

						command.Parameters.AddWithValue("@Serie", Serie);
					}
					else if (!string.IsNullOrEmpty(Placa))
					{
						command = new SqlCommand("SELECT TOP 200 v.*, mv.marcaVehiculo, sm.nombreSubmarca, e.nombreEntidad, cc.color, tv.tipoVehiculo, ts.tipoServicio, " +
												"p.CURP, p.RFC, p.nombre, p.apellidoPaterno, p.apellidoMaterno " +
												" FROM vehiculos v " +
												" LEFT JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
												" LEFT JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca " +
												" LEFT JOIN catEntidades e ON v.idEntidad = e.idEntidad " +
												" LEFT JOIN catColores cc ON v.idColor = cc.idColor " +
												" LEFT JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
												" LEFT JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
												" LEFT JOIN personas p ON v.idPersona = p.idPersona " +
												" WHERE v.estatus = 1 AND v.placas LIKE '%' + @Placa + '%' ORDER BY v.idVehiculo DESC", connection);
						command.Parameters.AddWithValue("@Placa", Placa);

					}
					else if (!string.IsNullOrEmpty(Folio))
					{
						command = new SqlCommand(@"SELECT 
                                                    sol.idSolicitud,
                                                    sol.folio,
                                                    dep.idDeposito,
                                                    dep.idVehiculo,
                                                    dep.IdConcesionario,
                                                    dep.idPension,
                                                    pen.pension,
                                                    pen.idDelegacion,
                                                    mv.marcaVehiculo,
                                                    sm.nombreSubmarca,
                                                    e.nombreEntidad,
                                                    cc.color,
                                                    tv.tipoVehiculo,
                                                    ts.tipoServicio,
                                                    p.nombre,
                                                    p.CURP,
                                                    p.RFC,
                                                    p.apellidoPaterno,
                                                    p.apellidoMaterno,
                                                      +ISNULL(sub.numerosEconomicos, '')  AS numerosEconomicos,
                                                      +ISNULL(sub.idGruas, '')  AS idGruas,
                                                    v.*
                                                FROM solicitudes sol
                                                LEFT JOIN depositos dep ON dep.idSolicitud = sol.idSolicitud
                                                LEFT JOIN vehiculos v ON v.idVehiculo = dep.idVehiculo

                                                LEFT JOIN(
                                                    SELECT
                                                        dep.idSolicitud,
                                                        STRING_AGG(CAST(g.noEconomico AS NVARCHAR(MAX)), ',') AS numerosEconomicos,
                                                        STRING_AGG(CAST(g.idGrua AS NVARCHAR(MAX)), ',') AS idGruas
                                                    FROM depositos dep
                                                    LEFT JOIN gruasAsignadas ga ON ga.idDeposito = dep.idDeposito AND ga.estatus = 1

                                                    LEFT JOIN gruas g ON g.idGrua = ga.idGrua

                                                    GROUP BY dep.idSolicitud
                                                ) sub ON sub.idSolicitud = sol.idSolicitud

                                                LEFT JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo
                                                LEFT JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca
                                                LEFT JOIN pensiones pen ON pen.idPension = dep.idPension
                                                LEFT JOIN catEntidades e ON v.idEntidad = e.idEntidad
                                                LEFT JOIN catColores cc ON v.idColor = cc.idColor
                                                LEFT JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo
                                                LEFT JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio
                                                LEFT JOIN personas p ON v.idPersona = p.idPersona
                                                WHERE v.estatus = 1 AND sol.folio = @Folio AND sol.BanderaTransito = @corp", connection);
						command.Parameters.AddWithValue("@Folio", Folio);
						command.Parameters.AddWithValue("@corp", corp);

					}
					else
					{
						// No se proporcionó ningún parámetro válido
						return Vehiculo;
					}

					command.CommandType = CommandType.Text;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CapturaAccidentesModel vehiculo = new CapturaAccidentesModel();
							vehiculo.IdVehiculo = Convert.IsDBNull(reader["IdVehiculo"]) ? 0 : Convert.ToInt32(reader["IdVehiculo"]);
							vehiculo.IdMarcaVehiculo = Convert.IsDBNull(reader["IdMarcaVehiculo"]) ? 0 : Convert.ToInt32(reader["IdMarcaVehiculo"]);
							vehiculo.IdSubmarca = Convert.IsDBNull(reader["IdSubmarca"]) ? 0 : Convert.ToInt32(reader["IdSubmarca"]);
							vehiculo.IdEntidad = Convert.IsDBNull(reader["IdEntidad"]) ? 0 : Convert.ToInt32(reader["IdEntidad"]);
							vehiculo.IdColor = Convert.IsDBNull(reader["IdColor"]) ? 0 : Convert.ToInt32(reader["IdColor"]);
							vehiculo.IdTipoVehiculo = Convert.IsDBNull(reader["IdTipoVehiculo"]) ? 0 : Convert.ToInt32(reader["IdTipoVehiculo"]);
							vehiculo.IdCatTipoServicio = Convert.IsDBNull(reader["IdCatTipoServicio"]) ? 0 : Convert.ToInt32(reader["IdCatTipoServicio"]);
							vehiculo.IdPersona = Convert.IsDBNull(reader["IdPersona"]) ? 0 : Convert.ToInt32(reader["IdPersona"]);
							vehiculo.RFC = reader["RFC"].ToString();
							vehiculo.CURP = reader["CURP"].ToString();
							vehiculo.Marca = reader["marcaVehiculo"].ToString();
							vehiculo.Submarca = reader["nombreSubmarca"].ToString();
							vehiculo.Modelo = reader["Modelo"].ToString();
							vehiculo.Placa = reader["Placas"].ToString();
							vehiculo.Tarjeta = reader["Tarjeta"].ToString();
							vehiculo.VigenciaTarjeta = Convert.IsDBNull(reader["VigenciaTarjeta"]) ? DateTime.MinValue : Convert.ToDateTime(reader["VigenciaTarjeta"]);
							vehiculo.Serie = reader["serie"].ToString();
							vehiculo.EntidadRegistro = reader["nombreEntidad"].ToString();
							vehiculo.Color = reader["color"].ToString();
							vehiculo.TipoServicio = reader["tipoServicio"].ToString();
							vehiculo.TipoVehiculo = reader["tipoVehiculo"].ToString();
							vehiculo.Propietario = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
							// Si la búsqueda es por Folio, asignar parametros adicionales
							if (!string.IsNullOrEmpty(Folio))
							{
								vehiculo.NumerosEconomicos = reader["numerosEconomicos"].ToString();
								vehiculo.IdGruas = reader["idGruas"].ToString();

								vehiculo.IdSolicitud = Convert.IsDBNull(reader["idSolicitud"]) ? 0 : Convert.ToInt32(reader["idSolicitud"]);
								vehiculo.IdPension = Convert.IsDBNull(reader["idPension"]) ? 0 : Convert.ToInt32(reader["idPension"]);
								vehiculo.FolioSolicitudDeposito = reader["folio"].ToString();
								vehiculo.IdDeposito = Convert.IsDBNull(reader["idDeposito"]) ? 0 : Convert.ToInt32(reader["idDeposito"]);
								vehiculo.IdConcesionario = Convert.IsDBNull(reader["idConcesionario"]) ? 0 : Convert.ToInt32(reader["idConcesionario"]);
								vehiculo.IdDelegacionPension = Convert.IsDBNull(reader["idDelegacion"]) ? 0 : Convert.ToInt32(reader["idDelegacion"]);

							}

							Vehiculo.Add(vehiculo);
						}
					}
				}
				catch (SqlException ex)
				{
					return Vehiculo;
				}
				finally
				{
					connection.Close();
				}
			}

			return Vehiculo;
		}



		public List<CapturaAccidentesModel> BuscarPorParametroid(string id)
		{
			List<CapturaAccidentesModel> Vehiculo = new List<CapturaAccidentesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command;
					command = new SqlCommand("SELECT v.*, " +
						"mv.marcaVehiculo, " +
						"sm.nombreSubmarca, " +
						"e.nombreEntidad, " +
						"cc.color, " +
						"tv.tipoVehiculo, " +
						"ts.tipoServicio," +
						"p.nombre, " +
						"p.apellidoPaterno, " +
						"p.apellidoMaterno, " +
						"p.CURP, " +
						"p.RFC " +
						"FROM vehiculos v " +
						"JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
						"left JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca " +
						"left JOIN catEntidades e ON v.idEntidad = e.idEntidad " +
						"left JOIN catColores cc ON v.idColor = cc.idColor " +
						"left JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
						"left JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
						"LEFT JOIN personas p ON v.idPersona = p.idPersona OR v.propietario = p.idPersona " +
						"WHERE v.estatus = 1 AND idVehiculo=@Folio;", connection);
					command.Parameters.AddWithValue("@Folio", Convert.ToInt32(id));

					command.CommandType = CommandType.Text;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CapturaAccidentesModel vehiculo = new CapturaAccidentesModel();
							vehiculo.IdVehiculo = Convert.IsDBNull(reader["IdVehiculo"]) ? 0 : Convert.ToInt32(reader["IdVehiculo"]);
							vehiculo.IdMarcaVehiculo = Convert.IsDBNull(reader["IdMarcaVehiculo"]) ? 0 : Convert.ToInt32(reader["IdMarcaVehiculo"]);
							vehiculo.IdSubmarca = Convert.IsDBNull(reader["IdSubmarca"]) ? 0 : Convert.ToInt32(reader["IdSubmarca"]);
							vehiculo.IdEntidad = Convert.IsDBNull(reader["IdEntidad"]) ? 0 : Convert.ToInt32(reader["IdEntidad"]);
							vehiculo.IdColor = Convert.IsDBNull(reader["IdColor"]) ? 0 : Convert.ToInt32(reader["IdColor"]);
							vehiculo.IdTipoVehiculo = Convert.IsDBNull(reader["IdTipoVehiculo"]) ? 0 : Convert.ToInt32(reader["IdTipoVehiculo"]);
							vehiculo.IdCatTipoServicio = Convert.IsDBNull(reader["IdCatTipoServicio"]) ? 0 : Convert.ToInt32(reader["IdCatTipoServicio"]);
							vehiculo.IdPersona = Convert.IsDBNull(reader["IdPersona"]) ? 0 : Convert.ToInt32(reader["IdPersona"]);
							vehiculo.Marca = reader["marcaVehiculo"].ToString();
							vehiculo.Submarca = reader["nombreSubmarca"].ToString();
							vehiculo.Modelo = reader["Modelo"].ToString();
							vehiculo.CURP = reader["CURP"].ToString();
							vehiculo.RFC = reader["RFC"].ToString();
							vehiculo.Placa = reader["Placas"].ToString();
							vehiculo.Tarjeta = reader["Tarjeta"].ToString();
							vehiculo.VigenciaTarjeta = Convert.IsDBNull(reader["VigenciaTarjeta"]) ? DateTime.MinValue : Convert.ToDateTime(reader["VigenciaTarjeta"]);
							vehiculo.Serie = reader["serie"].ToString();
							vehiculo.EntidadRegistro = reader["nombreEntidad"].ToString();
							vehiculo.Color = reader["color"].ToString();
							vehiculo.TipoServicio = reader["tipoServicio"].ToString();
							vehiculo.TipoVehiculo = reader["tipoVehiculo"].ToString();
							vehiculo.Propietario = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";


							Vehiculo.Add(vehiculo);
						}
					}
				}
				catch (SqlException ex)
				{
					return Vehiculo;
				}
				finally
				{
					//   connection.Close();
				}
			}

			return Vehiculo;
		}







		public int ActualizarConVehiculo(int idVehiculo, int idAccidente, int IdPersona, string Placa, string Serie, bool EsValidacion)
		{
			int idVehiculoInsertado = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand("usp_InsertaConVehiculo", connection);
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.AddWithValue("@idVehiculo", idVehiculo);
					command.Parameters.AddWithValue("@idAccidente", idAccidente);
					command.Parameters.AddWithValue("@idPersona", IdPersona);
					command.Parameters.AddWithValue("@Placa", Placa?.ToUpper());
					command.Parameters.AddWithValue("@Serie", Serie?.ToUpper());
					command.Parameters.AddWithValue("@EsValidacion", EsValidacion);

					var r = command.ExecuteReader();

					while (r.Read())
					{
						idVehiculoInsertado = (int)r["result"];
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
			}

			//Crear historico vehiculo accidente
			try
			{
				var bitacoraVehiculo = _vehiculosService.CrearHistoricoVehiculo(idAccidente, (int)idVehiculo, 2);
			}
			catch { }

			return idVehiculoInsertado;
		}

		public int BorrarVehiculoAccidente(int idVehiculo, int idAccidente)
		{

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand("UPDATE  vehiculosAccidente SET estatus = 0" +
														" WHERE idAccidente = @idAccidente AND idVehiculo = @idVehiculo", connection);
					command.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.Int)).Value = idAccidente;
					command.Parameters.Add(new SqlParameter("@idVehiculo", SqlDbType.Int)).Value = idVehiculo;
					command.CommandType = CommandType.Text;

					int filasAfectadas = command.ExecuteNonQuery();

					if (filasAfectadas > 0)
					{
						return idVehiculo;
					}
					else
					{
						return -1;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error al borrar el vehículo del accidente: " + ex.Message);
					return -1;
				}

			}
		}
		public CapturaAccidentesModel InvolucradoSeleccionado(int idAccidente, int IdVehiculoInvolucrado, int IdPropietarioInvolucrado)
		{
			CapturaAccidentesModel involucrado = new CapturaAccidentesModel();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand("usp_ObtieneVehiculoInvolucradoSeleccionado", connection);

					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.AddWithValue("@idAccidente", idAccidente);
					command.Parameters.AddWithValue("@idPersona", IdPropietarioInvolucrado);
					command.Parameters.AddWithValue("@IdVehiculoInvolucrado", IdVehiculoInvolucrado);

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							involucrado.IdPropietarioInvolucrado = reader["idPropietario"] != DBNull.Value ? Convert.ToInt32(reader["idPropietario"].ToString()) : 0;
							involucrado.IdAccidente = reader["idAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idAccidente"].ToString()) : 0;
							involucrado.IdVehiculoInvolucrado = reader["idVehiculo"] != DBNull.Value ? Convert.ToInt32(reader["idVehiculo"].ToString()) : 0;
							involucrado.IdTipoCarga = reader["IdTipoCarga"] != DBNull.Value ? Convert.ToInt32(reader["IdTipoCarga"].ToString()) : 0;
							involucrado.IdPension = reader["IdPension"] != DBNull.Value ? Convert.ToInt32(reader["IdPension"].ToString()) : 0;
							involucrado.IdFormaTrasladoInvolucrado = reader["idFormaTraslado"] != DBNull.Value ? Convert.ToInt32(reader["idFormaTraslado"].ToString()) : 0;
							involucrado.idPersonaInvolucrado = reader["IdConductor"] != DBNull.Value ? Convert.ToInt32(reader["IdConductor"].ToString()) : 0;
							involucrado.Placa = reader["placas"] != DBNull.Value ? reader["placas"].ToString() : string.Empty;
							involucrado.fechaNacimiento = reader["fechaNacimiento"] != DBNull.Value ? Convert.ToDateTime(reader["fechaNacimiento"]) : DateTime.MinValue;
							involucrado.Tarjeta = reader["tarjeta"] != DBNull.Value ? reader["tarjeta"].ToString() : string.Empty;
							involucrado.TipoCarga = reader["carga"] != DBNull.Value ? reader["carga"].ToString() : string.Empty;
							involucrado.TipoCargaBool = reader["carga"] != DBNull.Value && Convert.ToBoolean(reader["carga"]);
							involucrado.Poliza = reader["poliza"] != DBNull.Value ? reader["poliza"].ToString() : string.Empty;
							involucrado.Serie = reader["serie"] != DBNull.Value ? reader["serie"].ToString() : string.Empty;
							involucrado.Entidad = reader["nombreEntidad"] != DBNull.Value ? reader["nombreEntidad"].ToString() : string.Empty;
							involucrado.Marca = reader["marcaVehiculo"] != DBNull.Value ? reader["marcaVehiculo"].ToString() : string.Empty;
							involucrado.Submarca = reader["nombreSubmarca"] != DBNull.Value ? reader["nombreSubmarca"].ToString() : string.Empty;
							involucrado.TipoVehiculo = reader["tipoVehiculo"] != DBNull.Value ? reader["tipoVehiculo"].ToString() : string.Empty;
							involucrado.PropietarioInvolucrado = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
							involucrado.EntidadPropietario = reader["EntidadPropietario"] != DBNull.Value ? reader["EntidadPropietario"].ToString() : string.Empty;
							involucrado.MunicipioPropietario = reader["MunicipioPropietario"] != DBNull.Value ? reader["MunicipioPropietario"].ToString() : string.Empty;
							involucrado.ColoniaPropietario = reader["ColoniaPropietario"] != DBNull.Value ? reader["ColoniaPropietario"].ToString() : string.Empty;
							involucrado.CallePropietario = reader["CallePropietario"] != DBNull.Value ? reader["CallePropietario"].ToString() : string.Empty;
							involucrado.NumeroPropietario = reader["NumeroPropietario"] != DBNull.Value ? reader["NumeroPropietario"].ToString() : string.Empty;
							involucrado.TelefonoPropietario = reader["TelefonoPropietario"] != DBNull.Value ? reader["TelefonoPropietario"].ToString() : string.Empty;
							involucrado.CorreoPropietario = reader["CorreoPropietario"] != DBNull.Value ? reader["CorreoPropietario"].ToString() : string.Empty;
							involucrado.NumeroReporte = reader["numeroReporte"] != DBNull.Value ? reader["numeroReporte"].ToString() : string.Empty;
							involucrado.Modelo = reader["modelo"] != DBNull.Value ? reader["modelo"].ToString() : string.Empty;
							involucrado.Motor = reader["motor"] != DBNull.Value ? reader["motor"].ToString() : string.Empty;
							involucrado.Capacidad = reader["capacidad"] != DBNull.Value ? reader["capacidad"].ToString() : string.Empty;
							involucrado.Pension = reader["pension"] != DBNull.Value ? reader["pension"].ToString() : string.Empty;
							involucrado.Modelo = reader["modelo"] != DBNull.Value ? reader["modelo"].ToString() : string.Empty;
							involucrado.Color = reader["color"] != DBNull.Value ? reader["color"].ToString() : string.Empty;
							involucrado.RFC = reader["RFC"] != DBNull.Value ? reader["RFC"].ToString() : string.Empty;
							involucrado.CURP = reader["CURP"] != DBNull.Value ? reader["CURP"].ToString() : string.Empty;
							involucrado.TipoPersona = reader["tipoPersona"] != DBNull.Value ? reader["tipoPersona"].ToString() : string.Empty;
							involucrado.TipoServicio = reader["tipoServicio"] != DBNull.Value ? reader["tipoServicio"].ToString() : string.Empty;
							involucrado.VigenciaTarjeta = reader["vigenciaTarjeta"] != DBNull.Value ? (DateTime)reader["vigenciaTarjeta"] : DateTime.MinValue;
							involucrado.Otros = reader["otros"] != DBNull.Value ? reader["otros"].ToString() : string.Empty;


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
			return involucrado;
		}

		public CapturaAccidentesModel ObtenerConductorPorId(int IdPersona)
		{
			CapturaAccidentesModel model = new CapturaAccidentesModel();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();
					const string SqlTransact =
											@"SELECT p.idPersona,p.idCatTipoPersona,p.nombre,p.apellidoPaterno,p.apellidoMaterno,
                                            p.rfc,p.curp,CONVERT(varchar, p.fechaNacimiento, 103) AS fechaNacimiento,p.vigenciaLicencia,p.numeroLicencia,p.idGenero
                                            ,ctp.tipoPersona,v.idVehiculo,v.modelo
                                            ,pd.telefono
                                            ,pd.correo,pd.idEntidad,pd.idMunicipio,pd.colonia,pd.calle,pd.numero
							                ,tl.tipoLicencia,tv.tipoVehiculo
                                            ,mun.Municipio,cent.nombreEntidad,cg.genero
                                            ,v.vigenciaTarjeta


                                            FROM personas AS p                                           
                                            LEFT JOIN catTipoPersona AS ctp ON p.idCatTipoPersona = ctp.idCatTipoPersona
                                            LEFT JOIN vehiculos AS v ON p.idPersona = v.idPersona
                                            LEFT JOIN personasDirecciones AS pd ON p.idPersona = pd.idPersona
                                            LEFT JOIN catGeneros cg on p.idGenero = cg.idGenero
                                            LEFT JOIN catTipoLicencia tl ON p.idTipoLicencia = tl.idTipoLicencia
                                            LEFT JOIN catTiposVehiculo AS tv ON v.idTipoVehiculo = tv.idTipoVehiculo
                                            LEFT JOIN catMunicipios AS mun ON mun.idMunicipio = pd.idMunicipio 
                                            LEFT JOIN catEntidades AS cent ON pd.idEntidad = cent.idEntidad 
                                            WHERE p.idPersona = @IdPersona AND p.estatus = 1";
					SqlCommand command = new SqlCommand(SqlTransact, connection);
					command.Parameters.Add(new SqlParameter("@IdPersona", SqlDbType.Int)).Value = (object)IdPersona ?? DBNull.Value;
					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							model.IdPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
							model.IdVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idVehiculo"].ToString());
							model.IdCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
							model.nombre = reader["nombre"].ToString();
							model.apellidoPaterno = reader["apellidoPaterno"].ToString();
							model.apellidoMaterno = reader["apellidoMaterno"].ToString();
							model.Propietario = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
							model.RFC = reader["rfc"].ToString();
							model.CURP = reader["curp"].ToString();
							model.numeroLicencia = reader["numeroLicencia"].ToString();
							model.Genero = reader["genero"].ToString();
							model.TipoPersona = reader["tipoPersona"].ToString();
							model.FormatDateNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["fechaNacimiento"].ToString());
							model.vigenciaLicencia = reader["vigenciaLicencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());
							model.VigenciaTarjeta = reader["vigenciaTarjeta"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaTarjeta"].ToString());
							model.idGenero = reader["idGenero"] == DBNull.Value ? default(int) : Convert.ToInt32(reader["idGenero"]);
							model.Telefono = reader["telefono"].ToString();
							model.Correo = reader["correo"].ToString();
							model.Modelo = reader["modelo"].ToString();
							model.Colonia = reader["colonia"].ToString();
							model.Numero = reader["numero"].ToString();
							model.Calle = reader["calle"].ToString();
							model.IdEntidad = reader["IdEntidad"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["IdEntidad"].ToString());
							model.IdMunicipio = reader["IdMunicipio"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["IdMunicipio"].ToString());
							model.TipoLicencia = reader["tipoLicencia"].ToString();
							model.TipoVehiculo = reader["tipoVehiculo"].ToString();
							model.Entidad = reader["nombreEntidad"].ToString();
							model.Municipio = reader["Municipio"].ToString();


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
		public int InsertarConductor(int IdVehiculo, int idAccidente, int IdPersona)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = @"INSERT INTO conductoresVehiculosAccidente (idAccidente,
                                                                                idVehiculo, 
                                                                                idPersona, 
                                                                                estatus,
																				fechaActualizacion) 
                                                                                values(@idAccidente, 
                                                                                @idVehiculo,
                                                                                @idPersona,
                                                                                @estatus,
																				@fechaActualizacion);";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@idVehiculo", IdVehiculo);
					command.Parameters.AddWithValue("@idAccidente", idAccidente);
					command.Parameters.AddWithValue("@idPersona", IdPersona);
					command.Parameters.AddWithValue("@estatus", 1);
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

		public async Task<int> ActualizaInfoAccidente(int idAccidente, DateTime Fecha, TimeSpan Hora, int IdMunicipio, int IdCarretera, int IdTramo, long? IdTurno, string Kilometro, int idOficina, int? IdEmergencia, int? FolioEmergencia, string Latitud, string Longitud,string observacionesTramo, string lugarCalle,string lugarNumero,string lugarColonia)
		{
			int result = 0;
			
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				connection.Open();
				using (SqlTransaction trx = connection.BeginTransaction())
				{
					try
					{

						string query = "UPDATE accidentes SET fecha = @fecha, hora = @hora, idMunicipio = @idMunicipio, idCarretera = @idCarretera, idTramo = @idTramo, kilometro = @kilometro, latitud = @latitud, longitud = @longitud, fechaActualizacion = @fechaActualizacion, observacionesTramo= @observacionesTramo, " +
							" lugarCalle = @lugarCalle,lugarNumero=@lugarNumero,lugarColonia = @lugarColonia WHERE idAccidente = @idAccidente";

						SqlCommand command = new SqlCommand(query, connection, trx);

						command.Parameters.Add(new SqlParameter("@fecha", SqlDbType.Date)).Value = (object)Fecha ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@hora", SqlDbType.Time)).Value = (object)Hora ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = IdMunicipio;
						command.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = IdCarretera;
						command.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = IdTramo;
						command.Parameters.Add(new SqlParameter("@kilometro", SqlDbType.NVarChar)).Value = Kilometro;
						command.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.Int)).Value = idAccidente;
						command.Parameters.Add(new SqlParameter("@latitud", SqlDbType.NVarChar)).Value = (object)Latitud ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@longitud", SqlDbType.NVarChar)).Value = (object)Longitud ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@observacionesTramo", SqlDbType.NVarChar)).Value = (object)observacionesTramo ?? DBNull.Value;
                        command.Parameters.Add(new SqlParameter("@lugarCalle", SqlDbType.NVarChar)).Value = (object)lugarCalle ?? DBNull.Value;
                        command.Parameters.Add(new SqlParameter("@lugarColonia", SqlDbType.NVarChar)).Value = (object)lugarColonia ?? DBNull.Value;
						command.Parameters.Add(new SqlParameter("@lugarNumero", SqlDbType.NVarChar)).Value = (object)lugarNumero ?? DBNull.Value;

                        command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.Date)).Value = DateTime.Now;

                        result = command.ExecuteNonQuery();

						if (IdEmergencia != null)
						{
							string query2 = "UPDATE accidentesEmergencias SET folioEmergencia = @folioEmergencia, fechaActualizacion = @fechaActualizacion WHERE emergenciasId = @idEmergencia";

							SqlCommand command2 = new SqlCommand(query2, connection, trx);
							command2.Parameters.Add(new SqlParameter("@folioEmergencia", SqlDbType.Int)).Value = (object)FolioEmergencia ?? DBNull.Value;
							command2.Parameters.Add(new SqlParameter("@idEmergencia", SqlDbType.Int)).Value = (object)IdEmergencia ?? DBNull.Value;
                            command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.Date)).Value = DateTime.Now;

                            result = command2.ExecuteNonQuery();
						}
						else if (IdEmergencia == null && FolioEmergencia != null)
						{
							await GuardarAccidenteEmergencia(IdMunicipio, idAccidente, idOficina, FolioEmergencia);

						}

						if (IdTurno.HasValue)
						{
							UpsertTurnoAsignedToAccidente(connection, trx, IdTurno.Value, idAccidente);
						}
						else
						{
							RemoveTurnoAssignmentToAccidente(connection, trx, idAccidente);
						}

						trx.Commit();
					}
					catch (Exception)
					{
						trx.Rollback();
						return result;
					}
					finally
					{
						connection.Close();
					}
				}				

				return result;
			}


		}

		public int AgregarValorClasificacion(int IdClasificacionAccidente, int idAccidente)
		{
			int resultado = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();

					string query = "asp_GenerateClasificacionAccidente";

					SqlCommand command = new SqlCommand(query, connection);
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.AddWithValue("@IdClasificacionAccidente", IdClasificacionAccidente);
					command.Parameters.AddWithValue("@idAccidente", idAccidente);
					command.Parameters.Add("@Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;

					command.ExecuteNonQuery();

					// Obtener el valor del parámetro de salida
					resultado = Convert.ToInt32(command.Parameters["@Resultado"].Value);
				}
				catch (SqlException ex)
				{
					
				}
				finally
				{
					connection.Close();
				}

				return resultado;
			}
		}

		public List<CapturaAccidentesModel> ObtenerDatosGrid(int idAccidente)
		{
			//
			List<CapturaAccidentesModel> ListaClasificacion = new List<CapturaAccidentesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"
                        SELECT a.*, ca.nombreClasificacion 
                            FROM AccidenteClasificacion a 
                            JOIN catClasificacionAccidentes ca ON a.idCatClasificacionAccidentes = ca.idClasificacionAccidente 
                            WHERE a.idAccidente = @idAccidente 
                            and a.estatus=1 

                    ", connection);
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idAccidente", idAccidente);

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CapturaAccidentesModel clasificacion = new CapturaAccidentesModel();
							clasificacion.IdAccidente = Convert.ToInt32(reader["idAccidente"].ToString());
							clasificacion.IdClasificacionAccidente = Convert.ToInt32(reader["idCatClasificacionAccidentes"].ToString());
							clasificacion.NombreClasificacion = reader["NombreClasificacion"].ToString();


							ListaClasificacion.Add(clasificacion);

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
			return ListaClasificacion;


		}

		public List<CapturaAccidentesModel> AccidentePorID(int IdAccidente)
		{
			List<CapturaAccidentesModel> ListaAccidentePorID = new List<CapturaAccidentesModel>();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand("SELECT* FROM accidentes WHERE idAccidente = @idAccidente;", connection);
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idAccidente", IdAccidente);

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CapturaAccidentesModel clasificacion = new CapturaAccidentesModel();
							clasificacion.IdAccidente = Convert.ToInt32(reader["IdAccidente"].ToString());
							clasificacion.IdClasificacionAccidente = reader["IdClasificacionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["IdClasificacionAccidente"].ToString()) : null;
							clasificacion.Fecha = reader["Fecha"] != DBNull.Value ? Convert.ToDateTime(reader["Fecha"].ToString()): null;

							ListaAccidentePorID.Add(clasificacion);

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
			return ListaAccidentePorID;


		}
		public int ClasificacionEliminar(int IdAccidente, int IdClasificacionAccidente)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = @"UPDATE AccidenteClasificacion SET estatus = 0,fechaActualizacion = @fechaActualizacion WHERE idAccidente = @idAccidente and idCatClasificacionAccidentes=@claf";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@idAccidente", IdAccidente);
					command.Parameters.AddWithValue("@claf", IdClasificacionAccidente);
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

		public int AgregarValorFactorYOpcion(int IdFactorAccidente, int IdFactorOpcionAccidente, int idAccidente)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();

					// Verificar si ya existe un registro con los mismos idAccidente, idFactor, idFactorOpcion y estatus = 1
					string verificaExistenciaQuery = "SELECT COUNT(*) FROM AccidenteFactoresOpciones " +
													 "WHERE idAccidente = @idAccidente " +
													 "AND idFactor = @IdFactorAccidente " +
													 "AND idFactorOpcion = @IdFactorOpcionAccidente " +
													 "AND estatus = 1";
					SqlCommand verificaExistenciaCommand = new SqlCommand(verificaExistenciaQuery, connection);
					verificaExistenciaCommand.Parameters.AddWithValue("@idAccidente", idAccidente);
					verificaExistenciaCommand.Parameters.AddWithValue("@IdFactorAccidente", IdFactorAccidente);
					verificaExistenciaCommand.Parameters.AddWithValue("@IdFactorOpcionAccidente", IdFactorOpcionAccidente);

					int registrosExistencia = (int)verificaExistenciaCommand.ExecuteScalar();

					if (registrosExistencia == 0)
					{
						string insertQuery = "INSERT INTO AccidenteFactoresOpciones (idFactor, idFactorOpcion, idAccidente,fechaActualizacion,actualizadoPor,estatus) " +
											 "VALUES (@IdFactorAccidente, @IdFactorOpcionAccidente,@idAccidente,@fechaActualizacion,@actualizadoPor,@estatus)";

						SqlCommand command = new SqlCommand(insertQuery, connection);

						command.Parameters.AddWithValue("@IdFactorAccidente", IdFactorAccidente);
						command.Parameters.AddWithValue("@IdFactorOpcionAccidente", IdFactorOpcionAccidente);
						command.Parameters.AddWithValue("@idAccidente", idAccidente);
						command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);
						command.Parameters.AddWithValue("@actualizadoPor", 1);
						command.Parameters.AddWithValue("@estatus", 1);

						command.ExecuteNonQuery();
					}
					else
					{

						result = 2;
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
		}
		public int EliminarValorFactorYOpcion(int IdAccidenteFactorOpcion)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "UPDATE AccidenteFactoresOpciones SET estatus = 0, fechaActualizacion = @fechaActualizacion " +
						"WHERE idAccidenteFactorOpcion = @IdAccidenteFactorOpcion";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@IdAccidenteFactorOpcion", IdAccidenteFactorOpcion);
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
		public int EditarFactorOpcion(int IdFactorAccidente, int IdFactorOpcionAccidente, int IdAccidenteFactorOpcion, int idAccidente)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();

					// Verificar si ya existe un registro con los mismos idAccidente, idFactor, idFactorOpcion y estatus = 1
					string verificaExistenciaQuery = "SELECT COUNT(*) FROM AccidenteFactoresOpciones " +
													 "WHERE idAccidente = @idAccidente " +
													 "AND idFactor = @idFactor " +
													 "AND idFactorOpcion = @idFactorOpcion " +
													 "AND estatus = 1";
					SqlCommand verificaExistenciaCommand = new SqlCommand(verificaExistenciaQuery, connection);
					verificaExistenciaCommand.Parameters.AddWithValue("@idAccidente", idAccidente);
					verificaExistenciaCommand.Parameters.AddWithValue("@idFactor", IdFactorAccidente);
					verificaExistenciaCommand.Parameters.AddWithValue("@idFactorOpcion", IdFactorOpcionAccidente);

					int registrosExistencia = (int)verificaExistenciaCommand.ExecuteScalar();

					if (registrosExistencia == 0)
					{
						// No existe un registro con los mismos idAccidente, idFactor, idFactorOpcion y estatus = 1, se puede actualizar
						string query = "UPDATE AccidenteFactoresOpciones SET idFactor = @IdFactor, idFactorOpcion = @IdFactorOpcion,fechaActualizacion = @fechaActualizacion  " +
										"WHERE idAccidenteFactorOpcion = @IdAccidenteFactorOpcion";

						SqlCommand command = new SqlCommand(query, connection);

						command.Parameters.AddWithValue("@IdFactor", IdFactorAccidente);
						command.Parameters.AddWithValue("@IdFactorOpcion", IdFactorOpcionAccidente);
						command.Parameters.AddWithValue("@IdAccidenteFactorOpcion", IdAccidenteFactorOpcion);
                        command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

                        command.ExecuteNonQuery();
					}
					else
					{

						result = 2;
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
		}

		public List<CapturaAccidentesModel> ObtenerDatosGridFactor(int idAccidente)
		{
			//
			List<CapturaAccidentesModel> ListaGridFactor = new List<CapturaAccidentesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand("SELECT afo.idAccidenteFactorOpcion, afo.idFactor,afo.idFactorOpcion,afo.idAccidente,cfa.FactorAccidente,cfoa.FactorOpcionAccidente " +
						"FROM AccidenteFactoresOpciones AS afo " +
						"LEFT JOIN catFactoresAccidentes AS cfa ON afo.idFactor = cfa.idFactorAccidente " +
						"LEFT JOIN catFactoresOpcionesAccidentes AS cfoa ON afo.idFactorOpcion = cfoa.idFactorOpcionAccidente " +
						"WHERE afo.idAccidente = @IdAccidente AND afo.estatus = 1 ORDER BY afo.fechaActualizacion DESC", connection);
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idAccidente", idAccidente);

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CapturaAccidentesModel factorOpcion = new CapturaAccidentesModel();
							factorOpcion.IdAccidenteFactorOpcion = Convert.ToInt32(reader["idAccidenteFactorOpcion"].ToString());
							factorOpcion.IdAccidente = Convert.ToInt32(reader["IdAccidente"].ToString());
							factorOpcion.IdFactorAccidente = Convert.ToInt32(reader["idFactor"].ToString());
							factorOpcion.IdFactorOpcionAccidente = Convert.ToInt32(reader["idFactorOpcion"].ToString());
							factorOpcion.FactorAccidente = reader["FactorAccidente"].ToString();
							factorOpcion.FactorOpcionAccidente = reader["FactorOpcionAccidente"].ToString();


							ListaGridFactor.Add(factorOpcion);

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
			return ListaGridFactor;


		}

		public int AgregarValorCausa(int IdCausaAccidente, int idAccidente)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();

					// Verificar si ya existe un registro con los mismos idAccidente, idCausaAccidente y estatus = 1
					string verificaExistenciaQuery = "SELECT COUNT(*) FROM accidenteCausas " +
													 "WHERE idAccidente = @idAccidente " +
													 "AND idCausaAccidente = @idCausaAccidente " +
													 "AND estatus = 1";
					SqlCommand verificaExistenciaCommand = new SqlCommand(verificaExistenciaQuery, connection);
					verificaExistenciaCommand.Parameters.AddWithValue("@idAccidente", idAccidente);
					verificaExistenciaCommand.Parameters.AddWithValue("@idCausaAccidente", IdCausaAccidente);

					int registrosExistencia = (int)verificaExistenciaCommand.ExecuteScalar();

					if (registrosExistencia == 0)
					{
						string query = @"INSERT INTO accidenteCausas(idAccidente, idCausaAccidente, estatus, indice, fechaActualizacion)
VALUES (@idAccidente, @idCausaAccidente, 1, (SELECT ISNULL(MAX(indice), 0) + 1 FROM accidenteCausas WHERE idAccidente = @idAccidente AND idCausaAccidente <> 0),@fechaActualizacion)
";

						SqlCommand command = new SqlCommand(query, connection);

						command.Parameters.AddWithValue("@idCausaAccidente", IdCausaAccidente);
						command.Parameters.AddWithValue("@idAccidente", idAccidente);
                        command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

                        command.ExecuteNonQuery();
					}
					else
					{

						result = 2;
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
		}


		public void ActualizaIndiceCuasa(int idAccidenteCausa, int indice)
		{
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "UPDATE accidenteCausas SET indice = @indice WHERE idAccidenteCausa = @idCausaAccidente";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@idCausaAccidente", idAccidenteCausa);
					command.Parameters.AddWithValue("@indice", indice);

					command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					// Manejar la excepción
				}
				finally
				{
					connection.Close();
				}
			}
		}

		public int EditarValorCausa(int IdCausaAccidente, int idAccidenteCausa)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();

					// Verificar si ya existe un registro con los mismos idAccidente, idCausaAccidente y estatus = 1
					string verificaExistenciaQuery = "SELECT COUNT(*) FROM accidenteCausas " +
													 "WHERE idAccidente = (SELECT idAccidente FROM accidenteCausas WHERE idAccidenteCausa = @idAccidenteCausa) " +
													 "AND idCausaAccidente = @idCausaAccidente " +
													 "AND estatus = 1";
					SqlCommand verificaExistenciaCommand = new SqlCommand(verificaExistenciaQuery, connection);
					verificaExistenciaCommand.Parameters.AddWithValue("@idAccidenteCausa", idAccidenteCausa);
					verificaExistenciaCommand.Parameters.AddWithValue("@idCausaAccidente", IdCausaAccidente);

					int registrosExistencia = (int)verificaExistenciaCommand.ExecuteScalar();

					if (registrosExistencia == 0)
					{
						// No existe un registro con los mismos idAccidente, idCausaAccidente y estatus = 1, se puede actualizar
						string query = "UPDATE accidenteCausas SET idCausaAccidente = @idCausaAccidente,fechaActualizacion = @fechaActualizacion WHERE idAccidenteCausa = @idAccidenteCausa ";

						SqlCommand command = new SqlCommand(query, connection);

						command.Parameters.AddWithValue("@idCausaAccidente", IdCausaAccidente);
						command.Parameters.AddWithValue("@idAccidenteCausa", idAccidenteCausa);
                        command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

                        command.ExecuteNonQuery();
					}
					else
					{

						result = 2;
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
		}

		public void RecalcularIndex(int IdAccidente)
		{
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = @"
                    With UpdateData  As
                    (
                    SELECT idAccidenteCausa,idAccidente, idCausaAccidente, row_number() OVER (PARTITION BY idAccidente ORDER BY indice) indexs 
                    FROM accidenteCausas 
                    WHERE idAccidente = @idAccidente
                    and idCausaAccidente > 0
                    )
                    UPDATE accidenteCausas SET indice = indexs
                    FROM accidenteCausas
                    INNER JOIN UpdateData ON accidenteCausas.idAccidenteCausa = UpdateData.idAccidenteCausa and accidenteCausas.idAccidente = UpdateData.idAccidente ";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@idAccidente", IdAccidente);

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

		public int EliminarCausaBD(int IdCausaAccidente, int idAccidente, int idAccidenteCausa)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "UPDATE accidenteCausas SET idCausaAccidente = 0, fechaActualizacion = @fechaActualizacion WHERE idAccidente = @idAccidente AND idCausaAccidente = @idCausaAccidente AND idAccidenteCausa = @idAccidenteCausa ";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@idCausaAccidente", IdCausaAccidente);
					command.Parameters.AddWithValue("@idAccidente", idAccidente);
					command.Parameters.AddWithValue("@idAccidenteCausa", idAccidenteCausa);
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

		public int GetEstatusAccidente(int idAccidente)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "select idEstatusReporte from accidentes WHERE idAccidente = @idAccidente";


					SqlCommand command = new SqlCommand(query, connection);
					command.Parameters.AddWithValue("@idAccidente", idAccidente);

					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						result = (int)reader["idEstatusReporte"];
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
		public int GuardarDescripcion(int idAccidente, string descripcionCausa, int estatus = 1)
		{
			int result = 0;
			int estatusaccidente = 2;

			estatusaccidente = estatusaccidente > estatus ? estatusaccidente : estatus;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "UPDATE accidentes SET idEstatusReporte = @idEstatusReporte, fechaActualizacion = @fechaActualizacion";

					if (descripcionCausa != null)
					{
						query += ", descripcionCausas = @DescripcionCausa";
					}

					query += " WHERE idAccidente = @idAccidente";

					SqlCommand command = new SqlCommand(query, connection);
					command.Parameters.AddWithValue("@idAccidente", idAccidente);
					command.Parameters.AddWithValue("@idEstatusReporte", estatusaccidente);
                    command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

                    if (descripcionCausa != null)
					{
						command.Parameters.AddWithValue("@DescripcionCausa", descripcionCausa?.ToUpper());

                    }

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

		public List<CapturaAccidentesModel> ObtenerDatosGridCausa(int idAccidente)

		{
			//
			List<CapturaAccidentesModel> ListaGridCausa = new List<CapturaAccidentesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					int numeroContinuo = 1;
					connection.Open();
					SqlCommand command = new SqlCommand("SELECT ac.*,a.descripcionCausas, c.causaAccidente, ac.idAccidenteCausa, ac.indice FROM accidenteCausas ac " +
														"JOIN catCausasAccidentes c ON ac.idCausaAccidente = c.idCausaAccidente " +
														"LEFT JOIN accidentes AS a ON ac.idAccidente = a.idAccidente " +
														"WHERE ac.idAccidente = @idAccidente AND ac.idCausaAccidente > 0 Order By ac.indice", connection);
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idAccidente", idAccidente);

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CapturaAccidentesModel causa = new CapturaAccidentesModel();
							causa.idAccidenteCausa = Convert.ToInt32(reader["idAccidenteCausa"].ToString());

							causa.IdAccidente = Convert.ToInt32(reader["IdAccidente"].ToString());
							causa.IdCausaAccidente = Convert.ToInt32(reader["IdCausaAccidente"].ToString());
							causa.CausaAccidente = reader["causaAccidente"].ToString();
							causa.DescripcionCausa = reader["descripcionCausas"].ToString();
							string indx = reader["indice"].ToString();
							causa.indice = Convert.ToInt32((indx == string.Empty ? "0" : indx));
							causa.NumeroContinuo = numeroContinuo;
							ListaGridCausa.Add(causa);
							numeroContinuo++;


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
			return ListaGridCausa;


		}





		CapturaAccidentesModel ICapturaAccidentesService.ObtenerDetallePersona(int id)
		{
			CapturaAccidentesModel involucrado = new CapturaAccidentesModel();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand("usp_ObtieneDetallePersonaInvolucrada", connection);

					command.Parameters.Add(new SqlParameter("@idpersona", SqlDbType.NVarChar)).Value = id;
					command.CommandType = CommandType.StoredProcedure;
					using (
						SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							involucrado.IdPersona = reader.IsDBNull(reader.GetOrdinal("idPersona")) ? default(int) : Convert.ToInt32(reader["idPersona"]);
							involucrado.IdAsiento = reader.IsDBNull(reader.GetOrdinal("idPersona")) ? default(int) : Convert.ToInt32(reader["idPersona"]);
							involucrado.IdPersona = reader.IsDBNull(reader.GetOrdinal("idPersona")) ? default(int) : Convert.ToInt32(reader["idPersona"]);
							involucrado.IdPersona = reader.IsDBNull(reader.GetOrdinal("idPersona")) ? default(int) : Convert.ToInt32(reader["idPersona"]);
							involucrado.IdPersona = reader.IsDBNull(reader.GetOrdinal("idPersona")) ? default(int) : Convert.ToInt32(reader["idPersona"]);
							involucrado.IdPersona = reader.IsDBNull(reader.GetOrdinal("idPersona")) ? default(int) : Convert.ToInt32(reader["idPersona"]);

							involucrado.nombre = reader["nombre"].ToString();
							involucrado.apellidoPaterno = reader["apellidoPaterno"].ToString();
							involucrado.apellidoMaterno = reader["apellidoMaterno"].ToString();
							involucrado.RFC = reader["rfc"].ToString();
							involucrado.CURP = reader["curp"].ToString();
							involucrado.Calle = reader["calle"].ToString();
							involucrado.numeroLicencia = reader["numeroLicencia"].ToString();
							involucrado.Numero = reader["numero"].ToString();
							involucrado.Colonia = reader["colonia"].ToString();
							involucrado.Correo = reader["correo"].ToString();
							involucrado.FormatDateNacimiento = reader["fechaNacimiento"].ToString();

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

			return involucrado;

		}

		CapturaAccidentesModel ICapturaAccidentesService.DatosInvolucradoEdicion(int id, int idAccidente, int IdInvolucrado)
		{
			CapturaAccidentesModel involucrado = new CapturaAccidentesModel();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand("[usp_ObtieneDetallePersonaInvolucradaEdicion]", connection);

					command.Parameters.Add(new SqlParameter("@idpersona", SqlDbType.NVarChar)).Value = id;
					command.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.NVarChar)).Value = idAccidente;
					command.Parameters.Add(new SqlParameter("@idInvolucradosAccidente", SqlDbType.NVarChar)).Value = IdInvolucrado;

					command.CommandType = CommandType.StoredProcedure;
					using (
						SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							involucrado.IdInvolucrado = reader["idInvolucradosAccidente"] != DBNull.Value ?
								Convert.ToInt32(reader["idInvolucradosAccidente"]) :
								0;
							involucrado.IdVehiculo = reader["idVehiculo"] != DBNull.Value ?
								Convert.ToInt32(reader["idVehiculo"]) :
								0;
							involucrado.IdPersona = Convert.ToInt32(reader["idPersona"].ToString());
							involucrado.IdAsiento = Convert.ToInt32(reader["idAsiento"].ToString());
							if (involucrado.IdAsiento == 0)
							{
								involucrado.IdAsiento = null;
							}

							involucrado.nombre = reader["nombre"].ToString();
							involucrado.apellidoPaterno = reader["apellidoPaterno"].ToString();
							involucrado.apellidoMaterno = reader["apellidoMaterno"].ToString();
							involucrado.RFC = reader["rfc"].ToString();
							involucrado.CURP = reader["curp"].ToString();
							involucrado.Calle = reader["calle"].ToString();
							involucrado.numeroLicencia = reader["numeroLicencia"].ToString();
							involucrado.Numero = reader["numero"].ToString();
							involucrado.Colonia = reader["colonia"].ToString();
							involucrado.Placa = reader["placas"].ToString();
							involucrado.Tarjeta = reader["tarjeta"].ToString();
							involucrado.Serie = reader["serie"].ToString();
							involucrado.TipoVehiculo = reader["tipoVehiculo"].ToString();
							involucrado.Marca = reader["marcaVehiculo"].ToString();
							involucrado.Submarca = reader["nombreSubmarca"].ToString();
							involucrado.Municipio = reader["municipio"].ToString();
							involucrado.Entidad = reader["nombreEntidad"].ToString();
							involucrado.Genero = reader["genero"].ToString();
							involucrado.Telefono = reader["telefono"].ToString();

							involucrado.Modelo = reader["modelo"].ToString();
							involucrado.Propietario = $"{reader["propietarioNombre"]} {reader["apPaternoPropietario"]} {reader["apMaternoPropietario"]}";
							involucrado.Correo = reader["correo"].ToString();
							int fechaNacimientoIndex = reader.GetOrdinal("fechaNacimiento");

							// Verificar si el valor del campo es DBNull.Value
							if (!reader.IsDBNull(fechaNacimientoIndex))
							{
								DateTime fechaNacimiento = reader.GetDateTime(fechaNacimientoIndex);
								involucrado.FormatDateNacimiento = fechaNacimiento.ToString("dd/MM/yyyy");
							}
							else
							{
								involucrado.FormatDateNacimiento = ""; // O asignar cualquier otro valor predeterminado
							}

							involucrado.IdInstitucionTraslado = Convert.ToInt32(reader["idInstitucionTraslado"].ToString());
							involucrado.IdTipoInvolucrado = Convert.ToInt32(reader["idTipoInvolucrado"].ToString());
							involucrado.IdEstadoVictima = Convert.ToInt32(reader["idEstadoVictima"].ToString());
							involucrado.IdHospital = Convert.ToInt32(reader["idHospital"].ToString());
							involucrado.IdCinturon = Convert.ToInt32(reader["idCinturon"].ToString());
                            involucrado.IdAseguradora = reader["idAseguradora"] != DBNull.Value
                                ? Convert.ToInt32(reader["idAseguradora"])
                                : (int?)null; 
                            involucrado.Poliza = reader["poliza"].ToString();
							if (reader["fechaExpiracion"] != DBNull.Value)
							{
								DateTime fechaExpiracion;
								if (DateTime.TryParse(reader["fechaExpiracion"].ToString(), out fechaExpiracion))
								{
									involucrado.FechaExpiracionPoliza = fechaExpiracion;
								}
								else
								{
									// Manejar el caso en que la conversión a DateTime falla
									involucrado.FechaExpiracionPoliza = null; // o alguna otra lógica
								}
							}
							else
							{
								// Manejar el caso en que el valor es null
								involucrado.FechaExpiracionPoliza = null; // o DateTime.MinValue, según lo que necesites
							}

							//ARCHIVO DE INVOLUCRADO
							involucrado.archivoInvolucradoStr = reader["nombreArchivoInvolucrado"].ToString();
							involucrado.archivoInvolucradoPath = reader["rutaArchivoInvolucrado"].ToString();

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

			return involucrado;

		}


		public List<CapturaAccidentesModel> BusquedaPersonaInvolucrada(BusquedaInvolucradoModel model, string server = null)
		{
			//
			List<CapturaAccidentesModel> ListaInvolucrados = new List<CapturaAccidentesModel>();

			string condiciones = "";
			condiciones += string.IsNullOrEmpty(model.licencia) ? "" : " AND numeroLicencia LIKE '%' + @numeroLicencia + '%' ";
			condiciones += string.IsNullOrEmpty(model.curpBusqueda) ? "" : " AND curp LIKE '%' + @curp + '%'";
			condiciones += string.IsNullOrEmpty(model.rfcBusqueda) ? "" : " AND rfc LIKE '%' + @rfc + '%'";
			condiciones += string.IsNullOrEmpty(model.nombre) ? "" : " AND nombre LIKE '%' + @nombre + '%' ";
			condiciones += string.IsNullOrEmpty(model.apellidoPaterno) ? "" : " AND apellidoPaterno LIKE '%' + @apellidoPaterno + '%' ";
			condiciones += string.IsNullOrEmpty(model.apellidoMaterno) ? "" : " AND apellidoMaterno LIKE '%' + @apellidoMaterno + '%' ";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					connection.Open();

					string SqlTransact = @"SELECT * FROM personas WHERE estatus = 1 " + condiciones + " ORDER BY nombre";

					SqlCommand command = new SqlCommand(SqlTransact, connection);
					if (!string.IsNullOrEmpty(model.licencia))
						command.Parameters.Add(new SqlParameter("@numeroLicencia", SqlDbType.NVarChar)).Value = (object)model.licencia ?? DBNull.Value;

					if (!string.IsNullOrEmpty(model.curpBusqueda))
						command.Parameters.Add(new SqlParameter("@curp", SqlDbType.NVarChar)).Value = (object)model.curpBusqueda ?? DBNull.Value;

					if (!string.IsNullOrEmpty(model.rfcBusqueda))
						command.Parameters.Add(new SqlParameter("@rfc", SqlDbType.NVarChar)).Value = (object)model.rfcBusqueda ?? DBNull.Value;

					if (!string.IsNullOrEmpty(model.nombre))
						command.Parameters.Add(new SqlParameter("@nombre", SqlDbType.NVarChar)).Value = (object)model.nombre ?? DBNull.Value;

					if (!string.IsNullOrEmpty(model.apellidoPaterno))
						command.Parameters.Add(new SqlParameter("@apellidoPaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoPaterno ?? DBNull.Value;

					if (!string.IsNullOrEmpty(model.apellidoMaterno))
						command.Parameters.Add(new SqlParameter("@apellidoMaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoMaterno ?? DBNull.Value;

					command.CommandType = CommandType.Text;
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CapturaAccidentesModel involucrado = new CapturaAccidentesModel();
							involucrado.IdPersona = Convert.ToInt32(reader["idPersona"].ToString());
							involucrado.nombre = reader["nombre"].ToString();
							involucrado.apellidoPaterno = reader["apellidoPaterno"].ToString();
							involucrado.apellidoMaterno = reader["apellidoMaterno"].ToString();
							involucrado.RFC = reader["rfc"].ToString();
							involucrado.CURP = reader["curp"].ToString();
							involucrado.numeroLicencia = reader["numeroLicencia"].ToString();
							string fechaNac = reader["fechaNacimiento"].ToString();
							if (!fechaNac.IsNullOrEmpty())
								involucrado.fechaNacimiento = Convert.ToDateTime(fechaNac);

							ListaInvolucrados.Add(involucrado);
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

			return ListaInvolucrados;
		}
		public int AgregarPersonaInvolucrada(int idPersonaInvolucrado, int idAccidente)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "INSERT into involucradosAccidente(idAccidente,idPersona,fechaActualizacion) values(@idAccidente, @idPersonaInvolucrado,@fechaActualizacion)";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@idPersonaInvolucrado", idPersonaInvolucrado);
					command.Parameters.AddWithValue("@idAccidente", idAccidente);
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

		public int EliminarInvolucradoAcc(int IdVehiculoInvolucrado, int IdPropietarioInvolucrado, int IdAccidente)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = @"   DELETE FROM vehiculosAccidente
                                        WHERE   idAccidente = @IdAccidente 
                                                AND idPersona = @IdPropietarioInvolucrado 
                                                AND idVehiculo = @IdVehiculoInvolucrado;

                                        DELETE FROM conductoresVehiculosAccidente 
                                        WHERE   idAccidente = @IdAccidente 
                                                AND idPersona = @IdPropietarioInvolucrado 
                                                AND idVehiculo = @IdVehiculoInvolucrado;
                                        
                                        DELETE FROM opeConductoresVehiculosAccidentePersonas
                                        WHERE   idAccidente = @IdAccidente  AND idPersona = @IdPropietarioInvolucrado ";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@idAccidente", IdAccidente);
					command.Parameters.AddWithValue("@IdPropietarioInvolucrado", IdPropietarioInvolucrado);
					command.Parameters.AddWithValue("@IdVehiculoInvolucrado", IdVehiculoInvolucrado);


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

				//*******************************************************************
				//Eliminar del historico
				try
				{
					connection.Open();
					string query = "DELETE FROM opeAccidentesVehiculos WHERE idAccidente = @IdAccidente AND idVehiculo = @IdVehiculo;";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@idAccidente", IdAccidente);
					command.Parameters.AddWithValue("@IdVehiculo", IdVehiculoInvolucrado);


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
				//*******************************************************************


				return result;
			}
		}
		public List<CapturaAccidentesModel> VehiculosInvolucradosFiltro(int IdAccidente, Pagination pagination)
		{
			//
			List<CapturaAccidentesModel> ListaVehiculosInvolucrados = new List<CapturaAccidentesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					int numeroConsecutivo = 1;

					connection.Open();
					SqlCommand command = new SqlCommand("usp_ObtieneVehiculosInvolucradosFiltro", connection);

					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.AddWithValue("@idAccidente", IdAccidente);
					if (pagination.Filter.Trim() != "")
						command.Parameters.AddWithValue("@Filter", pagination.Filter);
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CapturaAccidentesModel vehiculo = new CapturaAccidentesModel();
							vehiculo.OrdenVehiculo = reader["Numv"] == DBNull.Value ? string.Empty : reader["Numv"].ToString();

							vehiculo.Edad = Convert.ToInt32(reader["Edad"]);
							vehiculo.FechaNacimientoTexto = reader["FechaNacimientoFormateada"].ToString();
							vehiculo.FechaVigenciaLicenciaTexto = reader["FechaVigenciaLicenciaFormateada"].ToString();
							vehiculo.VigenciaTarjetaFormateada = reader["VigenciaTarjetaFormateada"].ToString();
							vehiculo.IdPropietarioInvolucrado = Convert.IsDBNull(reader["idPersona"]) ? 0 : Convert.ToInt32(reader["idPersona"]);
							vehiculo.IdAccidente = Convert.IsDBNull(reader["idAccidente"]) ? 0 : Convert.ToInt32(reader["idAccidente"]);
							vehiculo.IdVehiculoInvolucrado = Convert.IsDBNull(reader["idVehiculo"]) ? 0 : Convert.ToInt32(reader["idVehiculo"]);
							vehiculo.IdTipoCarga = Convert.IsDBNull(reader["IdTipoCarga"]) ? 0 : Convert.ToInt32(reader["IdTipoCarga"]);
							vehiculo.IdPension = Convert.IsDBNull(reader["IdPension"]) ? 0 : Convert.ToInt32(reader["IdPension"]);
							vehiculo.IdFormaTrasladoInvolucrado = Convert.IsDBNull(reader["idFormaTraslado"]) ? 0 : Convert.ToInt32(reader["idFormaTraslado"]);
							vehiculo.idPersonaInvolucrado = Convert.IsDBNull(reader["IdConductor"]) ? 0 : Convert.ToInt32(reader["IdConductor"]);
							vehiculo.Placa = reader["placas"].ToString();
							vehiculo.fechaNacimiento = Convert.IsDBNull(reader["fechaNacimiento"]) ? DateTime.MinValue : Convert.ToDateTime(reader["fechaNacimiento"]);
							vehiculo.Tarjeta = reader["tarjeta"].ToString();
							vehiculo.TipoCarga = reader["tipoCarga"].ToString();
							vehiculo.Poliza = reader["poliza"].ToString();
							vehiculo.Serie = reader["serie"].ToString();
							vehiculo.Entidad = reader["nombreEntidad"].ToString();
							vehiculo.Marca = reader["marcaVehiculo"].ToString();
							vehiculo.Submarca = reader["nombreSubmarca"].ToString();
							vehiculo.TipoVehiculo = reader["tipoVehiculo"].ToString();
							vehiculo.PropietarioInvolucrado = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
							vehiculo.NumeroReporte = reader["numeroReporte"].ToString();
							vehiculo.Modelo = reader["modelo"].ToString();
							vehiculo.Motor = reader["motor"].ToString();
							vehiculo.Capacidad = reader["capacidad"].ToString();
							vehiculo.Pension = reader["pension"].ToString();
							vehiculo.Modelo = reader["modelo"].ToString();
							vehiculo.Color = reader["color"].ToString();
							vehiculo.RFC = reader["RFC"].ToString();
							vehiculo.CURP = reader["CURP"].ToString();
							vehiculo.TipoServicio = reader["tipoServicio"].ToString();
							vehiculo.FormaTrasladoInvolucrado = reader["formaTraslado"].ToString();
							vehiculo.Direccion = reader["direccion"].ToString();
							vehiculo.DireccionConductor = reader["direccionc"].ToString();
							vehiculo.Sexo = reader["genero"].ToString();
							vehiculo.numeroLicencia = reader["numeroLicencia"].ToString();
							vehiculo.Grua = reader["grua"].ToString();
							vehiculo.TipoLicencia = reader["tipoLicencia"].ToString();
							vehiculo.ConductorInvolucrado = $"{reader["nombreConductor"]} {reader["apellidoPConductor"]} {reader["apellidoMConductor"]}";
							vehiculo.vigenciaLicencia = reader["vigenciaLicencia"].GetType() == typeof(DBNull) ? DateTime.MinValue : (DateTime)reader["vigenciaLicencia"];
							vehiculo.NumeroEconomico = reader["numeroeconomico"].GetType() == typeof(DBNull) ? "" : reader["numeroeconomico"].ToString();
							vehiculo.IdVehiculo = Convert.IsDBNull(reader["idVehiculo"]) ? 0 : Convert.ToInt32(reader["idVehiculo"]);
							//vehiculo.MontoDanos = Convert.ToDecimal(reader["MontoDanos"]);
							string montoVehiculoString = reader["montoVehiculo"].ToString();
							float montoVehiculo;

							if (!string.IsNullOrEmpty(montoVehiculoString) && float.TryParse(montoVehiculoString, out montoVehiculo))
							{
								vehiculo.montoVehiculo = montoVehiculo;
							}
							else
							{
								vehiculo.montoVehiculo = 0.0f;
							}
							vehiculo.numeroConsecutivo = numeroConsecutivo;//Convert.ToInt32(reader["Numv"].ToString());

							ListaVehiculosInvolucrados.Add(vehiculo);
							numeroConsecutivo++;
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
			return ListaVehiculosInvolucrados;


		}
		public List<CapturaAccidentesModel> VehiculosInvolucrados(int IdAccidente)
		{
			//
			List<CapturaAccidentesModel> ListaVehiculosInvolucrados = new List<CapturaAccidentesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					int numeroConsecutivo = 1;

					connection.Open();
					SqlCommand command = new SqlCommand("usp_ObtieneVehiculosInvolucrados", connection);

					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.AddWithValue("@idAccidente", IdAccidente);

					using ( SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CapturaAccidentesModel vehiculo = new CapturaAccidentesModel();
							vehiculo.Edad = Convert.ToInt32(reader["Edad"]);
							vehiculo.FechaNacimientoTexto = reader["FechaNacimientoFormateada"].ToString();
							vehiculo.FechaVigenciaLicenciaTexto = reader["FechaVigenciaLicenciaFormateada"].ToString();
							vehiculo.VigenciaTarjetaFormateada = reader["VigenciaTarjetaFormateada"].ToString();
							vehiculo.IdPropietarioInvolucrado = Convert.IsDBNull(reader["idPersona"]) ? 0 : Convert.ToInt32(reader["idPersona"]);
							vehiculo.IdAccidente = Convert.IsDBNull(reader["idAccidente"]) ? 0 : Convert.ToInt32(reader["idAccidente"]);
							vehiculo.IdVehiculoInvolucrado = Convert.IsDBNull(reader["idVehiculo"]) ? 0 : Convert.ToInt32(reader["idVehiculo"]);
							vehiculo.IdTipoCarga = Convert.IsDBNull(reader["IdTipoCarga"]) ? 0 : Convert.ToInt32(reader["IdTipoCarga"]);
							vehiculo.IdPension = Convert.IsDBNull(reader["IdPension"]) ? 0 : Convert.ToInt32(reader["IdPension"]);
							vehiculo.IdFormaTrasladoInvolucrado = Convert.IsDBNull(reader["idFormaTraslado"]) ? 0 : Convert.ToInt32(reader["idFormaTraslado"]);
							vehiculo.idPersonaInvolucrado = Convert.IsDBNull(reader["IdConductor"]) ? 0 : Convert.ToInt32(reader["IdConductor"]);
							vehiculo.Placa = reader["placas"].ToString();
							vehiculo.fechaNacimiento = Convert.IsDBNull(reader["fechaNacimiento"]) ? DateTime.MinValue : Convert.ToDateTime(reader["fechaNacimiento"]);
							vehiculo.Tarjeta = reader["tarjeta"].ToString();
							vehiculo.TipoCarga = reader["tipoCarga"].ToString();
							vehiculo.Poliza = reader["poliza"].ToString();
							vehiculo.Serie = reader["serie"].ToString();
							vehiculo.Entidad = reader["nombreEntidad"].ToString();
							vehiculo.Marca = reader["marcaVehiculo"].ToString();
							vehiculo.Submarca = reader["nombreSubmarca"].ToString();
							vehiculo.TipoVehiculo = reader["tipoVehiculo"].ToString();
							vehiculo.PropietarioInvolucrado = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
							vehiculo.NumeroReporte = reader["numeroReporte"].ToString();
							vehiculo.Modelo = reader["modelo"].ToString();
							vehiculo.Motor = reader["motor"].ToString();
							vehiculo.Capacidad = reader["capacidad"].ToString();
							vehiculo.Pension = reader["pension"].ToString();
							vehiculo.Modelo = reader["modelo"].ToString();
							vehiculo.Color = reader["color"].ToString();
							vehiculo.RFC = reader["RFC"].ToString();
							vehiculo.CURP = reader["CURP"].ToString();
							vehiculo.TipoServicio = reader["tipoServicio"].ToString();
							vehiculo.FormaTrasladoInvolucrado = reader["formaTraslado"].ToString();
							vehiculo.Direccion = reader["direccion"].ToString();
							vehiculo.DireccionConductor = reader["direccionc"].ToString();
							vehiculo.Sexo = reader["genero"].ToString();
							vehiculo.numeroLicencia = reader["numeroLicencia"].ToString();
							vehiculo.Grua = reader["grua"].ToString();
							vehiculo.TipoLicencia = reader["tipoLicencia"].ToString();
							vehiculo.ConductorInvolucrado = $"{reader["nombreConductor"]} {reader["apellidoPConductor"]} {reader["apellidoMConductor"]}";
							vehiculo.vigenciaLicencia = reader["vigenciaLicencia"].GetType() == typeof(DBNull) ? DateTime.MinValue : (DateTime)reader["vigenciaLicencia"];
							vehiculo.NumeroEconomico = reader["numeEco"].GetType() == typeof(DBNull) ? "" : reader["numeEco"].ToString();
							vehiculo.IdVehiculo = Convert.IsDBNull(reader["idVehiculo"]) ? 0 : Convert.ToInt32(reader["idVehiculo"]);
							//vehiculo.MontoDanos = Convert.ToDecimal(reader["MontoDanos"]);
							vehiculo.concesionario = reader["concesionario"].GetType() == typeof(DBNull) ? "" : reader["concesionario"].ToString();

							string montoVehiculoString = reader["montoVehiculo"].ToString();
							float montoVehiculo;

							if (!string.IsNullOrEmpty(montoVehiculoString) && float.TryParse(montoVehiculoString, out montoVehiculo))
							{
								vehiculo.montoVehiculo = montoVehiculo;
							}
							else
							{
								vehiculo.montoVehiculo = 0.0f;
							}
							vehiculo.numeroConsecutivo = numeroConsecutivo;

							ListaVehiculosInvolucrados.Add(vehiculo);
							numeroConsecutivo++;
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
			return ListaVehiculosInvolucrados;


		}
		public List<CapturaAccidentesModel> VehiculosInvolucradosParaInfracciones(int IdAccidente)
		{
			//
			List<CapturaAccidentesModel> ListaVehiculosInvolucrados = new List<CapturaAccidentesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					int numeroConsecutivo = 1;

					connection.Open();
					SqlCommand command = new SqlCommand("usp_ObtieneVehiculosInvolucrados", connection);

					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.AddWithValue("@idAccidente", IdAccidente);

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CapturaAccidentesModel vehiculo = new CapturaAccidentesModel();
							vehiculo.Edad = Convert.ToInt32(reader["Edad"]);
							vehiculo.FechaNacimientoTexto = reader["FechaNacimientoFormateada"].ToString();
							vehiculo.FechaVigenciaLicenciaTexto = reader["FechaVigenciaLicenciaFormateada"].ToString();
							vehiculo.VigenciaTarjetaFormateada = reader["VigenciaTarjetaFormateada"].ToString();
							vehiculo.IdPropietarioInvolucrado = Convert.IsDBNull(reader["idPropietario"]) ? 0 : Convert.ToInt32(reader["idPropietario"]);
							vehiculo.IdAccidente = Convert.IsDBNull(reader["idAccidente"]) ? 0 : Convert.ToInt32(reader["idAccidente"]);
							vehiculo.IdVehiculoInvolucrado = Convert.IsDBNull(reader["idVehiculo"]) ? 0 : Convert.ToInt32(reader["idVehiculo"]);
							vehiculo.IdDelegacion = Convert.IsDBNull(reader["idOficinaDelegacion"]) ? 0 : Convert.ToInt32(reader["idOficinaDelegacion"]);

							vehiculo.IdTipoCarga = Convert.IsDBNull(reader["IdTipoCarga"]) ? 0 : Convert.ToInt32(reader["IdTipoCarga"]);
							vehiculo.IdPension = Convert.IsDBNull(reader["IdPension"]) ? 0 : Convert.ToInt32(reader["IdPension"]);
							vehiculo.IdFormaTrasladoInvolucrado = Convert.IsDBNull(reader["idFormaTraslado"]) ? 0 : Convert.ToInt32(reader["idFormaTraslado"]);
							vehiculo.idPersonaInvolucrado = Convert.IsDBNull(reader["IdConductor"]) ? 0 : Convert.ToInt32(reader["IdConductor"]);
							vehiculo.Placa = reader["placas"].ToString();
							vehiculo.fechaNacimiento = Convert.IsDBNull(reader["fechaNacimiento"]) ? DateTime.MinValue : Convert.ToDateTime(reader["fechaNacimiento"]);
							vehiculo.Tarjeta = reader["tarjeta"].ToString();
							vehiculo.TipoCarga = reader["tipoCarga"].ToString();
							vehiculo.Poliza = reader["poliza"].ToString();
							vehiculo.Serie = reader["serie"].ToString();
							vehiculo.Entidad = reader["nombreEntidad"].ToString();
							vehiculo.Marca = reader["marcaVehiculo"].ToString();
							vehiculo.Submarca = reader["nombreSubmarca"].ToString();
							vehiculo.TipoVehiculo = reader["tipoVehiculo"].ToString();
							vehiculo.PropietarioInvolucrado = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
							vehiculo.NumeroReporte = reader["numeroReporte"].ToString();
							vehiculo.Modelo = reader["modelo"].ToString();
							vehiculo.Motor = reader["motor"].ToString();
							vehiculo.Capacidad = reader["capacidad"].ToString();
							vehiculo.Pension = reader["pension"].ToString();
							vehiculo.Modelo = reader["modelo"].ToString();
							vehiculo.Color = reader["color"].ToString();
							vehiculo.RFC = reader["RFC"].ToString();
							vehiculo.CURP = reader["CURP"].ToString();
							vehiculo.TipoServicio = reader["tipoServicio"].ToString();
							vehiculo.FormaTrasladoInvolucrado = reader["formaTraslado"].ToString();
							vehiculo.Direccion = reader["direccion"].ToString();
							vehiculo.DireccionConductor = reader["direccionc"].ToString();
							vehiculo.Sexo = reader["genero"].ToString();
							vehiculo.numeroLicencia = reader["numeroLicencia"].ToString();
							vehiculo.Grua = reader["grua"].ToString();
							vehiculo.TipoLicencia = reader["tipoLicencia"].ToString();
							vehiculo.ConductorInvolucrado = $"{reader["nombreConductor"]} {reader["apellidoPConductor"]} {reader["apellidoMConductor"]}";
							vehiculo.vigenciaLicencia = reader["vigenciaLicencia"].GetType() == typeof(DBNull) ? DateTime.MinValue : (DateTime)reader["vigenciaLicencia"];
							vehiculo.NumeroEconomico = reader["numeroeconomico"].GetType() == typeof(DBNull) ? "" : reader["numeroeconomico"].ToString();
							vehiculo.IdVehiculo = Convert.IsDBNull(reader["idVehiculo"]) ? 0 : Convert.ToInt32(reader["idVehiculo"]);
							//vehiculo.MontoDanos = Convert.ToDecimal(reader["MontoDanos"]);
							string montoVehiculoString = reader["montoVehiculo"].ToString();
							float montoVehiculo;

							if (!string.IsNullOrEmpty(montoVehiculoString) && float.TryParse(montoVehiculoString, out montoVehiculo))
							{
								vehiculo.montoVehiculo = montoVehiculo;
							}
							else
							{
								vehiculo.montoVehiculo = 0.0f;
							}
							vehiculo.numeroConsecutivo = numeroConsecutivo;
							if (!ExisteInfraccionParaVehiculo(IdAccidente, vehiculo.IdVehiculo))
							{
								ListaVehiculosInvolucrados.Add(vehiculo);
							}

							numeroConsecutivo++;
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
			return ListaVehiculosInvolucrados;


		}

		private bool ExisteInfraccionParaVehiculo(int idAccidente, int idVehiculo)
		{
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM infraccionesAccidente WHERE idAccidente = @idAccidente AND idVehiculo = @idVehiculo", connection);
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@idAccidente", idAccidente);
				command.Parameters.AddWithValue("@idVehiculo", idVehiculo);

				connection.Open();
				int count = (int)command.ExecuteScalar();

				return count > 0;
			}
		}


		public int GuardarComplementoVehiculo(CapturaAccidentesModel model, int IdVehiculo, int idAccidente)
		{
			int result = 0;
			string strQuery = @"
                                IF EXISTS (SELECT 1 FROM conductoresVehiculosAccidente WHERE idVehiculo = @IdVehiculo AND idAccidente = @IdAccidente)
                                    UPDATE conductoresVehiculosAccidente
                                    SET idTipoCarga = @IdTipoCarga,
                                        poliza = @Poliza,
                                        idDelegacion = @IdDelegacion,
                                        idPension = @IdPension,
                                        idFormaTraslado = @IdFormaTraslado,
                                        fechaActualizacion = getdate(),
                                        actualizadoPor = @actualizadoPor,
                                        estatus = @estatus
                                    WHERE idVehiculo = @IdVehiculo AND idAccidente = @IdAccidente;";

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand(strQuery, connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@IdTipoCarga", SqlDbType.Int)).Value = (object)model.IdTipoCarga ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@Poliza", SqlDbType.NVarChar)).Value = (object)model.Poliza?.ToUpper() ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdDelegacion", SqlDbType.Int)).Value = (object)model.IdDelegacion ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdPension", SqlDbType.Int)).Value = (object)model.IdPension ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdFormaTraslado", SqlDbType.Int)).Value = (object)model.IdFormaTraslado ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdVehiculo", SqlDbType.Int)).Value = (object)IdVehiculo ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@IdAccidente", SqlDbType.Int)).Value = (object)idAccidente ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
					command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;

					result = command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					// Manejar la excepción
				}
				finally
				{
					connection.Close();
				}
			}

			return result;
		}

		public int AgregarMontoV(MontoModel model)

		{
			int result = 0;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand sqlCommand = new
						SqlCommand("Update vehiculosAccidente set montoVehiculo = @montoVehiculo, fechaActualizacion = @fechaActualizacion  where idAccidente=@idAccidente AND idvehiculo = @idVehiculo",
						connection);
					sqlCommand.Parameters.Add(new SqlParameter("@montoVehiculo", SqlDbType.Float)).Value = model.montoVehiculo;
					sqlCommand.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.Int)).Value = model.IdAccidente;
					sqlCommand.Parameters.Add(new SqlParameter("@idVehiculo", SqlDbType.Int)).Value = model.IdVehiculoInvolucrado;
					sqlCommand.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = model.IdPropietarioInvolucrado;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;

                    sqlCommand.CommandType = CommandType.Text;
					result = sqlCommand.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					//---Log
					return result;
				}
				finally
				{
					connection.Close();
				}
			}
			return result;


		}
		public List<CapturaAccidentesModel> InfraccionesVehiculosAccidete(int idAccidente)
		{
			//
			List<CapturaAccidentesModel> ListaVehiculosInfracciones = new List<CapturaAccidentesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{

					// en caso de no haber por accidente se mostraran las infracciones de los vehiculo
					if (ListaVehiculosInfracciones.Count == 0)
					{
						connection.Open();
						SqlCommand command = new SqlCommand(@"SELECT	
                                MAX(i.horainfraccion) AS horainfraccion,
                                MAX(CASE WHEN i.infraccionCortesia = 2 THEN 'Cortesia' ELSE 'No Cortesia' END) AS TipoCortesia,
                                Max(v.idVehiculo) AS idVehiculo,
                                MAX(v.placas) AS placas,
                                MAX(propietario.idPersona) AS propietario,
                                MAX(i.folioInfraccion) AS folioInfraccion,
                                MAX(i.fechaInfraccion) AS fechaInfraccion,
                                MAX(i.idInfraccion) AS idInfraccion,
                                MAX(i.idPersona) AS idPersona,
                                MAX(i.idPersonaConductor) AS idPersonaConductor,
                                MAX(conductor.idPersona) AS conductor,
                                MAX(ent.nombreEntidad) AS nombreEntidad,
                                MAX(propietario.nombre) AS propietario_nombre,
                                MAX(propietario.apellidoPaterno) AS propietario_apellidoPaterno,
                                MAX(propietario.apellidoMaterno) AS propietario_apellidoMaterno,
                                MAX(conductor.nombre) AS conductor_nombre,
                                MAX(conductor.apellidoPaterno) AS conductor_apellidoPaterno,
                                MAX(conductor.apellidoMaterno) AS conductor_apellidoMaterno
                            FROM 
                                conductoresVehiculosAccidente AS cva
                            LEFT JOIN 
                                vehiculos AS v ON cva.idVehiculo = v.idVehiculo
                            LEFT JOIN 
                                catEntidades AS ent ON v.idEntidad = ent.idEntidad
                            LEFT JOIN 
                                infracciones AS i ON cva.idVehiculo = i.idVehiculo
                            LEFT JOIN 
                                personas AS propietario ON v.idPersona = propietario.idPersona
                            LEFT JOIN 
                                opeInfraccionesPersonas AS conductor ON i.idPersonaConductor = conductor.idPersona AND i.idInfraccion = conductor.idInfraccion

							LEFT JOIN
								infraccionesAccidente AS ia ON ia.idInfraccion = i.idInfraccion  AND cva.idAccidente = ia.idAccidente
                            WHERE 
                                cva.idAccidente = @idAccidente
                                AND i.idInfraccion IS NOT NULL
								and ia.idInf_Acc IS NULL
                                and i.estatus = 1
                            GROUP BY 
                                i.idInfraccion;
                            ", connection);

						command.CommandType = CommandType.Text;
						command.Parameters.AddWithValue("@idAccidente", idAccidente);
						// command.Parameters.AddWithValue("@idDependencia", idDependencia);

						using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
						{
							while (reader.Read())
							{
								CapturaAccidentesModel elemnto = new CapturaAccidentesModel();
								elemnto.IdVehiculo = Convert.ToInt32(reader["IdVehiculo"].ToString());
								elemnto.IdInfraccion = Convert.ToInt32(reader["IdInfraccion"].ToString());
								elemnto.Placa = reader["placas"].ToString();
								elemnto.folioInfraccion = reader["folioInfraccion"].ToString();

								var aux = reader["horainfraccion"].ToString();
								var timeint = Convert.ToInt32(aux);
								var Min = (timeint % 100);
								var hour = (timeint - Min) / 100;

								elemnto.Fecha = Convert.ToDateTime(reader["fechaInfraccion"].ToString());

								elemnto.Fecha = elemnto.Fecha.Value.AddHours((double)hour).AddMinutes((double)Min);


								elemnto.ConductorInvolucrado = reader["conductor_nombre"].ToString() + " " + reader["conductor_apellidoPaterno"].ToString() + " " + reader["conductor_apellidoMaterno"].ToString();
								elemnto.Propietario = reader["propietario_nombre"].ToString() + " " + reader["propietario_apellidoPaterno"].ToString() + " " + reader["propietario_apellidoMaterno"].ToString();
								elemnto.EntidadRegistro = reader["nombreEntidad"].ToString();
								elemnto.Cortesia = reader["TipoCortesia"].ToString();

								ListaVehiculosInfracciones.Add(elemnto);

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

			ListaVehiculosInfracciones = ListaVehiculosInfracciones.OrderByDescending(x => x.Fecha).ToList();

			return ListaVehiculosInfracciones;


		}
		public int RelacionAccidenteInfraccion(int IdVehiculo, int idAccidente, int IdInfraccion)
		{
			int infraccionAgregada = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "INSERT INTO infraccionesAccidente (idVehiculo, idAccidente,idInfraccion,estatus,fechaActualizacion) VALUES (@IdVehiculo, @idAccidente, @IdInfraccion, @estatus, @fechaActualizacion)";

					SqlCommand command = new SqlCommand(query, connection);
					command.Parameters.AddWithValue("@idVehiculo", IdVehiculo);
					command.Parameters.AddWithValue("@idAccidente", idAccidente);
					command.Parameters.AddWithValue("@idInfraccion", IdInfraccion);
					command.Parameters.AddWithValue("@estatus", 1);
                    command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

                    command.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					// Manejar la excepción
				}
				finally
				{
					connection.Close();
				}
			}

			return infraccionAgregada;
		}

		public int RelacionAccidenteInfraccion2(int IdVehiculo, int idAccidente, int IdInfraccion)
		{
			int infraccionAgregada = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();

					// Verificar si ya existe un registro con los mismos idAccidente, idVehiculo y estatus = 1
					string checkQuery = "SELECT COUNT(*) FROM infraccionesAccidente WHERE idVehiculo = @idVehiculo AND idAccidente = @idAccidente AND estatus = @estatus";
					SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
					checkCommand.Parameters.AddWithValue("@idVehiculo", IdVehiculo);
					checkCommand.Parameters.AddWithValue("@idAccidente", idAccidente);
					checkCommand.Parameters.AddWithValue("@estatus", 1);

					int count = (int)checkCommand.ExecuteScalar();

					if (count > 0)
					{
						return 2;
					}

					// Si no existe un registro, realizar el INSERT
					string insertQuery = "INSERT INTO infraccionesAccidente (idVehiculo, idAccidente, idInfraccion, estatus,fechaActualizacion) VALUES (@idVehiculo, @idAccidente, @IdInfraccion, @estatus, @fechaActualizacion)";
					SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
					insertCommand.Parameters.AddWithValue("@idVehiculo", IdVehiculo);
					insertCommand.Parameters.AddWithValue("@idAccidente", idAccidente);
					insertCommand.Parameters.AddWithValue("@IdInfraccion", IdInfraccion);
					insertCommand.Parameters.AddWithValue("@estatus", 1);
                    insertCommand.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

                    insertCommand.ExecuteNonQuery();
					infraccionAgregada = 1; // Puedes cambiar este valor si quieres indicar que se ha insertado con éxito
				}
				catch (SqlException ex)
				{
					// Manejar la excepción
				}
				finally
				{
					connection.Close();
				}
			}

			return infraccionAgregada;
		}

		public int VerificarExistenciaInfraccion(int IdVehiculo, int idAccidente)
		{
			int infraccionAgregada = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();

					// Verificar si ya existe un registro con los mismos idAccidente, idVehiculo y estatus = 1
					string checkQuery = "SELECT COUNT(*) FROM infraccionesAccidente WHERE idVehiculo = @idVehiculo AND idAccidente = @idAccidente AND estatus = @estatus";
					SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
					checkCommand.Parameters.AddWithValue("@idVehiculo", IdVehiculo);
					checkCommand.Parameters.AddWithValue("@idAccidente", idAccidente);
					checkCommand.Parameters.AddWithValue("@estatus", 1);

					int count = (int)checkCommand.ExecuteScalar();

					if (count > 0)
					{
						return 2;
					}

					infraccionAgregada = 1; // Puedes cambiar este valor si quieres indicar que se ha insertado con éxito
				}
				catch (SqlException ex)
				{
					// Manejar la excepción
				}
				finally
				{
					connection.Close();
				}
			}

			return infraccionAgregada;
		}

		public List<CapturaAccidentesModel> InfraccionesDeAccidente(int idAccidente)
		{
			//
			List<CapturaAccidentesModel> ListaInfracciones = new List<CapturaAccidentesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"SELECT MAX(ia.idInf_Acc) AS idInf_Acc, MAX(ia.idAccidente) AS idAccidente, MAX(ia.idVehiculo) AS idVehiculo, 
                                                MAX(v.idMarcaVehiculo) AS idMarcaVehiculo, MAX(v.idSubmarca) AS idMarcaVehiculo, MAX(v.placas) AS placas, MAX(v.modelo) AS modelo, 
                                                MAX(a.idEstatusReporte) AS idEstatusReporte, 
                                                MAX(a.numeroReporte) AS numeroReporte, 
                                                MAX(i.folioInfraccion) AS folioInfraccion, 
                                                MAX(cei.estatusInfraccion) AS estatusInfraccion, 
                                                MAX(i.idEstatusInfraccion) AS idEstatusInfraccion, 
                                                MAX(mv.marcaVehiculo) AS marcaVehiculo, MAX(sv.nombreSubmarca) AS nombreSubmarca, MAX(i.idInfraccion) AS idInfraccion, 
                                                ISNULL(MAX(gr.garantia),'') garantia 
                                                FROM infraccionesAccidente AS ia JOIN vehiculos AS v ON ia.idVehiculo = v.idVehiculo 
                                                LEFT JOIN accidentes AS a ON ia.idAccidente = a.idAccidente 
                                                LEFT JOIN infracciones AS i ON ia.idInfraccion = i.idInfraccion 
                                                LEFT JOIN catEstatusInfraccion AS cei ON cei.idEstatusInfraccion = i.idEstatusInfraccion 
                                                LEFT JOIN catMarcasVehiculos AS mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo 
                                                LEFT JOIN catSubmarcasVehiculos AS sv ON v.idSubmarca = sv.idSubmarca 
                                                LEFT JOIN garantiasInfraccion AS gi ON gi.idInfraccion = ia.idInfraccion 
                                                LEFT JOIN catGarantias AS gr ON gr.idGarantia = gi.idCatGarantia 
                                                WHERE ia.idAccidente = @idAccidente AND ia.estatus != 0  
                                                group by ia.idAccidente, ia.idInfraccion, ia.idVehiculo, i.folioInfraccion 
                                                 ORDER BY  MAX(ia.idInf_Acc) DESC;", connection);



					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idAccidente", idAccidente);
					// command.Parameters.AddWithValue("@idDependencia", idDependencia);


					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CapturaAccidentesModel elemnto = new CapturaAccidentesModel();
							elemnto.IdInfraccion = Convert.IsDBNull(reader["idInfraccion"]) ? 0 : Convert.ToInt32(reader["idInfraccion"]);
							elemnto.IdInfAcc = Convert.IsDBNull(reader["IdInf_Acc"]) ? 0 : Convert.ToInt32(reader["IdInf_Acc"]);
							elemnto.IdAccidente = Convert.IsDBNull(reader["IdAccidente"]) ? 0 : Convert.ToInt32(reader["IdAccidente"]);
							elemnto.IdVehiculoInvolucrado = Convert.IsDBNull(reader["IdVehiculo"]) ? 0 : Convert.ToInt32(reader["IdVehiculo"]);
							elemnto.NumeroReporte = reader["numeroReporte"].ToString();
							elemnto.Placa = reader["placas"].ToString();
							elemnto.EstatusInfraccion = reader["estatusInfraccion"].ToString();
							elemnto.folioInfraccion = reader["folioInfraccion"].ToString();
							elemnto.garantia = reader["garantia"].ToString();
							// elemnto.EstatusReporte = reader["estatusReporte"].ToString();
							elemnto.Vehiculo = $"{reader["marcaVehiculo"]} {reader["nombreSubmarca"]} {reader["placas"]} {reader["modelo"]}";
							elemnto.IdVehiculo = Convert.IsDBNull(reader["IdVehiculo"]) ? 0 : Convert.ToInt32(reader["IdVehiculo"]);


							ListaInfracciones.Add(elemnto);

						}

					}

				}
				catch (SqlException ex)
				{
					Console.WriteLine("Error al ejecutar la consulta SQL: " + ex.Message);
				}
				finally
				{
					connection.Close();
				}
			return ListaInfracciones;


		}
		public int RelacionPersonaVehiculo(int IdPersona, int idAccidente, int IdVehiculoInvolucrado, int IdInvolucrado)
		{
			int result = 0;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();

					// Verificar si IdInvolucrado no es nulo y si existe en la tabla
					if (IdInvolucrado != 0)
					{
						string queryExistencia = "SELECT COUNT(*) FROM involucradosAccidente WHERE IdInvolucradosAccidente = @IdInvolucrado";
						SqlCommand commandExistencia = new SqlCommand(queryExistencia, connection);
						commandExistencia.Parameters.AddWithValue("@IdInvolucrado", IdInvolucrado);
						int count = (int)commandExistencia.ExecuteScalar();

						if (count > 0)
						{
							// Si existe, ejecutar un UPDATE en lugar de un INSERT
							string queryUpdate = @"UPDATE involucradosAccidente SET idPersona = @idPersona, idAccidente = @idAccidente, idVehiculo = @idVehiculoInvolucrado, fechaActualizacion = @fechaActualizacion WHERE IdInvolucradosAccidente = @IdInvolucrado";
							SqlCommand commandUpdate = new SqlCommand(queryUpdate, connection);
							commandUpdate.Parameters.AddWithValue("@idPersona", IdPersona);
							commandUpdate.Parameters.AddWithValue("@idAccidente", idAccidente);
							commandUpdate.Parameters.AddWithValue("@idVehiculoInvolucrado", IdVehiculoInvolucrado);
							commandUpdate.Parameters.AddWithValue("@IdInvolucrado", IdInvolucrado);
                            commandUpdate.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

                            commandUpdate.ExecuteNonQuery();
						}
						else
						{
							// Si no existe, continuar con el INSERT
							string queryInsert = "INSERT into involucradosAccidente(idPersona,idAccidente,idVehiculo,fechaActualizacion) values(@idPersona, @idAccidente, @idVehiculoInvolucrado,@fechaActualizacion)";
							SqlCommand commandInsert = new SqlCommand(queryInsert, connection);
							commandInsert.Parameters.AddWithValue("@idPersona", IdPersona);
							commandInsert.Parameters.AddWithValue("@idAccidente", idAccidente);
							commandInsert.Parameters.AddWithValue("@idVehiculoInvolucrado", IdVehiculoInvolucrado);
                            commandInsert.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

                            commandInsert.ExecuteNonQuery();
						}
					}
					else
					{
						// Si IdInvolucrado es null, continuar con el INSERT
						string queryInsert = "INSERT into involucradosAccidente(idPersona,idAccidente,idVehiculo,fechaActualizacion) values(@idPersona, @idAccidente, @idVehiculoInvolucrado,@fechaActualizacion)";
						SqlCommand commandInsert = new SqlCommand(queryInsert, connection);
						commandInsert.Parameters.AddWithValue("@idPersona", IdPersona);
						commandInsert.Parameters.AddWithValue("@idAccidente", idAccidente);
						commandInsert.Parameters.AddWithValue("@idVehiculoInvolucrado", IdVehiculoInvolucrado);
                        commandInsert.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

                        commandInsert.ExecuteNonQuery();
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
		public int ActualizarInvolucrado(CapturaAccidentesModel model, int idAccidente)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{


					SqlCommand cmd = new SqlCommand("usp_InsertaActualizaInvolucrado", connection);


					cmd.Parameters.AddWithValue("@idAccidente", idAccidente);
					cmd.Parameters.AddWithValue("@idPersona", model.IdPersona);
					if (model.IdVehiculo != null && model.IdVehiculo != 0)
					{
						cmd.Parameters.AddWithValue("@idVehiculo", model.IdVehiculo);
					}
					else
					{
						cmd.Parameters.AddWithValue("@idVehiculo", 0);
					}
					cmd.Parameters.AddWithValue("@idTipoInvolucrado", model.IdTipoInvolucrado);
					cmd.Parameters.AddWithValue("@idEstadoVictima", model.IdEstadoVictima);
					cmd.Parameters.AddWithValue("@idHospital", model.IdHospital);
					cmd.Parameters.AddWithValue("@idInstitucionTraslado", model.IdInstitucionTraslado);
					cmd.Parameters.AddWithValue("@idAsiento", model.IdAsiento ?? 0);
					cmd.Parameters.AddWithValue("@idCinturon", model.IdCinturon);
					cmd.Parameters.AddWithValue("@estatus", 1);

					result = cmd.ExecuteNonQuery();


				}

				catch (Exception ex)
				{
					// Manejar excepciones
				}
			}

			return result;

		}

		public int ActualizarInvolucrado2(CapturaAccidentesModel model, int idAccidente)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					string consultaExistencia = @"usp_InsertaInvolucradoPersonasValidate";

					SqlCommand comandoInsert = new SqlCommand(consultaExistencia, connection);
					comandoInsert.CommandType = CommandType.StoredProcedure;

					comandoInsert.Parameters.AddWithValue("@idAccidente", idAccidente);
					comandoInsert.Parameters.AddWithValue("@idPersona", model.IdPersona);
					comandoInsert.Parameters.AddWithValue("@idVehiculo", model.IdVehiculo);
					//comandoInsert.Parameters.AddWithValue("@idVehiculoNew", model.IdVehiculoNuevo != null ? model.IdVehiculoNuevo : 0);

					comandoInsert.Parameters.AddWithValue("@idTipoInvolucrado", model.IdTipoInvolucrado);
					comandoInsert.Parameters.AddWithValue("@idEstadoVictima", model.IdEstadoVictima);
					comandoInsert.Parameters.AddWithValue("@idHospital", model.IdHospital);
					comandoInsert.Parameters.AddWithValue("@idInstitucionTraslado", model.IdInstitucionTraslado);
					comandoInsert.Parameters.AddWithValue("@idAsiento", model.IdAsiento ?? 0);
					comandoInsert.Parameters.AddWithValue("@idCinturon", model.IdCinturon);
					comandoInsert.Parameters.AddWithValue("@estatus", 1);
					comandoInsert.Parameters.AddWithValue("@IdInvolucrado", model.IdInvolucrado ?? 0);

					connection.Open();
					var result2 = comandoInsert.ExecuteReader();
					while (result2.Read())
					{
						var error = (string)result2["error"];
						var operacion = (string)result2["result"];
						result = (int)result2["intresult"];
					}
					result2.Close();

                    if (model.IdAseguradora != null && model.idPersona != 0)
                    {
                        InsertaDatosAseguradora(model, idAccidente);
                    }

                }
				catch (Exception ex)
				{
					// Manejar excepciones
				}
			}
			return result;
		}

		public bool InsertaDatosAseguradora(CapturaAccidentesModel model, int idAccidente)
		{
			try
			{
                AccidentesDatosAseguradora datosAseguradora = dbContext.AccidentesDatosAseguradoras
                    .FirstOrDefault(x => x.IdAccidente == idAccidente && x.IdPersona == model.IdPersona)
                    ?? new AccidentesDatosAseguradora { IdAccidente = idAccidente, IdPersona = model.IdPersona };

                datosAseguradora.IdAseguradora = model.IdAseguradora;
                datosAseguradora.Poliza = model.Poliza?.ToUpper();
                datosAseguradora.FechaExpiracion = model.FechaExpiracionPoliza;
				datosAseguradora.fechaActualizacion = DateTime.Now;

                if (datosAseguradora.IdDatosAseguradora == 0)
                {
                    dbContext.AccidentesDatosAseguradoras.Add(datosAseguradora);
                }

                dbContext.SaveChanges();

                return true;
            }
			catch (Exception ex)
			{
                return false;
            }
		}
		public int ActualizarRelacionVehiculoPersona(CapturaAccidentesModel model, int idAccidente)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					string consultaExistencia = @"ActualizarRelacionVehiculoPersonaAccidente";

					// Ejecutar la primera consulta para insertar el involucrado
					SqlCommand comandoInsert = new SqlCommand(consultaExistencia, connection);
					comandoInsert.CommandType = CommandType.StoredProcedure;

					// Agregar parámetros para la stored procedure
					comandoInsert.Parameters.AddWithValue("@idAccidente", idAccidente);
					comandoInsert.Parameters.AddWithValue("@idPersona", model.IdPersona);
					comandoInsert.Parameters.AddWithValue("@idVehiculo", model.IdVehiculo);
					comandoInsert.Parameters.AddWithValue("@idVehiculoNuevo", model.IdVehiculoNuevo == null ? model.IdVehiculo : model.IdVehiculoNuevo);

					comandoInsert.Parameters.AddWithValue("@idTipoInvolucrado", model.IdTipoInvolucrado);
					comandoInsert.Parameters.AddWithValue("@idEstadoVictima", model.IdEstadoVictima);
					comandoInsert.Parameters.AddWithValue("@idHospital", model.IdHospital);
					comandoInsert.Parameters.AddWithValue("@idInstitucionTraslado", model.IdInstitucionTraslado);
					comandoInsert.Parameters.AddWithValue("@idAsiento", model.IdAsiento ?? 0);
					comandoInsert.Parameters.AddWithValue("@idCinturon", model.IdCinturon);

					comandoInsert.Parameters.AddWithValue("@estatus", 1);
					//comandoInsert.Parameters.AddWithValue("@IdInvolucrado", model.IdInvolucrado ?? 0);
					if (model.IdAseguradora != null && model.idPersona != 0)
					{
						InsertaDatosAseguradora(model, idAccidente);
					}
					connection.Open();
					var result2 = comandoInsert.ExecuteNonQuery();


				}
				catch (Exception ex)
				{
					// Manejar excepciones
				}
			}
			return result;
		}


		public List<CapturaAccidentesModel> InvolucradosAccidente(int idAccidente)
		{

			List<CapturaAccidentesModel> ListaInvolucrados = new List<CapturaAccidentesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try
				{
					int numeroConsecutivo = 1;
					connection.Open();

					SqlCommand command = new SqlCommand("usp_ObtienePersonasInvolucradosAccidente", connection);
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.AddWithValue("@idAccidente", idAccidente);

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CapturaAccidentesModel involucrado = new CapturaAccidentesModel();
							involucrado.IdInvolucrado = reader["idInvolucrado"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInvolucrado"].ToString());
							var testt = reader["Numv"];

							//involucrado.OrdenVehiculo = reader["Numv"].ToString();
							involucrado.OrdenVehiculo = reader["Numv"] == DBNull.Value ? string.Empty : reader["Numv"].ToString();

							involucrado.IdAccidente = reader["idAccidente"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idAccidente"].ToString());
							involucrado.IdTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"].ToString());
							involucrado.IdTipoVehiculo = reader["IdTipoVehiculo"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["IdTipoVehiculo"].ToString());
							involucrado.IdPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
							involucrado.IdVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idVehiculo"].ToString());
							involucrado.IdEstadoVictima = reader["idEstadoVictima"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idEstadoVictima"].ToString());
							involucrado.IdInstitucionTraslado = reader["idInstitucionTraslado"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInstitucionTraslado"].ToString());
							involucrado.IdHospital = reader["idHospital"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idHospital"].ToString());
							involucrado.IdAsiento = Convert.ToInt32(reader["idAsiento"].ToString());
							if (involucrado.IdAsiento == 0)
							{
								involucrado.IdAsiento = null;
							}
							involucrado.IdCinturon = reader["idCinturon"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCinturon"].ToString());
							involucrado.nombre = reader["nombre"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["nombre"].ToString());
							involucrado.apellidoPaterno = reader["apellidoPaterno"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["apellidoPaterno"].ToString());
							involucrado.apellidoMaterno = reader["apellidoMaterno"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["apellidoMaterno"].ToString());
							involucrado.RFC = reader["rfc"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["rfc"].ToString());
							involucrado.CURP = reader["curp"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["curp"].ToString());
							involucrado.TipoLicencia = reader["tipoLicencia"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["tipoLicencia"].ToString());
							involucrado.TipoVehiculo = reader["tipoVehiculo"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["tipoVehiculo"].ToString());
							involucrado.EstadoVictima = reader["estadoVictima"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["estadoVictima"].ToString());
							involucrado.NombreHospital = reader["nombreHospital"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["nombreHospital"].ToString());
							involucrado.Asiento = reader["idAsiento"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["idAsiento"].ToString());
							involucrado.InstitucionTraslado = reader["institucionTraslado"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["institucionTraslado"].ToString());
							involucrado.Placa = reader["placas"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["placas"].ToString());
							involucrado.Sexo = reader["genero"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["genero"].ToString());
							involucrado.Marca = reader["marcaVehiculo"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["marcaVehiculo"].ToString());
							involucrado.Submarca = reader["nombreSubmarca"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["nombreSubmarca"].ToString());
							involucrado.Direccion = reader["Direccion"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["Direccion"].ToString());
							involucrado.Municipio = reader["municipio"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["municipio"].ToString());
							involucrado.Telefono = reader["telefono"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["telefono"].ToString());
							involucrado.Correo = reader["correo"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["correo"].ToString());
							involucrado.Entidad = reader["nombreEntidad"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["nombreEntidad"].ToString());
							involucrado.Modelo = reader["modelo"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["modelo"].ToString());
							involucrado.ConductorInvolucrado = reader["tipoInvolucrado"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["tipoInvolucrado"].ToString());
							involucrado.Cinturon = reader["cinturon"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["cinturon"].ToString());
							involucrado.NumeroEconomico = reader["cinturon"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["cinturon"].ToString());
							involucrado.NoAccidente = reader["NoAccidente"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["NoAccidente"].ToString());
							involucrado.IdTipoInvolucrado = reader["idTipoInvolucrado"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoInvolucrado"].ToString());
							involucrado.TipoInvolucrado = reader["tipoInvolucrado"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["tipoInvolucrado"].ToString());
							involucrado.numeroLicencia = reader["numeroLicencia"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["numeroLicencia"].ToString());
							involucrado.FechaNacimientoTexto = reader["fechaNacimiento"] == System.DBNull.Value ? "" : Convert.ToString(reader["fechaNacimiento"].ToString());
							var Fecha = reader["fechaNacimiento"] == System.DBNull.Value ? "" : Convert.ToString(reader["fechaNacimiento"].ToString());
							involucrado.IdTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"].ToString());
							involucrado.IdTipoVehiculo = reader["IdTipoVehiculo"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["IdTipoVehiculo"].ToString());
							involucrado.IdPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
							involucrado.IdVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idVehiculo"].ToString());
							involucrado.IdEstadoVictima = reader["idEstadoVictima"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idEstadoVictima"].ToString());
							involucrado.IdInstitucionTraslado = reader["idInstitucionTraslado"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInstitucionTraslado"].ToString());
							involucrado.IdHospital = reader["idHospital"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idHospital"].ToString());
							involucrado.IdAsiento = Convert.ToInt32(reader["idAsiento"].ToString());
							if (involucrado.IdAsiento == 0)
							{
								involucrado.IdAsiento = null;
							}
							involucrado.IdCinturon = reader["idCinturon"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCinturon"].ToString());
							involucrado.nombre = reader["nombre"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["nombre"].ToString());
							involucrado.apellidoPaterno = reader["apellidoPaterno"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["apellidoPaterno"].ToString());
							involucrado.apellidoMaterno = reader["apellidoMaterno"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["apellidoMaterno"].ToString());
							involucrado.RFC = reader["rfc"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["rfc"].ToString());
							involucrado.CURP = reader["curp"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["curp"].ToString());
							involucrado.TipoLicencia = reader["tipoLicencia"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["tipoLicencia"].ToString());
							involucrado.TipoVehiculo = reader["tipoVehiculo"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["tipoVehiculo"].ToString());
							involucrado.EstadoVictima = reader["estadoVictima"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["estadoVictima"].ToString());
							involucrado.NombreHospital = reader["nombreHospital"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["nombreHospital"].ToString());
							involucrado.Asiento = reader["idAsiento"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["idAsiento"].ToString());
							involucrado.InstitucionTraslado = reader["institucionTraslado"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["institucionTraslado"].ToString());
							involucrado.Placa = reader["placas"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["placas"].ToString());
							involucrado.Sexo = reader["genero"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["genero"].ToString());
							involucrado.Marca = reader["marcaVehiculo"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["marcaVehiculo"].ToString());
							involucrado.Submarca = reader["nombreSubmarca"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["nombreSubmarca"].ToString());
							involucrado.Direccion = reader["Direccion"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["Direccion"].ToString());
							involucrado.Municipio = reader["municipio"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["municipio"].ToString());
							involucrado.Telefono = reader["telefono"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["telefono"].ToString());
							involucrado.Correo = reader["correo"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["correo"].ToString());
							involucrado.Entidad = reader["nombreEntidad"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["nombreEntidad"].ToString());
							involucrado.Modelo = reader["modelo"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["modelo"].ToString());
							involucrado.ConductorInvolucrado = reader["tipoInvolucrado"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["tipoInvolucrado"].ToString());
							involucrado.Cinturon = reader["cinturon"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["cinturon"].ToString());
							involucrado.NumeroEconomico = reader["cinturon"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["cinturon"].ToString());
							involucrado.NoAccidente = reader["NoAccidente"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["NoAccidente"].ToString());
							involucrado.IdTipoInvolucrado = reader["idTipoInvolucrado"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoInvolucrado"].ToString());
							involucrado.TipoInvolucrado = reader["tipoInvolucrado"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["tipoInvolucrado"].ToString());
							involucrado.numeroLicencia = reader["numeroLicencia"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["numeroLicencia"].ToString());
                             involucrado.nombreArchivo = reader["nombreArchivoInvolucrado"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["nombreArchivoInvolucrado"].ToString());
                            involucrado.rutaArchivo = reader["rutaArchivoInvolucrado"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["rutaArchivoInvolucrado"].ToString());


                            involucrado.FechaNacimientoTexto = reader["fechaNacimiento"] == System.DBNull.Value ? "" : Convert.ToString(reader["fechaNacimiento"].ToString());

							var auxdate = DateTime.MinValue;

							if (!string.IsNullOrEmpty(Fecha))
							{
								auxdate = DateTime.ParseExact(Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
							}



							var TicksDate = DateTime.Now.Ticks - (auxdate.Ticks == 0 ? DateTime.Now.Ticks : auxdate.Ticks);
							var age = auxdate.Ticks == 0 ? "" : ((new DateTime(TicksDate)).Year - 1).ToString();

							involucrado.FormatDateNacimiento = age;


							if (reader["fechaIngreso"] != System.DBNull.Value)
							{
								involucrado.FechaIngreso = Convert.ToDateTime(reader["fechaIngreso"].ToString());
							}
							else
							{
								involucrado.FechaIngreso = default(DateTime);
							}
							if (reader["horaIngreso"] != System.DBNull.Value)
							{
								involucrado.HoraIngreso = reader.GetTimeSpan(reader.GetOrdinal("horaIngreso"));
							}
							else
							{
								involucrado.HoraIngreso = default(TimeSpan);
							}

							var datosAseguradora = dbContext.AccidentesDatosAseguradoras
															.Include(x => x.IdAseguradoraNavigation)
															.FirstOrDefault(x => x.IdAccidente == idAccidente && x.IdPersona == involucrado.IdPersona);
							if(datosAseguradora != null)
							{
								involucrado.Poliza = datosAseguradora.Poliza;
								involucrado.FechaExpiracionPoliza = datosAseguradora.FechaExpiracion;
								involucrado.FechaExpiracionPolizaTexto = datosAseguradora?.FechaExpiracion?.ToString("dd/MM/yyyy");
								involucrado.IdAseguradora = datosAseguradora.IdAseguradora;
								involucrado.NombreAseguradora = datosAseguradora.IdAseguradoraNavigation.NombreAseguradora;
							}


                            involucrado.numeroConsecutivo = numeroConsecutivo;

							ListaInvolucrados.Add(involucrado);
							numeroConsecutivo++;
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
			return ListaInvolucrados;


		}
		public int EditarInvolucrado(CapturaAccidentesModel model)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "UPDATE AccidenteFactoresOpciones SET idFactor = @IdFactor, idFactorOpcion = @IdFactorOpcion  " +
									"WHERE idAccidenteFactorOpcion = @IdAccidenteFactorOpcion";

					SqlCommand command = new SqlCommand(query, connection);

					//  command.Parameters.AddWithValue("@idFactor", IdFactorAccidente);

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
		public int EliminarInvolucrado(int IdInvolucrado)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = @"   declare @idAccidente int , @idpersona int
                                        UPDATE involucradosAccidente SET estatus = 0 WHERE idInvolucradosAccidente = @IdInvolucrado;
                                        select @idAccidente=idaccidente,@idpersona= idpersona from involucradosAccidente where idInvolucradosAccidente = @IdInvolucrado;
                                        UPDATE opeConductoresVehiculosAccidentePersonas SET estatus = 0 where idpersona=@idpersona and idaccidente = @idAccidente and (tipoOperacion = 3 or tipoOperacion is null);";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@IdInvolucrado", IdInvolucrado);

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
		public int AgregarFechaHoraIngreso(FechaHoraIngresoModel model, int idAccidente)

		{
			int result = 0;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand sqlCommand = new
						SqlCommand("UPDATE involucradosAccidente SET fechaIngreso = @fechaIngreso, horaIngreso = @horaIngreso, fechaActualizacion = @fechaActualizacion WHERE idAccidente = @idAccidente AND idPersona = @idPersona;",
						connection);
					sqlCommand.Parameters.Add(new SqlParameter("@fechaIngreso", SqlDbType.DateTime)).Value = (object)model.FechaIngreso ?? DBNull.Value;
					sqlCommand.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.Int)).Value = idAccidente;
					sqlCommand.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = model.IdPersona;
					sqlCommand.Parameters.Add(new SqlParameter("@horaIngreso", SqlDbType.Time)).Value = (object)model.HoraIngreso ?? DBNull.Value;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;

                    sqlCommand.CommandType = CommandType.Text;
					result = sqlCommand.ExecuteNonQuery();
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

		public int AgregarDatosFinales(DatosAccidenteModel datosAccidente, int armasValue, int drogasValue, int valoresValue, int prendasValue, int otrosValue, int idAccidente, int convenioValue)
		{
			int result = 0;
			string qryUpdate = "";
			//  qryUpdate += !datosAccidente.montoCamino.Equals(null) ? " , montoCamino = @MontoCamino " : "";
			//qryUpdate += !datosAccidente.montoCarga.Equals(null) ? " , montoCarga = @MontoCarga " : "";
			// qryUpdate += !datosAccidente.montoPropietarios.Equals(null) ? " , montoPropietarios = @MontoPropietarios " : "";
			//qryUpdate += !datosAccidente.montoOtros.Equals(null) ? " , montoOtros = @MontoOtros " : "";
			qryUpdate += !string.IsNullOrEmpty(datosAccidente.montoCamino) ? " , montoCamino = @MontoCamino " : "";
			qryUpdate += !string.IsNullOrEmpty(datosAccidente.montoCarga) ? " , montoCarga = @MontoCarga " : "";
			qryUpdate += !string.IsNullOrEmpty(datosAccidente.montoPropietarios) ? " , montoPropietarios = @MontoPropietarios " : "";
			qryUpdate += !string.IsNullOrEmpty(datosAccidente.montoOtros) ? " , montoOtros = @MontoOtros " : "";
           
			qryUpdate += (datosAccidente.Latitud == null || datosAccidente.Latitud == "0") ?
                          " , latitud = '' " :
                          " , latitud = @Latitud ";
            
			qryUpdate += (datosAccidente.Longitud == null || datosAccidente.Longitud == "0") ?
              " , longitud = '' " :
              " , longitud = @Longitud ";

			qryUpdate += !datosAccidente.IdCertificado.Equals(null) ? " , idCertificado = @IdCertificado " : "";
			qryUpdate += !convenioValue.Equals(null) ? " , convenio = @convenioValue " : "";
			qryUpdate += !armasValue.Equals(null) ? " , armas = @armasValue " : "";
			qryUpdate += !drogasValue.Equals(null) ? " , drogas = @drogasValue " : "";
			qryUpdate += !valoresValue.Equals(null) ? " , valores = @valoresValue " : "";
			qryUpdate += !prendasValue.Equals(null) ? " , prendas = @prendasValue " : "";
			qryUpdate += !otrosValue.Equals(null) ? " , otros = @otrosValue " : "";
			qryUpdate += !string.IsNullOrEmpty(datosAccidente.entregaObjetos) ? " , entregaObjetos = @entregaObjetos " : "";
			qryUpdate += !string.IsNullOrEmpty(datosAccidente.entregaOtros) ? " , entregaOtros = @entregaOtros " : "";
			qryUpdate += !string.IsNullOrEmpty(datosAccidente.consignacionHechos) ? " , consignacionHechos = @consignacionHechos " : "";
			qryUpdate += !datosAccidente.IdCiudad.Equals(null) && datosAccidente.IdCiudad > 0 ? " , idCiudad = @idCiudad " : "";
			qryUpdate += !datosAccidente.IdAutoridadEntrega.Equals(null) ? " , idAutoridadEntrega = @IdAutoridadEntrega " : "";
			qryUpdate += !datosAccidente.IdAutoridadDisposicion.Equals(null) ? " , idAutoridadDisposicion = @IdAutoridadDisposicion " : "";
			qryUpdate += !datosAccidente.IdElaboraConsignacion.Equals(null) ? " , idElaboraConsignacion = @IdElaboraConsignacion " : "";
			qryUpdate += !string.IsNullOrEmpty(datosAccidente.numeroOficio) ? " , numeroOficio = @numeroOficio " : "";
			qryUpdate += !datosAccidente.IdAgenciaMinisterio.Equals(null) ? " , idAgenciaMinisterio = @IdAgenciaMinisterio " : "";
			qryUpdate += !string.IsNullOrEmpty(datosAccidente.RecibeMinisterio) ? " , recibeMinisterio = @RecibeMinisterio " : "";
			qryUpdate += !datosAccidente.IdElabora.Equals(null) ? " , idElabora = @IdElabora " : "";
			qryUpdate += !datosAccidente.IdAutoriza.Equals(null) ? " , idAutoriza = @IdAutoriza " : "";
			qryUpdate += !datosAccidente.IdSupervisa.Equals(null) ? " , idSupervisa = @IdSupervisa " : "";
			qryUpdate += !string.IsNullOrEmpty(datosAccidente.ArmasTexto) ? " , armasTexto = @armasTexto " : "";
			qryUpdate += !string.IsNullOrEmpty(datosAccidente.DrogasTexto) ? " , drogasTexto = @drogasTexto " : "";
			qryUpdate += !string.IsNullOrEmpty(datosAccidente.ValoresTexto) ? " , valoresTexto = @valoresTexto " : "";
			qryUpdate += !string.IsNullOrEmpty(datosAccidente.PrendasTexto) ? " , prendasTexto = @prendasTexto " : "";
			qryUpdate += !string.IsNullOrEmpty(datosAccidente.OtrosTexto) ? " , otrosTexto = @otrosTexto " : "";

			qryUpdate += !string.IsNullOrEmpty(datosAccidente.observacionesConvenio) ? " , observacionesConvenio = @observacionesConvenio " : "";

			qryUpdate += !datosAccidente.IdEntidadCompetencia.Equals(null) ? " , idEntidadCompetencia = @IdEntidadCompetencia " : "";
			//Cmpos fase 2///
			qryUpdate += !string.IsNullOrEmpty(datosAccidente.trayectoria) ? " , trayectoria = @trayectoria " : "";


			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = @"UPDATE accidentes SET idEstatusReporte = @idEstatusReporte, fechaActualizacion = @fechaActualizacion " + qryUpdate + " WHERE idAccidente = @IdAccidente";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@idAccidente", idAccidente);
					command.Parameters.AddWithValue("@MontoCamino", datosAccidente.montoCamino ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@MontoCarga", datosAccidente.montoCarga ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@MontoPropietarios", datosAccidente.montoPropietarios ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@MontoOtros", datosAccidente.montoOtros ?? (object)DBNull.Value);

					//command.Parameters.AddWithValue("@MontoCarga", datosAccidente.montoCarga);
					//command.Parameters.AddWithValue("@MontoPropietarios", datosAccidente.montoPropietarios);
					//command.Parameters.AddWithValue("@MontoOtros", datosAccidente.montoOtros);
					command.Parameters.AddWithValue("@Latitud", datosAccidente.Latitud);
					command.Parameters.AddWithValue("@Longitud", datosAccidente.Longitud);
					command.Parameters.AddWithValue("@IdCertificado", datosAccidente.IdCertificado);
					command.Parameters.Add(new SqlParameter("@convenioValue", SqlDbType.Bit)).Value = (object)convenioValue ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@armasValue", SqlDbType.Bit)).Value = (object)armasValue ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@drogasValue", SqlDbType.Bit)).Value = (object)drogasValue ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@valoresValue", SqlDbType.Bit)).Value = (object)valoresValue ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@prendasValue", SqlDbType.Bit)).Value = (object)prendasValue ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@otrosValue", SqlDbType.Bit)).Value = (object)otrosValue ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;

                    if (!string.IsNullOrEmpty(datosAccidente.ArmasTexto))
						command.Parameters.Add(new SqlParameter("@armasTexto", SqlDbType.NVarChar)).Value = (object)datosAccidente.ArmasTexto?.ToUpper() ?? DBNull.Value;

					if (!string.IsNullOrEmpty(datosAccidente.DrogasTexto))
						command.Parameters.Add(new SqlParameter("@drogasTexto", SqlDbType.NVarChar)).Value = (object)datosAccidente.DrogasTexto?.ToUpper() ?? DBNull.Value;

					if (!string.IsNullOrEmpty(datosAccidente.PrendasTexto))
						command.Parameters.Add(new SqlParameter("@prendasTexto", SqlDbType.NVarChar)).Value = (object)datosAccidente.PrendasTexto?.ToUpper() ?? DBNull.Value;

					if (!string.IsNullOrEmpty(datosAccidente.ValoresTexto))
						command.Parameters.Add(new SqlParameter("@valoresTexto", SqlDbType.NVarChar)).Value = (object)datosAccidente.ValoresTexto?.ToUpper() ?? DBNull.Value;

					if (!string.IsNullOrEmpty(datosAccidente.OtrosTexto))
						command.Parameters.Add(new SqlParameter("@otrosTexto", SqlDbType.NVarChar)).Value = (object)datosAccidente.OtrosTexto?.ToUpper() ?? DBNull.Value;

					if (!string.IsNullOrEmpty(datosAccidente.observacionesConvenio))
						command.Parameters.Add(new SqlParameter("@observacionesConvenio", SqlDbType.NVarChar)).Value = (object)datosAccidente.observacionesConvenio?.ToUpper() ?? DBNull.Value;


					if (!string.IsNullOrEmpty(datosAccidente.entregaObjetos))
						command.Parameters.Add(new SqlParameter("@entregaObjetos", SqlDbType.NVarChar)).Value = (object)datosAccidente.entregaObjetos?.ToUpper() ?? DBNull.Value;

					if (!string.IsNullOrEmpty(datosAccidente.entregaObjetos))
						command.Parameters.Add(new SqlParameter("@entregaOtros", SqlDbType.NVarChar)).Value = (object)datosAccidente.entregaOtros?.ToUpper() ?? DBNull.Value;

					if (!string.IsNullOrEmpty(datosAccidente.consignacionHechos))
						command.Parameters.Add(new SqlParameter("@consignacionHechos", SqlDbType.NVarChar)).Value = (object)datosAccidente.consignacionHechos?.ToUpper() ?? DBNull.Value;

					if (datosAccidente.IdCiudad > 0)
						command.Parameters.AddWithValue("@idCiudad", datosAccidente.IdCiudad);

					command.Parameters.AddWithValue("@IdAutoridadEntrega", datosAccidente.IdAutoridadEntrega);
					command.Parameters.AddWithValue("@IdAutoridadDisposicion", datosAccidente.IdAutoridadDisposicion);
					command.Parameters.AddWithValue("@IdElaboraConsignacion", datosAccidente.IdElaboraConsignacion);

					if (!string.IsNullOrEmpty(datosAccidente.numeroOficio))
						command.Parameters.Add(new SqlParameter("@numeroOficio", SqlDbType.NVarChar)).Value = (object)datosAccidente.numeroOficio?.ToUpper() ?? DBNull.Value;

					command.Parameters.AddWithValue("@IdAgenciaMinisterio", datosAccidente.IdAgenciaMinisterio);

					if (!string.IsNullOrEmpty(datosAccidente.RecibeMinisterio))
						command.Parameters.Add(new SqlParameter("@RecibeMinisterio", SqlDbType.NVarChar)).Value = (object)datosAccidente.RecibeMinisterio?.ToUpper() ?? DBNull.Value;
					command.Parameters.AddWithValue("@IdElabora", datosAccidente.IdElabora);
					command.Parameters.AddWithValue("@IdAutoriza", datosAccidente.IdAutoriza);
					command.Parameters.AddWithValue("@IdSupervisa", datosAccidente.IdSupervisa);
					command.Parameters.AddWithValue("@IdEntidadCompetencia", datosAccidente.IdEntidadCompetencia);
					command.Parameters.AddWithValue("@idEstatusReporte", 3);
					//fase 2
					if (!string.IsNullOrEmpty(datosAccidente.trayectoria))
						command.Parameters.Add(new SqlParameter("@trayectoria", SqlDbType.NVarChar)).Value = (object)datosAccidente.trayectoria?.ToUpper() ?? DBNull.Value;

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

				return result;
			}
		}


		public int GuardarDatosPrevioInfraccion(int idAccidente, string montoCamino, string montoCarga, string montoPropietarios, string montoOtros)
		{
			int result = 0;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = @"UPDATE accidentes 
                                            SET montoCamino = @MontoCamino, 
                                                montoCarga = @MontoCarga, 
                                                montoPropietarios = @MontoPropietarios, 
                                                montoOtros = @MontoOtros,
												fechaActualizacion = @fechaActualizacion
                                            WHERE idAccidente = @idAccidente
                                            ";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@idAccidente", idAccidente);
					command.Parameters.AddWithValue("@MontoCamino", montoCamino ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@MontoCarga", montoCarga ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@MontoPropietarios", montoPropietarios ?? (object)DBNull.Value);
					command.Parameters.AddWithValue("@MontoOtros", montoOtros ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

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

				return result;
			}
		}



		public int AnexarBoleta(DatosAccidenteModel datosAccidente, int idAccidente)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();

					// Primero, verifica si ya existe un registro con el mismo idAccidente y estatus 1
					string checkQuery = @"
                SELECT COUNT(*) 
                FROM boletasAccidentes 
                WHERE idAccidente = @idAccidente AND estatus = 1";

					SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
					checkCommand.Parameters.AddWithValue("@idAccidente", idAccidente);

					int count = (int)checkCommand.ExecuteScalar();

					if (count > 0)
					{
						// Si existe, realiza un UPDATE
						string updateQuery = @"
                    UPDATE boletasAccidentes
                    SET rutaBoleta = @rutaBoleta,
                        nombreBoleta = @nombreBoleta,
                        actualizadoPor = @actualizadoPor,
                        fechaActualizacion = @fechaActualizacion
                    WHERE idAccidente = @idAccidente AND estatus = 1";

						SqlCommand updateCommand = new SqlCommand(updateQuery, connection);

						updateCommand.Parameters.AddWithValue("@idAccidente", idAccidente);
						updateCommand.Parameters.AddWithValue("@rutaBoleta", datosAccidente.boletaPath ?? (object)DBNull.Value);
						updateCommand.Parameters.AddWithValue("@nombreBoleta", datosAccidente.boletaStr?.ToUpper() ?? (object)DBNull.Value);
						updateCommand.Parameters.AddWithValue("@actualizadoPor", 1);
						updateCommand.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

						result = updateCommand.ExecuteNonQuery();
					}
					else
					{
						// Si no existe, realiza un INSERT
						string insertQuery = @"
                    INSERT INTO boletasAccidentes 
                        (rutaBoleta, nombreBoleta, idAccidente, actualizadoPor, estatus, fechaActualizacion)
                    VALUES
                        (@rutaBoleta, @nombreBoleta, @idAccidente, @actualizadoPor, @estatus, @fechaActualizacion)";

						SqlCommand insertCommand = new SqlCommand(insertQuery, connection);

						insertCommand.Parameters.AddWithValue("@idAccidente", idAccidente);
						insertCommand.Parameters.AddWithValue("@rutaBoleta", datosAccidente.boletaPath ?? (object)DBNull.Value);
						insertCommand.Parameters.AddWithValue("@nombreBoleta", datosAccidente.boletaStr?.ToUpper() ?? (object)DBNull.Value);
						insertCommand.Parameters.AddWithValue("@actualizadoPor", 1);
						insertCommand.Parameters.AddWithValue("@estatus", 1);
						insertCommand.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

						result = insertCommand.ExecuteNonQuery();
					}
				}
				catch (SqlException ex)
				{
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

		public CatalogModel GetBoletaPath(int idAccidente)
		{

			var result = new CatalogModel();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "select rutaBoleta as result,nombreBoleta as filename from  boletasAccidentes  where idAccidente = @idAccidente AND estatus = 1";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@idAccidente", idAccidente);
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

		public CatalogModel GetArchivoPartePath(int idAccidente)
		{

			var result = new CatalogModel();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "select rutaArchivoParte as result,nombreArchivoParte as filename from  archivosParteAccidente  where idAccidente = @idAccidente AND estatus = 1";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@idAccidente", idAccidente);
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


		public int AnexarArchivoParte(DatosAccidenteModel datosAccidente, int idAccidente)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();

					// Primero, verifica si ya existe un registro con el mismo idAccidente y estatus 1
					string checkQuery = @"
                SELECT COUNT(*)
                FROM archivosParteAccidente
                WHERE idAccidente = @idAccidente AND estatus = 1";

					SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
					checkCommand.Parameters.AddWithValue("@idAccidente", idAccidente);

					int count = (int)checkCommand.ExecuteScalar();

					if (count > 0)
					{
						// Si existe, realiza un UPDATE
						string updateQuery = @"
                    UPDATE archivosParteAccidente
                    SET rutaArchivoParte = @rutaArchivoParte,
                        nombreArchivoParte = @nombreArchivoParte,
                        actualizadoPor = @actualizadoPor,
                        fechaActualizacion = @fechaActualizacion
                    WHERE idAccidente = @idAccidente AND estatus = 1";

						SqlCommand updateCommand = new SqlCommand(updateQuery, connection);

						updateCommand.Parameters.AddWithValue("@idAccidente", idAccidente);
						updateCommand.Parameters.AddWithValue("@rutaArchivoParte", datosAccidente.archivoPartePath ?? (object)DBNull.Value);
						updateCommand.Parameters.AddWithValue("@nombreArchivoParte", datosAccidente.archivoParteStr?.ToUpper() ?? (object)DBNull.Value);
						updateCommand.Parameters.AddWithValue("@actualizadoPor", 1);
						updateCommand.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

						result = updateCommand.ExecuteNonQuery();
					}
					else
					{
						// Si no existe, realiza un INSERT
						string insertQuery = @"
                    INSERT INTO archivosParteAccidente 
                        (rutaArchivoParte, nombreArchivoParte, idAccidente, actualizadoPor, fechaActualizacion, estatus)
                    VALUES
                        (@rutaArchivoParte, @nombreArchivoParte, @idAccidente, @actualizadoPor, @fechaActualizacion, @estatus)";

						SqlCommand insertCommand = new SqlCommand(insertQuery, connection);

						insertCommand.Parameters.AddWithValue("@idAccidente", idAccidente);
						insertCommand.Parameters.AddWithValue("@rutaArchivoParte", datosAccidente.archivoPartePath ?? (object)DBNull.Value);
						insertCommand.Parameters.AddWithValue("@nombreArchivoParte", datosAccidente.archivoParteStr?.ToUpper() ?? (object)DBNull.Value);
						insertCommand.Parameters.AddWithValue("@actualizadoPor", 1);
						insertCommand.Parameters.AddWithValue("@estatus", 1);
						insertCommand.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

						result = insertCommand.ExecuteNonQuery();
					}
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

		public int AnexarArchivoInvolucrado(DatosAccidenteModel datosAccidente, int idAccidente)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();

					// Primero, verifica si ya existe un registro con el mismo idAccidente y estatus 1
					string checkQuery = @"
                SELECT COUNT(*)
                FROM archivosInvolucradosAccidente
                WHERE idAccidente = @idAccidente AND idInvolucrado = @idInvolucrado AND estatus = 1";

					SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
					checkCommand.Parameters.AddWithValue("@idAccidente", idAccidente);
					checkCommand.Parameters.AddWithValue("@idInvolucrado", datosAccidente.idInvolucrado);

					int count = (int)checkCommand.ExecuteScalar();

					if (count > 0)
					{
						// Si existe, realiza un UPDATE
						string updateQuery = @"
                    UPDATE archivosInvolucradosAccidente
                    SET rutaArchivoInvolucrado = @rutaArchivoInvolucrado,
                        nombreArchivoInvolucrado = @nombreArchivoInvolucrado,
                        actualizadoPor = @actualizadoPor,
                        fechaActualizacion = @fechaActualizacion
                    WHERE idAccidente = @idAccidente AND idInvolucrado = @idInvolucrado AND estatus = 1";

						SqlCommand updateCommand = new SqlCommand(updateQuery, connection);

						updateCommand.Parameters.AddWithValue("@idAccidente", idAccidente);
						updateCommand.Parameters.AddWithValue("@idInvolucrado", datosAccidente.idInvolucrado);
						updateCommand.Parameters.AddWithValue("@rutaArchivoInvolucrado", datosAccidente.archivoInvolucradoPath ?? (object)DBNull.Value);
						updateCommand.Parameters.AddWithValue("@nombreArchivoInvolucrado", datosAccidente.archivoInvolucradoStr?.ToUpper() ?? (object)DBNull.Value);
						updateCommand.Parameters.AddWithValue("@actualizadoPor", 1);
						updateCommand.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

						result = updateCommand.ExecuteNonQuery();
					}
					else
					{
						// Si no existe, realiza un INSERT
						string insertQuery = @"
                    INSERT INTO archivosInvolucradosAccidente 
                        (rutaArchivoInvolucrado, nombreArchivoInvolucrado, idAccidente,idInvolucrado, actualizadoPor, fechaActualizacion, estatus)
                    VALUES
                        (@rutaArchivoInvolucrado, @nombreArchivoInvolucrado, @idAccidente,@idInvolucrado, @actualizadoPor, @fechaActualizacion, @estatus)";

						SqlCommand insertCommand = new SqlCommand(insertQuery, connection);

						insertCommand.Parameters.AddWithValue("@idAccidente", idAccidente);
						insertCommand.Parameters.AddWithValue("@idInvolucrado", datosAccidente.idInvolucrado);
						insertCommand.Parameters.AddWithValue("@rutaArchivoInvolucrado", datosAccidente.archivoInvolucradoPath ?? (object)DBNull.Value);
						insertCommand.Parameters.AddWithValue("@nombreArchivoInvolucrado", datosAccidente.archivoInvolucradoStr?.ToUpper() ?? (object)DBNull.Value);
						insertCommand.Parameters.AddWithValue("@actualizadoPor", 1);
						insertCommand.Parameters.AddWithValue("@estatus", 1);
						insertCommand.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

						result = insertCommand.ExecuteNonQuery();
					}
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

        public int AnexarArchivoCroquis(DatosAccidenteModel datosAccidente, int idAccidente)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
					connection.Open();

					string updateQuery = @"
                    UPDATE ACCIDENTES
                    SET urlCroquis = @urlCroquis,
						fechaActualizacion = @fechaActualizacion
                    WHERE idAccidente = @idAccidente";

                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);

                    string urlCroquis = datosAccidente.archivoInvolucradoPath;

					updateCommand.Parameters.AddWithValue("@urlCroquis", urlCroquis ?? (object)DBNull.Value);
					updateCommand.Parameters.AddWithValue("@idAccidente", idAccidente);
                    updateCommand.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

                    result = updateCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return result;
                }
				catch (Exception ex)
				{
					Console.WriteLine($"General Error: {ex.Message}");
					return -1; // Indicador de error
				}
			}

            return result;
        }

        public CatalogModel GetArchivoInvolucradoPath(int IdAccidente, int IdInvolucrado)
		{

			var result = new CatalogModel();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "select rutaArchivoInvolucrado as result,nombreArchivoInvolucrado as filename from  archivosInvolucradosAccidente  where idAccidente = @idAccidente AND idInvolucrado = @idInvolucrado AND estatus = 1";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@idAccidente", IdAccidente);
					command.Parameters.AddWithValue("@idInvolucrado", IdInvolucrado);

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
		public bool ValidarFolio(string folioInfraccion, int idDependencia)
		{
			int folio = 0;


			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				connection.Open();

				string query = "SELECT COUNT(*) AS Result FROM infracciones WHERE folioInfraccion = @folioInfraccion and  year(fechaInfraccion) = year(getdate())";

				using (SqlCommand command = new SqlCommand(query, connection))
				{

					command.Parameters.AddWithValue("@folioInfraccion", folioInfraccion);
					//command.Parameters.AddWithValue("@idDependencia", idDependencia);

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
		public int RegistrarInfraccion(NuevaInfraccionModel model, int idDependencia)
		{
			int result = 0;
			/* string strQuery = @"INSERT INTO infracciones
											 (fechaInfraccion
											 ,horaInfraccion    
											 ,folioInfraccion
											 ,idOficial
											 ,idMunicipio
											 ,idCarretera
											 ,idTramo
											 ,kmCarretera
											 ,idVehiculo
											 ,idPersona
											 ,idPersonaInfraccion
											 ,idPersonaConductor
											 ,placasVehiculo
											 ,NumTarjetaCirculacion
											 ,idEstatusInfraccion
											 ,fechaActualizacion
											 ,actualizadoPor
											 ,estatus
											 ,transito)
									  VALUES (@fechaInfraccion
											 ,@horaInfraccion
											 ,@folioInfraccion
											 ,@idOficial
											 ,@idMunicipio
											 ,@idCarretera
											 ,@idTramo
											 ,@kmCarretera
											 ,@idVehiculo
											 ,@idPersona
											 ,@idPersonaInfraccion
											 ,@idPersonaConductor
											 ,@placasVehiculo
											 ,@NumTarjetaCirculacion
											 ,1
											 ,@fechaActualizacion
											 ,@actualizadoPor
											 ,@estatus
											 ,@idDependencia
											 );SELECT SCOPE_IDENTITY()";*/
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					SqlCommand command = new SqlCommand("usp_InsertaInfraccion", connection);
					command.CommandType = CommandType.StoredProcedure;
					//   command.Parameters.Add(new SqlParameter("fechaInfraccion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					//  command.Parameters.Add(new SqlParameter("horaInfraccion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
					command.Parameters.Add(new SqlParameter("fechaInfraccion", SqlDbType.DateTime)).Value = (object)model.fechaInfraccion;
					command.Parameters.Add(new SqlParameter("folioEmergencia", SqlDbType.NVarChar)).Value = (object)model.folioEmergencia?.ToUpper() ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("folioInfraccion", SqlDbType.NVarChar)).Value = (object)model.folioInfraccion?.ToUpper() ?? "-";

					command.Parameters.Add(new SqlParameter("idOficial", SqlDbType.Int)).Value = (object)model.idOficial ?? 0;
					command.Parameters.Add(new SqlParameter("idMunicipio", SqlDbType.Int)).Value = (object)model.IdMunicipio ?? 0;
					command.Parameters.Add(new SqlParameter("idCarretera", SqlDbType.Int)).Value = (object)model.IdCarretera ?? 0;
					command.Parameters.Add(new SqlParameter("idTramo", SqlDbType.Int)).Value = (object)model.IdTramo ?? 0;
					command.Parameters.Add(new SqlParameter("kmCarretera", SqlDbType.Float)).Value = (object)model.Kilometro ?? 0;
					command.Parameters.Add(new SqlParameter("idDelegacion", SqlDbType.Int)).Value = (object)model.idDelegacion ?? 0;
					command.Parameters.Add(new SqlParameter("lugarCalle", SqlDbType.NVarChar)).Value = DBNull.Value;
					command.Parameters.Add(new SqlParameter("lugarNumero", SqlDbType.NVarChar)).Value = DBNull.Value;
					command.Parameters.Add(new SqlParameter("lugarColonia", SqlDbType.NVarChar)).Value = DBNull.Value;
					command.Parameters.Add(new SqlParameter("lugarEntreCalle", SqlDbType.NVarChar)).Value = DBNull.Value;
					command.Parameters.Add(new SqlParameter("idVehiculo", SqlDbType.Int)).Value = (object)model.IdVehiculo ?? 0;

					//command.Parameters.Add(new SqlParameter("idPersona", SqlDbType.Int)).Value = (object)model.IdPersona ?? 0;
					//command.Parameters.Add(new SqlParameter("idPersonaInfraccion", SqlDbType.Int)).Value = (object)model.IdPersonaInfraccion ?? 0;
					command.Parameters.Add(new SqlParameter("idPersonaConductor", SqlDbType.Int)).Value = (object)model.IdPersonaInfraccion ?? 0;
					command.Parameters.Add(new SqlParameter("placasVehiculo", SqlDbType.NVarChar)).Value = (object)model.Placa?.ToUpper() ?? "-";
					command.Parameters.Add(new SqlParameter("NumTarjetaCirculacion", SqlDbType.NVarChar)).Value = (object)model.Tarjeta?.ToUpper() ?? "-";
					command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
					command.Parameters.Add(new SqlParameter("estatus", SqlDbType.Int)).Value = (object)1;
					// command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;                  
					DateTime fechaInfraccion = model.fechaInfraccion;
					TimeSpan horaInfraccion = fechaInfraccion.TimeOfDay;
					string horaFormateada = horaInfraccion.ToString("hhmm");
					string horaInfraccionString = horaFormateada;
					command.Parameters.Add(new SqlParameter("horaInfraccion", SqlDbType.NVarChar)).Value = horaInfraccionString;
					command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = idDependencia;
					command.Parameters.Add(new SqlParameter("fechaVencimiento", SqlDbType.DateTime)).Value = (object)model.fechaVencimiento;


					result = Convert.ToInt32(command.ExecuteScalar());
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

			//Crear historico vehiculo infraccion
			try
			{
				var bitacoraVehiculo = _vehiculosService.CrearHistoricoVehiculo(result, (int)model.IdVehiculo, 1);
			}
			catch { }

			return result;
		}

		public string ObtenerDescripcionCausaDesdeBD(int idAccidente)
		{
			string descripcionCausa = null;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				string query = "SELECT descripcionCausas FROM accidentes WHERE idAccidente = @idAccidente";
				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idAccidente", idAccidente);

					connection.Open();

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							descripcionCausa = reader["descripcionCausas"].ToString();
						}

						reader.Close();
					}
				}

				return descripcionCausa;
			}
		}

		public DatosAccidenteModel ObtenerDatosFinales(int idAccidente)
		{

			DatosAccidenteModel datosFinales = new DatosAccidenteModel();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				string query = @"SELECT idEstatusReporte,
                                    montoCamino, montoCarga,montoPropietarios, montoOtros,
                                    latitud ,longitud ,idCertificado ,
                                    armas ,drogas ,valores ,prendas ,otros ,entregaObjetos, entregaOtros ,
                                    consignacionHechos, idCiudad , convenio ,
                                    idAutoridadEntrega , idAutoridadDisposicion , idElaboraConsignacion , 
                                    numeroOficio , idAgenciaMinisterio ,recibeMinisterio , 
                                    idElabora , idAutoriza , idSupervisa,armasTexto,drogasTexto,valoresTexto,prendasTexto,otrosTexto,
                                    observacionesConvenio, idEntidadCompetencia,a.trayectoria,
									 b.rutaArchivoParte,b.nombreArchivoParte,c.rutaBoleta,c.nombreBoleta
                                    FROM accidentes a 
									LEFT JOIN vehiculosAccidente d ON A.idAccidente = d.idAccidente 
                                   	LEFT JOIN archivosParteAccidente b ON A.idAccidente = b.idAccidente AND b.estatus = 1
									LEFT JOIN boletasAccidentes c ON c.idAccidente = A.idAccidente AND c.estatus = 1 
									WHERE A.idAccidente = @IdAccidente
                                    ";
				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idAccidente", idAccidente);

					connection.Open();

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							datosFinales.montoCamino = reader["montoCamino"] == DBNull.Value ? "" : reader["montoCamino"].ToString();
							datosFinales.montoCarga = reader["montoCarga"] == DBNull.Value ? "" : reader["montoCarga"].ToString();
							datosFinales.montoPropietarios = reader["montoPropietarios"] == DBNull.Value ? "" : reader["montoPropietarios"].ToString();
							datosFinales.montoOtros = reader["montoOtros"] == DBNull.Value ? "" : reader["montoOtros"].ToString();


							datosFinales.Latitud = reader["latitud"] == DBNull.Value ? "0" : reader["latitud"].ToString();
							datosFinales.Longitud = reader["longitud"] == DBNull.Value ? "0" : reader["longitud"].ToString();
							datosFinales.IdCertificado = reader["idCertificado"] == DBNull.Value ? 0 : int.Parse(reader["idCertificado"].ToString());
							datosFinales.EstadoArmas = reader["armas"] == DBNull.Value ? 0 : int.Parse(reader["armas"].ToString());
							datosFinales.EstadoDrogas = reader["drogas"] == DBNull.Value ? 0 : int.Parse(reader["drogas"].ToString());
							datosFinales.EstadoValores = reader["valores"] == DBNull.Value ? 0 : int.Parse(reader["valores"].ToString());
							datosFinales.EstadoPrendas = reader["prendas"] == DBNull.Value ? 0 : int.Parse(reader["prendas"].ToString());
							datosFinales.EstadoOtros = reader["otros"] == DBNull.Value ? 0 : int.Parse(reader["otros"].ToString());
							datosFinales.EstadoConvenio = reader["convenio"] == DBNull.Value ? 0 : int.Parse(reader["convenio"].ToString());
							datosFinales.consignacionHechos = reader["consignacionHechos"] == DBNull.Value ? "" : reader["consignacionHechos"].ToString();
							datosFinales.IdCiudad = reader["idCiudad"] == DBNull.Value ? 0 : int.Parse(reader["idCiudad"].ToString());
							datosFinales.entregaObjetos = reader["entregaObjetos"] == DBNull.Value ? "" : reader["entregaObjetos"].ToString();
							datosFinales.entregaOtros = reader["entregaOtros"] == DBNull.Value ? "" : reader["entregaOtros"].ToString();
							datosFinales.IdAutoridadEntrega = reader["idAutoridadEntrega"] == DBNull.Value ? 0 : int.Parse(reader["idAutoridadEntrega"].ToString());
							datosFinales.IdAutoridadDisposicion = reader["idAutoridadDisposicion"] == DBNull.Value ? 0 : int.Parse(reader["idAutoridadDisposicion"].ToString());
							datosFinales.IdElaboraConsignacion = reader["idElaboraConsignacion"] == DBNull.Value ? 0 : int.Parse(reader["idElaboraConsignacion"].ToString());
							datosFinales.numeroOficio = reader["numeroOficio"] == DBNull.Value ? "" : reader["numeroOficio"].ToString();
							datosFinales.IdAgenciaMinisterio = reader["idAgenciaMinisterio"] == DBNull.Value ? 0 : int.Parse(reader["idAgenciaMinisterio"].ToString());
							datosFinales.RecibeMinisterio = reader["recibeMinisterio"] == DBNull.Value ? "" : reader["recibeMinisterio"].ToString();
							datosFinales.IdElabora = reader["idElabora"] == DBNull.Value ? 0 : int.Parse(reader["idElabora"].ToString());
							datosFinales.IdAutoriza = reader["idAutoriza"] == DBNull.Value ? 0 : int.Parse(reader["idAutoriza"].ToString());
							datosFinales.IdSupervisa = reader["idSupervisa"] == DBNull.Value ? 0 : int.Parse(reader["idSupervisa"].ToString());
							datosFinales.IdEstatusReporte = reader["idEstatusReporte"] == DBNull.Value ? 0 : int.Parse(reader["idEstatusReporte"].ToString());
							datosFinales.ArmasTexto = reader["armasTexto"] == DBNull.Value ? "" : reader["armasTexto"].ToString();
							datosFinales.DrogasTexto = reader["drogasTexto"] == DBNull.Value ? "" : reader["drogasTexto"].ToString();
							datosFinales.ValoresTexto = reader["valoresTexto"] == DBNull.Value ? "" : reader["valoresTexto"].ToString();
							datosFinales.PrendasTexto = reader["prendasTexto"] == DBNull.Value ? "" : reader["prendasTexto"].ToString();
							datosFinales.OtrosTexto = reader["otrosTexto"] == DBNull.Value ? "" : reader["otrosTexto"].ToString();
							datosFinales.observacionesConvenio = reader["observacionesConvenio"] == DBNull.Value ? "" : reader["observacionesConvenio"].ToString();
							datosFinales.IdEntidadCompetencia = reader["idEntidadCompetencia"] == DBNull.Value ? 0 : int.Parse(reader["idEntidadCompetencia"].ToString());
                            datosFinales.trayectoria = reader["trayectoria"] == DBNull.Value ? "" : reader["trayectoria"].ToString();

                        }


                        //RECUPERAR VALOR D PARTE FISICO
                        datosFinales.archivoPartePath = reader["rutaArchivoParte"] == DBNull.Value ? "" : reader["rutaArchivoParte"].ToString();
						datosFinales.archivoParteStr = reader["nombreArchivoParte"] == DBNull.Value ? "" : reader["nombreArchivoParte"].ToString();
						datosFinales.boletaPath = reader["rutaBoleta"] == DBNull.Value ? "" : reader["rutaBoleta"].ToString();
						datosFinales.boletaStr = reader["nombreBoleta"] == DBNull.Value ? "" : reader["nombreBoleta"].ToString();

                        reader.Close();
                    }
					
				}
			}

			return datosFinales;
		}
	
		public int EliminarRegistroInfraccion(int IdInfraccion)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "UPDATE infraccionesAccidente SET estatus = 0, fechaActualizacion = @fechaActualizacion " +
						"WHERE idInfraccion = @IdInfraccion";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.AddWithValue("@IdInfraccion", IdInfraccion);
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

		public List<CapturaAccidentesModel> ObtenerAccidentesPagination(int idOficina, Pagination pagination)
		{
			List<CapturaAccidentesModel> ListaAccidentes = new List<CapturaAccidentesModel>();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					using (SqlCommand cmd = new SqlCommand("usp_ObtieneTodosLosAccidentes", connection))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@PageIndex", pagination.PageIndex);
						cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
						cmd.Parameters.AddWithValue("@IdOficina", idOficina);
						if (pagination.Filter.Trim() != "")
							cmd.Parameters.AddWithValue("@Filter", pagination.Filter);
						if (pagination.Sort != null && pagination.Sort != "")
						{
							cmd.Parameters.AddWithValue("@SotDireccion", pagination.Sort);
							cmd.Parameters.AddWithValue("@SortMember", pagination.SortCamp);
						}

						using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
						{
							while (reader.Read())
							{
								CapturaAccidentesModel accidente = new CapturaAccidentesModel();
								accidente.IdAccidente = reader["IdAccidente"] is DBNull ? 0 : Convert.ToInt32(reader["IdAccidente"]);
								accidente.NumeroReporte = reader["NumeroReporte"] is DBNull ? string.Empty : reader["NumeroReporte"].ToString();
								accidente.Fecha = reader["Fecha"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(reader["Fecha"]);
								accidente.Hora = reader["Hora"] is DBNull ? TimeSpan.MinValue : reader.GetTimeSpan(reader.GetOrdinal("Hora"));
								accidente.IdMunicipio = reader["IdMunicipio"] is DBNull ? 0 : Convert.ToInt32(reader["IdMunicipio"]);
								accidente.IdCarretera = reader["IdCarretera"] is DBNull ? 0 : Convert.ToInt32(reader["IdCarretera"]);
								accidente.IdTramo = reader["IdTramo"] is DBNull ? 0 : Convert.ToInt32(reader["IdTramo"]);
								accidente.idEstatusReporte = reader["idEstatusReporte"] is DBNull ? 0 : Convert.ToInt32(reader["idEstatusReporte"]);
								accidente.IdEmergencia = reader["emergenciasId"] is DBNull ? null : Convert.ToInt32(reader["emergenciasId"]);
								accidente.EstatusReporte = reader["estatusReporte"] is DBNull ? string.Empty : reader["estatusReporte"].ToString();
								accidente.Municipio = reader["Municipio"] is DBNull ? string.Empty : reader["Municipio"].ToString();
								accidente.Tramo = reader["Tramo"] is DBNull ? string.Empty : reader["Tramo"].ToString();
								accidente.Carretera = reader["Carretera"] is DBNull ? string.Empty : reader["Carretera"].ToString();
								accidente.FolioEmergencia = reader["FolioEmergencia"] is DBNull ? null : Convert.ToInt32(reader["FolioEmergencia"]);
								accidente.Total = Convert.ToInt32(reader["Total"]);
								ListaAccidentes.Add(accidente);

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
				return ListaAccidentes;

			}
		}

		public string GetAccidenteFolioDetencion(int idAccidente, int idPersona)
		{
			string result = "";
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();

					string queryExistenciaInvolucrados = "SELECT idInvolucradosAccidente FROM involucradosAccidente WHERE idAccidente = @idAccidente and idPersona = @idPersona";
					SqlCommand commandExistenciaInvolucrados = new SqlCommand(queryExistenciaInvolucrados, connection);
					commandExistenciaInvolucrados.Parameters.AddWithValue("@idAccidente", idAccidente);
					commandExistenciaInvolucrados.Parameters.AddWithValue("@idPersona", idPersona);
					int idInvolucradosAccidente = (int)commandExistenciaInvolucrados.ExecuteScalar();

					if (idInvolucradosAccidente > 0)
					{
						string queryExistenciaDetenidos = "SELECT folioDetenido FROM accidentesDetenidos WHERE idInvolucradosAccidente = @idInvolucradosAccidente";
						SqlCommand commandExistenciaDetenidos = new SqlCommand(queryExistenciaDetenidos, connection);
						commandExistenciaDetenidos.Parameters.AddWithValue("@idInvolucradosAccidente", idInvolucradosAccidente);
						var folioDetenido = commandExistenciaDetenidos.ExecuteScalar();

						if (folioDetenido is not null)
						{
							result = (string)folioDetenido;
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
			}
			return result;
		}

		public IEnumerable<FileData> GetArchivosLugarAccidente(int accidenteId)
		{
			Accidente accidente = FindOrThrowIfNotExists(accidenteId);

			return string.IsNullOrWhiteSpace(accidente.UrlAccidenteArchivosLugar)
				? Enumerable.Empty<FileData>()
				: _accidenteFileManager.GetAllLugarFiles(accidente.UrlAccidenteArchivosLugar);
		}


		public void GuardarArchivosLugarAccidente(int accidenteId, IEnumerable<IFormFile> files)
		{
			Accidente accidente = FindOrThrowIfNotExists(accidenteId);

			foreach (var file in files)
			{
				if (string.IsNullOrWhiteSpace(accidente.UrlAccidenteArchivosLugar))
				{
					accidente.UrlAccidenteArchivosLugar = _accidenteFileManager.UploadLugarFileInDefaultUrl(accidenteId, file.FileName, file.OpenReadStream());
					this.dbContext.Update(accidente);
					this.dbContext.SaveChanges();
					continue;
				}

				_accidenteFileManager.UploadFile(accidente.UrlAccidenteArchivosLugar, file.FileName, file.OpenReadStream());
			}
		}

		public void EliminarArchivosLugarAccidente(int accidenteId, IEnumerable<string> files)
		{
			Accidente accidente = FindOrThrowIfNotExists(accidenteId);

			// si no hay un directorio de archivos, no hay archivos que eliminar
			if (string.IsNullOrWhiteSpace(accidente.UrlAccidenteArchivosLugar))
				return;

			foreach (var file in files)
			{
				_accidenteFileManager.DeleteFile(accidente.UrlAccidenteArchivosLugar, file);
			}				
		}

		
		#region Private Methods
		private void UpsertTurnoAsignedToAccidente(SqlConnection connection, SqlTransaction transaction, long idTurno, int idAccidente)
		{
			string table = "turnoAccidentes";
			string infraccionField = "idAccidente";
			string turnoField = "idTurno";
			string query = $@"
				IF EXISTS (SELECT 1 FROM {table} WHERE {infraccionField} = @accidente)
				BEGIN
					UPDATE {table} SET {turnoField} = @turno
					WHERE {infraccionField} = @accidente;
				END
				ELSE
				BEGIN
					INSERT INTO {table} ({infraccionField}, {turnoField})
					VALUES (@accidente, @turno);
				END";

			SqlCommand command = new(query, connection, transaction);
			command.Parameters.Add(new SqlParameter("@accidente", SqlDbType.Int)).Value = idAccidente;
			command.Parameters.Add(new SqlParameter("@turno", SqlDbType.BigInt)).Value = idTurno;

			command.ExecuteNonQuery();
		}

		private void RemoveTurnoAssignmentToAccidente(SqlConnection connection, SqlTransaction trx, int idAccidente)
		{
			using SqlCommand turnoCommand = new("DELETE FROM turnoAccidentes WHERE idAccidente = @accidente", connection, trx);
			turnoCommand.Parameters.Add(new SqlParameter("accidente", SqlDbType.Int)).Value = idAccidente;
			turnoCommand.ExecuteNonQuery();
		}

		private Accidente FindOrThrowIfNotExists(int accidenteId)
			=> this.dbContext.Accidentes.Find(accidenteId)
				?? throw new NotFoundException($"{nameof(Accidente)} '{accidenteId}' no encontrado.");

		#endregion Private Methods

	}
}