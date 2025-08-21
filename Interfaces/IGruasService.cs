using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IGruasService
    {
        List<GruasConcesionariosModel> GetGruasConcesionariosByIdCocesionario(int Id);
        List<TipoGruaModel> GetTipoGruas();
        List<GruasModel> GetGruas();
        public int CrearGrua(Gruas2Model model); 
        public int EditarGrua(Gruas2Model model);

        public int EliminarGrua(Gruas2Model model);
        public Gruas2Model GetGruaById(int idGrua);
        List<Gruas2Model> GetGruaByPension(int iPg);

        public IEnumerable<Gruas2Model> GetAllGruas(int idOficina);
        public IEnumerable<Gruas2Model> GetGruasByIdConcesionario(int idConcesionario);
        public IEnumerable<Gruas2Model> GetGruasToGrid(string placas, string noEconomico, int? idTipoGrua,int idOficina, int? idDelegacion, int? idConcesionario);

    }
}
