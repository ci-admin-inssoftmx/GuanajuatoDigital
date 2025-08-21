using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class GruasConcesionariosModel
    {
        public int IdGrua { get; set; }
        public int IdConcesionario { get; set; }
        public int IdDeposito { get; set; }

        public string Concesionario { get; set; }
        public string noEconomico { get; set; }
        public string placas { get; set; }
        public int IdTipoGrua { get; set; }
        public int abanderamiento { get; set; }
        public int minutosManiobra { get; set; }

        public string TipoGrua { get; set; }
        public string modelo { get; set; }
        public string capacidad { get; set; }
        //public string clasificacion { get; set; }
        public DateTime fechaActualizacion { get; set; }
        public DateTime fechaArribo { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFinal { get; set; }
        public float costoAbanderamiento { get; set; }
        public float costoArrastre { get; set; }
        public float costoSalvamento { get; set; }
        public float costoTotal { get; set; }
        public float costoBanderazo { get; set; }

        public string operadorGrua { get; set; }


        public int actualizadoPor { get; set; }
        public int estatus { get; set; }
        public string capacidadT { get; set; }

    }
}
