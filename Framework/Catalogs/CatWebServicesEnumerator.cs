using System.ComponentModel;

namespace GuanajuatoAdminUsuarios.Framework.Catalogs
{
    public class CatWebServicesEnumerator
    {
        public enum RepuveWebServicesEnum : int
        {
            [Description("RepuveConsgral")]
            RepuveConsgral = 1,
            [Description("RepuveConsrobo")]
            RepuveConsrobo = 2
        }
    }
}
