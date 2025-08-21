using GuanajuatoAdminUsuarios.Data.Entities2;
using System;

namespace GuanajuatoAdminUsuarios.Models.Blocs
{
    public class BlocsInventarioModel
    {
        public string folioInfraccion { get; set; } 
        public int idDetalle { get; set; }
        public string Serie { get; set; }
        public string Folio { get; set; }
        public int TotalBoletas { get; set; }
        public int idEstatusDetalle { get; set; }
        public string Oficina { get; set; }
        public string TipoBlock { get; set; }
        public string Asigna { get; set; }
        public string FechAsignacion { get; set; }
        public string OficialAsignado { get; set; }
        public string FechaCarga { get; set; }
        public string Estado { get; set; }

        public bool IsCancelado => Estado == DetalleBloc.EstatusCancelado;
        public bool IsAsignado => Estado == DetalleBloc.EstatusAsignado;
    }
}
