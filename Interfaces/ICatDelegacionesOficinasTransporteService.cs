using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatDelegacionesOficinasTransporteService
    {
        List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinas();
        List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinasActivos();
        List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinasFiltrado(int idDependencia);
        List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinasByDependencia(int idDependencia);
        List<CatDelegacionesOficinasTransporteModel> GetDelegacionesFiltrado(int idDependencia);
        public List<CatDelegacionesOficinasTransporteModel> GetDelegacionesDropDown();
        public string GetDelegacionOficinaById(int idOficina);
        public int EditarDelegacion(CatDelegacionesOficinasTransporteModel model);
        public List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinasTransporteFirmasByDelegacionId(int delegacionId);
        public CatDelegacionesOficinasTransporteModel GetDelegacionOficinaTransporteFirmasById(int idOficinaTransporte);
		public int InsertDelegacionesOficinasTransporteFirmas(CatDelegacionesOficinasTransporteModel model);
        public int GetDelegacionesOficinasTransporteFirmasRangoFecha(CatDelegacionesOficinasTransporteModel model);
        public int UpdateDelegacionesOficinasTransporteFirmasFechaFin(int idOficinaTransporte, DateTime fechaFin);
		public int UpdateDelegacionesOficinasTransporteFirmas(CatDelegacionesOficinasTransporteModel model);



        public bool ExistCorp(int corpId);
        public bool ExistDelegacion(int idDelegacion);
        public int NewDelegacion(int delegacion, string descripcion, int corp);
        public int NewDelegacionMun(int delegacion, string descripcion, int corp, int muni);
        public int NewCorp(int corp, string descripcion);
        public bool ExistDelegacionMun(int idDelegacion);
        int NewMunicipio(string descripcion, int corp, int idOfi);


    }
}
