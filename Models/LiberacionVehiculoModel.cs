using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class LiberacionVehiculoModel
    {
        #region Depositos
        public int IdDeposito { get; set; }

        public int IdSolicitud { get; set; }

        public int IdDelegacion { get; set; }

        public int IdMarca { get; set; }

        public int IdSubmarca { get; set; }

        public int IdPension { get; set; }

        public int IdTramo { get; set; }

        public int IdColor { get; set; }

        public string Serie { get; set; }

        public string Placa { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaIngreso { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaLiberacion { get; set; }

        public string Folio { get; set; }

        public string Km { get; set; }

        public int Liberado { get; set; }

        //[Required(ErrorMessage = "Please select file.")]
        //[RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif)$", ErrorMessage = "Only Image files allowed.")]
        //public HttpPostedFileBase PostedFile { get; set; }
        public byte[] AcreditacionPropiedad { get; set; }

        public string AcreditacionPropiedadStr { get; set; }

        public byte[] AcreditacionPersonalidad { get; set; }
        public string AcreditacionPersonalidadStr { get; set; }

        public byte[] ReciboPago { get; set; }
        public string ReciboPagoStr { get; set; }

        public string Observaciones { get; set; }
        public string ObservacionesPropiedad { get; set; }
        public string ObservacionesInfraccion { get; set; }
        public string ObservacionesPersonalidad { get; set; }


        public string Autoriza { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public int ActualizadoPor { get; set; }

        public int Estatus { get; set; }

        public string Data { get; set; }
        public string ImageAcreditacionPropiedadPath { get; set; }
        public string ImageAcreditacionPersonalidadPath { get; set; }
        public string ImageReciboPagoPath { get; set; }
        public string FormatoManualPath { get; set; }
        #endregion

        #region Marcas,SubMarca,Delegacion, solicitante,color,pension,tramo, carretera
        public string marcaVehiculo { get; set; }

        public string nombreSubmarca { get; set; }

        public string delegacion { get; set; }

        public string solicitanteNombre { get; set; }

        public string solicitanteAp { get; set; }

        public string solicitanteAm { get; set; }

        public string fullName
        {
            get
            {
                return solicitanteNombre?.ToString()??"" + " " +
                solicitanteAp + " " +
                solicitanteAm;
            }
        }

        public string Color { get; set; }

        public string pension { get; set; }

        public string tramo { get; set; }

        public int IdCarretera { get; set; }

        public string carretera { get; set; }
        public string nombrePropietario { get; set; }
        public string apPaternoPropietario { get; set; }
        public string apMaternoPropietario { get; set; }
        public string fullPropietario
        {
            get
            {
                return nombrePropietario + " " +
                apPaternoPropietario + " " +
                apMaternoPropietario;
            }
        }
        public int idInfraccion { get; set; }

        #endregion


    }
}
