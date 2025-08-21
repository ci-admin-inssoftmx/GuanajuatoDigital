using System;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class SolicitudDepositoModel
    {

        public string NombreArchivo { get; set; }
        public string pathArchivo { get; set; }
        public string folioBusquedaInfraccion { get; set; }
        public int? idSolicitud { get; set; }
        public string folio { get; set; }
        public string folioInfraccion { get; set; }

        public int folioBDeposito { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime fechaSolicitud { get; set; }
        public TimeSpan horaSolicitud { get; set; }
        public string horaSolicitudStr { get; set; }

        
        public int? idTipoVehiculo { get; set; }
        public string tipoVehiculo { get; set; }
        public int? idServicioRequiere { get; set; }
        public string servicioRequiere { get; set; }
        public int? idConcecionario { get; set; }
        public string propietarioGrua { get; set; }

        public int? idOficial { get; set; }
        public string oficial { get; set; }

        public int? idEntidad { get; set; }
        public string entidad { get; set; }

        public int? idMunicipio { get; set; }
        public string municipio { get; set; }

        public string nombreUsuario { get; set; }
        public string apellidoPaternoUsuario { get; set; }
        public string apellidoMaternoUsuario { get; set; }
        public string usuarioCompleto { get; set; }


        public string coloniaUsuario { get; set; }
        public string calleUsuario { get; set; }
        public string numeroUsuario { get; set; }
        public string telefonoUsuario { get; set; }
        public int? idDescripcionEvento { get; set; }
        public string descripcionEvento { get; set; }

        public int? idTipoUsuario { get; set; }
        public string tipoUsuario { get; set; }

        public int? idMotivoAsignacion { get; set; }
        public string motivoAsignacion { get; set; }
        public string numeroUbicacion { get; set; }
        public string calleUbicacion { get; set; }
        public string coloniaUbicacion { get; set; }
        public string kilometroUbicacion { get; set; }
        public int? IdCarretera { get; set; }
        public int? IdTramo { get; set; }
        public int? idEntidadUbicacion { get; set; }
        public int? idMunicipioUbicacion{ get; set; }
        public int? idPensionUbicacion { get; set; }
        public string interseccion { get; set; }

        public int idInfraccion { get; set; }
        public int? idVehiculo { get; set; }
        public int? idPersona { get; set; }
        public int? idDeposito { get; set; }


        public DateTime? fechaInfraccion { get; set; }
        public TimeSpan horaInfraccion { get; set; }

        public bool robo { get; set; }


    }
}
