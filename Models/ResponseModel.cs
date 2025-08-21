namespace GuanajuatoAdminUsuarios.Models
{
    public class ResponseModel
    {
        public MT_CrearMultasTransito_Res MT_CrearMultasTransito_res { get; set; }
    }

    public class MT_CrearMultasTransito_Res
    {
        public string BUSINESSPARTNER { get; set; }
        public string ZTYPE { get; set; }
        public string ZMESSAGE { get; set; }
        public string CUENTA { get; set; }
        public string OBJETO { get; set; }
        public string DOCUMENTNUMBER { get; set; }
    }
}