using System.ComponentModel.DataAnnotations;
using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class EnvioInfraccionesModel

    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime FechaInicio { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime FechaFin { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string Oficio { get; set; }
        public int IdLugarEnvio { get; set; }

        public int IdInfraccion { get; set; }
        public string folioInfraccion { get; set; }
        public string conductorCompleto { get; set; }
        public string nombreConductor { get; set; }
        public string apellidoPaternoConductor { get; set; }
        public string apellidoMaternoConductor { get; set; }
        public string propietarioCompleto { get; set; }
        public string nombrePropietario { get; set; }
        public string apellidoPaternoPropietario { get; set; }
        public string apellidoMaternoPropietario { get; set; }
        public string placas { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime fechaInfraccion { get; set; }
		public string fechaVencimiento { get; set; }
		public int DiasTranscurridos
        {
            get
            {
                return (DateTime.Today - Convert.ToDateTime(fechaVencimiento).Date).Days;
            }
        }

        public int idEstatusInfraccion { get; set; }

        public string estatusInfraccion { get; set; }
        public string nombreCompletoConductor { get; set; }

        public string nombreCompletoPropietario { get; set; }

        public string fecha { get; set; }
		
	}
}
