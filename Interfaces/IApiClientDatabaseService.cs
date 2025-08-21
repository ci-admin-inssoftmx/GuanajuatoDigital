using GuanajuatoAdminUsuarios.Framework.Catalogs;
using static GuanajuatoAdminUsuarios.Framework.Catalogs.CatWebServicesEnumerator;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IApiClientDatabaseService
    {
        public TResponse HttpGet<TResponse, TRequest, TEnum>(TRequest requestModel, int service);
        public TResponse HttpPost<TResponse, TRequest, TEnum>(TRequest requestModel, int service, int corp);
    }
}
