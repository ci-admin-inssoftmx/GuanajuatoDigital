using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class IngresoVehiculosModel
    {
        public string serie { get; set; }
        public DateTime fechaIngreso { get; set; }
        public TimeSpan? horaIngreso { get; set; }
        public string folioInventario { get; set; }
        public int? idDeposito { get; set; }
        public int idVehiculo { get; set; }

        public int? idMarca { get; set; }
        public string placa { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public string motor { get; set; }
        public string tipoVehiculo { get; set; }
        public string color { get; set; }
        public DateTime fechaServicio { get; set; }
        public string numeroEconomico { get; set; }
        public string Propietario { get; set; }
        public string Solicitante { get; set; }
        public string evento { get; set; }
        public string propietarioGrua { get; set; }
        public string tramo { get; set; }
        public string carretera { get; set; }
        public string calle { get; set; }
        public string kilometro { get; set; }
        public string colonia { get; set; }
        public string numero { get; set; }
        public string interseccion { get; set; }
        public string municipio { get; set; }
        public string numeroEconomicoVehiculo { get; set; }

        
        public DateTime fechaSolicitud { get; set; }
        public DateTime fechaFinal { get; set; }


        public string FechaServicioFormateada => fechaServicio.ToString("dd/MM/yyyy");
        public string HoraServicioFormateada => fechaServicio.ToString("HH:mm");




    }
}
