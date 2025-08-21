namespace GuanajuatoAdminUsuarios.Common
{
    public static class StringFunctions
    {
        public static string CleanNullString(string input) => input?.Replace("NULL", "").Trim();
    }
}
