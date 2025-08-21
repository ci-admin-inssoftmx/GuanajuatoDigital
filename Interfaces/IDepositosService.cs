using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
        public interface IDepositosService
        {
                string GuardarSolicitud(SolicitudDepositoModel model, int idOficina, string oficina, string abreviaturaMunicipio, int anio, int dependencia, int usuario);
                SolicitudDepositoModel ObtenerSolicitudPorID(int Isol);
                int ReturnIdDeposito(int IdSolicitud);
                (string Folio, int? IdVehiculo) ActualizarSolicitud(int? Isol, SolicitudDepositoModel model);

                int CompletarSolicitud(SolicitudDepositoModel model);

                SolicitudDepositoModel ImportarInfraccion(string folioBusquedaInfraccion, int idDependencia);

                CatalogModel GetPathFile(string id);

                SolicitudDepositoModel ImportarInfraccion(int folioBusquedaInfraccion, int idDependencia);

                List<SolicitudDepositoModel> ObtenerServicios();

                DepositosModel GetDepositoByFolioSolicitud(string folioSolicitud);

                void CambiarDepositoVehiculo(int idDeposito, int idSolicitud, int idVehiculo);
        }
}
