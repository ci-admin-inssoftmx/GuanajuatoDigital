using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Controllers
{



    [Route("api/[controller]")]
    [ApiController]
    public class LicenciasController : BaseController
    {
        private ILicenciasService licenciasService;

        public LicenciasController(ILicenciasService licenciasService)
        {
            this.licenciasService = licenciasService;
        }

        [HttpGet]
        [Route("datos_generales")]
        //  public ActionResult ObtenerDatosGeneralesPersona(
        public List<LicenciaPersonaDatos> ObtenerDatosGeneralesPersona(
                                                   [FromQuery(Name = "licencia")] string licencia
                                                 , [FromQuery(Name = "curp")] string curp
                                                 , [FromQuery(Name = "rfc")] string rfc
                                                 , [FromQuery(Name = "nombre")] string nombre
                                                 , [FromQuery(Name = "primer_apellido")] string primer_apellido
                                                 , [FromQuery(Name = "segundo_apellido")] string segundo_apellido
           )
        {
            try
            {
                List<LicenciaPersonaDatos> persona = licenciasService.ObtenerDatosPersona(licencia, curp, rfc, nombre, primer_apellido, segundo_apellido);
                //LicenciaPersonaDatos persona =new LicenciaPersonaDatos();
             /*   if (persona == null)
                {
                //    persona = licenciasService.ObtenerDatosPersonaBD3(licencia, curp, rfc, nombre, primer_apellido, segundo_apellido);
                    if (persona == null)
                    {
                  //      persona = licenciasService.ObtenerDatosPersonaBD1(licencia, curp, rfc, nombre, primer_apellido, segundo_apellido);
                        if (persona == null)
                        {
                    //        persona = licenciasService.ObtenerDatosPersonaBD2(licencia, curp, rfc, nombre, primer_apellido, segundo_apellido);
                            if (persona == null)
                            {
                                return Ok(new LicenciaRespuestaPersona
                                {
                                    tipo = LicenciaTipoRespuesta.respuestas.warning.ToString(),
                                    mensaje = "No se obtuvieron resultados.",
                                    datos = null
                                });
                            }
                            else
                            {
                                return Ok(new
                                {
                                    tipo = LicenciaTipoRespuesta.respuestas.success.ToString(),
                                    mensaje = "Datos obtenidos correctamente.",
                                    datos = persona
                                });
                            }
                        }
                        else
                        {
                            return Ok(new
                            {
                                tipo = LicenciaTipoRespuesta.respuestas.success.ToString(),
                                mensaje = "Datos obtenidos correctamente.",
                                datos = persona
                            });
                        }
                    }
                    else
                    {
                        return Ok(new
                        {
                            tipo = LicenciaTipoRespuesta.respuestas.success.ToString(),
                            mensaje = "Datos obtenidos correctamente.",
                            datos = persona
                        });
                    }

                }
                */
                //   return Ok(new
                //  {
                //     tipo = LicenciaTipoRespuesta.respuestas.success.ToString(),
                //    mensaje = "Datos obtenidos correctamente.",
                //   datos = persona
                // });
                return (persona);

            }
            catch (Exception excepcion)
            {
          //      return UnprocessableEntity(new LicenciaRespuestaPersona
           //     {
            //        tipo = LicenciaTipoRespuesta.respuestas.error.ToString(),
             //       mensaje = "Ocurrió un error al obtener los datos. " + excepcion.Message +"; " + excepcion.InnerException,
              //      datos = null
               // });
               return(null);
            }
        }
    }
}
