using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class VehiculoBusquedaModel
    {
        public bool ReporteRobo { get; set; }
        public int? IdEntidadBusqueda { get; set; }
        public string PlacasBusqueda { get; set; } = null!;
        public string SerieBusqueda { get; set; } = null!;

        public string FolioBusqueda { get; set; } = null!;

        #region Campos de busqueda Modificacion
        public string tarjeta { get; set; } = null!;
        public string motor { get; set; } = null!;
        public int? idMarca { get; set; }
        public int? idSubMarca { get; set; }
        public int? idTipoVehiculo { get; set; }
        //public int? idTipoServicio { get; set; }
        public int? idSubtipoServicio { get; set; }
        public string modelo { get; set; } = null!;
        public string numeroEconomico { get; set; } = null!;
        public string propietario { get; set; } = null!;
        public int? idColor { get; set; }

        public bool? isFromUpdate { get; set; } = false;
        #endregion

        public VehiculoModel Vehiculo { get; set; }
        public List<VehiculoModel> ListVehiculo { get; set; }
    }
}
