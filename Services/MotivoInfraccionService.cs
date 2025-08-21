using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Services;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.IdentityModel.Tokens;

namespace GuanajuatoAdminUsuarios.Services
{
    public class MotivoInfraccionService : IMotivoInfraccionService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;

        public MotivoInfraccionService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public CatMotivosInfraccionModel GetMotivoByID(int IdCatMotivoInfraccion, int idDependencia)
        {
            List<CatMotivosInfraccionModel> motivos = new List<CatMotivosInfraccionModel>();
            string query = @"SELECT 
                                cmi.fechaInicio,
								cmi.fechaFinVigencia,
	                            cmi.idCatMotivoInfraccion ,
	                            cmi.nombre,
	                            cmi.fundamento ,
                                cmi.estatus,
	                            cmi.calificacionMinima , 
	                            cmi.calificacionMaxima , 
	                            cmi.IdConcepto , 
	                            c.concepto ,
                                cmi.idSubConcepto,
	                            sc.subConcepto ,
	                            e.estatusDesc as ValorEstatusMotivosInfraccion
                            FROM 
                            catMotivosInfraccion cmi
                            JOIN estatus e ON cmi.estatus = e.estatus
                            JOIN catConceptoInfraccion c ON cmi.idConcepto = c.idConcepto
                            JOIN catSubConceptoInfraccion sc ON cmi.idSubConcepto = sc.idSubConcepto
                            WHERE idCatMotivoInfraccion  = @idCatMotivoInfraccion AND cmi.transito = @idDependencia";


            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idDependencia", idDependencia);
					command.Parameters.Add(new SqlParameter("@idCatMotivoInfraccion", SqlDbType.Int)).Value = (object)IdCatMotivoInfraccion ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMotivosInfraccionModel model = new CatMotivosInfraccionModel();
                            model.IdCatMotivoInfraccion = reader["idCatMotivoInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatMotivoInfraccion"].ToString());
                            model.Estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
                            model.estatusDesc = reader["ValorEstatusMotivosInfraccion"].ToString();

                            model.Nombre = reader["nombre"].ToString();
                            model.Fundamento = reader["fundamento"].ToString();
                            model.CalificacionMinima = reader["calificacionMinima"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["calificacionMinima"].ToString());
                            model.CalificacionMaxima = reader["calificacionMaxima"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["calificacionMaxima"].ToString());
                            model.idConcepto = reader["IdConcepto"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["IdConcepto"].ToString());
                            model.concepto = reader["concepto"].ToString();
                            model.idSubConcepto = reader["IdSubConcepto"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["IdSubConcepto"].ToString());
                            model.subConcepto = reader["subConcepto"].ToString();
                            model.ValorEstatusMotivosInfraccion = reader["ValorEstatusMotivosInfraccion"].ToString() == "inactivo" ? false :true;
                            model.fechaInicioVigencia = reader["fechaInicio"] == System.DBNull.Value ? null : Convert.ToDateTime(reader["fechaInicio"]);
                            model.fechaFinVigencia = reader["fechaFinVigencia"] == System.DBNull.Value ? null : Convert.ToDateTime(reader["fechaFinVigencia"]);

