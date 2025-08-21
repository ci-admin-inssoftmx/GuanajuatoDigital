using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GuanajuatoAdminUsuarios.Common;
using Microsoft.AspNetCore.Razor.Language;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GuanajuatoAdminUsuarios.Models
{
    public class PersonaModel : EntityModel
    {
     
        
        public string MoralNombre { get; set; }
        public string moralrfc { get; set; }
        public int? idPersona { get; set; }
        public string numeroLicencia { get; set; }
        public string numeroLicenciaBusqueda { get; set; }
        public string numeroLicenciaFisico { get; set; }
        public int? IdPersona { get; set; }

        
        public string CURP { get; set; } = "";


        [MinLength(10, ErrorMessage = "El CURO debe tener al menos 10 caracteres.")]
        public string CURPBusqueda { get; set; }
        public string? banderaConductor { get; set; }
        public string CURPFisico { get; set; }

        public string RFC { get; set; }
        [MinLength(10, ErrorMessage = "El RFC debe tener al menos 10 caracteres.")]
        public string RFCBusqueda { get; set; }
        public string RFCFisico { get; set; }

        public string nombre { get; set; }
        public string nombreBusqueda { get; set; }
        public string nombreFisico { get; set; }

        public string apellidoPaterno { get; set; }
        public string apellidoPaternoBusqueda { get; set; }
        public string apellidoPaternoFisico { get; set; }

        public string apellidoMaterno { get; set; }
        public string apellidoMaternoBusqueda { get; set; }
        public string apellidoMaternoFisico { get; set; }

        public string nombreCompleto
        {
            get
            {

                return (nombre ?? "-") + " " + (apellidoPaterno ?? "-") + " " + (apellidoMaterno ?? "-");
            }
        }
        public int? idCatTipoPersona { get; set; }
        public string? tipoPersona { get; set; }
         public int? idVigencia { get; set; }
        
        public string? vigencia { get; set; }

        public int? idGenero { get; set; }
        public string genero { get; set; }
        public DateTime? fechaNacimiento { get; set; }
        public int? idTipoLicencia { get; set; }
        public int? idTipoLicenciaInfraccion { get; set; }
        public string? telefonoInfraccion { get; set; }
        public string? tipoLicencia { get; set; }
        public string? telefono { get; set; }

        public string? codigoPostal { get; set; }

        public string? colonia { get; set; }

        public string? calle { get; set; }

        public string? numero { get; set; }
        public string correo { get; set; }
        public string correoInfraccion { get; set; }
        public string Origen { get; set; }
        public bool generoBool { get; set; }

        public DateTime? vigenciaLicencia { get; set; }
        public DateTime? vigenciaLicenciaFisico { get; set; }

        public virtual PersonaDireccionModel PersonaDireccion { get; set; } = new PersonaDireccionModel();

        public int total { get; set; }
        public int idInfraccion { get; set; }
        public string folioDetencion { get; set; }

        public void ConvertirModeloDeLicencias(LicenciaPersonaDatos p)
        {
            TipoLicencia tipoLic = Constants.tipoLicencias.SingleOrDefault(item => item.Descripcion.Equals(p.TIPOLICENCIA));
            int vigecina = p.Vigencia > 3 ? 4 : (int)p.Vigencia;

            nombre = p.NOMBRE;
            apellidoPaterno = p.PRIMER_APELLIDO;
            apellidoMaterno = p.SEGUNDO_APELLIDO;
            fechaNacimiento = (p.FECHA_NACIMIENTO.Value.Year < 1800) ? null : p.FECHA_NACIMIENTO.Value;
            vigenciaLicencia = (p.FECHA_TERMINO_VIGENCIA.Value.Year < 1800) ? null : p.FECHA_TERMINO_VIGENCIA.Value;
            CURP = p.CURP;
            RFC = p.RFC;
            numeroLicencia = p.NUM_LICENCIA;
            idTipoLicencia = tipoLic != null ? tipoLic.Id : 0;
            tipoLicencia = tipoLic == null ? "" : tipoLic.Descripcion;
            calle = p.CALLE;
            numero = p.NUM_EXT;
            colonia = p.COLONIA;
            idGenero = p.ID_GENERO == null ? 1 : Convert.ToInt16(p.ID_GENERO);
            generoBool = idGenero == 1;
            Origen = "Licencias";
            idVigencia = vigecina;
            PersonaDireccion = new PersonaDireccionModel
            {
                idMunicipio = p.ID_MUNICIPIO,
                municipio = p.MUNICIPIO,
                codigoPostal = p.CP,
                entidad = p.ESTADO_NACIMIENTO,
                telefono = p.TELEFONO1,
                correo = p.EMAIL,
            };
        }
    }

}

