using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class GruasSalidaVehiculosModel
    {
        public int idGrua { get; set; }
        public int idDeposito { get; set; }

        public int abanderamiento { get; set; }
        public int arrastre { get; set; }
        public int salvamento { get; set; }

        public string grua { get; set; }
        public string tipoGrua { get; set; }

        public float costoTotalPorGrua { get; set; }
        public static float CalcularCostoTotalTodasGruas(IEnumerable<GruasSalidaVehiculosModel> modelos)
        {
            float costoTotal = 0;
            foreach (var modelo in modelos)
            {
                costoTotal += modelo.costoTotalPorGrua;
            }
            return costoTotal;
        }
    }
}
