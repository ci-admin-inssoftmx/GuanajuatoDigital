using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.RESTModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace GuanajuatoAdminUsuarios.Models
{
    public class VehiculoModel : EntityModel
    {

        public string ErrorFinanzas { get; set; }
		public bool ServicioDesactivado { get; set; }
        public string MensajeServicioDesactivado { get; set; }
        public bool ReporteRobo { get; set; }  
        public bool ErrorConsultaRepuve { get; set; }
        public bool showclose { get; set; } = true;
        
        public int idVehiculo { get; set; }
        public string placas { get; set; }
        public string serie { get; set; }
        public string tarjeta { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? vigenciaTarjeta { get; set; }
        public DateTime fechaVencimientoFisico { get; set; }
        
        public int idMarcaVehiculo { get; set; }
        public int idSubmarca { get; set; }
        public int idSubmarcaUpdated { get; set; }
        public int idTipoVehiculo { get; set; }
        public string modelo { get; set; }
        public int? idColor { get; set; }
        public int? idColorReal { get; set; }
        public int idEntidad { get; set; }
        public int idEdntidad2 { get; set; }
        public int idCatTipoServicio { get; set; }
        public int? idTipoPersona { get; set; }

        public int idSubtipoServicio { get; set; }
        public string propietario { get; set; }
        public string numeroEconomico { get; set; }
        public string paisManufactura { get; set; }
        public int? idPersona { get; set; }
        public int? idPropietario { get; set; }

        public DateTime fechaNacimiento { get; set; }
        
        public string marca { get; set; }
        public string submarca { get; set; }
        public string tipoVehiculo { get; set; }
        public string? color { get; set; }
        public string? colorReal { get; set; }
        public string entidadRegistro { get; set; }
        public string tipoServicio { get; set; }
        public string subTipoServicio { get; set; }
        public string fullVehiculo => $"{marca} {submarca} {modelo}";

        public string motor { get; set; }
        public string motorActual { get; set; }
        public int? capacidad { get; set; }
        public string poliza { get; set; }
        public bool? carga { get; set; }
        public string cargaTexto { get; set; }
        public int cargaInt { get; set; }
		public int NumeroSecuencial { get; set; }
		
		public string otros { get; set; }
        public string RFCMoral { get; set; }
        public string PersonaMoralNombre { get; set; }
        public string mensaje { get; set; }
        
        /// <summary>
        /// Estatus para saber si se encontro en Sitteg, Registro Estatal o no 
        /// </summary>
        public int? encontradoEn { get; set; }
        public int? idInfraccion { get; set; }
        public int? IdAccidente { get; set; }

        public string? origenDatos { get; set; }

        public PersonaMoralBusquedaModel PersonaMoralBusquedaModel { get; set; }

        //public string RFC { get; set; }
        //public string RazonSoccial { get; set; }

        public virtual PersonaModel Persona { get; set; }

        //public virtual PersonaModel PersonaUpdate { get; set; }
        public bool? showSubTipo { get; set; } = false;
        public int total { get; set; }

        public RepuveRoboModel RepuveRobo { get; set; }
        public List<PersonaModel> PersonasFisicas { get; set; }
        public string curp { get; set; }
        public string rfc { get; set; }

    }
}
