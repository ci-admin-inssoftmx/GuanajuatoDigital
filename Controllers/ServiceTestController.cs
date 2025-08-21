using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.RESTModels;
using GuanajuatoAdminUsuarios.Services;
using GuanajuatoAdminUsuarios.SOAPModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReciboPago;
using SITTEG.APIClientInfrastructure.Client;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoRequestModel;
using Microsoft.AspNetCore.Authorization;



namespace GuanajuatoAdminUsuarios.Controllers
{
    [Authorize]
    public class ServiceTestController : BaseController
    {
        private readonly IRequestDynamic<RecibosPagoWSRequestModel, RecibosPagoWSResponsetModel> _requestDynamic;
        private readonly ILogger<InicioController> _logger;
        private readonly IServiceAppSettingsService _serviceAppSettingsService;
        private readonly ICotejarDocumentosClientService _cotejarDocumentosClientService;
        private readonly IConsultarDocumentoService _consultarDocumentoService;

        public ServiceTestController(IServiceAppSettingsService serviceAppSettingsService,
            ILogger<InicioController> logger, IRequestDynamic<RecibosPagoWSRequestModel,
                RecibosPagoWSResponsetModel> requestDynamic, ICotejarDocumentosClientService cotejarDocumentosClientService
            , IConsultarDocumentoService consultarDocumentoService
            )
        {
            _serviceAppSettingsService = serviceAppSettingsService;
            _logger = logger;
            _requestDynamic = requestDynamic;
            _cotejarDocumentosClientService = cotejarDocumentosClientService;
            _consultarDocumentoService = consultarDocumentoService;
        }
        public async Task<IActionResult> Index()
        {
            var UrlSetting = _serviceAppSettingsService.GetSettingbyName("RecibosPagoWS");
            var XMLSetting = _serviceAppSettingsService.GetSettingbyName("ReversaDePagoXML");
            RecibosPagoWSRequestModel model = new RecibosPagoWSRequestModel();
            var fecha = DateTime.Now.ToString("yyyy-MM-dd");
            model.FechaReversa = "2023-02-23";
            //model.UsuarioLog = "POEXTSSP_USR";
            //model.PasswordLog = "fV115Kl*xGgV";
            model.UsuarioLog = "sg";
            model.PasswordLog = "1nt3rn0";
            model.ReciboControlInterno = "000000000001";
            RecibosPagoWSResponsetModel modelResponse = new RecibosPagoWSResponsetModel();

            var response = await _requestDynamic.EncryptionService(model, modelResponse, UrlSetting.SettingValue, XMLSetting.SettingValue).ConfigureAwait(false);

            //WebClient();
            //HttpClientCustome();
            //reversaDePagoRequest test = new reversaDePagoRequest();
            //test.FechaReversa = Convert.ToString(DateTime.Now);
            //test.UsuarioLog = "Ruben";
            //test.PasswordLog = "Password";
            //test.ReciboControlInterno = "1234";

            //RecibosPagoWSClient client = new RecibosPagoWSClient();

            //var response = getResponse(client, test);
            //response.Wait();
            //client.CloseAsync();
            //await client.CloseAsync();

            return View();
        }

        public IActionResult CotejarDatosRequest()
        {
            CotejarDatosRequestModel cotejarDatosRequestModel = new CotejarDatosRequestModel();
            cotejarDatosRequestModel.Tp_folio = "4";
            cotejarDatosRequestModel.Folio = "E01038";
            cotejarDatosRequestModel.tp_consulta = "3";


            var endPointName = "CotejarDatosEndPoint";
            var result = _cotejarDocumentosClientService.CotejarDatos(cotejarDatosRequestModel, endPointName);
            ViewBag.Pension = result;
            return View();
        }

        public IActionResult ConsultarDocumento()
        {
            RootConsultarDocumentoRequest rootRequest = new RootConsultarDocumentoRequest();
            MTConsultaDocumento mTConsultaDocumento = new MTConsultaDocumento();
            mTConsultaDocumento.PROCESO = "GENERAL";
            mTConsultaDocumento.DOCUMENTO = "006800006619";
            mTConsultaDocumento.USUARIO = "INNSJACOB";
            mTConsultaDocumento.PASSWORD = "123456";
            rootRequest.MT_Consulta_documento = mTConsultaDocumento;

            var endPointName = "ConsultarDocumentoEndPoint";
            var result = _consultarDocumentoService.ConsultarDocumento(rootRequest, endPointName);
            ViewBag.Pension = result;
            return View();
        }




        #region ejemplos
        //private void CreateMessage()
        //{
        //    WebRequest request = WebRequest.Create("http://www.XXXX.com/Feeds");
        //    string postData = "<airport>Heathrow</airport>";
        //    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
        //    request.ContentType = "application/soap+xml; charset=utf-8";
        //    request.ContentLength = byteArray.Length;

        //    Stream dataStream = request.GetRequestStream();
        //    dataStream.Write(byteArray, 0, byteArray.Length);
        //    dataStream.Close();

        //    // Get the response. 
        //    HttpWebResponse response = request.GetResponse() as HttpWebResponse;

        //    // Display the status. 
        //    HttpContext.Current.Response.Write(((HttpWebResponse)response).StatusDescription);

        //    // Get the stream containing content returned by the server. 
        //    dataStream = response.GetResponseStream();

        //    // Open the stream using a StreamReader for easy access. 
        //    StreamReader reader = new StreamReader(dataStream);

        //    // Read the content. 
        //    string responseFromServer = reader.ReadToEnd();

        //    // Display the content. 
        //    HttpContext.Current.Response.Write(responseFromServer);

        //    // Clean up the streams. 
        //    reader.Close();
        //    dataStream.Close();
        //    response.Close();

        //}

        //private void HttpClientCustome()
        //{
        //    reversaDePagoRequest test = new reversaDePagoRequest();
        //    test.FechaReversa = Convert.ToString(DateTime.Now);
        //    test.UsuarioLog = "Ruben";
        //    test.PasswordLog = "Password";
        //    test.ReciboControlInterno = "1234";

        //    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(test.GetType());
        //    var algoo= x.Serialize(test);


        //    HttpClient httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Add("SOAPAction", "http://spenlinea.guanajuato.gob.mx:8080/SittegWS/RecibosPagoWS/reversaDePago");
        //    //string soapEnvelope = "<s:Envelope xmlns:s= \"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body><Mymethod_Async xmlns=\"http://tempuri.org/\"/></s:Body></s:Envelope>";
        //    var content = new StringContent(test, Encoding.UTF8, "text/xml");
        //    HttpResponseMessage hrm = httpClient.PostAsync("http://spenlinea.guanajuato.gob.mx:8080/SittegWS/RecibosPagoWS?wsdl", content).Result;
        //    var result = hrm.Content.ReadAsStream();
        //    StreamReader reader = new StreamReader(result);
        //    string text = reader.ReadToEnd();
        //}


        private string ToXML()
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(stringwriter, this);
                return stringwriter.ToString();
            }
        }

        public static reversaDePagoRequest LoadFromXMLString(string xmlText)
        {
            using (var stringReader = new System.IO.StringReader(xmlText))
            {
                var serializer = new XmlSerializer(typeof(reversaDePagoRequest));
                return serializer.Deserialize(stringReader) as reversaDePagoRequest;
            }
        }

       
        private async Task<reversaDePagoResponse> getResponse(RecibosPagoWSClient client, reversaDePagoRequest request)
        {
            // get wallet coin list 
            await client.OpenAsync();

            var ret = await client.reversaDePagoAsync(request).ConfigureAwait(false);
            return ret;

        }
        #endregion
    }
}
