namespace GuanajuatoAdminUsuarios.Models
{
    public class CostosServicioModel
    {
        public int idGrua { get; set; }
        public int idDeposito { get; set; }

        public int abanderamiento { get; set; }
        public int arrastre { get; set; }
        public int salvamento { get; set; }

        public string grua { get; set; }
        public string tipoGrua { get; set; }
        public float costoAbanderamiento { get; set; }
        public float costoSalvamento { get; set; }
        public float costoArrastre { get; set; }
        public float costoDeposito { get; set; }
        public float costoTotalPorGrua { get; set; }
        public float costoBanderazo { get; set; }     
        public float costoTotal { get; set; }

    }
}
