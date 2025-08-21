using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatCarreterasModel
    {
        public int IdCarretera { get; set; }

        public string Carretera { get; set; }

        public int idOficinaTransporte { get; set; }
        public string nombreOficina { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }
        public bool ValorEstatusCarretera { get; set; }
        public int Transito { get; set; }
        
        public int? Corp { get; set; }
        public int? idDependencia { get; set; }


    }
}
