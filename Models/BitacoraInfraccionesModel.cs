using Microsoft.VisualBasic;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class BitacoraInfraccionesModel
    {

        public string folio { get; set; }
        public string Cambio { get; set; }
        public string SittegUsuario { get; set; }
        public string DescripcionSitteg { get; set; }

        public string documento { get; set; } = "-";
        public string monto { get; set; }="-";
         public string operacion { get; set; }
        public string operaciondesc { get
            {
                var t =  "Error";

                return t;


            } }
         public string fecha { get; set; }
        public string hora { get; set; }
        public string ip { get; set; }
        public string nombre { get; set; }
        public string desc { get { 
                var t= "Error";

                return t;
            
            
            } }

    }
}
