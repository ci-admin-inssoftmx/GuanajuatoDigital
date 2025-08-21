namespace GuanajuatoAdminUsuarios.Entity
{
    public class Vehiculo
    {
        public int IdVehiculo { get; set; }
        public string Placa { get; set; }
        public string Serie { get; set; }
        public int? IdMarca { get; set; }
        public int? IdSubmarca { get; set; }
        public int? IdColor { get; set; }
        public int? IdTipoVehiculo { get; set; }
        public int IdPersona {  get; set; }
    }
}
