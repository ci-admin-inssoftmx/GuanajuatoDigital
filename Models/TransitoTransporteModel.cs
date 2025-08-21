using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Collections.Generic;
using System.Runtime;

namespace GuanajuatoAdminUsuarios.Models
{
    public class TransitoTransporteModel : EntityModel
	{

        public int transito { get; set; }
        public int cons { get; set; }
        public string nombreEstatus { get; set; }

        #region Depositos



        public int IdDeposito { get; set; }

        public int IdSolicitud { get; set; }

        public int IdDelegacion { get; set; }

        public int IdMarca { get; set; }

        public int IdSubmarca { get; set; }

        public int IdPension { get; set; }

        public int IdTramo { get; set; }

        public int IdColor { get; set; }

        public string Serie { get; set; }

        public string Placa { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaIngreso { get; set; }

        public string FechaIngresoDesc { get { return FechaIngreso ==DateTime.MinValue?"": FechaIngreso.ToString("dd/MM/yyyy"); } }

        public DateTime FechaLiberacion { get; set; }

        public string FechaLiberacionDesc { get { return FechaLiberacion == DateTime.MinValue ? "" : FechaLiberacion.ToString("dd/MM/yyyy"); } }


        public string Folio { get; set; }

        public string Km { get; set; }

        public int Liberado { get; set; }

        public int DepositoEstatus { get; set; }

        public string Autoriza { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public int ActualizadoPor { get; set; }

        public int IdDependenciaGenera { get; set; }

        public int IdDependenciaTransito { get; set; }

        public int IdDependenciaNoTransito { get; set; }

        //public int Estatus { get; set; }
        #endregion

        #region Solicitudes

        public string FolioSolicitud { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaSolicitud { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]

        public DateTime FechaArribo { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]

        public DateTime FechaInicio { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]

        public DateTime FechaFinal { get; set; }

        
        public string solicitanteNombre { get; set; }

        public string solicitanteAp { get; set; }

        public string solicitanteAm { get; set; }
		public string solicitanteColonia { get; set; }
		public string solicitanteCalle { get; set; }
		public string solicitanteNumero { get; set; }
        
        public string solicitanteMunicipio { get; set; }
        public string solicitanteEntidad { get; set; }
        

        public int IdVehiculo { get; set; }

        public int IdInfraccion { get; set; }
        public int SolicitudEstatus { get; set; }
        public int? BanderaTransito { get; set; }

        #endregion

        #region Grua

        public int IdGrua { get; set; }

        //public string Grua { get; set; }

        #endregion

        #region Infracciones

        public string FolioInfraccion { get; set; }

        #endregion


        #region Vehiculo

        public string serie { get; set; }

        public string tarjeta { get; set; }

        public string vigenciaTarjeta { get; set; }

        public string marca { get; set; }

        public string submarca { get; set; }

        public string tipoVehiculo { get; set; }

        public string modelo { get; set; }

        public string color { get; set; }

        public string entidadRegistro { get; set; }

        public string tipoServicio { get; set; }

        public string propietario { get; set; }

        public string numeroEconomico { get; set; }

        public int idPersonaFisica { get; set; }

        public int idPersonaMoral { get; set; }

        public int EstatusVehiculo { get; set; }
        #endregion

        #region Pension
        public string pension { get; set; }
        #endregion

        #region Dependencia
        public int IdDependencia { get; set; }

        public string NombreDependencia { get; set; }

        #endregion


        public string marcaVehiculo { get; set; }

        public string nombreSubmarca { get; set; }

        public string delegacion { get; set; }

        public string Color { get; set; }
        public string tipoUsuario { get; set; }

        


        public string tramo { get; set; }
        public string evento { get; set; }
        public string motivoAsignacion { get; set; }

        public string oficialNombre { get; set; }

        public string oficialApellidoPaterno { get; set; }
        public string oficialApellidoMaterno { get; set; }
        public string calle { get; set; }
        public string colonia { get; set; }
        public string interseccion { get; set; }
        public string carretera { get; set; }
        public string numero { get; set; }

        public string municipio { get; set; }
        public string tipoGrua { get; set; }
        public string placasGrua { get; set; }
        public string operador { get; set; }
        public int arrastre { get; set; }
        public int abanderamiento { get; set; }
        public int salvamento { get; set; }
        public int minutosManiobra { get; set; }


        #region Concesionario
        public int IdConcesionario { get; set; }
        public string Concesionario { get; set; }
        public string estatusSolicitud { get; set; }

        

        #endregion
        //p.FolioSolicitud + " " + p.FechaSolicitud + " " + p.FolioInfraccion
        public string fullSolicitudfolioInfraccion
        {
            get
            {
                return @"Fecha: " + ((FechaSolicitud.Year <= 1900) ? "" : FechaSolicitud.ToString("dd/MM/yyyy")) + "\r\n\n " +
                "Solicitud: " + FolioSolicitud + "\r\n\n " +
                "Infracción: " + FolioInfraccion + "\r\n\n ";
               
            }
         }
        //p => p.Placa + " " + p.submarca + " " + p.modelo
        public string fullVehiculo
        {
            get
            {
                return @"Placas: " + Placa + "\r\n\n " +
                "Prop: " + propietario + "\r\n\n " +
                "Descr: " + marcaVehiculo + " " + nombreSubmarca + " " + modelo + "\r\n\n " +
                tipoVehiculo;
            }
        }
        public string UbicacionVehiculo
        {
            get
            {
                return @"Calle: " + calle + " " + " " + " " + "Municipio: " + municipio + "\r\n\n " +
                    "Número: " + numero + " " + " " + " " + "Pension: " + pension + "\r\n\n " +
                "Colonia: " + colonia + " " + " " + " " + "Intersección: " + interseccion + "\r\n\n " +
                "Carretera: " + carretera + " " + " " + " " + "Km.: " + Km + "\r\n\n " +
               "Tramo: " + tramo;

            }
        }

        public string DatosGrua
        {
            get
            {
                var servicios = new List<string>();

                if (arrastre == 1)
                {
                    servicios.Add("Arrastre");
                }
                if (abanderamiento == 1)
                {
                    servicios.Add("Abanderamiento");
                }
                if (salvamento == 1)
                {
                    servicios.Add("Salvamento");
                }
                if (servicios.Count > 0)
                {
                    return @"No. Económico: " + numeroEconomico + "\r\n\n " +
                        "Tipo de grúa: " + tipoGrua + "\r\n\n " +
                        "Placas: " + placasGrua + "\r\n\n " +
                        "Operador: " + operador + "\r\n\n " +
                        "Servicios: " + string.Join(", ", servicios);
                }
                else
                {
                    return @"No. Económico: " + numeroEconomico + "\r\n\n " +
                        "Tipo de grúa: " + tipoGrua + "\r\n\n " +
                        "Placas: " + placasGrua + "\r\n\n " +
                        "Operador: " + operador;
                }
            }
        }

        
              public string Tiempos
        {
            get
            {
                return @"Hora arribo: " + FechaArribo + "\r\n\n " +
                    "Hora Inicio: " + FechaInicio + "\r\n\n " +
                "Hora Final: " + FechaFinal + "\r\n\n " +
                "Minutos de maniobra: " + minutosManiobra;
               
            }
        }
        public string fullDependencia
        {
            get
            {
                return @"Estatus: " + estatusSolicitud + "\r\n\n";
                                        //NombreDependencia;
            }
        }
        public string propietarioNombre { get; set; }
        public string propietarioApellidoPaterno{ get; set; }
        public string propietarioApellidoMaterno { get; set; }
		public string CURP { get; set; }

		public string fullPropietario
        {
            get
            {
                return propietarioNombre + " " + propietarioApellidoPaterno + " " + propietarioApellidoMaterno;
            }
        }

        public string fullOficial
        {
            get
            {
                return oficialNombre + " " + oficialApellidoPaterno + " " + oficialApellidoMaterno;
            }
        }

        public int total { get; set; }

    }
}
