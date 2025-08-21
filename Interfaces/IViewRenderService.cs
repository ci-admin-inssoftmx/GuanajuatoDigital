using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}
