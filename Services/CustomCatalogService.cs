using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CustomCatalogService : ICustomCatalogService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;

        public CustomCatalogService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;

        }


        public List<CatalogModel> GetDelegaciones()
        {

            var result = new List<CatalogModel>();


            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select idDelegacion,delegacion from catdelegaciones where estatus=1 and transito=1", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["idDelegacion"].ToString();
                            cat.text = reader["delegacion"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }


            return result;
        }

        public List<CatalogModel> GetDelegaciones(int corp)
        {

            var result = new List<CatalogModel>();


            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select idDelegacion,delegacion from catdelegaciones where estatus=1 and transito=@corp", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corp);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["idDelegacion"].ToString();
                            cat.text = reader["delegacion"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }


            return result;
        }



        public List<CatalogModel> GetFactorAccidente(int corp)
        {
            var result = new List<CatalogModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select idFactorAccidente,factorAccidente from catFactoresAccidentes where estatus=1 and transito = @corp", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["idFactorAccidente"].ToString();
                            cat.text = reader["factorAccidente"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }



        public List<CatalogModel> GetAllCarreteras()
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select idCarretera,carretera from catCarreteras\r\n", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["idCarretera"].ToString();
                            cat.text = reader["carretera"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }

        public List<CatalogModel> GetCarreterasByDelegacion(int idDelegacion)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand(@"select idCarretera,carretera from catCarreteras
                                                         WHERE @idDelegacion = 0 OR idOficinaTransporte = @idDelegacion", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = (object)idDelegacion ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["idCarretera"].ToString();
                            cat.text = reader["carretera"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }

        public List<CatalogModel> GetTiposCargaActivos()
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select idTipoCarga,tipoCarga from catTiposcarga WHERE estatus = 1", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["idTipoCarga"].ToString();
                            cat.text = reader["tipoCarga"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }

        public List<CatalogModel> GetClasificacionesAccidentesActivas()
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select IdClasificacionAccidente,NombreClasificacion from catClasificacionAccidentes WHERE estatus = 1", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["IdClasificacionAccidente"].ToString();
                            cat.text = reader["NombreClasificacion"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }
        public List<CatalogModel> GetClasificacionesAccidentesActivas(int corp)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select IdClasificacionAccidente,NombreClasificacion from catClasificacionAccidentes WHERE estatus = 1 and transito = @corp", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@corp", corp);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["IdClasificacionAccidente"].ToString();
                            cat.text = reader["NombreClasificacion"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }

        public List<CatalogModel> GetCausasAccidentesActivas(int corp)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select IdCausaAccidente,CausaAccidente from catCausasAccidentes WHERE estatus = 1 and transito = @corp", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@corp", corp);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["IdCausaAccidente"].ToString();
                            cat.text = reader["CausaAccidente"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }



        public List<CatalogModel> GetCausasAccidentesActivasCorporacion(int corp)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    

                    connection.Open();
                    string sql = "select IdCausaAccidente,CausaAccidente from catCausasAccidentes WHERE estatus = 1 and transito=" + corp;
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["IdCausaAccidente"].ToString();
                            cat.text = reader["CausaAccidente"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }

        public List<CatalogModel> GetCausasAccidentesActivas()
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select IdCausaAccidente,CausaAccidente from catCausasAccidentes WHERE estatus = 1 ", connection);
                    command.CommandType = CommandType.Text;
                   // command.Parameters.AddWithValue("@corp", corp);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["IdCausaAccidente"].ToString();
                            cat.text = reader["CausaAccidente"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }


        public List<CatalogModel> GetHospitalesActivos(int corp)
        {
            var corporacion = corp < 2 ? corp : 1;
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select IdHospital,NombreHospital from catHospitales WHERE estatus = 1 and transito = @corp", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@corp", corporacion);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["IdHospital"].ToString();
                            cat.text = reader["NombreHospital"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }


        public List<CatalogModel> GetHospitales()
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select IdHospital,NombreHospital from catHospitales ", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["IdHospital"].ToString();
                            cat.text = reader["NombreHospital"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }


        public List<CatalogModel> GetAutoridadesDisposicionActivas(int corp)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select IdAutoridadDisposicion,NombreAutoridadDisposicion from catAutoridadesDisposicion WHERE estatus = 1 AND transito = @corp", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@corp", corp);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["IdAutoridadDisposicion"].ToString();
                            cat.text = reader["NombreAutoridadDisposicion"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }

        public List<CatalogModel> GetAutoridadesDisposicion(int corp)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select IdAutoridadDisposicion,NombreAutoridadDisposicion from catAutoridadesDisposicion AND transito = @corp ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@corp", corp);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["IdAutoridadDisposicion"].ToString();
                            cat.text = reader["NombreAutoridadDisposicion"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }


        public List<CatalogModel> GetAutoridadesEntregaActivas(int corp)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select IdAutoridadEntrega,AutoridadEntrega from catAutoridadesEntrega WHERE estatus = 1 AND transito = @corp", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@corp", corp);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["IdAutoridadEntrega"].ToString();
                            cat.text = reader["AutoridadEntrega"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }


        public List<CatalogModel> GetAutoridadesEntrega(int corp)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select IdAutoridadEntrega,AutoridadEntrega from catAutoridadesEntrega WHERE transito = @corp ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@corp", corp);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["IdAutoridadEntrega"].ToString();
                            cat.text = reader["AutoridadEntrega"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }


        public List<CatalogModel> GetEntidadesActivas(int corp)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select idEntidad,nombreEntidad from catEntidades WHERE estatus = 1 AND transito = @corp", connection);
                    command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corp ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["idEntidad"].ToString();
                            cat.text = reader["nombreEntidad"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }

		public List<CatalogModel> GetEntidadesActivasPorCorporacion(int corp)
		{
			var result = new List<CatalogModel>();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{

					connection.Open();
					SqlCommand command = new SqlCommand("select idEntidad,nombreEntidad from catEntidades WHERE estatus = 1 AND transito = @corp", connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corp ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{

							var cat = new CatalogModel();

							cat.value = reader["idEntidad"].ToString();
							cat.text = reader["nombreEntidad"].ToString();

							result.Add(cat);
						}
					}

				}
				catch (Exception ex)
				{

				}
				finally
				{

				}
			}
			return result;
		}


		public List<CatalogModel> GetEntidades()
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("select idEntidad,nombreEntidad from catEntidades ", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["idEntidad"].ToString();
                            cat.text = reader["nombreEntidad"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }


        public List<CatalogModel> GetAgenciasMinisterioActivas(int corporation)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT cAm.IdAgenciaMinisterio,cAm.NombreAgencia
                                                              FROM catAgenciasMinisterio cAm
                                                             JOIN estatus ON cAm.estatus = estatus.estatus 
															 LEFT JOIN catDelegacionesOficinasTransporte ctd ON ctd.idOficinaTransporte = cAm.idDelegacion
                                                                WHERE cAm.estatus = 1 AND cAm.transito = @corp
                                                            ORDER BY IdAgenciaMinisterio", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporation ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["IdAgenciaMinisterio"].ToString();
                            cat.text = reader["NombreAgencia"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }

        public List<CatalogModel> GetMunicipiosActivosPorEntidad(int entidadDDlValue)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT m.idMunicipio,m.municipio FROM catMunicipios AS m 
                                                        LEFT JOIN catEntidades AS ent ON m.idEntidad = ent.idEntidad 
                                                        LEFT JOIN estatus AS e ON m.estatus = e.estatus WHERE m.idEntidad = @idEntidad AND m.estatus = 1", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)entidadDDlValue ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["idMunicipio"].ToString();
                            cat.text = reader["municipio"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }

        public List<CatalogModel> GetMunicipiosActivosPorEntidad(int entidadDDlValue, int IdMun)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT m.idMunicipio,m.municipio FROM catMunicipios AS m 
                                                        LEFT JOIN catEntidades AS ent ON m.idEntidad = ent.idEntidad 
                                                        LEFT JOIN estatus AS e ON m.estatus = e.estatus WHERE (m.idEntidad = @idEntidad or m.idMunicipio = @Mun) AND m.estatus = 1", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)entidadDDlValue ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Mun", SqlDbType.Int)).Value = (object)IdMun ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["idMunicipio"].ToString();
                            cat.text = reader["municipio"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }


        public List<CatalogModel> GetMunicipiosTodosPorEntidad(int entidadDDlValue)
		{
			var result = new List<CatalogModel>();
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{

					connection.Open();
					SqlCommand command = new SqlCommand(@"SELECT m.idMunicipio,m.municipio FROM catMunicipios AS m 
                                                        LEFT JOIN catEntidades AS ent ON m.idEntidad = ent.idEntidad 
                                                        LEFT JOIN estatus AS e ON m.estatus = e.estatus WHERE m.idEntidad = @idEntidad", connection);
					command.CommandType = CommandType.Text;
					command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)entidadDDlValue ?? DBNull.Value;

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{

							var cat = new CatalogModel();

							cat.value = reader["idMunicipio"].ToString();
							cat.text = reader["municipio"].ToString();

							result.Add(cat);
						}
					}

				}
				catch (Exception ex)
				{

				}
				finally
				{

				}
			}
			return result;
		}

		public List<CatalogModel> GetTramosActivosPorCarretera(int carreteraDDValue)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT t.idTramo,
                                                                   t.tramo,
                                                                   t.estatus,
                                                                   t.idCarretera
                                                            FROM catTramos AS t
                                                            INNER JOIN catCarreteras AS c ON t.idCarretera = c.idCarretera
                                                            INNER JOIN estatus AS e ON t.estatus = e.estatus
                                                            WHERE (t.idCarretera = @idCarretera OR t.idCarretera IN (1, 2))
                                                              AND t.estatus = 1
                                                            ORDER BY t.tramo ASC;
                                                             ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@IdCarretera", SqlDbType.Int)).Value = (object)carreteraDDValue ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["idTramo"].ToString();
                            cat.text = reader["tramo"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }
        public List<CatalogModel> GetCarreterasActivasPorOficina(int idOficina)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT c.idCarretera,
                                                                   c.idOficinaTransporte,
                                                                   UPPER(c.carretera) AS carretera,
                                                                   c.estatus,
                                                                   c.FechaActualizacion,
                                                                   c.ActualizadoPor,
                                                                   e.estatus,
                                                                   ISNULL(d.Transito, 0) AS Transito
                                                            FROM catCarreteras AS c
                                                            LEFT JOIN estatus AS e ON c.estatus = e.estatus
                                                            INNER JOIN catDelegacionesOficinasTransporte b ON c.idOficinaTransporte = b.idOficinaTransporte
                                                            INNER JOIN catDelegaciones d ON b.idOficinaTransporte = d.idDelegacion
                                                            WHERE (c.estatus = 1 AND c.idOficinaTransporte = 2)
                                                               OR (c.estatus = 1 AND c.idOficinaTransporte = 1);
                                                            ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object) idOficina ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["idCarretera"].ToString();
                            cat.text = reader["carretera"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }
        public List<CatalogModel> GetTodasCarreterasPorOficina(int idOficina)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT c.idCarretera,c.idOficinaTransporte,UPPER(c.carretera) AS carretera,
                                                        c.estatus,c.FechaActualizacion,c.ActualizadoPor,e.estatus, ISNULL(d.Transito,0) Transito 
                                                        FROM catCarreteras AS c LEFT JOIN estatus AS e ON c.estatus = e.estatus 
                                                        inner join catDelegacionesOficinasTransporte b on c.idOficinaTransporte = b.idOficinaTransporte
                                                        INNER JOIN catDelegaciones d ON b.idOficinaTransporte = d.idDelegacion
                                                        WHERE (c.idOficinaTransporte= @idOficina) OR (c.idOficinaTransporte = 1)", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["idCarretera"].ToString();
                            cat.text = reader["carretera"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }

        public List<CatalogModel> GetTodoTramosPorCarretera(int carreteraDDValue)
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT t.idTramo,t.tramo,t.estatus, t.idCarretera FROM catTramos AS t INNER JOIN catCarreteras AS c ON t.idCarretera = c.idCarretera
                                                        INNER JOIN estatus AS e ON t.estatus = e.estatus 
                                                        WHERE t.idCarretera = @idCarretera or t.idCarretera in (1,2) ORDER BY Tramo ASC ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@IdCarretera", SqlDbType.Int)).Value = (object)carreteraDDValue ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

                            var cat = new CatalogModel();

                            cat.value = reader["idTramo"].ToString();
                            cat.text = reader["tramo"].ToString();

                            result.Add(cat);
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }
            return result;
        }



        public List<CatalogModel> GetOficialesByCorporacionActivos(int corporacion)
        {
            //
            List<CatalogModel> oficiales = new List<CatalogModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"
                                                            Select *, cd.nombreOficina,e.estatusDesc from catOficiales co
                                                            LEFT JOIN catDelegacionesOficinasTransporte cd ON cd.idOficinaTransporte = co.idOficina
                                                            LEFT JOIN estatus e ON e.estatus = co.estatus
															where co.transito=@corp AND co.estatus = 1", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporacion ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatalogModel oficial = new CatalogModel();
                            oficial.value =reader["IdOficial"].ToString();
                            oficial.text = reader["Nombre"].ToString() + " " + reader["ApellidoPaterno"].ToString() + " " + reader["ApellidoMaterno"].ToString();
                            
                            oficiales.Add(oficial);

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
            return oficiales;


        }


        public List<CatalogModel> GetOficialesByCorporacion(int corporacion)
        {
            //
            List<CatalogModel> oficiales = new List<CatalogModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"
                                                            Select *, cd.nombreOficina,e.estatusDesc from catOficiales co
                                                            LEFT JOIN catDelegacionesOficinasTransporte cd ON cd.idOficinaTransporte = co.idOficina
                                                            LEFT JOIN estatus e ON e.estatus = co.estatus
															where co.transito=@corp ", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporacion ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatalogModel oficial = new CatalogModel();
                            oficial.value = reader["IdOficial"].ToString();
                            oficial.text = reader["Nombre"].ToString() + " " + reader["ApellidoPaterno"].ToString() + " " + reader["ApellidoMaterno"].ToString();

                            oficiales.Add(oficial);

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
            return oficiales;


        }




        public List<CatalogModel> ObtenerAgenciasActivas(int corporation)
        {
            //
            List<CatalogModel> ListaAgencias = new List<CatalogModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT cAm.*, estatus.estatusdesc,ctd.nombreOficina
                                                              FROM catAgenciasMinisterio cAm
                                                             left JOIN estatus ON cAm.estatus = estatus.estatus 
															 LEFT JOIN catDelegacionesOficinasTransporte ctd ON ctd.idOficinaTransporte = cAm.idDelegacion
															 where cAm.estatus=1 AND cAm.transito = @corp
                                                            ORDER BY IdAgenciaMinisterio;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporation ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatalogModel agencia = new CatalogModel();
                            agencia.value = reader["IdAgenciaMinisterio"].ToString();
                            agencia.text = reader["NombreAgencia"].ToString();
                            ListaAgencias.Add(agencia);
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
            return ListaAgencias;


        }

        public List<CatalogModel> ObtenerAgencias(int corporation)
        {
            //
            List<CatalogModel> ListaAgencias = new List<CatalogModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT cAm.*, estatus.estatusdesc,ctd.nombreOficina
                                                              FROM catAgenciasMinisterio cAm
                                                             left JOIN estatus ON cAm.estatus = estatus.estatus 
															 LEFT JOIN catDelegacionesOficinasTransporte ctd ON ctd.idOficinaTransporte = cAm.idDelegacion	
                                                            WHERE cAm.transito = @corp
                                                            ORDER BY IdAgenciaMinisterio;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporation ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatalogModel agencia = new CatalogModel();
                            agencia.value = reader["IdAgenciaMinisterio"].ToString();
                            agencia.text = reader["NombreAgencia"].ToString();
                            ListaAgencias.Add(agencia);
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
            return ListaAgencias;
        }
		
		public List<CatalogModel> GetCatConceptoInfraccion(int corp)
        {
			List<CatalogModel> ListaAgencias = new List<CatalogModel>();
            var corporation = corp < 2 ? 1 : corp;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"SELECT *
                                                              FROM CatConceptoInfraccion 
                                                              where transito = @corp and estatus=1
                                                              ORDER BY idConcepto;", connection);
					command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CatalogModel agencia = new CatalogModel();
							agencia.value = reader["idConcepto"].ToString();
							agencia.text = reader["concepto"].ToString();
							ListaAgencias.Add(agencia);
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
			return ListaAgencias;
		}
		
		public List<CatalogModel> GetcatSubConceptoInfraccion(int corp)
		{
			List<CatalogModel> ListaAgencias = new List<CatalogModel>();
            var corporation = corp < 2 ? 1 : corp;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"SELECT *
                                                              FROM catSubConceptoInfraccion 
                                                                where transito = @corp and estatus=1
                                                              ORDER BY idSubConcepto;", connection);
					command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							CatalogModel agencia = new CatalogModel();
							agencia.value = reader["idSubConcepto"].ToString();
							agencia.text = reader["subConcepto"].ToString();
                            agencia.aux = reader["idConcepto"].ToString();

                            ListaAgencias.Add(agencia);
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
			return ListaAgencias;
		}

        public List<CatalogModel> GetAllPuestosActivos()
        {
            var result = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT idPuesto, nombrePuesto FROM catPuestos WHERE estatus = 1\r\n", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var cat = new CatalogModel();

                            cat.value = reader["idPuesto"].ToString();
                            cat.text = reader["nombrePuesto"].ToString();

                            result.Add(cat);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                }
            }
            return result;
        }
        public List<AdminCatalogosModel> ObtieneListaCatalogos()
        {
            //
            List<AdminCatalogosModel> listCatalogos = new List<AdminCatalogosModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"Select * from Catalogos", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            AdminCatalogosModel catalogo = new AdminCatalogosModel();
                            catalogo.idCatalogo = Convert.ToInt32(reader["idCatalogo"].ToString());
                            catalogo.catalogo = reader["catalogo"].ToString();

                            listCatalogos.Add(catalogo);

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
            return listCatalogos;
        }

        public List<DependenciasModel> ObtieneTodasCorporaciones()
        {
            //
            List<DependenciasModel> dependencias = new List<DependenciasModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"Select catCorp.*, e.estatusDesc from CatCorporaciones catCorp
                                                            LEFT JOIN estatus AS e ON e.estatus = catCorp.estatus
                                                            WHERE catCorp.estatus = 1
                                                            ", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            DependenciasModel dependencia = new DependenciasModel();
                            dependencia.IdDependencia = Convert.ToInt32(reader["IdCorporacion"].ToString());
                            dependencia.NombreDependencia = reader["Corporacion"].ToString();
                            dependencia.estatusDesc = reader["estatusDesc"].ToString();

                            dependencias.Add(dependencia);

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
            return dependencias;


        }
    }
}
