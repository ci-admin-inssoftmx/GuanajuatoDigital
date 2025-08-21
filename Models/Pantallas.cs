using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public static class VistaConfig
    {
        public static readonly Dictionary<int, string> VistaID_Nombre = new Dictionary<int, string>
    {
        { 105, "~/Views/Infracciones/Index.cshtml" },
        { 106, "~/Views/CapturaAccidentes/Index.cshtml" },
        { 30, "~/Views/CancelarInfracciones/CancelarInfraccion+" +
                ".cshtml" },
        // Agrega más vistas según corresponda
    };
    }

}
