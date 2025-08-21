using GuanajuatoAdminUsuarios.RESTModels;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class BuscarVehiculosResult
    {
        public List<VehiculoModel> VehiculosLocales { get; set; }
        public CotejarDatosResponseModel DatosCotejados { get; set; }
    }
}
