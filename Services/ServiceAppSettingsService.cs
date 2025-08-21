using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using GuanajuatoAdminUsuarios.Entity;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class ServiceAppSettingsService : IServiceAppSettingsService
    {

        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public ServiceAppSettingsService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public ServiceAppSettings GetSettingbyName(string SettingName)
        {
            ServiceAppSettings settingApp = new ServiceAppSettings();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT Id,SettingName,SettingValue,IsActive FROM serviceAppSettings where UPPER(SettingName)=@SettingName and IsActive=1", connection);
                    command.Parameters.Add(new SqlParameter("@SettingName", SqlDbType.NVarChar)).Value = (object)SettingName != null ? SettingName.ToUpper() : DBNull.Value;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {

                        while (reader.Read())
                        {
                            settingApp.Id = Convert.ToInt32(reader["Id"].ToString());
                            settingApp.SettingName = reader["SettingName"].ToString();
                            settingApp.SettingValue = reader["SettingValue"].ToString();
                            settingApp.IsActive = Convert.ToBoolean(reader["IsActive"].ToString());
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
            return settingApp;
        }


    }
}
