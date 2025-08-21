using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Models.Catalogos.Aseguradoras;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatAseguradorasService
    {
        Task<List<CatAseguradoras>> GetAllAsync();
        IQueryable<CatAseguradoras> GetAllListAsync();
        Task<AseguradoraModel> GetByIdAsync(int id);
        Task AddAsync(AseguradoraModel aseguradoraDto);
        Task UpdateAsync(CatAseguradoras catAseguradoras, int actualizadoPor);
        Task DeleteAsync(int id);
        Task<List<CatAseguradoras>> SearchAsync(AseguradoraFilterModel filter);
    }
}
