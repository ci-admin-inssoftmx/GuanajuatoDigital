using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.RESTModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using static GuanajuatoAdminUsuarios.Framework.Catalogs.CatWebServicesEnumerator;

namespace GuanajuatoAdminUsuarios.Services
{
    public class RepuveService : IRepuveService
    {
        private readonly IApiClientDatabaseService _clientDatabaseService;
        private readonly IAppSettingsService _appSettingsService;
        public RepuveService(IApiClientDatabaseService clientDatabaseService, IAppSettingsService appSettingsService)
        {
            _clientDatabaseService = clientDatabaseService;
            _appSettingsService = appSettingsService;
        }


        public List<RepuveConsgralResponseModel> ConsultaGeneral(RepuveConsgralRequestModel model,int corp)
        {
            model.placa = model.placa?.ToUpper();
            model.niv = model.niv?.ToUpper();
            var token = _appSettingsService.GetAppSetting("RepuveToken",corp).SettingValue;
            model.token = token;
            var response = _clientDatabaseService.HttpPost<List<RepuveConsgralResponseModel>, RepuveConsgralRequestModel, RepuveWebServicesEnum>(model, (int)RepuveWebServicesEnum.RepuveConsgral, corp);
            return response;
        }

		public async Task<List<RepuveRoboModel>> ConsultaRobo(RepuveConsgralRequestModel model)
		{
            var corp = 1;
            model.placa = model.placa?.ToUpper();
            model.niv = model.niv?.ToUpper();
			var token = _appSettingsService.GetAppSetting("RepuveToken").SettingValue;
			model.token = token;
			var response = _clientDatabaseService.HttpPost<List<RepuveRoboModel>, RepuveConsgralRequestModel, RepuveWebServicesEnum>(model, (int)RepuveWebServicesEnum.RepuveConsrobo,corp);
			return response;
		}
	}
}
