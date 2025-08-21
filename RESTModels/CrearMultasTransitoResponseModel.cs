using System;


namespace GuanajuatoAdminUsuarios.RESTModels
{
    public class CrearMultasTransitoResponseModel
    {
        public CrearMultasTransitoChild MT_CrearMultasTransito_res { get; set; }
        public string MensajeError { get; internal set; }
    }
    public class CrearMultasTransitoChild
    {
        public string BUSINESSPARTNER { get; set; }
        public string ZTYPE { get; set; }
        public string ZMESSAGE { get; set; }
        public string CUENTAnmbb { get; set; }
        public string OBJETO { get; set; }
        public string DOCUMENTNUMBER { get; set; }
        public string MensajeError { get; set; }

    }
}
