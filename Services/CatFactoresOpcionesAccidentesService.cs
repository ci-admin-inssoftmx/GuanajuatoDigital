using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatFactoresOpcionesAccidentesService : ICatFactoresOpcionesAccidentesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatFactoresOpcionesAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<CatFactoresOpcionesAccidentesModel> ObtenerOpcionesPorFactor(int factorDDValue,int corp)
        {
            //
            List<CatFactoresOpcionesAccidentesModel> ListaOpciones = new List<CatFactoresOpcionesAccidentesModel>();
			var corporation = corp < 2 ? 1 : corp;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT op.*, f.*, e.estatus 
                                                          FROM catFactoresOpcionesAccidentes AS op 
                                                          INNER JOIN catFactoresAccidentes AS f ON op.IdFactorAccidente = f.IdFactorAccidente 
                                                          INNER JOIN estatus AS e ON op.estatus = e.estatus 
                                                          WHERE op.IdFactorAccidente = @IdFactor and op.estatus = 1 and op.transito = @corp 
                                                           ORDER BY FactorOpcionAccidente ASC;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@IdFactor", SqlDbType.Int)).Value = (object)factorDDValue ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporation ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatFactoresOpcionesAccidentesModel opcion = new CatFactoresOpcionesAccidentesModel();
                            opcion.IdFactorOpcionAccidente = Convert.ToInt32(reader["IdFactorOpcionAccidente"].ToString());
                            opcion.IdFactorAccidente = Convert.ToInt32(reader["IdFactorAccidente"].ToString());
                            opcion.FactorOpcionAccidente = reader["FactorOpcionAccidente"].ToString();
                            opcion.estatusDesc = reader["estatus"].ToString();
                            opcion.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            opcion.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                           // opcion.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaOpciones.Add(opcion);

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
            return ListaOpciones;


        }

        public List<CatFactoresOpcionesAccidentesModel> ObtenerOpcionesFactorAccidente(int corp)
        {
            //
            List<CatFactoresOpcionesAccidentesModel> ListaOpciones = new List<CatFactoresOpcionesAccidentesModel>();
			var corporation = corp < 2 ? 1 : corp;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT op.*,e.estatusDesc,fa.factorAccidente,fa.idFactorAccidente
                                                            FROM catFactoresOpcionesAccidentes op
                                                            LEFT JOIN estatus e ON e.estatus = op.estatus
                                                            LEFT JOIN catFactoresAccidentes fa ON fa.idFactorAccidente = op.idFactorAccidente
                                                            where op.transito = @corp
                                                          ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatFactoresOpcionesAccidentesModel opcion = new CatFactoresOpcionesAccidentesModel();
                            opcion.IdFactorOpcionAccidente = Convert.ToInt32(reader["IdFactorOpcionAccidente"].ToString());
                            opcion.IdFactorAccidente = Convert.ToInt32(reader["IdFactorAccidente"].ToString());
                            opcion.Corp = Convert.ToInt32(reader["transito"].ToString());
                            opcion.FactorOpcionAccidente = reader["FactorOpcionAccidente"].ToString();
                            opcion.FactorAccidente = reader["factorAccidente"].ToString();
                            opcion.estatusDesc = reader["estatusDesc"].ToString();
                            opcion.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            // opcion.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaOpciones.Add(opcion);

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
            return ListaOpciones;


        }

        public CatFactoresOpcionesAccidentesModel ObtenerOpcionesParaEditar(int IdFactoropcionAccidente,int corp)
        {

            CatFactoresOpcionesAccidentesModel ListaOpciones = new CatFactoresOpcionesAccidentesModel();
			var corporation = corp < 2 ? 1 : corp;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT op.*, f.*, e.estatus
                        FROM catFactoresOpcionesAccidentes AS op
                        LEFT JOIN catFactoresAccidentes AS f ON op.IdFactorAccidente = f.IdFactorAccidente
                        LEFT JOIN estatus AS e ON op.estatus = e.estatus
                        WHERE op.IdFactorOpcionAccidente = @IdFactor and op.transito=@corp;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@IdFactor", SqlDbType.Int)).Value = (object)IdFactoropcionAccidente ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporation ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ListaOpciones.IdFactorOpcionAccidente = Convert.ToInt32(reader["IdFactorOpcionAccidente"].ToString());
                            ListaOpciones.IdFactorAccidente = Convert.ToInt32(reader["IdFactorAccidente"].ToString());
                            ListaOpciones.FactorOpcionAccidente = reader["FactorOpcionAccidente"].ToString();
                            ListaOpciones.FactorAccidente = reader["FactorOpcionAccidente"].ToString();
                            ListaOpciones.estatusDesc = reader["estatus"].ToString();
                            ListaOpciones.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            ListaOpciones.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            // opcion.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());

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
            return ListaOpciones;


        }
        public int CrearOpcionFactor(CatFactoresOpcionesAccidentesModel model,int corp)
        {
            int result = 0;
			var corporation = corp < 2 ? 1 : corp;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catFactoresOpcionesAccidentes(factorOpcionAccidente,idFactorAccidente,estatus,fechaActualizacion,actualizadoPor,transito) values(@factorOpcionAccidente,@idFactorAccidente,@estatus,@fechaActualizacion,@actualizadoPor,@corp)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@factorOpcionAccidente", SqlDbType.VarChar)).Value = model.FactorOpcionAccidente;
                    sqlCommand.Parameters.Add(new SqlParameter("@idFactorAccidente", SqlDbType.Int)).Value = model.IdFactorAccidente;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    sqlCommand.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = corporation;

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
        public int EditarOpcionFactor(CatFactoresOpcionesAccidentesModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catFactoresOpcionesAccidentes set factorOpcionAccidente=@factorOpcionAccidente,idFactorAccidente=@idFactorAccidente,estatus = @estatus,fechaActualizacion = @fechaActualizacion, actualizadoPor =@actualizadoPor where idFactorOpcionAccidente=@idFactorOpcion",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idFactorOpcion", SqlDbType.Int)).Value = model.IdFactorOpcionAccidente;
                    sqlCommand.Parameters.Add(new SqlParameter("@idFactorAccidente", SqlDbType.Int)).Value = model.IdFactorAccidente;
                    sqlCommand.Parameters.Add(new SqlParameter("@factorOpcionAccidente", SqlDbType.NVarChar)).Value = model.FactorOpcionAccidente;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.VarChar)).Value = model.Estatus;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    sqlCommand.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
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
        public List<CatFactoresOpcionesAccidentesModel> ObtenerOpcionesPorCorporacion(int corp)
        {
            //
            List<CatFactoresOpcionesAccidentesModel> ListaOpciones = new List<CatFactoresOpcionesAccidentesModel>();
            var corporation = corp < 2 ? 1 : corp;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT op.*, e.estatus 
                                                          FROM catFactoresOpcionesAccidentes AS op 
                                                          INNER JOIN estatus AS e ON op.estatus = e.estatus 
                                                          WHERE op.estatus = 1 and op.transito = @corp 
                                                           ORDER BY FactorOpcionAccidente ASC;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporation ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatFactoresOpcionesAccidentesModel opcion = new CatFactoresOpcionesAccidentesModel();
                            opcion.IdFactorOpcionAccidente = Convert.ToInt32(reader["IdFactorOpcionAccidente"].ToString());
                            opcion.IdFactorAccidente = Convert.ToInt32(reader["IdFactorAccidente"].ToString());
                            opcion.FactorOpcionAccidente = reader["FactorOpcionAccidente"].ToString();
                            opcion.estatusDesc = reader["estatus"].ToString();
                            opcion.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            opcion.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            // opcion.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaOpciones.Add(opcion);

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
            return ListaOpciones;


        }
    }

}


