using System;

namespace GuanajuatoAdminUsuarios.Models.PDFModels
{
    public class InfraccionesGeneralPDFModel
    {
        public string folioInfraccion { get; set; }
        public string conductor { get; set; }
        public string propietario { get; set; }
        public DateTime fechaAplicacion { get; set; }
        public string garantia { get; set; }
        public string vehiculo { get; set; }
        public string placas { get; set; }
        public string delegacion { get; set; }
        public string estatusInfraccion { get; set; }
        public string aplicacion { get; set; }
    }
}
