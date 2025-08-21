using GuanajuatoAdminUsuarios.Models;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IPagosInfraccionesService
    {
        public ResponsePagoModel Pagar(InfoPagoModel InfoPago);
        public ResponsePagoModel ReversaDePago(ReversaPagoModel ReversaPago);
    }
}
