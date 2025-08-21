using Microsoft.AspNetCore.Http;
using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class AsignacionGruaModel
    {



        public string NombreArchivo { get; set; }
        public string PathArchivo { get; set; }
        public string dateBusqueda { get; set; } 
        public string FolioSolicitud { get; set; }
        public DateTime fecha { get; set; }
        public DateTime fechaSolicitud { get; set; }
        public string fechaSolicitudFormateada => fechaSolicitud.ToString("dd/MM/yyyy");
        public string concesionario { get; set; }

        public string Grua { get; set; }
        
        public string Solicitante { get; set; }
        public int idSolicitud { get; set; }
        public string vehiculoCalle { get; set; }
        public string vehiculoColonia { get; set; }
        public string carretera { get; set; }
        public string municipio { get; set; }
        public string nombreEntidad { get; set; }
        public string tipoUsuario { get; set; }
        public string nombreOficial { get; set; }
        public string apellidoPaternoOficial { get; set; }
        public string apellidoMaternoOficial { get; set; }




        public string Ubicacion
        {
            get
            {
                return nombreEntidad + "    " +
                       municipio + "<br /><br />" +"    "+
                       vehiculoColonia + "    " +
                       vehiculoCalle + "<br /><br />";
            }
        }
        public string Oficial
        {
            get
            {
                return nombreOficial + "  " + apellidoPaternoOficial + "  " + apellidoMaternoOficial;



            }
        }


        public int IdPersona { get; set; }
        public int IdVehiculo { get; set; }
        public int IdMarcaVehiculo { get; set; }
        public int IdSubmarca { get; set; }
        public int IdEntidad { get; set; }
        public int IdColor { get; set; }
        public int IdTipoVehiculo { get; set; }
        public int IdCatTipoServicio { get; set; }
        public string Marca { get; set; }
        public string Submarca { get; set; }
        public string Modelo { get; set; }
        public string Placa { get; set; }
        public string Tarjeta { get; set; }
        public DateTime VigenciaTarjeta { get; set; }
        public string Serie { get; set; }
        public string Color { get; set; }
        public string TipoServicio { get; set; }
        public string EntidadRegistro { get; set; }
        public string TipoVehiculo { get; set; }
        public string Propietario { get; set; }
        public string Motor { get; set; }
        public string NumeroEconomico { get; set; }
        public int idPropietarioGrua { get; set; }
        public int idPension { get; set; }
        public int idTramoUbicacion { get; set; }
        public string kilometro { get; set; }
        public int idInfraccion { get; set; }
        public int? idVehiculo { get; set; }
        public string folioInfraccion { get; set; }
        public DateTime fechaInfraccion { get; set; }
        
       public int horaInfraccion { get; set; }
        public DateTime fechaYHoraInfraccion
        {
            get
            {
                if (horaInfraccion < 0 || horaInfraccion > 2359)
                {
                    throw new ArgumentOutOfRangeException(nameof(horaInfraccion), "La hora debe estar en formato HHmm y en el rango de 0000 a 2359.");
                }

                int horas = horaInfraccion / 100;
                int minutos = horaInfraccion % 100;

                if (minutos < 0 || minutos > 59)
                {
                    throw new ArgumentOutOfRangeException(nameof(horaInfraccion), "Los minutos deben estar entre 0 y 59.");
                }

                return new DateTime(fechaInfraccion.Year, fechaInfraccion.Month, fechaInfraccion.Day,
                                     horas, minutos, 0);
            }
        }
      
        public string CURP { get; set; }
        public string RFC { get; set; }
        public string observaciones { get; set; }
        public string numeroInventario { get; set; }
        public string inventarios { get; set; }
        public string Nombreinventarios { get; set; }
        public int IdDeposito { get; set; }
		public int estatusSolicitud { get; set; }

		public IFormFile MyFile { get; set; }


    }
}
