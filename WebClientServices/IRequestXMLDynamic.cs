namespace GuanajuatoAdminUsuarios.WebClientServices
{
    public interface IRequestXMLDynamic <T> where T : class
    {
        string GetXMLRequest(T model);
    }
}
