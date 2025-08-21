using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IPersonasService
    {
        public IEnumerable<PersonaModel> GetAllPersonas();
        List<PersonaModel> BusquedaPersona(PersonaModel model);
        List<PersonaModel> BusquedaPersonaPagination(BusquedaPersonaModel model, Pagination pagination);
        public bool VerificarLicenciaSitteg(string numeroLicencia);
        public int InsertarDesdeServicio(LicenciaPersonaDatos personaDatos);
        public PersonaDireccionModel GetPersonaDireccionByIdPersona(int idPersona, int idInfraccion);
        public PersonaModel GetPersonaByIdInfraccion(int idPersona, int idInfraccion);
        public PersonaModel GetPersonaById(int idPersona);
		//public PersonaModel GetOpeInfraccionesPersonaById(int idPersona, int idInfraccion);

		
        PersonaModel GetPersonaByIdHistorico(int idPersona, int idinfraccion, int tipoOrigen);

		public PersonaInfraccionModel GetPersonaInfraccionById(int idPersona, int idInfraccion);
        
        List<PersonaModel> ObterPersonaPorIDList(int idPersona);

        IEnumerable<PersonaModel> GetAllPersonasMorales();
        IEnumerable<PersonaModel> GetAllPersonasFisicas();
        IEnumerable<PersonaModel> GetAllPersonasFisicasPagination(Pagination pagination);
        public List<PersonaModel> ObtenerVigencias();

        IEnumerable<PersonaModel> GetAllPersonasMorales(PersonaMoralBusquedaModel model);
        int CreatePersonaMoral(PersonaModel model);
        int UpdatePersonaMoral(PersonaModel model);
        int CreatePersonaDireccion(PersonaDireccionModel model);
        int UpdatePersonaDireccion(PersonaDireccionModel model);
        int UpdateHistoricoDireccionPersonaAccidente(int idPersona, int idAccidente);

        int UpdateConductores(Object model);
        PersonaModel GetPersonaTypeById(int idPersona);
        public PersonaModel BuscarPersonaSoloLicencia(string numeroLicencia);
        public int UpdatePersona(PersonaModel model);
        public int CreatePersona(PersonaModel model);
        int UpdateHistoricoPersonas(int personas, int infraccion);
        int UpdateHistoricoPersonasProp(int personas, int infraccion);

        public IEnumerable<PersonaModel> GetAllPersonasPagination(Pagination pagination);

        List<PersonaModel> GetPersonas();
        int UpdateConductor(PersonaModel model);

        List<PersonaModel> BuscarPersonasWithPagination(BusquedaPersonaModel model, Pagination pagination);

        public int ExistePersona(string licencia, string curp);

        public int InsertarPersonaDeLicencias(PersonaLicenciaModel personaDatos);
       public int ObtenerTotalBusquedaPersona(BusquedaPersonaModel model, Pagination pagination);
    }
}
