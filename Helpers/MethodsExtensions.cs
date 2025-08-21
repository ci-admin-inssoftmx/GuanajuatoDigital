using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Helpers
{
    public static class MethodsExtensions
    {
        public static string Cut(this string text, int valor)
        {
            var val1 = valor + 1;
            var cut=(text ?? "").Length < val1 ? (text ?? "") : (text ?? "").Substring(0, valor);
            return cut;
        }


        public static int toInt(this string text) {

            int result = 0;

            var can = int.TryParse(text, out result);

            if (can)
                return result;
            else
                return -1;
        
        }
    }
}
