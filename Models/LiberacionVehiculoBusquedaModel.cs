using System.Collections.Generic;
using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class LiberacionVehiculoBusquedaModel
    {
        public List<LiberacionVehiculoModel> ListDepositosLiberacion { get; set; }
        public int? IdDeposito { get; set; }
        public int? IdMarcaVehiculo { get; set; }
        public string Serie { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string Folio { get; set; }
		public string Placas { get; set; }

	}
}
