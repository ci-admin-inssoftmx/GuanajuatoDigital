using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.ExternalServices.Interfaces
{
    public interface ICadService
    {
        Task<(bool, string, string)> FolioCadAsync(string folioEmergencia, string municipio);
    }
}
