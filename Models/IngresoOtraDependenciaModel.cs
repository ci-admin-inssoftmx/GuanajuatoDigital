using System.ComponentModel.DataAnnotations;
using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class IngresoOtraDependenciaModel
    {
        [Required(ErrorMessage = "-El campo Fecha  es obligatorio")]
        public DateTime FechaIngreso { get; set; }
        [Required(ErrorMessage = "-El campo Hora  es obligatorio")]
        public TimeSpan HoraIngreso { get; set; }
        public int IdEnvia { get; set; }
        public int IdMunicipio { get; set; }
        public int IdMotivo { get; set; }
        public int IdTramo { get; set; }
        public string motivoAsignacion { get; set; }
        public string numeroUbicacion { get; set; }
        public string calleUbicacion { get; set; }
        public string coloniaUbicacion { get; set; }
        public string kilometroUbicacion { get; set; }
        public int? IdCarretera { get; set; }
        public int? idEntidadUbicacion { get; set; }
        public int? idMunicipioUbicacion { get; set; }
        public int? idPensionUbicacion { get; set; }
        public string interseccion { get; set; }
        public string PlacasBusqueda { get; set; }
        public string SerieBusqueda { get; set; }

        

    }
}
