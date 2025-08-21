using Microsoft.AspNetCore.Http;
using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class DatosIngresoModel
    {
        public int IdDeposito { get; set; }
        public DateTime fechaIngreso { get; set; }
        public byte[] AnexarImagen1 { get; set; }
    }

}
