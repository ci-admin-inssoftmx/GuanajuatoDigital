using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GuanajuatoAdminUsuarios.Models
{
    [AttributeUsage(AttributeTargets.Property,
Inherited = false,
AllowMultiple = false)]
    internal sealed class OptionalAttribute : Attribute { }
    public class MenuViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        [Optional]
        public string Accion { get; set; }
        [Optional]
        public string Controlador { get; set; }
        public int Nivel { get; set; }
        public int Orden { get; set; }

    }
}
