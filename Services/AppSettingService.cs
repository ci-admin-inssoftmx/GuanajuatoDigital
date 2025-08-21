using DocumentFormat.OpenXml.Office2010.CustomUI;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.Data.SqlClient;
using SITTEG.APIClientInfrastructure.Client;
using System;
using System.Data;

namespace GuanajuatoAdminUsuarios.Services
{
    public class AppSettingService :IAppSettingsService
    {

        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;

        public AppSettingService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public AppSettingsModel GetAppSetting(string settingName)
        {
            AppSettingsModel model = null;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string strQuery = @"SELECT 
                                        Id
                                        ,SettingName
                                        ,SettingValue
                                        ,IsActive
                                        FROM serviceAppSettings
                                        WHERE SettingName = @settingName";
                    SqlCommand sqlCommand = new SqlCommand(strQuery, connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@settingName", SqlDbType.NVarChar)).Value = settingName;
                    sqlCommand.CommandType = CommandType.Text;

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new AppSettingsModel();
                            model.id = Convert.ToInt32(reader["Id"]);
                            model.SettingName = Convert.ToString(reader["SettingName"]);
                            model.SettingValue = Convert.ToString(reader["SettingValue"]);
                            model.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Manejo de errores y log
                    return model = null;
                }
                finally
                {
                    connection.Close();
                }
            }
            return model;
        }

		public AppSettingsModel GetAppSetting(string settingName,int corp)
		{
            var corporation = corp < 2 ? 1 : corp;
            AppSettingsModel model = new AppSettingsModel();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string strQuery = @"SELECT 
                                        Id
                                        ,SettingName
                                        ,SettingValue
                                        ,IsActive
                                        FROM serviceAppSettings
                                        WHERE SettingName = @settingName and transito = @corp";
					SqlCommand sqlCommand = new SqlCommand(strQuery, connection);
					sqlCommand.Parameters.Add(new SqlParameter("@settingName", SqlDbType.NVarChar)).Value = settingName;
					sqlCommand.Parameters.AddWithValue("@corp", corporation);

					sqlCommand.CommandType = CommandType.Text;
                    
					using (SqlDataReader reader = sqlCommand.ExecuteReader())
					{
						if (reader.Read())
						{
							model = new AppSettingsModel();
							model.id = Convert.ToInt32(reader["Id"]);
							model.SettingName = Convert.ToString(reader["SettingName"]);
							model.SettingValue = Convert.ToString(reader["SettingValue"]);
							model.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
						}
					}
				}
				catch (SqlException ex)
				{
					// Manejo de errores y log
					return model = null;
				}
				finally
				{
					connection.Close();
				}
			}
			return model;
		}



		public bool VerificarActivo(string endPointName,int corp)
		{
			var corporation = corp < 2 ? 1 : corp;

			bool isActive = false;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT IsActive FROM serviceAppSettings WHERE SettingName = @endPointName AND transito = @corp", connection);
                    command.Parameters.Add(new SqlParameter("@endPointName", SqlDbType.NVarChar)).Value = endPointName;
					command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = corporation;

					command.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.Read())
                        {
                            isActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
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
            }

            return isActive;
        }
    }
}
