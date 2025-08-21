using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatCarreterasService : ICatCarreterasService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatCarreterasService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatCarreterasModel> ObtenerCarreteras()
        {
            //
            List<CatCarreterasModel> ListaCarreteras = new List<CatCarreterasModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT c.*, e.estatusDesc,del.nombreOficina, ISNULL(D.transito,0) transito   
                                                        FROM catCarreteras AS c 
                                                        INNER JOIN estatus AS e ON c.estatus = e.estatus 
                                                        INNER JOIN catDelegacionesOficinasTransporte AS del ON c.idOficinaTransporte = del.idOficinaTransporte  
                                                        INNER JOIN catDelegaciones d ON del.idOficinaTransporte = d.idDelegacion   
                                                        ORDER BY Carretera ASC;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {

                        //command.Parameters.Add(new SqlParameter("@FechaIngreso", SqlDbType.DateTime)).Value = model.FechaIngreso == DateTime.MinValue ? new DateTime(1800, 01, 01) : (object)model.FechaIngreso;
                        while (reader.Read())
                        {
                            CatCarreterasModel carretera = new CatCarreterasModel();
                            carretera.IdCarretera = reader["IdCarretera"] != DBNull.Value ? Convert.ToInt32(reader["IdCarretera"]) : 0;
                            carretera.idOficinaTransporte = reader["idOficinaTransporte"] != DBNull.Value ? Convert.ToInt32(reader["idOficinaTransporte"]) : 0;
                            carretera.Carretera = reader["Carretera"] != DBNull.Value ? reader["Carretera"].ToString() : string.Empty;
                            carretera.nombreOficina = reader["nombreOficina"] != DBNull.Value ? reader["nombreOficina"].ToString() : string.Empty;
                            carretera.estatusDesc = reader["estatusDesc"] != DBNull.Value ? reader["estatusDesc"].ToString() : string.Empty;
                            carretera.FechaActualizacion = reader["FechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaActualizacion"]) : DateTime.MinValue;
                            carretera.Estatus = reader["estatus"] != DBNull.Value ? Convert.ToInt32(reader["estatus"]) : 0;
                            carretera.ActualizadoPor = reader["ActualizadoPor"] != DBNull.Value ? Convert.ToInt32(reader["ActualizadoPor"]) : 0;
                            carretera.Transito = (Convert.ToBoolean(reader["transito"])) ? 1 : 0;
                            ListaCarreteras.Add(carretera);

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
            return ListaCarreteras;


        }

        public List<CatCarreterasModel> ObtenerCarreteras(int corp)
        {
            //
            List<CatCarreterasModel> ListaCarreteras = new List<CatCarreterasModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT c.*, e.estatusDesc,del.nombreOficina, ISNULL(D.transito,0) transito   
                                                        FROM catCarreteras AS c 
                                                        INNER JOIN estatus AS e ON c.estatus = e.estatus 
                                                        INNER JOIN catDelegacionesOficinasTransporte AS del ON c.idOficinaTransporte = del.iddelegacion  
                                                        INNER JOIN catDelegaciones d ON del.iddelegacion = d.idDelegacion
                                                        where d.transito=@Corp
                                                        ORDER BY Carretera ASC;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("Corp", corp);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {

                        //command.Parameters.Add(new SqlParameter("@FechaIngreso", SqlDbType.DateTime)).Value = model.FechaIngreso == DateTime.MinValue ? new DateTime(1800, 01, 01) : (object)model.FechaIngreso;
                        while (reader.Read())
                        {
                            CatCarreterasModel carretera = new CatCarreterasModel();
                            carretera.IdCarretera = reader["IdCarretera"] != DBNull.Value ? Convert.ToInt32(reader["IdCarretera"]) : 0;
                            carretera.idOficinaTransporte = reader["idOficinaTransporte"] != DBNull.Value ? Convert.ToInt32(reader["idOficinaTransporte"]) : 0;
                            carretera.Carretera = reader["Carretera"] != DBNull.Value ? reader["Carretera"].ToString() : string.Empty;
                            carretera.nombreOficina = reader["nombreOficina"] != DBNull.Value ? reader["nombreOficina"].ToString() : string.Empty;
                            carretera.estatusDesc = reader["estatusDesc"] != DBNull.Value ? reader["estatusDesc"].ToString() : string.Empty;
                            carretera.FechaActualizacion = reader["FechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaActualizacion"]) : DateTime.MinValue;
                            carretera.Estatus = reader["estatus"] != DBNull.Value ? Convert.ToInt32(reader["estatus"]) : 0;
                            carretera.ActualizadoPor = reader["ActualizadoPor"] != DBNull.Value ? Convert.ToInt32(reader["ActualizadoPor"]) : 0;
                            carretera.Transito = (Convert.ToBoolean(reader["transito"])) ? 1 : 0;
                            ListaCarreteras.Add(carretera);

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
            return ListaCarreteras;


        }



        public CatCarreterasModel ObtenerCarreteraByID(int IdCarretera)
        {
            CatCarreterasModel carretera = new CatCarreterasModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT c.*, e.estatusdesc, del.nombreOficina FROM catCarreteras AS c INNER JOIN estatus AS e ON c.estatus = e.estatus INNER JOIN catDelegacionesOficinasTransporte AS del ON c.idOficinaTransporte = del.idOficinaTransporte WHERE c.idCarretera=@IdCarretera", connection);
                    command.Parameters.Add(new SqlParameter("@IdCarretera", SqlDbType.Int)).Value = IdCarretera;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            carretera.IdCarretera = Convert.ToInt32(reader["IdCarretera"].ToString());
                            carretera.idOficinaTransporte = Convert.ToInt32(reader["idOficinaTransporte"].ToString());
                            carretera.Estatus = Convert.ToInt32(reader["estatus"].ToString());

                            carretera.Carretera = reader["Carretera"].ToString();
                            carretera.nombreOficina = reader["nombreOficina"].ToString();

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

            return carretera;
        }
        public int CrearCarretera(CatCarreterasModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand(@"
                    Insert into catCarreteras (carretera,estatus,fechaActualizacion,actualizadoPor,idOficinaTransporte,transito) 
                                        values(@Carretera,@estatus,@fechaActualizacion,@actualizadoPor,@idOficinaTransporte,@corp)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@carretera", SqlDbType.VarChar)).Value = model.Carretera;
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficinaTransporte", SqlDbType.Int)).Value = model.idOficinaTransporte;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    sqlCommand.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = model.Corp;

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
        public int EditarCarretera(CatCarreterasModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catCarreteras set carretera=@carretera, estatus = @estatus,fechaActualizacion = @fechaActualizacion, actualizadoPor =@actualizadoPor, idOficinaTransporte =@idOficinaTransporte " +
                        " WHERE idCarretera = @idCarretera",
                        connection) ;
                    sqlCommand.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = model.IdCarretera;
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficinaTransporte", SqlDbType.Int)).Value = model.idOficinaTransporte;
                    sqlCommand.Parameters.Add(new SqlParameter("@carretera", SqlDbType.NVarChar)).Value = model.Carretera;
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

        public List<CatCarreterasModel> GetCarreterasPorDelegacion(int idOficina)
        {
            //
            List<CatCarreterasModel> ListaCarreteras = new List<CatCarreterasModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    //c.idOficinaTransporte = @idOficina OR c.idOficinaTransporte = 1 AND
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT c.idCarretera,c.idOficinaTransporte,UPPER(c.carretera) AS carretera,
                                                        c.estatus,c.FechaActualizacion,c.ActualizadoPor,e.estatus, ISNULL(d.Transito,0) Transito 
                                                        FROM catCarreteras AS c LEFT JOIN estatus AS e ON c.estatus = e.estatus 
                                                        inner join catDelegacionesOficinasTransporte b on c.idOficinaTransporte = b.idOficinaTransporte
                                                        INNER JOIN catDelegaciones d ON b.idOficinaTransporte = d.idDelegacion
                                                        WHERE (c.estatus = 1 and c.idOficinaTransporte= @idOficina) OR (c.estatus = 1 and c.idOficinaTransporte = 1) 

                                                        ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatCarreterasModel carretera = new CatCarreterasModel();
                            carretera.IdCarretera = Convert.ToInt32(reader["idCarretera"].ToString());
                            carretera.idOficinaTransporte = Convert.ToInt32(reader["idOficinaTransporte"].ToString());
                            carretera.Carretera = reader["carretera"].ToString();
                            carretera.estatusDesc = reader["estatus"].ToString();
                            carretera.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            carretera.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            carretera.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
                            carretera.Transito = Convert.ToInt32(reader["Transito"]);
                            ListaCarreteras.Add(carretera);

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
            return ListaCarreteras;


        }


        public List<CatCarreterasModel> GetCarreterasPorDelegacionTodos(int idOficina)
        {
            //
            List<CatCarreterasModel> ListaCarreteras = new List<CatCarreterasModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    //c.idOficinaTransporte = @idOficina OR c.idOficinaTransporte = 1 AND
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT c.idCarretera,c.idOficinaTransporte,UPPER(c.carretera) AS carretera,
                                                        c.estatus,c.FechaActualizacion,c.ActualizadoPor,e.estatus, ISNULL(d.Transito,0) Transito 
                                                        FROM catCarreteras AS c LEFT JOIN estatus AS e ON c.estatus = e.estatus 
                                                        inner join catDelegacionesOficinasTransporte b on c.idOficinaTransporte = b.idOficinaTransporte
                                                        INNER JOIN catDelegaciones d ON b.idOficinaTransporte = d.idDelegacion
                                                        WHERE (c.idOficinaTransporte= @idOficina) OR (c.idOficinaTransporte = 1) 

                                                        ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatCarreterasModel carretera = new CatCarreterasModel();
                            carretera.IdCarretera = Convert.ToInt32(reader["idCarretera"].ToString());
                            carretera.idOficinaTransporte = Convert.ToInt32(reader["idOficinaTransporte"].ToString());
                            carretera.Carretera = reader["carretera"].ToString();
                            carretera.estatusDesc = reader["estatus"].ToString();
                            carretera.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            carretera.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            carretera.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
                            carretera.Transito = Convert.ToBoolean(reader["Transito"]) ? 1 : 0;
                            ListaCarreteras.Add(carretera);

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
            return ListaCarreteras;


        }




        public List<CatCarreterasModel> GetCarreterasPorDelegacion2(int idOficina)
        {
            //
            List<CatCarreterasModel> ListaCarreteras = new List<CatCarreterasModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    //c.idOficinaTransporte = @idOficina OR c.idOficinaTransporte = 1 AND
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT c.idCarretera,c.idOficinaTransporte,UPPER(c.carretera) AS carretera,
                                                        c.estatus,c.FechaActualizacion,c.ActualizadoPor,e.estatus, ISNULL(d.Transito,0) Transito 
                                                        FROM catCarreteras AS c LEFT JOIN estatus AS e ON c.estatus = e.estatus 
                                                        inner join catDelegacionesOficinasTransporte b on c.idOficinaTransporte = b.idOficinaTransporte
                                                        INNER JOIN catDelegaciones d ON b.idOficinaTransporte = d.idDelegacion
                                                        WHERE c.estatus = 1 and  (c.idOficinaTransporte=@idOficina  or c.idOficinaTransporte=1)

                                                        ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatCarreterasModel carretera = new CatCarreterasModel();
                            carretera.IdCarretera = Convert.ToInt32(reader["idCarretera"].ToString());
                            carretera.idOficinaTransporte = Convert.ToInt32(reader["idOficinaTransporte"].ToString());
                            carretera.Carretera = reader["carretera"].ToString().ToUpper();
                            carretera.estatusDesc = reader["estatus"].ToString();
                            carretera.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            carretera.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            carretera.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
                            carretera.Transito = Convert.ToBoolean(reader["Transito"]) ? 1 : 0;
                            ListaCarreteras.Add(carretera);

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
            return ListaCarreteras;


        }

        public List<CatCarreterasModel> GetCarreterasParaIngreso(int idMunicipio)
        {
            //
            List<CatCarreterasModel> ListaCarreteras = new List<CatCarreterasModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"
                                                        SELECT DISTINCT catCar.idCarretera, catCar.carretera
                                                        FROM catCarreteras catCar 
                                                        LEFT JOIN catMunicipios mun ON mun.idOficinaTransporte = catCar.idOficinaTransporte
                                                        WHERE (mun.idMunicipio = @idMunicipio OR catCar.idOficinaTransporte = 1) AND mun.idEntidad = 11 AND catCar.estatus= 1;
                                                        ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)idMunicipio ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatCarreterasModel carretera = new CatCarreterasModel();
                            carretera.IdCarretera = Convert.ToInt32(reader["idCarretera"].ToString());
                            carretera.Carretera = reader["carretera"].ToString();                          
                            ListaCarreteras.Add(carretera);

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
            return ListaCarreteras;


        }



    }
}



