using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace GuanajuatoAdminUsuarios.Services
{
    public class InfraccionesCortesiaService : IInfraccionesCortesiaService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
		private readonly IInfraccionesService _infraccionesService;


		public InfraccionesCortesiaService(ISqlClientConnectionBD sqlClientConnectionBD, IInfraccionesService infraccionesService)

        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
            _infraccionesService = infraccionesService;

		}
       public List<InfraccionesCortesiaModel> ObtenerTodasInfraccionesCortesia(int idOficina, int corp)
        {
            List<InfraccionesCortesiaModel> ListaInfracciones = new List<InfraccionesCortesiaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@$"SELECT 
                                                                i.folioInfraccion,
                                                                i.idInfraccion,
                                                                CONVERT(varchar, i.fechaInfraccion, 103) AS fechaInfraccion, 
                                                                CONVERT(varchar, i.fechaVencimiento, 103) AS fechaVencimiento, 
                                                                CONCAT(pI.nombre, ' ', pI.apellidoPaterno, ' ', pI.apellidoMaterno) AS Conductor,
                                                                i.placasVehiculo, 
                                                                i.idEstatusInfraccion,
                                                                v.serie,
                                                                CONCAT(pV.nombre, ' ', pV.apellidoPaterno, ' ', pV.apellidoMaterno) AS Propietario,
                                                                e.estatusInfraccion,
                                                                ISNULL(modInf.movimientos, '') AS movimientos
                                                            FROM 
                                                                infracciones AS i 
                                                                LEFT JOIN opeInfraccionesVehiculos v 
                                                                    ON v.idOperacion = (
                                                                        SELECT MAX(idOperacion) 
                                                                        FROM opeInfraccionesVehiculos z 
                                                                        WHERE z.idVehiculo = i.idVehiculo 
                                                                          AND z.idInfraccion = i.idInfraccion
                                                                    )
                                                                LEFT JOIN catEstatusInfraccion AS e 
                                                                    ON i.idEstatusInfraccion = e.idEstatusInfraccion 
                                                                LEFT JOIN opeInfraccionesPersonas AS pI 
                                                                    ON pI.IdPersona = i.IdPersonaConductor 
                                                                    AND pI.idInfraccion = i.idInfraccion 
                                                                    AND TipoPersonaOrigen = 1
                                                                LEFT JOIN personas AS pV 
                                                                    ON pV.IdPersona = v.idPersona 
                                                                LEFT JOIN catDelegaciones cde 
                                                                    ON cde.idDelegacion = i.idDelegacion   
                                                                LEFT JOIN (
                                                                    SELECT 
                                                                        idInfraccion,
                                                                        STRING_AGG(CONCAT(CONVERT(varchar, fechaVencimiento, 103), ' ', ISNULL(observaciones, '')), '; ') WITHIN GROUP (ORDER BY idModificacion) AS movimientos
                                                                    FROM 
                                                                        modificacionInfraccionesCortesia 
                                                                    GROUP BY 
                                                                        idInfraccion
                                                                ) AS modInf 
                                                                    ON modInf.idInfraccion = i.idInfraccion
                                                            WHERE 
                                                                i.infraccionCortesia = 2 
                                                                AND i.idDelegacion = @idOficina
                                                                AND i.transito = @corporacion
                                                                AND i.idEstatusInfraccion IN (1, 2, 7) 
                                                                AND i.estatus = 1
                                                            ORDER BY 
                                                                i.idInfraccion DESC;
                                                               ", connection);

                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = idOficina;
                    command.Parameters.Add(new SqlParameter("@corporacion", SqlDbType.Int)).Value = corp;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            InfraccionesCortesiaModel infraccion = new InfraccionesCortesiaModel();
                            infraccion.IdInfraccion = Convert.ToInt32(reader["idInfraccion"].ToString());
                            infraccion.IdEstatusInfraccion = Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
                            infraccion.folioInfraccion = reader["folioInfraccion"].ToString();
                            infraccion.Placas = reader["placasVehiculo"].ToString();
                            infraccion.FechaInfraccion = reader["fechaInfraccion"].ToString();
                            infraccion.FechaVencimientoStr = reader["fechaVencimiento"].ToString();
                            infraccion.Propietario = reader["Propietario"] is DBNull ? string.Empty : reader["Propietario"].ToString();
                            infraccion.Serie = reader["Serie"] is DBNull ? string.Empty : reader["Serie"].ToString();
                            infraccion.Conductor = reader["Conductor"] is DBNull ? string.Empty : reader["Conductor"].ToString();
                            infraccion.Estatus = reader["estatusInfraccion"].ToString();
                            infraccion.ObervacionesVencimiento= reader["movimientos"] is DBNull ? string.Empty : reader["movimientos"].ToString();

                            DateTime fechaInfraccion = DateTime.ParseExact(infraccion.FechaInfraccion, "dd/MM/yyyy", CultureInfo.InvariantCulture);
							DateTime fechaVencimiento = DateTime.ParseExact(infraccion.FechaVencimientoStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            infraccion.FechaVencimiento = fechaVencimiento;

                            // Llamada al método ContarDiasHabiles
                            infraccion.DiferenciaDias = ContarDiasHabiles(fechaInfraccion, fechaVencimiento, idOficina);

							ListaInfracciones.Add(infraccion);

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
            return ListaInfracciones;
        }

        public int ContarDiasHabiles(DateTime fechaInicio, DateTime fechaFin, int idOficina)
        {
            if (fechaInicio > fechaFin)
            {
                throw new ArgumentException("La fecha de inicio no puede ser posterior a la fecha final.");
            }

            int contadorDiasHabiles = 0;

            // Empezar desde el día siguiente a la fecha de inicio
            DateTime fecha = fechaInicio.AddDays(1);

            while (fecha <= fechaFin)
            {
                // Excluir sabados y los domingos
                if (fecha.DayOfWeek.ToString() == "Saturday" || fecha.DayOfWeek.ToString() == "Sabado" || fecha.DayOfWeek.ToString() == "Sábado" || fecha.DayOfWeek.ToString() == "Sunday" || fecha.DayOfWeek.ToString() == "Domingo")
                {
                    fecha = fecha.AddDays(1);
                    continue;
                }

                // Excluir los días festivos
                if (_infraccionesService.GetDiaFestivo(idOficina, fecha) != 0)
                {
                    fecha = fecha.AddDays(1);
                    continue;
                }

                contadorDiasHabiles++;

                fecha = fecha.AddDays(1);
            }

            return contadorDiasHabiles;
        }


        public int GuardarNuevaFecha(InfraccionesCortesiaModel model)
        {
            var resultado = 0; 
            var connectionString = _sqlClientConnectionBD.GetConnection();

            var strQueryExistencia = @"SELECT COUNT(*) FROM modificacionInfraccionesCortesia WHERE idInfraccion = @idInfraccion";

            // Consultas SQL para insertar nuevos registros
            var strQueryInsertPrimer = @"INSERT INTO modificacionInfraccionesCortesia (idInfraccion, fechaVencimiento, observaciones, estatus, fechaActualizacion) 
                                 VALUES (@idInfraccion, @fechaVencimiento, @observaciones, @estatus, @fechaActualizacion)";

            var strQueryInsertSegundo = @"INSERT INTO modificacionInfraccionesCortesia (idInfraccion, fechaVencimiento, observaciones, estatus, fechaActualizacion) 
                                  VALUES (@idInfraccion, @nuevaFechaVencimiento, @observaciones, @estatus, @fechaActualizacion)";

            var strQueryUpdateInfracciones = @"UPDATE infracciones
                                       SET fechaVencimiento = @nuevaFechaVencimiento
                                       WHERE idInfraccion = @idInfraccion";

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Verificar si el registro ya existe
                    using (var checkCommand = new SqlCommand(strQueryExistencia, connection))
                    {
                        checkCommand.CommandType = CommandType.Text;
                        checkCommand.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = model.IdInfraccion;

                        var count = (int)checkCommand.ExecuteScalar();

                        using (var command = new SqlCommand())
                        {
                            command.Connection = connection;
                            command.CommandType = CommandType.Text;

                            if (count == 0)
                            {
                                // Si no existe, insertar primero con fechaVigencia
                                command.CommandText = strQueryInsertPrimer;
                                command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = model.IdInfraccion;
                                command.Parameters.Add(new SqlParameter("@fechaVencimiento", SqlDbType.DateTime)).Value = model.FechaVencimiento;
                                command.Parameters.Add(new SqlParameter("@observaciones", SqlDbType.NVarChar)).Value = "Primera fecha de vencimiento";
                                command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                                command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;

                                resultado += command.ExecuteNonQuery(); 

                                // Luego insertar con nuevaFechaVigencia
                                command.CommandText = strQueryInsertSegundo;
                                command.Parameters.Clear(); 
                                command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = model.IdInfraccion;
                                command.Parameters.Add(new SqlParameter("@nuevaFechaVencimiento", SqlDbType.DateTime)).Value = model.NuevaFechaVencimiento;
                                command.Parameters.Add(new SqlParameter("@observaciones", SqlDbType.NVarChar)).Value = model.ObervacionesVencimiento ?? (object)DBNull.Value;
                                command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                                command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;

                                resultado += command.ExecuteNonQuery(); 
                            }
                            else
                            {
                                // Si ya existe, solo insertar con nuevaFechaVencimiento
                                command.CommandText = strQueryInsertSegundo;
                                command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = model.IdInfraccion;
                                command.Parameters.Add(new SqlParameter("@nuevaFechaVencimiento", SqlDbType.DateTime)).Value = model.NuevaFechaVencimiento;
                                command.Parameters.Add(new SqlParameter("@observaciones", SqlDbType.NVarChar)).Value = model.ObervacionesVencimiento ?? (object)DBNull.Value;
                                command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                                command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;

                                resultado += command.ExecuteNonQuery(); 
                            }
                        }
                    }

                    using (var updateCommand = new SqlCommand(strQueryUpdateInfracciones, connection))
                    {
                        updateCommand.CommandType = CommandType.Text;
                        updateCommand.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = model.IdInfraccion;
                        updateCommand.Parameters.Add(new SqlParameter("@nuevaFechaVencimiento", SqlDbType.DateTime)).Value = model.NuevaFechaVencimiento;

                        resultado += updateCommand.ExecuteNonQuery(); 
                    }
                }
                catch (SqlException ex)
                {
                    // Manejar la excepción, por ejemplo, guardarla en un log
                    // LogException(ex);
                }
                finally
                {
                    connection.Close();
                }
            }

            return resultado;
        }

        public int ModificarEstatusCortesia(int idInfraccion)
        {
            int result = 0;
            string strQuery = @"UPDATE infracciones
                                SET infraccionCortesia = @infraccionCortesia,
                                    fechaActualizacion = @fechaActualizacion,
                                    actualizadoPor = @actualizadoPor                             
                                WHERE idInfraccion = @idInfraccion";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idInfraccion", SqlDbType.Int)).Value = idInfraccion;
                    command.Parameters.Add(new SqlParameter("@infraccionCortesia", SqlDbType.Int)).Value = 4;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
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



    }
}

