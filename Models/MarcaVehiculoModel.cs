using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class MarcaVehiculoModel
    {
        public int IdMarcaVehiculo { get; set; }

        public string MarcaVehiculo { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ModificadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
        
        public bool ValorEstatusMarcas { get; set; }


    }


}
