using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class BusquedaDepositoModel
    {
        public int idDeposito { get; set; }
        public int idVehiculo { get; set; }
        public int idGrua { get; set; }
        public int idInfraccion { get; set; }

        public string placa { get; set; }
        public string serie { get; set; }

        public string folioInfraccion { get; set; }
        public DateTime fechaIngreso { get; set; }
        public DateTime fechaEvento { get; set; }
        public DateTime fechaSalida { get; set; }
        public string FechaIngresoFormateada => fechaIngreso.ToString("dd/MM/yyyy");
        public string FechaEventoFormateada => fechaEvento.ToString("dd/MM/yyyy");
        public string FechaSalidaFormateada => fechaSalida.ToString("dd/MM/yyyy");

        public string pension { get; set; }
        public string grua { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string propietario { get; set; }

        public string propietarioCompleto
        {
            get
            {
                return $"{nombre} {apellidoPaterno} {apellidoMaterno}";
            }
     
        }
    }
}
