using GuanajuatoAdminUsuarios.Models;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IAppSettingsService
    {
        public AppSettingsModel GetAppSetting(string settingName);
		public AppSettingsModel GetAppSetting(string settingName,int corp);


		public bool VerificarActivo(string endPointName, int corp);

    }
}
