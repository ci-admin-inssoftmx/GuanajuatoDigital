using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class EstadisticaInfraccionMotivosModel
    {
        public string Motivo { get; set; }
        public int Contador { get; set; }
        public int NumeroMotivos { get; set; }
        public int ContadorMotivos { get; set; }
        public int ResultadoMultiplicacion { get; set; }
        public int TotalReal { get; set; }
        public int Total { get; set; }
        public int totalp { get; set; }
        public virtual IEnumerable<MotivosInfraccionVistaModel> MotivosInfraccion { get; set; }




        public int Con1Motivo { get; set; }
        public int Con2Motivos { get; set; }
        public int Con3Motivos { get; set; }


        public string TodosMotivos
        {
            get
            {
                return $"{Con1Motivo}<br />{Con2Motivos}<br />Co{Con3Motivos}";
            }
        }

        public int idCarretera { get; set; }
        public int idDelegacion { get; set; }
        public int idOficial { get; set; }
        public int idTramo { get; set; }
        public int idTipoVehiculo { get; set; }
        public int idTipoServicio { get; set; }
        public int idSubTipoServicio { get; set; }
        public int idTipoLicencia { get; set; }
        public int IdTipoCortesia { get; set; }
        public int idMunicipio { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }


    }
}
