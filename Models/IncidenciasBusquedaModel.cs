using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class IncidenciasBusquedaModel
    {
        public int? idTipoMotivo { get; set; }
        public int idDelegacion { get; set; }
        public int idOficial { get; set; }
        public int idCarretera { get; set; }
        public int idTramo { get; set; }
        public int IdTipoCortesia { get; set; }
        
        public int idTipoVehiculo { get; set; }
        public int idSubtipoVehiculo { get; set; }       
        public int idTipoServicio { get; set; }
        public int   idSubTipoServicio { get; set; }
        public int idTipoLicencia { get; set; }
        public int idMunicipio { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
    }



    public class IncidenciasBusquedaModelEstadisticas
    {
        public int? idTipoMotivo { get; set; }
        public int? idDelegacion { get; set; }
        public int? idOficial { get; set; }
        public int? idCarretera { get; set; }
        public int? idTramo { get; set; }
        public int? IdTipoCortesia { get; set; }

        public int? idTipoVehiculo { get; set; }
        public int? idSubtipoVehiculo { get; set; }
        public int? idTipoServicio { get; set; }
        public int? idSubTipoServicio { get; set; }
        public int? idTipoLicencia { get; set; }
        public int? idMunicipio { get; set; }
        public DateTime? fechaInicio { get; set; }
        public DateTime? fechaFin { get; set; }
    }


}
