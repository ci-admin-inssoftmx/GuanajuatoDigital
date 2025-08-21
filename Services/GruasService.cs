using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services
{
    public class GruasService : IGruasService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public GruasService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<GruasConcesionariosModel> GetGruasConcesionariosByIdCocesionario(int Id)
        {
            List<GruasConcesionariosModel> GruasConcesionariosList = new List<GruasConcesionariosModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact =
                        @"SELECT ga.idGrua,ga.operadorGrua,ga.fechaArribo,
                                ga.fechaInicio,ga.fechaFinal,ga.abanderamiento,ga.costoArrastre,ga.costoBanderazo,ga.costoSalvamento,ga.minutosManiobra,
                                ga.costoAbanderamiento,ga.costoTotal,
                                g.placas,g.capacidad,g.idTipoGrua,tg.tipoGrua,d.idDeposito
                                FROM gruasAsignadas AS ga
                                LEFT JOIN gruas AS g on g.idGrua = ga.idGrua
                                LEFT JOIN catTipoGrua tg on g.idTipoGrua = tg.idTipoGrua
                                LEFT JOIN depositos d on d.idDeposito = ga.idDeposito
                                WHERE ga.idDeposito = @idDeposito and ga.estatus=1";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@idDeposito", SqlDbType.Int)).Value = (object)Id ?? DBNull.Value;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            GruasConcesionariosModel gruasConcesionario = new GruasConcesionariosModel();
                            gruasConcesionario.IdGrua = reader["IdGrua"] is DBNull ? 0 : Convert.ToInt32(reader["IdGrua"]);
                            gruasConcesionario.IdDeposito = reader["idDeposito"] is DBNull ? 0 : Convert.ToInt32(reader["idDeposito"]);
                            gruasConcesionario.abanderamiento = reader["abanderamiento"] is DBNull ? 0 : Convert.ToInt32(reader["abanderamiento"]);
                            gruasConcesionario.minutosManiobra = reader["minutosManiobra"] is DBNull ? 0 : Convert.ToInt32(reader["minutosManiobra"]);
                            gruasConcesionario.fechaArribo = reader["fechaArribo"] is DBNull ? DateTime.MinValue : (DateTime)reader["fechaArribo"];
                            gruasConcesionario.fechaInicio = reader["fechaInicio"] is DBNull ? DateTime.MinValue : (DateTime)reader["fechaInicio"];
                            gruasConcesionario.fechaFinal = reader["fechaFinal"] is DBNull ? DateTime.MinValue : (DateTime)reader["fechaFinal"];
                            gruasConcesionario.costoAbanderamiento = reader["costoAbanderamiento"] is DBNull ? 0.0f : float.Parse(reader["costoAbanderamiento"].ToString());
                            gruasConcesionario.costoArrastre = reader["costoArrastre"] is DBNull ? 0.0f : float.Parse(reader["costoArrastre"].ToString());
                            gruasConcesionario.costoSalvamento = reader["costoSalvamento"] is DBNull ? 0.0f : float.Parse(reader["costoSalvamento"].ToString());
                            gruasConcesionario.costoBanderazo = reader["costoBanderazo"] is DBNull ? 0.0f : float.Parse(reader["costoBanderazo"].ToString());
                            gruasConcesionario.costoTotal = reader["costoTotal"] is DBNull ? 0.0f : float.Parse(reader["costoTotal"].ToString());
                            gruasConcesionario.IdTipoGrua = reader["IdTipoGrua"] is DBNull ? 0 : Convert.ToInt32(reader["IdTipoGrua"]);
                            gruasConcesionario.placas = reader["placas"] is DBNull ? string.Empty : reader["placas"].ToString();
                            gruasConcesionario.capacidad = reader["capacidad"] is DBNull ? string.Empty : reader["capacidad"].ToString();

                            string capacidadOriginal = reader["capacidad"].ToString();
                            string[] partes = capacidadOriginal.Split('.');
                            if (partes.Length > 1)
                            {
                                var decimales = partes[1].Substring(0, Math.Min(1, partes[1].Length));
                                if (decimales.Trim() != "0")
                                    capacidadOriginal = partes[0] + "." + partes[1].Substring(0, Math.Min(1, partes[1].Length));
                                else
                                    capacidadOriginal = partes[0];
                            }
                            gruasConcesionario.capacidadT = "(" + capacidadOriginal + " t)" ;
                            gruasConcesionario.capacidad = capacidadOriginal + " t";
                            gruasConcesionario.TipoGrua = reader["TipoGrua"] is DBNull ? string.Empty : reader["TipoGrua"].ToString();
                            gruasConcesionario.operadorGrua = reader["operadorGrua"] is DBNull ? string.Empty : reader["operadorGrua"].ToString();


                            GruasConcesionariosList.Add(gruasConcesionario);

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
            return GruasConcesionariosList;

        }

        public List<TipoGruaModel> GetTipoGruas()
        {
            List<TipoGruaModel> ListTipoGruas = new List<TipoGruaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catTipoGrua where estatus=1", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            TipoGruaModel tipoGruaModel = new TipoGruaModel();
                            tipoGruaModel.IdTipoGrua = Convert.ToInt32(reader["IdTipoGrua"].ToString());
                            tipoGruaModel.TipoGrua = reader["TipoGrua"].ToString();
                            ListTipoGruas.Add(tipoGruaModel);

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
            return ListTipoGruas;

        }

        public List<GruasModel> GetGruas()
        {
            List<GruasModel> ListGruas = new List<GruasModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from Gruas where estatus=1", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            GruasModel gruaModel = new GruasModel();
                            gruaModel.IdGrua = Convert.ToInt32(reader["IdGrua"].ToString());
                            gruaModel.Placas = reader["Placas"].ToString();
                            gruaModel.noEconomico = reader["noEconomico"].ToString();

                            ListGruas.Add(gruaModel);

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
            return ListGruas;

        }

        public int CrearGrua(Gruas2Model model)
        {
            int result = 0;
            string strQuery = @"INSERT INTO gruas (idConcesionario
                                                        ,idClasificacion
                                                        ,idTipoGrua
                                                        ,idSituacion
                                                        ,noEconomico
                                                        ,placas
                                                        ,modelo
                                                        ,capacidad
                                                        ,fechaActualizacion
                                                        ,actualizadoPor
                                                        ,estatus
                                                    ) VALUES(@idConcesionario
                                                        ,@idClasificacion
                                                        ,@idTipoGrua
                                                        ,@idSituacion
                                                        ,@noEconomico
                                                        ,@placas
                                                        ,@modelo
                                                        ,@capacidad
                                                        ,@fechaActualizacion
                                                        ,@actualizadoPor
                                                        ,@estatus)";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    //command.Parameters.Add(new SqlParameter("@idGrua", SqlDbType.Int)).Value = (object)model.idGrua ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idConcesionario", SqlDbType.Int)).Value = (object)model.idConcesionario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idClasificacion", SqlDbType.Int)).Value = (object)model.idClasificacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoGrua", SqlDbType.Int)).Value = (object)model.idTipoGrua ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idSituacion", SqlDbType.Int)).Value = (object)model.idSituacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@noEconomico", SqlDbType.NVarChar)).Value = (object)model.noEconomico.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@placas", SqlDbType.NVarChar)).Value = (object)model.placas.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@modelo", SqlDbType.NVarChar)).Value = (object)model.modelo.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@capacidad", SqlDbType.NVarChar)).Value = (object)model.capacidad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
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

        public int EditarGrua(Gruas2Model model)
        {
            int result = 0;
            string strQuery = @"UPDATE gruas SET idConcesionario       = @idConcesionario    
                                                 ,idClasificacion	   = @idClasificacion	
                                                 ,idTipoGrua			   = @idTipoGrua			
                                                 ,idSituacion		   = @idSituacion		
                                                 ,noEconomico		   = @noEconomico		
                                                 ,placas				   = @placas				
                                                 ,modelo				   = @modelo				
                                                 ,capacidad			   = @capacidad			
                                                 ,fechaActualizacion	   = @fechaActualizacion	
                                                 ,actualizadoPor		   = @actualizadoPor		
                                                 ,estatus			   = @estatus		
                                WHERE idGrua = @idGrua;";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idGrua", SqlDbType.Int)).Value = (object)model.idGrua ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idConcesionario", SqlDbType.Int)).Value = (object)model.idConcesionario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idClasificacion", SqlDbType.Int)).Value = (object)model.idClasificacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoGrua", SqlDbType.Int)).Value = (object)model.idTipoGrua ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idSituacion", SqlDbType.Int)).Value = (object)model.idSituacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@noEconomico", SqlDbType.NVarChar)).Value = (object)model.noEconomico.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@placas", SqlDbType.NVarChar)).Value = (object)model.placas.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@modelo", SqlDbType.NVarChar)).Value = (object)model.modelo.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@capacidad", SqlDbType.NVarChar)).Value = (object)model.capacidad.ToUpper() ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
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

        public int EliminarGrua(Gruas2Model model)
        {
            int result = 0;
            string strQuery = @"UPDATE gruas SET fechaActualizacion	   = @fechaActualizacion	
                                                 ,actualizadoPor		   = @actualizadoPor		
                                                 ,estatus			   = @estatus		
                                WHERE idGrua = @idGrua";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idGrua", SqlDbType.Int)).Value = (object)model.idGrua ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 0;
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

        public IEnumerable<Gruas2Model> GetAllGruas(int idOficina)
        {
            List<Gruas2Model> ListGruas = new List<Gruas2Model>();
            string strQuery = @"SELECT
                                 g.idGrua
                                ,g.idConcesionario
                                ,g.idClasificacion
                                ,g.idTipoGrua
                                ,g.idSituacion
                                ,g.noEconomico
                                ,g.placas
                                ,g.modelo
                                ,g.capacidad
                                ,g.fechaActualizacion
                                ,g.actualizadoPor
                                ,g.estatus
                                ,g.capacidad
								,cm.municipio
								,c.concesionario
                                ,c.idDelegacion
								,ccg.clasificacion
								,ctg.TipoGrua
								,csg.situacion
                                FROM gruas g
								INNER JOIN catClasificacionGruas ccg
								on g.idClasificacion = ccg.idClasificacionGrua
								INNER JOIN catTipoGrua ctg
								on g.idTipoGrua = ctg.IdTipoGrua
								INNER JOIN catSituacionGruas csg
								on g.idSituacion = csg.idSituacion
								INNER JOIN concesionarios c
								on g.idConcesionario = c.idConcesionario AND c.estatus = 1
								INNER JOIN catMunicipios cm
								on c.idMunicipio = cm.idMunicipio AND c.estatus = 1
                                WHERE g.estatus = 1";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = idOficina;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Gruas2Model gruasModel = new Gruas2Model();
                            gruasModel.idGrua = Convert.ToInt32(reader["idGrua"].ToString());
                            gruasModel.idConcesionario = Convert.ToInt32(reader["idConcesionario"].ToString());
                            gruasModel.idClasificacion = Convert.ToInt32(reader["idClasificacion"].ToString());
                            gruasModel.idTipoGrua = Convert.ToInt32(reader["idTipoGrua"].ToString());
                            gruasModel.idSituacion = Convert.ToInt32(reader["idSituacion"].ToString());
                            gruasModel.noEconomico = reader["noEconomico"].ToString();
                            gruasModel.placas = reader["placas"].ToString();
                            gruasModel.modelo = reader["modelo"].ToString();
                            gruasModel.capacidad = reader["capacidad"].ToString();
                            gruasModel.concesionario = reader["concesionario"].ToString();
                            gruasModel.municipio = reader["municipio"].ToString();
                            gruasModel.clasificacion = reader["clasificacion"].ToString();
                            gruasModel.tipoGrua = reader["tipoGrua"].ToString();
                            gruasModel.situacion = reader["situacion"].ToString();
                            ListGruas.Add(gruasModel);
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
            return ListGruas;
        }

        public Gruas2Model GetGruaById(int idGrua)
        {
            List<Gruas2Model> ListGruas = new List<Gruas2Model>();
            string strQuery = @"SELECT
                                 g.idGrua
                                ,g.idConcesionario        
                                ,con.idDelegacion
								,con.concesionario
								,cdo.nombreOficina
                                ,g.idClasificacion
                                ,g.idTipoGrua
                                ,g.idSituacion
                                ,g.noEconomico
                                ,g.placas
                                ,g.modelo
                                ,g.capacidad
                                ,g.fechaActualizacion
                                ,g.actualizadoPor
                                ,g.estatus
                                FROM gruas g 
								LEFT JOIN concesionarios con ON con.idConcesionario = g.idConcesionario
								LEFT JOIN catDelegacionesOficinasTransporte cdo ON cdo.idOficinaTransporte = con.idDelegacion
                                WHERE idGrua = @idGrua AND g.estatus = 1";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idGrua", SqlDbType.Int)).Value = (object)idGrua ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Gruas2Model gruasModel = new Gruas2Model();
                            gruasModel.idGrua = Convert.ToInt32(reader["idGrua"].ToString());
                            gruasModel.idDelegacion = Convert.ToInt32(reader["idDelegacion"].ToString());

                            gruasModel.idConcesionario = Convert.ToInt32(reader["idConcesionario"].ToString());
                            gruasModel.idClasificacion = Convert.ToInt32(reader["idClasificacion"].ToString());
                            gruasModel.idTipoGrua = Convert.ToInt32(reader["idTipoGrua"].ToString());
                            gruasModel.idSituacion = Convert.ToInt32(reader["idSituacion"].ToString());
                            gruasModel.noEconomico = reader["noEconomico"].ToString();
                            gruasModel.concesionario = reader["concesionario"].ToString();
                            gruasModel.Delegacion = reader["nombreOficina"].ToString();

                            gruasModel.placas = reader["placas"].ToString();
                            gruasModel.modelo = reader["modelo"].ToString();
							// gruasModel.capacidad = reader["capacidad"].ToString();
							string capacidadOriginal = reader["capacidad"].ToString();
							string[] partes = capacidadOriginal.Split('.');
							if (partes.Length > 1)
							{
								string parteDecimal = partes[1];
								if (parteDecimal.Length > 2)
								{
									parteDecimal = parteDecimal.Substring(0, 2); 
								}
								capacidadOriginal = partes[0] + "." + parteDecimal;
							}
							gruasModel.capacidad = capacidadOriginal;


							ListGruas.Add(gruasModel);
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
            return ListGruas.FirstOrDefault();
        }
        public List<Gruas2Model> GetGruaByPension(int iPg)

        {
            //
            List<Gruas2Model> ListGruas = new List<Gruas2Model>();
            string strQuery = @"SELECT
                                 idGrua
                                ,idConcesionario
                                ,idClasificacion
                                ,idTipoGrua
                                ,idSituacion
                                ,noEconomico
                                ,placas
                                ,modelo
                                ,capacidad
                                ,fechaActualizacion
                                ,actualizadoPor
                                ,estatus
                                FROM gruas
                                WHERE idConcesionario = @idPropietarioGrua AND estatus = 1";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idPropietarioGrua", SqlDbType.Int)).Value = (object)iPg ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Gruas2Model gruasModel = new Gruas2Model();
                            gruasModel.idGrua = Convert.ToInt32(reader["idGrua"].ToString());
                            gruasModel.idConcesionario = Convert.ToInt32(reader["idConcesionario"].ToString());
                            gruasModel.idClasificacion = Convert.ToInt32(reader["idClasificacion"].ToString());
                            gruasModel.idTipoGrua = Convert.ToInt32(reader["idTipoGrua"].ToString());
                            gruasModel.idSituacion = Convert.ToInt32(reader["idSituacion"].ToString());
                            gruasModel.noEconomico = reader["noEconomico"].ToString();
                            gruasModel.placas = reader["placas"].ToString();
                            gruasModel.modelo = reader["modelo"].ToString();
                            gruasModel.capacidad = reader["capacidad"].ToString();
                            ListGruas.Add(gruasModel);
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
            return ListGruas;
        }
        public IEnumerable<Gruas2Model> GetGruasByIdConcesionario(int idConcesionario)
        {
            List<Gruas2Model> ListGruas = new List<Gruas2Model>();
            string strQuery = @"SELECT
                                idGrua
                                ,idConcesionario
                                ,idClasificacion
                                ,idTipoGrua
                                ,idSituacion
                                ,CONCAT( noEconomico, ' (', SUBSTRING(capacidad,0,CHARINDEX('.', capacidad)) , '', CASE WHEN CONVERT(VARCHAR,ROUND(SUBSTRING(capacidad,CHARINDEX('.', capacidad)+1,1),0))=0 THEN '' ELSE '.' + CONVERT(VARCHAR,ROUND(SUBSTRING(capacidad,CHARINDEX('.', capacidad)+1,1),0)) END,' t)') noEconomico
                                ,placas
                                ,modelo
                                ,capacidad
                                ,fechaActualizacion
                                ,actualizadoPor
                                ,estatus
                                FROM gruas
                                WHERE idConcesionario = @idConcesionario AND estatus = 1";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idConcesionario", SqlDbType.Int)).Value = (object)idConcesionario ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Gruas2Model gruasModel = new Gruas2Model();
                            gruasModel.idGrua = Convert.ToInt32(reader["idGrua"].ToString());
                            gruasModel.idConcesionario = Convert.ToInt32(reader["idConcesionario"].ToString());
                            gruasModel.idClasificacion = Convert.ToInt32(reader["idClasificacion"].ToString());
                            gruasModel.idTipoGrua = Convert.ToInt32(reader["idTipoGrua"].ToString());
                            gruasModel.idSituacion = Convert.ToInt32(reader["idSituacion"].ToString());
                            gruasModel.noEconomico = reader["noEconomico"].ToString();
                            gruasModel.placas = reader["placas"].ToString();
                            gruasModel.modelo = reader["modelo"].ToString();
                            gruasModel.capacidad = reader["capacidad"].ToString();
                            ListGruas.Add(gruasModel);
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
            return ListGruas;
        }

        public IEnumerable<Gruas2Model> GetGruasToGrid(string placas, string noEconomico, int? idTipoGrua, int idOficina, int? idDelegacion, int? idConcesionario)
        {
            string strWherePlacas = !string.IsNullOrEmpty(placas) ? string.Format("'{0}'", placas) : "g.placas";
            string strWhereNoEconomico = !string.IsNullOrEmpty(noEconomico) ? string.Format("'{0}'", noEconomico) : "g.noEconomico";
            string strWhereidTipoGrua = idTipoGrua != null ? idTipoGrua.ToString() : "g.idTipoGrua";
            string strWhereidDelegacion = idDelegacion != null ? $"c.idDelegacion = {idDelegacion}" : "";
            string strWhereidConcesionario = idConcesionario != null ? $"g.idConcesionario = {idConcesionario}" : "";

            List<Gruas2Model> ListGruas = new List<Gruas2Model>();
            string strQuery = @"SELECT
                                 g.idGrua
                                ,g.idConcesionario
                                ,g.idClasificacion
                                ,g.idTipoGrua
                                ,g.idSituacion
                                ,g.noEconomico
                                ,g.placas
                                ,g.modelo
                                ,g.capacidad
                                ,g.fechaActualizacion
                                ,g.actualizadoPor
                                ,g.estatus
								,cm.municipio
								,c.concesionario
                                ,c.idDelegacion
								,ccg.clasificacion
								,ctg.TipoGrua
								,csg.situacion
                                FROM gruas g
								LEFT JOIN catClasificacionGruas ccg
								on g.idClasificacion = ccg.idClasificacionGrua
								INNER JOIN catTipoGrua ctg
								on g.idTipoGrua = ctg.IdTipoGrua
								INNER JOIN catSituacionGruas csg
								on g.idSituacion = csg.idSituacion
								INNER JOIN concesionarios c
								on g.idConcesionario = c.idConcesionario AND c.estatus = 1
								INNER JOIN catMunicipios cm
								on c.idMunicipio = cm.idMunicipio AND c.estatus = 1
                                 WHERE
        g.estatus = 1
        AND (g.placas = @placas OR @placas IS NULL)
        AND (c.idDelegacion = @idDelegacion OR @idDelegacion IS NULL)
        AND (g.noEconomico = @noEconomico OR @noEconomico IS NULL)
        AND (g.idTipoGrua = @idTipoGrua OR @idTipoGrua IS NULL)
        " + (string.IsNullOrEmpty(strWhereidDelegacion) ? "" : "AND " + strWhereidDelegacion) + @"
        " + (string.IsNullOrEmpty(strWhereidConcesionario) ? "" : "AND " + strWhereidConcesionario);

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = idOficina;
                    command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = idDelegacion ?? (object)DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoGrua", SqlDbType.Int)).Value = idTipoGrua ?? (object)DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@idConcesionario", SqlDbType.Int)).Value = idConcesionario ?? (object)DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@placas", SqlDbType.NVarChar)).Value = string.IsNullOrEmpty(placas) ? (object)DBNull.Value : placas;
                    command.Parameters.Add(new SqlParameter("@noEconomico", SqlDbType.NVarChar)).Value = string.IsNullOrEmpty(noEconomico) ? (object)DBNull.Value : noEconomico;


                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Gruas2Model gruasModel = new Gruas2Model();
                            gruasModel.idGrua = Convert.ToInt32(reader["idGrua"].ToString());
                            gruasModel.idConcesionario = Convert.ToInt32(reader["idConcesionario"].ToString());
                            gruasModel.idClasificacion = Convert.ToInt32(reader["idClasificacion"].ToString());
                            gruasModel.idTipoGrua = Convert.ToInt32(reader["idTipoGrua"].ToString());
                            gruasModel.idSituacion = Convert.ToInt32(reader["idSituacion"].ToString());
                            gruasModel.noEconomico = reader["noEconomico"].ToString();
                            gruasModel.placas = reader["placas"].ToString();
                            gruasModel.modelo = reader["modelo"].ToString();
                            gruasModel.capacidad = reader["capacidad"].ToString();
                            gruasModel.concesionario = reader["concesionario"].ToString();
                            gruasModel.municipio = reader["municipio"].ToString();
                            gruasModel.clasificacion = reader["clasificacion"].ToString();
                            gruasModel.tipoGrua = reader["tipoGrua"].ToString();
                            gruasModel.situacion = reader["situacion"].ToString();
                            ListGruas.Add(gruasModel);
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
                return ListGruas;
            }
        }
    }
}
