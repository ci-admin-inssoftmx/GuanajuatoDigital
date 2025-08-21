/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

using GuanajuatoAdminUsuarios.Models;

namespace GuanajuatoAdminUsuarios.Data
{
    public class ConexionBD
    {
        private readonly IConfiguration _configuracion;
        public string CadenaConexion { get; set; }

        public ConexionBD(IConfiguration configuracion)
        {
            _configuracion = configuracion;
            CadenaConexion = _configuracion.GetConnectionString("ConexionDefault");
        }
       
    }
}*/
