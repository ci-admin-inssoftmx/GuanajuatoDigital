namespace GuanajuatoAdminUsuarios.Models
{
    public class ReportesModel
    {
        public int Id { get; set; }
        public int cantidad { get; set; }
        public string tipoLicencia { get; set; }
        public string nombreOficina { get; set; }       
        public string Corporacion { get; set; }
        public string municipo { get; set; }
        public string colonia { get; set; }
        public int cantidadAccidentes { get; set; }
        public int cantidadInfracciones { get; set; }        
         public string folioInfraccion { get; set; }
        public string estatusInfraccion { get; set; }
        public int ContadorTotal { get; set; }
        public string numeroReporte { get; set; }
        public string montoCarga { get; set; }
        public string montoCamino { get; set; }
        public string montoPropietarios { get; set; }
        public string montoOtros { get; set; }
        public string montoVehiculo { get; set; } 
        public string hora { get; set; }
        public string diaSemana { get; set; }





    }
}
