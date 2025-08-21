using GuanajuatoAdminUsuarios.ExternalServices.Interfaces;
using GuanajuatoAdminUsuarios.Models.Accidentes;
using GuanajuatoAdminUsuarios.Util;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.ExternalServices.Services
{
    public class CadService : ICadService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClient;

        public CadService(IConfiguration configuration, IHttpClientFactory httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<(bool, string, string)> FolioCadAsync(string folio, string municipio)
        {
            try
            {
                string UrlHost = _configuration.GetSection("AppSettings").GetSection("UrlCyberW").Value;
                string urlRequest = $"{UrlHost}{folio}/{municipio}/validate-emergency";

                var httpClient = _httpClient.CreateClient();
                HttpResponseMessage response = await httpClient.GetAsync(urlRequest);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    AccidentesFolioDto folioDto = JsonConvert.DeserializeObject<AccidentesFolioDto>(responseData);
                    if (folioDto != null)
                    {
                        return (folioDto.data.exist, null, null);
                    }
                }
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return (false, "Folio Emergencia no válido.", null);
                }
                else
                {
                    return (false, "Ocurrió un error al validar el folio de emergencia.", null);
                }
            }
            catch (HttpRequestException httpEx)
            {
                Logger.Error("Ocurrió un error al validar el folio de emergencia: " + httpEx.Message);
                return (false, "Ocurrió un error al validar el folio de emergencia.", httpEx.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Ocurrió un error al validar el folio de emergencia: " + ex.Message);
                return (false, "Ocurrió un error al validar el folio de emergencia.", ex.Message);
            }
        }
    }
}
