namespace GuanajuatoAdminUsuarios.Models
{
    public class InfoPagoModel
    {
        public string UsuarioLog { get; set; }
        public string PasswordLog { get; set; }
        public string FolioInfraccion { get; set; }
        public string MontoPagado { get; set; }
        public string FechaPago { get; set; }
        public string ReciboControlInterno { get; set; }
        public string LugarPagoId { get; set; }
    }

    public class ResponsePagoModel
    {
        public int CodigoRespuesta { get; set; }
        public string Mensaje { get; set; }
        public bool HasError { get; set; }
    }

    public class ResponsePagoSuccessModel : ResponsePagoModel
    {
        public string cveOficina            { get; set; }
        public string fechaInfraccion       { get; set; }
        public string fechaPago             { get; set; }
        public string fechaVencimiento      { get; set; }
        public string folio                 { get; set; }
        public string identificador         { get; set; }
        public string lugarPagoID           { get; set; }
        public string monto                 { get; set; }
        public string montoPagado           { get; set; }
        public string montoSinDescuento     { get; set; }
        public string reciboControlInterno { get; set; }
    }
}
