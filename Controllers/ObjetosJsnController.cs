using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.RESTModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using GuanajuatoAdminUsuarios.Framework;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GuanajuatoAdminUsuarios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjetosJsnController : ControllerBase
    {
        private readonly IInfraccionesService _infraccionesService;

        public ObjetosJsnController( IInfraccionesService infraccionesService)
        {
            _infraccionesService = infraccionesService;
        }

        // GET: api/<ObjetosJsnController>
        [HttpGet("nombre/{id}")]
        public string LeerNombre(int id)
        {
            return id switch
            {
                1 => "Net",
                2 => "mentor",
                _ => throw new System.NotImplementedException()
            };
        }

        // POST api/<ObjetosJsnController>
        [HttpGet("Objetoinf/{idInfraccion}/{idDependencia}")]

        public string Objetoinf(int idInfraccion, int idDependencia)
        {
            //int idDependencia = (int)HttpContext.Session.GetInt32("IdDependencia");

            try
            {
                var infraccionBusqueda = _infraccionesService.GetInfraccionById(idInfraccion, idDependencia);

                CrearMultasTransitoRequestModel crearMultasRequestModel = new CrearMultasTransitoRequestModel();

                PersonaModel Persona = infraccionBusqueda.PersonaInfraccion2;
                var validrfc = 13;

                if (infraccionBusqueda.idAplicacion == 1)
                {

                    Persona = infraccionBusqueda.PersonaInfraccion2;
                }
                if (infraccionBusqueda.idAplicacion == 2 || infraccionBusqueda.idAplicacion == 4)
                {
                    Persona = infraccionBusqueda.Persona;
                }
                if (infraccionBusqueda.idAplicacion == 3)
                {
                    if (infraccionBusqueda.PersonaInfraccion2?.nombre.ToLower() != "se ignora")
                    {
                        Persona = infraccionBusqueda.PersonaInfraccion2;
                    }
                    else
                    {
                        Persona = infraccionBusqueda.Persona;
                    }
                }
                if (Persona?.tipoPersona == "1")
                {
                    validrfc = 13;
                }
                else
                {
                    validrfc = 12;
                }
                string prefijo = (idDependencia == 1) ? "TTO-" : (idDependencia == 0) ? "TTE-" : "";

                crearMultasRequestModel.CR1RFC = (Persona?.RFC ?? "").Length == validrfc ? Persona?.RFC : (prefijo + infraccionBusqueda.folioInfraccion.ToUpper());


                if ((Persona?.tipoPersona ?? "Persona física") == "Persona física")
                {
                    crearMultasRequestModel.CR1APAT = (Persona?.apellidoPaterno ?? "").Cut(40);
                    crearMultasRequestModel.CR1AMAT = (Persona?.apellidoMaterno ?? "").Cut(40);
                    crearMultasRequestModel.CR1NAME = (Persona?.nombre ?? "").Cut(40);
                }
                else
                {
                    crearMultasRequestModel.CR1RAZON = Persona?.nombre ?? "";
                }



                crearMultasRequestModel.BIRTHDT = Persona?.fechaNacimiento?.ToString("yyyy-MM-dd") ?? "1900-01-01";


                crearMultasRequestModel.CR1CALLE = (Persona?.PersonaDireccion?.calle ?? "" ?? "").Cut(60);
                crearMultasRequestModel.CR1NEXT = (Persona?.PersonaDireccion?.numero ?? "" ?? "").Cut(10);
                crearMultasRequestModel.CR1NINT = "";
                crearMultasRequestModel.CR1ENTRE = "";
                crearMultasRequestModel.CR2ENTRE = "";
                crearMultasRequestModel.CR1COLONIA = (Persona?.PersonaDireccion?.colonia ?? "" ?? "").Cut(40);
                crearMultasRequestModel.CR1LOCAL = "";
                crearMultasRequestModel.CR1MPIO = (Persona?.PersonaDireccion?.municipio ?? "" ?? "").Cut(40);
                crearMultasRequestModel.CR1CP = (Persona?.PersonaDireccion?.codigoPostal ?? "" ?? "").Cut(10);
                crearMultasRequestModel.CR1TELE = (Persona?.PersonaDireccion?.telefono ?? "" ?? "").Cut(30);
                crearMultasRequestModel.CR1EDO = ("GTO").Cut(3);
                crearMultasRequestModel.CR1EMAIL = (Persona?.PersonaDireccion?.correo ?? "" ?? "").Cut(241);

                if ((Persona?.genero ?? "0") == "MASCULINO")
                {
                    crearMultasRequestModel.XSEXM = "1";
                }
                else
                {
                    crearMultasRequestModel.XSEXF = "1";
                }

                var placaStr = infraccionBusqueda.placasVehiculo.Replace("-", "");
                var nombreinfra = (Persona?.nombreCompleto ?? "").Length < 61 ? (Persona?.nombreCompleto ?? "") : (Persona?.nombreCompleto ?? "").Substring(0, 60);
                var nombreResp = (infraccionBusqueda.Persona?.nombreCompleto ?? "").Length < 61 ? (infraccionBusqueda.Persona?.nombreCompleto ?? "") : (infraccionBusqueda.Persona?.nombreCompleto ?? "").Substring(0, 60);


                crearMultasRequestModel.IMPORTE_MULTA = infraccionBusqueda.totalInfraccion.ToString("F2");
                crearMultasRequestModel.FEC_IMPOSICION = infraccionBusqueda.fechaInfraccion.ToString("yyyy-MM-dd");
                crearMultasRequestModel.FEC_VENCIMIENTO = infraccionBusqueda.fechaVencimiento.ToString("yyyy-MM-dd");
                crearMultasRequestModel.NOM_INFRACTOR = nombreinfra;
                crearMultasRequestModel.DOM_INFRACTOR = (Persona?.PersonaDireccion.calle ?? "" + " " + Persona?.PersonaDireccion.numero ?? "" + ", " + Persona?.PersonaDireccion.colonia ?? "").Cut(60);
                crearMultasRequestModel.NUM_PLACA = (placaStr).Cut(7);
                crearMultasRequestModel.DOC_GARANTIA = infraccionBusqueda.idGarantia.ToString();
                crearMultasRequestModel.NOM_RESP_SOLI = nombreResp;
                crearMultasRequestModel.DOM_RESP_SOLI = ((infraccionBusqueda.Persona?.PersonaDireccion.calle ?? "") + " " + (infraccionBusqueda.Persona?.PersonaDireccion.numero ?? "") + ", " + (infraccionBusqueda.Persona?.PersonaDireccion.colonia ?? "")).Cut(150);
                crearMultasRequestModel.FOLIO_MULTA = (prefijo + infraccionBusqueda.folioInfraccion.ToUpper()).Cut(20);
                crearMultasRequestModel.OBS_GARANT = (infraccionBusqueda.NombreGarantia + " " + (infraccionBusqueda.idGarantia == 1 ? infraccionBusqueda.Garantia.numPlaca : infraccionBusqueda.idGarantia == 2 ? infraccionBusqueda.Garantia.numLicencia : infraccionBusqueda.idGarantia == 3 ? "-" : infraccionBusqueda.Vehiculo.placas)).Cut(100);


                int count = 1;
                foreach (var enumeration in infraccionBusqueda.MotivosInfraccion)
                {
                    if (count == 1) { crearMultasRequestModel.ZMOTIVO1 = enumeration.Motivo.Cut(250); }
                    if (count == 2) { crearMultasRequestModel.ZMOTIVO2 = enumeration.Motivo.Cut(250); }
                    if (count == 3) { crearMultasRequestModel.ZMOTIVO3 = enumeration.Motivo.Cut(250); }
                    count++;
                }

                var json = JsonConvert.SerializeObject(crearMultasRequestModel, Formatting.Indented);
                var bodyRequest = json;
                return bodyRequest;

            }
            catch (SqlException ex)
            {
                return "Erros en obtenr Objeto";
            }
        }


        [HttpPost]
        public string SaveNewValue([FromBody] string value)
        {
            return "ok";
        }


        // PUT api/<ObjetosJsnController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ObjetosJsnController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
