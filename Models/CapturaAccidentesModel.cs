using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CapturaAccidentesModel
    {
        public string lugarCalle { get; set; }
        public string lugarNumero { get; set; }
        public string lugarColonia { get; set; }
        public string OrdenVehiculo { get; set; }
        public string jefeOficina { get; set; }

        public int idPersonaInput { get; set; }

        public int? IdAccidente { get; set; }
        public int? IdInfAcc { get; set; }
        public int? idAccidenteCausa { get; set; }
        public int? IdAccidenteFactorOpcion { get; set; }
        public int? idEstatusReporte { get; set; }
        public int? IdInvolucrado { get; set; }

        public string NumeroReporte { get; set; }
        [Required(ErrorMessage = "-El campo Fecha es obligatorio")]
        public DateTime? Fecha { get; set; }
        public string FechaFormateada => Fecha.HasValue ? Fecha.Value.ToString("dd/MM/yyyy") : string.Empty;

        public TimeSpan? Hora { get; set; }
        public string HoraStr { get; set; }

        //[Required(ErrorMessage = "-Debe seleccionar una opción para Municipio")]
        public int? IdMunicipio { get; set; }

        //[Required(ErrorMessage = "-Debe seleccionar una opción para Carretera")]
        public int? IdCarretera { get; set; }

        //[Required(ErrorMessage = "-Debe seleccionar una opción para Tramo")]
        public int? IdTramo { get; set; }

		//[Required(ErrorMessage = "-El campo Kilómetro es obligatorio")] 
		public string Kilometro { get; set; }
		public string observacionesTramo { get; set; }
		public string Colonia { get; set; }
        public string Numero { get; set; }
        public string Calle { get; set; }

        public string EstatusReporte { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public int ActualizadoPor { get; set; }
        public int estatus { get; set; }

        public int idGenero { get; set; }

        public string Municipio { get; set; }
        public string Tramo { get; set; }
        public string Carretera { get; set; }

        public List<CapturaAccidentesModel> ListaAccidentes { get; internal set; }

        /// <summary>
        /// MODEL VEHICULOS
        /// </summary>
        public int? IdEmergencia { get; set; }
        public int? FolioEmergencia { get; set; }

        public int IdVehiculo { get; set; }
        public int? IdVehiculoNuevo { get; set; }

        public int IdMarcaVehiculo { get; set; }
        public int IdSubmarca { get; set; }
        public int IdEntidad { get; set; }
        public int IdColor { get; set; }
        public int IdTipoVehiculo { get; set; }
        public int IdCatTipoServicio { get; set; }
        public int IdPersona { get; set; }
        public string Serie { get; set; }
        public string Placa { get; set; }

        public string FolioBusqueda { get; set; }

        public string Tarjeta { get; set; }
        public string Otros { get; set; }

        public DateTime VigenciaTarjeta { get; set; }
        public string Marca { get; set; }
        public string Submarca { get; set; }
        public string TipoVehiculo { get; set; }
        public string Modelo { get; set; }

        public string VehiculoCompleto
        {
            get
            {

                return (Marca ?? "") + " " + (Submarca ?? "") + " " + (Modelo ?? "-");
            }
        }
        public string Color { get; set; }
        public string EntidadRegistro { get; set; }
        public string TipoServicio { get; set; }
        public string Propietario { get; set; }

        public string NumeroEconomico { get; set; }
        public string concesionario { get; set; }

        public string gruas { get; set; }

        public int IdPersonaFisica { get; set; }
        public int IdPersonaMoral { get; set; }

        public string DatoBusquedaVehiculo { get; set; }
        public int IdTipoLicencia { get; set; }
        public int IdCatTipoPersona { get; set; }
        public DateTime? fechaNacimiento { get; set; }
        public string FormatDateNacimiento { get; set; }

        public DateTime dateNacimiento { get; set; }
        public string FechaNacimientoFormateada => fechaNacimiento.HasValue ? fechaNacimiento.Value.ToString("dd/MM/yyyy") : string.Empty;

        public DateTime vigenciaLicencia { get; set; }
        public string TipoPersona { get; set; }
        public string TipoLicencia { get; set; }
        public string Genero { get; set; }

        public int IdTipoCarga { get; set; }
        public string Poliza { get; set; }

        public int IdDelegacion { get; set; }
        public int IdPension { get; set; }
        public int IdDelegacionPension { get; set; }


        public int IdFormaTraslado { get; set; }
        // Propiedades adicionales cuando la búsqueda es por Folio

        public string NumerosEconomicos { get; set; }
        public string IdGruas { get; set; }

        public int IdSolicitud { get; set; }
        public string FolioSolicitudDeposito { get; set; }
        public int IdDeposito { get; set; }
        public int IdConcesionario { get; set; }

        //////////
        ///MODEL INVOLUCRADOS///////////
        ///

        public int IdVehiculoInvolucrado { get; set; }
        public int IdConductorInvolucrado { get; set; }
        public int IdPropietarioInvolucrado { get; set; }
        public int IdFormaTrasladoInvolucrado { get; set; }
        public string VehiculoInvolucrado { get; set; }
        public string PropietarioInvolucrado { get; set; }
        public string EntidadPropietario { get; set; }
        public string MunicipioPropietario { get; set; }
        public string ColoniaPropietario { get; set; }
        public string CallePropietario { get; set; }
        public string NumeroPropietario { get; set; }
        public string TelefonoPropietario { get; set; }
        public string CorreoPropietario { get; set; }
        public string FormaTrasladoInvolucrado { get; set; }
        public string Pension { get; set; }
        public string Motor { get; set; }
        public string Capacidad { get; set; }
        public string ConductorInvolucrado { get; set; }
        public int NoAccidente { get; set; }

        public int NumeroContinuo { get; set; }

        public string Cortesia { get; set; }
        public int total { get; set; }






        ///////////
        /////////////////MODEL CLASIFICACION//////////
        ///
        public int? IdClasificacionAccidente { get; set; }

        public string NombreClasificacion { get; set; }


        //////////////////
        ///MODEL CAPTURA PARRTE 2
        ///

        public int? IdFactorAccidente { get; set; }
        public int? IdFactorOpcionAccidente { get; set; }
        public string FactorAccidente { get; set; }
        public string FactorOpcionAccidente { get; set; }
        public int? IdCausaAccidente { get; set; }
        public int? IdCausaAccidenteEdit { get; set; }
        public string CausaAccidente { get; set; }
        public string TipoCarga { get; set; }
        public bool TipoCargaBool { get; set; }

        public string DescripcionCausa { get; set; }
        public int idPersonaInvolucrado { get; set; }
        public string numeroLicencia { get; set; }
        public string CURP { get; set; }
        public string nombreArchivo { get; set; }
        public string rutaArchivo { get; set; }
        public string UrlImagen { get; set; }
        public string DescripcionArchivo { get; set; }
		public bool EsImagen { get; set; }
		public string CURPyRFC
        {
            get
            {
                return CURP + "/" + RFC;
            }
        }
        public string CURPBusqueda { get; set; }

        public string numeroLicenciaBusqueda { get; set; }

        public string RFC { get; set; }
        public string RFCBusqueda { get; set; }

        public string nombre { get; set; }

        public string nombreBusqueda { get; set; }

        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }

        public string NombreCompleto
        {
            get
            {

                return ((nombre ?? "") + " " + (apellidoPaterno ?? "") + " " + (apellidoMaterno ?? "")).ToUpper().Trim();
            }
        }
        public string apellidoPaternoBusqueda { get; set; }
        public string apellidoMaternoBusqueda { get; set; }
        public string Sexo { get; set; }
        public string Nacimiento { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public string LLevadoA { get; set; }
        public string Traslado { get; set; }
        public string Asiento { get; set; }
        public string Cinturon { get; set; }
        public string Entidad { get; set; }
        public string Direccion { get; set; }
        public string DireccionConductor { get; set; }

        public string Telefono { get; set; }
        public string Correo { get; set; }


        /////////////////
        ///CAPTURA PARTE 3//////////////
        ///
        public string folioInfraccion { get; set; }

        public float montoCamino { get; set; }
        public float montoCarga { get; set; }
        public float montoPropietarios { get; set; }
        public float montoOtros { get; set; }
        public int IdInfraccionAccidente { get; set; }
        public int IdInfraccion { get; set; }//Folio de infraccion
        public string EstatusInfraccion { get; set; }

        public float montoVehiculo { get; set; }
        public int IdTipoInvolucrado { get; set; }
        public string TipoInvolucrado { get; set; }
        public int IdEstadoVictima { get; set; }
        public int IdHospital { get; set; }
        public int IdInstitucionTraslado { get; set; }
        public int? IdAsiento { get; set; }
        public int IdCinturon { get; set; }
        public int IdCasco { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public int IdCiudad { get; set; }
        public int IdCertificado { get; set; }
        public string entregaObjetos { get; set; }
        public string entregaOtros { get; set; }
        public string consignacionHechos { get; set; }
        public string numeroOficio { get; set; }
        public int IdAgenciaMinisterio { get; set; }
        public string RecibeMinisterio { get; set; }
        public int IdElabora { get; set; }
        public int IdSupervisa { get; set; }
        public int IdAutoriza { get; set; }
        public int IdElaboraConsignacion { get; set; }
        public string Vehiculo { get; set; }
        public string Grua { get; set; }

        public int OtraColumna { get; set; }
        public string EstadoVictima { get; set; }
        public string NombreHospital { get; set; }
        public string InstitucionTraslado { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public TimeSpan? HoraIngreso { get; set; }
        public int IdAutoridadEntrega { get; set; }
        public int IdAutoridadDisposicion { get; set; }
        public int ArmasBool { get; set; }
        public int ArmasValue { get; set; }
        public int DrogasValue { get; set; }
        public int ValoresValue { get; set; }
        public int PrendasValue { get; set; }
        public int OtrosValue { get; set; }
        public int idOficial { get; set; }
        public string ArmasTexto { get; set; }
        public string DelegacionOficina { get; set; }
        public int indice { get; set; }
        public int numeroConsecutivo { get; set; }
        public string FolioDetencion { get; set; }

        public string fullVehiculo
        {
            get
            {
                return "Vehiculo: " + Marca + " " + Submarca + " " + Modelo + Environment.NewLine +
                       "Placas: " + Placa + Environment.NewLine +
                       "Tarjeta: " + Tarjeta + Environment.NewLine +
                       "Numero Vehículo: " + IdVehiculoInvolucrado + Environment.NewLine +
                       "Tipo de Carga: " + TipoCarga + Environment.NewLine +
                       "Poliza de Carga: " + Poliza;
            }
        }


        public string fullDetalles
        {
            get
            {
                return @"Pension: " + Pension + "\r\n\n " +
                "Grua: " + Grua + "\r\n\n " +
                "Forma de Traslado: " + FormaTrasladoInvolucrado + "\r\n\n " +
                "Tipo Vehiculo: " + TipoVehiculo;


            }
        }

        public string garantia { get; set; }
        public int Total { get; set; }

        //Por Modelo Personas
        public int idPersona { get; set; }
        public int Edad { get; set; }
        public string? FechaNacimientoTexto { get; set; }
        public string? FechaVigenciaLicenciaTexto { get; set; }
        public string? VigenciaTarjetaFormateada { get; set; }

		//PAARA ARCHIVO INVOLUCRADO
		public IFormFile archivoInvolucrado { get; set; }
		public string archivoInvolucradoPath { get; set; }
		public string? ArchivoInvolucradoBase64 { get; set; }
		public string archivoInvolucradoStr { get; set; }
        public string trayectoria { get; set; }

        //Croquis
        public IFormFile? archivoCroquis { get; set; }
		public string archivoCroquisPath { get; set; }
		public string? archivoLugarPath { get; set; }
		public long? IdTurno { get; set; }
        public int? IdAseguradora { get; set; }
        public string NombreAseguradora { get; set; }
        public DateTime? FechaExpiracionPoliza { get; set; }
        public string? FechaExpiracionPolizaTexto { get; set; }
    }
}
