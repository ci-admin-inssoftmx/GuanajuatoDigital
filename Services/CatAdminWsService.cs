using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatAdminWsService : ICatAdminWsService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatAdminWsService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<AppSettingsModel> ObtenerWebServices()
        {
            //
            List<AppSettingsModel> ListaServices = new List<AppSettingsModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT * FROM serviceAppSettings WHERE esCatalogo = 1;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            AppSettingsModel service = new AppSettingsModel();
                            service.id = Convert.ToInt32(reader["Id"].ToString());
                            service.SettingName = reader["SettingName"].ToString();
                            service.SettingValue = reader["SettingValue"].ToString();
                            service.IsActive = (bool)reader["IsActive"];

                            ListaServices.Add(service);

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
            return ListaServices;


        }
        public List<AppSettingsModel> ObtenerWebServices(int corp)
        {
            //
            List<AppSettingsModel> ListaServices = new List<AppSettingsModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT * FROM serviceAppSettings WHERE esCatalogo = 1 and transito = @corp;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@corp", corp);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            AppSettingsModel service = new AppSettingsModel();
                            service.id = Convert.ToInt32(reader["Id"].ToString());
                            service.SettingName = reader["SettingName"].ToString();
                            service.SettingValue = reader["SettingValue"].ToString();
                            service.IsActive = (bool)reader["IsActive"];

                            ListaServices.Add(service);

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
            return ListaServices;


        }

        public AppSettingsModel GetServiceById(int idService)
        {
            
            AppSettingsModel service = new AppSettingsModel();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT * FROM serviceAppSettings WHERE Id = @idService;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idService", idService);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            service.id = Convert.ToInt32(reader["Id"].ToString());
                            service.SettingName = reader["SettingName"].ToString();
                            service.SettingValue = reader["SettingValue"].ToString();
                            service.IsActive = (bool)reader["IsActive"];


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
            return service;


        }
        public int EditarService(AppSettingsModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand(@"Update serviceAppSettings 
                                    set SettingName=@SettingName,
                                        SettingValue = @SettingValue,
                                        IsActive = @IsActive
                                       WHERE Id = @idService", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idService", SqlDbType.Int)).Value = model.id;
                    sqlCommand.Parameters.Add(new SqlParameter("@SettingName", SqlDbType.NVarChar)).Value = model.SettingName;
                    sqlCommand.Parameters.Add(new SqlParameter("@SettingValue", SqlDbType.NVarChar)).Value = model.SettingValue;
                    sqlCommand.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit)).Value = model.IsActive;
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

        public int CrearService(AppSettingsModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand(@"INSERT INTO serviceAppSettings (SettingName, SettingValue, IsActive,esCatalogo)
                                        VALUES (@SettingName, @SettingValue, @IsActive,1);
                                        ", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@SettingName", SqlDbType.NVarChar)).Value = model.SettingName;
                    sqlCommand.Parameters.Add(new SqlParameter("@SettingValue", SqlDbType.NVarChar)).Value = model.SettingValue;
                    sqlCommand.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit)).Value = 1;
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
        public int CrearService(AppSettingsModel model,int corp)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand(@"INSERT INTO serviceAppSettings (SettingName, SettingValue, IsActive,esCatalogo,transito)
                                        VALUES (@SettingName, @SettingValue, @IsActive,1,@corp);
                                        ", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@SettingName", SqlDbType.NVarChar)).Value = model.SettingName;
                    sqlCommand.Parameters.Add(new SqlParameter("@SettingValue", SqlDbType.NVarChar)).Value = model.SettingValue;
                    sqlCommand.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@corp", SqlDbType.Bit)).Value = corp;
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

    }
}
