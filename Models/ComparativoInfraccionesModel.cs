using iTextSharp.text;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class ComparativoInfraccionesModel
    {
        public int año1 { get; set; }
        public int año2 { get; set; }
        public int idTipoMotivo { get; set; }
        public int idDelegacion { get; set; }
        public int idTipoVehiculo { get; set; }
        public int idOficial { get; set; }
        public int idTipoServicio { get; set; }
        public int idCarretera { get; set; }
        public int idSubTipoServicio { get; set; }
        public int idTramo { get; set; }            
        public int idTipoLicencia { get; set; }
        public int idMunicipio { get; set; }
        public int idTipo { get; set; }
    }

    public class ComparativoInfraccionesResumenModel
    {
        public int año1 { get; set; }
        public int año2 { get; set; }
        public List<ResultadoGeneral> resultadosGenerales { get; set; }
        public List<DetallePorCausa> detallesPorCausa { get; set; }
        public List<DesgloseTotalInfraccion> desgloseTotalDeInfracciones { get; set; }
    }

    public class ResultadoGeneral
    {
        public int año { get; set; }
        public int total { get; set; }
    }

    public class DetallePorCausa
    {
		
		public string causa { get; set; }
		public string causaTrunc { get; set; }

		public int cantidad { get; set; }
        public int año { get; set; }
    }

    public class DesgloseTotalInfraccion
    {
        public string numeroMotivo { get; set; }
        public int totalInfracciones { get; set; }
        public int totalContabiliza { get; set; }
        public int año { get; set; }
    }
}
