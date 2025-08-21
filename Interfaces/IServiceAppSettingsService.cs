using GuanajuatoAdminUsuarios.Entity;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IServiceAppSettingsService
    {
        ServiceAppSettings GetSettingbyName(string SettingName);
    }
}
