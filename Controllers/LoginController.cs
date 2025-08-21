using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.WSRest;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Security;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
//using Telerik.SvgIcons;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Services;
using System.Net;
using GuanajuatoAdminUsuarios.Models.Generales;
using GuanajuatoAdminUsuarios.LoginController;

namespace GuanajuatoAdminUsuarios.Controllers
{
    public class LoginController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IBitacoraService _bitacoraServices;
        private readonly ICatDelegacionesOficinasTransporteService _catDelegacionesOficinasTransporteService;
        private readonly IPensionesService _pensionesService;
        private readonly IDelegacionesService _delegacionesService;

        public LoginController(IHttpClientFactory httpClientFactory, IBitacoraService bitacoraService, ICatDelegacionesOficinasTransporteService catDelegacionesOficinasTransporteService,
           IPensionesService pensionesService, IDelegacionesService delegacionesService)
        {
            _httpClientFactory = httpClientFactory;
            _bitacoraServices = bitacoraService;
            _catDelegacionesOficinasTransporteService = catDelegacionesOficinasTransporteService;
            _pensionesService = pensionesService;
            _delegacionesService = delegacionesService;

        }
        /* [HttpPost]
         public async Task<IActionResult> ConsumirServicio(string usuario, string contrasena)
         {
             try
             {
                 var usuariosPermitidos = new Dictionary<(string, string), (int, List<int>, string, string)>
         {
             { ("usuarioPec", "Pec123"), (1, new List<int> {     100, 101, 102, 103, 104, 105, 106, 200, 300, 301,
                                                                         302, 303, 304, 400, 401, 402, 500, 501, 502, 600,
                                                                         601, 602, 700, 701, 702, 703, 704, 705, 706, 707,
                                                                         708, 709, 800, 801, 802, 900, 901, 902, 903, 904,
                                                                         905, 906, 907, 908, 909, 910, 911, 912, 913, 914,
                                                                         915, 916, 917, 918, 919, 920, 921, 922, 923, 924,
                                                                         925, 926, 927, 928, 929, 930, 931, 932, 933, 934,
                                                                         935, 936, 937, 938, 939, 940, 941, 942, 943, 944,
                                                                         945, 946, 947, 948, 949, 950, 951, 952, 953, 954,
                                                                         955, 956, 957, 958, 959, 960, 961, 962, 963, 964,
                                                                         965, 966, 967, 968, 969, 970, 971, 972, 973, 974,
                                                                         975, 976, 977, 978, 979, 980, 981, 982, 983, 984,
                                                                         985, 986, 987, 988, 989, 990, 991, 992, 993, 994,
                                                                         995, 996, 997, 998, 999 }, "UsuarioPEC", "PEC") },

             { ("usuarioMovilidad", "Mov123"), (2, new List<int> {    100, 101, 102, 103, 104, 105, 106, 200, 300, 301,
                                                                         302, 303, 304, 400, 401, 402, 500, 501, 502, 600,
                                                                         601, 602, 700, 701, 702, 703, 704, 705, 706, 707,
                                                                         708, 709, 800, 801, 802, 900, 901, 902, 903, 904,
                                                                         905, 906, 907, 908, 909, 910, 911, 912, 913, 914,
                                                                         915, 916, 917, 918, 919, 920, 921, 922, 923, 924,
                                                                         925, 926, 927, 928, 929, 930, 931, 932, 933, 934,
                                                                         935, 936, 937, 938, 939, 940, 941, 942, 943, 944,
                                                                         945, 946, 947, 948, 949, 950, 951, 952, 953, 954,
                                                                         955, 956, 957, 958, 959, 960, 961, 962, 963, 964,
                                                                         965, 966, 967, 968, 969, 970, 971, 972, 973, 974,
                                                                         975, 976, 977, 978, 979, 980, 981, 982, 983, 984,
                                                                         985, 986, 987, 988, 989, 990, 991, 992, 993, 994,
                                                                         995, 996, 997, 998, 999 }, "UsuarioMovilidad", "Movilidad") },
  { ("usuarioPrueba", "Prueba123"), (3, new List<int> {    100, 101, 102, 103, 104, 105, 106, 200, 300, 301,
                                                                         302, 303, 304, 400, 401, 402, 500, 501, 502, 600,
                                                                         601, 601, 700, 701, 702, 703, 704, 705, 706, 707,
                                                                         905, 906, 907, 908, 909, 910, 911, 912, 913, 914,
                                                                         915, 916, 917, 918, 919, 920, 921, 922, 923, 924,
                                                                         925, 926, 927, 928, 929, 930, 931, 932, 933, 934,
                                                                         935, 936, 937, 938, 939, 940, 941, 942, 943, 944,
                                                                         945, 946, 947, 948, 949, 950, 951, 952, 953, 954,
                                                                         955, 956, 957, }, "PruebaPermisos", "Prueba") },
                 };

                 if (usuariosPermitidos.TryGetValue((usuario, contrasena), out var valoresSesion))
                 {
                     var (idOficina, listaIdsPermitidos, nombre, oficina) = valoresSesion;

                     // Asignar valores a las variables de sesión
                     HttpContext.Session.SetInt32("IdOficina", idOficina);
                     HttpContext.Session.SetString("IdsPermitidos", JsonConvert.SerializeObject(listaIdsPermitidos));
                     HttpContext.Session.SetString("Nombre", nombre);
                     HttpContext.Session.SetString("Oficina", oficina);

                     return Json(JsonConvert.SerializeObject(listaIdsPermitidos));
                 }
                 var url = $"https://10.16.157.142:9096/serviciosinfracciones/getlogin?userWS=1&claveWS=18&usuario={usuario}&contraseña={contrasena}";

                 var httpClient = _httpClientFactory.CreateClient();
                 var response = await httpClient.GetAsync(url);

                 if (response.IsSuccessStatusCode)
                 {
                     var content = await response.Content.ReadAsStringAsync();
                     dynamic json = JsonConvert.DeserializeObject(content);

                     if (json != null && json.Count > 0)
                     {
                         string nombre = json[0].nombre;
                         string oficina = json[0].oficina;
                         string idOficinaStr = Regex.Match(oficina, @"\d+").Value;
                         string entidad = Regex.Match(oficina, @"\d+(.+?)\|").Groups[1].Value.Trim();
                         if (int.TryParse(idOficinaStr, out int idOficina))
                         {
                             HttpContext.Session.SetInt32("IdOficina", idOficina);

                         }
                         else
                         {

                         }
                         string delegacion = Regex.Match(oficina, @"\|(.+)").Groups[1].Value.Trim();

                         List<RespuestaServicio> listaRespuestas = JsonConvert.DeserializeObject<List<RespuestaServicio>>(content);
                         string vectorString = listaRespuestas.FirstOrDefault()?.Vector;
                         if (!string.IsNullOrEmpty(vectorString))
                         {
                             List<int> listaIdsPermitidos = vectorString.Split(',').Select(int.Parse).ToList();
                             string listaIdsPermitidosJson = JsonConvert.SerializeObject(listaIdsPermitidos);

                             // Guardar la lista en la variable de sesión
                             HttpContext.Session.SetString("IdsPermitidos", listaIdsPermitidosJson);
                             HttpContext.Session.SetString("Nombre", nombre);
                             HttpContext.Session.SetString("Oficina", oficina);

                             return Json(listaIdsPermitidosJson);
                         }
                     }
                 }

                 // En caso de respuestas inválidas o vacías, limpiar la variable de sesión
                 HttpContext.Session.Remove("IdsPermitidos");
                 HttpContext.Session.Remove("Nombre");
                 HttpContext.Session.Remove("Oficina");

                 return BadRequest("Error en la respuesta del servicio");
             }
             catch (Exception ex)
             {
                 // En caso de errores, limpiar la variable de sesión y manejar el error
                 HttpContext.Session.Clear();
                 return StatusCode(500, $"Error en el servidor: {ex.Message}");
             }
         }

         /* protected void Page_Load(object sender, EventArgs e)
          {

          }
          public async Task<IActionResult> Button1_Click(string usuario, string contrasena)
           {
               Reply oReply = new Reply();
              oReply = await Consumer.Execute<List<Post>>(
                  "https://10.16.157.142:9096/serviciosinfracciones/getlogin?userWS=1&claveWS=18&usuario="+usuario+"&contraseña="+contrasena,
                  methodHttp.GET, 
                  null);
               if (oReply.StatusCode == "OK")
               {
                   List<Post> listPost = (List<Post>)oReply.Data;

                   return View("NombreDeVista", listPost);
               }

               return View("ErrorView", oReply);
           }*/
        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string usuario, string contrasena, int horaUsuario)
        {

            var horaEnviada = horaUsuario;

            try
            {
                HttpContext.Session.Clear();

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
                    {
                        Console.WriteLine("SSL error skipped");
                        return true;
                    }
                };



                using (HttpClient client = new HttpClient(handler))
                {

                    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


                    //string url = $"https://10.16.158.31:9096/serviciosinfracciones/getlogin?userWS=1&claveWS=18&usuario={usuario}&contraseña={contrasena}";
                    string url = $"https://10.16.157.142:9096/serviciosinfracciones/getlogin?userWS=1&claveWS=18&usuario={usuario}&contraseña={contrasena}";

                    var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {

                        string content = await response.Content.ReadAsStringAsync();
                        dynamic json = JsonConvert.DeserializeObject(content);

                        if (json != null && json.Count > 0)
                        {

                            string idUsuario = json[0].idUsuario;

                            AuthManager.SingOutUser(idUsuario);




                            var fechaServidor = DateTime.Now;
                            int horaServidor = (fechaServidor.Hour * 100) + (fechaServidor.Minute);
                            // Calcular la diferencia de tiempo entre la hora del servidor y la hora enviada por el cliente
                            int diferenciaTiempo = Math.Abs(horaEnviada - horaServidor);

                            // Comprobar si la diferencia es mayor o igual a dos minutos
                            if (diferenciaTiempo > 2)
                            {
                                return Json(new { success = false });
                            }

                            string descripcion_corporacion = json[0].descripcion_corporacion;
                            string esPension = "0";
                            if (json[0].tipo_oficina == 3)
                            {
                                esPension = "1";
                            }
                                if (json[0].tipo_oficina == 4)
                            {
                                json[0].tipo_oficina = json[0].id_corporacion;
                                if (json[0].clave_oficina == "100")
                                {
                                    json[0].clave_oficina = Convert.ToString(json[0].id_oficina);
                                    json[0].clave_depOficina = Convert.ToString(json[0].id_oficina);
                                }
                            }

                            if (!string.IsNullOrEmpty(descripcion_corporacion) &&
                                !_catDelegacionesOficinasTransporteService.ExistCorp((int)json[0].tipo_oficina))
                            {
                               var id= _catDelegacionesOficinasTransporteService.NewCorp((int)json[0].tipo_oficina, descripcion_corporacion);
                            }

                            if (!_catDelegacionesOficinasTransporteService.ExistDelegacion((int)json[0].clave_oficina))
                            {
                                var id = _catDelegacionesOficinasTransporteService.NewDelegacion((int)json[0].clave_oficina, (string)json[0].descripcion_corporacion, (int)json[0].tipo_oficina);
                                json[0].clave_oficina = id.ToString();
                                json[0].clave_depOficina = id.ToString();
                            }
                            if (!_catDelegacionesOficinasTransporteService.ExistDelegacionMun((int)json[0].clave_oficina))
                            {
                                var idMun = _catDelegacionesOficinasTransporteService.NewMunicipio((string)json[0].descripcion_corporacion, (int)json[0].tipo_oficina, (int)json[0].clave_oficina);

                                var id = _catDelegacionesOficinasTransporteService.NewDelegacionMun((int)json[0].clave_oficina, (string)json[0].descripcion_corporacion, (int)json[0].tipo_oficina, idMun);
                                json[0].clave_oficina = id.ToString();
                                json[0].clave_depOficina = id.ToString();
                            }
                            string usuarioLogin = usuario;
                            string nombre = json[0].nombre;
                            string oficina = json[0].oficina;
                            string idDependenciaStr = json[0].tipo_oficina;


                            if (int.TryParse(idDependenciaStr, out int idDependencia))
                            {
                                if (idDependencia == 2)
                                {
                                    HttpContext.Session.SetInt32("IdDependencia", 0);
                                }
                                else
                                {
                                    HttpContext.Session.SetInt32("IdDependencia", idDependencia);
                                }
                            }
                            else
                            {
                                // Manejo para el caso en que no se pueda convertir a entero
                            }
                            string strIdPension = json[0].clave_pension;
                            if (int.TryParse(strIdPension, out int idPension))
                            {
                                HttpContext.Session.SetInt32("IdPension", idPension);

                            }
                            else
                            {

                            }
                            string pension = _pensionesService.GetPensionLogin(idPension);

                            string idOficinaStr = json[0].clave_oficina;
                            string idOfic = json[0].clave_depOficina;
                            // string idDependenciaStr = json[0].tipo_oficina;
                            
                            string TipoOfi = json[0].tipo_oficina == "2" ? "0" : json[0].tipo_oficina;

                            if (int.TryParse(idOficinaStr, out int idOficina))
                            {
                                HttpContext.Session.SetInt32("IdOficina", idOficina);

                            }
                            else
                            {

                            }
                            //var nombreOficina = json[0].oficina;//_catDelegacionesOficinasTransporteService.GetDelegacionOficinaById(idOficina);

                            //Se obtiene la abreviatura del municipio asociado a la delegacion
                            string abreviaturaMunicipio = _delegacionesService.getAbreviaturaMunicipio(idOficina);
                            CatalogModel extensions = _delegacionesService.GetExtensions();

                            await SignInUser(idUsuario, nombre, oficina, pension, TipoOfi, abreviaturaMunicipio, usuarioLogin, idOfic, HttpContext.Connection.RemoteIpAddress.ToString(),extensions.value,extensions.text, esPension);
                            _bitacoraServices.BitacoraGenerales(CodigosGeneral.C5001, "", idUsuario, HttpContext.Connection.RemoteIpAddress.ToString(), idOfic);

                            //BITACORA.
                            //var user = Convert.ToDecimal(User.FindFirst(CustomClaims.IdUsuario).Value);
                            _bitacoraServices.BitacoraGeneral("-", "-", OperacionBitacora.lOGIN, usuarioLogin, HttpContext.Connection.RemoteIpAddress.ToString());


                            string delacion = Regex.Match(oficina, @"\|(.+)").Groups[1].Value.Trim();

                            List<RespuestaServicio> listaRespuestas = JsonConvert.DeserializeObject<List<RespuestaServicio>>(content);
                            string vectorString = listaRespuestas.FirstOrDefault()?.Vector;
                            string autorizacionesString = listaRespuestas.FirstOrDefault()?.autorizaciones;
                            List<int> listaPermisos = autorizacionesString
                                  .Split(',')
                                  .Where(s => !string.IsNullOrWhiteSpace(s)) // Filtrar cadenas vacías o nulas
                                  .Select(int.Parse)
                                  .ToList();

                            string listaPermisosJson = JsonConvert.SerializeObject(listaPermisos);

                            HttpContext.Session.SetString("IdsPermitidos", listaPermisosJson);

                            List<int> listaIdsPermitidos = vectorString.Split(',').Select(int.Parse).ToList();
                            string listaIdsPermitidosJson = JsonConvert.SerializeObject(listaIdsPermitidos);

                            if (!string.IsNullOrEmpty(listaIdsPermitidosJson))
                            {
                                // Guardar la lista en la variable de sesión
                                HttpContext.Session.SetString("IdsPermitidos", listaIdsPermitidosJson);
                                HttpContext.Session.SetString("Autorizaciones", listaPermisosJson);
                                HttpContext.Session.SetString("Nombre", nombre);
                                HttpContext.Session.SetString("Oficina", oficina);
                                // HttpContext.Session.SetInt32("IdDependencia", idDependencia);
                                return Json(new { success = true, data = listaIdsPermitidosJson });
                            }
                            else
                            {
                                Console.WriteLine("VACIO");
                            }


                        }
                    }



                    // En caso de respuestas inválidas o vacías, limpiar la variable de sesión
                    HttpContext.Session.Remove("IdsPermitidos");
                    HttpContext.Session.Remove("Nombre");
                    HttpContext.Session.Remove("Oficina");
                    HttpContext.Session.Remove("IdDependencia");


                    return BadRequest("Error en la respuesta del servicio");
                }
            }
            catch (Exception ex)
            {
                HttpContext.Session.Clear();
                return StatusCode(500, $"Error en el servidor: {ex.Message}");
            }
        }

        private static bool CertCheck(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }


        [HttpGet]
        public IActionResult GetIdsPermitidos()
        {
            var idsPermitidosJson = HttpContext.Session.GetString("IdsPermitidos");


            var idsPermitidos = JsonConvert.DeserializeObject<List<int>>(idsPermitidosJson) ?? new List<int>();
            return Json(idsPermitidos);
        }



        private async Task SignInUser(string idUsuario, string nombre, string oficina, string pension, string TipoOfi, string abreviaturaMunicipio, string usuarioLogin, string dependenciaOficina = "0", string ip = "::1",string size="5",string extencions = "", string esPension = "")
        {
            var idguid = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim(CustomClaims.IdUsuario, idUsuario),
                new Claim(CustomClaims.OficinaDelegacion, dependenciaOficina),
                new Claim(CustomClaims.Nombre, nombre),
                new Claim(CustomClaims.Ip, ip),
                new Claim(CustomClaims.Oficina, oficina),
                new Claim(CustomClaims.TipoOficina, TipoOfi),
                new Claim(CustomClaims.Pension, pension),
                new Claim(CustomClaims.AbreviaturaMunicipio, abreviaturaMunicipio),
                new Claim(CustomClaims.Usuario, usuarioLogin),
                new Claim(CustomClaims.GUID, idguid.ToString()),
                new Claim(CustomClaims.size, size),
                new Claim(CustomClaims.extencions, extencions),
                 new Claim(CustomClaims.esPension, esPension),

            };


            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            
            AuthManager.AddNewUser(idguid.ToString(), idUsuario);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }


        public IActionResult Logout()
        {

            _bitacoraServices.BitacoraGenerales(CodigosGeneral.C5002, "");


            HttpContext.Session.Clear();
            return Redirect("/inicio");
        }


    }
}
