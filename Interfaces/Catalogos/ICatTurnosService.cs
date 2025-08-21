using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models.Catalogos.Turnos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Interfaces.Catalogos
{
    public interface ICatTurnosService
    {
        Task<CatTurno> CreateAsync(CatTurno turno);
        Task<bool> DeleteAsync(long id);
        Task<List<CatTurno>> GetAllByDependenciaAsync(int dependencia);
        Task<List<CatTurno>> GetAllByDelegacionAsync(int Oficina);
		Task<List<CatTurno>> GetAllByMunicipioAsync(int municipio);
		Task<List<CatTurno>> SearchAsync(int dependencia, TurnoFilterModel filter);
        Task<CatTurno> GetByIdAsync(long id);
        Task<CatTurno> UpdateAsync(CatTurno turno);
	}
}
