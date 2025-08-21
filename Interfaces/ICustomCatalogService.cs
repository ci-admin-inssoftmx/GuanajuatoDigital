using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICustomCatalogService
    {
        List<CatalogModel> GetDelegaciones();
        List<CatalogModel> GetDelegaciones(int Corp);
        List<CatalogModel> GetFactorAccidente(int corp);
        List<CatalogModel> GetAllCarreteras();
        List<CatalogModel> GetCarreterasByDelegacion(int idDelegacion);

        List<CatalogModel> GetTiposCargaActivos();
        List<CatalogModel> GetClasificacionesAccidentesActivas();
        List<CatalogModel> GetClasificacionesAccidentesActivas(int corp);
        List<CatalogModel> GetCausasAccidentesActivas();
        List<CatalogModel> GetCausasAccidentesActivas(int corp);
        List<CatalogModel> GetCausasAccidentesActivasCorporacion(int corp);
        List<CatalogModel> GetHospitalesActivos(int corp);
        List<CatalogModel> GetHospitales();
         List<CatalogModel> GetAutoridadesDisposicionActivas(int corporation);
         List<CatalogModel> GetAutoridadesDisposicion(int corporation);
        List<CatalogModel> GetAutoridadesEntregaActivas(int corporation); 
        List<CatalogModel> GetAutoridadesEntrega(int corporation); 
        List<CatalogModel> GetEntidadesActivas(int corp);
		List<CatalogModel> GetEntidadesActivasPorCorporacion(int corp);

		List<CatalogModel> GetEntidades();
        List<CatalogModel> GetMunicipiosActivosPorEntidad(int entidadDDlValue);
        public List<CatalogModel> GetMunicipiosActivosPorEntidad(int entidadDDlValue, int IdMun);

        List<CatalogModel> GetMunicipiosTodosPorEntidad(int entidadDDlValue);
        List<CatalogModel> GetCarreterasActivasPorOficina(int idOficina);
        List<CatalogModel> GetTodasCarreterasPorOficina(int idOficina);

        List<CatalogModel> GetTramosActivosPorCarretera(int carreteraDDValue);
        List<CatalogModel> GetTodoTramosPorCarretera(int carreteraDDValue);

        List<CatalogModel> GetOficialesByCorporacionActivos(int corporacion);
        List<CatalogModel> GetOficialesByCorporacion(int corporacion);

        List<CatalogModel> ObtenerAgenciasActivas(int corporation);
        List<CatalogModel> ObtenerAgencias(int corporation);
		List<CatalogModel> GetCatConceptoInfraccion(int corp);
        List<CatalogModel> GetcatSubConceptoInfraccion(int corp);
        List<CatalogModel> GetAllPuestosActivos();
        public List<AdminCatalogosModel> ObtieneListaCatalogos();
        public List<DependenciasModel> ObtieneTodasCorporaciones();


    }
}
