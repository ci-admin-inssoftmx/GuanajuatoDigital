using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Helpers
{
    public class UserSession
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const int _adminAutorizacionCode = 10001;

        public UserSession(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public int GetDependenciaId() => _httpContextAccessor.HttpContext.Session.GetInt32("IdDependencia").Value;

        public int GetCorporacionId()
        {
            var dependenciaId = GetDependenciaId();
            return dependenciaId < 2 ? 1 : dependenciaId;
        }

        public int[] GetAutorizaciones()
        {
            string value = _httpContextAccessor.HttpContext.Session.GetString("Autorizaciones");
            return string.IsNullOrWhiteSpace(value) 
                ? Array.Empty<int>() 
                : Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(value);
        }

        public bool IsAdmin() => GetAutorizaciones().Contains(_adminAutorizacionCode);
        
        public string  GetNombreUsuario() => _httpContextAccessor.HttpContext.User.FindFirst(CustomClaims.Nombre).Value;

        public int GetUsuarioId() => Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirst(CustomClaims.IdUsuario).Value);

        public int GetOficinaDelegacionId() => Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirst(CustomClaims.OficinaDelegacion).Value);
    }
}