                            motivos.Add(model);
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
            return motivos.FirstOrDefault();
        }

        public List<CatMotivosInfraccionModel> GetMotivos(int idDependencia)
        {
            List<CatMotivosInfraccionModel> motivos = new List<CatMotivosInfraccionModel>();
            string query = @"SELECT 
	                            cmi.idCatMotivoInfraccion ,
	                            cmi.nombre,
	                            cmi.estatus,
	                            cmi.fundamento , 
	                            cmi.calificacionMinima , 
	                            cmi.calificacionMaxima , 
	                            cmi.IdConcepto ,
                                cmi.fechaInicio ,
                                cmi.fechaFinVigencia,
	                            c.concepto ,
                                cmi.idSubConcepto,
	                            sc.subConcepto ,
	                            e.estatusDesc
                            FROM 
                            catMotivosInfraccion cmi
                            LEFT JOIN estatus e ON cmi.estatus = e.estatus
                            LEFT JOIN catConceptoInfraccion c ON cmi.idConcepto = c.idConcepto
                            LEFT JOIN catSubConceptoInfraccion sc ON cmi.idSubConcepto = sc.idSubConcepto 
                            WHERE cmi.transito = @idDependencia";


            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idDependencia", idDependencia);
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMotivosInfraccionModel model = new CatMotivosInfraccionModel();
                            model.IdCatMotivoInfraccion = reader["idCatMotivoInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatMotivoInfraccion"].ToString());
                            model.Estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatMotivoInfraccion"].ToString());

                            model.Nombre = reader["nombre"].ToString();
                            model.Fundamento = reader["fundamento"].ToString();
                            model.CalificacionMinima = reader["calificacionMinima"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["calificacionMinima"].ToString());
                            model.CalificacionMaxima = reader["calificacionMaxima"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["calificacionMaxima"].ToString());
                            model.idConcepto = reader["IdConcepto"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["IdConcepto"].ToString());
                            model.concepto = reader["concepto"].ToString();
                            model.idSubConcepto = reader["IdSubConcepto"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["IdSubConcepto"].ToString());
                            model.subConcepto = reader["subConcepto"].ToString();
                            model.estatusDesc = reader["estatusDesc"] is DBNull ? string.Empty : reader["estatusDesc"].ToString();
                            model.fechaInicioVigencia = reader["fechaInicio"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInicio"]);
                            model.fechaFinVigencia = reader["fechaFinVigencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaFinVigencia"]);

                            motivos.Add(model);
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
            return motivos;
        }


        public List<CatMotivosInfraccionModel> GetCatMotivos(int idDependencia)
        {
            List<CatMotivosInfraccionModel> motivos = new List<CatMotivosInfraccionModel>();
            string query = @"SELECT 
                            idCatMotivoInfraccion , nombre 
                            FROM 
                            catMotivosInfraccion cmi WHERE transito = @idDependencia";


            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idDependencia", idDependencia);
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMotivosInfraccionModel model = new CatMotivosInfraccionModel();
                            model.IdCatMotivoInfraccion = reader["idCatMotivoInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatMotivoInfraccion"].ToString());
                            model.Nombre = reader["nombre"].ToString();
                            
                            motivos.Add(model);
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
            return motivos;
        }

        public int CrearMotivo(CatMotivosInfraccionModel motivo, int idDependencia)
        {
            int result = 0;

            string query = @"INSERT INTO catMotivosInfraccion
                            (nombre
                                , IdSubConcepto
                                , fechaActualizacion
                                , actualizadoPor
                                , estatus
                                , IdConcepto
                                , calificacionMinima
                                , calificacionMaxima
                                , fundamento
                                , fechaInicio
                                , fechaFinVigencia
                                 ,transito)
                            VALUES(@nombre
                                , @IdSubConcepto
                                , @fechaActualizacion
                                , @actualizadoPor
                                , @estatus
                                , @IdConcepto
                                , @calificacionMinima
                                , @calificacionMaxima
                                , @fundamento
                                , @fechaInicioVigencia
                                , @fechaFinalVigencia
                                , @idDependencia)";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idDependencia", idDependencia);
					command.Parameters.AddWithValue("@nombre", motivo.Nombre);
                    command.Parameters.AddWithValue("@IdSubConcepto", motivo.idSubConcepto);
                    command.Parameters.AddWithValue("@IdConcepto", (motivo.idConcepto==null) ? DBNull.Value : motivo.idConcepto);
                    command.Parameters.AddWithValue("@calificacionMinima", motivo.CalificacionMinima);
                    command.Parameters.AddWithValue("@calificacionMaxima", motivo.CalificacionMaxima);
                    command.Parameters.AddWithValue("@fundamento", motivo.Fundamento);
                    command.Parameters.AddWithValue("@fechaInicioVigencia", (motivo.fechaInicioVigencia == null) ? DBNull.Value : motivo.fechaInicioVigencia);
                    command.Parameters.AddWithValue("@fechaFinalVigencia", (motivo.fechaFinVigencia==null) ? DBNull.Value : motivo.fechaFinVigencia);

                    command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);
                    command.Parameters.AddWithValue("@actualizadoPor", 1);
                    command.Parameters.AddWithValue("@estatus", 1);


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
            return result;
        }


        public int UpdateMotivo(CatMotivosInfraccionModel motivo, int idDependencia)
        {
            int result = 0;
            string strQuery = @"UPDATE catMotivosInfraccion
                                SET nombre=@nombre
                                , IdSubConcepto=@IdSubConcepto
                                , fechaActualizacion=@fechaActualizacion
                                , actualizadoPor=@actualizadoPor
                                , estatus=@estatus
                                , IdConcepto=@IdConcepto
                                , calificacionMinima=@calificacionMinima
                                , calificacionMaxima=@calificacionMaxima
                                , fundamento=@fundamento
                                , fechaInicio = @fechaInicioVigencia
                                , fechaFinVigencia = @fechaFinalVigencia
                                , transito=@idDependencia                               
                                WHERE idCatMotivoInfraccion=@idCatMotivoInfraccion";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idDependencia", idDependencia);
					command.Parameters.AddWithValue("@idCatMotivoInfraccion", motivo.IdCatMotivoInfraccion);
                    command.Parameters.AddWithValue("@nombre", motivo.Nombre);
                    command.Parameters.AddWithValue("@IdSubConcepto",motivo.idSubConcepto);
                    command.Parameters.AddWithValue("@IdConcepto",motivo.idConcepto);
                    command.Parameters.AddWithValue("@calificacionMinima",motivo.CalificacionMinima);
                    command.Parameters.AddWithValue("@calificacionMaxima", motivo.CalificacionMaxima);
                    command.Parameters.AddWithValue("@fundamento", motivo.Fundamento);
					command.Parameters.AddWithValue("@fechaInicioVigencia", (motivo.fechaInicioVigencia == null) ? DBNull.Value : motivo.fechaInicioVigencia);
					command.Parameters.AddWithValue("@fechaFinalVigencia", (motivo.fechaFinVigencia == null) ? DBNull.Value : motivo.fechaFinVigencia);

					command.Parameters.AddWithValue("@estatus", motivo.estatus);


                    command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);
                    command.Parameters.AddWithValue("@actualizadoPor", 1);

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



        public int DeleteMotivo(CatMotivosInfraccionModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("DELETE FROM catMotivosInfraccion WHERE idCatMotivoInfraccion=@idCatMotivo", connection);
                    command.Parameters.Add(new SqlParameter("@idCatMotivo", SqlDbType.Int)).Value = model.IdCatMotivoInfraccion;
                    command.CommandType = CommandType.Text;
                    result = command.ExecuteNonQuery();

                }
                catch (Exception ex)
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

       public List<CatMotivosInfraccionModel> GetMotivosBusqueda(CatMotivosInfraccionModel model, int idDependencia)
        {
            List<CatMotivosInfraccionModel> motivosList = new List<CatMotivosInfraccionModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();                    
                    string condiciones = "";

                    condiciones += model.Nombre.IsNullOrEmpty() ? "" : " AND mi.nombre LIKE '%' + @Nombre + '%' ";
                    condiciones += model.Fundamento.IsNullOrEmpty() ? "" : " AND mi.fundamento LIKE '%' + @Fundamento + '%' ";
                    if (model.InicioVigenciaDesde.HasValue || model.InicioVigenciaHasta.HasValue ||
                        model.FinVigenciaDesde.HasValue || model.FinVigenciaHasta.HasValue)
                    {
                        if (model.InicioVigenciaDesde.HasValue && model.InicioVigenciaHasta.HasValue)
                        {
                            condiciones += " AND mi.fechaInicio BETWEEN @InicioVigenciaDesde AND @InicioVigenciaHasta";
                        }
                        else if (model.InicioVigenciaDesde.HasValue && !model.InicioVigenciaHasta.HasValue)
                        {
                            condiciones += " AND mi.fechaInicio >= @InicioVigenciaDesde";
                        }
                        else if (!model.InicioVigenciaDesde.HasValue && model.InicioVigenciaHasta.HasValue)
                        {
                            condiciones += " AND mi.fechaInicio <= @InicioVigenciaHasta";
                        }

                        if (model.FinVigenciaDesde.HasValue && model.FinVigenciaHasta.HasValue)
                        {
                            condiciones += " AND mi.fechaFinVigencia BETWEEN @FinVigenciaDesde AND @FinVigenciaHasta";
                        }
                        else if (model.FinVigenciaDesde.HasValue && !model.FinVigenciaHasta.HasValue)
                        {
                            condiciones += " AND mi.fechaFinVigencia >= @FinVigenciaDesde";
                        }
                        else if (!model.FinVigenciaDesde.HasValue && model.FinVigenciaHasta.HasValue)
                        {
                            condiciones += " AND mi.fechaFinVigencia <= @FinVigenciaHasta";
                        }
                    }
                    if (model.IdVigencia != 0)
                    {
                        if (model.IdVigencia == 1)
                        {
                            condiciones += " AND (GETDATE() BETWEEN mi.fechaInicio AND mi.fechaFinVigencia OR mi.fechaFinVigencia IS NULL)";
                        }
                        else if (model.IdVigencia == 2)
                        {
                            condiciones += " AND NOT (GETDATE() BETWEEN mi.fechaInicio AND mi.fechaFinVigencia)";
                        }
                    }



                    string SqlTransact =
                                @"SELECT mi.idCatMotivoInfraccion, mi.nombre,mi.fundamento,mi.calificacionMinima,mi.calificacionMaxima,
                                         mi.fechaInicio, mi.fechaFinVigencia,mi.estatus,mi.transito,e.estatusDesc
										 FROM catMotivosInfraccion AS mi
										 LEFT JOIN estatus AS e ON mi.estatus = e.estatus
                                         WHERE mi.transito = @idDependencia" + condiciones;

                    SqlCommand command = new SqlCommand(SqlTransact, connection);

                    command.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar)).Value = (object)model.Nombre ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Fundamento", SqlDbType.NVarChar)).Value = (object)model.Fundamento ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object) idDependencia;                   
                    command.Parameters.Add(new SqlParameter("@InicioVigenciaDesde", SqlDbType.DateTime)).Value = (model.InicioVigenciaDesde != null) ? (object)model.InicioVigenciaDesde : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@InicioVigenciaHasta", SqlDbType.DateTime)).Value = (model.InicioVigenciaHasta != null) ? (object)model.InicioVigenciaHasta : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@FinVigenciaDesde", SqlDbType.DateTime)).Value = (model.FinVigenciaDesde != null) ? (object)model.FinVigenciaDesde : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@FinVigenciaHasta", SqlDbType.DateTime)).Value = (model.FinVigenciaHasta != null) ? (object)model.FinVigenciaHasta : DBNull.Value;

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMotivosInfraccionModel motivo = new CatMotivosInfraccionModel();
                            motivo.IdCatMotivoInfraccion = Convert.ToInt32(reader["idCatMotivoInfraccion"] is DBNull ? 0 : reader["idCatMotivoInfraccion"]);
                            motivo.Nombre = reader["nombre"].ToString();
                            motivo.Fundamento = reader["fundamento"].ToString();
                            motivo.CalificacionMinima = reader["calificacionMinima"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["calificacionMinima"].ToString());
                            motivo.CalificacionMaxima = reader["calificacionMaxima"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["calificacionMaxima"].ToString());
                            motivo.estatusDesc = reader["estatusDesc"] is DBNull ? string.Empty : reader["estatusDesc"].ToString();
                            motivo.fechaInicioVigencia = reader["fechaInicio"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInicio"]);
                            motivo.fechaFinVigencia = reader["fechaFinVigencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaFinVigencia"]);
                            motivo.Corp = Convert.ToInt32(reader["transito"] is DBNull ? 0 : reader["transito"]);

                            motivosList.Add(motivo);

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
            return motivosList;
        }
        public List<CatMotivosInfraccionModel> GetMotivosDropDown(int idDependencia, int idSubconcepto,int idConcepto)
        {
            List<CatMotivosInfraccionModel> motivos = new List<CatMotivosInfraccionModel>();
            string query = @"SELECT 
                                    cmi.idCatMotivoInfraccion,
                                    cmi.nombre + ' - '+ cmi.fundamento  nombre,
                                    cmi.fundamento,
                                    cmi.calificacionMinima,
                                    cmi.calificacionMaxima,
                                    cmi.IdConcepto,
                                    cmi.fechaInicio,
                                    cmi.fechaFinVigencia,
                                    c.concepto,
                                    cmi.idSubConcepto,
                                    sc.subConcepto,
                                    e.estatusDesc
                                FROM 
                                    catMotivosInfraccion cmi
                                LEFT JOIN 
                                    estatus e ON cmi.estatus = e.estatus
                                LEFT JOIN 
                                    catConceptoInfraccion c ON cmi.idConcepto = c.idConcepto
                                LEFT JOIN 
                                    catSubConceptoInfraccion sc ON cmi.idSubConcepto = sc.idSubConcepto 
                               WHERE 
                                cmi.transito = @idDependencia
                                AND cmi.estatus = 1 
                                AND (cmi.fechaFinVigencia >= GETDATE() OR cmi.fechaFinVigencia IS NULL)

                                    {0}
                                ";


            if(idConcepto!=0 && idSubconcepto!=0)            
               query= String.Format(query, "AND cmi.idConcepto=@Consepto and  cmi.idSubConcepto=@idSubconcepto");            
            else if(idConcepto != 0)
                query = String.Format(query, "AND cmi.idConcepto=@Consepto ");
            else if (idSubconcepto != 0)
                query = String.Format(query, "AND cmi.idSubConcepto=@idSubconcepto");
            else
                query = String.Format(query, "");

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idDependencia", idDependencia);

                    if(idSubconcepto!=0)
                    command.Parameters.AddWithValue("@idSubconcepto", idSubconcepto);
                    if (idConcepto != 0)
                        command.Parameters.AddWithValue("@Consepto", idConcepto);



                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMotivosInfraccionModel model = new CatMotivosInfraccionModel();
                            model.IdCatMotivoInfraccion = reader["idCatMotivoInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatMotivoInfraccion"].ToString());
                            model.Nombre = reader["nombre"].ToString();
                           
                            motivos.Add(model);
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
            return motivos;
        }

        public List<CatMotivosInfraccionModel> GetSubconceptos(int idConceptoValue)
        {
            List<CatMotivosInfraccionModel> motivos = new List<CatMotivosInfraccionModel>();
            string query = @"SELECT sc.idSubConcepto,sc.subConcepto FROM catSubConceptoInfraccion sc
                            WHERE idConcepto = @idConcepto AND sc.estatus = 1";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idConcepto", idConceptoValue);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMotivosInfraccionModel model = new CatMotivosInfraccionModel();
                            model.idSubConcepto = reader["idSubConcepto"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idSubConcepto"].ToString());
                            model.subConcepto = reader["subConcepto"].ToString();
                            motivos.Add(model);
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
            return motivos;
        }

    }
}