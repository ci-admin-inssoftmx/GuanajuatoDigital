using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using System.Diagnostics;
using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class Infracciones1Model
    {
        public int IdInfraccion { get; set; }
        public string folioInfraccion { get; set; }
        public string placas { get; set; }
        public int idOficial { get; set; }
        public int idDependencia { get; set; }
        public int idDelegacion { get; set; }
        public string delegacion { get; set; }
        public string oficial { get; set; }
        public string municipio { get; set; }
        public DateTime fechaInfraccion { get; set; }
        public string carretera { get; set; }
        public string tramo { get; set; }
        public string kmCarretera { get; set; }
        public int idVehiculo { get; set; }
        public int idConductor { get; set; }
        public string conductor { get; set; }
        public string propietario { get; set; }
        public int idAplicacion { get; set; }
        public bool infraccionCortesia { get; set; }
        public string observaciones { get; set; }
        public int idGarantia { get; set; }
        public string garantia { get; set; }
        public int idEstatusInfraccion { get; set; }
        public DateTime fechaActualizacion { get; set; }
        public int actualizadoPor { get; set; }
        public int estatus { get; set; }


        #region NuevasColumnas

        public string lugarCalle { get; set; }
        public string lugarNumero { get; set; }
        public string lugarColonia { get; set; }
        public string lugarentreCalle { get; set; }
        public int idMunicipio { get; set; }
        public int idTramo { get; set; }
        public int idCarretera { get; set; }
        public string NumTarjetaCirculacion { get; set; }
        public int idPropietario { get; set; }

        #endregion

        public string FullFechaConductor
        {
            get
            {
                return fechaInfraccion.ToString("dd/MM/yyyy") + " " +
                 conductor;

            }


        }

    }
}
