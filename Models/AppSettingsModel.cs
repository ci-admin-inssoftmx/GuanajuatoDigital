namespace GuanajuatoAdminUsuarios.Models
{
    public class AppSettingsModel
    {
        public int id { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        public bool IsActive { get; set; }
        public string NuevoEndPoint { get; set; }

    }
}
