using System;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoRequestModel;

namespace GuanajuatoAdminUsuarios.RESTModels
{
    public class AnulacionDocumentoRequestModel
    {
        public class MT_Consulta_documento { 
            public string DOCUMENTO { get; set; }
            public string USUARIO { get; set; }
            public string PASSWORD { get; set; }
        }

        public class RootAnulacionDocumentoRequest
        {
            public MT_Consulta_documento MT_Consulta_documento { get; set; }
        }
    }
}
