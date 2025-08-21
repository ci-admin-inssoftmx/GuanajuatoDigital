
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{

    public class PropietarioModel : EntityModel
    {
        public int? idPropietario { get; set; }

        public string CURP { get; set; }


        public string RFC { get; set; }


        public string nombre { get; set; }

        public string apellidoPaterno { get; set; }


        public string apellidoMaterno { get; set; }


        public string nombreCompleto
        {
            get
            {

                return (nombre ?? "-") + " " + (apellidoPaterno ?? "-") + " " + (apellidoMaterno ?? "-");
            }
        }

        public int idGenero { get; set; }
        public string genero { get; set; }
        public DateTime? fechaNacimiento { get; set; }
        public int? idTipoLicencia { get; set; }
        public string? tipoLicencia { get; set; }
        public string telefono { get; set; }
        public string correo { get; set; }
        public bool generoBool { get; set; }

        public DateTime vigenciaLicencia { get; set; }
        public DateTime vigenciaLicenciaFisico { get; set; }

        public virtual PersonaDireccionModel PersonaDireccion { get; set; }

    }


}