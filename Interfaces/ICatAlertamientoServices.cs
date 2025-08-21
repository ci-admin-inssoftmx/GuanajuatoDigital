using DocumentFormat.OpenXml.Office2010.ExcelAc;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatAlertamientoServices
    {

        public List<AlertamientoGridModel> GetdataGrid(int corp);
        public AlertamientoGridModel Getdata(int IdAlertamiento);
        public int EditarAlertamiento(int IdAlertamiento,int cantidad);
        public int CrearAlertamiento(int cantidad, int idAplicacion, int Delegacion);
        public List<CatalogModel> GetCorpCatalog();
        public List<CatalogModel> GetAplicadaCatalog(int corp);

    }
}
