namespace GuanajuatoAdminUsuarios.RESTModels
{
    public class RepuveConsgralRequestModel
    {

        public RepuveConsgralRequestModel( string placa="", string niv = "")
		{
			this.placa = placa;
			this.niv = niv;
		}

		public string token { get; set; }
        public string placa { get; set; }
        public string niv { get; set; }
    }
}
