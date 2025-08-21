using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class BusquedaAccidentesModel
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public int IdAccidente { get; set; }
        public int idPropietario { get; set; }

        public string folioBusqueda { get; set; }
        public string Delegacion { get; set; }   
        public int? IdDelegacionBusqueda { get; set; }
        public int? IdCarreteraBusqueda { get; set; }
        public int? IdTramoBusqueda { get; set; }
        public string placasBusqueda { get; set; }
        public string serieBusqueda { get; set; }
        public string propietarioBusqueda { get; set; }
        public string conductorBusqueda { get; set; }
        public string FolioEmergenciaBusqueda { get; set; }
        public int? IdOficialBusqueda { get; set; }
        public int IdEstatusAccidente { get; set; }
        public string estatusAccidente { get; set; }
        //////
        ///Model Regreso/////
        ///

        public int? idMunicipio { get; set; }
        public int? idCarretera { get; set; }
        public int? idTramo { get; set; }
        public string estatusReporte { get; set; }
        public int idPersona { get; set; }
        public int idVehiculo { get; set; }
        public int? idClasificacionAccidente { get; set; }
        public int? idCausaAccidente { get; set; }
        public int? idFactorAccidente { get; set; }
        public int? idFactorOpcionAccidente { get; set; }
        public float montoCamino { get; set; }
        public float montoPropietarios { get; set; }
        public float montoOtros { get; set; }
        public float montoVehiculo { get; set; }
        public int? idElabora { get; set; }
        public int? idSupervisa { get; set; }
        public int? idAutoriza { get; set; }
        public int idElaboraConsignacion { get; set; }
        public int idCiudad { get; set; }
        public int idAgenciaMinisterio { get; set; }
        public int idAutoridadEntrega { get; set; }
        public int idAutoridadDisposicion { get; set; }
        public int armas { get; set; }
        public int drogas { get; set; }
        public int valores { get; set; }
        public int prendas { get; set; }
        public int otros { get; set; }
        public int? idEstatusReporte { get; set; }
        public int idConductor { get; set; }
        public int Propietario { get; set; }
        public float latitud { get; set; }
        public float longitud { get; set; }
        public int idCertificado { get; set; }

        public string kilometro { get; set; }
        public string municipio { get; set; }


        public string descripcionAccidente { get; set; }
        public string numeroReporte { get; set; }
        public DateTime fecha { get; set; }
        public TimeSpan hora { get; set; }
        public string entregaOtros { get; set; }
        public string entregaObjetos { get; set; }
        public int Total { get; set; }

        public string consignacionHechos { get; set; }
        public string numeroOficio { get; set; }
        public string recibeMinisterio { get; set; }
        public string nombreClasificacion { get; set; }
        public string factorAccidente { get; set; }
        public string factorOpcionAccidente { get; set; }
        public string nombreElabora { get; set; }
        public string apellidoPaternoElabora { get; set; }
        public string apellidoMaternoElabora { get; set; }
        public string nombreSupervisa { get; set; }
        public string apellidoPaternoSupervisa { get; set; }
        public string apellidoMaternoSupervisa { get; set; }
        public string nombreAutoriza { get; set; }
        public string apellidoPaternoAutoriza { get; set; }
        public string apellidoMaternoAutoriza { get; set; }
        public string nombreElaboraCons { get; set; }
        public string apellidoPaternoElaboraCons { get; set; }
        public string apellidoMaternoElaboraCons { get; set; }
        public string ciudad { get; set; }
        public string certificadoPor { get; set; }
        public string nombreAgencia { get; set; }
        public string autoridadEntrega { get; set; }
        public string nombreAutoridadDisposicion { get; set; }
        public string placa { get; set; }
        public string serie { get; set; }
        public string nombrePropietario { get; set; }
        public string apellidoPaternoPropietario { get; set; }
        public string apellidoMaternoPropietario { get; set; }
		public string NombreCompletoPropietario
		{
			get
			{
				return $"{nombrePropietario} {apellidoPaternoPropietario} {apellidoMaternoPropietario}".Trim();
			}
		}
		public int idTipoLicencia { get; set; }
        public string nombreConductor { get; set; }
        public string apellidoPaternoConductor { get; set; }
        public string apellidoMaternoConductor { get; set; }
        public string tramo { get; set; }
        public string carretera { get; set; }
        private string resultValue = string.Empty;

        public DateTime fechaCompleta { get; set; }
        public string nombrePropietarioCompleto { get; set; }
        public string nombreConductorCompleto { get; set; }
        public int? IdOficial { get; set; }
        public int? idDelegacion { get; set; }
        public string folio { get; set; }
        public string propietario { get; set; }
		 public int Numero { get; set; }

		public string conductor { get; set; }
        public int? IdTipoVehiculo { get; set; }
        public int? IdTipoServicio { get; set; }
		public int? IdSubtipoServicio { get; set; }
		
		public int estatus { get; set; }
		public int total { get; set; }
        public int NumeroSecuencial { get; set; }
        public int? folioEmergencia { get; set; }
        public int? emergenciasId { get; set; }

        


    }
}

