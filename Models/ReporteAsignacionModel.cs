using GuanajuatoAdminUsuarios.Entity;
using System;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class ReporteAsignacionModel
    {
        public int idSolicitud { get; set; }
        public int pensionEstatus { get; set; }

        public string vehiculoCarretera { get; set; }
        public string carretera { get; set; }
        public string tramo { get; set; }

        public string vehiculoTramo { get; set; }
        public string vehiculoKm { get; set; }
        public string fechaSolicitud2 { get; set; }
        public string fechaLiberacion2 { get; set; }
        public string descripcionEstatus { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime fechaSolicitud { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? fechaLiberacion { get; set; }

      
        public string evento { get; set; }

        public string solicitanteNombre { get; set; }
        public string solicitanteAp { get; set; }
        public string solicitanteAm { get; set; }

        public string fullName
        {
            get
            {
                return solicitanteNombre.ToString() + " " +
                solicitanteAp + " " +
                solicitanteAm;
            }
        }

        //Direccion
        public string solicitanteColonia { get; set; }
        public string solicitanteCalle { get; set; }
        public string solicitanteNumero { get; set; }

        public string fullcolonia
        {
            get
            {
                return solicitanteColonia.ToString() + " " +
                solicitanteCalle + " " +
                solicitanteNumero;
            }
        }

        public string tipoVehiculo { get; set; }

        //ToDo:sobre que filtro grua? ya que el drop esta sobre id y placa
        //este se mostrara para  el filtro de grua
        //tbl gruas
        public string noEconomico { get; set; }
        public int IdGrua { get; set; } 
        
        public string propietarioGrua { get; set; }

        //ToDo no esta alias seria sacarlo de la tabla que cree con join de Concesionario
        public string Alias { get; set; }
        public string folio { get; set; }


        /// <summary>
        /// Filtro por id pesiones mostrar nombre pension
        /// </summary>
        public string vehiculoPension { get; set; }

        //ToDo no esta delegacion hacer join para llegar a delegacion

        /// <summary>
        /// tbl delegacion
        /// </summary>
        public string Delegacion { get; set; }

        //ToDo no esta motivo
        //ToDo no esta inventario 

        public string servicio { get; set; }
        public string tipoUsuario { get; set; }
        public string solicitanteTel { get; set; }
        public string solicitanteEntidad { get; set; }
        public string solicitanteMunicipio { get; set; }
        public string vehiculoCalle { get; set; }
        public string vehiculoNumero { get; set; }
        public string vehiculoColonia { get; set; }

        public string vehiculoEntidad { get; set; }
        public string vehiculoMunicipio { get; set; }

        public string vehiculoInterseccion { get; set; }

        public int idVehiculo { get; set; }
        public int idInfraccion { get; set; }
        public DateTime fechaActualizacion { get; set; }
        public string actualizadoPor { get; set; }
        public string estatus { get; set; }
        //public int IdDependencia { get; set; }
        public int idMotivo { get; set; }
        public string motivoAsignacion { get; set; }
        public string numeroIventario { get; set; }
        public string apellidoPaternoOficial { get; set; }
        public string apellidoMaternoOficial { get; set; }
        public string nombreOficial { get; set; }
        public string oficial
        { 
             get
        {
            // Concatena los valores de los tres campos
            return $"{apellidoPaternoOficial} {apellidoMaternoOficial} {nombreOficial}";
        }
       }



    }
}
