using System;
using static GuanajuatoAdminUsuarios.RESTModels.AnulacionDocumentoResponseModel;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;

namespace GuanajuatoAdminUsuarios.RESTModels
{
    public class AnulacionDocumentoResponseModel
    {

        public class MTAnulacionDocumentoRes
        {
            public resultModel result { get; set;}
        }
         
    }  
    public class resultModel
    {
        public string WTYPE { get; set; }
        public string WMESSAGE { get; set; }
        public string DOCTO_CANCEL { get; set; }
    }

    public class RootAnulacionDocumentoResponse
    {
        public MTAnulacionDocumentoRes MT_AnulacionDocumento_res { get; set; }
    }
}
