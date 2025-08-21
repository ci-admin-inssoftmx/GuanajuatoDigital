using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class InfraccionesResumen
    {
        public int IdInfraccion { get; set; }
        public string conductor { get; set; }
        public string numeroLicencia { get; set; }
        public string CURP { get; set; }
        public string folioInfraccion { get; set; }
        public DateTime fechaInfraccion { get; set; }
        public string estatusInfraccion { get; set; }
        public string nombreOficial { get; set; }
        public string municipio { get; set; }
        public string color { get; set; }
        public string marcaVehiculo { get; set; }
        public string nombreSubmarca {  get; set; }
        public string placas { get; set; }
        public string modelo { get; set; }
        public string serie { get; set; }
        public string tarjeta { get; set; }
        public string vigenciaTarjeta { get; set; } 
        
    }
}
