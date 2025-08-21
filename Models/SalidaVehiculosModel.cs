using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class SalidaVehiculosModel
    {
        public int? idDeposito { get; set; }
        public int idVehiculo { get; set; }
        public string serie { get; set; }
        public string tipoVehiculo { get; set; }
        public string modelo { get; set; }
        public string solicitante { get; set; }
        public string evento { get; set; }
        public string propietarioGrua { get; set; }
        public DateTime fechaIngreso { get; set; }
        public string fechaIngresoFormateada => fechaIngreso.ToString("dd/MM/yyyy");
        public bool esExterno { get; set; }

        public string folioInventario { get; set; }
        public int? idMarca { get; set; }
        
        public int idSubMarca { get; set; }
        public int idColor { get; set; }
        public int idPropietario { get; set; }
        public int idPension { get; set; }
        public int diasResguardo { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio.")]

        public float costoDeposito { get; set; }
        public string marca { get; set; }
        
                    public string oficio { get; set; }
        public DateTime fechaOficio { get; set; }
        public string autorizaSalida { get; set; }

        public string submarca { get; set; }
        public string color { get; set; }
        public string propietario { get; set; }
        public string pension { get; set; }
        public string placa { get; set; }
        public DateTime fechaSolicitud { get; set; }
        public DateTime fechaFinal { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio.")]
        public DateTime? fechaSalida { get; set; } 
        public string tramo { get; set; }
        public string carretera { get; set; }
        public string kilometro { get; set; }
        public string colonia { get; set; }
        public string calle { get; set; }
        public string numero { get; set; }
        public string municipio { get; set; }
        public string grua { get; set; }
        public string tipoGrua { get; set; }
        public string interseccion { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio.")]

        public string recibe { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio.")]

        public string entrega { get; set; }
        public string observaciones { get; set; }
        public string enviaVehiculo { get; set; }
         public string motivoIngreso { get; set; }

        public float costoTotalPorGrua{ get; set; }
        public float costoTotalTodasGruas { get; set; }

        


    }

}
