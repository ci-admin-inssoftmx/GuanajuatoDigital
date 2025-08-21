namespace GuanajuatoAdminUsuarios.RESTModels
{
    public class ConsultarDocumentoResponseModel
    {
        public class MTConsultarDocumentoRes
        {
            public Result result { get; set; }
            public edocpago e_doc_pago { get; set; }
        }

        public class edocpago
        {
      public string nro_documento {get; set;}
      public string fecha_pago { get; set;}
      public string hora_pago {get; set;}
      public string cantidad {get; set;}
      public int importe { get; set;}
      public string moneda {get; set;}
      public string entidad {get; set;}
      public string oficina {get; set;}
      public string id_medio {get; set;}
      public string id_linea {get; set;}
      public string fol_confirmacion {get; set;}
      public string UUID {get; set;}
      public string tp_factura {get; set;}
      public string Fecha_UUID {get; set;}
      public string estatus_fel { get; set; }
        }

        public class Result
        {
            public string NUM_DOCUMENTO { get; set; }
            public string FOL_MULTA { get; set; }
            public string FECHA_PAGO { get; set; }
            public string OFICINA { get; set; }
            public double IMPORTE { get; set; }
            public string CONCEPTO { get; set; }
            public string WTYPE { get; set; }
            public string WMESSAGE { get; set; }
        }

        public class RootConsultarDocumentoResponse
        {
            public MTConsultarDocumentoRes MT_ConsultarDocumento_res { get; set; }
        }
    }
}
