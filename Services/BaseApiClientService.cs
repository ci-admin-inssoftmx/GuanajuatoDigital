using GuanajuatoAdminUsuarios.Interfaces;
using SITTEG.APIClientHelper.Client;
using SITTEG.APIClientInfrastructure;
using SITTEG.APIClientInfrastructure.Client;

namespace GuanajuatoAdminUsuarios.Services
{
    public class BaseApiClientService
    {
        private readonly IApiClient _apiClient;
        protected readonly IGenericClient _apiGenericClient;

        public BaseApiClientService(IGenericClient apiGenericClient, string baseUri)
        {
            HttpClientInstance.BaseUri = baseUri;
            _apiClient = new ApiClient(HttpClientInstance.Instance);
        }
    }
}
