using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class DepositosModel
    {
        public int IdDeposito { get; set; }

        public int IdSolicitud { get; set; }

        public int IdDelegacion { get; set; }

        public int IdMarca { get; set; }

        public int IdSubmarca { get; set; }

        public int IdPension { get; set; }

        public int IdTramo { get; set; }

        public int IdColor { get; set; }

        public string Serie { get; set; }

        public string Placa { get; set; }

        public DateTime FechaIngreso { get; set; }
        public DateTime FechaLiberacion { get; set; }

        public string Folio { get; set; }

        public string Km { get; set; }

        public int Liberado { get; set; }

        public string AcreditacionPropiedad { get; set; }

        public string AcreditacionPersonalidad { get; set; }

        public string ReciboPago { get; set; }

        public string Observaciones { get; set; }

        public string Autoriza { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public int ActualizadoPor { get; set; }

        public int Estatus { get; set; }
    }
}
