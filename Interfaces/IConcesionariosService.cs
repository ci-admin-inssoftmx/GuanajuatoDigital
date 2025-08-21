using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IConcesionariosService
    {
        List<ConcesionariosModel> GetAllConcesionarios();

        
        List<ConcesionariosModel> GetConcesionarios(int idOficina);
        List<ConcesionariosModel> GetConcesionariosActivos(int idOficina);
        public List<Concesionarios2Model> GetConcesionarios2ByIdDelegacion(int idDelegacion);
        public int CrearConcesionario(Concesionarios2Model model);
        public int EditarConcesionario(Concesionarios2Model model);
        public List<Concesionarios2Model> GetAllConcesionarios(int idOficina);
        public List<ConcesionariosModel> GetAllConcesionariosConMunicipio();

        public Concesionarios2Model GetConcesionarioById(int idConcesionario);
        public IEnumerable<Concesionarios2Model> GetConcecionariosBusqueda(int? idMunicipio, int idOficina, int? idDelegacion, int? idConcesionario);

    }
}
