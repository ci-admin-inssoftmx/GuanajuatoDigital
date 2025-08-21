/*
 * Descripción:
 * Proyecto: Commons
 * Fecha de creación: Tuesday, February 20th 2024 10:36:45 am
 * Autor: Osvaldo S. (osvaldo.sanchez@zeitek.net)
 * -----
 * Última modificación: Wed Feb 21 2024
 * Modificado por: Osvaldo S.
 * -----
 * Copyright (c) 2023 - 2024 Accesos Holográficos
 * -----
 * HISTORIAL:
 */
namespace GuanajuatoAdminUsuarios.Models
{
  public class VehiculoPropietarioBusquedaModel
  {
    public int IdEntidadBusqueda { get; set; }
    public string PlacaBusqueda { get; set; }
    public string SerieBusqueda { get; set; }
    public VehiculoModel Vehiculo { get; set; }

  }


    public class VehiculoEditViewModel
    {
        public string id { get; set; }
        public string idEntidad { get; set; }
        public string placas { get; set; }
        public string serie { get; set; }
        public string tarjeta { get; set; }
        public string idColor { get; set; }
        public string idColorReal { get; set; }
        public string vigenciaTarjeta { get; set; }
        public string ddlMarcas { get; set; }
        public string idSubmarca { get; set; }
        public string idTipoVehiculo { get; set; }
        public string modelo { get; set; }
        public string numeroEconomico { get; set; }
        public string paisManufactura { get; set; }
        public string motor { get; set; }
        public string motorActual { get; set; }
        public string capacidad { get; set; }
        public string poliza { get; set; }
        public string otros { get; set; }
        public string cargaSwitch { get; set; }
        public string ddlCatTipoServicio { get; set; }
        public string ddlCatSubTipoServicio { get; set; }

        public int idInfraccion { get; set; }
    }


}