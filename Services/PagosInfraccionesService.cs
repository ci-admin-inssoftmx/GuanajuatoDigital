using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services
{
    public class PagosInfraccionesService : IPagosInfraccionesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        private readonly IAppSettingsService _appSettingsService;
        private readonly IInfraccionesService _infraccionesService;
        public PagosInfraccionesService(ISqlClientConnectionBD sqlClientConnectionBD, IAppSettingsService appSettingService, IInfraccionesService infraccionesService)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
            _appSettingsService = appSettingService;
            _infraccionesService = infraccionesService;
        }

        public ResponsePagoModel Pagar(InfoPagoModel InfoPago) 
        {
            var Response = new ResponsePagoModel();
            if (!validarUsuarioContraseña(InfoPago.UsuarioLog, InfoPago.PasswordLog))
            {
                Response.HasError = true;
                Response.CodigoRespuesta = 1;
                Response.Mensaje = "No se pudo comprobar el usuario y contraseña";
                return Response;
            }

            if (!validarInfraccion(InfoPago))
            {
                Response.HasError = true;
                Response.CodigoRespuesta = 5;
                Response.Mensaje = "No se pudo validar el Folio ingresado";
                return Response;
            }

            //Pendiente Por Validar con el cliente no definido en documento
            if (!validarLugarPago(InfoPago.LugarPagoId))
            {
                Response.HasError = true;
                Response.CodigoRespuesta = 5;
                Response.Mensaje = "El identificador de lugar de pago no tiene el formato adecuado";
                return Response;
            }

            if (!validarFormatoFolioInfraccion(InfoPago.FolioInfraccion))
            {
                Response.HasError = true;
                Response.CodigoRespuesta = 5;
                Response.Mensaje = "El folio proporcionado no tiene el formato requerido (TTO-#### -> para tránsito;   TTE-#### -> para transporte)";
                return Response;
            }

            var infraccion = buscarInfraccionByFolio(InfoPago.FolioInfraccion);
            if (infraccion == null)
            {
                Response.HasError = true;
                Response.CodigoRespuesta = 4;
                Response.Mensaje = "FOLIO NO ENCONTRADO. (Pago capturado para cuando se capture folio)";
                return Response;
            }

            if (infraccion.estatusInfraccion == "Pagada" || infraccion.estatusInfraccion == "Pagada con descuento" || infraccion.estatusInfraccion == "Pagada con recargo" || infraccion.estatusInfraccion == "Solventada")
            {
                Response.HasError = true;
                Response.CodigoRespuesta = 2;
                Response.Mensaje = "FOLIO PAGADO ANTERIORMENTE";
                return Response;
            }

            //Pendiente por definir con el cliente
            //if (!validaMontoPago(InfoPago.MontoPagado, infraccion.totalInfraccion))
            //{
            //    Response.HasError = true;
            //    Response.CodigoRespuesta = 5;
            //    Response.Mensaje = "El monto pagado es incorrecto o no tiene formato adecuado";
            //    return Response;
            //}

            return SuccessPago(InfoPago, infraccion);
        }

        public ResponsePagoModel ReversaDePago(ReversaPagoModel ReversaPago)
        {
            var Response = new ResponsePagoModel();
            if (!validarUsuarioContraseña(ReversaPago.UsuarioLog, ReversaPago.PasswordLog))
            {
                Response.HasError = true;
                Response.CodigoRespuesta = 1;
                Response.Mensaje = "No se pudo comprobar el usuario y contraseña";
                return Response;
            }
            if (false)//ReversaPago.ReciboControlInterno.Length != 12)
            {
                Response.HasError = true;
                Response.CodigoRespuesta = 5;
                Response.Mensaje = "El recibo de pago (no. de control interno) proporcionado no tiene el formato requerido (12 digitos)";
                return Response;
            }

            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime dt;
            if (!DateTime.TryParseExact(ReversaPago.FechaReversa, "yyyy-MM-dd", provider,DateTimeStyles.None, out dt))
            {
                Response.HasError = true;
                Response.CodigoRespuesta = 5;
                Response.Mensaje = "El formato de la fecha de reversa no es correcto (AAAA-MM-DD)";
                return Response;
            }

            var infraccion = buscarInfraccionByReciboPago(ReversaPago.ReciboControlInterno);
            if (infraccion == null)
            {
                Response.HasError = true;
                Response.CodigoRespuesta = 4;
                Response.Mensaje = "FOLIO NO ENCONTRADO.";
                return Response;
            }

            //Péndiente validar con el cliente, validación no especificada en el documento
            //if (infraccion.estatusInfraccion != "Pagada" || infraccion.estatusInfraccion != "Pagada con descuento" || infraccion.estatusInfraccion != "Pagada con recargo")
            //{
            //    Response.HasError = true;
            //    Response.CodigoRespuesta = 2;
            //    Response.Mensaje = "FOLIO AUN NO PAGADO";
            //    return Response;
            //}

            UpdatePagoInfraccionReversa(ReversaPago.ReciboControlInterno);

            Response.HasError = false;
            Response.CodigoRespuesta = 0;
            Response.Mensaje = "OK";
            return Response;
        }

        private bool validarUsuarioContraseña(string usuario, string password)
        {
            var User = _appSettingsService.GetAppSetting("PagoInfraccionUser").SettingValue;
            var Pasw = _appSettingsService.GetAppSetting("PagoInfraccionPassword").SettingValue;

            if (usuario == User && password == Pasw)
                return true;
            else 
                return false;
        }
        private bool validarLugarPago(string lugarPago)
        {
            //Pendiente Por Validar con el cliente no definido en documento
            return true;
        }

        private bool validarInfraccion(InfoPagoModel Modelo)
        {
            try
            {
                var spl = Modelo.FolioInfraccion.Split("-");
                if (spl.Length > 1)
                    Modelo.FolioInfraccion = spl[1];
                else
                    Modelo.FolioInfraccion = spl[0];

            }
            catch { }

            //Pendiente Por Validar con el cliente no definido en documento
            return true;
        }
        private bool validarFormatoFolioInfraccion(string folioInfraccion)
        {
            //TTO-#### -> Para Transito
            //TTE-#### -> Para Transporte
            //return folioInfraccion.StartsWith("TTO-") || folioInfraccion.StartsWith("TTE-");
            return true;
        }
        private InfraccionesModel buscarInfraccionByFolio(string FolioInfraccion) => _infraccionesService.GetAllInfraccionesByFolioInfraccion(FolioInfraccion).FirstOrDefault();
        private InfraccionesModel buscarInfraccionByReciboPago(string reciboPago) => _infraccionesService.GetAllInfraccionesByReciboPago(reciboPago).FirstOrDefault();
        private bool validaMontoPago(string montoPago, decimal montoPorPagar)
        {
            decimal montoPagoD = 0;
            if (!decimal.TryParse(montoPago, out montoPagoD))            
                return false;

            if (montoPagoD == montoPorPagar)
                return true;
            else
                return false;

            return true;
        }
        private ResponsePagoSuccessModel SuccessPago(InfoPagoModel infoPago , InfraccionesModel infraccion)
        {
            //DateTime dt = DateTime.Now;
            ResponsePagoSuccessModel response = new ResponsePagoSuccessModel();
            response.CodigoRespuesta = 0;
            response.cveOficina = infoPago.LugarPagoId;
            response.fechaInfraccion = infraccion.fechaInfraccion.ToString("dd/MM/yyyy");
            //response.fechaPago = dt.ToString("dd/MM/yyyy");
            response.fechaPago = infoPago.FechaPago;
            response.fechaVencimiento = (infraccion.fechaVencimiento == null)? "":infraccion.fechaVencimiento.ToString("dd/MM/yyyy");
            response.folio = infoPago.FolioInfraccion;
            response.identificador = "";//PENDIENTE
            response.lugarPagoID = infoPago.LugarPagoId;
            response.Mensaje = "OK";
            response.monto = infraccion.totalInfraccion.ToString();
            response.montoPagado = infoPago.MontoPagado;
            response.montoSinDescuento = infoPago.MontoPagado;//PENDIENTE
            response.reciboControlInterno = infoPago.ReciboControlInterno;

            //UPDATE LUGAR PAGO PENDIENTE
            UpdatePagoInfraccion(infoPago.FolioInfraccion, infoPago.ReciboControlInterno, response.monto, response.montoPagado, response.fechaPago, response.lugarPagoID);

            return response;
        }
        private int UpdatePagoInfraccion(string folioInfraccion, string reciboPago, string monto, string montoPagado, string fechaPago, string lugarPagoID)
        {
            int estatusinf = 0;
            int result = 0;
            var Monto = decimal.Parse(monto);
            var MontoPagado = decimal.Parse(montoPagado);

            if (Monto == MontoPagado)
            {
                estatusinf = 3;
            }
            else if (MontoPagado > Monto)
            {
                estatusinf = 6;
            }
            else if (MontoPagado < Monto)
            {
                estatusinf = 4;
            }
            string strQuery = @"Exec usp_UpdatePagoInfraccion @idEstatusInfraccion,@reciboPago,@MontoPagado,@fechaPago,@lugarPagoId,@folioInfraccion";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@folioInfraccion", folioInfraccion);
                    command.Parameters.AddWithValue("@reciboPago", reciboPago);
                    command.Parameters.AddWithValue("@MontoPagado", MontoPagado);
                    command.Parameters.Add(new SqlParameter("@fechaPago", SqlDbType.DateTime2)).Value = fechaPago;
                    command.Parameters.AddWithValue("@lugarPagoId", lugarPagoID);
                    command.Parameters.AddWithValue("@idEstatusInfraccion", estatusinf);//depende del monto se cambia estatus
                    
                    result = command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return result;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }
        private int UpdatePagoInfraccionReversa(string reciboPago)
        {
            int result = 0;
            string strQuery = @"UPDATE infracciones
                                SET idEstatusInfraccion=@idEstatusInfraccion
                                WHERE reciboPago=@reciboPago ";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@reciboPago", reciboPago);
                    command.Parameters.AddWithValue("@idEstatusInfraccion", 5);//Pagada

                    result = command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return result;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }
    }
}
