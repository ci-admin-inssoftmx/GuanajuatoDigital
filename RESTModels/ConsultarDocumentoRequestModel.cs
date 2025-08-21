namespace GuanajuatoAdminUsuarios.RESTModels
{
    public class ConsultarDocumentoRequestModel
    {
        public class MTConsultaDocumento
        {
            public string PROCESO { get; set; }
            public string DOCUMENTO { get; set; }

            public string USUARIO { get; set; }
            public string PASSWORD { get; set; }
        }

        public class RootConsultarDocumentoRequest
        {
            public MTConsultaDocumento MT_Consulta_documento { get; set; }
        }
    }
}
