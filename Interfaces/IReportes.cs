using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IReportes
    {
        List<ReportesModel> BusquedaInfPorTipoLicencia(int corp); 
        List<ReportesModel> BusquedaInfraccionesAccidentesMunicipio(int corp); 
        List<ReportesModel> BusquedaInfTodasCorporaciones();
        List<ReportesModel> BusquedaAccidentesTodasCorporaciones();
       // List<ReportesModel> BusquedaInfraccionesTipoLicencia(int corp); 
        List<ReportesModel> BusquedaInfraccionesPorLicencia(string txtLicencia);
        List<ReportesModel> BusquedaMunicipiosMasAccidentes(int corp); 
        List<ReportesModel> BusquedaMunicipiosMasInfracciones(int corp); 
        List<ReportesModel> BusquedaMunicipiosMasAccidentesB();
        List<ReportesModel> BusquedaMunicipiosColoniasMasInfracciones(int corp);       
        List<ReportesModel> BusquedaMunicipiosColoniasMasAccidentes();
        List<ReportesModel> BusquedaDanosAccidentes(); 
        List<ReportesModel> BusquedaInfraccionesProDiayHora();

    }
}
