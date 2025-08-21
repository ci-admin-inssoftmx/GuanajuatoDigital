using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.WebClientServices
{
    public interface IRequestDynamic<T, D> where T : class where D : class
    {
        Task<D> EncryptionService(T modelRequest,D modelResponse, string urlName, string requestXMLName);
    }
}
