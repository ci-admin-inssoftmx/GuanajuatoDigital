using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatAdminWsService
    {
        List<AppSettingsModel> ObtenerWebServices();
        List<AppSettingsModel> ObtenerWebServices(int corp);
        AppSettingsModel GetServiceById(int idService);
        public int CrearService(AppSettingsModel model);
        public int CrearService(AppSettingsModel model,int corp);
        public int EditarService(AppSettingsModel model);

    }
}
