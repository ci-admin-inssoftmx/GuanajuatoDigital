using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using static iTextSharp.text.pdf.AcroFields;
//using Telerik.SvgIcons;

namespace GuanajuatoAdminUsuarios.Services
{
    public class AsignacionGruasService : IAsignacionGruasService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        private readonly IVehiculosService _vehiculosService;
        private readonly IPersonasService _personasService;
        public AsignacionGruasService(ISqlClientConnectionBD sqlClientConnectionBD
                                  , IVehiculosService vehiculosService
                                  , IPersonasService personasService)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
            _vehiculosService = vehiculosService;
            _personasService = personasService;
        }

        public List<AsignacionGruaModel> BuscarSolicitudes(AsignacionGruaModel model, int idOficina, int idDependencia)
        {
            //
            List<AsignacionGruaModel> ListaSolicitudes = new List<AsignacionGruaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("usp_CapturaGruasBusquedaDeSolicitudes", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@folioBusqueda", SqlDbType.VarChar)).Value = (String.IsNullOrEmpty(model.FolioSolicitud)) ? DBNull.Value : model.FolioSolicitud;
                    command.Parameters.Add(new SqlParameter("@fechaSolicitud", SqlDbType.Date)).Value = (model.fecha.Year == 1) ? DBNull.Value : model.fecha;
                    command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = idOficina;
                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.VarChar)).Value = idDependencia;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            AsignacionGruaModel solicitud = new AsignacionGruaModel();
                            solicitud.idSolicitud = reader["idSolicitud"] != DBNull.Value ? Convert.ToInt32(reader["idSolicitud"]) : 0;
                            solicitud.FolioSolicitud = reader["folio"].ToString();
                            solicitud.fechaSolicitud = reader["fechaSolicitud"] != DBNull.Value ? Convert.ToDateTime(reader["fechaSolicitud"]) : DateTime.MinValue;
                            solicitud.idPension = reader["idPension"] != DBNull.Value ? Convert.ToInt32(reader["idPension"]) : 0;
                            solicitud.idPropietarioGrua = reader["idPropietarioGrua"] != DBNull.Value ? Convert.ToInt32(reader["idPropietarioGrua"]) : 0;
                            solicitud.vehiculoCalle = reader["vehiculoCalle"].ToString();
                            solicitud.vehiculoColonia = reader["vehiculoColonia"].ToString();
                            solicitud.municipio = reader["municipio"].ToString();
                            solicitud.carretera = reader["carretera"].ToString();
                            solicitud.nombreEntidad = reader["nombreEntidad"].ToString();
                            solicitud.tipoUsuario = reader["tipoUsuario"].ToString();
                            solicitud.nombreOficial = reader["nombre"].ToString();
                            solicitud.apellidoPaternoOficial = reader["apellidoPaterno"].ToString();
                            solicitud.apellidoMaternoOficial = reader["apellidoMaterno"].ToString();
                            solicitud.concesionario = reader["concesionario"].ToString();


                            ListaSolicitudes.Add(solicitud);

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
            return ListaSolicitudes;


        }
        public List<AsignacionGruaModel> ObtenerTodasSolicitudes()
        {
            //
            List<AsignacionGruaModel> ListaSolicitudes = new List<AsignacionGruaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT sol.idSolicitud,sol.folio,sol.fechaSolicitud,sol.idMunicipioUbicacion,sol.idCarreteraUbicacion, " + "" +
                        "sol.idEntidadUbicacion,sol.vehiculoCalle,sol.vehiculoCarretera,sol.vehiculoColonia,sol.idOficial,sol.idTipoUsuario, " +
                        "mun.municipio, " +
                        "car.carretera, " +
                        "ent.nombreEntidad, " +
                        "tip_us.tipoUsuario, " +
                        "ofi.nombre,ofi.apellidoPaterno,ofi.apellidoMaterno " +
                        "From solicitudes AS sol " +
                        "LEFT JOIN catMunicipios AS mun ON sol.idMunicipioUbicacion = mun.idMunicipio " +
                        "LEFT JOIN catCarreteras AS car ON sol.idCarreteraUbicacion = car.idCarretera " +
                        "LEFT JOIN catEntidades AS ent ON sol.idEntidadUbicacion = ent.idEntidad " +
                        "LEFT JOIN catTiposUsuario AS tip_us ON sol.idTipoUsuario = tip_us.idTipoUsuario " +
                        "LEFT JOIN catOficiales AS ofi ON sol.idOficial = ofi.idOficial " +
                        "WHERE sol.estatus = 1", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            AsignacionGruaModel solicitud = new AsignacionGruaModel();
                            solicitud.idSolicitud = reader["idSolicitud"] != DBNull.Value ? Convert.ToInt32(reader["idSolicitud"]) : 0;
                            solicitud.FolioSolicitud = reader["folio"].ToString();
                            solicitud.fechaSolicitud = reader["fechaSolicitud"] != DBNull.Value ? Convert.ToDateTime(reader["fechaSolicitud"]) : DateTime.MinValue;

                            solicitud.vehiculoCalle = reader["vehiculoCalle"].ToString();
                            solicitud.vehiculoColonia = reader["vehiculoColonia"].ToString();
                            solicitud.municipio = reader["municipio"].ToString();
                            solicitud.carretera = reader["carretera"].ToString();
                            solicitud.nombreEntidad = reader["nombreEntidad"].ToString();
                            solicitud.tipoUsuario = reader["tipoUsuario"].ToString();
                            solicitud.nombreOficial = reader["nombre"].ToString();
                            solicitud.apellidoPaternoOficial = reader["apellidoPaterno"].ToString();
                            solicitud.apellidoPaternoOficial = reader["apellidoMaterno"].ToString();


                            ListaSolicitudes.Add(solicitud);

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
            return ListaSolicitudes;


        }

        public List<AsignacionGruaModel> BuscarPorParametro(string Placa, string Serie)
        {
            List<AsignacionGruaModel> Vehiculo = new List<AsignacionGruaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command;

                    if (!string.IsNullOrEmpty(Serie))
                    {
                        command = new SqlCommand(
                       "SELECT TOP 200 v.*, mv.marcaVehiculo, sm.nombreSubmarca, e.nombreEntidad, cc.color, tv.tipoVehiculo, ts.tipoServicio, p.nombre, p.apellidoPaterno, p.apellidoMaterno,p.CURP,p.RFC, " +
                        "FROM vehiculos v " +
                        "JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
                        "JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca " +
                        "JOIN catEntidades e ON v.idEntidad = e.idEntidad " +
                        "JOIN catColores cc ON v.idColor = cc.idColor " +
                        "JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
                        "JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
                        "JOIN personas p ON v.idPersona = p.idPersona " +
                        "WHERE v.estatus = 1 AND v.serie LIKE '%' + @Serie + '%' ORDER BY v.idVehiculo DESC;", connection);

                        command.Parameters.AddWithValue("@Serie", Serie);
                    }
                    else if (!string.IsNullOrEmpty(Placa))
                    {
                        command = new SqlCommand("SELECT TOP 200 v.*, mv.marcaVehiculo, sm.nombreSubmarca, e.nombreEntidad, cc.color, tv.tipoVehiculo, ts.tipoServicio,p.nombre, p.apellidoPaterno, p.apellidoMaterno, " +
                            "p.CURP,p.RFC " +
                            "FROM vehiculos v " +
                            "JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
                            "JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca " +
                            "JOIN catEntidades e ON v.idEntidad = e.idEntidad " +
                            "JOIN catColores cc ON v.idColor = cc.idColor " +
                            "JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
                            "JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
                            "JOIN personas p ON v.idPersona = p.idPersona " +
                            "WHERE v.estatus = 1 AND v.placas LIKE '%' + @Placa + '%' ORDER BY v.idVehiculo DESC;", connection);
                        command.Parameters.AddWithValue("@Placa", Placa);
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
                            AsignacionGruaModel vehiculo = new AsignacionGruaModel();
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
                            vehiculo.Placa = reader["Placas"].ToString();
                            vehiculo.Tarjeta = reader["Tarjeta"].ToString();
                            vehiculo.VigenciaTarjeta = Convert.IsDBNull(reader["VigenciaTarjeta"]) ? DateTime.MinValue : Convert.ToDateTime(reader["VigenciaTarjeta"]);
                            vehiculo.Serie = reader["serie"].ToString();
                            vehiculo.EntidadRegistro = reader["nombreEntidad"].ToString();
                            vehiculo.Color = reader["color"].ToString();
                            vehiculo.TipoServicio = reader["tipoServicio"].ToString();
                            vehiculo.TipoVehiculo = reader["tipoVehiculo"].ToString();
                            vehiculo.Motor = reader["motor"].ToString();
                            vehiculo.NumeroEconomico = reader["numeroEconomico"].ToString();
                            vehiculo.Propietario = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
                            vehiculo.CURP = reader["CURP"].ToString();
                            vehiculo.RFC = reader["RFC"].ToString();

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

        public AsignacionGruaModel BuscarSolicitudPord(int iSo, string folio, int idOficina, int idDependencia)
        {
            AsignacionGruaModel solicitud = new AsignacionGruaModel();
            string whereDeposito = " WHERE A.idSolicitud = @FolioIdSolicitud AND B.BanderaTransito = @dependencia";
            if (!folio.IsNullOrEmpty())
            {
                whereDeposito = " WHERE A.folio = @FolioIdSolicitud AND B.BanderaTransito = @dependencia";
            }
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();

                    // Consulta para buscar si el folio ya existe en la tabla depositos
                    SqlCommand searchCommand = new SqlCommand(@"SELECT	ISNULL(A.idSolicitud,0) idSolicitud, 
		                                                                ISNULL(A.idDeposito,0) idDeposito ,
		                                                                ISNULL(A.folio,'') folio ,
		                                                                A.observaciones,
		                                                                A.numeroInventario,
                                                                        A.estatusSolicitud,
                                                                        B.idPropietarioGrua,
		                                                                isnull(A.archivoInventario, inf.archivoInventario) ExistInv ,
		                                                                isnull(A.nombreArchivo, inf.nombreArchivo) nombreArchivo,
                                                                        a.archivoInventario  inventario,
		                                                                inf.idInfraccion, 
		                                                                inf.idVehiculo,
		                                                                inf.idPersona,
		                                                                inf.folioInfraccion,
		                                                                inf.fechaInfraccion,
                                                                        v.placas,v.serie,
		                                                                v.idMarcaVehiculo,
		                                                                v.idSubmarca,
		                                                                v.modelo,
		                                                                v.idPersona,
		                                                                v.tarjeta,
                                                                        p.nombre,
		                                                                p.apellidoPaterno,
		                                                                p.apellidoMaterno,
		                                                                p.CURP,
		                                                                p.RFC,
                                                                        cmv.marcaVehiculo,
		                                                                csv.nombreSubmarca,
		                                                                col.color,
		                                                                inf.folioInfraccion FolioVinculado
                                                                FROM depositos			A 
                                                                LEFT JOIN solicitudes	B	ON A.idSolicitud = B.idSolicitud
                                                                LEFT JOIN infracciones	inf ON B.idInfraccion = inf.idInfraccion

                                                                LEFT JOIN opeDepositosVehiculos v ON v.idOperacion = 
											                    (
											                       SELECT MAX(idOperacion) 
											                       FROM opeDepositosVehiculos z 
											                       WHERE z.idVehiculo = inf.idVehiculo and z.idDeposito = A.idDeposito
											                    )

                                                                LEFT JOIN personas		p	ON v.idPersona = p.idPersona
                                                                LEFT JOIN catMarcasVehiculos as cmv ON v.idMarcaVehiculo = cmv.idMarcaVehiculo
                                                                LEFT JOIN catSubmarcasVehiculos as csv ON v.idSubmarca = csv.idSubmarca
                                                                LEFT JOIN catColores  col ON v.idColor = col.idColor " + whereDeposito, connection);
                   
                    searchCommand.Parameters.Add(new SqlParameter("@dependencia", SqlDbType.Int)).Value = idDependencia;

                    if (!folio.IsNullOrEmpty())
                        searchCommand.Parameters.Add(new SqlParameter("@FolioIdSolicitud", SqlDbType.NVarChar)).Value = folio;
                    else
                        searchCommand.Parameters.Add(new SqlParameter("@FolioIdSolicitud", SqlDbType.Int)).Value = iSo;

                    // Ejecutar la consulta de búsqueda
                    using (SqlDataReader searchReader = searchCommand.ExecuteReader())
                    {
                        // ...
                        if (searchReader.Read())
                        {
                            //Se obtiene el nombre del propietario
                            string nombrePropietario = searchReader["nombre"] == System.DBNull.Value ? "" : searchReader["nombre"].ToString().Trim();
                            string apellidoPaternoPropietario = searchReader["apellidoPaterno"] == System.DBNull.Value ? "" : searchReader["apellidoPaterno"].ToString().Trim();
                            string apellidoMaternoPropietario = searchReader["apellidoMaterno"] == System.DBNull.Value ? "" : searchReader["apellidoMaterno"].ToString().Trim();
                            string nombreCompletoPropietario = nombrePropietario + " " + apellidoPaternoPropietario + " " + apellidoMaternoPropietario;

                            solicitud.idSolicitud = int.Parse(searchReader["idSolicitud"].ToString());
                            solicitud.FolioSolicitud = searchReader["folio"].ToString();
                            solicitud.folioInfraccion = searchReader["FolioVinculado"].ToString();

                            solicitud.observaciones = searchReader["observaciones"].ToString();
                            solicitud.numeroInventario = searchReader["numeroInventario"].ToString();
                            solicitud.inventarios = searchReader["ExistInv"] is DBNull ? "" : searchReader["ExistInv"].ToString();
                            solicitud.Nombreinventarios = searchReader["nombreArchivo"] is DBNull ? "" : searchReader["nombreArchivo"].ToString();

                            solicitud.estatusSolicitud = int.Parse(searchReader["estatusSolicitud"].ToString());
                            if (!searchReader.IsDBNull(searchReader.GetOrdinal("idPropietarioGrua")))
                            {
                                solicitud.idPropietarioGrua = searchReader.GetInt32(searchReader.GetOrdinal("idPropietarioGrua"));
                            }
                            else
                            {
                                solicitud.idPropietarioGrua = 0;
                            }

                            solicitud.observaciones = searchReader["observaciones"].ToString();
                            solicitud.IdDeposito = int.Parse(searchReader["idDeposito"].ToString());
                            //HMG - NUEVOS CAMPOS
                            solicitud.idInfraccion = searchReader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(searchReader["idInfraccion"].ToString());
                            if (searchReader["idVehiculo"] != System.DBNull.Value && searchReader["idVehiculo"] != null)
                            {
                                solicitud.IdVehiculo = Convert.ToInt32(searchReader["idVehiculo"]);
                            }
                            else
                            {
                                solicitud.IdVehiculo = 0;
                            }
                            solicitud.fechaInfraccion = searchReader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(searchReader["fechaInfraccion"].ToString());
                            solicitud.IdMarcaVehiculo = Convert.IsDBNull(searchReader["IdMarcaVehiculo"]) ? 0 : Convert.ToInt32(searchReader["IdMarcaVehiculo"]);
                            solicitud.IdSubmarca = Convert.IsDBNull(searchReader["IdSubmarca"]) ? 0 : Convert.ToInt32(searchReader["IdSubmarca"]);
                            solicitud.Marca = searchReader["marcaVehiculo"].ToString();
                            solicitud.Submarca = searchReader["nombreSubmarca"].ToString();
                            solicitud.Modelo = searchReader["Modelo"].ToString();
                            solicitud.Color = searchReader["color"].ToString();
                            solicitud.Placa = searchReader["placas"].ToString();
                            solicitud.Serie = searchReader["serie"].ToString();
                            solicitud.Tarjeta = searchReader["tarjeta"].ToString();
                            solicitud.Propietario = nombreCompletoPropietario.Trim();
                            solicitud.CURP = searchReader["CURP"].ToString();
                            solicitud.RFC = searchReader["RFC"].ToString();

                            string filePath = searchReader["inventario"].ToString();

                            if (!string.IsNullOrEmpty(filePath))
                            {
                                var file = new FormFile(Stream.Null, 0, 0, "MyFile", Path.GetFileName(filePath))
                                {
                                    Headers = new HeaderDictionary(),
                                    ContentType = "application/octet-stream"
                                };

                                solicitud.MyFile = file;
                            }
                            else
                            {
                                solicitud.MyFile = null;
                            }

                            return solicitud;
                        }
                    }


                    // HMG: Se quita porque esta funcionalidad la realiza el SP: usp_InsertaSolicitud
                    // 15-03-2024


                    // Continuar con la consulta y la inserción
                    //string whereSolicitud = "WHERE folio = @FolioIdSolicitud";
                    //if (iSo > 0)
                    //    whereSolicitud = "WHERE idSolicitud = @FolioIdSolicitud";
                    //SqlCommand command = new SqlCommand("SELECT ISNULL(sol.idSolicitud,0) idSolicitud,sol.fechaSolicitud,ISNULL(sol.folio,'') folio ,ISNULL(sol.idPropietarioGrua,0) idPropietarioGrua,ISNULL(sol.idPension,0) idPension,ISNULL(sol.idTramoUbicacion,0) idTramoUbicacion, " +
                    //                                    "sol.vehiculoKm, sol.idInfraccion " +
                    //                                    "FROM solicitudes AS sol " +
                    //                                    whereSolicitud, connection);
                    //if(!folio.IsNullOrEmpty())
                    //command.Parameters.Add(new SqlParameter("@FolioIdSolicitud", SqlDbType.NVarChar)).Value = folio;
                    //else
                    //command.Parameters.Add(new SqlParameter("@FolioIdSolicitud", SqlDbType.NVarChar)).Value = iSo;

                    //command.CommandType = System.Data.CommandType.Text;
                    //using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    //{
                    //    while (reader.Read())
                    //    {
                    //        solicitud.idSolicitud = reader["idSolicitud"] != DBNull.Value ? Convert.ToInt32(reader["idSolicitud"]) : 0;
                    //        solicitud.idInfraccion = reader["idInfraccion"] != DBNull.Value ? Convert.ToInt32(reader["idInfraccion"]) : 0;

                    //        solicitud.idPropietarioGrua = reader["idPropietarioGrua"] != DBNull.Value ? Convert.ToInt32(reader["idPropietarioGrua"]) : 0;
                    //        solicitud.FolioSolicitud = reader["folio"].ToString();
                    //        solicitud.idPension = reader["idPension"] != DBNull.Value ? Convert.ToInt32(reader["idPension"]) : 0;
                    //        solicitud.idTramoUbicacion = reader["idTramoUbicacion"] != DBNull.Value ? Convert.ToInt32(reader["idTramoUbicacion"]) : 0;
                    //        solicitud.kilometro = reader["vehiculoKm"].ToString();
                    //        solicitud.fechaSolicitud = reader["fechaSolicitud"] != DBNull.Value ? Convert.ToDateTime(reader["fechaSolicitud"]) : DateTime.MinValue;

                    //    }
                    //}

                    //int idDeposito = -1;
                    //using (SqlConnection insertConnection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                    //{
                    //    insertConnection.Open();
                    //    SqlCommand insertCommand = new SqlCommand("INSERT INTO depositos " +
                    //                                            "(idSolicitud,folio,idTramo,idPension,idInfraccion,km,liberado,IdConcesionario,idDelegacion,idDependenciaGenera,estatus,esExterno) " +
                    //                                            "VALUES (@idSolicitud,@folio,@idTramo,@idPension,@idInfraccion,@km,@liberado,@idPropietarioGruas,@idDelegacion,@idDependencia,@estatus,@esExterno);" +
                    //                                            "SELECT SCOPE_IDENTITY()", insertConnection);
                    //    insertCommand.Parameters.Add(new SqlParameter("@idSolicitud", SqlDbType.Int)).Value = solicitud.idSolicitud;
                    //    insertCommand.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = idOficina;
                    //    insertCommand.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = solicitud.idInfraccion;

                    //    insertCommand.Parameters.Add(new SqlParameter("@folio", SqlDbType.VarChar, 50)).Value = solicitud.FolioSolicitud;
                    //    insertCommand.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = solicitud.idTramoUbicacion;
                    //    insertCommand.Parameters.Add(new SqlParameter("@idPropietarioGruas", SqlDbType.Int)).Value = solicitud.idPropietarioGrua;
                    //    insertCommand.Parameters.Add(new SqlParameter("@idPension", SqlDbType.Int)).Value = solicitud.idPension;
                    //    insertCommand.Parameters.Add(new SqlParameter("@km", SqlDbType.NVarChar)).Value = solicitud.kilometro;
                    //    insertCommand.Parameters.Add(new SqlParameter("@liberado", SqlDbType.Int)).Value = 0;
                    //    insertCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    //    insertCommand.Parameters.Add(new SqlParameter("@esExterno", SqlDbType.Bit)).Value = 0;
                    //    insertCommand.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = idDependencia;

                    //    idDeposito = Convert.ToInt32(insertCommand.ExecuteScalar());
                    //    solicitud.IdDeposito = idDeposito;
                    //}

                }
                catch (Exception ex)
                {
                    // Manejar las excepciones adecuadamente
                }
                finally
                {
                    connection.Close();
                }
            }

            return solicitud;
        }


        public int ActualizarDatos(AsignacionGruaModel selectedRowData, int iDep)
        {
            int result = 0;
            int idSolicitud = -1; // Variable para almacenar el idSolicitud recuperado

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                SqlTransaction transaction = null;

                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    // Actualizar datos en la tabla depositos
                    SqlCommand updateDepositosCommand = new SqlCommand(@"UPDATE depositos 
                                                                 SET idVehiculo = @idVehiculo, 
                                                                     idMarca = @idMarca,
                                                                     idSubmarca = @idSubmarca, 
                                                                     idColor = @idColor, 
                                                                     serie = @Serie,
                                                                     placa = @Placa, 
                                                                     idInfraccion = @idInfraccion,
                                                                    fechaActualizacion = @fechaActualizacion
                                                                 WHERE idDeposito = @idDeposito", connection);
                    updateDepositosCommand.Parameters.AddWithValue("@idVehiculo", selectedRowData.IdVehiculo);
                    updateDepositosCommand.Parameters.AddWithValue("@idMarca", selectedRowData.IdMarcaVehiculo);
                    updateDepositosCommand.Parameters.AddWithValue("@idSubmarca", selectedRowData.IdSubmarca);
                    updateDepositosCommand.Parameters.AddWithValue("@idColor", selectedRowData.IdColor);
                    updateDepositosCommand.Parameters.AddWithValue("@Serie", selectedRowData.Serie.ToUpper());
                    updateDepositosCommand.Parameters.AddWithValue("@Placa", selectedRowData.Placa.ToUpper());
                    updateDepositosCommand.Parameters.AddWithValue("@idInfraccion", selectedRowData.idInfraccion);
                    updateDepositosCommand.Parameters.AddWithValue("@fechaActualizacion",DateTime.Now );
                    updateDepositosCommand.Parameters.AddWithValue("@idDeposito", iDep);

                    updateDepositosCommand.Transaction = transaction;
                    result = updateDepositosCommand.ExecuteNonQuery();

                    // Recuperar idSolicitud de la tabla depositos
                    SqlCommand selectIdSolicitudCommand = new SqlCommand("SELECT idSolicitud FROM depositos WHERE idDeposito = @idDeposito", connection, transaction);
                    selectIdSolicitudCommand.Parameters.AddWithValue("@idDeposito", iDep);

                    using (SqlDataReader reader = selectIdSolicitudCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idSolicitud = reader.GetInt32(0);
                        }
                    }

                    // Actualizar datos en la tabla solicitudes
                    if (idSolicitud != -1)
                    {
                        SqlCommand updateSolicitudesCommand = new SqlCommand(@"UPDATE solicitudes 
                                                                       SET idVehiculo = @idVehiculo, 
                                                                           idInfraccion = @idInfraccion,
                                                                            fechaActualizacion = @fechaActualizacion
                                                                       WHERE idSolicitud = @idSolicitud", connection, transaction);
                        updateSolicitudesCommand.Parameters.AddWithValue("@idVehiculo", selectedRowData.IdVehiculo);
                        updateSolicitudesCommand.Parameters.AddWithValue("@idInfraccion", selectedRowData.idInfraccion);
                        updateSolicitudesCommand.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                        updateSolicitudesCommand.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

                        result += updateSolicitudesCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (SqlException ex)
                {
                    // Si ocurre un error, se hace rollback
                    transaction?.Rollback();
                    Console.WriteLine("Error al actualizar los datos: " + ex.Message);
                    return result; // Esto devuelve 0, lo que indica un fallo en la actualización
                }
                finally
                {
                    connection.Close();
                }
            }

            //Crear historico vehiculo depositos
            try
            {
                var bitacoraVehiculo = _vehiculosService.CrearHistoricoVehiculo(iDep, (int)selectedRowData.IdVehiculo, 3);
            }
            catch { }
            InsertarDepositosPersonas(selectedRowData,iDep, selectedRowData.IdPersona);

            return result;
        }

        public List<AsignacionGruaModel> ObtenerInfracciones(string folioInfraccion)
        {
            List<AsignacionGruaModel> modelList = new List<AsignacionGruaModel>();
            string strQuery = @"SELECT inf.idInfraccion, inf.idVehiculo,inf.idPersona,inf.folioInfraccion,inf.fechaInfraccion,inf.horaInfraccion,
                                        v.placas,v.serie,v.idMarcaVehiculo,v.idSubmarca,v.modelo,v.idPersona,v.tarjeta,
                                        p.nombre,p.apellidoPaterno,p.apellidoMaterno,p.CURP,p.RFC,
                                        cmv.marcaVehiculo,csv.nombreSubmarca,col.color,
                                        inf.archivoInventario,inf.nombreArchivo

                                        FROM infracciones AS inf
                                        LEFT JOIN vehiculos AS v ON inf.idVehiculo = v.idVehiculo
                                        LEFT JOIN personas AS p ON v.idPersona = p.idPersona
                                        LEFT JOIN catMarcasVehiculos as cmv ON v.idMarcaVehiculo = cmv.idMarcaVehiculo
                                        LEFT JOIN catSubmarcasVehiculos as csv ON v.idSubmarca = csv.idSubmarca
                                        LEFT JOIN catColores AS col ON v.idColor = col.idColor
                                        WHERE inf.estatus = 1 AND inf.folioInfraccion = @folioInfraccion";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    // command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@folioInfraccion", SqlDbType.NVarChar)).Value = (object)folioInfraccion ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            AsignacionGruaModel model = new AsignacionGruaModel();

                            model.NombreArchivo = reader["nombreArchivo"] is DBNull ? "" : (string)reader["nombreArchivo"];
                            model.PathArchivo = reader["archivoInventario"] is DBNull ? "" : (string)reader["archivoInventario"];

                            model.IdPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());

                            model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
                            model.IdVehiculo = (int)(reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString()));
                            model.folioInfraccion = reader["folioInfraccion"].ToString();
                            model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
                            model.horaInfraccion = reader["horaInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["horaInfraccion"].ToString());

                            model.IdMarcaVehiculo = Convert.IsDBNull(reader["IdMarcaVehiculo"]) ? 0 : Convert.ToInt32(reader["IdMarcaVehiculo"]);
                            model.IdSubmarca = Convert.IsDBNull(reader["IdSubmarca"]) ? 0 : Convert.ToInt32(reader["IdSubmarca"]);
                            model.Marca = reader["marcaVehiculo"].ToString();
                            model.Submarca = reader["nombreSubmarca"].ToString();
                            model.Modelo = reader["Modelo"].ToString();
                            model.Color = reader["color"].ToString();
                            model.Placa = reader["placas"].ToString();
                            model.Serie = reader["serie"].ToString();
                            model.Tarjeta = reader["tarjeta"].ToString();
                            model.Propietario = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
                            model.CURP = reader["CURP"].ToString();
                            model.RFC = reader["RFC"].ToString();

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
        public int UpdateDatosGrua(IFormCollection formData, int abanderamiento, int arrastre, int salvamento, int iDep, int iSo, string horaInicioInsertEdit, string horaArriboInsertEdit, string horaTerminoInsertEdit)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = @"UPDATE gruasAsignadas SET  fechaArribo     =   @fechaArribo,
                                                                fechaInicio     =   @fechaInicio,
                                                                fechaFinal      =   @fechaFinal,
                                                                operadorGrua    =   @operadorGrua,
                                                                abanderamiento  =   @abanderamiento,
                                                                arrastre        =   @arrastre,
                                                                salvamento      =   @salvamento,
                                                                idGrua          =   @idGrua ,
                                                                minutosManiobra =   @minutosManiobra,
                                                                idDeposito      =   @idDeposito,
                                                                actualizadoPor  =   @actualizadoPor,
                                                                fechaActualizacion= @fechaActualizacion,
                                                                estatus         =   @estatus
                                WHERE idAsignacion = @idAsignacion";

                    SqlCommand command = new SqlCommand(query, connection);

                    DateTime fechaInicio = DateTime.Parse(formData["fechaInicio"]);
                    TimeSpan horaInicio = TimeSpan.Parse(Convert.ToDateTime(formData["horaInicioInsertEdit"].ToString().Split("GMT")[0].Trim()).ToString("HH:mm"));
                    DateTime fechaHoraInicio = fechaInicio.Date.Add(horaInicio);
                    command.Parameters.AddWithValue("@fechaInicio", fechaHoraInicio);

                    DateTime fechaArribo = DateTime.Parse(formData["fechaArribo"]);
                    TimeSpan horaArribo = TimeSpan.Parse(Convert.ToDateTime(formData["horaArriboInsertEdit"].ToString().Split("GMT")[0].Trim()).ToString("HH:mm"));
                    DateTime fechaHoraArribo = fechaArribo.Date.Add(horaArribo);
                    command.Parameters.AddWithValue("@fechaArribo", fechaHoraArribo);

                    DateTime fechaFinal = DateTime.Parse(formData["fechaFinal"]);
                    TimeSpan horaTermino = TimeSpan.Parse(Convert.ToDateTime(formData["horaTerminoInsertEdit"].ToString().Split("GMT")[0].Trim()).ToString("HH:mm"));
                    DateTime fechaHoraFinal = fechaFinal.Date.Add(horaTermino);
                    command.Parameters.AddWithValue("@fechaFinal", fechaHoraFinal);
                    if (formData.ContainsKey("operadorGrua"))
                    {
                        var operadorGruaValues = formData["operadorGrua"];

                        string operadorGruaString = operadorGruaValues.FirstOrDefault();

                        if (!string.IsNullOrEmpty(operadorGruaString))
                        {
                            string operadorGruaUpper = operadorGruaString.ToUpper();

                            command.Parameters.AddWithValue("@operadorGrua", operadorGruaString);
                        }
                    }
                    command.Parameters.AddWithValue("@abanderamiento", abanderamiento);
                    command.Parameters.AddWithValue("@arrastre", arrastre);
                    command.Parameters.AddWithValue("@salvamento", salvamento);
                    command.Parameters.AddWithValue("@minutosManiobra", int.Parse(formData["tiempoManiobras"].ToString()));
                    command.Parameters.AddWithValue("@idDeposito", iDep);
                    //command.Parameters.AddWithValue("@idSolicitud", iSo);
                    command.Parameters.AddWithValue("@idAsignacion", int.Parse(formData["idAsignacion"].ToString()));

                    command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);
                    command.Parameters.AddWithValue("@actualizadoPor", 1);
                    command.Parameters.AddWithValue("@estatus", 1);
                    int idGrua;
                    if (int.TryParse(formData["idGrua"].ToString(), out idGrua))
                    {
                        command.Parameters.AddWithValue("@idGrua", idGrua);
                    }
                    else
                    {



                        command.Parameters.AddWithValue("@idGrua", DBNull.Value);
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



        public int InsertDatosGrua(IFormCollection formData, int abanderamiento, int arrastre, int salvamento, int iDep, int iSo, string horaInicioInsert, string horaArriboInsert, string horaTerminoInsert)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO gruasAsignadas (fechaArribo,fechaInicio,fechaFinal,operadorGrua,abanderamiento,arrastre,salvamento,idGrua,minutosManiobra,idDeposito,actualizadoPor,fechaActualizacion,estatus)" +
                                    "VALUES(@fechaArribo,@fechaInicio,@fechaFinal,@operadorGrua,@abanderamiento,@arrastre,@salvamento,@idGrua,@minutosManiobra,@idDeposito,@actualizadoPor,@fechaActualizacion,@estatus)";

                    SqlCommand command = new SqlCommand(query, connection);
                    DateTime fechaInicio = DateTime.Parse(formData["fechaInicio"]);
                    TimeSpan horaInicio = TimeSpan.Parse(Convert.ToDateTime(formData["horaInicioInsert"].ToString().Split("GMT")[0].Trim()).ToString("HH:mm"));
                    DateTime fechaHoraInicio = fechaInicio.Date.Add(horaInicio);
                    command.Parameters.AddWithValue("@fechaInicio", fechaHoraInicio);

                    DateTime fechaArribo = DateTime.Parse(formData["fechaArribo"]);
                    TimeSpan horaArribo = TimeSpan.Parse(Convert.ToDateTime(formData["horaArriboInsert"].ToString().Split("GMT")[0].Trim()).ToString("HH:mm"));
                    DateTime fechaHoraArribo = fechaArribo.Date.Add(horaArribo);
                    command.Parameters.AddWithValue("@fechaArribo", fechaHoraArribo);

                    DateTime fechaFinal = DateTime.Parse(formData["fechaFinal"]);
                    TimeSpan horaTermino = TimeSpan.Parse(Convert.ToDateTime(formData["horaTerminoInsert"].ToString().Split("GMT")[0].Trim()).ToString("HH:mm"));
                    DateTime fechaHoraFinal = fechaFinal.Date.Add(horaTermino);
                    command.Parameters.AddWithValue("@fechaFinal", fechaHoraFinal);

                    //command.Parameters.AddWithValue("@fechaArribo", DateTime.Parse(formData["fechaArribo"].ToString()));
                    //command.Parameters.AddWithValue("@fechaInicio", DateTime.Parse(formData["fechaInicio"].ToString()));
                    //command.Parameters.AddWithValue("@fechaFinal", DateTime.Parse(formData["fechaFinal"].ToString()));
                    if (formData.ContainsKey("operadorGrua"))
                    {
                        var operadorGruaValues = formData["operadorGrua"];

                        string operadorGruaString = operadorGruaValues.FirstOrDefault();

                        if (!string.IsNullOrEmpty(operadorGruaString))
                        {
                            string operadorGruaUpper = operadorGruaString.ToUpper();

                            command.Parameters.AddWithValue("@operadorGrua", operadorGruaString);
                        }
                    }
                    command.Parameters.AddWithValue("@abanderamiento", abanderamiento);
                    command.Parameters.AddWithValue("@arrastre", arrastre);
                    command.Parameters.AddWithValue("@salvamento", salvamento);
                    if (formData["tiempoManiobras"].ToString() != "")
                        command.Parameters.AddWithValue("@minutosManiobra", int.Parse(formData["tiempoManiobras"].ToString()));
                    else
                        command.Parameters.AddWithValue("@minutosManiobra", 0);
                    command.Parameters.AddWithValue("@idDeposito", iDep);
                    //command.Parameters.AddWithValue("@idSolicitud", iSo);

                    command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);
                    command.Parameters.AddWithValue("@actualizadoPor", 1);
                    command.Parameters.AddWithValue("@estatus", 1);
                    int idGrua;
                    if (int.TryParse(formData["idGrua"].ToString(), out idGrua))
                    {
                        command.Parameters.AddWithValue("@idGrua", idGrua);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@idGrua", DBNull.Value);
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



        public List<SeleccionGruaModel> BusquedaGruaTabla(int iDep)
        {
            List<SeleccionGruaModel> ListaSolicitudes = new List<SeleccionGruaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT ga.idAsignacion, ga.idGrua, ga.operadorGrua, ga.abanderamiento, ga.arrastre, ga.salvamento, 
                                                          ga.fechaArribo, ga.fechaInicio, ga.fechaFinal,ga.minutosManiobra,
                                                          g.noEconomico, g.idTipoGrua, g.placas, ctg.TipoGrua 
                                                          ,g.capacidad  capacidad
														  FROM gruasAsignadas AS ga 
                                                          LEFT JOIN gruas AS g ON ga.idGrua = g.idGrua 
                                                          LEFT JOIN catTipoGrua AS ctg ON g.idTipoGrua = ctg.IdTipoGrua  
                                                          WHERE ga.idDeposito = @idDeposito AND ga.estatus = 1", connection);



                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idDeposito", SqlDbType.Int)).Value = iDep;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            SeleccionGruaModel solicitud = new SeleccionGruaModel();
                            solicitud.idAsignacion = reader["idAsignacion"] != DBNull.Value ? Convert.ToInt32(reader["idAsignacion"]) : 0;
                            solicitud.idGrua = reader["idGrua"] != DBNull.Value ? Convert.ToInt32(reader["idGrua"]) : 0;
                            solicitud.EstadoAbanderamiento = reader["abanderamiento"] != DBNull.Value ? Convert.ToInt32(reader["abanderamiento"]) : 0;
                            solicitud.EstadoArrastre = reader["arrastre"] != DBNull.Value ? Convert.ToInt32(reader["arrastre"]) : 0;
                            solicitud.EstadoSalvamento = reader["salvamento"] != DBNull.Value ? Convert.ToInt32(reader["salvamento"]) : 0;
                            solicitud.fechaArribo = reader["fechaArribo"] != DBNull.Value ? Convert.ToDateTime(reader["fechaArribo"]) : DateTime.MinValue;
                            solicitud.fechaInicio = reader["fechaInicio"] != DBNull.Value ? Convert.ToDateTime(reader["fechaInicio"]) : DateTime.MinValue;
                            solicitud.fechaFinal = reader["fechaFinal"] != DBNull.Value ? Convert.ToDateTime(reader["fechaFinal"]) : DateTime.MinValue;
                            solicitud.operadorGrua = reader["operadorGrua"].ToString();
                            solicitud.numeroEconomico = reader["noEconomico"].ToString();
                            solicitud.Placas = reader["placas"].ToString();
                            solicitud.TipoGrua = reader["TipoGrua"].ToString() + " ( " + Convert.ToDecimal(reader["capacidad"].ToString()).ToString("G29") + " t)";

                            ListaSolicitudes.Add(solicitud);

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
            return ListaSolicitudes;


        }
        public int AgregarObs(AsignacionGruaModel formData, int iDep)
        {
            int result = 0;

            // Validar si idVehiculo es igual a 0 o nulo
            if (formData.IdVehiculo != 0)
            {
                using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                {
                    try
                    {
                        connection.Open();

                        // Buscar la placa y la serie del vehículo utilizando su idVehiculo
                        string vehiculoQuery = "SELECT placas, serie,idMarcaVehiculo,idPersona,idSubmarca FROM vehiculos WHERE idVehiculo = @idVehiculo";

                        SqlCommand vehiculoCommand = new SqlCommand(vehiculoQuery, connection);
                        vehiculoCommand.Parameters.AddWithValue("@idVehiculo", formData.IdVehiculo);

                        SqlDataReader reader = vehiculoCommand.ExecuteReader();

                        string placa = "";
                        string serie = "";
                        int idMArca = 0;
                        int idsubmarca = 0;
                        int idPersona = 0;

                        if (reader.Read())
                        {
                            placa = reader["placas"].ToString();
                            serie = reader["serie"].ToString();
                            idMArca = (int)reader["idMarcaVehiculo"];
                            idPersona = reader["idPersona"] is DBNull ? 133 :(int)reader["idPersona"];
                            idsubmarca = (int)reader["idSubmarca"];
                        }

                        reader.Close();

                                      string query = "UPDATE depositos SET " +
                                    "observaciones = @observaciones, " +
                                    "idVehiculo = @idVehiculo, " +
                                    "placa = @placa, " +
                                    "serie = @serie, " +
                                    "estatusSolicitud = CASE when estatusSolicitud IN (1,2) then 3 ELSE estatusSolicitud END, " +
                                    "idMarca=@idMarca," +
                                    "idPropietario=@idPropietario, " +
                                    "fechaActualizacion=@fechaActualizacion, " +
                                    "idSubmarca=@idSubmarca " +
                                    " WHERE idDeposito = @idDeposito";

                    SqlCommand command = new SqlCommand(query, connection);
                        string observaciones = formData.observaciones?.ToUpper() ?? "";
                        command.Parameters.AddWithValue("@observaciones", observaciones); 
                        command.Parameters.AddWithValue("@idDeposito", iDep);
                    command.Parameters.AddWithValue("@idVehiculo", formData.IdVehiculo);
                    command.Parameters.AddWithValue("@placa", placa.ToUpper());
                    command.Parameters.AddWithValue("@idMarca", idMArca);
                    command.Parameters.AddWithValue("@idPropietario", idPersona);
                        command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);
                        command.Parameters.AddWithValue("@idSubmarca", idsubmarca);
                    command.Parameters.AddWithValue("@serie", serie.ToUpper());

                        result = command.ExecuteNonQuery();

                        //Crear historico vehiculo deposito
                        try
                        {
                            var bitacoraVehiculo = _vehiculosService.CrearHistoricoVehiculo(iDep, (int)formData.IdVehiculo, 3);
                        }
                        catch { }
                        InsertarDepositosPersonas(formData, iDep, idPersona);

                    }
                    catch (SqlException ex)
                    {
                        // Manejar la excepción aquí, si es necesario
                        return result;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            else
            {
                // Si idVehiculo es igual a 0 o nulo, solo actualiza la tabla depositos sin incluir los elementos idVehiculo, placa y serie
                using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                {
                    try
                    {
                        connection.Open();

                        string query = "UPDATE depositos SET " +
                                        "observaciones = @observaciones, " +
                                        "fechaActualizacion = @fechaActualizacion, " +
                                        "estatusSolicitud = 3 " +
                                        "WHERE idDeposito = @idDeposito";

                        SqlCommand command = new SqlCommand(query, connection);
                        string observaciones = formData.observaciones?.ToUpper() ?? "";
                        command.Parameters.AddWithValue("@observaciones", observaciones); 
                        command.Parameters.AddWithValue("@idDeposito", iDep);
                        command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

                        result = command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        // Manejar la excepción aquí, si es necesario
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
        private void InsertarDepositosPersonas(AsignacionGruaModel formData, int idDeposito, int idPersona)
        {
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("usp_InsertaDepositosPersonas", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetros
                    command.Parameters.Add(new SqlParameter("@TipoPersonaOrigen", SqlDbType.Int)).Value = 2;
                    command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = idPersona;
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
        public int InsertarInventario(string archivoInventario, int iDep, string numeroInventario, string nombre = "GenericFile.txt")
        {
            int result = 0;
            string strQuery = @"UPDATE depositos
               SET numeroInventario = @numeroInventario,
                   fechaActualizacion = @fechaActualizacion,
                   nombreArchivo = @Filename";

            // Agregar actualización de archivoInventario solo si no es null o vacío
            if (!string.IsNullOrEmpty(archivoInventario))
            {
                strQuery += ", archivoInventario = @inventario";
            }

            strQuery += " WHERE idDeposito = @idDeposito";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idDeposito", SqlDbType.Int)).Value = iDep;
                    command.Parameters.AddWithValue("@numeroInventario", numeroInventario == null ? DBNull.Value : (object)numeroInventario.ToUpper());
                    command.Parameters.Add(new SqlParameter("@Filename", SqlDbType.VarChar)).Value = nombre.ToUpper();
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;

                    // Agregar el parámetro de archivoInventario solo si no es null o vacío
                    if (!string.IsNullOrEmpty(archivoInventario))
                    {
                        command.Parameters.Add(new SqlParameter("@inventario", SqlDbType.VarChar)).Value = archivoInventario;
                    }

                    result = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // Manejar la excepción
                }
            }

            return result;
        }
        public SeleccionGruaModel ObtenerAsignacionPorId(int idAsignacion)
        {
            SeleccionGruaModel asignacion = new SeleccionGruaModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM gruasAsignadas WHERE idAsignacion = @idAsignacion", connection);
                    command.Parameters.Add(new SqlParameter("@idAsignacion", SqlDbType.Int)).Value = idAsignacion;
                    //command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = idOficina;

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            SeleccionGruaModel solicitud = new SeleccionGruaModel();
                            asignacion.idAsignacion = reader["idAsignacion"] != DBNull.Value ? Convert.ToInt32(reader["idAsignacion"]) : 0;
                            asignacion.idGrua = reader["idGrua"] != DBNull.Value ? Convert.ToInt32(reader["idGrua"]) : 0;
                            asignacion.EstadoAbanderamiento = reader["abanderamiento"] != DBNull.Value ? Convert.ToInt32(reader["abanderamiento"]) : 0;
                            asignacion.EstadoArrastre = reader["arrastre"] != DBNull.Value ? Convert.ToInt32(reader["arrastre"]) : 0;
                            asignacion.EstadoSalvamento = reader["salvamento"] != DBNull.Value ? Convert.ToInt32(reader["salvamento"]) : 0;
                            asignacion.fechaArribo = reader["fechaArribo"] != DBNull.Value ? Convert.ToDateTime(reader["fechaArribo"]) : DateTime.MinValue;
                            asignacion.fechaInicio = reader["fechaInicio"] != DBNull.Value ? Convert.ToDateTime(reader["fechaInicio"]) : DateTime.MinValue;
                            asignacion.fechaFinal = reader["fechaFinal"] != DBNull.Value ? Convert.ToDateTime(reader["fechaFinal"]) : DateTime.MinValue;

                            asignacion.horaArribo = reader["fechaArribo"] != DBNull.Value ? Convert.ToDateTime(reader["fechaArribo"]).TimeOfDay : TimeSpan.MinValue;
                            asignacion.horaInicio = reader["fechaInicio"] != DBNull.Value ? Convert.ToDateTime(reader["fechaInicio"]).TimeOfDay : TimeSpan.MinValue;
                            asignacion.horaTermino = reader["fechaFinal"] != DBNull.Value ? Convert.ToDateTime(reader["fechaFinal"]).TimeOfDay : TimeSpan.MinValue;

                            asignacion.operadorGrua = reader["operadorGrua"].ToString();
                            asignacion.numeroEconomico = reader["numeroEconomico"].ToString();
                            asignacion.Placas = reader["placas"].ToString();
                            asignacion.TipoGrua = reader["idTipoGrua"].ToString();




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

            return asignacion;
        }

        public int EliminarGrua(int idAsignacion)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE gruasAsignadas SET estatus = 0, fechaActualizacion = @fechaActualizacion WHERE idAsignacion = @idAsignacion";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idAsignacion", idAsignacion);
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

        public int EditarDatosGrua(IFormCollection formData, int abanderamiento, int arrastre, int salvamento)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE gruasAsignadas " +
                                   "SET fechaArribo = @fechaArribo, " +
                                   "fechaInicio = @fechaInicio, " +
                                   "fechaFinal = @fechaFinal, " +
                                   "operadorGrua = @operadorGrua, " +
                                   "abanderamiento = @abanderamiento, " +
                                   "arrastre = @arrastre, " +
                                   "salvamento = @salvamento, " +
                                   "idGrua = @idGrua, " +
                                   "minutosManiobra = @minutosManiobra, " +
                                   "actualizadoPor = @actualizadoPor, " +
                                   "fechaActualizacion = @fechaActualizacion, " +
                                   "estatus = @estatus " +
                                   "WHERE idAsignacion = @idAsignacion";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@fechaArribo", DateTime.Parse(formData["fechaArribo"].ToString()));
                    command.Parameters.AddWithValue("@fechaInicio", DateTime.Parse(formData["fechaInicio"].ToString()));
                    command.Parameters.AddWithValue("@fechaFinal", DateTime.Parse(formData["fechaFinal"].ToString()));
                    if (formData.ContainsKey("operadorGrua"))
                    {
                        var operadorGruaValues = formData["operadorGrua"];

                        string operadorGruaString = operadorGruaValues.FirstOrDefault();

                        if (!string.IsNullOrEmpty(operadorGruaString))
                        {
                            string operadorGruaUpper = operadorGruaString.ToUpper();

                            command.Parameters.AddWithValue("@operadorGrua", operadorGruaString);
                        }
                    }
                    command.Parameters.AddWithValue("@abanderamiento", abanderamiento);
                    command.Parameters.AddWithValue("@arrastre", arrastre);
                    command.Parameters.AddWithValue("@salvamento", salvamento);
                    command.Parameters.AddWithValue("@idGrua", int.Parse(formData["idGrua"].ToString()));
                    command.Parameters.AddWithValue("@minutosManiobra", int.Parse(formData["tiempoManiobras"].ToString()));
                    command.Parameters.AddWithValue("@idAsignacion", int.Parse(formData["idAsignacion"].ToString()));
                    command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);
                    command.Parameters.AddWithValue("@actualizadoPor", 1);
                    command.Parameters.AddWithValue("@estatus", 1);
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
        public AsignacionGruaModel DatosInfraccionAsociada(string folioSolicitud, int idDependencia)
        {
            AsignacionGruaModel solicitud = new AsignacionGruaModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();

                    // Consulta para buscar si el folio ya existe en la tabla depositos
                    SqlCommand searchCommand = new SqlCommand(@"SELECT	ISNULL(A.idSolicitud,0) idSolicitud, 
		                                                                ISNULL(A.idDeposito,0) idDeposito ,
		                                                                ISNULL(A.folio,'') folio ,
		                                                                A.observaciones,
		                                                                A.numeroInventario,
		                                                                A.inventario,
                                                                        A.idInfraccion,
                                                                        B.idPropietarioGrua,
                                                                        A.idInfraccion,
		                                                                inf.idVehiculo,
		                                                                inf.idPersona,
		                                                                inf.folioInfraccion,
		                                                                inf.fechaInfraccion,
                                                                        v.placas,v.serie,
		                                                                v.idMarcaVehiculo,
		                                                                v.idSubmarca,
		                                                                v.modelo,
		                                                                v.idPersona,
		                                                                v.tarjeta,
                                                                        p.nombre,
		                                                                p.apellidoPaterno,
		                                                                p.apellidoMaterno,
		                                                                p.CURP,
		                                                                p.RFC,
                                                                        cmv.marcaVehiculo,
		                                                                csv.nombreSubmarca,
		                                                                col.color,
		                                                                inf.folioInfraccion FolioVinculado
                                                                FROM depositos			A 
                                                                LEFT JOIN solicitudes	B	ON A.idSolicitud = B.idSolicitud
                                                                LEFT JOIN infracciones	inf ON B.idInfraccion = inf.idInfraccion
                
                                                                LEFT JOIN opeInfraccionesVehiculos v ON v.idOperacion = 
											                    (
											                       SELECT MAX(idOperacion) 
											                       FROM opeInfraccionesVehiculos z 
											                       WHERE z.idVehiculo = inf.idVehiculo and z.idInfraccion = inf.idInfraccion
											                    )

                                                                LEFT JOIN personas		p	ON inf.idPersona = p.idPersona
                                                                LEFT JOIN catMarcasVehiculos as cmv ON v.idMarcaVehiculo = cmv.idMarcaVehiculo
                                                                LEFT JOIN catSubmarcasVehiculos as csv ON v.idSubmarca = csv.idSubmarca
                                                                LEFT JOIN catColores  col ON v.idColor = col.idColor
                                                                WHERE A.folio = @FolioSolicitud AND B.BanderaTransito = @dependencia", connection);
                    searchCommand.Parameters.Add(new SqlParameter("@FolioSolicitud", SqlDbType.NVarChar)).Value = folioSolicitud;
                    searchCommand.Parameters.Add(new SqlParameter("@dependencia", SqlDbType.Int)).Value = idDependencia;

                    // Ejecutar la consulta de búsqueda
                    using (SqlDataReader searchReader = searchCommand.ExecuteReader())
                    {
                        // ...
                        if (searchReader.Read())
                        {
                            solicitud.idSolicitud = int.Parse(searchReader["idSolicitud"].ToString());
                            solicitud.FolioSolicitud = searchReader["folio"].ToString();
                            solicitud.observaciones = searchReader["observaciones"].ToString();
                            solicitud.numeroInventario = searchReader["numeroInventario"].ToString();
                            object idInfraccionValue = searchReader["idInfraccion"];
                            if (idInfraccionValue != DBNull.Value)
                            {
                                solicitud.idInfraccion = Convert.ToInt32(idInfraccionValue);
                            }
                            else
                            {
                                solicitud.idInfraccion = default(int);
                            }

                            if (!searchReader.IsDBNull(searchReader.GetOrdinal("idPropietarioGrua")))
                            {
                                solicitud.idPropietarioGrua = searchReader.GetInt32(searchReader.GetOrdinal("idPropietarioGrua"));
                            }
                            else
                            {
                                solicitud.idPropietarioGrua = 0;
                            }
                           // solicitud.IdVehiculo = (int)(searchReader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(searchReader["idVehiculo"].ToString()));
                            object idVehiculoValue = searchReader["idVehiculo"];
                            if (idVehiculoValue != DBNull.Value)
                            {
                                solicitud.IdVehiculo = Convert.ToInt32(idVehiculoValue);
                            }
                            else
                            {
                                solicitud.IdVehiculo = default(int);
                            }
                            solicitud.folioInfraccion = searchReader["FolioVinculado"].ToString();
                            solicitud.fechaInfraccion = searchReader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(searchReader["fechaInfraccion"].ToString());
                            solicitud.IdMarcaVehiculo = Convert.IsDBNull(searchReader["IdMarcaVehiculo"]) ? 0 : Convert.ToInt32(searchReader["IdMarcaVehiculo"]);
                            solicitud.IdSubmarca = Convert.IsDBNull(searchReader["IdSubmarca"]) ? 0 : Convert.ToInt32(searchReader["IdSubmarca"]);
                            solicitud.Marca = searchReader["marcaVehiculo"].ToString();
                            solicitud.Submarca = searchReader["nombreSubmarca"].ToString();
                            solicitud.Modelo = searchReader["Modelo"].ToString();
                            solicitud.Color = searchReader["color"].ToString();
                            solicitud.Placa = searchReader["placas"].ToString();
                            solicitud.Serie = searchReader["serie"].ToString();
                            solicitud.Tarjeta = searchReader["tarjeta"].ToString();
                            solicitud.Propietario = $"{searchReader["nombre"]} {searchReader["apellidoPaterno"]} {searchReader["apellidoMaterno"]}";
                            solicitud.CURP = searchReader["CURP"].ToString();
                            solicitud.RFC = searchReader["RFC"].ToString();

                            string filePath = searchReader["inventario"].ToString();

                            if (!string.IsNullOrEmpty(filePath))
                            {
                                var file = new FormFile(Stream.Null, 0, 0, "MyFile", Path.GetFileName(filePath))
                                {
                                    Headers = new HeaderDictionary(),
                                    ContentType = "application/octet-stream"
                                };

                                solicitud.MyFile = file;
                            }
                            else
                            {
                                solicitud.MyFile = null;
                            }

                            solicitud.observaciones = searchReader["observaciones"].ToString();
                            solicitud.IdDeposito = int.Parse(searchReader["idDeposito"].ToString());
                            return solicitud;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejar las excepciones adecuadamente
                }
                finally
                {
                    connection.Close();
                }
            }

            return solicitud;
        }

        public AsignacionGruaModel DatosVehiculoAsociado(string folioSolicitud, int idDependencia)
        {
            AsignacionGruaModel solicitud = new AsignacionGruaModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();

                    SqlCommand searchCommand = new SqlCommand(@"SELECT	ISNULL(A.idSolicitud,0) idSolicitud, 
		                                                                ISNULL(A.idDeposito,0) idDeposito ,
		                                                                ISNULL(A.folio,'') folio ,
		                                                                A.observaciones,
		                                                                A.numeroInventario,
		                                                                A.inventario,
		                                                                A.archivoInventario,
                                                                        B.idPropietarioGrua,
		                                                                A.idInfraccion, 
		                                                                A.idVehiculo,
		                                                                v.idPersona,
                                                                        v.placas,v.serie,
		                                                                v.idMarcaVehiculo,
		                                                                v.idSubmarca,
		                                                                v.modelo,
		                                                                v.idPersona,
		                                                                v.tarjeta,
                                                                        p.nombre,
		                                                                p.apellidoPaterno,
		                                                                p.apellidoMaterno,
		                                                                p.CURP,
		                                                                p.RFC,
                                                                        cmv.marcaVehiculo,
		                                                                csv.nombreSubmarca,
		                                                                col.color
                                                                FROM depositos			A 
                                                                LEFT JOIN solicitudes	B	ON A.idSolicitud = B.idSolicitud
    
                                                                LEFT JOIN opeDepositosVehiculos v ON v.idOperacion = 
											                    (
											                       SELECT MAX(idOperacion) 
											                       FROM opeDepositosVehiculos z 
											                       WHERE z.idVehiculo = A.idVehiculo and z.idDeposito = A.idDeposito
											                    )

                                                                LEFT JOIN opeDepositosPersonas p ON p.idOperacion = 
                                                                (
                                                                SELECT MAX(idOperacion) 
                                                                FROM opeDepositosPersonas z 
                                                                WHERE z.idPersona = v.idPersona and z.idDeposito = A.idDeposito
                                                                )

                                                                LEFT JOIN catMarcasVehiculos as cmv ON v.idMarcaVehiculo = cmv.idMarcaVehiculo
                                                                LEFT JOIN catSubmarcasVehiculos as csv ON v.idSubmarca = csv.idSubmarca
                                                                LEFT JOIN catColores  col ON v.idColor = col.idColor
                                                                WHERE A.folio = @FolioSolicitud AND B.BanderaTransito = @dependencia", connection);
                    searchCommand.Parameters.Add(new SqlParameter("@FolioSolicitud", SqlDbType.NVarChar)).Value = folioSolicitud;
                    searchCommand.Parameters.Add(new SqlParameter("@dependencia", SqlDbType.Int)).Value = idDependencia;

                    // Ejecutar la consulta de búsqueda
                    using (SqlDataReader searchReader = searchCommand.ExecuteReader())
                    {
                        // ...
                        if (searchReader.Read())
                        {
                            solicitud.idSolicitud = int.Parse(searchReader["idSolicitud"].ToString());
                            solicitud.FolioSolicitud = searchReader["folio"].ToString();
                            solicitud.observaciones = searchReader["observaciones"].ToString();

                                solicitud.idInfraccion = Convert.IsDBNull(searchReader["idInfraccion"]) ? 0 : Convert.ToInt32(searchReader["idInfraccion"]);
                                

                            if (!searchReader.IsDBNull(searchReader.GetOrdinal("idPropietarioGrua")))
                            {
                                solicitud.idPropietarioGrua = searchReader.GetInt32(searchReader.GetOrdinal("idPropietarioGrua"));
                            }
                            else
                            {
                                solicitud.idPropietarioGrua = 0;
                            }
							object idVehiculoObj = searchReader["idVehiculo"];
							if (idVehiculoObj != DBNull.Value && !Convert.IsDBNull(idVehiculoObj))
							{
								solicitud.IdVehiculo = Convert.ToInt32(idVehiculoObj);                                
							}
							else
							{
								solicitud.IdVehiculo = 0;
							}

							solicitud.IdMarcaVehiculo = Convert.IsDBNull(searchReader["IdMarcaVehiculo"]) ? 0 : Convert.ToInt32(searchReader["IdMarcaVehiculo"]);
                            solicitud.IdSubmarca = Convert.IsDBNull(searchReader["IdSubmarca"]) ? 0 : Convert.ToInt32(searchReader["IdSubmarca"]);
                            solicitud.Marca = searchReader["marcaVehiculo"].ToString();
                            solicitud.Submarca = searchReader["nombreSubmarca"].ToString();
                            solicitud.Modelo = searchReader["Modelo"].ToString();
                            solicitud.Color = searchReader["color"].ToString();
                            solicitud.Placa = searchReader["placas"].ToString();
                            solicitud.Serie = searchReader["serie"].ToString();
                            solicitud.Tarjeta = searchReader["tarjeta"].ToString();
                            solicitud.Propietario = $"{searchReader["nombre"]} {searchReader["apellidoPaterno"]} {searchReader["apellidoMaterno"]}";
                            solicitud.CURP = searchReader["CURP"].ToString();
                            solicitud.RFC = searchReader["RFC"].ToString();
                            solicitud.numeroInventario = searchReader["numeroInventario"].ToString();
                            string filePath = searchReader["archivoInventario"].ToString();

                            if (!string.IsNullOrEmpty(filePath))
                            {
                                var file = new FormFile(Stream.Null, 0, 0, "MyFile", Path.GetFileName(filePath))
                                {
                                    Headers = new HeaderDictionary(),
                                    ContentType = "application/octet-stream"
                                };

                                solicitud.MyFile = file;
                            }
                            else
                            {
                                solicitud.MyFile = null;
                            }

                            solicitud.observaciones = searchReader["observaciones"].ToString();
                            solicitud.IdDeposito = int.Parse(searchReader["idDeposito"].ToString());
                            return solicitud;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejar las excepciones adecuadamente
                }
                finally
                {
                    connection.Close();
                }
            }

            return solicitud;
        }
    }
}







